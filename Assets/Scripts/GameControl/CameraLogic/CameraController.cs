using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Ship PlayerShip => GameController.Current?.PlayerShip;
    private Vector3 BaseOffset;
    [Range(0.1f, 5)] public float Zoom = 1;

    private Vector3 CameraOffset => BaseOffset * Zoom;
    private Transform attachedListener;

    [SerializeField] private bool followRotation;

    void Start()
    {
        BaseOffset = transform.position - PlayerShip.transform.position;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position, PlayerShip.transform.position + CameraOffset, 2f * Time.unscaledDeltaTime
        );
        if (followRotation)
            transform.LookAt(PlayerShip.transform);
    }
}