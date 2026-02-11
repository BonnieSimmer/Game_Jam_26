using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleFader : MonoBehaviour
{
    public static SimpleFader Instance;
    public CanvasGroup faderGroup;
    public Image faderImage;
    public float fadeSpeed = 2.0f;

    void Awake() 
    {
        Instance = this;
        if (!faderGroup) faderGroup = GetComponent<CanvasGroup>();
        if (!faderImage) faderImage = GetComponent<Image>();
    }

    public void FadeOutAndIn(System.Action onFadeComplete)
    {
        FadeOutAndIn(Color.black, onFadeComplete);
    }

    public void FadeOutAndIn(Color targetColor, System.Action onFadeComplete)
    {
        if (faderImage) faderImage.color = targetColor;
        
        StartCoroutine(FadeRoutine(onFadeComplete));
    }

    private IEnumerator FadeRoutine(System.Action onFadeComplete)
    {
        while (faderGroup.alpha < 1)
        {
            faderGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        onFadeComplete?.Invoke();
        
        yield return new WaitForSeconds(0.5f);

        while (faderGroup.alpha > 0)
        {
            faderGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}