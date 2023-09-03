using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReBloomAndFlares : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<BloomAndFlares>();
            var target = gameObject.GetComponent<BloomAndFlares>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<BloomAndFlares>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.tweakMode = source.tweakMode;
            target.screenBlendMode = source.screenBlendMode;
            target.hdr = source.hdr;
            target.sepBlurSpread = source.sepBlurSpread;
            target.useSrcAlphaAsMask = source.useSrcAlphaAsMask;
            target.bloomIntensity = source.bloomIntensity;
            target.bloomThreshold = source.bloomThreshold;
            target.bloomBlurIterations = source.bloomBlurIterations;
            target.lensflares = source.lensflares;
            target.hollywoodFlareBlurIterations = source.hollywoodFlareBlurIterations;
            target.lensflareMode = source.lensflareMode;
            target.hollyStretchWidth = source.hollyStretchWidth;
            target.lensflareIntensity = source.lensflareIntensity;
            target.lensflareThreshold = source.lensflareThreshold;
            target.flareColorA = source.flareColorA;
            target.flareColorB = source.flareColorB;
            target.flareColorC = source.flareColorC;
            target.flareColorD = source.flareColorD;
            target.lensFlareVignetteMask = source.lensFlareVignetteMask;
            target.lensFlareShader = source.lensFlareShader;
            target.lensFlareMaterial = source.lensFlareMaterial;
            target.vignetteShader = source.vignetteShader;
            target.vignetteMaterial = source.vignetteMaterial;
            target.separableBlurShader = source.separableBlurShader;
            target.separableBlurMaterial = source.separableBlurMaterial;
            target.addBrightStuffOneOneShader = source.addBrightStuffOneOneShader;
            target.addBrightStuffBlendOneOneMaterial = source.addBrightStuffBlendOneOneMaterial;
            target.screenBlendShader = source.screenBlendShader;
            target.screenBlend = source.screenBlend;
            target.hollywoodFlaresShader = source.hollywoodFlaresShader;
            target.hollywoodFlaresMaterial = source.hollywoodFlaresMaterial;
            target.brightPassFilterShader = source.brightPassFilterShader;
            target.brightPassFilterMaterial = source.brightPassFilterMaterial;
        }
    }
}
