using System;

using HarmonyLib;
using UnityEngine;

namespace KKS_VROON.Patches.HandPatches
{
    // Exclude ScreenPointToRayPatch in SendMouseEvents.DoSendMouseEvents.

    [HarmonyPatch(typeof(SendMouseEvents))]
    public class SendMouseEventsPatch
    {
        [HarmonyPatch(nameof(SendMouseEvents.DoSendMouseEvents))]
        [HarmonyPrefix]
        public static bool Prefix()
        {
            ScreenPointToRayPatch.Enabled = false;
            return true;
        }

        [HarmonyPatch(nameof(SendMouseEvents.DoSendMouseEvents))]
        [HarmonyFinalizer]
        public static Exception Finalizer(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = true;
            return __exception;
        }
    }
}
