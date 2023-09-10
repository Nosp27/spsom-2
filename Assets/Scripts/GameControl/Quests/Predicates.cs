using System;
using GameEventSystem;
using UnityEngine;
using UnityEngine.Events;

public class Predicates : MonoBehaviour
{
    private class Tracker
    {
        public bool triggered { get; protected set; }

        public Tracker(UnityEvent _event)
        {
            triggered = false;
            if (_event != null)
                _event.AddListener(() => triggered = true);
        }
    }

    private class Tracker<T> : Tracker
    {
        public Tracker(UnityEvent<T> _event, Func<T, bool> if_ = null) : base(null)
        {
            if (_event != null)
                _event.AddListener(x => triggered |= if_ == null || if_(x));
        }
    }

    public static Func<bool> LootPickup(GameObject targetLoot)
    {
        Tracker<GameObject> tracker = new Tracker<GameObject>(
            EventLibrary.onCraneGrab, loot =>
            {
                print($" LootPickup: {loot} == {targetLoot} ({loot == targetLoot}) ");
                return loot == targetLoot;
            }
        );
        return () => tracker.triggered;
    }

    public static Func<bool> TriggerEnter(ColliderTrigger trigger)
    {
        Tracker<Collider> tracker = new Tracker<Collider>(trigger.onTriggerEnter);
        return () => tracker.triggered;
    }

    public static Func<bool> ShipDestroyed(Ship ship)
    {
        DamageModel damageModel = ship.GetComponent<DamageModel>();
        Tracker<DamageModel> tracker = new Tracker<DamageModel>(
            EventLibrary.objectDestroyed, dm => dm == damageModel
        );
        return () => tracker.triggered;
    }

    public static Func<bool> PlayerAtLocation(LocationManager lm, string name)
    {
        return () => lm.PlayerLocation != null && lm.PlayerLocation.LocationName == name;
    }
}