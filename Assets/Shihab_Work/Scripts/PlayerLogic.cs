using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Yarn.Unity;

public class PlayerLogic : MonoBehaviour
{
    private Rigidbody rb;
   

    public RectTransform pressE_UI;
    public RectTransform interactableIndicator;
    public TextMeshProUGUI interactableIndicatorText;

    public bool canHoldItem_inLeftHand = true;
    public bool canHoldItem_inRightHand = true;
    public Camera mainCamera;
    public GameObject closestObject;
    private bool isInRange = false;
    public float distanceFactor = 0.6f;

    public AudioSource playerSrc;
    public AudioClip[] audioClips;

    public Dictionary<string, bool> inventory;
    public DialogueRunner dialogueRunner;

    [SerializeField] private float interactableIndicatorRange = 3f;
    [SerializeField] private float interactableRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer = 6;
    private Vector3 interactableUI_Offset; // Offset for the interaction UI element

    [Header("Player light level Stats")]
    public int lightHeartLevel = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Dictionary<string, bool>();
        rb = GetComponent<Rigidbody>();
        interactableIndicator.gameObject.SetActive(false);
        pressE_UI.gameObject.SetActive(false);
        playerSrc = GetComponent<AudioSource>();
        lightHeartLevel = PlayerPrefs.GetInt("lightHeartLevel", 20); // Load light heart level from PlayerPrefs, defaulting to 20 if not found
        if (dialogueRunner != null)
        {
            dialogueRunner.onNodeComplete.AddListener(PlayChoiceSound);
        }
    }

    private void PlayChoiceSound(string nodeName)
    {
        if (playerSrc != null)
        {
            // You can play a specific clip
            if (audioClips.Length >0)
            {
                playerSrc.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
            }
            // Or use one from your existing audioClips array
            else if (audioClips.Length > 0)
            {
                playerSrc.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        // Prevent interaction when clicking on UI elements
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
        {
            // Optional: Hide the "Press E" UI while talking so it looks cleaner
            if (pressE_UI.gameObject.activeSelf) pressE_UI.gameObject.SetActive(false);
            return;
        }

        DetermineClosestInteractable();
        ShowInteractableUI();
        if (Input.GetKeyDown(KeyCode.E) && closestObject != null && isInRange)
        {
            // Implement interaction logic here, such as opening a door or picking up an item
            Debug.Log("Interacted with: " + closestObject.name);
            closestObject.GetComponent<InteractableLogic>().Interact();
            var logic = closestObject.GetComponent<InteractableLogic>();
            if (logic != null)
            {
                interactableIndicatorText.text = logic.interactionMessage;
            }
        }
        if (dialogueRunner != null && dialogueRunner.VariableStorage != null)
        {
            // Sync Yarn variable -> C# variable
            if (dialogueRunner.VariableStorage.TryGetValue("$lightHeartLevel", out float yarnLevel))
            {
                int audioIndex = Random.Range(0, audioClips.Length);
                playerSrc.PlayOneShot(audioClips[audioIndex],1);
                // Update the C# variable so HeartDisplay can read the correct value
                this.lightHeartLevel = (int)yarnLevel;
            }
        }

    }

    private void DetermineClosestInteractable()
    {
        Collider[] nearByObjects = Physics.OverlapSphere(transform.position, interactableIndicatorRange, interactableLayer);
        closestObject = null;

        float closestDistance = Mathf.Infinity;
        if (nearByObjects.Length > 0)
        {
            foreach (Collider col in nearByObjects)
            {
                InteractableLogic logic = col.GetComponent<InteractableLogic>();
                if (logic == null) continue; // Skip objects that don't have the script

                float distanceToCol = Vector3.Distance(transform.position, col.transform.position);
                if (distanceToCol < closestDistance)
                {
                    closestObject = col.gameObject;
                    closestDistance = distanceToCol;
                    interactableRange = col.GetComponent<InteractableLogic>().interactionRange;
                    distanceFactor = col.GetComponent<InteractableLogic>().distanceFactor;
                    interactableUI_Offset = col.GetComponent<InteractableLogic>().interactionUI_Offset;
                }

            }
        }
        else
        {
            closestObject = null;
        }
    }
    private void ShowInteractableUI()
    {
        if (closestObject != null)
        {
            //Vector3 screenPos = mainCamera.WorldToScreenPoint(closestObject.GetComponent<Collider>().bounds.center);
            /* if (screenPos.z < 0) // Check if the object is behind the camera
             {
                 interactableIndicator.gameObject.SetActive(false);
                 pressE_UI.gameObject.SetActive(false);
                 return;
             }*/
            //distanceFactor = 0.6f;


            float distanceToPlayer = Vector3.Distance(transform.position, closestObject.transform.position);
            Vector3 objectCenter = closestObject.GetComponent<Collider>().bounds.center;
            Vector3 dirToCamera = (mainCamera.transform.position - closestObject.transform.position).normalized;
            Vector3 uiPosition = closestObject.transform.position + dirToCamera * distanceToPlayer * distanceFactor+interactableUI_Offset; // Adjust the distance of the UI from the object


            interactableIndicator.rotation = mainCamera.transform.rotation; // Make the indicator face the camera
            interactableIndicator.position = uiPosition;
            float distanceToObject = Vector3.Distance(transform.position, closestObject.transform.position);

            //WithinInteractableRange();
            if (distanceToObject <= interactableRange)
            {
                isInRange = true;
                interactableIndicator.gameObject.SetActive(false);
                pressE_UI.gameObject.SetActive(true);
                pressE_UI.position = uiPosition;
                pressE_UI.rotation = mainCamera.transform.rotation;
                //+ new Vector3(0, 30, 0); // Adjust the position of the "Press E" UI
            }
            else
            {
                isInRange = false;
                interactableIndicator.gameObject.SetActive(true);
                pressE_UI.gameObject.SetActive(false);
                interactableIndicator.rotation = mainCamera.transform.rotation; // Make the indicator face the camera
            }
        }
        else
        {
            isInRange = false;
            pressE_UI.gameObject.SetActive(false);
            interactableIndicator.gameObject.SetActive(false);
        }
    }

    public void HandleInventory(string itemName, bool add)
    {
        if (add)
        {
            if(inventory.ContainsKey(itemName)) inventory[itemName] = true;
            else inventory.Add(itemName, true);
            
        }
        else
        {
            if(inventory.ContainsKey(itemName)) inventory.Remove(itemName);
            
        }

    }
    /*
    private void WithinInteractableRange()
    {
        Collider[] nearByObjects = Physics.OverlapSphere(transform.position, interactableRange, interactableLayer);
        float closestDistance = Mathf.Infinity;
        if (nearByObjects.Length > 0)
        {
            isInRange = true;
            foreach (Collider col in nearByObjects)
            {
                float distanceToCol = Vector3.Distance(transform.position, col.transform.position);
                if (distanceToCol < closestDistance)
                {
                    closestObject = col.gameObject;
                    closestDistance = distanceToCol;
                }
            }
        }
        else
        {
            isInRange = false;
        }
    }
     * if (col.gameObject.CompareTag("Door"))
                {
                    // Apply damage to the player
                    Debug.Log("Player is near an Interactable");
                    // You can implement your damage logic here, such as reducing health or triggering a hit animation
                }
    */
}
