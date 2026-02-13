using UnityEngine;
using System.Collections;

public class NPCVisual : MonoBehaviour
{
    private Animator animator;
    private int gestureLayerIndex;
    private Coroutine currentGestureCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Ensure we have the layer
        gestureLayerIndex = animator.GetLayerIndex("Gestures");
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 0);
    }

    // This function is called by the Manager
    public void PlayNamedGesture(string animName)
    {
        // Map names to triggers (Reuse the same logic as Player for consistency)
        string triggerName = "";

        // Use the same names as your Player Visual for simplicity
        switch (animName)
        {
            case "agree": triggerName = "isAgreed"; break;
            case "nod": triggerName = "isNoding"; break;
            case "wave": triggerName = "isWaving"; break;
            case "deny": triggerName = "isDenying"; break;
            case "dismiss": triggerName = "isDismissing"; break;
            case "cocky": triggerName = "isCocky"; break; // Maybe he scoffs at you
            case "angry": triggerName = "isAngryPointing"; break;
            case "offer": triggerName = "isOffering"; break;
                // Add more as needed
        }

        if (triggerName != "")
        {
            if (currentGestureCoroutine != null) StopCoroutine(currentGestureCoroutine);

            // Assuming the animation state name follows the pattern "trigger_anim" 
            // OR you can use the exact state names from your Player controller if you copied it.
            // Let's assume you copied the Player Animator, so the state names are:
            // "agreeing_anim", "headNod_anim", etc.

            string stateName = GetStateNameFromTrigger(triggerName);
            currentGestureCoroutine = StartCoroutine(PlayGestureRoutine(triggerName, stateName));
        }
    }

    // Helper to map Trigger -> Animation State Name (Must match your Animator)
    private string GetStateNameFromTrigger(string trigger)
    {
        if (trigger == "isAgreed") return "agreeing_anim";
        if (trigger == "isNoding") return "headNod_anim";
        if (trigger == "isWaving") return "waving_anim";
        if (trigger == "isDenying") return "denying_anim";
        if (trigger == "isDismissing") return "dismis_anim";
        if (trigger == "isCocky") return "cocky_anim";
        if (trigger == "isAngryPointing") return "angryPoint_anim";
        if (trigger == "isOffering") return "isOffering_anim"; // Check your spelling
        return "";
    }

    private IEnumerator PlayGestureRoutine(string triggerName, string stateName)
    {
        if (gestureLayerIndex != -1) animator.SetLayerWeight(gestureLayerIndex, 1);
        animator.SetBool(triggerName, true);
        animator.CrossFadeInFixedTime(stateName, 0.1f, gestureLayerIndex, 0f);

        yield return null; // Wait for transition

        float duration = 2f; // Default safety
        if (animator.IsInTransition(gestureLayerIndex))
            duration = animator.GetNextAnimatorStateInfo(gestureLayerIndex).length;
        else
            duration = animator.GetCurrentAnimatorStateInfo(gestureLayerIndex).length;

        float blendOutTime = 0.4f;
        yield return new WaitForSeconds(Mathf.Max(0, duration - blendOutTime));

        float timer = 0f;
        while (timer < blendOutTime)
        {
            timer += Time.deltaTime;
            animator.SetLayerWeight(gestureLayerIndex, Mathf.Lerp(1f, 0f, timer / blendOutTime));
            yield return null;
        }

        animator.SetLayerWeight(gestureLayerIndex, 0);
        animator.SetBool(triggerName, false);
        currentGestureCoroutine = null;
    }
}