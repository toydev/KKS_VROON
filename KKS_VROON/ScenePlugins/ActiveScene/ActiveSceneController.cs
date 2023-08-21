using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.VRUtils;
using KKS_VROON.WindowNativeUtils;
using KKS_VROON.Patches.HandPatches;

namespace KKS_VROON.ScenePlugins.ActiveScene
{
    public class ActiveSceneController : MonoBehaviour
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
            PluginLog.Info($"Awake: {name}");

            ActionScene = FindObjectOfType<ActionScene>();

            HandController = new GameObject(gameObject.name + nameof(HandController)).AddComponent<VRHandController>();
            InputPatch.Emulator = new ActiveSceneMouseEmulator(HandController);
            ScreenPointToRayPatch.GetRay = () => HandController ? HandController.GetRay() : null;
            ColDisposableInfoPatch.Raycast = (collider) => HandController ? HandController.WideCast(collider, 0.4f, 10, 10, 10f) : null;
            ColDisposableInfoPatch.MouseDown = () => HandController.State.IsTriggerOn;
        }

        void LateUpdate()
        {
            if (!VR.Initialized) return;
            var controllerState = HandController.State;
            var plugin = GetComponent<ActiveScenePlugin>();

            if (!ActionScene || ActionScene.isCursorLock != true)
            {
                // Control the mouse pointer.
                if (controllerState.IsPositionChanging() && plugin.UIScreen && HandController.RayCast(plugin.UIScreen.GetScreenPlane(), out var hit))
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
