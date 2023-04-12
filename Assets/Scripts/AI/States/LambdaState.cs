using System;
using AI.NeuralNetwork;
using GameControl.StateMachine;
using UnityEngine;


class LambdaState : IState
{
    private Action[] m_EnterActions;
    private Action[] m_TickActions;
    private Action[] m_ExitActions;

    public static LambdaState create => new LambdaState();

    private LambdaState()
    {
    }

    public LambdaState WithEnterActions(params Action[] actions)
    {
        m_EnterActions = actions;
        return this;
    }

    public LambdaState WithTickActions(params Action[] actions)
    {
        m_TickActions = actions;
        return this;
    }

    public LambdaState WithExitActions(params Action[] actions)
    {
        m_ExitActions = actions;
        return this;
    }

    public void Tick()
    {
        if (m_TickActions == null)
            return;
        foreach (Action a in m_TickActions)
        {
            a.Invoke();
        }
    }

    public void OnEnter()
    {
        if (m_EnterActions == null)
            return;
        foreach (Action a in m_EnterActions)
        {
            a.Invoke();
        }
    }

    public void OnExit()
    {
        if (m_ExitActions == null)
            return;
        foreach (Action a in m_ExitActions)
        {
            a.Invoke();
        }
    }

    public bool Done()
    {
        return true;
    }
}