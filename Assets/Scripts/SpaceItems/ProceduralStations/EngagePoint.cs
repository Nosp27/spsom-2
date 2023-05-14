using System.Collections;
using System.Collections.Generic;
using SpaceItems.ProceduralStations;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EngagePoint : MonoBehaviour
{
    public StationModule attachedModule { get; private set; }

    public void SetAttachedModule(StationModule module)
    {
        attachedModule = module;
    }
}
