using AI;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CameraController))]
public class CameraZoomControl : MonoBehaviour
{
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.Linear(
        30, 1f, 400, 5
    );

    [SerializeField] private float autoZoomFreezeTimeout = 3;

    private Ship m_PlayerShip;
    private EnemyDetector _playerEnemyDetector;
    private CameraController cam;
    
    private float autoZoomFreeze;

    void Start()
    {
        cam = GetComponent<CameraController>();
        m_PlayerShip = GameController.Current.PlayerShip;
        _playerEnemyDetector = m_PlayerShip.GetComponent<EnemyDetector>();
    }

    void Update()
    {
        ProcessManualZoom();
        ProcessAutoZoom();
    }

    void ProcessManualZoom()
    {
        float m = Mouse.current.scroll.ReadValue().y;
        if (m == 0)
            return;
        float zoom = cam.Zoom;
        zoom += m;
        zoom = Mathf.Clamp(zoom, 0.1f, 5);
        cam.Zoom = zoom;
        autoZoomFreeze = autoZoomFreezeTimeout;
    }

    void ProcessAutoZoom()
    {
        if (autoZoomFreeze > 0)
        {
            autoZoomFreeze -= Time.deltaTime;
            return;
        }

        float newZoom = Mathf.Max(AutoZoomForMovementDistance(), AutoZoomForEnemies());
        if (newZoom > 0.05)
            cam.Zoom = newZoom;
    }

    float AutoZoomForMovementDistance()
    {
        if (m_PlayerShip.MoveAim != Vector3.zero)
            return zoomCurve.Evaluate((m_PlayerShip.MoveAim - m_PlayerShip.transform.position).magnitude);
        return 0;
    }

    float AutoZoomForEnemies()
    {
        if (_playerEnemyDetector.Target != null)
            return zoomCurve.Evaluate((_playerEnemyDetector.Target.transform.position - m_PlayerShip.transform.position)
                .magnitude);
        return 0;
    }
}