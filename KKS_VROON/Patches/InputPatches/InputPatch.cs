using System;

using HarmonyLib;
using KKS_VROON.Logging;
using UnityEngine;

namespace KKS_VROON.Patches.InputPatches
{
    // Interrupt Input.GetAxis with IMouseEmulator.

    [HarmonyPatch(typeof(Input))]
    public class InputPatch
    {
        public static IMouseEmulator Emulator { get; set; }

        [HarmonyPatch(nameof(Input.GetAxis))]
        [HarmonyPostfix]
        public static void PostfixGetAxis(ref float __result, string axisName)
        {
            var value = Emulator?.GetAxis(axisName);
            if (value != null && Math.Abs(__result) < Math.Abs(value.Value)) __result = value.Value;
        }
    }
}
