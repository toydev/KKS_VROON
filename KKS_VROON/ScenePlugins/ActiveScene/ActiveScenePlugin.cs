using ADV.Commands.CameraEffect;
using KKS_VROON.Effects;
using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace KKS_VROON.ScenePlugins.ActiveScene
{
    public class ActiveScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            gameObject.AddComponent<ActiveSceneController>().SetLayer(UI_SCREEN_LAYER);

            UGUICapture = UGUICapture.Create(
                new GameObject(gameObject.name + nameof(UGUICapture)),
                UGUI_CAPTURE_LAYER,
                (canvas) => UGUI_CAPTURE_TARGET_LAYER.Contains(canvas.gameObject.layer) && !IGNORE_CANVAS.Contains(canvas.name));

            ActionScene = FindObjectOfType<ActionScene>();

            UpdateCamera(false);
        }

        void LateUpdate()
        {
            var gameMainCamera = Camera.main;
            if (gameMainCamera)
            {
                // 実メインカメラの変更またはオブジェクトの無効化でカメラを更新する。
                // オブジェクトの無効化は、Additive シーンのカメラとの関連付けから連鎖して発生する。
                if (gameMainCamera != CurrentGameMainCamera || !MainCamera)
                {
                    UpdateCamera(false);
                }
            }

            // 強制的に FPS モードに変更する。
            if (ActionScene?.CameraState.Mode != ActionGame.CameraMode.FPS)
            {
                ActionScene?.CameraState?.ModeChangeForce(ActionGame.CameraMode.FPS);
            }

            if (UIScreen) UIScreen.MouseCursorVisible = ActionScene?.isCursorLock == false || ActionScene?.isCursorLock == null;
        }

        // Correspnd to the following camera updates.
        //
        // - initial construction
        // - user request
        // - change the game main camera
        // - change the scene (when the game main camera is broken)
        public void UpdateCamera(bool updateBaseHead)
        {
            var gameMainCamera = CurrentGameMainCamera = Camera.main;
            PluginLog.Info($"UpdateCamera to {gameMainCamera.name}");
            if (gameMainCamera != null)
            {
                // Create objects as needed.
                if (!MainCamera)
                {
                    MainCamera = new GameObject(GetType().Name + nameof(MainCamera)).AddComponent<VRCamera>();
                    MainCamera.gameObject.AddComponent<CameraCurtain>();

                    UIScreen = UIScreen.Create(new GameObject(gameObject.name + nameof(UIScreen)), UGUICapture, UI_SCREEN_LAYER);
                    UIScreen.Camera.gameObject.AddComponent<CameraCurtain>();
                    UIScreen.Camera.Normal.depth = UI_SCREEN_CAMERA_DEPTH;

                    GetComponent<ActiveSceneController>().SetOrigin(MainCamera.VR.origin);
                }

                CameraHijacker.Hijack(gameMainCamera, MainCamera.Normal);
                MainCamera.GetOrAddComponent<ReGlobalFog>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReAmplifyOcclusionEffect>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReBloomAndFlares>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReAmplifyColorEffect>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReSunShafts>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReVignetteAndChromaticAberration>().Source = gameMainCamera;
                // Stopped DepthOfField, because it's blurry.
                // MainCamera.GetOrAddComponent<ReDepthOfField>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReBlur>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReCrossFade>().Source = gameMainCamera;
                MainCamera.GetOrAddComponent<ReSepiaTone>().Source = gameMainCamera;
                MainCamera.Normal.depth = MAIN_CAMERA_DEPTH;

                if (VR.Initialized)
                {
                    if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

                    // 実メインカメラとメインカメラの位置を紐づける。
                    // 更新タイミングの頭の位置と向きを差し引くことで実メインカメラの視点と現在の HMD の視点を一致させる。
                    // 向きを差し引き、差し引いた後の向きで位置を差し引く。
                    MainCamera.VR.origin.rotation = gameMainCamera.transform.rotation * Quaternion.Inverse(VRCamera.BaseHeadLocalRotation);
                    MainCamera.VR.origin.position = gameMainCamera.transform.position - MainCamera.VR.origin.rotation * VRCamera.BaseHeadLocalPosition;
                    MainCamera.VR.origin.SetParent(gameMainCamera.transform);

                    // Put the screen in front.
                    UIScreen.Camera.VR.origin.SetParent(MainCamera.VR.origin);
                    UIScreen.Camera.VR.origin.localPosition = Vector3.zero;
                    UIScreen.Camera.VR.origin.localRotation = Quaternion.identity;
                    UIScreen.transform.SetParent(MainCamera.VR.origin);
                    UIScreen.transform.localPosition = VRCamera.BaseHeadLocalPosition + VRCamera.BaseHeadLocalRotation * (DISTANCE_OF_SCREEN * Vector3.forward);
                    UIScreen.transform.localRotation = VRCamera.BaseHeadLocalRotation;
                }
                else
                {
                    // 実メインカメラとメインカメラの位置を紐づける。
                    MainCamera.Normal.transform.SetParent(gameMainCamera.transform);
                    MainCamera.Normal.transform.position = gameMainCamera.transform.position;
                    MainCamera.Normal.transform.rotation = gameMainCamera.transform.rotation;

                    // 画面サイズに合わせて 2D スクリーンとして配置する。
                    UIScreen.Camera.Normal.orthographic = true;
                    UIScreen.Camera.Normal.orthographicSize = Screen.height / 2;
                    UIScreen.transform.SetParent(UIScreen.Camera.transform);
                    UIScreen.transform.localPosition = Vector3.forward;
                    UIScreen.transform.localRotation = Quaternion.identity;
                    UIScreen.transform.localScale = new Vector3(Screen.height, Screen.height, 1);
                }
            }
        }

        public UIScreen UIScreen { get; set; }
        private VRCamera MainCamera { get; set; }
        private Camera CurrentGameMainCamera { get; set; }
        private ActionScene ActionScene { get; set; }


        #region Constants
        // Set depth larger than existing game camera.
        private const int MAIN_CAMERA_DEPTH = 100;
        private const int UI_SCREEN_CAMERA_DEPTH = 101;
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
