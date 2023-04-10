using System.Collections.Generic;
using System.IO;
using System.Linq;
using AI.NeuralNetwork;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceShip.PhysicalMovement
{
    public class PhysicalEngineSplitter : MonoBehaviour
    {
        
        [SerializeField] private float angularVelocityLimit = 0.4f;
        [SerializeField] private float force = 50;
        [SerializeField] private float maxTorque = 2;
        
        private Rigidbody m_Rigidbody;
        private PhysicalEngine[] m_Engines;
        private NeuralNet m_NN;

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.maxAngularVelocity = angularVelocityLimit;
            m_Engines = GetComponentsInChildren<PhysicalEngine>();
            m_NN = FitNN(18_000);
        }

        private NeuralNet FitNN(int maxSamplesGenerated)
        {
            try
            {
                NeuralNet loaded = Utils.GameStorage.LoadData<NeuralNet>("ship-movement-weights");
                if (loaded.Version == Checksum())
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

            NeuralNet net = new NeuralNet(3, 3, m_Engines.Length, 1, .1, .8);
            Dictionary<int, List<DataRow>> byAngle = new Dictionary<int, List<DataRow>>();
            List<DataRow> dataRows = new List<DataRow>();
            for (int i = 0; i < maxSamplesGenerated; i++)
            {
                double[] targets = new double[m_Engines.Length];
                for (int engineIdx = 0; engineIdx < m_Engines.Length; engineIdx++)
                {
                    targets[engineIdx] = Random.value;
                }

                Vector3 featureVector = CalculateForce(targets);
                if (targets.Sum() / 2f > featureVector.magnitude)
                {
                    continue;
                }
                double[] featureValues = {featureVector.x, featureVector.y, featureVector.z};
                DataRow datarow = new DataRow(featureValues, targets);
                List<DataRow> dr;
                int key = (int) Vector3.SignedAngle(Vector3.forward, featureVector, Vector3.up);
                if (!byAngle.TryGetValue(key, out dr))
                {
                    dr = new List<DataRow>();
                    byAngle[key] = dr;
                }

                if (dr.Count > 15)
                    continue;

                dr.Add(datarow);
                dataRows.Add(datarow);
            }

            net.Train(dataRows, 0.3, 100, true);
            net.Version = Checksum();
            Utils.GameStorage.SaveData("ship-movement-weights", net, true);
            return net;
        }

        private Vector3 CalculateForce(double[] engineThrottles)
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
            double[] throttles = m_NN.Compute(v.x, v.y, v.z);
            
            Debug.DrawRay(transform.position, CalculateForce(throttles), Color.blue);

            for (int i = 0; i < throttles.Length; i++)
            {
                PhysicalEngine engine = m_Engines[i];
                float thr = (float) throttles[i];
                Vector3 eventualForce = engine.transform.forward * force * thr * Time.deltaTime;
                m_Rigidbody.AddForceAtPosition(eventualForce, engine.transform.position);
                Debug.DrawRay(engine.transform.position, -eventualForce, Color.red);
                engine.renderer.Perform((int)(Mathf.Clamp01(thr) * 100));
            }
        }
        
        public Vector3 CalculateForce(Vector3 v)
        {
            v = transform.InverseTransformDirection(v).normalized;
            Vector3 f = CalculateForce(m_NN.Compute(v.x, v.y, v.z));
            Debug.DrawRay(transform.position, f, Color.green);
            return f;
        }

        public void ApplyRotationTorque(Vector3 v)
        {
            v = transform.InverseTransformDirection(v).normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);
            Vector3 torque = Vector3.up * angle * Time.deltaTime * maxTorque;
            m_Rigidbody.AddTorque(torque);
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
    }
}