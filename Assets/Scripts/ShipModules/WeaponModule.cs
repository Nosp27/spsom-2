using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModule : ShipModule
{
    public override void Install()
    {
        Ship s = GetComponentInParent<Ship>();
        s.ScanWeaponary();
    }

    public override void Uninstall()
    {
        Ship s = GetComponentInParent<Ship>();
        transform.SetParent(null);
        s.ScanWeaponary();
    }
}
