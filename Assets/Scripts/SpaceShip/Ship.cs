using System;
using System.Collections;
using System.Collections.Generic;
using SpaceShip.ShipServices;
using UnityEngine;
using UnityEngine.Events;

public class Ship : MonoBehaviour
{
    [SerializeField] private MovementConfig movementConfig;
    public float LinearSpeed => movementConfig.LinearSpeed;
    
    [Header("Weapons")]
    public bool HasGunnery;
    public List<Weapon> Weapons;
    public bool isPlayerShip;

    public Vector3 MoveAim => m_MovementService.MoveAim;
    public float currentThrottle => m_MovementService.CurrentThrottle;
    public bool Alive { get; private set; }

    private Camera currentCamera;

    [SerializeField] private bool useAllWeapons = true;

    public int SelectedWeaponIndex { get; private set; }
    public UnityEvent OnWeaponSelect = new UnityEvent();
    public UnityEvent OnWeaponMutate = new UnityEvent();

    private MovementService m_MovementService;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        isPlayerShip = gameObject.CompareTag("PlayerShip");
        Alive = true;
        
        m_MovementService = new MovementService(transform, movementConfig);
        
        yield return new WaitForEndOfFrame();
        ScanWeaponary();
        HasGunnery = Weapons.Count > 0;
    }

    public void SelectWeapon(int i)
    {
        if (i >= 0 && i < Weapons.Count)
        {
            SelectedWeaponIndex = i;
            OnWeaponSelect.Invoke();
        }
    }

    public void TurnOnPlace(Vector3 target)
    {
        if (!Alive)
            return;
        m_MovementService.TurnOnPlace(target);
    }

    public void Move(Vector3 target, float throttleCutoff = 1)
    {
        if (!Alive)
            return;

        m_MovementService.Move(target, throttleCutoff);
    }

    public bool IsMoving()
    {
        if (!Alive)
            return false;

        return m_MovementService.IsMoving();
    }

    public void CancelMovement()
    {
        m_MovementService.CancelMovement();
    }

    public void Fire(Vector3 cursor)
    {
        if (!Alive || Weapons.Count == 0)
            return;

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
        OnWeaponMutate.Invoke();
        SelectWeapon(SelectedWeaponIndex);
    }

    void Die()
    {
        Alive = false;
    }

    private void FixedUpdate()
    {
        if (Alive)
        {
            m_MovementService.Tick();
        }
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