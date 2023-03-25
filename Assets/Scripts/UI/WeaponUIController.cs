using System.Collections.Generic;
using UnityEngine;

public class WeaponUIController : MonoBehaviour
{
    private Ship ship;
    [SerializeField] private GameObject slotPrefab;
    private List<WeaponUISlot> weapons;

    private int highlightedWeapon;

    private void Start()
    {
        ship = GameController.Current.PlayerShip;
        highlightedWeapon = ship.SelectedWeaponIndex;
        SnapshotWeapons();
        ship.OnWeaponSelect.AddListener(OnWeaponChanged);
        ship.OnWeaponMutate.AddListener(SnapshotWeapons);
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
            WeaponUISlot slot = Instantiate(slotPrefab, transform).GetComponent<WeaponUISlot>();
            slot.AttachWeapon(w);
            weapons.Add(slot);
        }
    }

    private void OnWeaponChanged()
    {
        if (weapons.Count == 0)
            return;
        
        int newIndex = ship.SelectedWeaponIndex;
        weapons[highlightedWeapon].SwitchHighlight(false);
        highlightedWeapon = newIndex;
        weapons[highlightedWeapon].SwitchHighlight(true);
    }
}