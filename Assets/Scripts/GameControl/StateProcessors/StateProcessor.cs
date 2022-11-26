using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameControl.StateProcessors
{
    public abstract class StateProcessor : MonoBehaviour
    {
        protected GameController gameController => GameController.Current;

        public abstract InputState GetInputState(InputState state);
        
        public abstract void ProcessState(InputState state);

        public abstract List<InputState> RelevantInputStates();
    }
}