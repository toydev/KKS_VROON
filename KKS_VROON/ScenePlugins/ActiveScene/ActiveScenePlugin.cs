﻿using System.Linq;

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

            gameObject.AddComponent<ActiveSceneController>().SetLayer(UI_SCREEN_LAYER);

            MainCamera = VRCamera.Create(gameObject, nameof(MainCamera));
            UGUICapture = UGUICapture.Create(new GameObject(gameObject.name + nameof(UGUICapture)), UGUI_CAPTURE_LAYER, IsTargetCanvas);

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
                if (ActionScene.CameraState.Mode != ActionGame.CameraMode.FPS)
                {
                    ActionScene.CameraState.ModeChangeForce(ActionGame.CameraMode.FPS);
                }

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

                // Create objects as needed.
                if (!UIScreen)
                {
                    UIScreen = UIScreen.Create(new GameObject(gameObject.name + nameof(UIScreen)), UGUICapture, UI_SCREEN_LAYER,
                        // issue #2: Don't use CameraCurtain during OpeningScene for dialog control when playing the game for the first time.
                        withCurtain: Manager.Scene.NowSceneNames?.Contains(SceneNames.OPENING_SCENE) != true);
                }

                UIScreen.LinkToFront(MainCamera, DISTANCE_OF_SCREEN);

                GetComponent<ActiveSceneController>().SetOrigin(MainCamera);
            }
        }

        public UIScreen UIScreen { get; set; }
        private VRCamera MainCamera { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private ActionScene ActionScene { get; set; }


        #region Constants
        // Use game unused layers.
        private const int UGUI_CAPTURE_LAYER = 15;
        private const int UI_SCREEN_LAYER = 31;
        // Limit the UGUI Capture layer (UI and self layer).
        private static int[] UGUI_CAPTURE_TARGET_LAYER = new int[] { LayerMask.NameToLayer("UI"), UGUI_CAPTURE_LAYER };
        // Distance(meter)
        public const float DISTANCE_OF_SCREEN = 1f;
        // Ignore the background during talk.
        private string[] IGNORE_CANVAS = new string[] { "Canvas_BackGround" };
        private string[] IGNORE_CAMERA = new string[] { "Camera_BackGround" };
        #endregion

        #region Control canvas
        private bool IsTargetCanvas(Canvas canvas)
        {
            // Canvas_Background is the background canvas for the talk scene.
            // In VR, it will be displayed in front of the person, so exclude it.
            if ("Canvas_BackGround" == canvas.name)
            {
                return false;
            }

            // SceneCanvas is a canvas for loading display.
            // issue #2: Disable SceneCanvas during autosave tutorial.
            if ("SceneCanvas" == canvas.name && !Tutorial.Checked(Tutorial.Category.AutoSave))
            {
                return false;
            }

            // Basic rule.
            return UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer);
        }
        #endregion

        #region IgnoreCamera
        void OnEnable()
        {
            Camera.onPreRender += OnPreRenderAllCamera;
        }

        void OnDisable()
        {
            Camera.onPreRender -= OnPreRenderAllCamera;
        }

        void OnPreRenderAllCamera(Camera camera)
        {
            if (IGNORE_CAMERA.Contains(camera.name)) CameraHijacker.Hijack(camera);
        }
        #endregion

        private UGUICapture UGUICapture { get; set; }
    }
}
