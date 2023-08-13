using System;

using HarmonyLib;
using UnityEngine;

namespace KKS_VROON.Patches.HandPatches
{
    // Get the mouse cursor texture.
    // Limitation: default cursor is null.

    [HarmonyPatch(typeof(Cursor))]
    public class CursorPatch
    {
        public delegate void CursorCallback(Texture2D cam);

        public static CursorCallback onChangeCursor;

        [HarmonyPatch(nameof(Cursor.SetCursor), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(CursorMode) })]
        [HarmonyPrefix]
        public static bool PrefixSetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
        {
            onChangeCursor?.Invoke(texture);
            return true;
        }
    }
}
