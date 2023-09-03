using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.SimpleScreenScene
{
    public class SimpleScreenScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Debug($"Awake: {name}");

            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER);
            IMGUICapture = IMGUICapture.Create(gameObject);
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 100, CustomLayers.UI_SCREEN_LAYER,
                new UIScreenPanel[] {
                    new UIScreenPanel(UGUICapture.Texture),
                    new UIScreenPanel(IMGUICapture.Texture, -0.001f * Vector3.forward, Vector3.one),
                },
                clearFlags: CameraClearFlags.Skybox
            );
            HandController = VRHandController.Create(gameObject, nameof(VRHandController), CustomLayers.UI_SCREEN_LAYER);
            InputPatch.Emulator = new BasicMouseEmulator(HandController);

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            InputPatch.Emulator.SendMouseEvent();

            // Control the mouse pointer.
            if (HandController.State.IsPositionChanging() && HandController.RayCast(UIScreen.GetScreenPlane(), out var hit))
            {
                var screenPosition = UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameClientRect());
                MouseKeyboardUtils.NativeMethods.SetCursorPos((int)screenPosition.x, (int)screenPosition.y);
            }

            // Update base head.
            if (HandController.State.IsButtonYDown || HandController.State.IsButtonBDown) UpdateCamera(true);
        }

        // Correspond to the following camera updates.
        //
        // - initial construction
        // - user request
        private void UpdateCamera(bool updateBaseHead)
        {
            if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

            UIScreen.LinkToFront(UIScreen.Camera, 1.0f);
            HandController.Link(UIScreen.Camera);
        }

        private UGUICapture UGUICapture { get; set; }
        private IMGUICapture IMGUICapture { get; set; }
        private UIScreen UIScreen { get; set; }
        private VRHandController HandController { get; set; }
    }
}
