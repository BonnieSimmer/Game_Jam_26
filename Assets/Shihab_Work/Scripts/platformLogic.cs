using UnityEngine;
using System.Collections;

public class platformLogic : InteractableLogic
{
    [Header("Elevator Doors")]
    [SerializeField] private Transform groundDoor;
    [SerializeField] private Transform firstFloorDoor;

    [Header("Components")]
    [SerializeField] private AudioSource elevatorMusic;

    [Header("Floor Markers")]
    [SerializeField] private Transform groundFloorPos;
    [SerializeField] private Transform firstFloorPos;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float doorSpeed = 2f;
    [SerializeField] private float openedZScale = 0.05f;

    // Door State Tracking
    private Vector3 gDoorDefaultScale;
    private Vector3 fDoorDefaultScale;
    private bool isMoving = false;

    public override void Start()
    {
        base.Start(); // Keeps the interactable setup from your base class

        // Capture the "Full" size of the doors at the start
        if (groundDoor) gDoorDefaultScale = groundDoor.localScale;
        if (firstFloorDoor) fDoorDefaultScale = firstFloorDoor.localScale;
        elevatorMusic.Play();
    }

    // This replaces OnTriggerEnter
    public override void Interact()
    {
        if (isMoving) return;

        base.Interact();

        // Determine target floor based on current position
        float distToGround = Vector3.Distance(transform.position, groundFloorPos.position);
        Transform target = (distToGround < 0.5f) ? firstFloorPos : groundFloorPos;

        StartCoroutine(ElevatorSequence(target));
    }

    public void CallElevator(Transform targetFloor)
    {
        if (!isMoving)
        {
            if (Vector3.Distance(transform.position, targetFloor.position) > 0.1f)
            {
                StartCoroutine(ElevatorSequence(targetFloor));
            }
            else
            {
                // Already here, just ensure the door is open
                StartCoroutine(AnimateDoor(GetCurrentDoor(), true));
            }
        }
    }

    private IEnumerator ElevatorSequence(Transform targetFloor)
    {
        isMoving = true;
        Transform currentDoor = GetCurrentDoor();
        Transform destinationDoor = (targetFloor == groundFloorPos) ? groundDoor : firstFloorDoor;

        // Start Music (assuming player is on platform since they interacted)
        if (elevatorMusic != null) elevatorMusic.Play();

        // 1. CLOSE DOOR (Scale back to Default)
        yield return StartCoroutine(AnimateDoor(currentDoor, false));

        // 2. MOVE PLATFORM
        Vector3 endPos = new Vector3(transform.position.x, targetFloor.position.y, transform.position.z);
        while (Vector3.Distance(transform.position, endPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = endPos;

        //if (elevatorMusic != null) elevatorMusic.Stop();

        // 3. OPEN DOOR (Scale down to 0.05)
        yield return StartCoroutine(AnimateDoor(destinationDoor, true));

        isMoving = false;
    }

    private Transform GetCurrentDoor()
    {
        float distToGround = Vector3.Distance(transform.position, groundFloorPos.position);
        return (distToGround < 0.5f) ? groundDoor : firstFloorDoor;
    }

    private IEnumerator AnimateDoor(Transform door, bool opening)
    {
        if (door == null) yield break;

        Vector3 defaultScale = (door == groundDoor) ? gDoorDefaultScale : fDoorDefaultScale;

        // Target Z is 0.05 when opening, and the original default scale when closing
        float targetZ = opening ? openedZScale : defaultScale.z;
        Vector3 targetScale = new Vector3(defaultScale.x, defaultScale.y, targetZ);

        float t = 0;
        Vector3 startScale = door.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime * doorSpeed;
            door.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        door.localScale = targetScale;

        // Toggle Collider so player can walk through
        if (door.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = !opening;
        }
    }
}