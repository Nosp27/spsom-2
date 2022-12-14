using System;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController Current;

    public InventoryController Inventory => GetComponentInChildren<InventoryController>();
    
    private bool Alive;
    public GameObject CursorVisualizerPrefab;
    private GameObject CursorVisualizer;
    public Ship PlayerShip;
    public Vector3 cursor;
    public HealthBar healthBar;
    private CursorControl cursorControl;

    private AimLockTarget LockTarget;

    public void SwitchCursorControl(bool enabled)
    {
        cursorControl.enabled = enabled;
    }

    private void Start()
    {
        Cursor.visible = false;
        PlayerShip = GameObject.FindWithTag("PlayerShip").GetComponent<Ship>();
        healthBar.Health = healthBar.MaxHealth = PlayerShip.GetComponent<ShipDamageModel>().Health;
        if (PlayerShip == null)
            throw new Exception("Player Ship not found");

        cursorControl = GetComponentInChildren<CursorControl>();
        cursorControl.Setup(PlayerShip);

        CursorVisualizer = Instantiate(CursorVisualizerPrefab);
        Alive = true;
        if (Current == null)
        {
            Current = this;
            
        }
        else
        {
            throw new Exception("GameController tries to be duplicated");
        }
    }

    void Die()
    {
        Alive = false;
        healthBar.Health = PlayerShip.GetComponent<ShipDamageModel>().Health;
    }

    void GetDamage()
    {
        healthBar.Health = PlayerShip.GetComponent<ShipDamageModel>().Health;
    }

    void LateUpdate()
    {
        if (Alive)
        {
            cursor = cursorControl.Cursor();
            CursorVisualizer.transform.position = cursor;
        }
    }
}