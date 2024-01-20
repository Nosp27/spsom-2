using System;
using UnityEngine;

namespace GameControl.StateMachine
{
    public class GameController : MonoBehaviour
    {
        public static GameController Current;

        public Ship PlayerShip { get; private set; }
        [SerializeField] private Ship _playerShip;

        private void Awake()
        {
            if (Current != null)
            {
                throw new Exception("Game object failed standalone constraint");
            }
            Current = this;
            
            PlayerShip = _playerShip;
        }
    }
}