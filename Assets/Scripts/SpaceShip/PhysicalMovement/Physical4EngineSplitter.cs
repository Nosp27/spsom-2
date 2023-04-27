using System;
using System.Collections.Generic;
using System.Linq;
using SpaceShip.ShipServices;
using UnityEngine;

namespace SpaceShip.PhysicalMovement
{
    public class Physical4EngineSplitter : MonoBehaviour
    {
        // Sentinel for not using
        private new int transform;

        [SerializeField] private float angularVelocityLimit = 2;
        [SerializeField] private float force = 50;
        [SerializeField] private float maxTorque = 10f;

        private Rigidbody m_Rigidbody;
        private PhysicalEngine[] m_Engines;

        private Transform movedTransform;

        private float[] m_EngineMomentums;

        private Dictionary<Vector3, float[]> m_PushCombinations;
        private float[][] m_SpinCombinations;

        private Vector3 tickDv;
        private float tickMomentum;

        public void Init(Transform t, MovementConfig config)
        {
            movedTransform = t;
            m_Rigidbody = movedTransform.GetComponent<Rigidbody>();
            m_Rigidbody.maxAngularVelocity = angularVelocityLimit;
            m_Engines = movedTransform.GetComponentsInChildren<PhysicalEngine>();
            InitEngineCombinations();
            // force = config.Power;
            // angularVelocityLimit = config.RotationSpeed;
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
            Vector3 com = movedTransform.TransformPoint(m_Rigidbody.centerOfMass);
            Vector3 distanceVector = engine.transform.position - com;
            Vector3 ortho = Vector3.Cross(distanceVector, Vector3.up).normalized;
            float projection = Utils.Projection(engine.transform.forward, ortho);
            Debug.DrawRay(engine.transform.position, ortho * projection * distanceVector.magnitude, Color.blue);
            Debug.DrawRay(com, distanceVector, Color.blue);
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

        private void InitEngineCombinations()
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
                float _m = 0;
                for (int k = 0; k < m_Engines.Length; k++)
                {
                    _m += result[k] * EngineMomentum(m_Engines[k]);
                }

                print(
                    $"{direction}\nM: {_m} VS expected {totalMomentum}\n{string.Join("\n", m_Engines.Zip(result, (x, y) => (x.name, y, EngineMomentum(x))).ToArray())}");
                //

                if (Mathf.Abs(totalMomentum) > 0.001f)
                {
                    throw new ArgumentException(
                        $"Cannot calculate force for direction {direction}. Non zero momentum: {totalMomentum}" +
                        $"\n{string.Join("\n", m_Engines.Select(x => x.name).Zip(result, (x, y) => (x, y)).ToArray())}");
                }

                NormalizeThrottles(result);
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
                    if (expectedSign != (int) Mathf.Sign(momentum))
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
                    if (expectedSign != (int) Mathf.Sign(momentum))
                        continue;

                    if (EngineProjection(m_Engines[i]) != -totalForce)
                    {
                        Debug.DrawRay(m_Engines[i].transform.position, -10 * m_Engines[i].transform.forward, Color.red);
                        continue;
                    }

                    Debug.DrawRay(m_Engines[i].transform.position, -10 * m_Engines[i].transform.forward, Color.green);

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

            m_PushCombinations = new Dictionary<Vector3, float[]>();
            foreach (Vector3 direction in new[] {Vector3.forward, Vector3.right, Vector3.back, Vector3.left})
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
                NormalizeThrottles(totalThrottles, maxDvThrottle);
            }

            if (momentum != 0)
            {
                float[] momentumImpact =
                    m_SpinCombinations[(int) Mathf.Sign(momentum) == 1 ? 1 : 0];

                float[] deltas = new float[m_Engines.Length];

                for (int i = 0; i < m_Engines.Length; i++)
                {
                    deltas[i] = momentumImpact[i] * Mathf.Min(Mathf.Abs(momentum), maxTorque);
                }
                NormalizeThrottles(deltas, maxTorqueThrottle);
                
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
                f += throttles[i] * engine.transform.forward * force;
                i++;
            }

            return f;
        }

        public void ApplyDeltaV(Vector3 dv)
        {
            tickDv = dv;
        }

        public void ApplyRotationTorque(Vector3 _v)
        {
            Vector3 v = movedTransform.InverseTransformDirection(_v).normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);

            if (Mathf.Abs(angle) < 0.01f)
            {
                return;
            }

            float torque = angle * maxTorque / 180f;
            float degreesForStop = PredictDegreesForStop(maxTorque);
            if (Mathf.Abs(angle) <= degreesForStop)
            {
                tickMomentum = AngularBrakeTorque();
            }
            else
            {
                tickMomentum = torque;
            }
        }

        public void ApplyImpact(Vector3 dv, float momentum = 0)
        {
            float[] throttles = CalculateThrottles(dv, momentum);
            int i = 0;
            foreach (PhysicalEngine engine in m_Engines)
            {
                m_Rigidbody.AddForceAtPosition(
                    throttles[i] * engine.transform.forward * force, engine.transform.position
                );
                engine.renderer.Perform((int) (throttles[i] * 100));
                Debug.DrawRay(engine.transform.position, -engine.transform.forward * throttles[i] * force, Color.red);
                i++;
            }
        }

        public void AngularBrake(float limit)
        {
            tickMomentum = AngularBrakeTorque(limit);
        }

        private float AngularBrakeTorque(float limit = 1)
        {
            Vector3 angularVelocity = m_Rigidbody.angularVelocity;
            if (angularVelocity.magnitude > 0.01f)
            {
                return -Mathf.Sign(angularVelocity.y) * maxTorque * Mathf.Min(1, limit);
            }

            return 0;
        }

        public Vector3 PredictFinalPointNoDrag()
        {
            Vector3 f = CalculateForce(-m_Rigidbody.velocity, 0);
            float v = m_Rigidbody.velocity.magnitude;

            if (v <= 0.1f)
                return Vector3.zero;

            float dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
            float dv = f.magnitude / m_Rigidbody.mass * dt;
            float sumn = v * v / (2f * dv);
            return movedTransform.position + m_Rigidbody.velocity.normalized * sumn * dt;
        }

        public float PredictDegreesForStop(float brakingTorque)
        {
            float av = m_Rigidbody.angularVelocity.magnitude;
            if (av < 0.01f)
                return 0;
            float dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
            float dav = brakingTorque / m_Rigidbody.mass * dt;
            return Mathf.Rad2Deg * av * av / (2f * dav);
        }

        public void Tick()
        {
            ApplyImpact(tickDv, tickMomentum);
            tickDv = Vector3.zero;
            tickMomentum = 0;
        }
    }
}