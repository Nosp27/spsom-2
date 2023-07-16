using GameControl.StateMachine;
using UnityEngine;

namespace AI.StationAI
{
    public class StationAIStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseState attackState;
        [SerializeField] private BaseState scanState;
        
        private StateMachine sm;
        private ITargetDetector _mTargetDetector;
    
    

        private void Start()
        {
            _mTargetDetector = GetComponentInParent<ITargetDetector>();
            sm = new StateMachine();
            sm.AddAnyTransition(attackState, () => _mTargetDetector.Target != null);
            sm.AddAnyTransition(scanState, () => _mTargetDetector.Target == null);
        }

        private void Update()
        {
            sm.Tick();
        }

        void CreateHostile()
        {
        
        }
    }
}
