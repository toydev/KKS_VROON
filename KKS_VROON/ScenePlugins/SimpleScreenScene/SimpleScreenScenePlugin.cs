using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.VRUtils;

namespace KKS_VROON.ScenePlugins.SimpleScreenScene
{
    public class SimpleScreenScenePlugin : MonoBehaviour
    {
        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            UGUICapture = UGUICapture.Create(new GameObject(gameObject.name + nameof(UGUICapture)), UGUI_CAPTURE_LAYER);
            UGUICapture.gameObject.transform.SetParent(gameObject.transform);

            UpdateCamera(false);
        }

        // Correspnd to the following camera updates.
        //
        // - initial construction
        // - user request
        public void UpdateCamera(bool updateBaseHead)
        {
            // Create objects as needed.
            if (!UIScreen)
            {
                UIScreen = UIScreen.Create(new GameObject(gameObject.name + nameof(UIScreen)), UGUICapture, UI_SCREEN_LAYER);
                UIScreen.Camera.Normal.clearFlags = CameraClearFlags.Skybox;
                UIScreen.Camera.Normal.depth = UI_SCREEN_CAMERA_DEPTH;
            }

            if (VR.Initialized)
            {
                if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

                // Put the screen in front.
                UIScreen.transform.SetParent(UIScreen.Camera.VR.origin);
                UIScreen.transform.localPosition = VRCamera.BaseHeadLocalPosition + VRCamera.BaseHeadLocalRotation * (DISTANCE_OF_SCREEN * Vector3.forward);
                UIScreen.transform.localRotation = VRCamera.BaseHeadLocalRotation;
            }
            else
            {
                // Set as normal 2D screen.
                UIScreen.Camera.Normal.orthographic = true;
                UIScreen.Camera.Normal.orthographicSize = Screen.height / 2;
                UIScreen.transform.SetParent(UIScreen.Camera.transform);
                UIScreen.transform.localPosition = Vector3.forward;
                UIScreen.transform.localRotation = Quaternion.identity;
                UIScreen.transform.localScale = new Vector3(Screen.height, Screen.height, 1);
            }
        }

        #region Constants
        // Set depth larger than existing game camera.
        private const int UI_SCREEN_CAMERA_DEPTH = 101;
        // Use game unused layers.
        public static int UGUI_CAPTURE_LAYER = 15;
        public const int UI_SCREEN_LAYER = 31;
        // Distance(meter)
        public const float DISTANCE_OF_SCREEN = 1f;
        #endregion

        private UGUICapture UGUICapture { get; set; }
        private UIScreen UIScreen { get; set; }
    }
}
