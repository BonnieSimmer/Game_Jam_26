using System.Collections;
using UnityEngine;

public class NpcOpenDoor : MonoBehaviour
{
    private Animator doorAnimator;
    private bool isDoorOpen = false;
    private Coroutine openDoorByNPC;
    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("npc"))
        {
            if(openDoorByNPC == null)
            {
                openDoorByNPC = StartCoroutine(openDoor());
            }
            else
            {
                StopCoroutine(openDoorByNPC);
            }
        }
    }

    private IEnumerator openDoor()
    {
        isDoorOpen=true;
        doorAnimator.SetBool("isOpen", isDoorOpen);
        yield return new WaitForSeconds(3f); // Keep the door open for 3 seconds
        isDoorOpen = false;
        doorAnimator.SetBool("isOpen", isDoorOpen);
    }
}
