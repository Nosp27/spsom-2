using System;
using AI.States;
using GameControl.StateMachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AI.StationAI
{
    public class StationAIStateMachine : MonoBehaviour
    {
        private StateMachine sm;
        [SerializeField] private bool hostile;

        [SerializeField] private BaseState attackState;
    
    

        private void Start()
        {
            sm = new StateMachine();
            sm.AddAnyTransition(attackState, () => true);
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
