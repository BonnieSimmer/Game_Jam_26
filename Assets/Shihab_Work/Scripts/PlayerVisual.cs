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

    [Header("References")]
    public DialogueRunner dialogueRunner;

    // --- NEW: DIRECT REFERENCE ---
    [Tooltip("Drag Shiekh_Khaled here to fix the 'Not Found' error")]
    public NPCVisual mainNPC;
    // -----------------------------

    // Internal State
    private float currentLookWeight = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerLogic = GetComponent<PlayerLogic>();

        gestureLayerIndex = animator.GetLayerIndex("Gestures");
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 0);

        if (dialogueRunner != null)
        {
            // 1. REGISTER PLAYER GESTURES
            dialogueRunner.AddCommandHandler<string>("gesture", (gestureName) => {
                if (currentGestureCoroutine != null) StopCoroutine(currentGestureCoroutine);

                string triggerName = "";
                string animationStateName = "";

                switch (gestureName)
                {
                    case "agree": triggerName = "isAgreed"; animationStateName = "agreeing_anim"; break;
                    case "nod": triggerName = "isNoding"; animationStateName = "headNod_anim"; break;
                    case "wave": triggerName = "isWaving"; animationStateName = "waving_anim"; break;
                    case "deny": triggerName = "isDenying"; animationStateName = "denying_anim"; break;
                    case "dismiss": triggerName = "isDismissing"; animationStateName = "dismis_anim"; break;
                    case "lookAway": triggerName = "isLookingAway"; animationStateName = "lookAway_anim"; break;
                    case "shrug": triggerName = "isShrugging"; animationStateName = "shrugging_anim"; break;
                    case "cocky": triggerName = "isCocky"; animationStateName = "cocky_anim"; break;
                    case "sarcastic": triggerName = "isSarcastic"; animationStateName = "sarcastic_anim"; break;
                    case "pout": triggerName = "isPouting"; animationStateName = "pouting_anim"; break;
                    case "angry": triggerName = "isAngryPointing"; animationStateName = "angryPoint_anim"; break;
                    case "mad": triggerName = "isMad"; animationStateName = "angryFists_anim"; break;
                }

                if (triggerName != "" && animationStateName != "")
                {
                    currentGestureCoroutine = StartCoroutine(PlayGesture(triggerName, animationStateName));
                }
            });

            // 2. REGISTER NPC GESTURES (The Fix)
            dialogueRunner.AddCommandHandler<string, string>("npc_gesture", (npcName, animName) => {

                NPCVisual targetVisual = null;

                // A. Check the Manual Reference first (Fail-proof)
                if (mainNPC != null && (npcName == "Shiekh_Khaled" || mainNPC.name == npcName))
                {
                    targetVisual = mainNPC;
                }
                // B. If not manually assigned, try finding it (Fallback)
                else
                {
                    GameObject npcObj = GameObject.Find(npcName);
                    if (npcObj != null) targetVisual = npcObj.GetComponent<NPCVisual>();
                }

                // C. Execute
                if (targetVisual != null)
                {
                    targetVisual.PlayNamedGesture(animName);
                }
                else
                {
                    Debug.LogWarning($"Still could not find NPC '{npcName}'. Did you drag him into the PlayerVisual 'Main NPC' slot?");
                }
            });
        }
    }

    private void LateUpdate()
    {
        if (headBone == null || playerLogic == null) return;

        Transform target = null;
        if (playerLogic.closestObject != null)
        {
            target = playerLogic.closestObject.transform;
        }

        float targetWeight = 0f;

        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle < maxLookAngle) targetWeight = 1f;
        }

        currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * lookSpeed);

        if (currentLookWeight > 0.01f && target != null)
        {
            RotateHeadTowards(target, currentLookWeight);
        }
    }

    private void RotateHeadTowards(Transform target, float weight)
    {
        Quaternion animationRotation = headBone.rotation;
        Vector3 direction = (target.position + lookOffset) - headBone.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            headBone.rotation = Quaternion.Slerp(animationRotation, targetRotation, weight);
        }
    }

    private IEnumerator PlayGesture(string triggerName, string animStateName)
    {
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 1);
        animator.SetBool(triggerName, true);
        animator.CrossFadeInFixedTime(animStateName, 0.1f, gestureLayerIndex, 0f);
        yield return null;

        float duration = 2.0f;
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

    public void SetTiredState(bool state)
    {
        if (animator != null) animator.SetBool("isTired", state);
    }
}