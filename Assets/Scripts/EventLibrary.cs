using GameControl.InputHandlers;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem
{
    public static class EventLibrary
    {
        // General ship events
        public static readonly UnityEvent<Ship> shipSpawned = new();
        public static readonly UnityEvent<Ship, BulletHitDTO> shipDealsDamage = new();
        public static readonly UnityEvent<DamageModel> objectDestroyed = new();
        public static readonly UnityEvent<DamageModel, BulletHitDTO> objectReceivesDamage = new();
        public static readonly UnityEvent<Ship, DamageModel> shipKills = new();
        public static readonly UnityEvent<Ship> shipShoots = new();
        public static readonly UnityEvent<ShipDamageModel> shipReceivesHeal = new();
        public static readonly UnityEvent<Ship, InventoryItem> shipGrabItem = new();

        // Player ship events
        public static readonly UnityEvent<Ship, Ship> switchPlayerShip = new();
        public static readonly UnityEvent mutatePlayerShipWeapons = new();
        public static readonly UnityEvent<Weapon> selectPlayerShipWeapon = new();

        // Game controller events
        public static readonly UnityEvent<GameObject> cursorHoverTargetChanged = new();
        public static readonly UnityEvent<AimLockTarget> lockTargetChanged = new();
        
        // Module Events
        // -- Crane
        public static readonly UnityEvent<GameObject> onCraneGrab = new();
        
        // -- Radar
        public static readonly UnityEvent<RadarTarget> onObjectEncounter = new();
        public static readonly UnityEvent<RadarTarget> onObjectLost = new();
        
        // Inventory events
        public static readonly UnityEvent<InventoryItem> onInventoryPutItem = new();
        public static readonly UnityEvent<InventoryItem> onInventoryrRemoveItem = new();
        
        // User input events
        public static readonly UnityEvent<PlayerInput> inputMoveActionPerformed = new();
        public static readonly UnityEvent<PlayerInput> inputShootActionPerformed = new();
        public static readonly UnityEvent<PlayerInput> inputStopMovementActionPerformed = new();
        public static readonly UnityEvent<PlayerInput> inputBrakeActionPerformed = new();
        public static readonly UnityEvent<PlayerInput> inputInteractActionPerformed = new();
    }
}