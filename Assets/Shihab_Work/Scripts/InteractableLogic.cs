using UnityEngine;

public class InteractableLogic : MonoBehaviour
{
    //public string interactableType; // e.g., "Door", "Item", "NPC"
    
    protected PlayerLogic playerLogic; // Reference to the player's logic script

    [Header("Interaction UI Settings")]
    public float interactionRange = 2f; // Range within which the player can interact with this object
    public float distanceFactor = 0.6f; // Factor to determine if the player is close enough to interact
    public Vector3 interactionUI_Offset = Vector3.zero; // Offset for the interaction UI element

    //For doors

    // For items
   

    public string interactionMessage=""; // Message to display when the player can interact with this object (e.g., "Press E to open the door")
    public virtual void Start()
    {
        playerLogic = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLogic>();
    }

    
    void Update()
    {
       
    }

    public virtual void Interact()
    {
        // Implement specific interaction logic based on the interactable type
        /*switch (interactableType)
        {
            case "Door":
               HandleDoor();
                break;
            case "Item":
                PickUpItem();
                break;
            case "NPC":
                //TalkToNPC();
                break;
            default:
                Debug.Log("Unknown interactable type: " + interactableType);
                break;
        }*/
        Debug.Log("Interacted with: " + gameObject.name);
    }

    

    
}
