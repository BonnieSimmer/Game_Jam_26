using System.Collections;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeartDisplay : MonoBehaviour
{
    public static HeartDisplay Instance;

    [Header("Setup")]
    public GameObject heartVirtualCamera; 

    [Header("Visuals")]
    public Color baseColor = Color.white; 
    public float maxIntensity = 5f; 

    [Header("Transition UI")]
    [SerializeField] private GameObject fadeIn; 
    [SerializeField] private GameObject fadeOut;
    private Vignette vignetteEffect;
    private float vignetteDefaultIntensity = 0.16f; // Default value

    // Internal Components
    private Animator heartAnimator;
    private Material heartInstanceMaterial;
    private Renderer heartRenderer;
    private ThirdPersonController playerController;
    private PlayerLogic playerLogic;

    private bool isViewingHeart = false;
    private bool isSwitching = false; // Prevents spamming Tab during fade
    private Vector3 baseScale;

    private AudioListener mainListener;
    private CinemachineImpulseSource impulseSource;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        baseScale = transform.localScale;

        impulseSource = GetComponent<CinemachineImpulseSource>();

        heartAnimator = GetComponent<Animator>();
        heartRenderer = GetComponent<Renderer>();
        if (heartRenderer != null)
        {
            heartInstanceMaterial = heartRenderer.material;
            heartInstanceMaterial.SetColor("_EmissionColor", Color.black);
            heartInstanceMaterial.EnableKeyword("_EMISSION");
        }

        if (heartVirtualCamera != null) heartVirtualCamera.SetActive(false);
    }

    private void Start()
    {
        // 1. Get Settings from GameManager if available, or Defaults
        if (GameManager.Instance != null)
        {
            // Note: GameManager's vignetteDefaultIntensity is private in your previous code. 
            // If you can't access it, 0.16f is a safe hardcoded default.
            // vignetteDefaultIntensity = GameManager.Instance.vignetteDefaultIntensity; 
        }

        RefreshReferences(); // Find UI and Volume

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<ThirdPersonController>();
            playerLogic = player.GetComponent<PlayerLogic>();
            if (playerLogic != null) UpdateHeartVisuals();
        }
    }

    private void RefreshReferences()
    {
        // A. Find UI 
        GameObject canvas = GameObject.Find("UI_Canvas");
        if (canvas != null)
        {
            Transform inTrans = canvas.transform.Find("FadeIN");
            Transform outTrans = canvas.transform.Find("FadeOUT");

            if (inTrans != null) { fadeIn = inTrans.gameObject; fadeIn.SetActive(false); }
            if (outTrans != null) { fadeOut = outTrans.gameObject; fadeOut.SetActive(false); }
        }

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera!=null)
        {
            mainListener = mainCamera.GetComponent<AudioListener>();
        }
        // B. Find Vignette
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

    void Update()
    {
        // Check isSwitching to prevent input spam while fading
        if (!isSwitching && Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(SwitchViewSequence());
        }
        transform.Rotate(Vector3.up, 20f * Time.deltaTime); // Slow rotation for visual interest
        //transform.Rotate(Vector3.right, maxIntensity * Time.deltaTime); // Subtle pulsation effect
        //transform.Rotate(Vector3.forward, maxIntensity * Time.deltaTime * 0.5f); // Subtle pulsation effect
    }

    // --- MAIN SEQUENCE COROUTINE ---
    IEnumerator SwitchViewSequence()
    {
        isSwitching = true; // Lock Input

        // 1. Fade OUT (To Black)
        yield return StartCoroutine(FadeToBlack());

        yield return new WaitForSeconds(0.1f); // Short pause to ensure fade completes before toggling
        // 2. Toggle The Logic (While Screen is Black)
        ToggleHeartState();
        
        
        // 3. Short pause to ensure camera cut happens while hidden
        yield return new WaitForSeconds(0.1f);

        // 4. Fade IN (To Clear)
        yield return StartCoroutine(FadeToClear());

        isSwitching = false; // Unlock Input
    }

    // The logic to swap cameras and player control
    private void ToggleHeartState()
    {
        isViewingHeart = !isViewingHeart;

        if (isViewingHeart)
        {
            UpdateHeartVisuals();
            if (heartVirtualCamera != null) heartVirtualCamera.SetActive(true);
            if (playerController != null) playerController.enabled = false;

            mainListener.enabled = !isViewingHeart; // Disable audio when viewing heart, re-enable when back to player
            heartVirtualCamera.GetComponent<AudioListener>().enabled = isViewingHeart; // Enable heart cam audio when viewing heart

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            mainListener.enabled = !isViewingHeart; // Disable audio when viewing heart, re-enable when back to player
            heartVirtualCamera.GetComponent<AudioListener>().enabled = isViewingHeart; // Enable heart cam audio when viewing heart

            if (heartVirtualCamera != null) heartVirtualCamera.SetActive(false);
            if (playerController != null) playerController.enabled = true;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- REFACTORED FADE COROUTINES ---
    // These now simply handle the visual fade, no scene loading.

    IEnumerator FadeToBlack()
    {
        if (fadeOut != null) fadeOut.SetActive(false);
        
        float timer = 0f;
        float duration = 0.5f; // Faster fade for UI feel
        float startIntensity = (vignetteEffect != null) ? vignetteEffect.intensity.value : 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            if (vignetteEffect != null)
                vignetteEffect.intensity.value = Mathf.Lerp(startIntensity, 1f, t);
            yield return null;
        }

        if (vignetteEffect != null) vignetteEffect.intensity.value = 1f;
        if (fadeIn != null) fadeIn.SetActive(true); // Black screen ON
    }

    IEnumerator FadeToClear()
    {
        if (fadeIn != null) fadeIn.SetActive(false); // Black screen OFF
        if (fadeOut != null) fadeOut.SetActive(true); // Fade animation ON

        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            if (vignetteEffect != null)
                vignetteEffect.intensity.value = Mathf.Lerp(1f, vignetteDefaultIntensity, t);
            yield return null;
        }

        if (vignetteEffect != null) vignetteEffect.intensity.value = vignetteDefaultIntensity;
        if (fadeOut != null) fadeOut.SetActive(false);
    }

    public void UpdateHeartVisuals()
    {
        if (playerLogic == null) return;

        float currentLight = (float)playerLogic.lightHeartLevel;

        if (heartAnimator != null)
        {
            // Map light (0-100) to speed (0.5x - 3x)
            float animSpeed = Mathf.Lerp(0.5f, 3f, currentLight / 100f);
            heartAnimator.SetFloat("LightLevel", animSpeed);
        }

        if (heartInstanceMaterial != null)
        {
            float intensity = Mathf.Lerp(0.1f, maxIntensity, currentLight / 100f);

            Color finalColor = baseColor * Mathf.LinearToGammaSpace(intensity);
            heartInstanceMaterial.SetColor("_EmissionColor", finalColor);
        }
    }

    public void TriggerHeartBeatPulse()
    {
        if (impulseSource != null)
        {
            // You can scale the shake based on light level if you want!
            float shakeStrength = 0.5f;

            if (playerLogic != null)
            {
                // Stronger light = Stronger beat? Or Weaker? 
                // Let's make it stronger with more light:
                shakeStrength = Mathf.Lerp(0.1f, 1.0f, playerLogic.lightHeartLevel / 100f);
            }

            impulseSource.GenerateImpulse(shakeStrength);
        }
    }
}