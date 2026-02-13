using UnityEngine;
using System.Collections;

public class NPCVisual : MonoBehaviour
{
    private Animator animator;
    private int gestureLayerIndex;
    private Coroutine currentGestureCoroutine;

    [Header("Procedural Look Settings")]
    [SerializeField] private Transform headBone;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float maxLookAngle = 60f;
    [SerializeField] private Vector3 lookOffset = new Vector3(0, -0.5f, 0);
    [SerializeField] private float lookDistance = 4f;

    [Header("Rotation Fixer")]
    // CHANGE THIS if he looks at the sky. Try (90, 0, 0) or (-90, 0, 0)
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    // Internal State
    private Transform playerTransform;
    private float currentLookWeight = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();

        // 1. Setup Gestures Layer
        gestureLayerIndex = animator.GetLayerIndex("Gestures");
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 0);

        // 2. Find Player for Looking
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

    }

    // --- PROCEDURAL LOOK LOGIC ---
    private void LateUpdate()
    {
        if (headBone == null || playerTransform == null) return;

        float targetWeight = 0f;

        // 1. Check Distance & Angle
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= lookDistance)
        {
            // Calculate direction to player
            Vector3 directionToPlayer = (playerTransform.position + lookOffset) - headBone.position;

            // Flatten direction so he doesn't break his neck looking up/down too much
            // directionToPlayer.y = 0; 

            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < maxLookAngle)
            {
                targetWeight = 1f;
            }
        }

        // 2. Smoothly Blend Weight
        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * lookSpeed);

        // 3. Apply Rotation
        if (currentLookWeight > 0.01f)
        {
            RotateHeadTowards(playerTransform, currentLookWeight);
        }
    }

    private void RotateHeadTowards(Transform target, float weight)
    {
        Quaternion animationRotation = headBone.rotation;

        Vector3 direction = (target.position + lookOffset) - headBone.position;
        // direction.y = 0; // Commented out to allow looking up/down slightly

        if (direction != Vector3.zero)
        {
            // 1. Calculate the base look rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 2. Apply the Manual Offset (The Fix for Sky Looking)
            // This adds your custom rotation on top of the look rotation
            targetRotation *= Quaternion.Euler(rotationOffset);

            // 3. Blend
            headBone.rotation = Quaternion.Slerp(animationRotation, targetRotation, weight);
        }
    }

    // --- GESTURE LOGIC ---
    public void PlayNamedGesture(string animName)
    {
        string triggerName = "";
        string stateName = "";

        // Map simple Yarn commands to Animation Triggers/States
        switch (animName)
        {
            case "agree": triggerName = "isAgreed"; stateName = "agreeing_anim"; break;
            case "nod": triggerName = "isNoding"; stateName = "headNod_anim"; break;
            case "wave": triggerName = "isWaving"; stateName = "waving_anim"; break;
            case "deny": triggerName = "isDenying"; stateName = "denying_anim"; break;
            case "dismiss": triggerName = "isDismissing"; stateName = "dismis_anim"; break;
            case "cocky": triggerName = "isCocky"; stateName = "cocky_anim"; break;
            case "angry": triggerName = "isAngryPointing"; stateName = "angryPoint_anim"; break;
            case "offer": triggerName = "isOffering"; stateName = "isOffering_anim"; break;
        }

        if (triggerName != "")
        {
            if (currentGestureCoroutine != null) StopCoroutine(currentGestureCoroutine);
            currentGestureCoroutine = StartCoroutine(PlayGestureRoutine(triggerName, stateName));
        }
    }

    private IEnumerator PlayGestureRoutine(string triggerName, string stateName)
    {
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 1);
        animator.SetBool(triggerName, true);
        animator.CrossFadeInFixedTime(stateName, 0.1f, gestureLayerIndex, 0f);

        yield return null;

        float duration = 2f;
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
        animator.SetBool(triggerName, false);
        currentGestureCoroutine = null;
    }
}