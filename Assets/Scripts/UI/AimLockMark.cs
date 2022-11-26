using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimLockMark : MonoBehaviour
{
    private Camera cam;
    private GameObject lockedTarget;
    [SerializeField] private Material LockMat;

    private Dictionary<MeshRenderer, Material[]> lockTargetMatMap;
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
        lockedTarget = target.GetComponentInParent<Ship>().gameObject;
        transform.position = target.transform.position;
        lockTargetMatMap = new Dictionary<MeshRenderer, Material[]>();
        foreach (var lock_target_mr in lockedTarget.GetComponentsInChildren<MeshRenderer>())
        {
            Material[] materials = lock_target_mr.materials;
            lockTargetMatMap[lock_target_mr] = lock_target_mr.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = LockMat;
            }

            lock_target_mr.materials = materials;
        }
        
        transform.SetParent(target.transform);
    }

    public void Unlock()
    {
        if (lockedTarget == null)
            return;

        foreach (var lock_target_mr in lockTargetMatMap)
        {
            Material[] mats = lock_target_mr.Value;
            MeshRenderer mr = lock_target_mr.Key;
            mr.materials = mats;
        }
        
        lockedTarget = null;
        transform.SetParent(null);
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 camFwd = cam.transform.forward;
        Vector3 direction = camFwd - Vector3.up * LinUtils.Projection(camFwd, Vector3.up);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
