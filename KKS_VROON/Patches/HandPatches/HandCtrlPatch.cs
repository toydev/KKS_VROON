using System;

using HarmonyLib;

namespace KKS_VROON.Patches.HandPatches
{
    // Interrupt Camera.ScreenPointToRay during HandCtrl.LateProc.

    [HarmonyPatch(typeof(HandCtrl))]
    public class HandCtrlPatch
    {
        [HarmonyPatch(nameof(HandCtrl.LateProc))]
        [HarmonyPrefix]
        public static bool PrefixLateProc()
        {
            ScreenPointToRayPatch.Enabled = true;
            return true;
        }

        [HarmonyPatch(nameof(HandCtrl.LateProc))]
        [HarmonyFinalizer]
        public static Exception FinalizerLateProc(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = false;
            return __exception;
        }
    }
}
