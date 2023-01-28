using AI.States;
using UnityEngine;

public class FlyByPoints : BaseShipAIState
{
    [SerializeField] private Transform[] points;
    private int m_PointIndex = 0;
    
    public override void Tick()
    {
        if (ShipAt(points[m_PointIndex % points.Length]))
        {
            m_PointIndex++;
        }
        ShipAIControls.MoveAt(points[m_PointIndex % points.Length].position);
    }

    public override void OnExit()
    {
    }

    bool ShipAt(Transform place)
    {
        return (
                   ShipAIControls.thisShip.transform.position - place.transform.position).magnitude < 20 &&
               !ShipAIControls.IsMoving();
    }
}