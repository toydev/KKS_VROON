using System;

using HarmonyLib;

namespace KKS_VROON.Patches.HandPatches
{
    // Interrupt Camera.ScreenPointToRay during HPointMove.IsMouseOnCollider.

    [HarmonyPatch(typeof(HPointMove))]
    public class HPointMovePatch
    {
        [HarmonyPatch(nameof(HPointMove.IsMouseOnCollider))]
        [HarmonyPrefix]
        public static bool PrefixIsMouseOnCollider()
        {
            ScreenPointToRayPatch.Enabled = true;
            return true;
        }

        [HarmonyPatch(nameof(HPointMove.IsMouseOnCollider))]
        [HarmonyFinalizer]
        public static Exception FinalizerIsMouseOnCollider(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = false;
            return __exception;
        }
    }
}
