using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System;
using System.Collections;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    [Header("Time settings")]
    public int dayNumber = 0;
    public float bedTimeThreshold = 0.85f; // Threshold for activating tired mode (e.g., 90% of the day)

    //[Header("Tiredness indicator")]
    private float pulseSpeed = 0.5f; // Speed of the pulsing effect
    private float minVignetteIntensity = 0.5f; // Minimum intensity of the vignette effect
    private float maxVignetteIntensity = 0.78f; // Maximum intensity of the vignette effect
    private float vignetteDefaultIntensity = 0.16f; // Default intensity of the vignette effect when not in tired mode

    [Header("Sleeping Transition")]
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject fadeOut;

    private DayCycle dayCycle;
    public Vignette vignetteEffect;
    private ThirdPersonController playerLogic;

    private float defaultPlayerSpeed =2f;
    private float tiredPlayerSpeed = 1f;
    private float defaultSprintSpeed = 5.335f;
    private float tiredSprintSpeed = 1f;

    public static bool isSleeping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fadeIn.SetActive(false);
        fadeOut.SetActive(false);

        playerLogic = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();

        dayCycle = GameObject.Find("Directional Light").GetComponent<DayCycle>();
        Volume volume = GameObject.Find("Volume Profile").GetComponent<Volume>();
        if (volume.profile.TryGet<Vignette>(out vignetteEffect))
        {
            vignetteEffect.intensity.value = vignetteDefaultIntensity; // Start with default vignette intensity
        }
        else
        {
            Debug.LogError("Vignette effect not found in Volume Profile.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dayCycle == null || vignetteEffect == null)
        {
            return;
        }
        if (dayCycle.GetDayProgress() >= bedTimeThreshold)
        {
            SetTiredMode();
        }
        else
        {
            if(vignetteEffect.intensity.value != vignetteDefaultIntensity)
            {
                vignetteEffect.intensity.value = Mathf.Lerp(vignetteEffect.intensity.value, vignetteDefaultIntensity, Time.deltaTime); // Reset to default intensity when not in tired mode
            }
        }
    }

    private void SetTiredMode()
    {
        float wave = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // Normalize to range [0, 1]
        float targetPulse = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, wave);
        vignetteEffect.intensity.value = Mathf.Lerp(vignetteEffect.intensity.value, targetPulse, 2f*Time.deltaTime); // Gradually increase vignette intensity for tired mode
        
        playerLogic.MoveSpeed = tiredPlayerSpeed;
        playerLogic.SprintSpeed = tiredSprintSpeed;
        // Implement tired mode logic here, such as reducing player speed, changing UI, etc.
        
    }
    public void GoingToSleep()
    {
               
        StartCoroutine(SleepCoroutine());
    }

    IEnumerator SleepCoroutine()
    {
        float timer = 0f;
        float duration = 2f; // Duration of the sleep transition
        float startIntensity = vignetteEffect.intensity.value;
        isSleeping = true;

        fadeOut.SetActive(false);

        while (timer<duration)
        {
            
            timer += Time.deltaTime;
            float t = timer / duration;
            vignetteEffect.intensity.value = Mathf.Lerp(startIntensity, 1f, t);// Intensify vignette effect for sleep transition
            yield return null; // Wait for the next frame
        }
        vignetteEffect.intensity.value = 1f; // Ensure vignette is fully intensified after the transition
        fadeIn.SetActive(true);

        // Implement sleep logic here, such as fading the screen, waiting for a few seconds, etc.
        yield return new WaitForSeconds(2f); // Simulate sleep duration
        
        fadeIn.SetActive(false);
        fadeOut.SetActive(true);

        playerLogic.MoveSpeed = defaultPlayerSpeed;
        playerLogic.SprintSpeed = defaultSprintSpeed;

        dayNumber++;
        Debug.Log("Day " + dayNumber);

        dayCycle.StartNewDay(); // Start a new day after sleeping
        timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            vignetteEffect.intensity.value = Mathf.Lerp(1f, vignetteDefaultIntensity, t); // Gradually reduce vignette intensity after waking up
            yield return null; // Wait for the next frame
        }
        vignetteEffect.intensity.value = vignetteDefaultIntensity; // Reset vignette intensity after waking up
        isSleeping = false;
    }

}
