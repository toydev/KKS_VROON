using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReAmplifyOcclusionEffect : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<AmplifyOcclusionEffect>();
            var target = gameObject.GetComponent<AmplifyOcclusionEffect>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<AmplifyOcclusionEffect>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.ApplyMethod = source.ApplyMethod;
            target.SampleCount = source.SampleCount;
            target.PerPixelNormals = source.PerPixelNormals;
            target.Intensity = source.Intensity;
            target.Tint = source.Tint;
            target.Radius = source.Radius;
            target.PowerExponent = source.PowerExponent;
            target.Bias = source.Bias;
            target.CacheAware = source.CacheAware;
            target.Downsample = source.Downsample;
            target.FadeEnabled = source.FadeEnabled;
            target.FadeStart = source.FadeStart;
            target.FadeLength = source.FadeLength;
            target.FadeToIntensity = source.FadeToIntensity;
            target.FadeToRadius = source.FadeToRadius;
            target.FadeToPowerExponent = source.FadeToPowerExponent;
            target.BlurEnabled = source.BlurEnabled;
            target.BlurRadius = source.BlurRadius;
            target.BlurPasses = source.BlurPasses;
            target.BlurSharpness = source.BlurSharpness;
        }
    }
}
