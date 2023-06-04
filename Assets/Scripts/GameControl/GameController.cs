using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class GameController : MonoBehaviour
{
    public static GameController Current { get; private set; }
    [SerializeField] private EventReference deathEvent;
    public InventoryController Inventory => GetComponentInChildren<InventoryController>();
    public CursorControl CurrentCursorControl => cursorControl;

    private bool Alive;
    public GameObject CursorVisualizerPrefab;
    private GameObject CursorVisualizer;
    public Ship PlayerShip { get; private set; }
    public Vector3 cursor;
    public HealthBarCircleLine healthBarCircleLine;
    private CursorControl cursorControl;

    private AimLockTarget LockTarget;

    public UnityEvent<Ship, Ship> OnShipChange { get; private set; }

    private void Awake()
    {
        OnShipChange = new UnityEvent<Ship, Ship>();
    }

    public void SwitchCursorControl(bool enabled)
    {
        cursorControl.enabled = enabled;
    }

    public void SwitchPlayerShip(Ship newShip)
    {
        if (newShip == null)
            throw new Exception("Try to switch player ship to null");
        
        if (PlayerShip != null)
        {
            PlayerShip.damageModel.OnDamage.RemoveListener(SyncDamage);
            PlayerShip.damageModel.OnDie.RemoveListener(Die);
        }
        OnShipChange.Invoke(PlayerShip, newShip);
        PlayerShip = newShip;
        healthBarCircleLine.MaxHealth = PlayerShip.GetComponent<ShipDamageModel>().MaxHealth;
        healthBarCircleLine.Health = PlayerShip.GetComponent<ShipDamageModel>().Health;
        PlayerShip.damageModel.OnDamage.AddListener(SyncDamage);
        PlayerShip.damageModel.OnDie.AddListener(Die);
    }

    private IEnumerator Start()
    {
        Cursor.visible = false;
        cursorControl = GetComponentInChildren<CursorControl>();
        cursorControl.Setup();
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

        yield return 0;
        LevelInit();
    }

    void LevelInit()
    {
        var firstPlayerShip = GameObject.FindWithTag("PlayerShip").GetComponent<Ship>();
        if (firstPlayerShip == null)
            throw new Exception("Player Ship not found");
        SwitchPlayerShip(firstPlayerShip);
    }

    void Die()
    {
        if (Alive)
            RuntimeManager.PlayOneShot(deathEvent);
        Alive = false;
        healthBarCircleLine.Health = PlayerShip.GetComponent<ShipDamageModel>().Health;
    }

    void SyncDamage(BulletHitDTO hit)
    {
        healthBarCircleLine.Health = PlayerShip.GetComponent<ShipDamageModel>().Health;
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