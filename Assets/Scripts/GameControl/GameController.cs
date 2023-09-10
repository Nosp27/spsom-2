using System;
using System.Collections;
using FMODUnity;
using GameEventSystem;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController Current { get; private set; }
    [SerializeField] private EventReference deathEvent;
    public InventoryController Inventory => GetComponentInChildren<InventoryController>();

    private bool Alive;
    public GameObject CursorVisualizerPrefab;
    private GameObject CursorVisualizer;
    public Ship PlayerShip { get; private set; }
    public Vector3 cursor;
    private CursorControl cursorControl;

    private AimLockTarget LockTarget;

    public void SwitchCursorControl(bool enabled)
    {
        cursorControl.enabled = enabled;
    }

    public void SwitchPlayerShip(Ship newShip)
    {
        if (newShip == null)
            throw new Exception("Try to switch player ship to null");

        PlayerShip = newShip;
        EventLibrary.switchPlayerShip.Invoke(PlayerShip, newShip);
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
        EventLibrary.objectDestroyed.AddListener(OnDieListener);
    }

    private void OnDieListener(DamageModel deadDM)
    {
        if (deadDM == PlayerShip.damageModel)
            Die();
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