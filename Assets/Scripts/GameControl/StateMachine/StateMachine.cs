using System;
using System.Collections.Generic;

namespace GameControl.StateMachine
{
    public class StateMachine
    {
        private IState CurrentState;
        private IState InitialState;
        private Dictionary<Type, List<Transition>> TransitionsMapping = new Dictionary<Type, List<Transition>>();
        private List<Transition> CurrentStateTransitions = new List<Transition>();
        private List<Transition> TransitionsStub = new List<Transition>();

        public void AddTransition(IState from, IState to, Func<bool> predicate, bool exitForSetup = true)
        {
            if (InitialState == null)
                InitialState = from;
            
            if (exitForSetup)
            {
                from.OnExit();
                to.OnExit();
            }
            
            Type stateType = from.GetType();
            if (!TransitionsMapping.ContainsKey(stateType))
                TransitionsMapping[stateType] = new List<Transition>();
            TransitionsMapping[stateType].Add(new Transition(this, from, to, predicate));
        }

        public void Tick()
        {
            MaybeFireTransition();
            CurrentState?.Tick();
        }

        private void SetState(IState state)
        {
            if (state == null)
                return;
            
            if (CurrentState != null)
                CurrentState.OnExit();
            
            CurrentState = state;
            CurrentState.OnEnter();
            
            Type currentStateType = CurrentState.GetType();
            List<Transition> possibleTransitions;
            if (TransitionsMapping.TryGetValue(currentStateType, out possibleTransitions))
                CurrentStateTransitions = possibleTransitions;
            else
                CurrentStateTransitions = TransitionsStub;
        }

        private void MaybeFireTransition()
        {
            if (CurrentState == null)
            {
                SetState(InitialState);
                return;
            }
            
            foreach (Transition transition in CurrentStateTransitions)
            {
                if (transition.IsFiring())
                    SetState(transition.To);
            }
        }
        
        
        private class Transition
        {
            private Func<bool> predicate;
            public IState From { get; }
            public IState To { get; }
            private StateMachine fsm;

            public Transition(StateMachine fsm, IState from, IState to, Func<bool> predicate)
            {
                this.fsm = fsm;
                this.From = from;
                this.To = to;
                this.predicate = predicate;
            }

            public bool IsFiring()
            {
                return predicate() && (fsm.CurrentState != To);
            }
        }   
    }
}
