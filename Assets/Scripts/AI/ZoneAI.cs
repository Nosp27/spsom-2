using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using Random = UnityEngine.Random;


enum AIState
{
    MOVE,
    ATTACK,
    DODGE
}

public class ZoneAI : MonoBehaviour
{
    public float Radius;

    private Ship ship;

    private Vector3 ZoneCenter;

    public Ship PlayerShip => GameController.Current?.PlayerShip;

    [SerializeField] private AIState State;

    private Coroutine moveCoro, attackCoro, dodgeCoro;

    private Rigidbody rb;

    public float MinimalTargetDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        State = AIState.MOVE;
        ship = GetComponent<Ship>();
        ZoneCenter = transform.position;
        attackCoro = StartCoroutine(Attack());
        moveCoro = StartCoroutine(Move());
        dodgeCoro = StartCoroutine(Dodge(PlayerShip));
    }

    void Update()
    {
        float distance = Vector3.Distance(PlayerShip.transform.position, transform.position);
        State = distance > 5000
            ? AIState.MOVE
            : AIState.ATTACK;
    }

    IEnumerator Move()
    {
        bool moving = false;
        float maxMovingCD = 5f;
        float movingCD = 0f;
        while (true)
        {
            movingCD -= 0.2f;
            yield return new WaitForSeconds(0.2f);

            if (State != AIState.MOVE)
            {
                if (moving)
                    ship.CancelMovement();
                yield return new WaitForSeconds(1);
                continue;
            }

            moving = true;

            if (!ship.IsMoving() || movingCD <= 0f)
            {
                movingCD = maxMovingCD;
                float azimuth = Random.value * 360;
                float distance = Mathf.Min(Random.value * Radius, MinimalTargetDistance);
                Vector3 nextTarget = (Quaternion.AngleAxis(azimuth, Vector3.up) * Vector3.forward) * distance +
                                     ZoneCenter;
                nextTarget.y = 0;
                nextTarget += ZoneCenter;
                ship.Move(nextTarget);
            }
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(0);

            if (State != AIState.ATTACK)
            {
                yield return new WaitForSeconds(0.3f);
                continue;
            }

            Vector3 targetPosition =
                InterceptionCalculator.ShootingDirection(
                    rb,
                    PlayerShip.GetComponent<Rigidbody>(),
                    ship.Weapons[0].BulletSpeed
                );
            ship.Aim(targetPosition);
            ship.TurnOnPlace(targetPosition);
            if (ship.Aimed(targetPosition))
            {
                ship.Shoot(PlayerShip.transform.position);
            }
        }
    }

    IEnumerator Dodge(Ship target)
    {
        bool moving = false;

        Vector3 dodgeVector = Vector3.zero;
        float maxDodgeVectorTTL = 3;
        float dodgeVectorTTL = 0;
        while (true)
        {
            yield return new WaitForSeconds(0);

            if (State != AIState.DODGE)
            {
                if (moving)
                {
                    ship.CancelMovement();
                    dodgeVector = Vector3.zero;
                    moving = false;
                }

                yield return new WaitForSeconds(1);
                continue;
            }

            moving = true;

            Ship[] enemies = new[] {target};

            if (dodgeVector == Vector3.zero || dodgeVectorTTL <= 0)
            {
                Vector3 attackVector = Vector3.zero;
                for (int i = 0; i < enemies.Length; i++)
                {
                    attackVector += transform.position - enemies[i].transform.position;
                }

                dodgeVector = Vector3.Cross(attackVector.normalized, Vector3.up);
                float dodgeDirectionChanger = Mathf.Sign(Random.value - 0.5f);
                dodgeVector *= dodgeDirectionChanger;
                dodgeVectorTTL = maxDodgeVectorTTL;
            }

            if (dodgeVector != Vector3.zero)
            {
                ship.Move(
                    ship.transform.position +
                    1200 * (Quaternion.AngleAxis(Random.Range(-20f, 20f), Vector3.up) * dodgeVector).normalized
                );
                dodgeVectorTTL -= 0.3f;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}