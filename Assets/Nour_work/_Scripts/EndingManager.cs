using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 

public class EndingManager : MonoBehaviour
{
    [Header("Settings")]
    public int goodEndingThreshold = 70;
    public string menuSceneName = "MainMenu";
    public KeyCode continueKey = KeyCode.Space; 

    [Header("Dialogue UI")]
    public TextMeshProUGUI subtitleText; 
    public GameObject continueIcon;
    public GameObject subtitlePanel;
    public float typingSpeed = 0.05f;    

    [Header("Dialogue Lines")]
    [TextArea] public string[] goodLines; 
    [TextArea] public string[] badLines;  

    [Header("Visual References")]
    public Light mainLight;
    public Transform waterObject;
    public Image fadePanel;

    [Header("Audio")]
    public AudioSource happyMusic;
    public AudioSource sadMusic;
    
    [Header("Speaker")]
    public GameObject entity;
    public GameObject oldMan;
    public GameObject secondaryCamera;

    private Color goodFogColor = Color.white;
    private Color badFogColor = new Color(0.6f, 0.0f, 0.0f); 
    private float waterRiseSpeed = 1f;

    IEnumerator Start()
    {
        int currentLevel = PlayerPrefs.GetInt("lightHeartLevel", 0);
        
        if (fadePanel) fadePanel.color = Color.clear;
        if (continueIcon) continueIcon.SetActive(false);
        if (subtitlePanel) subtitlePanel.SetActive(true);
        if (entity) entity.SetActive(false);
        if (oldMan) oldMan.SetActive(false);
        if (secondaryCamera) secondaryCamera.gameObject.SetActive(false);

        if (currentLevel >= goodEndingThreshold)
        {
            if (oldMan) oldMan.SetActive(true);
            if (secondaryCamera) secondaryCamera.gameObject.SetActive(true);
            if(happyMusic) happyMusic.Play();
            yield return StartCoroutine(RunDialogue(goodLines));
            yield return StartCoroutine(PlayGoodEndingVisuals());
        }
        else
        {
            if (entity) entity.SetActive(true); 
            if (waterObject)
            {
                waterObject.gameObject.SetActive(true);
                Renderer r = waterObject.GetComponent<Renderer>();
                if (r)
                {
                    r.material.SetColor("Color_7D9A58EC", badFogColor);
                    r.material.SetColor("Color_F01C36BF", badFogColor * 1.2f);
                }
            }
            if (sadMusic) sadMusic.Play();
            yield return StartCoroutine(RunDialogue(badLines));
            yield return StartCoroutine(PlayBadEndingVisuals());
        }

        SceneManager.LoadScene(menuSceneName);
    }

    IEnumerator RunDialogue(string[] lines)
    {
        subtitleText.text = ""; 
        subtitleText.gameObject.SetActive(true);

        foreach (string line in lines)
        {
            subtitleText.text = "";
            bool lineSkipped = false;

            if (continueIcon) continueIcon.SetActive(false);

            foreach (char letter in line.ToCharArray())
            {
                subtitleText.text += letter;
                
                float timer = 0f;
                while (timer < typingSpeed)
                {
                    timer += Time.deltaTime;

                    if (Input.GetKeyDown(continueKey) || Input.GetMouseButtonDown(0))
                    {
                        subtitleText.text = line; 
                        lineSkipped = true;       
                        break;                   
                    }
                    yield return null;
                }

                if (lineSkipped) break;
            }

            if (continueIcon) continueIcon.SetActive(true);
            
            yield return null; 

            while (!Input.GetKeyDown(continueKey) && !Input.GetMouseButtonDown(0))
            {
                yield return null; 
            }
            
            yield return null; 
        }

        subtitleText.text = "";
        subtitleText.gameObject.SetActive(false);
        if(subtitlePanel) subtitlePanel.SetActive(false);
        if (continueIcon) continueIcon.SetActive(false);
    }

    // --- VISUAL ENDINGS ---
    IEnumerator PlayGoodEndingVisuals()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = goodFogColor;
        RenderSettings.fogDensity = 0.01f;
        
        float duration = 6.0f;
        float timer = 0f;
        float startIntensity = mainLight.intensity;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            mainLight.intensity = Mathf.Lerp(startIntensity, 5.0f, progress);
            
            if (fadePanel)
            {
                Color c = Color.white;
                c.a = progress; 
                fadePanel.color = c;
            }
            yield return null;
        }
    }

    IEnumerator PlayBadEndingVisuals()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = badFogColor; 
        RenderSettings.fogDensity = 0f;
        
        float duration = 6.0f; 
        float timer = 0f;
        float startFogDensity = RenderSettings.fogDensity;
        float fadeStartTime = duration * 0.75f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            if (waterObject) waterObject.Translate(waterRiseSpeed * Time.deltaTime * Vector3.up);

            RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, 0.2f, progress);

            if (fadePanel)
            {
                Color c = Color.black;
                float fadeProgress = Mathf.Clamp01((timer - fadeStartTime) / (duration - fadeStartTime));
                c.a = fadeProgress; 
                fadePanel.color = c;
            }

            // Lights Out
            mainLight.intensity = Mathf.Lerp(1.0f, 0.0f, progress);

            yield return null;
        }
    }
}