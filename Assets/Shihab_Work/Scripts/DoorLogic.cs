using UnityEngine;

public class DoorLogic : InteractableLogic
{
    [Header("Door Settings")]
    private bool isDoorOpen = false; // Flag to check if the door is currently open
    private bool isLocked = false;
    public int doorID; // Unique identifier for the door, used to link with the neighbor door

    [Header("Door Ownership")]
    public bool isPlayerDoor = false;
    public bool isNeighborDoor = false;
    public GameObject neighbor;
    
    public Animator doorAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        if(doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Interact()
    {
        
       HandleDoor();
        
    }
    private void HandleDoor()
    {
        // Implement door opening logic here
        Debug.Log("Door interacted with: " + gameObject.name);
        
        if (doorAnimator != null)
        {
            if (isLocked)
            {
                string keyName = "Key" + doorID.ToString();
                if (playerLogic.inventory[keyName])
                {
                    isLocked = false; // Unlock the door if the player has the corresponding key
                    Debug.Log("You used the key to unlock the door.");
                }
                else
                {
                    Debug.Log("The door is locked. You need a key to open it.");
                    return; // Exit the method if the door is locked and the player doesn't have the key
                }
            }
            if (isPlayerDoor || !isLocked)
            {
                isDoorOpen = !isDoorOpen; // Toggle the door state

                if (doorAnimator != null)
                {
                    
                    doorAnimator.SetBool("isOpen", isDoorOpen); // Set the animator parameter to open/close the door
                }
                else
                {

                   Debug.LogError("Door Animator is not assigned for " + gameObject.name);
                }
            }
            else if (isNeighborDoor)
            {
                //get relation with the neighbor for the corresponding door
                //if relation is satisfied, open the door
                //else, knock on the door or show a message that the door is locked
                interactionMessage = "Knock on the door";
            }
        }
    }
}
