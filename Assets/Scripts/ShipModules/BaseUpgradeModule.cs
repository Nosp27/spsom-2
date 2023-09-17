using GameEventSystem;
using UnityEngine;

public class BaseUpgradeModule : MonoBehaviour
{
    [SerializeField] private Transform aliveMesh;
    [SerializeField] private Transform debrisMesh;
    [SerializeField] private bool canDrop;
    private Ship carrier;

    public void Install(Ship _carrier)
    {
        carrier = _carrier;
        if (carrier.damageModel.AliveMesh && carrier.damageModel.DebrisMesh && aliveMesh && debrisMesh)
        {
            aliveMesh.SetParent(carrier.damageModel.AliveMesh.transform);
            debrisMesh.SetParent(carrier.damageModel.DebrisMesh.transform);
            
            // Parent mesh controls visibility of debris meshes
            debrisMesh.gameObject.SetActive(true);
        }

        foreach (BaseBuff buff in GetComponentsInChildren<BaseBuff>())
        {
            buff.Apply(carrier.gameObject);
        }
        
        EventLibrary.mutatePlayerShipWeapons.Invoke();
    }

    public void Drop()
    {
        if (!canDrop)
            return;
        
        if (carrier.damageModel.AliveMesh && carrier.damageModel.DebrisMesh && aliveMesh && debrisMesh)
        {
            aliveMesh.SetParent(transform);
            debrisMesh.SetParent(transform);
            
            debrisMesh.gameObject.SetActive(false);
        }
        
        foreach (BaseBuff buff in GetComponentsInChildren<BaseBuff>())
        {
            buff.Cancel();
        }
        
        carrier = null;
        EventLibrary.mutatePlayerShipWeapons.Invoke();
    }
}
