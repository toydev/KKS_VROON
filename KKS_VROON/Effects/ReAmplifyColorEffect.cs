using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReAmplifyColorEffect : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<AmplifyColorEffect>();
            var target = gameObject.GetComponent<AmplifyColorEffect>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<AmplifyColorEffect>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.Tonemapper = source.Tonemapper;
            target.Exposure = source.Exposure;
            target.LinearWhitePoint = source.LinearWhitePoint;
            target.ApplyDithering = source.ApplyDithering;
            target.QualityLevel = source.QualityLevel;
            target.BlendAmount = source.BlendAmount;
            target.LutTexture = source.LutTexture;
            target.LutBlendTexture = source.LutBlendTexture;
            target.MaskTexture = source.MaskTexture;
            target.UseDepthMask = source.UseDepthMask;
            target.DepthMaskCurve = source.DepthMaskCurve;
            target.UseVolumes = source.UseVolumes;
            target.ExitVolumeBlendTime = source.ExitVolumeBlendTime;
            target.TriggerVolumeProxy = source.TriggerVolumeProxy;
            target.VolumeCollisionMask = source.VolumeCollisionMask;
        }
    }
}
