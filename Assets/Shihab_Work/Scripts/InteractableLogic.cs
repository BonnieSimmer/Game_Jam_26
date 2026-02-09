using UnityEngine;

public class InteractableLogic : MonoBehaviour
{
    public string interactableType; // e.g., "Door", "Item", "NPC"
    public GameObject pickUpItemPos_leftHand; // Position where the item will be held when picked up (if applicable)
    public GameObject pickUpItemPos_rightHand; // Position where the item will be held when picked up (if applicable)

    public PlayerLogic playerLogic; // Reference to the player's logic script
    public string itemName;

    public float interactionRange = 2f; // Range within which the player can interact with this object
    public float distanceFactor = 0.6f; // Factor to determine if the player is close enough to interact
    public Vector3 interactionUI_Offset = Vector3.zero; // Offset for the interaction UI element

    public Vector3 itemScale;
    private bool isHeld = false; // Flag to check if the item is currently held by the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(interactableType == "Item")
        {
            itemScale = transform.localScale; // Store the original scale of the item
        }
        pickUpItemPos_leftHand = GameObject.Find("PickedUpItemPos_L");
        pickUpItemPos_rightHand = GameObject.Find("PickedUpItemPos_R");
        playerLogic = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactableType == "Item" && isHeld)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DropItem();
            }
            
        }
    }

    public void Interact()
    {
        // Implement specific interaction logic based on the interactable type
        switch (interactableType)
        {
            case "Door":
               // OpenDoor();
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
        }
    }

    private void PickUpItem()
    {
        if(playerLogic.canHoldItem_inLeftHand)
        {
            this.GetComponent<Rigidbody>().isKinematic = true; // Make the item kinematic so it doesn't fall
            this.GetComponent<Collider>().enabled = false; // Disable the collider to prevent physics interactions

            transform.SetParent(pickUpItemPos_leftHand.transform); // Parent the item to the player
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity; // Reset rotation to align with the player's hand
            transform.localScale = itemScale; // Reset scale to the original scale
            playerLogic.HandleInventory(itemName, true);
            isHeld = true;

        }
        else if(playerLogic.canHoldItem_inRightHand)
        {
            this.GetComponent<Rigidbody>().isKinematic = true; // Make the item kinematic so it doesn't fall
            this.GetComponent<Collider>().enabled = false; // Disable the collider to prevent physics interactions

            transform.SetParent(pickUpItemPos_rightHand.transform); // Parent the item to the player
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity; // Reset rotation to align with the player's hand
            transform.localScale = itemScale; // Reset scale to the original scale
            playerLogic.HandleInventory(itemName, false);
            isHeld = true;
        }
        
    }
    private void DropItem()
    {
        bool amInLeftHand = transform.parent == pickUpItemPos_leftHand.transform;
        bool amInRightHand = transform.parent == pickUpItemPos_rightHand.transform;

        GetComponent<Rigidbody>().isKinematic = false; // Make the item affected by physics again
        GetComponent<Collider>().enabled = true; // Enable the collider to allow interactions
        transform.SetParent(null); // Unparent the item from the player
        playerLogic.HandleInventory(itemName, false);
        transform.localScale = itemScale; // Reset scale to the original scale
        if (amInLeftHand) {
            playerLogic.canHoldItem_inLeftHand = true; // Allow the player to hold an item in the left hand again
            playerLogic.HandleInventory(itemName, false);
        }
        if(amInRightHand){
            playerLogic.canHoldItem_inRightHand = true; // Allow the player to hold an item in the right hand again
            playerLogic.HandleInventory(itemName, false);
        }
            isHeld = false;
    }
}
