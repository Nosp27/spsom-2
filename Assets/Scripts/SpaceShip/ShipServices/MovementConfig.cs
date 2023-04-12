using UnityEngine;

namespace SpaceShip.ShipServices
{
    [CreateAssetMenu]
    public class MovementConfig : ScriptableObject
    {
        public float Power;
        public float LinearSpeed;
        public float RotationSpeed;
    }
}
