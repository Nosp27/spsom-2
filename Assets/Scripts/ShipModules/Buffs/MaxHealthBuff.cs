using UnityEngine;

public class MaxHealthBuff : BaseBuff
{
    [SerializeField] private float healthFactor = 1.45f;
    protected override bool ApplyBuffInternal(GameObject go)
    {
        ShipDamageModel sdm = go.GetComponent<ShipDamageModel>();
        if (sdm == null)
            return false;
        
        sdm.ChangeMaxHealth((int)(sdm.MaxHealth * healthFactor));
        return true;
    }
}
