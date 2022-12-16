using UnityEngine;

namespace GameControl.StateMachine
{
    public abstract class BaseState : MonoBehaviour, IState
    {
        public abstract void Tick();

        public abstract void OnEnter();

        public abstract void OnExit();

        public bool Done()
        {
            return true;
        }
    }
}
