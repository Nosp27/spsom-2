using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class CursorControl : MonoBehaviour
{
    private float yZero;
    private Camera currentCamera;
    public GameObject cursorHoverTarget { get; private set; }
    private Vector3 _cursor;

    public UnityEvent<GameObject> onCursorHoverTargetChanged { get; private set; }

    public void Setup(Ship playerShip)
    {
        onCursorHoverTargetChanged = new UnityEvent<GameObject>();
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
            onCursorHoverTargetChanged.Invoke(cursorHoverTarget);
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

