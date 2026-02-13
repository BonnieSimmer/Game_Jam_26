using System.Collections;
using UnityEngine;
using Yarn.Unity;
public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private PlayerLogic playerLogic;

    [Header("Gesture Settings")]
    private int gestureLayerIndex;
    private Coroutine currentGestureCoroutine;

    [Header("Procedural Look Settings")]
    [SerializeField] private Transform headBone;
    [SerializeField] private float lookSpeed = 5f;
    [SerializeField] private float maxLookAngle = 70f;
    [SerializeField] private Vector3 lookOffset = new Vector3(0, -0.5f, 0);

    public DialogueRunner dialogueRunner; // Reference to the DialogueRunner component for handling dialogues

    // Internal State
    private float currentLookWeight = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerLogic = GetComponent<PlayerLogic>();

        gestureLayerIndex = animator.GetLayerIndex("Gestures");
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 0);

        if(dialogueRunner != null)
        {
            // Register the command "gesture" to call your coroutine logic
            dialogueRunner.AddCommandHandler<string>("gesture", (animName) => {
                if (currentGestureCoroutine != null) StopCoroutine(currentGestureCoroutine);
                // Map simple names to animation triggers
                string triggerName = "";
                if (animName == "agreeing") triggerName = "isAgreed";
                if (animName == "angryPoint") triggerName = "isAngryPointing";
                if (animName == "anxious") triggerName = "isAnxious";
                if (animName == "offering") triggerName = "isOffering";

                // You might need to tweak your Coroutine to take just the trigger name
                // or pass both if your setup requires it.
                currentGestureCoroutine = StartCoroutine(PlayGesture(triggerName, animName + "_anim"));
            });
        }
    }

    void Update()
    {
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

    private void LateUpdate()
    {
        // 1. Safety Checks
        if (headBone == null || playerLogic == null) return;

        // 2. Determine Target
        Transform target = null;
        if (playerLogic.closestObject != null)
        {
            target = playerLogic.closestObject.transform;
        }

        float targetWeight = 0f;

        // 3. Check Angle (Owl Prevention)
        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            // If target exists AND is in front of us, we want full weight
            if (angle < maxLookAngle)
            {
                targetWeight = 1f;
            }
        }
        // If target is null OR angle is too big, targetWeight stays 0f

        // 4. Smoothly Blend Weight
        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * lookSpeed);

        // 5. Apply Rotation (ONLY if weight is significant)
        // If weight is near 0, we do nothing, letting the Animator control the head.
        if (currentLookWeight > 0.01f && target != null)
        {
            RotateHeadTowards(target, currentLookWeight);
        }
    }

    private void RotateHeadTowards(Transform target, float weight)
    {
        // A. Get the rotation the animation WANTS right now
        Quaternion animationRotation = headBone.rotation;

        // B. Calculate where we WANT to look
        Vector3 direction = (target.position + lookOffset) - headBone.position;
        direction.y = 0; // Keep head level (yaw only)

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // C. Blend between Animation and Target
            headBone.rotation = Quaternion.Slerp(animationRotation, targetRotation, weight);
        }
    }

    // ... (PlayGesture Coroutine remains unchanged) ...
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

    public void SetTiredState(bool state)
    {
        if (animator != null)
        {
            animator.SetBool("isTired", state);
        }
    }
}