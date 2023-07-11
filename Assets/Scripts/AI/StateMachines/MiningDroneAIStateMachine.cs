using AI;
using AI.States;
using GameControl.StateMachine;
using UnityEngine;

public interface IMiningController
{
    public void StartMining();
    public void StopMining();
    public bool IsMining();

    public bool StoppedMiningImmidiately();

    public bool DoneMining();
    public bool CanChooseNextMiningTarget();

    public bool ShouldSwitchMiningPlace();

    public bool ReachedAsteroid();

    public bool ReachedStation();
}

public class MiningDroneAIStateMachine : MonoBehaviour, IMiningController
{
    private StateMachine fsm;

    [SerializeField] private BaseShipAIState flyToMine;
    [SerializeField] private BaseShipAIState doMining;
    [SerializeField] private BaseShipAIState leaveMiningTarget;
    [SerializeField] private BaseShipAIState flyToStation;
    [SerializeField] private BaseShipAIState dockToStation;
    [SerializeField] private BaseShipAIState runAway;

    [SerializeField] private EnemyDetector enemyDetector;

    private IMiningController m_MiningController;

    void Start()
    {
        m_MiningController = this;
    }

    StateMachine BuildStateMachine()
    {
        StateMachine sm = new StateMachine();
        sm.AddAnyTransition(flyToMine, () => true);
        sm.AddTransition(flyToMine, doMining, m_MiningController.ReachedAsteroid);
        sm.AddTransition(doMining, leaveMiningTarget, m_MiningController.ShouldSwitchMiningPlace);
        sm.AddTransition(leaveMiningTarget, flyToMine, m_MiningController.CanChooseNextMiningTarget);
        sm.AddTransition(doMining, flyToStation, m_MiningController.DoneMining);
        sm.AddTransition(flyToStation, dockToStation, m_MiningController.ReachedStation);
        sm.AddAnyTransition(runAway, () => enemyDetector.Enemy != null);
        sm.AddTransition(runAway, flyToStation,
            () => enemyDetector.Enemy == null && !m_MiningController.StoppedMiningImmidiately());
        sm.AddTransition(runAway, flyToMine,
            () => enemyDetector.Enemy == null && m_MiningController.StoppedMiningImmidiately());
        return sm;
    }

    public void StartMining()
    {
        throw new System.NotImplementedException();
    }

    public void StopMining()
    {
        throw new System.NotImplementedException();
    }

    public bool IsMining()
    {
        throw new System.NotImplementedException();
    }

    public bool StoppedMiningImmidiately()
    {
        throw new System.NotImplementedException();
    }

    public bool DoneMining()
    {
        throw new System.NotImplementedException();
    }

    public bool CanChooseNextMiningTarget()
    {
        throw new System.NotImplementedException();
    }

    public bool ReachedAsteroid()
    {
        return false;
    }

    public bool ReachedStation()
    {
        throw new System.NotImplementedException();
    }

    public bool ShouldSwitchMiningPlace()
    {
        return false;
    }
}