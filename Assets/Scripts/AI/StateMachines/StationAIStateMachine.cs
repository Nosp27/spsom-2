using GameControl.StateMachine;
using UnityEngine;

namespace AI.StationAI
{
    public class StationAIStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseState attackState;
        [SerializeField] private BaseState scanState;
        
        private StateMachine sm;
        private IEnemyDetector m_EnemyDetector;
    
    

        private void Start()
        {
            m_EnemyDetector = GetComponentInParent<IEnemyDetector>();
            sm = new StateMachine();
            sm.AddAnyTransition(attackState, () => m_EnemyDetector.Enemy != null);
            sm.AddAnyTransition(scanState, () => m_EnemyDetector.Enemy == null);
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
