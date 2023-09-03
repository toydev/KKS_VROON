using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReDepthOfField : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<DepthOfField>();
            var target = gameObject.GetComponent<DepthOfField>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<DepthOfField>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.visualizeFocus = source.visualizeFocus;
            target.focalLength = source.focalLength;
            target.focalSize = source.focalSize;
            target.aperture = source.aperture;
            target.focalTransform = source.focalTransform;
            target.maxBlurSize = source.maxBlurSize;
            target.highResolution = source.highResolution;
            target.blurType = source.blurType;
            target.blurSampleCount = source.blurSampleCount;
            target.nearBlur = source.nearBlur;
            target.foregroundOverlap = source.foregroundOverlap;
            target.dofHdrShader = source.dofHdrShader;
            target.dx11BokehShader = source.dx11BokehShader;
            target.dx11BokehThreshold = source.dx11BokehThreshold;
            target.dx11SpawnHeuristic = source.dx11SpawnHeuristic;
            target.dx11BokehTexture = source.dx11BokehTexture;
            target.dx11BokehScale = source.dx11BokehScale;
            target.dx11BokehIntensity = source.dx11BokehIntensity;
        }
    }
}
