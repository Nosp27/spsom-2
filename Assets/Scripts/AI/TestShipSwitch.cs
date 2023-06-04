using UnityEngine;
using UnityEngine.InputSystem;

public class TestShipSwitch : MonoBehaviour
{
    [SerializeField] private Ship[] pool;
    
    public void SwitchShip()
    {
        foreach (Ship ship in pool)
        {
            if (GameController.Current.PlayerShip == ship)
            {
                continue;
            }
            GameController.Current.SwitchPlayerShip(ship);
            break;
        }
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            SwitchShip();
        }
    }
}
