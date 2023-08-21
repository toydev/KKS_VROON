using System;

using HarmonyLib;
using UnityEngine;

namespace KKS_VROON.Patches.HandPatches
{
    // Interrupt Camera.ScreenPointToRay with Enabled and Func<Ray>.
    //
    // Basic usage:
    // Put hand controller operations in Func<Ray>.
    // Set Enabled in Prefix of the target method and release in Finalizer.

    [HarmonyPatch(typeof(Camera))]
    class ScreenPointToRayPatch
    {
        public static bool Enabled { get; set; } = true;
        public static Func<Ray?> GetRay { get; set; }

        [HarmonyPatch(nameof(Camera.ScreenPointToRay), new Type[] { typeof(Vector3) })]
        [HarmonyPrefix]
        public static bool PrefixCameraScreenPointToRay(ref Ray __result, Vector3 pos)
        {
            if (Enabled)
            {
                var result = GetRay?.Invoke();
                if (result != null)
                {
                    __result = result.Value;
                    return false;
                }
            }

            return true;
        }
    }
}
