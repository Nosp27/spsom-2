using UI.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem
{
    public static class EventLibrary
    {
        // General ship events
        public static readonly UnityEvent<Ship> shipSpawned = new UnityEvent<Ship>();
        public static readonly UnityEvent<Ship, BulletHitDTO> shipDealsDamage = new UnityEvent<Ship, BulletHitDTO>();
        public static readonly UnityEvent<DamageModel> objectDestroyed = new UnityEvent<DamageModel>();
        public static readonly UnityEvent<DamageModel, BulletHitDTO> objectReceivesDamage = new UnityEvent<DamageModel, BulletHitDTO>();
        public static readonly UnityEvent<Ship, DamageModel> shipKills = new UnityEvent<Ship, DamageModel>();
        
        // Player ship events
        public static readonly UnityEvent<Ship, Ship> switchPlayerShip = new UnityEvent<Ship, Ship>();
        public static readonly UnityEvent mutatePlayerShipWeapons = new UnityEvent();
        public static readonly UnityEvent<Weapon> selectPlayerShipWeapon = new UnityEvent<Weapon>();
        
        // Game controller events
        public static readonly UnityEvent<GameObject> cursorHoverTargetChanged = new UnityEvent<GameObject>();
        
        // Module Events
        // -- Crane
        public static readonly UnityEvent<GameObject> onCraneGrab = new UnityEvent<GameObject>();
        
        // -- Radar
        public static readonly UnityEvent<RadarTarget> onObjectEncounter = new UnityEvent<RadarTarget>();
        public static readonly UnityEvent<RadarTarget> onObjectLost = new UnityEvent<RadarTarget>();
        
        // Inventory events
        public static readonly UnityEvent<InventoryItem> onInventoryPutItem = new UnityEvent<InventoryItem>();
        public static readonly UnityEvent<InventoryItem> onInventoryrRemoveItem = new UnityEvent<InventoryItem>();
    }
}