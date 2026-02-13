using UnityEngine;
using System.Collections;

public class ElevatorBtn : InteractableLogic
{
    [SerializeField] private platformLogic brain;
    [SerializeField] private Transform myFloorMarker; // Drag the Ground marker for the Ground button, etc.

    public override void Interact()
    {
        base.Interact();
        brain.CallElevator(myFloorMarker);
    }
}