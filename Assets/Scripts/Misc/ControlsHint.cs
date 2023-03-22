using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsHint : MonoBehaviour
{
    [SerializeField] private GameObject forKeyboard;
    [SerializeField] private GameObject forGamepad;

    void Update()
    {
        bool gamepadAvailable = Gamepad.current != null;
        forKeyboard.SetActive(!gamepadAvailable);
        forGamepad.SetActive(gamepadAvailable);
    }
}
