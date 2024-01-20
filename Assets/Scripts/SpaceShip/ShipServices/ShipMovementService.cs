using SpaceShip.ShipServices;
using UnityEngine;

public enum HEADING_MODE
{
    LOCKED_HEADING,
    FREE_HEADING,
}

public abstract class ShipMovementService : MonoBehaviour
{
    [HideInInspector] public new int transform;
    public abstract void Init(Transform t, MovementConfig config);
    public abstract Vector3 MoveAim { get; protected set; }
    public abstract float CurrentThrottle { get; protected set; }
    public abstract void MoveAtDirection(Vector3 target);
    public abstract void Move(Vector3 v);
    public abstract void ChangeHeadingMode(HEADING_MODE headingMode);
    public abstract void LimitThrottle(float limit);
    public abstract void TurnAt(Vector3 v);
    public abstract void CancelMovement(bool forceBrake=true);
    public abstract bool IsMoving();
}
