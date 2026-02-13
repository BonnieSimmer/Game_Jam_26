using System.Collections;
using UnityEngine;
using UnityEngine.AI;
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

    private NpcRoaming npcRoaming; // Reference to the NpcRoaming script for controlling NPC movement
    private NavMeshAgent agent; // Reference to the NavMeshAgent component for controlling NPC movement

    [SerializeField] private AudioSource npcAudioSource; // Reference to the AudioSource component for playing NPC sounds
    [SerializeField] private AudioClip[] npcAudioClips;
    [Header("Relationship Settings")]
    public int relationship=0;
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

        if(agent == null)
            agent = GetComponent<NavMeshAgent>();
        else
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name + ". Please attach a NavMeshAgent component.");

    }
    public override void Start()
    {
        // Create a new Transform to store the original rotation
        base.Start();
       npcAudioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the NPC
        npcRoaming = GetComponent<NpcRoaming>(); // Get the NpcRoaming component attached to the NPC
        relationship = PlayerPrefs.GetInt(npcName+"_relationship", 0); // Load relationship level from PlayerPrefs, defaulting to 0 if not found

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact()
    {
        if (gameManager == null || dialogueRunner == null)
        {
            Debug.LogError("GameManager or DialogueRunner is not assigned. Please ensure they are properly set up in the scene.");
            return;
        }
        defaultRotation = transform.rotation; // Store the original rotation of the NPC
        npcRoaming.PauseNpcRoaming();

        string progressVarKey = "$"+npcName+"_progress";
        string lastTalkedDay_Key = "$"+npcName+"_lastTalkedDay";
        string relationshipKey = "$"+npcName+"_relationship";
       
        dialogueRunner.VariableStorage.SetValue(relationshipKey, relationship);
       
        int savedProgress = PlayerPrefs.GetInt(npcName+"_progress", 0); 
        dialogueRunner.VariableStorage.SetValue(progressVarKey, savedProgress);
        
        int savedLastTalkedDay = PlayerPrefs.GetInt(npcName+"_lastTalkedDay", -1); 
        dialogueRunner.VariableStorage.SetValue(lastTalkedDay_Key, savedLastTalkedDay);
        
        
        int currentDay = gameManager.dayNumber;

        Debug.Log($"DIAGNOSIS: Last Talked Day: {savedLastTalkedDay} | Current Day: {currentDay}");
        // ---------
        string targetNode = "";

        if(savedLastTalkedDay< currentDay)
        {
            targetNode = npcName + "_stage" + savedProgress.ToString();
            dialogueRunner.VariableStorage.SetValue(lastTalkedDay_Key, currentDay);
            PlayerPrefs.SetInt(npcName+"_lastTalkedDay", currentDay);
            PlayerPrefs.Save();
        }
        else
        {
            targetNode = npcName + "_exhausted";
        }

            
        if (dialogueRunner.Dialogue.NodeExists(targetNode))
        {
            dialogueRunner.StartDialogue(targetNode);
            
        }
        else
        {
            string defaultNode = npcName + "_default";  // change _defualt to _stage0 
            if (dialogueRunner.Dialogue.NodeExists(defaultNode))
            {
                dialogueRunner.StartDialogue(defaultNode);
            }
            else
            {
                Debug.LogWarning($"Neither '{targetNode}' nor '{defaultNode}' found!");
                npcRoaming.ResumeNpcRoaming(); // Resume roaming if no dialogue nodes are found
            }
        }

        if(lookAtNPCCoroutine != null) StopCoroutine(lookAtNPCCoroutine);
        if(lookAtPlayerCoroutine != null) StopCoroutine(lookAtPlayerCoroutine);
        
        lookAtPlayerCoroutine = StartCoroutine(SmoothLookAt(transform, playerLogic.transform));
        lookAtNPCCoroutine = StartCoroutine(SmoothLookAt(playerLogic.transform, transform));

    }
    public void OnEnable()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.AddListener(OnDialogueEnded);
            dialogueRunner.onNodeComplete.AddListener(PlayNpcNodeSound);
        }
    }
    public void OnDisable()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueEnded);
            dialogueRunner.onNodeComplete.RemoveListener(PlayNpcNodeSound);
        }
    }
    private void PlayNpcNodeSound(string nodeName)
    {
        if (nodeName.Contains(npcName))
        {
            if (npcAudioSource != null && npcAudioClips.Length > 0)
            {
                // Stop any previous clip immediately to prevent the "cascade" overlap
                npcAudioSource.Stop();

                // Optional: Add a slight pitch variation to make it sound more natural
                npcAudioSource.pitch = Random.Range(0.85f, 1.15f);

                npcAudioSource.PlayOneShot(npcAudioClips[Random.Range(0, npcAudioClips.Length)]);
            }
        }
    }

    private void OnDialogueEnded()
    {
        StopLookingAtPlayer();

        string progressVarKey = "$"+npcName+"_progress";
        if(dialogueRunner.VariableStorage.TryGetValue(progressVarKey, out float progressValue))
        {
            int progressInt = (int)progressValue;
            PlayerPrefs.SetInt(npcName+"_progress", progressInt);
            PlayerPrefs.Save();
            Debug.Log($"Updated progress for {npcName}: {progressInt}");
        }
        string relationshipKey = "$"+npcName+"_relationship";
        if(dialogueRunner.VariableStorage.TryGetValue(relationshipKey, out float relationshipValue))
        {
            relationship = (int)relationshipValue;
            Debug.Log($"Updated relationship with {npcName}: {relationship}");
            
            PlayerPrefs.SetInt(npcName+"_relationship", relationship);
            PlayerPrefs.Save();
        }
        string lightKey = "$lightAdded";
        if(dialogueRunner.VariableStorage.TryGetValue(lightKey, out float lightAddedValue))
        {
            if(lightAddedValue != 0)
            {
                playerLogic.lightHeartLevel += (int)lightAddedValue;
                Debug.Log($"Adding {lightAddedValue} light to player from dialogue with {npcName}");
                Debug.Log($"Player's new light heart level: {playerLogic.lightHeartLevel}");
                
                PlayerPrefs.SetInt("lightHeartLevel", playerLogic.lightHeartLevel);
                PlayerPrefs.Save();
                
                lightAddedValue = 0;
                dialogueRunner.VariableStorage.SetValue(lightKey, lightAddedValue);
            }
        }
        
        PlayerPrefs.Save();
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
        if(npcRoaming != null)
            npcRoaming.ResumeNpcRoaming(); // Resume the NPC's roaming behavior when the dialogue is complete
    }
    public void StopLookingAtPlayer()
    {
        StartCoroutine(RotateBackToDefault());
        
    }

}
