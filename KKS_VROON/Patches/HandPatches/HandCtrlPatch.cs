using System;

using HarmonyLib;

namespace KKS_VROON.Patches.HandPatches
{
    // Report HandCtrl processing.

    [HarmonyPatch(typeof(HandCtrl))]
    public class HandCtrlPatch
    {
        public static bool Processing { get; private set; }

        [HarmonyPatch(nameof(HandCtrl.LateProc))]
        [HarmonyPrefix]
        public static bool PrefixLateProc()
        {
            Processing = true;
            return true;
        }

        [HarmonyPatch(nameof(HandCtrl.LateProc))]
        [HarmonyFinalizer]
        public static Exception FinalizerLateProc(Exception __exception)
        {
            Processing = false;
            return __exception;
        }
    }
}
