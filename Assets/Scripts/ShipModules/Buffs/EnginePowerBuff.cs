using SpaceShip.PhysicalMovement;
using UnityEngine;

public class EnginePowerBuff : BaseBuff
{
    [SerializeField] private float enginePowerFactor = 1.45f;
    protected override bool ApplyBuffInternal(GameObject go)
    {
        Physical4EngineSplitter splitter = go.GetComponentInChildren<Physical4EngineSplitter>();
        if (splitter == null)
            return false;

        splitter.ChangeForce(splitter.Force * enginePowerFactor);
        splitter.Init(go.transform, null);
        print("Engine buff apply");
        return true;
    }
}
