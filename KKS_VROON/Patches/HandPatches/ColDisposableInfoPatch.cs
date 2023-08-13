using HarmonyLib;
using UnityEngine;

using KKS_VROON.VRUtils;

namespace KKS_VROON.Patches.HandPatches
{
    // Re-implementing touch interactions for Talk scene with hand controller.

    [HarmonyPatch(typeof(TalkScene.ColDisposableInfo))]
    public class ColDisposableInfoPatch
    {
        public static VRHandController Controller { get; set; }

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
            Object.Destroy(__instance.col.GetComponent<ColDisposableInfoWrapper>());
            return true;
        }

        class ColDisposableInfoWrapper : MonoBehaviour
        {
            public TalkScene.ColDisposableInfo Target { get; set; }

            void Update()
            {
                if (Target.col && ((1 << Target.talkScene.touchMode) & Target.mode) != 0)
                {
                    var ray = (Controller != null) ? Controller.GetRay() : null;
                    if (ray != null)
                    {
                        if (Target.col.Raycast(ray.Value, out var hitInfo, 10f))
                        {
                            if (Controller.State.IsTriggerOn) Target.touchFunc(Target.name, hitInfo.point);
                            else Target.enterFunc(Target.name);
                        }
                        else Target.exitFunc(Target.name);
                    }
                }
            }
        }
    }
}
