using System;
using GameControl.StateMachine;
using UnityEngine;

namespace AI.StationAI
{
    public class StationAttack : BaseState
    {
        private ITargetDetector _mTargetDetector;
        private Weapon[] m_Weapons = Array.Empty<Weapon>();
        private void Start()
        {
            Transform root = transform.parent.parent; // first parent is AI controller, second - the root
            
            m_Weapons = root.GetComponentsInChildren<Weapon>();
            _mTargetDetector = GetComponentInParent<ITargetDetector>();
        }

        public override void Tick()
        {
            Transform enemy = _mTargetDetector?.Target?.transform;
            if (enemy)
            {
                foreach(Weapon w in m_Weapons)
                    w.FireIfAimed();
            }
        }

        public override void OnEnter()
        {
            Transform enemy = _mTargetDetector?.Target?.transform;
            if (enemy)
            {
                foreach(Weapon w in m_Weapons)
                    w.Track(enemy);
            }
        }

        public override void OnExit()
        {
            foreach(Weapon w in m_Weapons)
                w.Track(null);
        }
    }
}
