using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins.Common;
using KKS_VROON.Patches.HandPatches;
using KKS_VROON.Patches.InputPatches;
using KKS_VROON.WindowNativeUtils;

namespace KKS_VROON.ScenePlugins.ActionScene
{
    public class ActionScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera), 100);
            var UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("UI"), CustomLayers.UGUI_CAPTURE_LAYER };
            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER, (canvas) =>
            {
                // Canvas_Background is the background canvas for the talk scene.
                // In VR, it will be displayed in front of the person, so exclude it.
                if ("Canvas_BackGround" == canvas.name) return UGUICapture.CanvasUpdateType.DISABLE;
                // Basic rule.
                return UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer) ? UGUICapture.CanvasUpdateType.CAPTURE : UGUICapture.CanvasUpdateType.DISABLE;
            });
            IMGUICapture = IMGUICapture.Create(gameObject);
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 110, CustomLayers.UI_SCREEN_LAYER,
                new UIScreenPanel[] {
                    new UIScreenPanel(UGUICapture.Texture),
                    new UIScreenPanel(IMGUICapture.Texture, -0.001f * Vector3.forward, Vector3.one),
                }
            );
            HandController = VRHandController.Create(gameObject, nameof(VRHandController), CustomLayers.UI_SCREEN_LAYER);
            HandController.GetOrAddComponent<VRHandControllerMouseIconAttachment>();
            InputPatch.Emulator = new ActionSceneMouseEmulator(HandController);
            ScreenPointToRayPatch.GetRay = () => HandController ? HandController.GetRay() : null;
            ColDisposableInfoPatch.Raycast = (collider) => HandController ? HandController.WideCast(collider, 0.4f, 10, 10, 10f) : null;
            ColDisposableInfoPatch.MouseDown = () => HandController.State.IsTriggerOn;
            ActionScene = FindObjectOfType<global::ActionScene>();

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            var gameMainCamera = Camera.main;
            if ((gameMainCamera && gameMainCamera != CurrentGameMainCamera) || !MainCamera) UpdateCamera(false);

            InputPatch.Emulator.SendMouseEvent();

            if (ActionScene != null)
            {
                // Force FPS mode only once in each scene.
                if (!CameraModeAdjustedInScene)
                {
                    CameraModeAdjustedInScene = true;
                    if (ActionScene.CameraState.Mode != ActionGame.CameraMode.FPS)
                    {
                        PluginLog.Info("Force FPS mode");
                        ActionScene.CameraState.ModeChangeForce(ActionGame.CameraMode.FPS);
                    }
                }

                // Control the mouse pointer.
                if (UIScreen) UIScreen.MouseCursorVisible = !ActionScene.isCursorLock;
                if (ActionScene.isCursorLock != true && HandController.State.IsPositionChanging() && UIScreen && HandController.RayCast(UIScreen.GetScreenPlane(), out var hit))
                    MouseKeyboardUtils.SetCursorPos(UIScreen.GetScreenPositionFromWorld(hit.point, WindowUtils.GetGameWindowRect()));
            }

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
                MainCamera.Hijack(gameMainCamera, synchronization: false);
                ReEffectUtils.AddEffects(gameMainCamera, MainCamera, /* Stopped DepthOfField, because it's blurry. */ useDepthOfField: false);
                UIScreen.LinkToFront(MainCamera, 1.0f);
                HandController.Link(MainCamera);
            }
        }

        #region Reset CameraModeAdjustedInScene
        private bool CameraModeAdjustedInScene { get; set; }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Additive) CameraModeAdjustedInScene = false;
        }
        void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
        void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }
        #endregion

        private VRCamera MainCamera { get; set; }
        private UGUICapture UGUICapture { get; set; }
        private IMGUICapture IMGUICapture { get; set; }
         private UIScreen UIScreen { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private VRHandController HandController { get; set; }
        private global::ActionScene ActionScene { get; set; }
    }
}
