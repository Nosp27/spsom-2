using SpaceShip.PhysicalMovement;
using SpaceShip.ShipServices;
using UnityEngine;

[ExecuteInEditMode]
public class EditorHelperEngines : MonoBehaviour
{
    [SerializeField] private MovementConfig testMC;
    [SerializeField] private Transform shipTransform;
    [SerializeField] private Physical4EngineSplitter splitter;

    void Update()
    {
        splitter.Init(shipTransform, testMC);
    }
}