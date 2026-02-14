using UnityEngine;
using System.Collections;

public class platformLogic : InteractableLogic
{
    [Header("Elevator Doors")]
    [SerializeField] private Transform groundDoor;
    [SerializeField] private Transform firstFloorDoor;

    [Header("Components")]
    [SerializeField] private AudioSource elevatorMusic;
    [SerializeField] private AudioClip elevatorSound;

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
        base.Start();

        if (groundDoor) gDoorDefaultScale = groundDoor.localScale;
        if (firstFloorDoor) fDoorDefaultScale = firstFloorDoor.localScale;

        if (elevatorMusic != null)
        {
            elevatorMusic.loop = true;           // Satisfies "loop it once it ends"
            elevatorMusic.spatialBlend = 1f;     // Force 3D sound

            // NEW: Distance settings to satisfy "don't want it to reach me on the roof"
            elevatorMusic.rolloffMode = AudioRolloffMode.Linear;
            elevatorMusic.minDistance = 1f;      // Loudest when standing IN the elevator
            elevatorMusic.maxDistance = 8f;      // Completely silent if 8 meters away (adjust as needed)

            if (!elevatorMusic.isPlaying)
            {
                elevatorMusic.PlayOneShot(elevatorSound, 0.1f);
            }
        }
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

        // REMOVED: elevatorMusic.Play(); 
        // Reason: The music is already playing from Start(). Calling Play() here would 
        // restart the song from 0:00 every time you move, which sounds glitchy.

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