using System.Collections;
using UnityEngine;

public class NpcOpenDoor : MonoBehaviour
{
    private Animator doorAnimator;
    private Coroutine currentDoorCoroutine;

    [SerializeField] private string npcTag = "npc"; // Editable in Inspector to prevent typos

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // 1. Check the Tag
        if (collision.CompareTag(npcTag))
        {
            // 2. Logic: If we are already running the routine, stop it so we can restart the timer
            if (currentDoorCoroutine != null)
            {
                StopCoroutine(currentDoorCoroutine);
            }

            // 3. Start the routine and save the reference
            currentDoorCoroutine = StartCoroutine(OpenDoorRoutine());
        }
    }

    private IEnumerator OpenDoorRoutine()
    {
        // Open
        doorAnimator.SetBool("isOpen", true);

        // Wait
        yield return new WaitForSeconds(3f);

        // Close
        doorAnimator.SetBool("isOpen", false);

        // Important: Reset the reference so we know we are finished
        currentDoorCoroutine = null;
    }
}