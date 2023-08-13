using System;

using HarmonyLib;
using UnityEngine;

namespace KKS_VROON.Patches.InputPatches
{
    // Interrupt Input.GetXxx with IMouseEmulator.

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

        [HarmonyPatch(nameof(Input.GetMouseButton))]
        [HarmonyPostfix]
        public static void PostfixGetMouseButton(ref bool __result, int button)
        {
            __result = __result || Emulator?.GetMouseButton(button) == true;
        }

        [HarmonyPatch(nameof(Input.GetMouseButtonDown))]
        [HarmonyPostfix]
        public static void PostfixGetMouseButtonDown(ref bool __result, int button)
        {
            __result = __result || Emulator?.GetMouseButtonDown(button) == true;
        }

        [HarmonyPatch(nameof(Input.GetMouseButtonUp))]
        [HarmonyPostfix]
        public static void PostfixGetMouseButtonUp(ref bool __result, int button)
        {
            __result = __result || Emulator?.GetMouseButtonUp(button) == true;
        }
    }
}
