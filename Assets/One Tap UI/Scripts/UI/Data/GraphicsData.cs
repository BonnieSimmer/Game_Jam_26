using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using One_Tap_UI.Utilities;
using UnityEngine.UIElements;

namespace One_Tap_UI.UI.Data
{
    [CreateAssetMenu(fileName = "Graphics Data", menuName = "One Tap UI/UI/Tabs/Graphics Data")]
    public class GraphicsData : ScriptableObject // This class handles all the graphics settings
    {
        // Bloom settings
        [SerializeField] public List<float> bloom = new() { 0, 0.5f, 1, 1.5f, 2 };

        // Motion Blur settings
        [SerializeField] public List<float> motionBlur = new() { 0, 0.5f, 1, 1.5f, 2 };

        // Anti-Aliasing settings
        [SerializeField] public List<float> antiAliasing = new() { 0f, 0.25f, 0.5f, 0.75f, 1f };

        // Shadow Quality settings
        [SerializeField] public List<ShadowResolution> shadowResolution = new() { ShadowResolution.Low, ShadowResolution.Medium, ShadowResolution.Medium, ShadowResolution.High, ShadowResolution.VeryHigh };
        [SerializeField] public List<int> shadowCascades = new() { 2, 2, 2, 4, 5 };
        [SerializeField] public List<float> shadowDistances = new() { 50, 75, 100, 150, 200 };
        [SerializeField] public List<ShadowmaskMode> shadowmaskModes = new() { ShadowmaskMode.Shadowmask, ShadowmaskMode.DistanceShadowmask, ShadowmaskMode.DistanceShadowmask, ShadowmaskMode.DistanceShadowmask, ShadowmaskMode.DistanceShadowmask };
        [SerializeField] public List<ShadowProjection> shadowProjections = new() { ShadowProjection.CloseFit, ShadowProjection.StableFit, ShadowProjection.StableFit, ShadowProjection.StableFit, ShadowProjection.StableFit };
        [SerializeField] public List<float> shadowCascadeSplits2 = new() { 1f, 0.75f, 0.5f, 0.25f, 0.1f };
        [SerializeField] public List<ShadowQuality> shadowQuality = new() { ShadowQuality.Disable, ShadowQuality.HardOnly, ShadowQuality.All, ShadowQuality.All, ShadowQuality.All };

        // Texture Quality settings
        [SerializeField] public List<AnisotropicFiltering> textureFiltering = new() { AnisotropicFiltering.Disable, AnisotropicFiltering.Disable, AnisotropicFiltering.Enable, AnisotropicFiltering.ForceEnable, AnisotropicFiltering.ForceEnable };
        [SerializeField] public List<int> globalTextureMipmapLimits = new() { 4, 3, 2, 1, 0 };
        [SerializeField] public List<bool> streamingMipmapsActiveList = new() { false, true, true, true, true };
        [SerializeField] public List<int> streamingMipmapsMaxLevelReductions = new() { 0, 1, 2, 3, 4 };
        [SerializeField] public List<int> streamingMipmapsMaxFileIORequests = new() { 2, 4, 6, 8, 10 };
        [SerializeField] public List<int> streamingMipmapsRenderersPerFrame = new() { 1, 2, 4, 6, 8 };
        [SerializeField] public List<int> streamingMipmapsMemoryBudgets = new() { 64, 128, 256, 512, 1024 };

        // Effect Quality settings
        [SerializeField] public List<int> particleRaycastBudget = new() { 4, 8, 16, 32, 64 };
        [SerializeField] public List<bool> softParticlesOptions = new() { false, true, true, true, true };
        [SerializeField] public List<bool> softVegetationOptions = new() { false, true, true, true, true };

        // Reflection Quality settings
        [SerializeField] public List<bool> realtimeReflectionProbes = new() { false, true, true, true, true };

        // Lighting Quality settings
        [SerializeField] public List<int> pixelLightCounts = new() { 0, 1, 2, 4, 8 };

        /*
        ! You may enable these settings if you use LOD's. Do not forget to tweak list values according to your project. They aren't well-tuned.

        * Use cross-fading for LOD transitions.
        * True: LODs will cross-fade between each other.
        * False: LODs will switch instantly.
        [SerializeField] public List<bool> enableLODCrossFadeOptions = new() { false, true, true, true, true };

        * Global multiplier for the LOD's switching distance.
        * A larger value leads to a longer view distance before a lower resolution LOD is picked.
        [SerializeField] public List<float> lodBiasLevels = new() { 0.5f, 1f, 1.5f, 2f, 2.5f };
        
        * A maximum LOD level. All LOD groups.
        [SerializeField] public List<int> maximumLODLevels = new() { 2, 4, 6, 8, 10 };
        */

        private UnityEngine.Rendering.PostProcessing.PostProcessLayer layer;

        private Spectrum usedSpectrumGamma = new(.5f, 1.5f),                functionalSpectrumGamma = new(-0.1f, 0.1f),
                         usedSpectrumExposure = new Spectrum(.5f, 1.5f),    functionalSpectrumExposure = new Spectrum(-0.01f, 0.01f);

        // For SRP specific functions
        private SRPs.GraphicsData SRPSpecific = new();

        internal void OnGenerate(VolumeProfile profile)
        {
            layer = Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();

            SRPSpecific.Initialize(profile);
        }

        // Gamma Adjustment
        public void SetGamma(float level)
        {
            var newValue = usedSpectrumGamma.MapValueTo(level, functionalSpectrumGamma);
            SRPSpecific.SetGamma(newValue);
        }

        // Exposure Adjustment
        public void SetExposure(float level)
        {
            var newValue = usedSpectrumExposure.MapValueTo(level, functionalSpectrumExposure);
            SRPSpecific.SetExposure(newValue);
        }

        // Bloom Adjustment
        public void SetBloom(int level)
        {
            SetValidLevel(SRPSpecific.SetBloom, bloom, level);
        }

        // Motion Blur Adjustment
        public void SetMotionBlur(int level)
        {
            SetValidLevel(SRPSpecific.SetMotionBlur, motionBlur, level);
        }

        // Anti-Aliasing Adjustment
        public void SetAntiAliasing(int level)
        {
            ClampToValidLevel(antiAliasing, ref level);
            if (level == 0)
            {
                layer.antialiasingMode = UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.None;
                return;
            }

            layer.antialiasingMode = UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing.TemporalAntialiasing;
            layer.temporalAntialiasing.jitterSpread = antiAliasing[level] * 0.5f;
        }

        // Shadow Quality Adjustment
        public void SetShadowQuality(int level)
        {
            SetValidLevel(
                (resolution) => QualitySettings.shadowResolution = resolution,
                shadowResolution, level
            );

            SetValidLevel(
                (cascades) => QualitySettings.shadowCascades = cascades,
                shadowCascades, level
            );

            SetValidLevel(
                (distance) => QualitySettings.shadowDistance = distance,
                shadowDistances, level
            );

            SetValidLevel(
                (maskMode) => QualitySettings.shadowmaskMode = maskMode,
                shadowmaskModes, level
            );

            SetValidLevel(
                (projection) => QualitySettings.shadowProjection = projection,
                shadowProjections, level
            );

            SetValidLevel(
                (split) => QualitySettings.shadowCascade2Split = split,
                shadowCascadeSplits2, level
            );
            
            SetValidLevel(
                (quality) => QualitySettings.shadows = quality,
                shadowQuality, level
            );
        }

        // Texture Quality Adjustment
        public void SetTextureQuality(int level)
        {
            SetValidLevel(
                (limit) => QualitySettings.globalTextureMipmapLimit = limit,
                globalTextureMipmapLimits, level
            );

            SetValidLevel(
                (active) => QualitySettings.streamingMipmapsActive = active,
                streamingMipmapsActiveList, level
            );

            SetValidLevel(
                (reduction) => QualitySettings.streamingMipmapsMaxLevelReduction = reduction,
                streamingMipmapsMaxLevelReductions, level
            );

            SetValidLevel(
                (budget) => QualitySettings.streamingMipmapsMemoryBudget = budget,
                streamingMipmapsMemoryBudgets, level
            );

            SetValidLevel(
                (requests) => QualitySettings.streamingMipmapsMaxFileIORequests = requests,
                streamingMipmapsMaxFileIORequests, level
            );

            SetValidLevel(
                (renderers) => QualitySettings.streamingMipmapsRenderersPerFrame = renderers,
                streamingMipmapsRenderersPerFrame, level
            );

            SetValidLevel(
                (filtering) => QualitySettings.anisotropicFiltering = filtering,
                textureFiltering, level
            );
        }

        // Effect Quality Adjustment
        public void SetEffectQuality(int level)
        {
            SetValidLevel(
                (budget) => QualitySettings.particleRaycastBudget = budget,
                particleRaycastBudget, level
            );

            SetValidLevel(
                (softParticles) => QualitySettings.softParticles = softParticlesOptions[level],
                softParticlesOptions, level
            );

            SetValidLevel(
                (softVegetation) => QualitySettings.softVegetation = softVegetationOptions[level],
                softVegetationOptions, level
            );
        }

        // Reflection Quality Adjustment
        public void SetRealtimeReflection(int level)
        {
            SetValidLevel(
                (probes) => QualitySettings.realtimeReflectionProbes = probes,
                realtimeReflectionProbes, level
            );
        }

        // Lighting Quality Adjustment
        public void SetLightingQuality(int level)
        {
            SetValidLevel(
                (count) => QualitySettings.pixelLightCount = count,
                pixelLightCounts, level
            );

            /*
            * Use cross-fading for LOD transitions.
            SetValidLevel(
                (crossFade) => QualitySettings.enableLODCrossFade = crossFade,
                enableLODCrossFadeOptions, level
            );

            * Global multiplier for the LOD's switching distance.
            * A larger value leads to a longer view distance before a lower resolution LOD is picked.
            SetValidLevel(
                (lodBias) => QualitySettings.lodBias = lodBias,
                lodBiasLevels, level
            );

            * A maximum LOD level. All LOD groups.
            SetValidLevel(
                (maxLOD) => QualitySettings.maximumLODLevel = maxLOD,
                maximumLODLevels, level
            );
            */
        }

        private bool ClampToValidLevel<T>(List<T> list, ref int level)
        {
            if (list == null || list.Count == 0 || level < 0)
                return false;
            level = Mathf.Clamp(level, 0, list.Count - 1);
            return true;
        }

        private void SetValidLevel<T>(Action<T> setFunction, List<T> list, int level)
        {
            if (!ClampToValidLevel(list, ref level)) 
                return;
            setFunction(list[level]);
        }
    }
}
