using System;
using System.Collections.Generic;
using GameControl.StateProcessors;
using UnityEngine;


public enum InputState
{
    MOVE,
    AIM,
    START_FACILITY,
    FACILITY,
    LEAVE_FACILITY,
    NOPE,
}


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

    public StateProcessor[] StateProcessors;
    private Dictionary<InputState, List<StateProcessor>> StateProcessorMap;
    
    private AimLockTarget LockTarget;
    private InputState State;

    private void Start()
    {
        State = InputState.NOPE;
        
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

        InitStateProcessors();
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
            ProcessInput();   
        }
    }

    void ProcessInput()
    {
        InputState state = GetInputState();
        foreach (StateProcessor stateProcessor in StateProcessorMap[state])
        {
            stateProcessor.ProcessState(state);
        }
    }
    
    InputState GetInputState()
    {
        foreach (var stateProcessor in StateProcessors)
        {
            State = stateProcessor.GetInputState(State);
            if (State != InputState.NOPE)
                return State;
        }

        return InputState.NOPE;
    }

    void InitStateProcessors()
    {
        StateProcessorMap = new Dictionary<InputState, List<StateProcessor>>();
        foreach (InputState inputState in Enum.GetValues(typeof(InputState)))
        {
            StateProcessorMap[inputState] = new List<StateProcessor>();
        }

        foreach (StateProcessor stateProcessor in StateProcessors)
        {
            foreach (InputState relevantState in stateProcessor.RelevantInputStates())
            {
                StateProcessorMap[relevantState].Add(stateProcessor);   
            }
        }
    }
}