using System;

using HarmonyLib;

namespace KKS_VROON.Patches.HandPatches
{
    // Interrupt Camera.ScreenPointToRay during EstheticProc.MainProc/ObiOnCollision.

    [HarmonyPatch(typeof(EstheticProc))]
    public class EstheticProcPatch
    {
        [HarmonyPatch(nameof(EstheticProc.MainProc))]
        [HarmonyPatch(nameof(EstheticProc.ObiOnCollision))]
        [HarmonyPrefix]
        public static bool PrefixGetOnMouseCollider()
        {
            ScreenPointToRayPatch.Enabled = true;
            return true;
        }

        [HarmonyPatch(nameof(EstheticProc.MainProc))]
        [HarmonyPatch(nameof(EstheticProc.ObiOnCollision))]
        [HarmonyFinalizer]
        public static Exception FinalizerGetOnMouseCollider(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = false;
            return __exception;
        }
    }
}
