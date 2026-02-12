using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private PlayerLogic playerLogic; // Reference to get the closest interactable

    [Header("Gesture Settings")]
    private int gestureLayerIndex;
    private Coroutine currentGestureCoroutine;

    [Header("Procedural Look Settings")]
    [SerializeField] private Transform headBone; // DRAG YOUR NECK/HEAD BONE HERE
    [SerializeField] private float lookSpeed = 5f;       // How fast the head turns
    [SerializeField] private float maxLookAngle = 70f;   // Owl prevention (70 degrees max)
    [SerializeField] private Vector3 lookOffset = new Vector3(0, -0.5f, 0); // Adjust to look at object center

    // Internal State
    private float currentLookWeight = 0f; // 0 = Animation only, 1 = Full Look at Target

    void Start()
    {
        animator = GetComponent<Animator>();
        playerLogic = GetComponent<PlayerLogic>(); // Grab the logic script
        
        // --- Setup Gestures ---
        gestureLayerIndex = animator.GetLayerIndex("Gestures");
        if (gestureLayerIndex == -1)
        {
            Debug.LogError("Gestures layer not found! Please check Animator.");
        }
        else
        {
            animator.SetLayerWeight(gestureLayerIndex, 0);
        }
    }

    void Update()
    {
        // ... (Your Input Code for T and F) ...
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (currentGestureCoroutine != null) StopCoroutine(currentGestureCoroutine);
            currentGestureCoroutine = StartCoroutine(PlayGesture("isAgreed", "agreeing_anim"));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentGestureCoroutine != null) StopCoroutine(currentGestureCoroutine);
            currentGestureCoroutine = StartCoroutine(PlayGesture("isAngryPointing", "angryPoint_anim"));
        }
    }

    // --- PROCEDURAL LOOK LOGIC ---
    // Runs AFTER the animation has finished for the frame
    private void LateUpdate()
    {
        if (headBone == null || playerLogic == null) return;

        if(playerLogic.closestObject == null)
        {
            // No target, smoothly return to animation pose
            currentLookWeight = Mathf.Lerp(currentLookWeight, 0f, Time.deltaTime * lookSpeed);
            headBone.localRotation = Quaternion.Slerp(headBone.localRotation, Quaternion.identity, currentLookWeight);
            return;
        }
        Transform target = playerLogic.closestObject.transform;
        float targetWeight = 0f;

        // 1. Determine if we should look
        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;

            // Calculate Angle between Player Body Forward and Target
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            // "Owl Prevention": Only look if within 70 degrees
            if (angle < maxLookAngle)
            {
                targetWeight = 1f;
            }
        }

        // 2. Smoothly blend the weight (prevents snapping)
        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * lookSpeed);

        // 3. Apply the Rotation Override
        if (currentLookWeight > 0.01f)
        {
            RotateHeadTowards(target, currentLookWeight);
        }
    }

    private void RotateHeadTowards(Transform target, float weight)
    {
        // A. Capture the rotation the animation WANTS to be at this frame
        Quaternion animationRotation = headBone.rotation;

        // B. Calculate the rotation we WANT to be at
        Vector3 direction = (target.position + lookOffset) - headBone.position;

        // Flatten direction so we ONLY rotate on Y-Axis (Look Left/Right, not Up/Down)
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // C. Correct for bone orientation if needed
            // Sometimes bones are rotated -90 degrees. If your head snaps sideways, 
            // uncomment the line below and adjust the vector (e.g., Vector3.right or Vector3.up)
            // targetRotation *= Quaternion.Euler(0, 90, 0); 

            // D. Blend: Start at Animation Rotation -> Move towards Target Rotation
            // We use Slerp with our smoothed weight
            headBone.rotation = Quaternion.Slerp(animationRotation, targetRotation, weight);
        }
    }

    // ... (Your PlayGesture Coroutine remains unchanged) ...
    private IEnumerator PlayGesture(string paramterName, string animName)
    {
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 1);
        animator.SetBool(paramterName, true);
        animator.CrossFadeInFixedTime(animName, 0.1f, gestureLayerIndex, 0f);
        yield return null;

        float duration;
        if (animator.IsInTransition(gestureLayerIndex))
            duration = animator.GetNextAnimatorStateInfo(gestureLayerIndex).length;
        else
            duration = animator.GetCurrentAnimatorStateInfo(gestureLayerIndex).length;

        if (duration <= 0) duration = 2.0f;

        float blendOutTime = 0.4f;
        yield return new WaitForSeconds(Mathf.Max(0, duration - blendOutTime));

        float timer = 0f;
        while (timer < blendOutTime)
        {
            timer += Time.deltaTime;
            float newWeight = Mathf.Lerp(1f, 0f, timer / blendOutTime);
            animator.SetLayerWeight(gestureLayerIndex, newWeight);
            yield return null;
        }

        animator.SetLayerWeight(gestureLayerIndex, 0);
        animator.SetBool(paramterName, false);
        currentGestureCoroutine = null;
    }
}