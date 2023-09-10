using GameEventSystem;
using UnityEngine;
using UnityEngine.UI;

public class Telemetry : MonoBehaviour
{
    private Ship ship;
    private Rigidbody shipRB;
    [SerializeField] private Slider velocity;
    [SerializeField] private Slider throttle;

    // Start is called before the first frame update
    void Start()
    {
        EventLibrary.switchPlayerShip.AddListener(OnShipChange);
    }

    void OnShipChange(Ship old, Ship _new)
    {
        ship = _new;
        shipRB = ship.GetComponent<Rigidbody>();

        velocity.minValue = 0;
        velocity.maxValue = ship.LinearSpeed;

        throttle.minValue = 0f;
        throttle.maxValue = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ship)
            return;

        velocity.value = shipRB.velocity.magnitude;
        throttle.value = ship.currentThrottle;
    }
}