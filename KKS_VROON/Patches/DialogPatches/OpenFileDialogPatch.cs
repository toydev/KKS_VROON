using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

using HarmonyLib;
using KKAPI.Utilities;

using KKS_VROON.Logging;

namespace KKS_VROON.Patches.DialogPatches
{
    // Force background processing of OpenFileDialog to be enabled.

    [HarmonyPatch(typeof(OpenFileDialog))]
    public static class OpenFileDialogPatch
    {
        [HarmonyPatch(nameof(OpenFileDialog.ShowDialog), new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(OpenFileDialog.OpenSaveFileDialgueFlags), typeof(string), typeof(IntPtr) })]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TranspilerShowDialog(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            PluginLog.Info($"... {typeof(OpenFileDialog).GetNestedType("NativeMethods", BindingFlags.NonPublic)}");
            var getOpenFileNameMethod = typeof(OpenFileDialog).GetNestedType("NativeMethods", BindingFlags.NonPublic).GetMethod("GetOpenFileName", BindingFlags.Public | BindingFlags.Static);

            for (int i = 0; i < codes.Count; i++)
            {
                PluginLog.Info($"{i}: {codes[i].opcode}, {codes[i].operand}");
                if ((codes[i].opcode == OpCodes.Call || codes[i].opcode == OpCodes.Callvirt) &&
                    codes[i].operand as MethodInfo == getOpenFileNameMethod)
                {
                    codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_1));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, typeof(UnityEngine.Application).GetProperty("runInBackground").GetSetMethod()));

                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }
}
