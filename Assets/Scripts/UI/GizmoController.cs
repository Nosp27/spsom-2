using System;
using UnityEngine;

public class GizmoController : MonoBehaviour
{
    public FacilityGUI selectedFacility;
    private CursorControl cursorControl;

    private void Start()
    {
        cursorControl = GameController.Current.GetComponent<CursorControl>();
    }

    public void ShipFacilityHover(FacilityGUI facility)
    {

    }

    private void Update()
    {
        FacilityGUI newSelectedFacility = cursorControl.GetHoveredFacility();
        if (selectedFacility != newSelectedFacility)
        {
            if (newSelectedFacility != null)
                newSelectedFacility.SendMessage("Hover");

            if (selectedFacility != null)
                selectedFacility.SendMessage("Unhover");

            ShipFacilityHover(newSelectedFacility);
            selectedFacility = newSelectedFacility;
        }
    }
}