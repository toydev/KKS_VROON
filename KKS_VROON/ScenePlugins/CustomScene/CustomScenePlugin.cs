using System.Linq;

using UnityEngine;

using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.CustomScene
{
    public class CustomScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera), 100);
            var UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("UI"), CustomLayers.UGUI_CAPTURE_LAYER };
            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER, (canvas) =>
            {
                if ("CvsBackground" == canvas.name) return UGUICapture.CanvasUpdateType.SKIP;
                // Basic rule.
                return UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer) ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.SKIP;
            });
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 101, CustomLayers.UI_SCREEN_LAYER, UGUICapture);
            BackgroundUGUICapture = UGUICapture.Create(gameObject, nameof(BackgroundUGUICapture), CustomLayers.BACKGROUND_UGUI_CAPTURE_LAYER, (canvas) =>
                "CvsBackground" == canvas.name ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.SKIP);
            BackgroundUIScreen = UIScreen.Create(gameObject, nameof(BackgroundUIScreen), 99, CustomLayers.BACKGROUND_UI_SCREEN_LAYER, BackgroundUGUICapture, clearFlags: CameraClearFlags.Skybox, mouseCursorVisible: false);
            HandController = VRHandController.Create(gameObject, nameof(VRHandController), CustomLayers.UI_SCREEN_LAYER);
            HandController.GetOrAddComponent<VRHandControllerMouseIconAttachment>();
            InputPatch.Emulator = new BasicMouseEmulator(HandController);

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            var gameMainCamera = Camera.main;
            if ((gameMainCamera && gameMainCamera != CurrentGameMainCamera) || !MainCamera) UpdateCamera(false);

            // Control the mouse pointer.
            if (HandController.State.IsPositionChanging() && UIScreen && HandController.RayCast(UIScreen.GetScreenPlane(), out var hit))
                MouseKeyboardUtils.SetCursorPos(UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameWindowRect()));

            // Update base head.
            if (HandController.State.IsButtonYDown || HandController.State.IsButtonBDown) UpdateCamera(true);
        }

        // Correspond to the following camera updates.
        //
        // - initial construction
        // - user request
        // - change the game main camera
        // - change the scene (when the game main camera is broken)
        private void UpdateCamera(bool updateBaseHead)
        {
            if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

            var gameMainCamera = CurrentGameMainCamera = Camera.main;
            if (gameMainCamera != null)
            {
                PluginLog.Info($"UpdateCamera to {gameMainCamera.name}");
                MainCamera.Hijack(gameMainCamera);
                ReEffectUtils.AddEffects(gameMainCamera, MainCamera, /* Stopped DepthOfField, because it's blurry. */ useDepthOfField: false);
                UIScreen.LinkToFront(MainCamera, 1.0f);
                BackgroundUIScreen.LinkToFront(MainCamera, 1.0f);
                HandController.Link(MainCamera);
            }
        }

        private VRCamera MainCamera { get; set; }
        private UGUICapture UGUICapture { get; set; }
        private UIScreen UIScreen { get; set; }
        private UGUICapture BackgroundUGUICapture { get; set; }
        private UIScreen BackgroundUIScreen { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private VRHandController HandController { get; set; }
    }
}
