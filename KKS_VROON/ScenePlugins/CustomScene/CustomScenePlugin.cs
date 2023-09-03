using System.Linq;

using UnityEngine;

using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.WindowNativeUtils;
using Sirenix.Serialization.Utilities;

namespace KKS_VROON.ScenePlugins.CustomScene
{
    public class CustomScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            var baseBackground = gameObject.AddComponent<Camera>();
            baseBackground.depth = 90;
            baseBackground.cullingMask = 0;
            baseBackground.clearFlags = CameraClearFlags.SolidColor;
            baseBackground.backgroundColor = Color.black;
            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera), 100);
            var UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("UI"), CustomLayers.UGUI_CAPTURE_LAYER };
            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER,
                (canvas) =>
                {
                    if ("CvsBackground" == canvas.name) return UGUICapture.CanvasUpdateType.SKIP;
                    // Basic rule.
                    return UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer) ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.SKIP;
                }
            );
            IMGUICapture = IMGUICapture.Create(gameObject);
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 101, CustomLayers.UI_SCREEN_LAYER,
                new UIScreenPanel[] {
                    new UIScreenPanel(UGUICapture.Texture),
                    new UIScreenPanel(IMGUICapture.Texture, -0.001f * Vector3.forward, Vector3.one),
                },
                clearFlags: CameraClearFlags.Depth
            );
            UGUICaptureForBackground = UGUICapture.Create(gameObject, nameof(UGUICaptureForBackground), CustomLayers.BACKGROUND_UGUI_CAPTURE_LAYER,
                (canvas) => "CvsBackground" == canvas.name ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.SKIP);
            UIScreenForBackground = UIScreen.Create(gameObject, nameof(UIScreenForBackground), 99, CustomLayers.BACKGROUND_UI_SCREEN_LAYER,
                new UIScreenPanel[] {
                    new UIScreenPanel(UGUICaptureForBackground.Texture, Vector3.forward * 3, Vector3.one * 5),
                },
                mouseCursorVisible: false, clearFlags: CameraClearFlags.Nothing
            );
            HandController = VRHandController.Create(gameObject, nameof(VRHandController), CustomLayers.UI_SCREEN_LAYER);
            HandController.GetOrAddComponent<VRHandControllerMouseIconAttachment>();
            InputPatch.Emulator = new BasicMouseEmulator(HandController);

            Camera.allCameras.Where(i => i.name == "ColorEffectMaskCamera").ForEach(i => { CameraHijacker.Hijack(i); });

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            var gameMainCamera = Camera.main;
            if ((gameMainCamera && gameMainCamera != CurrentGameMainCamera) || !MainCamera) UpdateCamera(false);

            InputPatch.Emulator.SendMouseEvent();

            // Control the mouse pointer.
            if (HandController.State.IsPositionChanging() && UIScreen && HandController.RayCast(UIScreen.GetScreenPlane(), out var hit))
                MouseKeyboardUtils.SetCursorPos(UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameClientRect()));

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
                MainCamera.Hijack(gameMainCamera, useCopyFrom: false);
                ReEffectUtils.AddEffects(gameMainCamera, MainCamera, /* Stopped DepthOfField, because it's blurry. */ useDepthOfField: false);
                UIScreen.LinkToFront(MainCamera, 1.0f);
                UIScreenForBackground.LinkToFront(MainCamera, 1.0f);
                HandController.Link(MainCamera);
            }
        }

        private VRCamera MainCamera { get; set; }
        private UGUICapture UGUICapture { get; set; }
        private IMGUICapture IMGUICapture { get; set; }
         private UIScreen UIScreen { get; set; }
        private UGUICapture UGUICaptureForBackground { get; set; }
        private UIScreen UIScreenForBackground { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private VRHandController HandController { get; set; }
    }
}
