using System;
using GameControl.StateMachine;

public class LambdaState : IState
{
    private Action[] m_EnterActions;
    public LambdaState(params Action[] enterAction)
    {
        m_EnterActions = enterAction;
    }
    
    public void Tick()
    {
        
    }

    public void OnEnter()
    {
        if (m_EnterActions == null)
            return;
        foreach (Action enterAction in m_EnterActions)
        {
            enterAction.Invoke();
        }
    }

    public void OnExit()
    {
        
    }

    public bool Done()
    {
        return true;
    }
}
