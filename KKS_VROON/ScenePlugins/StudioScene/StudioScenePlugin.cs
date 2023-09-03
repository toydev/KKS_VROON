using System.Linq;

using UnityEngine;

using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.Patches.HandPatches;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.StudioScene
{
    public class StudioScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera), 100);
            StudioRootCamera = VRCamera.Create(gameObject, nameof(StudioRootCamera), 100);
            var UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("UI"), CustomLayers.UGUI_CAPTURE_LAYER };
            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER, (canvas) =>
                UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer) ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.DISABLE);
            IMGUICapture = IMGUICapture.Create(gameObject);
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 101, CustomLayers.UI_SCREEN_LAYER,
                new UIScreenPanel[] {
                    new UIScreenPanel(UGUICapture.Texture),
                    new UIScreenPanel(IMGUICapture.Texture, -0.001f * Vector3.forward, Vector3.one),
                }
            );
            HandController = VRHandController.Create(gameObject, nameof(VRHandController), CustomLayers.UI_SCREEN_LAYER);
            HandController.GetOrAddComponent<VRHandControllerMouseIconAttachment>();
            InputPatch.Emulator = new BasicMouseEmulator(HandController);
            ScreenPointToRayPatch.GetRay = () => HandController ? HandController.GetRay() : null;
            ColDisposableInfoPatch.Raycast = (collider) => HandController ? HandController.WideCast(collider, 0.4f, 10, 10, 10f) : null;
            ColDisposableInfoPatch.MouseDown = () => HandController.State.IsTriggerOn;

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
            var gameStudioRootCamera = Camera.allCameras.Where(i => 0 < (i.cullingMask & 1 << LayerMask.NameToLayer("Studio/Route"))).FirstOrDefault();
            if (gameMainCamera != null)
            {
                PluginLog.Info($"UpdateCamera to {gameMainCamera.name}");
                MainCamera.Hijack(gameMainCamera);
                ReEffectUtils.AddEffects(gameMainCamera, MainCamera);
                if (gameStudioRootCamera) StudioRootCamera.Hijack(gameStudioRootCamera);
                UIScreen.LinkToFront(MainCamera, 1.0f);
                HandController.Link(MainCamera);
            }
        }

        private VRCamera MainCamera { get; set; }
        private VRCamera StudioRootCamera { get; set; }
        private UGUICapture UGUICapture { get; set; }
        private IMGUICapture IMGUICapture { get; set; }
        private UIScreen UIScreen { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private VRHandController HandController { get; set; }
    }
}
