using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLockMark : MonoBehaviour
{
    private Camera cam;
    private GameObject lockedTarget;
    [SerializeField] private Material LockMat;

    private Dictionary<MeshRenderer, Material[]> lockTargetMatMap;
    private PositionTracker m_PositionTracker => GetComponent<PositionTracker>();
    void Start()
    {
        cam = Camera.main;
    }

    public void Lock(AimLockTarget target)
    {
        if (lockedTarget)
        {
            Unlock();
        }

        m_PositionTracker.target = target == null ? null : target.transform;
    }

    public void Unlock()
    {
        if (lockedTarget == null)
            return;

        lockedTarget = null;
        m_PositionTracker.target = null;
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 camFwd = cam.transform.forward;
        Vector3 direction = camFwd - Vector3.up * Utils.Projection(camFwd, Vector3.up);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
