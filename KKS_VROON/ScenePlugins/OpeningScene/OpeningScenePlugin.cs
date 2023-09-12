using System.Linq;

using UnityEngine;

using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.OpeningScene
{
    public class OpeningScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Debug($"Awake: {name}");

            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera), 100);
            var UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("UI"), CustomLayers.UGUI_CAPTURE_LAYER };
            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER, (canvas) =>
            {
                // SceneCanvas is a canvas for loading display.
                // issue #2: Disable SceneCanvas during autosave tutorial.
                if ("SceneCanvas" == canvas.name && !Tutorial.Checked(Tutorial.Category.AutoSave)) return UGUICapture.CanvasUpdateType.DISABLE;
                // Basic rule.
                return UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer) ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.DISABLE;
            });
            IMGUICapture = IMGUICapture.Create(gameObject);
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 101, CustomLayers.UI_SCREEN_LAYER,
                new UIScreenPanel[] {
                    new UIScreenPanel(UGUICapture.Texture),
                    new UIScreenPanel(IMGUICapture.Texture, -0.001f * Vector3.forward, Vector3.one),
                },
                // issue #2: Don't use CameraCurtain during OpeningScene for dialog control when playing the game for the first time.
                withCurtain: Manager.Scene.NowSceneNames?.Contains(SceneNames.OPENING_SCENE) != true,
                mouseCursorVisible: () => Cursor.visible
            );
            HandController = VRHandController.Create(gameObject, nameof(VRHandController), CustomLayers.UI_SCREEN_LAYER);
            InputPatch.Emulator = new BasicMouseEmulator(HandController);

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            var gameMainCamera = Camera.main;
            if ((gameMainCamera && gameMainCamera != CurrentGameMainCamera) || !MainCamera) UpdateCamera(false);

            InputPatch.Emulator.SendMouseEvent();

            // Control the mouse pointer.
            if (Cursor.visible && HandController.State.IsPositionChanging() && UIScreen && HandController.RayCast(UIScreen.GetScreenPlane(), out var hit))
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
                PluginLog.Debug($"UpdateCamera to {gameMainCamera.name}");
                MainCamera.Hijack(gameMainCamera);
                ReEffectUtils.AddEffects(gameMainCamera, MainCamera, /* Stopped DepthOfField, because it's blurry. */ useDepthOfField: false);
                UIScreen.LinkToFront(MainCamera, 1.0f);
                HandController.Link(MainCamera);
            }
        }

        private VRCamera MainCamera { get; set; }
        private UGUICapture UGUICapture { get; set; }
        private IMGUICapture IMGUICapture { get; set; }
        private UIScreen UIScreen { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private VRHandController HandController { get; set; }
    }
}
