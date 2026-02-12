using UnityEngine;

public class ItemsLogic : InteractableLogic
{
    [Header("assign a name for the item")]
    public string itemName;
    [Header("assign an ID for (Key)")]
    public int itemID; // Unique identifier for the item, used for inventory management

    private bool isHeld = false; // Flag to check if the item is currently held by the player
    private Vector3 itemScale;
    private GameObject pickUpItemPos_leftHand; // Position where the item will be held when picked up (if applicable)
    private GameObject pickUpItemPos_rightHand; // Position where the item will be held when picked up (if applicable)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();



        itemScale = transform.localScale; // Store the original scale of the item
        if (itemName == "Key")
        {
            itemName = "Key" + itemID.ToString();
        }

        pickUpItemPos_leftHand = GameObject.Find("PickedUpItemPos_L");
        pickUpItemPos_rightHand = GameObject.Find("PickedUpItemPos_R");

        if(pickUpItemPos_leftHand == null || pickUpItemPos_rightHand == null)
        {
            Debug.LogError("Pick up item positions not found. Please ensure there are GameObjects named 'PickedUpItemPos_L' and 'PickedUpItemPos_R' in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DropItem();
            }

        }
    }
    public override void Interact()
    {
        if (!isHeld)
        {
            PickUpItem();
        }
    }
    private void PickUpItem()
    {
        if (playerLogic.canHoldItem_inLeftHand)
        {
            this.GetComponent<Rigidbody>().isKinematic = true; // Make the item kinematic so it doesn't fall
            this.GetComponent<Collider>().enabled = false; // Disable the collider to prevent physics interactions

            transform.SetParent(pickUpItemPos_leftHand.transform); // Parent the item to the player
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity; // Reset rotation to align with the player's hand
            transform.localScale = itemScale; // Reset scale to the original scale
            playerLogic.HandleInventory(itemName, true);
            isHeld = true;

            playerLogic.canHoldItem_inLeftHand = false; // Prevent the player from holding another item in the left hand

        }
        else if (playerLogic.canHoldItem_inRightHand)
        {
            this.GetComponent<Rigidbody>().isKinematic = true; // Make the item kinematic so it doesn't fall
            this.GetComponent<Collider>().enabled = false; // Disable the collider to prevent physics interactions

            transform.SetParent(pickUpItemPos_rightHand.transform); // Parent the item to the player
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity; // Reset rotation to align with the player's hand
            transform.localScale = itemScale; // Reset scale to the original scale
            playerLogic.HandleInventory(itemName, false);
            isHeld = true;

            playerLogic.canHoldItem_inRightHand = false; // Prevent the player from holding another item in the right hand
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
        if (amInLeftHand)
        {
            playerLogic.canHoldItem_inLeftHand = true; // Allow the player to hold an item in the left hand again
            
        }
        if (amInRightHand)
        {
            playerLogic.canHoldItem_inRightHand = true; // Allow the player to hold an item in the right hand again
            
        }
        isHeld = false;
    }
}
