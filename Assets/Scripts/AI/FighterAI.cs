using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Ship))]
[RequireComponent(typeof(ShipDamageModel))]
[RequireComponent(typeof(CollisionAvoidance))]
public class FighterAI : MonoBehaviour
{
    [SerializeField] private float AttackRange = 150;
    [SerializeField] private float ApproachRange = 70;
    [SerializeField] private float DiscoveryRange = 500;
    [SerializeField] private float LoseRange = 600;

    [SerializeField] private float MinMoveRange = 80;
    [SerializeField] private float MaxMoveRange = 400;

    [SerializeField] private String state;
    [SerializeField] private float Distance;

    [SerializeField] public Ship Enemy;
    public Ship ThisShip { get; private set; }
    private ShipDamageModel DamageModel;

    private Rigidbody rb;

    private bool DodgeToRight;

    [SerializeField] private Vector3 AnywhereMovePoint;
    private Vector3 DodgeVector = Vector3.zero;

    private bool ShouldDoSmallDodge;

    [SerializeField] private bool logged;

    private CollisionAvoidance CA;
    private Vector3 AvoidPoint;


    private void Start()
    {
        CA = GetComponent<CollisionAvoidance>();
        rb = GetComponent<Rigidbody>();
        ThisShip = GetComponent<Ship>();
        DamageModel = GetComponent<ShipDamageModel>();
        StartCoroutine(SeekEnemyShip());
        StartCoroutine(SmallDodgeSometimes());
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, ThisShip.MoveAim);
        state = "NOOP";

        if (!ThisShip.Alive)
            return;

        if (!Enemy)
        {
            state = "Move Anywhere";
            if (ThisShip.IsMoving())
                state += " (M) ";
            MoveAnywhere(MinMoveRange, MaxMoveRange, 180f);
            return;
        }

        bool mustDodge = LowHP() && EnemyInDangerDistance();
        bool canAttack = CanAttack();
        bool readyToFight = !LowHP();

        if (mustDodge)
        {
            state = "Dodge";
            DodgeEnemy();
        }
        else
        {
            if (readyToFight && NeedApproach())
            {
                state = "Approach";
                ApproachEnemy();
            }
        }

        if (canAttack && !mustDodge)
        {
            state += " & Attack";
            AttackEnemy();
            if (ShouldDoSmallDodge)
            {
                state += " & Small Dodge";
                DoSmallDodge();
            }
        }

        if (NeedChase())
        {
            state += " & Chase";
            ChaseEnemy();
        }

        if (EnemyFarAway() || (!readyToFight && !mustDodge) || !Enemy.Alive || !Enemy.gameObject.activeInHierarchy)
        {
            state = "Lose Enemy";
            Enemy = null;
        }
    }

    bool EnemyFarAway()
    {
        return EnemyLookVector().magnitude > LoseRange;
    }

    bool NeedApproach()
    {
        Vector3 lookVector = EnemyLookVector();
        return lookVector.magnitude > AttackRange || lookVector.magnitude < ApproachRange / 4;
    }

    bool LowHP()
    {
        return DamageModel.Health < DamageModel.MaxHealth / 2;
    }

    bool EnemyInDangerDistance()
    {
        return Enemy && EnemyLookVector().magnitude < AttackRange * 1.5f;
    }

    bool NeedChase()
    {
        float distance = EnemyLookVector().magnitude;
        return distance > AttackRange && distance < LoseRange && !LowHP();
    }

    Vector3 EnemyLookVector()
    {
        Distance = (Enemy.transform.position - transform.position).magnitude;
        return Enemy.transform.position - transform.position;
    }

    bool CanAttack()
    {
        if (!Enemy)
            return false;
        return EnemyLookVector().magnitude < AttackRange;
    }

    void MoveAnywhere(float minMoveRange, float maxMoveRange, float angleDeviation)
    {
        if (!ThisShip.IsMoving())
            AnywhereMovePoint = Vector3.zero;
        
        if (AnywhereMovePoint == Vector3.zero)
        {
            Vector3 moveDirection =
                (Quaternion.AngleAxis(Random.Range(-angleDeviation / 2f, angleDeviation / 2f), Vector3.up) *
                 transform.forward).normalized;
            AnywhereMovePoint = transform.position + moveDirection * Random.Range(minMoveRange, maxMoveRange);
        }
        MoveAt(AnywhereMovePoint);
    }

    void DoSmallDodge()
    {
        log("SD");
        Vector3 moveDirection =
            (Quaternion.AngleAxis(Random.Range(-30, 30), Vector3.up) * transform.forward).normalized;
        Vector3 movePoint = transform.position + moveDirection * Random.Range(15, 30);

        MoveAt(movePoint);
        ShouldDoSmallDodge = false;
    }

    void AttackEnemy()
    {
        Vector3 aimTarget =
            InterceptionCalculator.ShootingDirection(
                rb,
                Enemy.GetComponent<Rigidbody>(),
                ThisShip.Weapons[0].BulletSpeed
            );

        ThisShip.Aim(aimTarget);
        ThisShip.TurnOnPlace(aimTarget);
        if (ThisShip.Aimed(aimTarget))
            ThisShip.Shoot(aimTarget);
    }

    void ChaseEnemy()
    {
        Vector3 Target = Enemy.transform.position;
        MoveAt(Target);
    }

    void DodgeEnemy()
    {
        Vector3 dodgeVector = GetDodgeVector();
        MoveAt(dodgeVector);
        Debug.DrawLine(transform.position, dodgeVector);
    }

    Vector3 GetDodgeVector()
    {
        if (DodgeVector != Vector3.zero && (DodgeVector - transform.position).magnitude > 20)
            return DodgeVector;
        float dodgeDirectionSign = Random.value < 0.5f ? 1 : -1;

        Vector3 attackVector = transform.position - Enemy.transform.position;
        Vector3 normal = Vector3.Cross(attackVector, Vector3.up) * dodgeDirectionSign;
        DodgeVector = transform.position + normal + attackVector;
        return DodgeVector;
    }

    void ApproachEnemy()
    {
        log("Approach");
        Vector3 targetDirection = EnemyLookVector();
        targetDirection = ApproachRange * targetDirection.normalized;
        MoveAt(Enemy.transform.position - targetDirection);
    }

    IEnumerator SeekEnemyShip()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            if (Enemy)
                continue;

            Collider[] colliders = Physics.OverlapSphere(transform.position, DiscoveryRange);
            foreach (var col in colliders)
            {
                Ship ship = col.GetComponentInParent<Ship>();
                if (IsEnemy(ship))
                {
                    Enemy = ship;
                    break;
                }
            }
        }
    }

    IEnumerator SmallDodgeSometimes()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            ShouldDoSmallDodge = Random.Range(1, 100) < 40;
            yield return new WaitUntil(() => ShouldDoSmallDodge == false);
        }
    }

    public bool IsEnemy(Ship go)
    {
        return go && go.Alive && go != ThisShip && go.isPlayerShip;
    }

    void log(String text)
    {
        if (logged)
            print(text);
    }

    void MoveAt(Vector3 point)
    {
        if (point != Vector3.zero)
        {
            AvoidPoint = CA.AvoidPoint(point);
            Debug.DrawLine(transform.position, AvoidPoint, Color.yellow);
        }
        
        if (!ThisShip.IsMoving() && AvoidPoint != Vector3.zero) {
            AvoidPoint = Vector3.zero;
        }

        if (AvoidPoint == Vector3.zero)
        {
            ThisShip.Move(point);
        }
        else
        {
            ThisShip.Move(AvoidPoint);
        }
    }
}