using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReVignetteAndChromaticAberration : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<VignetteAndChromaticAberration>();
            var target = gameObject.GetComponent<VignetteAndChromaticAberration>();

            if (source && !target)
            {
                PluginLog.Info($"AddComponent");
                target = gameObject.AddComponent<VignetteAndChromaticAberration>();
            }
            if (!source && target)
            {
                PluginLog.Info($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.mode = source.mode;
            target.intensity = source.intensity;
            target.chromaticAberration = source.chromaticAberration;
            target.axialAberration = source.axialAberration;
            target.blur = source.blur;
            target.blurSpread = source.blurSpread;
            target.luminanceDependency = source.luminanceDependency;
            target.blurDistance = source.blurDistance;
            target.vignetteShader = source.vignetteShader;
            target.separableBlurShader = source.separableBlurShader;
            target.chromAberrationShader = source.chromAberrationShader;
        }
    }
}
