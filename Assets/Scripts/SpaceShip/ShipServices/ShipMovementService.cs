using SpaceShip.ShipServices;
using UnityEngine;

public abstract class ShipMovementService : MonoBehaviour
{
    public new int transform;
    public abstract void Init(Transform t, MovementConfig config);
    public abstract Vector3 MoveAim { get; protected set; }
    public abstract float CurrentThrottle { get; protected set; }
    public abstract void Move(Vector3 v, float throttleCutoff);
    public abstract void TurnOnPlace(Vector3 v);
    public abstract void CancelMovement();
    public abstract void Tick();
    public abstract bool IsMoving();
}
