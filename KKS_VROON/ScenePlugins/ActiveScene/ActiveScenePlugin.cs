using System.Linq;

using UnityEngine;

using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;

namespace KKS_VROON.ScenePlugins.ActiveScene
{
    public class ActiveScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera), 100);
            var UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("UI"), CustomLayers.UGUI_CAPTURE_LAYER };
            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER, (canvas) =>
            {
                // Canvas_Background is the background canvas for the talk scene.
                // In VR, it will be displayed in front of the person, so exclude it.
                if ("Canvas_BackGround" == canvas.name) return false;

                // SceneCanvas is a canvas for loading display.
                // issue #2: Disable SceneCanvas during autosave tutorial.
                if ("SceneCanvas" == canvas.name && !Tutorial.Checked(Tutorial.Category.AutoSave)) return false;

                // Basic rule.
                return UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer);
            });
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 101, CustomLayers.UI_SCREEN_LAYER, UGUICapture,
                // issue #2: Don't use CameraCurtain during OpeningScene for dialog control when playing the game for the first time.
                withCurtain: Manager.Scene.NowSceneNames?.Contains(SceneNames.OPENING_SCENE) != true);
            gameObject.AddComponent<ActiveSceneController>().SetLayer(CustomLayers.UI_SCREEN_LAYER);
            ActionScene = FindObjectOfType<ActionScene>();

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            var gameMainCamera = Camera.main;
            if ((gameMainCamera && gameMainCamera != CurrentGameMainCamera) || !MainCamera) UpdateCamera(false);

            if (ActionScene != null)
            {
                // Force FPS mode
                if (ActionScene.CameraState.Mode != ActionGame.CameraMode.FPS) ActionScene.CameraState.ModeChangeForce(ActionGame.CameraMode.FPS);
                if (UIScreen) UIScreen.MouseCursorVisible = !ActionScene.isCursorLock;
            }
        }

        // Correspond to the following camera updates.
        //
        // - initial construction
        // - user request
        // - change the game main camera
        // - change the scene (when the game main camera is broken)
        public void UpdateCamera(bool updateBaseHead)
        {
            if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

            var gameMainCamera = CurrentGameMainCamera = Camera.main;
            if (gameMainCamera != null)
            {
                PluginLog.Info($"UpdateCamera to {gameMainCamera.name}");
                MainCamera.Hijack(gameMainCamera);
                ReEffectUtils.AddEffects(gameMainCamera, MainCamera, /* Stopped DepthOfField, because it's blurry. */ useDepthOfField: false);
                UIScreen.LinkToFront(MainCamera, 1.0f);
                GetComponent<ActiveSceneController>().SetOrigin(MainCamera);
            }
        }

        public UIScreen UIScreen { get; set; }
        private VRCamera MainCamera { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private ActionScene ActionScene { get; set; }
        private UGUICapture UGUICapture { get; set; }
    }
}
