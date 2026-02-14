using System;
using System.Collections;

using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Time settings")]
    public int dayNumber = 1;
    public float bedTimeThreshold = 0.85f; // Threshold for activating tired mode (e.g., 90% of the day)

    //[Header("Tiredness indicator")]
    private float pulseSpeed = 0.5f; // Speed of the pulsing effect
    private float minVignetteIntensity = 0.5f; // Minimum intensity of the vignette effect
    private float maxVignetteIntensity = 0.78f; // Maximum intensity of the vignette effect
    public float vignetteDefaultIntensity = 0.16f; // Default intensity of the vignette effect when not in tired mode

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

    private PlayerVisual playerVisual;
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
            // IMPORTANT: Disable the component so Start() and Update() do not run on the duplicate
            this.enabled = false;
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
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
            Transform inTrans = canvas.transform.Find("FadeIN");
            Transform outTrans = canvas.transform.Find("FadeOUT");

            if (inTrans != null) { fadeIn = inTrans.gameObject; fadeIn.SetActive(false); }
            if (outTrans != null) { fadeOut = outTrans.gameObject; fadeOut.SetActive(false); }
        }

        // B. Find Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLogic = player.GetComponent<ThirdPersonController>();
            playerVisual = player.GetComponent<PlayerVisual>();
        }

        // C. Find DayCycle
        GameObject lightObj = GameObject.Find("Directional Light");
        if (lightObj != null) dayCycle = lightObj.GetComponent<DayCycle>();

        // D. Find Vignette (ROBUST VERSION)
        // Search ALL volumes to find the one that actually has a Vignette
        Volume[] allVolumes = FindObjectsByType<Volume>(FindObjectsSortMode.None);
        foreach (Volume vol in allVolumes)
        {
            if (vol.profile.TryGet<Vignette>(out Vignette v))
            {
                vignetteEffect = v;
                vignetteEffect.intensity.value = vignetteDefaultIntensity;
                Debug.Log($"GameManager: Found Vignette on '{vol.gameObject.name}'");
                return; // Found it! Stop looking.
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
        if (playerVisual != null)
        {
            playerVisual.SetTiredState(true);
        }

    }
    public void GoingToSleep()
    {
        if (isSleeping) return; // Prevent double-triggering
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

        if (dayNumber >= 3)
        {
            SceneManager.LoadSceneAsync("EndScene");
        }
        else
        {
            SceneManager.LoadSceneAsync("Nour_work/Scenes/Scene_Dream"); // Load the next scene after sleeping)
        }
        
    }

    IEnumerator WakeUpCoroutine()
    {
        yield return null;

        // Double check reference if it missed the first time
        if (vignetteEffect == null) RefreshReferences();

        // If STILL null, abort visual effect to prevent crash
        if (vignetteEffect == null)
        {
            Debug.LogError("WakeUpCoroutine aborted: Vignette Effect not found.");
            isWakingUp = false;
            isSleeping = false;
            yield break;
        }

        isWakingUp = true;
        isSleeping = false;

        float timer = 0f;
        float duration = 2f;
        float startIntensity = vignetteEffect.intensity.value;
        if (vignetteEffect == null)
        {
            Debug.LogError("WakeUpCoroutine aborted...");
            isWakingUp = false;
            yield break; // STOP here to prevent crash
        }
        if (fadeIn != null) fadeIn.SetActive(false);
        if (fadeOut != null) fadeOut.SetActive(true);

        if (playerLogic != null)
        {
            playerLogic.MoveSpeed = defaultPlayerSpeed;
            playerLogic.SprintSpeed = defaultSprintSpeed;
        }
        if (playerVisual != null) playerVisual.SetTiredState(false);

        dayNumber++;
        if (dayCycle != null) dayCycle.StartNewDay();

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            vignetteEffect.intensity.value = Mathf.Lerp(1f, vignetteDefaultIntensity, t);
            yield return null;
        }

        vignetteEffect.intensity.value = vignetteDefaultIntensity;
        if (fadeOut != null) fadeOut.SetActive(false);

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
        RefreshReferences();

        if (isWakingUp) return;

        // 2. Fix the Day Number Logic
        if (isPlayTesting)
        {
            // Reset logic for testing
            if (dayNumber <= 1 && (scene.name == mainSceneName || scene.name.Contains("Indoors")))
            {
                PlayerPrefs.DeleteAll();
                dayNumber = 1;
            }
        }
        else
        {
            LoadData();
            if (dayNumber < 1) dayNumber = 1;
        }

        // 3. Trigger Wake Up only in the main indoor scene
        if (scene.name == mainSceneName || scene.name.Contains("Indoors"))
        {
            isSleeping = false;
            StartCoroutine(WakeUpCoroutine());
        }

    }

   
}

