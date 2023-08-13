using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.Effects
{
    public class ReCrossFade : MonoBehaviour
    {
        public Camera Source { get; set; }

        void OnPreRender()
        {
            var source = Source.GetComponent<CrossFade>();
            var target = gameObject.GetComponent<CrossFade>();

            if (source && !target)
            {
                PluginLog.Info($"AddComponent");
                target = gameObject.AddComponent<CrossFade>();
            }
            if (!source && target)
            {
                PluginLog.Info($"RemoveComponent");
                Destroy(target);
                return;
            }
            if (!source) return;

            target.enabled = source.enabled;
            target.materiaEffect = source.materiaEffect;
            target.time = source.time;
            target.FadeStartButton = source.FadeStartButton;
        }
    }
}
