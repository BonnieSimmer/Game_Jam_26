using UnityEngine;
using System.Collections;

#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

namespace One_Tap_UI.SRPs
{
public class MenuController
{
    private MonoBehaviour mono;
    private float blurValue;
    private float blurDuration;
    private float lastBlurTime;
    private Material blur;
    private static readonly int Value = Shader.PropertyToID("_Value");
    

    #if USING_URP
    private DepthOfField depthOfField;
    #endif

    [System.Obsolete]
    public void Start(UnityEngine.Rendering.VolumeProfile urpProfile, Material blur, float blurValue, float blurDuration)
    {
        mono = Object.FindObjectOfType<MonoBehaviour>();
        this.blur = blur;
        this.blurValue = blurValue;
        this.blurDuration = blurDuration;

        #if USING_URP
        urpProfile.TryGet<DepthOfField>(out depthOfField);
        #endif
    }
        
    public void BlurState(bool value)
    {
        #if USING_HDRP
        mono.StartCoroutine(LerpBlurShader(value ? blurValue / 3.333333f : 0, blurDuration));
        #endif

        #if USING_URP
        if (depthOfField != null)
        {
            mono.StartCoroutine(LerpBlurVolume(value ? blurValue : 0, blurDuration));
        }
        #endif
    }

    #if USING_URP
    private IEnumerator LerpBlurVolume(float target, float duration)
    {
        var start = depthOfField.focalLength.value;
        var blurTime = 0f;
        while (blurTime < duration)
        {
            blurTime += Time.deltaTime;
            depthOfField.focalLength.value = Mathf.Lerp(start, target, blurTime / duration);
            yield return null;
        }
        depthOfField.focalLength.value = target;
    }
    #endif

    #if USING_HDRP
    private IEnumerator LerpBlurShader(float target, float duration)
    {
        var start = blur.GetFloat(Value);
        var selfBlurTime = Time.time;
        lastBlurTime = selfBlurTime;
        var blurTime = 0f;
        while (blurTime < duration)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (selfBlurTime != lastBlurTime)
            {
                yield break;
            }
            blurTime = Time.time - lastBlurTime;
            blur.SetFloat(Value, Mathf.Lerp(start, target, blurTime / duration));
            yield return null;
        }
        blur.SetFloat(Value, target);
    }
    #endif

    public void Quit()
    {
        #if USING_HDRP
        blur.SetFloat(Value, 0);
        #endif
        #if USING_URP
        if (depthOfField != null) depthOfField.focalLength.value = 0;
        #endif
    }
}
}