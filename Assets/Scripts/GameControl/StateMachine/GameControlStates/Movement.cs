using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

        Mouse mouse => Mouse.current;
        Gamepad gamepad => Gamepad.current;
        Keyboard keyboard => Keyboard.current;

        private void Start()
        {
            m_GameController = GameController.Current;
            EventLibrary.switchPlayerShip.AddListener(OnPlayerShipChanged);
        }

        void OnPlayerShipChanged(Ship old, Ship _new)
        {
            m_PlayerShip = _new;
        }

        public void Tick()
        {
            if (m_PlayerShip == null)
                return;

            Vector3 cursor = m_CursorControl.Cursor();
            if (gamepad != null)
            {
                ProcessMoveGamepad(m_PlayerShip, cursor);
                ProcessAimGamepad(m_PlayerShip, cursor);
            }
            else
            {
                ProcessMove(m_PlayerShip, cursor);
                ProcessAim(m_PlayerShip, cursor);
            }

            ProcessWeaponSelection();
            ValidateTargetLock();
        }

        public void OnEnter()
        {
            if (!m_CursorControl)
            {
                m_CursorControl = GameController.Current.GetComponentInChildren<CursorControl>();
                EventLibrary.cursorHoverTargetChanged.AddListener(ProcessTargetLock);
            }

            if (!m_MoveAim && moveAimPrefab)
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
            float directMultiplier = 0;
            float sideMultiplier = 0;
            if (Keyboard.current.wKey.isPressed)
            {
                directMultiplier = 1;
            }
            else if (Keyboard.current.sKey.isPressed)
            {
                directMultiplier = -1;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                sideMultiplier = -1;
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                sideMultiplier = 1;
            }

            Vector3 way = cursor - playerShip.transform.position;
            Vector3 orthoWay = -Vector3.Cross(way, Vector3.up);
            Vector3 delta = (way * directMultiplier + orthoWay * sideMultiplier).normalized * way.magnitude;
            Vector3 movementTarget = playerShip.transform.position + delta;

            bool moveCommand = directMultiplier != 0 || sideMultiplier != 0;
            if (moveCommand)
            {
                if (m_MoveAim)
                    m_MoveAim.transform.position = movementTarget;
                playerShip.MovementService.ChangeHeadingMode(HEADING_MODE.FREE_HEADING);
                playerShip.MovementService.LimitThrottle(1);
                playerShip.MovementService.MoveAtDirection(movementTarget);
            }
            else
            {
                playerShip.MovementService.CancelMovement();
            }

            playerShip.MovementService.TurnAt(cursor);

            if (m_MoveAim)
                m_MoveAim.SetActive(playerShip.MovementService.IsMoving());
        }

        void ProcessMoveGamepad(Ship playerShip, Vector3 cursor)
        {
            if (gamepad == null)
                return;
            Vector2 moveVectorRaw = gamepad.leftStick.ReadValue();

            if (moveVectorRaw == Vector2.zero)
                return;

            Vector3 moveVector = (Vector3.forward * moveVectorRaw.y + Vector3.right * moveVectorRaw.x) * 100;
            playerShip.MovementService.MoveAtDirection(playerShip.transform.position + moveVector);
        }

        void ProcessAim(Ship playerShip, Vector3 cursor)
        {
            playerShip.Aim(cursor);
            if (mouse.leftButton.isPressed)
            {
                playerShip.Fire(cursor);
            }
        }

        void ProcessAimGamepad(Ship playerShip, Vector3 cursor)
        {
            Vector2 rightStick = gamepad.rightStick.ReadValue();

            if (rightStick != Vector2.zero)
            {
                Vector3 aimVector = (Vector3.forward * rightStick.y + Vector3.right * rightStick.x) * 100;
                playerShip.Aim(playerShip.transform.position + aimVector);
            }

            if (gamepad.rightTrigger.isPressed)
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
            return lt != null && lt.AttachedShip != null && lt.AttachedShip.Alive &&
                   (lt.AttachedShip.transform.position - m_PlayerShip.transform.position).magnitude < aimLockDistance;
        }

        void ActivateLockTarget(AimLockTarget lt)
        {
            activeLockTarget = lt;
            aimLockMark.gameObject.SetActive(true);
            aimLockMark.Unlock();
            aimLockMark.Lock(lt);
            EventLibrary.lockTargetChanged.Invoke(lt);
        }

        void DeactivateLockTarget()
        {
            aimLockMark.Unlock();
            aimLockMark.gameObject.SetActive(false);
            activeLockTarget = null;

            EventLibrary.lockTargetChanged.Invoke(null);
        }

        public bool Done() => true;

        void ProcessWeaponSelection()
        {
            Dictionary<KeyControl, int> nums = new Dictionary<KeyControl, int>();
            nums[keyboard.digit1Key] = 1 - 1;
            nums[keyboard.digit2Key] = 2 - 1;
            nums[keyboard.digit3Key] = 3 - 1;
            nums[keyboard.digit4Key] = 4 - 1;
            nums[keyboard.digit5Key] = 5 - 1;
            nums[keyboard.digit6Key] = 6 - 1;
            nums[keyboard.digit7Key] = 7 - 1;
            nums[keyboard.digit8Key] = 8 - 1;
            nums[keyboard.digit9Key] = 9 - 1;

            foreach (var n in nums)
            {
                if (n.Key.isPressed)
                {
                    m_PlayerShip.SelectWeapon(n.Value);
                    return;
                }
            }

            if (gamepad != null)
            {
                if (gamepad.rightShoulder.wasPressedThisFrame)
                {
                    m_PlayerShip.SelectWeapon(m_PlayerShip.SelectedWeaponIndex + 1);
                }

                if (gamepad.leftShoulder.wasPressedThisFrame)
                {
                    m_PlayerShip.SelectWeapon(m_PlayerShip.SelectedWeaponIndex - 1);
                }
            }
        }
    }
}