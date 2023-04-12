using GameControl.StateMachine;

public class FlyToShipQuest : BaseQuest
{
    protected override StateMachine InitStateMachine()
    {
        StateMachine sm = new StateMachine();

        UINotification flyToShipNotification = UINotification.Add(notificationManager, "Fly to ship");
        UINotification grabItemNotification =
            UINotification.Add(notificationManager, "Find box among debris and grab it");
        UINotification shootShipNotification =
            UINotification.Add(notificationManager, "Set up turret and destroy an enemy ship");


        IState flyToShipState = LambdaState.create.WithEnterActions(
            () => notificationManager.RegisterNotification(flyToShipNotification),
            () => SetMarker(locationManager.QuestLocations["SHIP"].transform)
        );
        IState grabItemState = LambdaState.create.WithEnterActions(
            () => notificationManager.RemoveNotification(flyToShipNotification, true),
            () => notificationManager.RegisterNotification(grabItemNotification),
                () => SetMarker(QuestItemTransform("LOOT"))
        );
        IState shootShipState = LambdaState.create.WithEnterActions(
            () => notificationManager.RemoveNotification(grabItemNotification, true),
            () => notificationManager.RegisterNotification(shootShipNotification),
            () => SetMarker(QuestItemTransform("ENEMY_SHIP"))
        );
        IState questComplete = LambdaState.create.WithEnterActions(
            () => notificationManager.RemoveNotification(shootShipNotification, true),
            Complete,
            () => SetMarker(null)
        );

        var lootPickupPredicate = Predicates.LootPickup(playerShip, questItems["LOOT"]);
        var shipDestroyedPredicate = Predicates.ShipDestroyed(questItems["ENEMY_SHIP"].GetComponent<Ship>());
        
        sm.AddTransition(
            flyToShipState,
            grabItemState,
            Predicates.PlayerAtLocation(locationManager, "SHIP"),
            false
        );
        sm.AddTransition(
            grabItemState,
            shootShipState,
            () => questItems["LOOT"] == null || lootPickupPredicate.Invoke(),
            false
        );
        sm.AddTransition(
            shootShipState,
            questComplete,
            shipDestroyedPredicate,
            false
        );
        return sm;
    }
}