using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class CursorControl : MonoBehaviour
{
    private float yZero;
    private Camera currentCamera;
    private GameObject cursorHoverTarget;
    private Vector3 _cursor;

    public UnityEvent<GameObject> OnCursorHoverTargetChanged;

    public void Setup(Ship playerShip)
    {
        currentCamera = Camera.main;
        yZero = playerShip.transform.position.y;
    }

    private void Update()
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = LayerMask.GetMask("UI");
        RaycastHit[] hits = Physics.RaycastAll(ray, 2000f, mask);
        Vector3 point = Vector3.zero;
        GameObject newCursorHoverTarget = null;
        foreach(RaycastHit h in hits)
        {
            if (h.collider.gameObject.name == "ZeroPlane")
            {
                point = h.point;
                point.y = yZero;
            }
            else
            {
                newCursorHoverTarget = h.collider.gameObject;
            }
        }

        if (newCursorHoverTarget != cursorHoverTarget)
        {
            cursorHoverTarget = newCursorHoverTarget;
            OnCursorHoverTargetChanged.Invoke(cursorHoverTarget);
        }
        _cursor = point;
    }

    public Vector3 Cursor()
    {
        return _cursor;
    }

    [CanBeNull]
    public AimLockTarget GetLockTarget()
    {
        return cursorHoverTarget?.GetComponent<AimLockTarget>();
    }
    
    [CanBeNull]
    public FacilityGUI GetHoveredFacility()
    {
        return cursorHoverTarget?.GetComponentInParent<FacilityGUI>();
    }
}

