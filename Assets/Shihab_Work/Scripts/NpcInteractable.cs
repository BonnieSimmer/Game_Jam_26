using System.Collections;
using UnityEngine;
using Yarn.Unity;
public class NpcInteractable : InteractableLogic
{
    public string npcName;
    public DialogueRunner dialogueRunner; // Reference to the DialogueRunner component for handling dialogues

    private int relationshipLevel = 0; // Example variable to track relationship level with the NPC
    private GameManager gameManager;
    private bool isLookingAtPlayer = false; // Flag to check if the NPC is currently looking at the player
    private bool playerIsLookingAtNPC = false; // Flag to check if the player is currently looking at the NPC

    private Quaternion defaultRotation; // To store the original rotation of the NPC
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Coroutine lookAtPlayerCoroutine; // Reference to the coroutine for looking at the player
    private Coroutine lookAtNPCCoroutine;
    private void Awake()
    {
        // Ensure that the NPC has a DialogueRunner component attached
        if (dialogueRunner == null)
        {
            dialogueRunner = GameObject.Find("Dialogue System").GetComponent<DialogueRunner>();
            if (dialogueRunner == null)
            {
                Debug.LogError("DialogueRunner component not found on " + gameObject.name + ". Please attach a DialogueRunner component.");
            }
        }

        if (GameObject.Find("GameManager") != null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        else
            Debug.LogError("GameManager not found in the scene. Please ensure there is a GameObject named 'GameManager' with a GameManager component attached.");


    }
    public override void Start()
    {
        // Create a new Transform to store the original rotation
        defaultRotation = transform.rotation; // Store the original rotation of the NPC
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact()
    {
        if(gameManager == null || dialogueRunner == null)
        {
            Debug.LogError("GameManager or DialogueRunner is not assigned. Please ensure they are properly set up in the scene.");
            return;
        }

        int currentDay = gameManager.dayNumber;
        string codeName = npcName + "_stage" + currentDay.ToString();
        if (dialogueRunner.Dialogue.NodeExists(codeName))
        {
            dialogueRunner.StartDialogue(codeName);

        }
        else
        {
            Debug.LogWarning("No dialogue found for " + codeName);
        }

        if(lookAtNPCCoroutine != null) StopCoroutine(lookAtNPCCoroutine);
        if(lookAtPlayerCoroutine != null) StopCoroutine(lookAtPlayerCoroutine);
        
        lookAtPlayerCoroutine = StartCoroutine(SmoothLookAt(transform, playerLogic.transform));
        lookAtNPCCoroutine = StartCoroutine(SmoothLookAt(playerLogic.transform, transform));

    }
    public void OnEnable()
    {
        if(dialogueRunner != null)
            dialogueRunner.onDialogueComplete.AddListener(StopLookingAtPlayer);
    }
    public void OnDisable()
    {
        if(dialogueRunner != null)
            dialogueRunner.onDialogueComplete.RemoveListener(StopLookingAtPlayer);
    }
    IEnumerator SmoothLookAt(Transform currTransform, Transform targetTransform)
    {

        float timer = 0f;
        float lookAtDuration = 2f; // Duration for which the NPC will look at the player

        Quaternion originalRotation = currTransform.rotation; // Store the original rotation of the NPC
        Vector3 lookAtDirection = targetTransform.position - currTransform.position; // Calculate the direction to look at
        lookAtDirection.y = 0; // Keep the NPC's head level by ignoring the y-axis

        if (lookAtDirection != Vector3.zero) // Check to avoid zero-length direction
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookAtDirection); // Calculate the target rotation to look at the player
            while (timer <= lookAtDuration)
            {
                timer += Time.deltaTime;
                float t = timer / lookAtDuration; // Normalize the timer to a 0-1 range

                t = t * t * (3f - 2f * t); // Apply a smooth step function for smoother interpolation

                currTransform.rotation = Quaternion.Slerp(originalRotation, targetRotation, t);

                yield return null; // Wait for the next frame
            }
            currTransform.rotation = targetRotation; // Ensure the final rotation is exactly the target rotation
        }
    }
    IEnumerator RotateBackToDefault()
    {

        float timer = 0f;
        float lookAtDuration = 2f; // Duration for which the NPC will look at the player

        Quaternion originalRotation = transform.rotation; // Store the original rotation of the NPC




        while (timer <= lookAtDuration)
        {
            timer += Time.deltaTime;
            float t = timer / lookAtDuration; // Normalize the timer to a 0-1 range

            t = t * t * (3f - 2f * t); // Apply a smooth step function for smoother interpolation

            transform.rotation = Quaternion.Slerp(originalRotation, defaultRotation, t);

            yield return null; // Wait for the next frame
        }
        transform.rotation = defaultRotation; // Ensure the final rotation is exactly the target rotation
    }
    public void StopLookingAtPlayer()
    {
        StartCoroutine(RotateBackToDefault());
    }

}
