using GameControl.StateMachine;
using UnityEngine;

namespace AI.StationAI
{
    public class Attack : BaseState
    {
        private EnemyDetector m_EnemyDetector;
        private Weapon[] m_Weapons;
        private void Start()
        {
            m_Weapons = GetComponentsInChildren<Weapon>();
            m_EnemyDetector = GetComponentInParent<EnemyDetector>();
        }

        public override void Tick()
        {
            Transform enemy = m_EnemyDetector?.Enemy?.transform;
            if (enemy)
            {
                foreach(Weapon w in m_Weapons)
                    w.FireIfAimed();
            }
        }

        public override void OnEnter()
        {
            Transform enemy = m_EnemyDetector?.Enemy?.transform;
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
