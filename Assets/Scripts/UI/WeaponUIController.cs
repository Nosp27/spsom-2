using System.Collections.Generic;
using UnityEngine;

public class WeaponUIController : MonoBehaviour
{
    private Ship ship;
    [SerializeField] private GameObject weaponSlotPrefab;
    private List<WeaponUISlot> weapons;

    private void Start()
    {
        ship = GameController.Current.PlayerShip;
        SnapshotWeapons();
        ship.onWeaponMutate.AddListener(SnapshotWeapons);
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
        
        foreach (Weapon w in ship.Weapons)
        {
            WeaponUISlot slot = Instantiate(weaponSlotPrefab, transform).GetComponent<WeaponUISlot>();
            slot.AttachWeapon(w);
            weapons.Add(slot);
        }
    }
}
