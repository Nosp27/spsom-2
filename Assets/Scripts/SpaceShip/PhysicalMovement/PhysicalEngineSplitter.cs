using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AI.NeuralNetwork;
using UnityEngine;
using DataRow = AI.NeuralNetwork.DataRow;
using Random = UnityEngine.Random;

namespace SpaceShip.PhysicalMovement
{
    public class PhysicalEngineSplitter : MonoBehaviour
    {
        [SerializeField] private float angularVelocityLimit = 2;
        [SerializeField] private float force = 50;
        [SerializeField] private float maxTorque = 300;

        private Rigidbody m_Rigidbody;
        private PhysicalEngine[] m_Engines;
        private NeuralNet m_NN;

        [SerializeField] private Transform checker;

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.maxAngularVelocity = angularVelocityLimit;
            m_Engines = GetComponentsInChildren<PhysicalEngine>();
            m_NN = FitNN(36_000);
        }

        private void Update()
        {
            Vector3 d = checker.position - transform.position;
            d = transform.InverseTransformDirection(d).normalized;
            double[] answer = m_NN.Compute(d.x, d.y, d.z, 0);
            Vector3 result = CalculateForce(answer);
            float useless = (float) (1 - result.magnitude / answer.Sum());
            Debug.DrawRay(transform.position, result * 10, Color.blue);
            Debug.DrawRay(transform.position + Vector3.forward * 0.1f, result.normalized * 10 * useless, Color.red);
        }

        private NeuralNet FitNN(int maxSamplesGenerated)
        {
            try
            {
                NeuralNet loaded = Utils.GameStorage.LoadData<NeuralNet>(Filename());
                if (loaded.Version == Filename())
                {
                    print($"Net load done: {loaded.Version}");
                    return loaded;
                }
            }
            catch (IOException)
            {
                print("Load failed");
            }

            print("Create net");
            int numSamples = 0;
            NeuralNet net = new NeuralNet(4, 4, m_Engines.Length, 1, .1, .8);

            List<DataRow> dataRows = new ThrottleDatasetGenerator(this, m_Engines, maxSamplesGenerated, 10).Generate();

            print($"Num samples: {numSamples}");
            net.Train(dataRows, 0.3, 100, true);
            net.Version = Checksum();
            Utils.GameStorage.SaveData(Filename(), net, true);
            return net;
        }

        public Vector3 CalculateForce(double[] engineThrottles)
        {
            Vector3 result = Vector3.zero;
            for (int i = 0; i < m_Engines.Length; i++)
            {
                PhysicalEngine engine = m_Engines[i];
                result += (float) engineThrottles[i] * engine.transform.forward;
            }

            return result;
        }

        public void ApplyDeltaV(Vector3 v)
        {
            v = transform.InverseTransformDirection(v).normalized;
            double[] throttles = m_NN.Compute(v.x, v.y, v.z, 0);

            Debug.DrawRay(transform.position, CalculateForce(throttles), Color.blue);

            for (int i = 0; i < throttles.Length; i++)
            {
                PhysicalEngine engine = m_Engines[i];
                float thr = (float) throttles[i];
                Vector3 eventualForce = engine.transform.forward * force * thr;
                m_Rigidbody.AddForceAtPosition(eventualForce, engine.transform.position);
                Debug.DrawRay(engine.transform.position, -eventualForce, Color.red);
                engine.renderer.Perform((int) (Mathf.Clamp01(thr) * 100));
            }
        }

        public Vector3 CalculateForce(Vector3 v, bool dry = true)
        {
            v = transform.InverseTransformDirection(v).normalized;
            Vector3 f = CalculateForce(m_NN.Compute(v.x, v.y, v.z, 0));
            if (!dry)
            {
                f *= force;
            }

            Debug.DrawRay(transform.position, f, Color.green);
            return f;
        }

        public void ApplyRotationTorque(Vector3 _v)
        {
            Debug.DrawRay(transform.position, _v * 10);
            Vector3 v = transform.InverseTransformDirection(_v).normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);

            if (Mathf.Abs(angle) < 0.01f)
            {
                return;
            }

            Vector3 torque = Vector3.up * angle * maxTorque / 180f;
            float degreesForStop = PredictDegreesForStop(maxTorque);
            if (Mathf.Abs(angle) <= degreesForStop)
            {
                AngularBrake();
            }
            else
            {
                m_Rigidbody.AddTorque(torque);
            }
        }

        public void AngularBrake(float maxTorqueMultiplier = 1f)
        {
            Vector3 angularVelocity = m_Rigidbody.angularVelocity;
            if (angularVelocity.magnitude > 0.01f)
            {
                m_Rigidbody.AddTorque(
                    -Vector3.up * Mathf.Sign(angularVelocity.y) * maxTorque * Mathf.Clamp01(maxTorqueMultiplier)
                );
            }
        }

        public Vector3 PredictFinalPointNoDrag(Vector3 f)
        {
            float v = m_Rigidbody.velocity.magnitude;

            if (v <= 0.1f)
                return Vector3.zero;

            float dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
            float dv = f.magnitude / m_Rigidbody.mass * dt;
            float sumn = v * v / (2f * dv);
            return transform.position + m_Rigidbody.velocity.normalized * sumn * dt;
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

        private string Checksum()
        {
            long hash = 0;
            foreach (PhysicalEngine engine in m_Engines)
            {
                hash += engine.transform.localPosition.GetHashCode() + engine.transform.localRotation.GetHashCode();
            }

            return hash.ToString();
        }
        
        private string Filename()
        {
            return $"ship-movement-weights_{Checksum()}";
        }
    }

    class ThrottleDatasetGenerator
    {
        class DataRowComparer : IComparer<DataRow>
        {
            private Func<DataRow, DataRow, int> compareFunction;

            public DataRowComparer(Func<DataRow, DataRow, int> cmp)
            {
                compareFunction = cmp;
            }

            public int Compare(DataRow x, DataRow y)
            {
                return compareFunction.Invoke(x, y);
            }
        }

        private int sizeLimit;
        private int degreeBucketSize;
        private PhysicalEngine[] engines;
        private PhysicalEngineSplitter splitter;

        public ThrottleDatasetGenerator(PhysicalEngineSplitter _splitter, PhysicalEngine[] _engines,
            int _sizeLimit = 18000, int _degreeBucketSize = 15)
        {
            splitter = _splitter;
            sizeLimit = _sizeLimit;
            degreeBucketSize = _degreeBucketSize;
            engines = _engines;
        }

        float UselessThrottle(double[] throttles)
        {
            Vector3 thr = Vector3.zero;
            int i = 0;
            foreach (PhysicalEngine engine in engines)
            {
                thr += engine.transform.forward * (float) throttles[i++];
            }

            double sumThrottle = throttles.Sum();
            return (float) (1 - thr.magnitude / sumThrottle);
        }

        public List<DataRow> Generate()
        {
            IComparer<DataRow> cpm =
                new DataRowComparer((a, b) => UselessThrottle(a.Targets).CompareTo(UselessThrottle(b.Targets)));
            Dictionary<int, SortedSet<DataRow>> byAngle = new Dictionary<int, SortedSet<DataRow>>();
            List<DataRow> dataRows = new List<DataRow>();
            for (int i = 0; i < sizeLimit; i++)
            {
                double[] targets = new double[engines.Length];
                for (int engineIdx = 0; engineIdx < engines.Length; engineIdx++)
                {
                    targets[engineIdx] = Random.value;
                }

                float uselessThrottlePart = UselessThrottle(targets);
                if (uselessThrottlePart > 0.5f)
                    continue;

                Vector3 featureVector = splitter.CalculateForce(targets);

                double[] featureValues = {featureVector.x, featureVector.y, featureVector.z, uselessThrottlePart};
                DataRow datarow = new DataRow(featureValues, targets);
                SortedSet<DataRow> dr;
                int key = (int) Vector3.SignedAngle(Vector3.forward, featureVector, Vector3.up);
                if (!byAngle.TryGetValue(key, out dr))
                {
                    dr = new SortedSet<DataRow>(cpm);
                    byAngle[key] = dr;
                }

                if (dr.Count <= degreeBucketSize)
                {
                    dr.Add(datarow);
                    dataRows.Add(datarow);
                }
                else if (cpm.Compare(datarow, dr.Max) == -1)
                {
                    dataRows.Remove(dr.Max);
                    dr.Remove(dr.Max);
                    dr.Add(datarow);
                }
            }

            return dataRows;
        }
    }
}