using System;
using AI.States;
using UnityEngine;

public class FormationMember : MonoBehaviour
{
    [SerializeField] private float formationDetectionRange = 100;
    
    private Formation _formation;

    public Ship thisShip => GetComponentInParent<Ship>();

    public Formation formation => _formation;

    private BaseShipAIState innerState;
    
    public bool isLeader;
    public Transform formationSlot {get; private set; }
    
    void Start()
    {
        _formation = FindNearestFormation();
        if (_formation != null)
        {
            _formation.RegisterMember(this);
        }
    }

    public void OnRegister(Formation newFormation, Transform slot)
    {
        if (_formation != null && slot == null)
            throw new Exception("WTF");
        _formation = newFormation;
        formationSlot = slot;
    }

    public void OnRemove()
    {
        _formation = null;
        formationSlot = null;
    }

    private Formation FindNearestFormation()
    {
        Formation[] allFormations = FindObjectsOfType<Formation>();
        Formation selected = null;
        float minDistance = float.MaxValue;
        foreach (var formation in allFormations)
        {
            float distance = Vector3.Distance(formation.transform.position, transform.position);
            if (distance > formationDetectionRange)
                continue;

            if (selected == null || distance < minDistance)
                selected = formation;
        }

        return selected;
    }
}
