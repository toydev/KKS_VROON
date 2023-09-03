using KKS_VROON.Logging;
using UnityEngine;

namespace KKS_VROON.VRUtils
{
    public class VRMirrorManager : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Debug($"Awake: {name}");

            // Simply disable all mirrors for now.
            foreach (var i in FindObjectsOfType<MirrorReflection>()) Destroy(i);
        }
    }
}
