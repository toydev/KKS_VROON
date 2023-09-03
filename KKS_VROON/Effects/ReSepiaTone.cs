using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReSepiaTone : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<SepiaTone>();
            var target = gameObject.GetComponent<SepiaTone>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<SepiaTone>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
        }
    }
}
