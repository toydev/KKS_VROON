using System.Linq;

using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.VRUtils;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.ActiveScene
{
    public class ActiveSceneController : MonoBehaviour, IMouseEmulator
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
            PluginLog.Info($"Awake: {name}");

            ActionScene = FindObjectOfType<ActionScene>();

            HandController = new GameObject(gameObject.name + nameof(HandController)).AddComponent<VRHandController>();
            InputPatch.Emulator = this;
        }

        void LateUpdate()
        {
            if (!VR.Initialized) return;
            var controllerState = HandController.State;
            var plugin = GetComponent<ActiveScenePlugin>();

            if (ActionScene == null || ActionScene.isCursorLock != true)
            {
                // Control the mouse pointer.
                if (HandController.State.IsPositionChanging() && HandController.RayCast(plugin.UIScreen.GetScreenPlane(), out var hit))
                {
                    var screenPosition = plugin.UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameWindowRect());
                    MouseKeyboardUtils.NativeMethods.SetCursorPos((int)screenPosition.x, (int)screenPosition.y);
                }
            }

            // Update base head.
            if (controllerState.IsButtonYDown || controllerState.IsButtonBDown) plugin.UpdateCamera(true);
        }

        private VRHandController HandController { get; set; }
        private ActionScene ActionScene { get; set; }

        #region IMouseEmulator
        float? IMouseEmulator.GetAxis(string axisName)
        {
            if (!HandController) return null;

            if (axisName == "Mouse X") return HandController.State.JoystickAxis.x;
            else if (axisName == "Mouse Y")
            {
                // Stop vertical movement of vision when walking.
                if (Manager.Scene.NowSceneNames?.First() == "Action") return null;
                return HandController.State.JoystickAxis.y;
            }
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
