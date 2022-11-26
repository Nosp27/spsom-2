using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameControl.StateProcessors
{
    public class MovementSP : StateProcessor
    {
        [SerializeField] private GameObject MoveAimPrefab;
        private GameObject MoveAim;

        private List<InputState> knownStates = new List<InputState>()
            {InputState.NOPE, InputState.MOVE, InputState.AIM};

        private CursorControl cursorControl;
        public AimLockTarget LockTarget;
        public AimLockMark aimLockMark;

        private void Start()
        {
            MoveAim = Instantiate(MoveAimPrefab);
            MoveAim.SetActive(false);
            cursorControl = GetComponentInParent<CursorControl>();
        }

        public override List<InputState> RelevantInputStates()
        {
            return new List<InputState>() {InputState.AIM, InputState.MOVE};
        }

        public override InputState GetInputState(InputState state)
        {
            if (state != InputState.AIM && state != InputState.MOVE && state != InputState.NOPE)
            {
                return InputState.NOPE;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                return InputState.AIM;
            }
            else
            {
                return InputState.MOVE;
            }
        }

        public override void ProcessState(InputState state)
        {
            Ship playerShip = gameController.PlayerShip;
            Vector3 cursor = cursorControl.Cursor();

            if (!playerShip.Alive)
                return;

            switch (state)
            {
                case InputState.MOVE:
                case InputState.AIM:
                    ProcessMove(playerShip, cursor);
                    ProcessAim(playerShip, cursor);
                    break;
            }
        }

        void ProcessMove(Ship playerShip, Vector3 cursor)
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                MoveAim.transform.position = cursor;
                playerShip.Move(cursor);
            }
            else
            {
                playerShip.TurnOnPlace(cursor);
            }

            MoveAim.SetActive(playerShip.IsMoving());
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
            playerShip.Aim(cursor);
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift))
            {
                print("Shoot");
                playerShip.Shoot(cursor);
            }

            if (Input.GetMouseButtonDown(1))
            {
                AimLockTarget lt = cursorControl.GetLockTarget();
                if (lt != null)
                {
                    LockTarget = lt;
                    aimLockMark.gameObject.SetActive(true);
                    aimLockMark.Unlock();
                    aimLockMark.Lock(lt);
                }
                else
                {
                    aimLockMark.Unlock();
                    aimLockMark.gameObject.SetActive(false);
                    LockTarget = null;
                }
            }
        }
    }
}