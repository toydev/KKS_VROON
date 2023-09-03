using UnityStandardAssets.ImageEffects;
using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReBlur : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<Blur>();
            var target = gameObject.GetComponent<Blur>();

            if (source && !target)
            {
                PluginLog.Debug($"AddComponent");
                target = gameObject.AddComponent<Blur>();
            }
            if (!source && target)
            {
                PluginLog.Debug($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.iterations = source.iterations;
            target.blurSpread = source.blurSpread;
            target.blurShader = source.blurShader;
        }
    }
}
