using System;
using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;

public class WeaponUIController : MonoBehaviour
{
    private Ship trackedShip;
    [SerializeField] private GameObject slotPrefab;
    private List<WeaponUISlot> weapons;

    private int highlightedWeapon;


    private void Start()
    {
        EventLibrary.switchPlayerShip.AddListener(OnPlayerShipChanged);
        EventLibrary.selectPlayerShipWeapon.AddListener(OnWeaponChanged);
        EventLibrary.mutatePlayerShipWeapons.AddListener(SnapshotWeapons);
    }

    private void OnPlayerShipChanged(Ship old, Ship _new)
    {
        trackedShip = _new;
    }

    private void SnapshotWeapons()
    {
        if (weapons != null)
        {
            foreach (WeaponUISlot w in weapons)
            {
                Destroy(w.gameObject);
            }
            weapons.Clear();
        }
        else
        {
            weapons = new List<WeaponUISlot>();
        }
        
        foreach (Weapon w in trackedShip.Weapons)
        {
            WeaponUISlot slot = Instantiate(slotPrefab, transform).GetComponent<WeaponUISlot>();
            slot.AttachWeapon(w);
            weapons.Add(slot);
        }
    }

    private void OnWeaponChanged(Weapon selected)
    {
        int newIndex = trackedShip.SelectedWeaponIndex;
        weapons[highlightedWeapon].SwitchHighlight(false);
        highlightedWeapon = newIndex;
        weapons[highlightedWeapon].SwitchHighlight(true);
    }
}