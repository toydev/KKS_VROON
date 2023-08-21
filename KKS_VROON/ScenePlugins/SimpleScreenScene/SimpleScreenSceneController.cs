using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.VRUtils;
using KKS_VROON.WindowNativeUtils;
using KKS_VROON.ScenePlugins.Common;

namespace KKS_VROON.ScenePlugins.SimpleScreenScene
{
    public class SimpleScreenSceneController : MonoBehaviour
    {
        public void SetOrigin(VRCamera targetCamera)
        {
            if (targetCamera.VR) HandController.SetOrigin(targetCamera.VR.origin);
        }

        public void SetLayer(int layer)
        {
            HandController.SetLayer(layer);
        }

        void Awake()
        {
            PluginLog.Info("Awake");

            HandController = gameObject.AddComponent<VRHandController>();
            InputPatch.Emulator = new BasicMouseEmulator(HandController);
        }

        void Update()
        {
            if (!VR.Initialized) return;
            var controllerState = HandController.State;
            var plugin = GetComponent<SimpleScreenScenePlugin>();

            // Control the mouse pointer.
            if (controllerState.IsPositionChanging() && HandController.RayCast(plugin.UIScreen.GetScreenPlane(), out var hit))
            {
                var screenPosition = plugin.UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameWindowRect());
                MouseKeyboardUtils.NativeMethods.SetCursorPos((int)screenPosition.x, (int)screenPosition.y);
            }

            // Update base head.
            if (controllerState.IsButtonYDown || controllerState.IsButtonBDown) plugin.UpdateCamera(true);
        }

        private VRHandController HandController { get; set; }
    }
}
