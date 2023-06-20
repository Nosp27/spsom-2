using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public Transform target;
    [SerializeField] private bool captureOffset;
    private bool m_OffsetCaptured;
    private Vector3 m_Offset;

    void LateUpdate()
    {
        if (target == null)
            return;
        
        if (!m_OffsetCaptured && captureOffset)
        {
            m_Offset = transform.position - target.position;
            m_OffsetCaptured = true;
        }
        transform.position = target.position + m_Offset;
    }
}