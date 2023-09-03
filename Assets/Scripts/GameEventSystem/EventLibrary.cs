using System;

namespace GameEventSystem
{
    public static class EventLibrary
    {
        public static readonly GameEvent<Ship> shipSpawned = new GameEvent<Ship>();
        public static readonly GameEvent<Ship> shipDestroyed = new GameEvent<Ship>();
        public static readonly GameEvent<Tuple<Ship, Ship>> shipGetsDamage = new GameEvent<Tuple<Ship, Ship>>();
        public static readonly GameEvent<Tuple<Ship, Ship>> shipDealsDamage = new GameEvent<Tuple<Ship, Ship>>();
        public static readonly GameEvent<Tuple<Ship, Ship>> shipKills = new GameEvent<Tuple<Ship, Ship>>();
    }
}