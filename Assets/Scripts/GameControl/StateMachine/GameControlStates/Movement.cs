using UnityEngine;

namespace GameControl.StateMachine.GameControlStates
{
    public class Movement : MonoBehaviour, IState
    {
        public AimLockTarget lockTarget { get; private set; }
        
        [SerializeField] private float aimLockDistance;
        [SerializeField] private GameObject moveAimPrefab;
        [SerializeField] private AimLockMark aimLockMark;

        private Ship m_PlayerShip;
        private GameObject m_MoveAim;
        private CursorControl m_CursorControl;
        private GameController m_GameController;

        public void Tick()
        {
            Vector3 cursor = m_CursorControl.Cursor();
            ProcessMove(m_PlayerShip, cursor);
            ProcessAim(m_PlayerShip, cursor);
            ProcessTargetLock();
        }

        public void OnEnter()
        {
            if (!m_GameController)
                m_GameController = GameController.Current;

            if (!m_CursorControl)
                m_CursorControl = GameController.Current.GetComponentInChildren<CursorControl>();

            if (!m_PlayerShip)
                m_PlayerShip = m_GameController.PlayerShip;

            if (!m_MoveAim)
            {
                m_MoveAim = Instantiate(moveAimPrefab);
                m_MoveAim.SetActive(false);
            }
        }

        public void OnExit()
        {
        }

        void ProcessMove(Ship playerShip, Vector3 cursor)
        {
            if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                m_MoveAim.transform.position = cursor;
                playerShip.Move(cursor);
            }
            else
            {
                playerShip.TurnOnPlace(cursor);
            }

            m_MoveAim.SetActive(playerShip.IsMoving());
        }

        void ProcessMoveW(Ship playerShip, Vector3 cursor)
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction = playerShip.transform.position + (cursor - playerShip.transform.position).normalized * 10;
            }

            if (direction == Vector3.zero)
            {
                playerShip.CancelMovement();
                playerShip.TurnOnPlace(cursor);
            }
            else
            {
                playerShip.Move(direction);
            }
        }

        void ProcessAim(Ship playerShip, Vector3 cursor)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerShip.Aim(cursor);
                if (Input.GetMouseButton(0))
                {
                    playerShip.Fire(cursor);
                }
            }
        }

        void ProcessTargetLock()
        {
            if (Input.GetMouseButtonDown(1))
            {
                AimLockTarget lt = m_CursorControl.GetLockTarget();
                if (IsValidLockTarget(lt))
                {
                    ActivateLockTarget(lt);
                }
                else
                {
                    DeactivateLockTarget();
                }
            }
            
            if (!IsValidLockTarget(lockTarget))
            {
                DeactivateLockTarget();
            }
        }

        bool IsValidLockTarget(AimLockTarget lt)
        {
            return lt != null && lt.AttachedShip.Alive &&
                   (lt.AttachedShip.transform.position - m_PlayerShip.transform.position).magnitude < aimLockDistance;
        }

        void ActivateLockTarget(AimLockTarget lt)
        {
            lockTarget = lt;
            aimLockMark.gameObject.SetActive(true);
            aimLockMark.Unlock();
            aimLockMark.Lock(lt);

            Transform ltTransform = lt == null ? null : lt.transform;
            m_PlayerShip.Track(ltTransform);
        }

        void DeactivateLockTarget()
        {
            aimLockMark.Unlock();
            aimLockMark.gameObject.SetActive(false);
            lockTarget = null;
            
            m_PlayerShip.Track(null);
        }
        
        public bool Done() => true;
    }
}