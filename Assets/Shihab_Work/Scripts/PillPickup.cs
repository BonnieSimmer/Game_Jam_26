using UnityEngine;
using Yarn.Unity;

public class PillPickup : ItemsLogic
{
    [Header("Pill Settings")]
    public bool isCorrectPill; // Check this for Green, Uncheck for Red
    public DialogueRunner dialogueRunner;

    // Override the base Interact() from your InteractableLogic
    public override void Interact()
    {
        base.Interact(); // Show the "Picked up" UI text

        if (dialogueRunner != null)
        {
            // Set Yarn Variables
            if (isCorrectPill)
            {
                dialogueRunner.VariableStorage.SetValue("$has_green_pill", true);
                dialogueRunner.VariableStorage.SetValue("$has_red_pill", false);
                Debug.Log("Picked up GREEN pill");
            }
            else
            {
                dialogueRunner.VariableStorage.SetValue("$has_green_pill", false);
                dialogueRunner.VariableStorage.SetValue("$has_red_pill", true);
                Debug.Log("Picked up RED pill");
            }
        }

        // Hide the object so player can't pick it up twice
        gameObject.SetActive(false);
    }
}
