using System;
using System.Collections.Generic;

using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KKS_VROON.Patches.HandPatches
{
    // Exclude ScreenPointToRayPatch in GraphicRaycaster.Raycast.

    [HarmonyPatch(typeof(GraphicRaycaster))]
    public class GraphicRaycasterPatch
    {
        [HarmonyPatch(nameof(GraphicRaycaster.Raycast), new Type[] { typeof(PointerEventData), typeof(List<RaycastResult>) })]
        [HarmonyPatch(nameof(GraphicRaycaster.Raycast), new Type[] { typeof(Canvas), typeof(Camera), typeof(Vector2), typeof(List<Graphic>), typeof(List<Graphic>) })]
        [HarmonyPrefix]
        public static bool PrefixRaycast()
        {
            ScreenPointToRayPatch.Enabled = false;
            return true;
        }

        [HarmonyPatch(nameof(GraphicRaycaster.Raycast), new Type[] { typeof(PointerEventData), typeof(List<RaycastResult>) })]
        [HarmonyPatch(nameof(GraphicRaycaster.Raycast), new Type[] { typeof(Canvas), typeof(Camera), typeof(Vector2), typeof(List<Graphic>), typeof(List<Graphic>) })]
        [HarmonyFinalizer]
        public static Exception FinalizerRaycast(Exception __exception)
        {
            ScreenPointToRayPatch.Enabled = true;
            return __exception;
        }
    }
}
