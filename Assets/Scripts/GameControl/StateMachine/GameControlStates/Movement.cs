using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameControl.StateMachine.GameControlStates
{
    public class Movement : MonoBehaviour, IState
    {
        public AimLockTarget activeLockTarget { get; private set; }

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
            ProcessWeaponSelection();
            ValidateTargetLock();
        }

        public void OnEnter()
        {
            if (!m_GameController)
                m_GameController = GameController.Current;

            if (!m_CursorControl)
            {
                m_CursorControl = GameController.Current.GetComponentInChildren<CursorControl>();
                m_CursorControl.onCursorHoverTargetChanged.AddListener(ProcessTargetLock);
            }

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
        
        void ProcessMoveWASD(Ship playerShip, Vector3 cursor)
        {
            Vector3 direction =
                playerShip.transform.position
                + playerShip.transform.forward * Mathf.Clamp01(Input.GetAxis("Vertical")) * 100
                + playerShip.transform.right * Input.GetAxis("Horizontal") * 100;

            if (direction == Vector3.zero)
            {
                playerShip.CancelMovement();
            }
            else
            {
                playerShip.Move(direction);
            }
        }

        void ProcessAim(Ship playerShip, Vector3 cursor)
        {
            playerShip.Aim(cursor);
            if (Input.GetKey(KeyCode.Space))
            {
                playerShip.Fire(cursor);
            }
        }

        void ProcessTargetLock(GameObject newLockTarget)
        {
            AimLockTarget lt = newLockTarget ? newLockTarget.GetComponent<AimLockTarget>() : null;
            if (IsValidLockTarget(lt))
            {
                ActivateLockTarget(lt);
            }
            else
            {
                DeactivateLockTarget();
            }

            if (!IsValidLockTarget(activeLockTarget))
            {
                DeactivateLockTarget();
            }
        }

        void ValidateTargetLock()
        {
            if (!IsValidLockTarget(activeLockTarget))
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
            activeLockTarget = lt;
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
            activeLockTarget = null;

            m_PlayerShip.Track(null);
        }

        public bool Done() => true;

        void ProcessWeaponSelection()
        {
            Dictionary<KeyCode, int> nums = new Dictionary<KeyCode, int>();
            nums[KeyCode.Alpha1] = 1 - 1;
            nums[KeyCode.Alpha2] = 2 - 1;
            nums[KeyCode.Alpha3] = 3 - 1;
            nums[KeyCode.Alpha4] = 4 - 1;
            nums[KeyCode.Alpha5] = 5 - 1;
            nums[KeyCode.Alpha6] = 6 - 1;
            nums[KeyCode.Alpha7] = 7 - 1;
            nums[KeyCode.Alpha8] = 8 - 1;
            nums[KeyCode.Alpha9] = 9 - 1;

            foreach (var n in nums)
            {
                if (Input.GetKeyDown(n.Key))
                {
                    m_PlayerShip.SelectWeapon(n.Value);
                    return;
                }
            }
        }
    }
}