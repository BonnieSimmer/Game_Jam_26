using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcRoaming : MonoBehaviour
{
    [Header("Go to locations")]
    [SerializeField] private Transform[] targetLocations;

    private NavMeshAgent agent;
    private Animator animator;
    [Header("Stopping duration boundaries")]
    public float minWaitTime = 2f; // Time to wait at each location
    public float maxWaitTime = 3f;


    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] stepsClips;
    [SerializeField] private AudioClip landClip;
    private AudioSource npcSRC;

    
    private float turnSpeed = 5f;

    private Coroutine roamingCoroutine;
    private bool isInteracting = false; // Flag to check if the NPC is currently interacting with the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcSRC = GetComponent<AudioSource>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.autoBraking = true;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        roamingCoroutine = StartCoroutine(RoamingLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteracting) return;


        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            direction.y = 0; // Keep flat
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            }
        }

        float moveSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", moveSpeed);
    }
    public void PauseNpcRoaming()
    {
        
        isInteracting = true;

        agent.velocity = Vector3.zero; // Stop movement immediately
        animator.SetFloat("Speed", 0); // Set animation to idle

        if (roamingCoroutine != null)
            StopCoroutine(roamingCoroutine);
        roamingCoroutine = null;

        agent.ResetPath(); // Clear the current path to prevent unintended movement when resuming
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // Ensure velocity is zero to stop movement immediately
        animator.SetFloat("Speed", 0); // Set animation to idle

    }

    public void ResumeNpcRoaming()
    {
        agent.isStopped = false;
        isInteracting = false;

        if (roamingCoroutine != null)
            StopCoroutine(roamingCoroutine);
        roamingCoroutine = StartCoroutine(RoamingLoop());
    }

    private IEnumerator RoamingLoop()
    {
        if (targetLocations.Length == 0) yield break;

        while (true)
        {
            
            animator.SetFloat("Speed", 0);
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Ensure velocity is zero to stop movement immediately

            float waitDuration = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitDuration);

            Transform target = targetLocations[Random.Range(0, targetLocations.Length)];
            agent.SetDestination(target.position);

            agent.isStopped = true;
            while(agent.pathPending)
            {
                yield return null; // Wait until the path is calculated
            }

            // 3. ROTATE TO FACE TARGET (The "Rotate First" Logic)
            Vector3 nextPathPoint = agent.steeringTarget;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0; // Flatten

            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                // Loop until we are facing roughly the right way (within 5 degrees)
                while (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
                {
                    directionToTarget = (agent.steeringTarget - transform.position).normalized;
                    directionToTarget.y = 0;
                    if (directionToTarget != Vector3.zero)
                        targetRotation = Quaternion.LookRotation(directionToTarget);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    yield return null;
                }
            }

            agent.isStopped = false;
            
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                if(isInteracting) yield break; // Exit if we start interacting with the player
                yield return null;
            }

            

        }

    }


    // This function catches the "OnFootstep" event so Unity stops complaining.
    // If you want sound later, put code here!
    public void OnFootstep(AnimationEvent animationEvent)
    {
        if (stepsClips.Length > 0)
        {
            int clipIndex = Random.Range(0, stepsClips.Length);
            AudioClip clipToPlay = stepsClips[clipIndex];
            npcSRC.PlayOneShot(clipToPlay);
        }
        // Do nothing for now (silence the error)
    }

    // Sometimes there is also an "OnLand" event in the jump animation
    public void OnLand(AnimationEvent animationEvent)
    {
        if (landClip != null)
            npcSRC.PlayOneShot(landClip);
        // Do nothing
    }
}
