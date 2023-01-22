using AI.States;
using UnityEngine;

public class FormationState : BaseShipAIState
{
    private FormationMember linkedMember;
    [SerializeField] private BaseShipAIState innerState;

    private void Start()
    {
        linkedMember = GetComponentInParent<FormationMember>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        innerState.OnEnter();
    }

    public override void Tick()
    {
        if (
            linkedMember.formationSlot == null || linkedMember.formation == null || linkedMember.isLeader ||
            !linkedMember.formation.keep
        )
        {
            innerState.Tick();
            return;
        }

        ShipAIControls.MoveAt(linkedMember.formationSlot.position);
    }

    public override void OnExit()
    {
        innerState.OnExit();
    }
}