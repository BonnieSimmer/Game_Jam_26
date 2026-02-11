using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    
    private Animator animator;
    private int gestureLayerIndex;

    private Coroutine currentGestureCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        gestureLayerIndex = animator.GetLayerIndex("Gestures");

        if(gestureLayerIndex == -1)
        {
            Debug.LogError("Gestures layer not found in the Animator. Please ensure there is a layer named 'Gestures' in the Animator.");
        }
        else
        {
            animator.SetLayerWeight(gestureLayerIndex, 0);
            animator.SetLayerWeight(animator.GetLayerIndex("Base Layer"), 1);

        }

        //animator.SetLayerWeight(animator.GetLayerIndex("Facial Expressions Layer"), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(currentGestureCoroutine != null)
            {
                StopCoroutine(currentGestureCoroutine);
                
            }
             
            currentGestureCoroutine = StartCoroutine(PlayGesture("isAgreed", "agreeing_anim"));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentGestureCoroutine != null)
            {
                StopCoroutine(currentGestureCoroutine);

            }

            currentGestureCoroutine = StartCoroutine(PlayGesture("isAngryPointing", "angryPoint_anim"));
        }
    }

    private IEnumerator PlayGesture(string paramterName, string animName)
    {
        if(gestureLayerIndex != -1)
        {
            animator.SetLayerWeight(gestureLayerIndex, 1);
        }

        
        animator.SetBool(paramterName, true);
        // animeName must match the name of the animation box in your Animator Window.
        // 0.1f = Transition duration (smooth blend)
        // 0f = Normalized Time (0.0 is the start, 1.0 is the end)
        animator.CrossFadeInFixedTime(animName, 0.1f, gestureLayerIndex, 0f);
        yield return null; // Wait 1 frame for the 'True' to register and trigger the animation

        float duration;
        if (animator.IsInTransition(gestureLayerIndex))
        {
            // Get the length of the TARGET state (The Agree Animation)
            duration = animator.GetNextAnimatorStateInfo(gestureLayerIndex).length;
        }
        else
        {
            // If the transition happened instantly (rare, but possible), get the CURRENT state
            duration = animator.GetCurrentAnimatorStateInfo(gestureLayerIndex).length;
        }
        if (duration <= 0) duration = 2.0f;
        
       
        float blendOutTime = 0.4f;

        // 6. Wait for the animation almost to the end (Duration - Blend Time)
        // Mathf.Max ensures we don't wait a negative number if the animation is super short
        yield return new WaitForSeconds(Mathf.Max(0, duration - blendOutTime));

        // 7. Smoothly Fade Out the Weight
        float timer = 0f;
        while (timer < blendOutTime)
        {
            timer += Time.deltaTime;
            // Calculate weight from 1.0 down to 0.0
            float newWeight = Mathf.Lerp(1f, 0f, timer / blendOutTime);
            animator.SetLayerWeight(gestureLayerIndex, newWeight);
            yield return null;
        }

        // 8. Ensure it is fully off and reset
        animator.SetLayerWeight(gestureLayerIndex, 0);
        animator.SetBool(paramterName, false);

        currentGestureCoroutine = null; // Clear the reference to indicate the gesture has finished
    }
}
