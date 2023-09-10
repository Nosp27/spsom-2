using GameEventSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CursorControl : MonoBehaviour
{
    private float yZero;
    private Camera currentCamera;
    public GameObject cursorHoverTarget { get; private set; }
    private Vector3 _cursor;
    
    public void Setup()
    {
        currentCamera = Camera.main;
        yZero = 0;
    }

    private void Update()
    {
        Ray ray = currentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
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
            EventLibrary.cursorHoverTargetChanged.Invoke(cursorHoverTarget);
        }
        _cursor = point;
    }

    public Vector3 Cursor()
    {
        return _cursor;
    }

    [CanBeNull]
    public FacilityGUI GetHoveredFacility()
    {
        if (!cursorHoverTarget)
            return null;
        return cursorHoverTarget.GetComponentInParent<FacilityGUI>();
    }
}

