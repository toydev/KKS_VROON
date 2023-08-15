using System;

using HarmonyLib;
using UnityEngine;

namespace KKS_VROON.Patches.HandPatches
{
    // Re-implementing touch interactions for Talk scene with hand controller.

    [HarmonyPatch(typeof(TalkScene.ColDisposableInfo))]
    public class ColDisposableInfoPatch
    {
        public static Func<Collider, RaycastHit?> Raycast { get; set; }
        public static Func<bool> MouseDown { get; set; }

        [HarmonyPatch(nameof(TalkScene.ColDisposableInfo.Start))]
        [HarmonyPostfix]
        public static void PostfixStart(ref TalkScene.ColDisposableInfo __instance)
        {
            var previousActiveSelf = __instance.col.gameObject.activeSelf;
            __instance.col.gameObject.SetActive(false);
            var wrapper = __instance.col.GetOrAddComponent<ColDisposableInfoWrapper>();
            wrapper.Target = __instance;
            __instance.col.gameObject.SetActive(previousActiveSelf);
        }

        [HarmonyPatch(nameof(TalkScene.ColDisposableInfo.End))]
        [HarmonyPrefix]
        public static bool PrefixEnd(ref TalkScene.ColDisposableInfo __instance)
        {
            UnityEngine.Object.Destroy(__instance.col.GetComponent<ColDisposableInfoWrapper>());
            return true;
        }

        class ColDisposableInfoWrapper : MonoBehaviour
        {
            public TalkScene.ColDisposableInfo Target { get; set; }

            void Update()
            {
                if (Target.col && ((1 << Target.talkScene.touchMode) & Target.mode) != 0)
                {
                    if (Raycast != null && MouseDown != null)
                    {
                        var hitInfo = Raycast(Target.col);
                        if (hitInfo != null)
                        {
                            if (MouseDown()) Target.touchFunc(Target.name, hitInfo.Value.point);
                            else Target.enterFunc(Target.name);
                        }
                        else Target.exitFunc(Target.name);
                    }
                }
            }
        }
    }
}
