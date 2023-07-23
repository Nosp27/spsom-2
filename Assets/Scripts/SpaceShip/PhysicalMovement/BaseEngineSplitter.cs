using SpaceShip.ShipServices;
using UnityEngine;

namespace SpaceShip.PhysicalMovement
{
    public abstract class BaseEngineSplitter : MonoBehaviour
    {
        public abstract void ApplyRotationTorque(Vector3 v);
        public abstract void Brake(float throttleCuttoff=1f);
        public abstract void ApplyDeltaV(Vector3 dv, float throttleCuttoff = 1f);
        public abstract void Init(Transform t, MovementConfig config);

        public abstract Vector3 PredictFinalPointNoDrag();

        public abstract void Tick();
    }
}