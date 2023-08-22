using System;
using System.Collections.Generic;

using HarmonyLib;
using UnityEngine.EventSystems;

namespace KKS_VROON.Patches.HandPatches
{
    // Exclude ScreenPointToRayPatch in EventSystem.

    [HarmonyPatch(typeof(EventSystem))]
    public class EventSystemPatch
    {
        [HarmonyPatch(nameof(EventSystem.Update))]
        [HarmonyPrefix]
        public static bool PrefixUpdate()
        {
            ScreenPointToRayPatch.Enabled = false;
            return true;
        }

        [HarmonyPatch(nameof(EventSystem.Update))]
        [HarmonyFinalizer]
        public static Exception FinalizerUpdate(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = true;
            return __exception;
        }

        [HarmonyPatch(nameof(EventSystem.RaycastAll), new Type[] { typeof(PointerEventData), typeof(List<RaycastResult>) })]
        [HarmonyPrefix]
        public static bool PrefixRaycastAll()
        {
            ScreenPointToRayPatch.Enabled = false;
            return true;
        }

        [HarmonyPatch(nameof(EventSystem.RaycastAll), new Type[] { typeof(PointerEventData), typeof(List<RaycastResult>) })]
        [HarmonyFinalizer]
        public static Exception FinalizerRaycastAll(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = true;
            return __exception;
        }
    }
}
