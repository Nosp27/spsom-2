using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Ship PlayerShip => GameController.Current?.PlayerShip;
    private Vector3 BaseOffset;
    [Range(0.1f, 5)] private float Zoom = 1;

    private Vector3 CameraOffset => BaseOffset * Zoom;
    private Transform attachedListener;

    // Start is called before the first frame update
    void Start()
    {
        attachedListener = GetComponentInChildren<AudioListener>().transform;
        BaseOffset = transform.position - PlayerShip.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float m = Input.GetAxisRaw("Mouse ScrollWheel");
        Zoom += m;
        Zoom = Mathf.Clamp(Zoom, 0.1f, 5);
        transform.position = Vector3.Lerp(
            transform.position, PlayerShip.transform.position + CameraOffset, 2f * Time.unscaledDeltaTime
        );
        transform.LookAt(PlayerShip.transform);

        if (PlayerShip)
        {
            attachedListener.transform.position = PlayerShip.transform.position;
        }
    }
}