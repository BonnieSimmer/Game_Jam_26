using UnityEngine;
using UnityEngine.Rendering;
#if USING_URP
using UnityEngine.Rendering.Universal;
#elif USING_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace One_Tap_UI.SRPs
{
public class GraphicsData
{
    private LiftGammaGain liftGammaGainComponent;
    private Bloom bloomComponent;
    private MotionBlur motionBlurComponent;

    public void Initialize(VolumeProfile profile)
    {
        var lgg = profile.TryGet<LiftGammaGain>(out liftGammaGainComponent);
        if (!lgg) liftGammaGainComponent = profile.Add<LiftGammaGain>(true);
        else liftGammaGainComponent.active = true;

        var b = profile.TryGet<Bloom>(out bloomComponent);
        if (!b) bloomComponent = profile.Add<Bloom>(true);
        else bloomComponent.active = true;

        var mb = profile.TryGet<MotionBlur>(out motionBlurComponent);
        if (!mb) motionBlurComponent = profile.Add<MotionBlur>(true);
        else motionBlurComponent.active = true;
    }

    public void SetGamma(float value)
    {
        liftGammaGainComponent.gamma.value = new Vector4(1, 1, 1, value);
    }

    public void SetExposure(float value) 
    {
        liftGammaGainComponent.lift.value = new Vector4(1, 1, 1, value);
    }
    public void SetBloom(float value) 
    {
        bloomComponent.intensity.value = value;
    }
    public void SetMotionBlur(float value)
    {
        motionBlurComponent.intensity.value = value;
    }
}
}