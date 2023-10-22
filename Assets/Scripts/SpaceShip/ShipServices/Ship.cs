using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using SpaceShip.ShipServices;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private MovementConfig movementConfig;
    [SerializeField] private ShipMovementService movementService;
    public float LinearSpeed => movementConfig.LinearSpeed;
    
    [Header("Weapons")]
    public List<Weapon> Weapons;
    public bool isPlayerShip;

    public Vector3 MoveAim => movementService.MoveAim;
    public ShipMovementService MovementService => movementService;
    public float currentThrottle => movementService.CurrentThrottle;
    public bool Alive => damageModel.Alive;
    public DamageModel damageModel { get; private set; }

    private Camera currentCamera;

    [SerializeField] private bool useAllWeapons = true;

    public int SelectedWeaponIndex { get; private set; }


    // Start is called before the first frame update
    IEnumerator Start()
    {
        damageModel = GetComponent<DamageModel>();
        isPlayerShip = gameObject.CompareTag("PlayerShip");

        movementService.Init(transform, movementConfig);
        
        yield return new WaitForEndOfFrame();
        ScanWeaponary();
        EventLibrary.shipSpawned.Invoke(this);
        EventLibrary.shipKills.AddListener(OnAnyShipDie);
        EventLibrary.lockTargetChanged.AddListener(OnLockTargetChanged);
    }

    private void OnAnyShipDie(Ship kills, DamageModel dies)
    {
        if (dies != damageModel)
            return;
        
        movementService.enabled = false;
        enabled = false;
        foreach (var w in Weapons)
        {
            Destroy(w.gameObject);
        }
    }

    private void OnLockTargetChanged(AimLockTarget aimLockTarget)
    {
        if (!isPlayerShip)
            return;
        Track(aimLockTarget == null ? null : aimLockTarget.transform);
    }

    public void SelectWeapon(int i)
    {
        if (i >= 0 && i < Weapons.Count)
        {
            SelectedWeaponIndex = i;
            EventLibrary.selectPlayerShipWeapon.Invoke(Weapons[SelectedWeaponIndex]);
        }
    }

    public void Fire(Vector3 cursor)
    {
        if (!Alive || Weapons.Count == 0)
            return;

        EventLibrary.shipShoots.Invoke(this);
        DoForUsedWeapon_s(w => w.Fire());
    }

    public void Aim(Vector3 cursor)
    {
        if (!Alive || Weapons.Count == 0)
            return;

        DoForUsedWeapon_s(w => w.Aim(cursor));
    }

    public void Track(Transform target)
    {
        if (!Alive || Weapons.Count == 0)
            return;

        DoForUsedWeapon_s(w => w.Track(target));
    }

    public bool Aimed()
    {
        if (!Alive || Weapons.Count == 0)
            return false;

        return CheckAnyForUsedWeapon_s(x => x.Aimed());
    }

    public void ScanWeaponary()
    {
        Weapons = new List<Weapon>(GetComponentsInChildren<Weapon>());
        EventLibrary.mutatePlayerShipWeapons.Invoke();
        SelectWeapon(SelectedWeaponIndex);
    }

    private void DoForUsedWeapon_s(Action<Weapon> func)
    {
        if (useAllWeapons)
        {
            foreach (var w in Weapons)
                func.Invoke(w);
        }
        else
        {
            func.Invoke(Weapons[SelectedWeaponIndex]);
        }
    }

    private bool CheckAnyForUsedWeapon_s(Func<Weapon, bool> func)
    {
        if (useAllWeapons)
        {
            foreach (var w in Weapons)
                if (func.Invoke(w))
                    return true;
        }
        else
        {
            return func.Invoke(Weapons[SelectedWeaponIndex]);
        }

        return false;
    }
}