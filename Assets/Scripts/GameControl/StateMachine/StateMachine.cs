using System;
using System.Collections.Generic;

namespace GameControl.StateMachine
{
    public enum AnyStatePriority
    {
        ANY_STATES_FIRST,
        ANY_STATES_LAST,
    }
    public class StateMachine
    {
        public IState CurrentState { get; private set; }
        private IState InitialState;
        private Dictionary<Type, List<Transition>> TransitionsMapping = new Dictionary<Type, List<Transition>>();
        private List<Transition> CurrentStateTransitions = new List<Transition>();
        private List<Transition> TransitionsStub = new List<Transition>();
        private AnyStatePriority anyStatePriority;

        public StateMachine(AnyStatePriority anyStatePriority=AnyStatePriority.ANY_STATES_LAST)
        {
            this.anyStatePriority = anyStatePriority;
            TransitionsMapping[typeof(AnyState)] = new List<Transition>();
        }

        public HashSet<IState> GetAllStates()
        {
            HashSet<IState> states = new HashSet<IState>();
            foreach (var transitions in TransitionsMapping.Values)
            {
                foreach (var transition in transitions)
                {
                    states.Add(transition.From);
                    states.Add(transition.To);
                }
            }

            return states;
        }

        public void AddAnyTransition(IState to, Func<bool> predicate, bool exitForSetup = true)
        {
            if (InitialState == null)
                InitialState = to;
            
            if (exitForSetup)
            {
                to.OnExit();
            }
            
            TransitionsMapping[typeof(AnyState)].Add(new Transition(this, null, to, predicate));
        }
        
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

        private void _fireTransitionList(List<Transition> transitions)
        {
            foreach (Transition transition in transitions)
            {
                if (transition.IsFiring())
                {
                    SetState(transition.To);
                    return;
                }
            }
        }

        private void MaybeFireTransition()
        {
            if (CurrentState == null)
            {
                SetState(InitialState);
                return;
            }

            if (anyStatePriority == AnyStatePriority.ANY_STATES_FIRST)
            {
                _fireTransitionList(TransitionsMapping[typeof(AnyState)]);
                _fireTransitionList(CurrentStateTransitions);
            }
            else
            {
                _fireTransitionList(CurrentStateTransitions);
                _fireTransitionList(TransitionsMapping[typeof(AnyState)]);
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

            public bool IsFiring(bool allowSelfFire=false, bool forceTracked=false)
            {
                if (From != null && !From.Done() && !forceTracked)
                {
                    return false;
                }
                return predicate() && ((fsm.CurrentState != To) || allowSelfFire) && (From == null || From == fsm.CurrentState);
            }
        }

        private class AnyState
        {
        }
    }
}
