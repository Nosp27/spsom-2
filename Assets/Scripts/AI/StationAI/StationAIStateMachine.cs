using System;
using AI.States;
using GameControl.StateMachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AI.StationAI
{
    public class StationAIStateMachine : MonoBehaviour
    {
        [SerializeField] private bool hostile;

        [SerializeField] private BaseState attackState;
        [SerializeField] private BaseState scanState;
        
        private StateMachine sm;
        private EnemyDetector m_EnemyDetector;
    
    

        private void Start()
        {
            m_EnemyDetector = GetComponentInParent<EnemyDetector>();
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
