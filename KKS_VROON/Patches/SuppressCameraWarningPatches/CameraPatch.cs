using HarmonyLib;
using UnityEngine;

using KKS_VROON.VRUtils;

namespace KKS_VROON.Patches.ResearchPatches
{
    // Suppress changes in fieldOfView to suppress logs below:
    // Cannot set field of view on camera with name 'xxx' while VR is enabled.

    [HarmonyPatch(typeof(Camera), "set_fieldOfView")]
    class CameraPatch
    {
        static bool Prefix()
        {
            return !VR.Initialized;
        }
    }
}
