using System;
using System.Collections.Generic;
using System.Linq;
using Bonsai;
using SpaceShip.ShipServices;
using UnityEngine;

namespace SpaceShip.PhysicalMovement
{
    public class Physical4EngineSplitter : BaseEngineSplitter
    {
        // Sentinel for not using
        private new int transform;

        [SerializeField] private float angularVelocityLimit = 2;
        [SerializeField] private float force = 50;
        [SerializeField] private float maxTorque = 10f;
        [SerializeField] private float pidA = 1;
        [SerializeField] private float pidD = 1;

        public float Force => force;
        
        [SerializeField] private Transform debugStopTarget;

        private Rigidbody m_Rigidbody;
        private PhysicalEngine[] m_Engines;

        private Transform movedTransform;

        private float[] m_EngineMomentums;

        private Dictionary<Vector3, float[]> m_PushCombinations;
        private float[][] m_SpinCombinations;

        private Vector3 tickDv;
        private float tickMomentum;

        [SerializeField] private bool debug;

        public override void Init(Transform t, MovementConfig config)
        {
            movedTransform = t;
            m_Rigidbody = movedTransform.GetComponent<Rigidbody>();
            m_Rigidbody.maxAngularVelocity = angularVelocityLimit;
            m_Engines = movedTransform.GetComponentsInChildren<PhysicalEngine>();
            InitEngineCombinations();
            // force = config.Power;
            // angularVelocityLimit = config.RotationSpeed;
        }

        public void ChangeForce(float newForce)
        {
            force = newForce;
        }

        private void NormalizeThrottles(float[] throttles, float coefficient = 1f)
        {
            float max = throttles[0];
            for (int i = 1; i < throttles.Length; i++)
            {
                if (throttles[i] > max)
                    max = throttles[i];
            }

            for (int i = 0; i < throttles.Length; i++)
            {
                if (max > 0)
                    throttles[i] = (throttles[i] / max) * coefficient;
                else throttles[i] = 0;
            }
        }

        float EngineMomentum(PhysicalEngine engine)
        {
            Vector3 com = movedTransform.position;
            Vector3 distanceVector = engine.transform.position - com;
            Vector3 ortho = Vector3.Cross(distanceVector, Vector3.up).normalized;
            float projection = Utils.Projection(engine.transform.forward, ortho);

            if (debug)
            {
                Debug.DrawRay(engine.transform.position, ortho * (projection * distanceVector.magnitude), Color.blue);
                Debug.DrawRay(com, distanceVector, Color.blue);
            }

            return projection * distanceVector.magnitude;
        }

        Vector3 EngineProjection(PhysicalEngine engine)
        {
            Vector3 referenceDirection = engine.transform.forward.normalized;
            if (referenceDirection == movedTransform.forward)
            {
                return Vector3.forward;
            }

            if (referenceDirection == movedTransform.right)
            {
                return Vector3.right;
            }

            if (referenceDirection == -movedTransform.right)
            {
                return Vector3.left;
            }

            if (referenceDirection == -movedTransform.forward)
            {
                return Vector3.back;
            }

            throw new ArgumentException($"Engine {engine.name} has inappropriate direction");
        }

        public void CheckEngineCombinations()
        {
            InitEngineCombinations(true);
        }

        private void InitEngineCombinations(bool dry = false)
        {
            float[] ZeroMomentumForceForDirection(Vector3 direction)
            {
                // Irrelevant engines and those which momentum already was considered for adjustment
                HashSet<int> skips = new HashSet<int>();

                // Set max throttle for engines that point in the direction. Add to skips those who don't 
                float[] result = new float[m_Engines.Length];
                float totalMomentum = 0;
                int i = -1;
                foreach (PhysicalEngine engine in m_Engines)
                {
                    i++;
                    if (EngineProjection(engine) != direction)
                    {
                        skips.Add(i);
                        continue;
                    }

                    result[i] = 1;
                    totalMomentum += EngineMomentum(engine);
                }

                // If there is non-zero momentum - adjust it //
                while (Mathf.Abs(totalMomentum) > 0.001f && skips.Count < m_Engines.Length)
                {
                    int idx = -1;
                    float maxEffect = Single.NaN;
                    // Select engine with max momentum
                    for (i = 0; i < m_Engines.Length; i++)
                    {
                        if (skips.Contains(i))
                            continue;

                        float effect = EngineMomentum(m_Engines[i]);

                        if (idx == -1 || effect > maxEffect)
                        {
                            idx = i;
                            maxEffect = effect;
                        }
                    }

                    // Calculate needed throttle adjustment to compensate momentum
                    float momentum = EngineMomentum(m_Engines[idx]);
                    float adjustmentDv = -Mathf.Max(totalMomentum) / momentum;
                    result[idx] += adjustmentDv;
                    totalMomentum += adjustmentDv * momentum;
                    skips.Add(idx);
                }

                // Debug
                if (debug)
                {
                    float _m = 0;
                    for (int k = 0; k < m_Engines.Length; k++)
                    {
                        _m += result[k] * EngineMomentum(m_Engines[k]);
                    }
                }
                //

                if (Mathf.Abs(totalMomentum) > 0.001f)
                {
                    throw new ArgumentException(
                        $"Cannot calculate force for direction {direction}. Non zero momentum: {totalMomentum}" +
                        $"\n{string.Join("\n", m_Engines.Select(x => x.name).Zip(result, (x, y) => (x, y)).ToArray())}");
                }

                float sum = result.Sum();
                result = result.Select(x => x / sum).ToArray();
                return result;
            }

            float[] ZeroForceForMomentum(int spinDirectionIndex)
            {
                int expectedSign = spinDirectionIndex == 1 ? -1 : 1;
                float[] result = new float[m_Engines.Length];
                Vector3 totalForce = Vector3.zero;
                float totalMomentum = 0;

                // Find max momentum engines that correspond spin direction
                int spinIdx = -1;
                float maxMomentum = 0;
                for (int i = 0; i < m_Engines.Length; i++)
                {
                    float momentum = EngineMomentum(m_Engines[i]);
                    if (expectedSign != (int)Mathf.Sign(momentum))
                        continue;

                    if (spinIdx == -1 || momentum > maxMomentum)
                    {
                        spinIdx = i;
                        maxMomentum = momentum;
                    }
                }

                if (spinIdx < 0)
                {
                    throw new Exception($"No engines for momentum {expectedSign} found");
                }

                totalForce += EngineProjection(m_Engines[spinIdx]);
                totalMomentum += EngineMomentum(m_Engines[spinIdx]);
                result[spinIdx] = 1;

                int adjustIdx = -1;
                float maxAdjustMomentum = 0;
                for (int i = 0; i < m_Engines.Length; i++)
                {
                    float momentum = EngineMomentum(m_Engines[i]);
                    if (expectedSign != (int)Mathf.Sign(momentum))
                        continue;

                    if (EngineProjection(m_Engines[i]) != -totalForce)
                    {
                        continue;
                    }

                    if (debug)
                    {
                        Debug.DrawRay(
                            m_Engines[i].transform.position, -10 * m_Engines[i].transform.forward, Color.red
                        );
                    }

                    if (adjustIdx == -1 || momentum > maxAdjustMomentum)
                    {
                        adjustIdx = i;
                        maxAdjustMomentum = momentum;
                    }
                }

                if (adjustIdx < 0)
                {
                    throw new Exception($"No engines for adjustment momentum {expectedSign} found (tf: {totalForce})");
                }

                totalMomentum += EngineMomentum(m_Engines[adjustIdx]);
                totalForce += EngineProjection(m_Engines[adjustIdx]);
                result[adjustIdx] = 1;

                if (totalForce != Vector3.zero)
                    throw new ArgumentException(
                        $"Cannot balance momentum for zero force ({expectedSign}). Total remaining force: {totalForce}");

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] /= Mathf.Abs(totalMomentum);
                }

                return result;
            }

            if (dry)
            {
                foreach (Vector3 direction in new[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left })
                {
                    ZeroMomentumForceForDirection(direction);
                }

                ZeroForceForMomentum(0);
                ZeroForceForMomentum(1);
                return;
            }

            m_PushCombinations = new Dictionary<Vector3, float[]>();
            foreach (Vector3 direction in new[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left })
            {
                m_PushCombinations[direction] = ZeroMomentumForceForDirection(direction);
            }

            m_SpinCombinations = new float[2][];
            m_SpinCombinations[0] = ZeroForceForMomentum(0);
            m_SpinCombinations[1] = ZeroForceForMomentum(1);
        }

        private float[] CalculateThrottles(Vector3 dv, float momentum, float maxDvThrottle = 0.5f,
            float maxTorqueThrottle = 0.5f)
        {
            float[] totalThrottles = new float[m_Engines.Length];

            // Calculate force impact with zero momentum
            if (dv != Vector3.zero)
            {
                float sideProjection = Utils.Projection(movedTransform.right, dv);
                float sideProjectionAbs = Mathf.Abs(sideProjection);
                Vector3 sideVector = Mathf.Sign(sideProjection) > 0 ? Vector3.right : Vector3.left;

                float headProjection = Utils.Projection(movedTransform.forward, dv);
                float headProjectionAbs = Mathf.Abs(headProjection);
                Vector3 headVector = Mathf.Sign(headProjection) > 0 ? Vector3.forward : Vector3.back;


                float[] sideThrottles = m_PushCombinations[sideVector];
                float[] headThrottles = m_PushCombinations[headVector];
                for (int i = 0; i < m_Engines.Length; i++)
                {
                    totalThrottles[i] = sideThrottles[i] * sideProjectionAbs + headThrottles[i] * headProjectionAbs;
                }

                NormalizeThrottles(totalThrottles, Mathf.Min(dv.magnitude, maxDvThrottle * force));
            }

            if (momentum != 0)
            {
                float[] momentumImpact =
                    m_SpinCombinations[(int)Mathf.Sign(momentum) == 1 ? 1 : 0];

                float[] deltas = new float[m_Engines.Length];

                for (int i = 0; i < m_Engines.Length; i++)
                {
                    deltas[i] = momentumImpact[i] * Mathf.Min(Mathf.Abs(momentum), maxTorque);
                }

                for (int i = 0; i < m_Engines.Length; i++)
                {
                    totalThrottles[i] += deltas[i];
                }
            }

            return totalThrottles;
        }

        Vector3 CalculateForce(Vector3 dv, float momentum)
        {
            Vector3 f = Vector3.zero;
            float[] throttles = CalculateThrottles(dv, momentum);
            int i = 0;
            foreach (PhysicalEngine engine in m_Engines)
            {
                f += throttles[i] * engine.transform.forward;
                i++;
            }

            return f;
        }

        public override void ApplyDeltaV(Vector3 dv, float throttleCutoff = 1f)
        {
            tickDv = dv * Mathf.Clamp01(throttleCutoff);
        }

        public override void Brake(float throttleCutoff = 1f)
        {
            Vector3 v = m_Rigidbody.velocity;
            if (v.magnitude > 2)
                v = v.normalized;
            tickDv = -v * force * Mathf.Clamp01(throttleCutoff);
        }

        public override void ApplyRotationTorque(Vector3 _v)
        {
            Vector3 v = movedTransform.InverseTransformDirection(_v).normalized;
            Debug.DrawRay(movedTransform.position, v * 300, Color.blue);
            float angle = Vector3.SignedAngle(
                Vector3.forward, Vector3.ProjectOnPlane(v, Vector3.up), Vector3.up
            );
            float angularVelocity = m_Rigidbody.angularVelocity.y;
            float torque = (angle * pidA / 180f - angularVelocity * pidD);

            torque = Mathf.Clamp(torque, -maxTorque, maxTorque);
            tickMomentum = torque;
        }

        public void ApplyImpact(Vector3 dv, float momentum = 0)
        {
            float[] throttles = CalculateThrottles(dv, momentum);
            int i = 0;
            foreach (PhysicalEngine engine in m_Engines)
            {
                m_Rigidbody.AddForceAtPosition(
                    throttles[i] * engine.transform.forward, engine.transform.position
                );
                engine.renderer.Perform((int)(throttles[i] * 100 / force));
                if (debug)
                {
                    Debug.DrawRay(engine.transform.position, -engine.transform.forward * throttles[i], Color.red);
                }

                i++;
            }
        }

        public override Vector3 PredictFinalPointNoDrag()
        {
            Vector3 f = CalculateForce(-m_Rigidbody.velocity.normalized * force, 0);
            float v = m_Rigidbody.velocity.magnitude;

            if (v <= 0.1f)
                return Vector3.zero;

            float dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
            float dv = f.magnitude / m_Rigidbody.mass * dt;
            float sumn = v * v / (2f * dv);
            Vector3 result = movedTransform.position + m_Rigidbody.velocity.normalized * sumn * dt;
            if (debugStopTarget)
                debugStopTarget.position = result;
            return result;
        }

        public override void Tick()
        {
            if (debug)
            {
                Debug.DrawRay(movedTransform.position + Vector3.forward, tickDv, Color.cyan);
                Debug.DrawRay(
                    movedTransform.position + tickDv + Vector3.forward,
                    -Vector3.Cross(tickDv, Vector3.up).normalized * tickMomentum, Color.cyan
                );
            }

            ApplyImpact(tickDv, tickMomentum);
            tickDv = Vector3.zero;
            tickMomentum = 0;
        }
    }
}