using GameControl.StateMachine;

public class FlyToShipQuest : BaseQuest
{
    protected override StateMachine InitStateMachine()
    {
        StateMachine sm = new StateMachine();
        sm.AddTransition(
            NotificationState.Add(this, "Fly to ship"), 
            NotificationState.Success(this),
            () => locationManager.PlayerLocation?.LocationName == "SHIP",
            false
        );
        return sm;
    }
}