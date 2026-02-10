using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleFader : MonoBehaviour
{
    public static SimpleFader Instance;
    public CanvasGroup faderGroup;
    public float fadeSpeed = 2.0f;

    void Awake() 
    {
        Instance = this;
        if (!faderGroup) faderGroup = GetComponent<CanvasGroup>();
    }

    public void FadeOutAndIn(System.Action onBlack)
    {
        StartCoroutine(FadeRoutine(onBlack));
    }

    private IEnumerator FadeRoutine(System.Action onBlack)
    {
        while (faderGroup.alpha < 1)
        {
            faderGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        onBlack?.Invoke();
        
        yield return new WaitForSeconds(0.5f);

        while (faderGroup.alpha > 0)
        {
            faderGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}