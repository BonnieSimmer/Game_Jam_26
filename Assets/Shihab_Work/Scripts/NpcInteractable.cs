using UnityEngine;
using Yarn.Unity;
public class NpcInteractable : InteractableLogic
{
    public string npcName;
    public DialogueRunner dialogueRunner; // Reference to the DialogueRunner component for handling dialogues

    private int relationshipLevel = 0; // Example variable to track relationship level with the NPC
    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        dialogueRunner = GameObject.Find("Dialogue System").GetComponent<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        int currentDay = gameManager.dayNumber;
        if (npcName == "oldManNPC")
        {
            dialogueRunner.StartDialogue("oldManNPC_stage"+currentDay.ToString());

        }
    }

}
