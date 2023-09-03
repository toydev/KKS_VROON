using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReGlobalFog : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<GlobalFog>();
            var target = gameObject.GetComponent<GlobalFog>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<GlobalFog>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.distanceFog = source.distanceFog;
            target.excludeFarPixels = source.excludeFarPixels;
            target.useRadialDistance = source.useRadialDistance;
            target.heightFog = source.heightFog;
            target.height = source.height;
            target.heightDensity = source.heightDensity;
            target.startDistance = source.startDistance;
            target.fogShader = source.fogShader;
        }
    }
}
