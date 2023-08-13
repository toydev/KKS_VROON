using System.Linq;

using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.VRUtils;
using KKS_VROON.WindowNativeUtils;
using KKS_VROON.Patches.HandPatches;

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
            ScreenPointToRayPatch.GetRay = () => HandController ? HandController.GetRay() : null;
            ColDisposableInfoPatch.Controller = HandController;
        }

        void LateUpdate()
        {
            if (!VR.Initialized) return;
            var controllerState = HandController.State;
            var plugin = GetComponent<ActiveScenePlugin>();

            PluginLog.Info($"LateUpdate1");
            if (!ActionScene || ActionScene.isCursorLock != true)
            {
                PluginLog.Info($"LateUpdate2");
                // Control the mouse pointer.
                if (controllerState.IsPositionChanging() && plugin.UIScreen && HandController.RayCast(plugin.UIScreen.GetScreenPlane(), out var hit))
                {
                    var screenPosition = plugin.UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameWindowRect());
                    MouseKeyboardUtils.NativeMethods.SetCursorPos((int)screenPosition.x, (int)screenPosition.y);
                }
            }
            PluginLog.Info($"LateUpdate4");

            // Update base head.
            if (controllerState.IsButtonYDown || controllerState.IsButtonBDown) plugin.UpdateCamera(true);
            PluginLog.Info($"LateUpdate5");
        }

        private VRHandController HandController { get; set; }
        private ActionScene ActionScene { get; set; }

        #region IMouseEmulator
        float? IMouseEmulator.GetAxis(string axisName)
        {
            if (!HandController) return null;

            // Translate hand movements to mouse movements while interrupting Camera.ScreenPointToRay.
            if (ScreenPointToRayPatch.Enabled)
            {
                var deltaThreshold = 0.1f;  /* 10cm */
                if (axisName == "Mouse X")
                {
                    var value = HandController.State.PositionDelta.x;
                    if (value <= -deltaThreshold) return -1.0f;
                    if (deltaThreshold <= value) return 1.0f;
                    return value / deltaThreshold;
                }
                else if (axisName == "Mouse Y")
                {
                    var value = HandController.State.PositionDelta.y;
                    if (value <= -deltaThreshold) return -1.0f;
                    if (deltaThreshold <= value) return 1.0f;
                    return value / deltaThreshold;
                }
            }
            else
            {
                if (axisName == "Mouse X") return HandController.State.JoystickAxis.x;
                else if (axisName == "Mouse Y")
                {
                    // Stop vertical movement of vision when walking.
                    if (Manager.Scene.NowSceneNames?.First() == "Action") return null;
                    return HandController.State.JoystickAxis.y;
                }
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

        #region Cursor
        void OnEnable()
        {
            CursorPatch.onChangeCursor += OnChangeCursor;
        }

        void OnDisable()
        {
            if (HandController) HandController.SetHandIcon(null);
            CursorPatch.onChangeCursor -= OnChangeCursor;
        }

        void OnChangeCursor(Texture2D texture)
        {
            if (HandController) HandController.SetHandIcon(texture);
        }
        #endregion
    }
}
