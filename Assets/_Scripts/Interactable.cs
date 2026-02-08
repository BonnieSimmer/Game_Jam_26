using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    // Called from player
    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
      // Overriden in child classes  
    }
}
