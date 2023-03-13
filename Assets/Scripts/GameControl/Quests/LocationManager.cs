using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public Dictionary<string, QuestLocation> QuestLocations { get; private set; }
    public QuestLocation PlayerLocation { get; private set; }

    private void Awake()
    {
        QuestLocations = new Dictionary<string, QuestLocation>();
    }

    public void Bind(QuestLocation loc)
    {
        QuestLocations[loc.LocationName] = loc;
        loc.LocationTrigger.onTriggerEnter.AddListener(_ => UpdatePlayerLocation(loc));
        loc.LocationTrigger.onTriggerExit.AddListener(_ => UpdatePlayerLocation(null));
    }

    private void UpdatePlayerLocation(QuestLocation loc)
    {
        PlayerLocation = loc;
    }
}