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
    
    private bool holdTopView;

    void Start()
    {
        BaseOffset = transform.position - PlayerShip.transform.position;
    }

    void LateUpdate()
    {
        if(!holdTopView)
        {
            transform.position = Vector3.Lerp(
                transform.position, PlayerShip.transform.position + CameraOffset, 2f * Time.unscaledDeltaTime
            );
            if (followRotation)
                transform.LookAt(PlayerShip.transform);
        }
        

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            HoldTopView(!holdTopView);
        }
    }

    public void HoldTopView(bool on)
    {
        holdTopView = on;
        if (on)
        {
            Vector3 top = PlayerShip.transform.position + Vector3.up * 120 + PlayerShip.transform.right * -100;
            transform.DOMove(top, 0.5f).SetUpdate(true);
            transform.DOLookAt(transform.position + Vector3.down * 50, 0.5f, AxisConstraint.None, PlayerShip.transform.forward).SetUpdate(true);
        }
        else
        {
            transform.DOKill();
        }
    }
}