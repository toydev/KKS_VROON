using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReSunShafts : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<SunShafts>();
            var target = gameObject.GetComponent<SunShafts>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<SunShafts>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.resolution = source.resolution;
            target.screenBlendMode = source.screenBlendMode;
            target.sunTransform = source.sunTransform;
            target.radialBlurIterations = source.radialBlurIterations;
            target.sunColor = source.sunColor;
            target.sunThreshold = source.sunThreshold;
            target.sunShaftBlurRadius = source.sunShaftBlurRadius;
            target.sunShaftIntensity = source.sunShaftIntensity;
            target.maxRadius = source.maxRadius;
            target.useDepthTexture = source.useDepthTexture;
            target.sunShaftsShader = source.sunShaftsShader;
            target.simpleClearShader = source.simpleClearShader;
        }
    }
}
