using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System;
using System.Collections;
using StarterAssets;
using UnityEngine.SceneManagement;

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
    [SerializeField] private CanvasGroup fadeOutCanvasGroup;
    [Header("Testing")]
    public bool isPlayTesting = true;

    string mainSceneName = "IndoorsScene";

    private DayCycle dayCycle;
    public Vignette vignetteEffect;
    private ThirdPersonController playerLogic;

    private float defaultPlayerSpeed =2f;
    private float tiredPlayerSpeed = 1f;
    private float defaultSprintSpeed = 5.335f;
    private float tiredSprintSpeed = 1f;

    public static bool isSleeping = false;
    private bool isWakingUp = false;

    public static GameManager Instance;

    private void Awake()
    {
        // This ensures only ONE GameManager exists and it survives scene changes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        
        LoadData();

        RefreshReferences(); // Find and assign references to UI elements, DayCycle, Vignette, etc.

        if (isPlayTesting)
        {
            Debug.LogWarning("Play testing mode is ON. All saved data will be cleared.");
            PlayerPrefs.DeleteAll(); // Clear all saved data (for testing purposes, remove this line in production)
            PlayerPrefs.Save();
            Debug.Log("Saved data cleared. Starting fresh for play testing.");
            Debug.Log("go to GameManager line 66 to prevent data deletion"); // Verify that the day number is reset to 0
        }

        if(SceneManager.GetActiveScene().name == mainSceneName)
        {

            StartCoroutine(WakeUpCoroutine()); // Start the wake-up transition at the beginning of the game
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dayCycle == null || vignetteEffect == null)
        {
            return;
        }
        if(isSleeping || isWakingUp)
        {
            return; // Skip tired mode logic during sleeping or waking up transitions
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
    private void RefreshReferences()
    {
        // A. Find UI (Robust method)
        GameObject canvas = GameObject.Find("UI_Canvas");
        if (canvas != null)
        {
            // Note: Caps Sensitive! Matches your Hierarchy
            Transform inTrans = canvas.transform.Find("FadeIN");
            Transform outTrans = canvas.transform.Find("FadeOUT");

            if (inTrans != null)
            {
                fadeIn = inTrans.gameObject;
                fadeIn.SetActive(false); // Reset to hidden
            }
            if (outTrans != null)
            {
                fadeOut = outTrans.gameObject;
                fadeOut.SetActive(false); // Reset to hidden
            }
        }

        // B. Find Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerLogic = player.GetComponent<ThirdPersonController>();

        // C. Find DayCycle
        GameObject lightObj = GameObject.Find("Directional Light");
        if (lightObj != null) dayCycle = lightObj.GetComponent<DayCycle>();

        // D. Find Vignette
        GameObject volObj = GameObject.Find("Volume Profile");
        if (volObj != null)
        {
            Volume volume = volObj.GetComponent<Volume>();
            if (volume.profile.TryGet<Vignette>(out vignetteEffect))
            {
                vignetteEffect.intensity.value = vignetteDefaultIntensity;
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
        NightmareManager.Instance.SetLevelData(10 + dayNumber * 5, dayNumber + 1, 120, SceneManager.GetActiveScene().name);
        StartCoroutine(SleepCoroutine());
    }

    IEnumerator SleepCoroutine()
    {
        float timer = 0f;
        float duration = 2f; // Duration of the sleep transition
        float startIntensity = vignetteEffect.intensity.value;
        isSleeping = true;

        if(fadeOut !=null)fadeOut.SetActive(false);

        while (timer<duration)
        {
            
            timer += Time.deltaTime;
            float t = timer / duration;
            vignetteEffect.intensity.value = Mathf.Lerp(startIntensity, 1f, t);// Intensify vignette effect for sleep transition
            yield return null; // Wait for the next frame
        }
        vignetteEffect.intensity.value = 1f; // Ensure vignette is fully intensified after the transition
        if(fadeIn!=null)fadeIn.SetActive(true);

        SaveData();
        // Implement sleep logic here, such as fading the screen, waiting for a few seconds, etc.
        yield return new WaitForSeconds(2f); // Simulate sleep duration
        
        SceneManager.LoadSceneAsync("Nour_work/Scenes/Scene_Dream"); // Load the next scene after sleeping)
        
    }

    IEnumerator WakeUpCoroutine()
    {
        isWakingUp = true;
        isSleeping = false;

        float timer = 0f;
        float duration = 2f; // Duration of the sleep transition
        float startIntensity = vignetteEffect.intensity.value;

        if(fadeIn !=null)fadeIn.SetActive(false);
        if(fadeOut !=null)fadeOut.SetActive(true);
        
        if(playerLogic != null)
        {
            playerLogic.MoveSpeed = defaultPlayerSpeed;
            playerLogic.SprintSpeed = defaultSprintSpeed;

        }

        dayNumber++;
        Debug.Log("Day " + dayNumber);

        dayCycle.StartNewDay(); // Start a new day after sleeping
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            vignetteEffect.intensity.value = Mathf.Lerp(1f, vignetteDefaultIntensity, t); // Gradually reduce vignette intensity after waking up
            yield return null; // Wait for the next frame
        }
        vignetteEffect.intensity.value = vignetteDefaultIntensity; // Reset vignette intensity after waking up
        isSleeping = false;
        if(fadeOut != null)fadeOut.SetActive(false);
        isWakingUp = false;
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("DayNumber", dayNumber);
        PlayerPrefs.Save();
        
    }
    private void LoadData()
    {
        dayNumber = PlayerPrefs.GetInt("DayNumber", 0); // Load the day number, default to 0 if not found
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("New Scene Loaded. Re-connecting references...");
        RefreshReferences();

        // Check if we returned home
        if (scene.name == mainSceneName)
        {
            StartCoroutine(WakeUpCoroutine());
        }
    }
}

