using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.VRUtils;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.SimpleScreenScene
{
    public class SimpleScreenSceneController : MonoBehaviour, IMouseEmulator
    {
        public void SetOrigin(Transform vrCameraOrigin)
        {
            HandController.SetOrigin(vrCameraOrigin);
        }

        public void SetLayer(int layer)
        {
            HandController.SetLayer(layer);
        }

        void Awake()
        {
            PluginLog.Info("Awake");

            HandController = gameObject.AddComponent<VRHandController>();
            InputPatch.Emulator = this;
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

        #region IMouseEmulator

        float? IMouseEmulator.GetAxis(string axisName)
        {
            if (!HandController) return null;

            if (axisName == "Mouse X") return HandController.State.JoystickAxis.x;
            else if (axisName == "Mouse Y") return HandController.State.JoystickAxis.y;
            return null;
        }

        bool? IMouseEmulator.GetMouseButton(int button)
        {
            if (!HandController) return null;

            switch (button)
            {
                // Left button
                case 0: return HandController.State.IsTriggerOn;
                // right button
                case 1: return HandController.State.IsGripOn;
                // middle button
                case 2: return HandController.State.IsButtonXOn || HandController.State.IsButtonAOn;
                default: return null;
            }
        }

        bool? IMouseEmulator.GetMouseButtonDown(int button)
        {
            if (!HandController) return null;

            switch (button)
            {
                // Left button
                case 0: return HandController.State.IsTriggerDown;
                // right button
                case 1: return HandController.State.IsGripDown;
                // middle button
                case 2: return HandController.State.IsButtonXDown || HandController.State.IsButtonADown;
                default: return null;
            }
        }

        bool? IMouseEmulator.GetMouseButtonUp(int button)
        {
            if (!HandController) return null;

            switch (button)
            {
                // Left button
                case 0: return HandController.State.IsTriggerUp;
                // right button
                case 1: return HandController.State.IsGripUp;
                // middle button
                case 2: return HandController.State.IsButtonXUp || HandController.State.IsButtonAUp;
                default: return null;
            }
        }
        #endregion
    }
}
