using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceItems.ProceduralStations
{
    class StationModuleIntersection : Exception
    {
    }

    public class StationGenerator : MonoBehaviour
    {
        [SerializeField] private Transform modulesCollection;
        [SerializeField] private int numGenerations;
        [SerializeField] private float moduleSpacing = 5f;
        [SerializeField] private uint seed;

        private int numModules => stationModules.Count;
        private Dictionary<StationModuleTag, List<StationModule>> stationModules;
        private Transform stationRoot;
        private Unity.Mathematics.Random random;

        // Start is called before the first frame update
        private void Start()
        {
            random = new Unity.Mathematics.Random(seed);
            CreateStationRoot();
            LocateStationModules();
            Generate(numGenerations);
        }

        private void LocateStationModules()
        {
            StationModule[] allStationModules = modulesCollection.GetComponentsInChildren<StationModule>(true);
            stationModules = new Dictionary<StationModuleTag, List<StationModule>>();
            foreach (StationModule module in allStationModules)
            {
                foreach (var tag in module.tags)
                {
                    if (!stationModules.ContainsKey(tag))
                    {
                        stationModules[tag] = new List<StationModule>();
                    }

                    stationModules[tag].Add(module);
                }
            }
        }

        private void CreateStationRoot()
        {
            stationRoot = new GameObject("StationRoot").transform;
            stationRoot.position = transform.position;
            stationRoot.rotation = transform.rotation;
        }

        private StationModule PickModule(StationModuleTag moduleTag)
        {
            List<StationModule> modules = stationModules[moduleTag];
            
            int idx = random.NextInt(0, modules.Count);
            StationModule reference = modules[idx];
            return reference;
        }

        private StationModule AddModule(EngagePoint engagePoint, StationModuleTag moduleTag)
        {
            StationModule reference = PickModule(moduleTag);
            reference.gameObject.SetActive(true);
            StationModule instance =
                Instantiate(reference, stationRoot).GetComponent<StationModule>();
            reference.gameObject.SetActive(false);
            instance.gameObject.name += random.NextInt(0, 100).ToString();
            
            instance.Init();
            if (engagePoint != null)
            {
                instance.transform.position = engagePoint.transform.position;
                instance.transform.rotation = engagePoint.transform.rotation;
                instance.Engage(engagePoint.attachedModule);
            }
            else
            {
                instance.transform.position = stationRoot.position;
                instance.transform.rotation = stationRoot.rotation;
            }

            Physics.SyncTransforms();
            if (instance.Collides(moduleSpacing))
            {
                Destroy(instance.gameObject);
                throw new StationModuleIntersection();
            }

            return instance;
        }

        private StationModule AddBridgedModule(EngagePoint engagePoint, int numBridges)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            for (int i = 0; i < numBridges; i++)
            {
                StationModule nextBridge = AddModule(engagePoint, StationModuleTag.BRIDGE);
                engagePoint = nextBridge.engagePoints[0];
                gameObjects.Add(nextBridge.gameObject);
            }

            try
            {
                return AddModule(engagePoint, StationModuleTag.LARGE_MODULE);
            }
            catch (StationModuleIntersection)
            {
                foreach (GameObject go in gameObjects)
                {
                    Destroy(go);
                }

                throw;
            }
        }

        public void Generate(int generations)
        {
            StationModule rootModule = AddModule(null, StationModuleTag.LARGE_MODULE);
            HashSet<EngagePoint> availableEngagePoints = new HashSet<EngagePoint>(rootModule.engagePoints);

            for (int i = 0; i < generations; i++)
            {
                EngagePoint[] engagePoints = availableEngagePoints.ToArray();
                foreach (EngagePoint point in engagePoints)
                {
                    if (availableEngagePoints.Count == 0)
                    {
                        return;
                    }

                    try
                    {
                        StationModule newModule = AddBridgedModule(point, 1);
                        availableEngagePoints.UnionWith(newModule.engagePoints);
                    }
                    catch (StationModuleIntersection)
                    {
                        
                    }

                    availableEngagePoints.Remove(point);
                }
            }
        }
    }
}