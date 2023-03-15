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


        IState flyToShipState = new LambdaState(
            () => notificationManager.RegisterNotification(flyToShipNotification),
            () => SetMarker(locationManager.QuestLocations["SHIP"].transform)
        );
        IState grabItemState = new LambdaState(
            () => notificationManager.RemoveNotification(flyToShipNotification, true),
            () => notificationManager.RegisterNotification(grabItemNotification),
                () => SetMarker(questItems["LOOT"].transform)
        );
        IState shootShipState = new LambdaState(
            () => notificationManager.RemoveNotification(grabItemNotification, true),
            () => notificationManager.RegisterNotification(shootShipNotification),
            () => SetMarker(questItems["ENEMY_SHIP"].transform)
        );
        IState questComplete = new LambdaState(
            () => notificationManager.RemoveNotification(shootShipNotification, true),
            Complete,
            () => SetMarker(null)
        );

        sm.AddTransition(
            flyToShipState,
            grabItemState,
            Predicates.PlayerAtLocation(locationManager, "SHIP"),
            false
        );
        sm.AddTransition(
            grabItemState,
            shootShipState,
            Predicates.LootPickup(playerShip, questItems["LOOT"]),
            false
        );
        sm.AddTransition(
            shootShipState,
            questComplete,
            Predicates.ShipDestroyed(questItems["ENEMY_SHIP"].GetComponent<Ship>()),
            false
        );
        return sm;
    }
}