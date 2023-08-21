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

            gameObject.AddComponent<SimpleScreenSceneController>().SetLayer(UI_SCREEN_LAYER);

            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), UGUI_CAPTURE_LAYER);

            UpdateCamera(false);
        }

        // Correspond to the following camera updates.
        //
        // - initial construction
        // - user request
        public void UpdateCamera(bool updateBaseHead)
        {
            if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

            // Create objects as needed.
            if (!UIScreen)
            {
                UIScreen = UIScreen.Create(new GameObject(gameObject.name + nameof(UIScreen)), UGUICapture, UI_SCREEN_LAYER);
                UIScreen.Camera.Normal.clearFlags = CameraClearFlags.Skybox;
            }

            UIScreen.LinkToFront(UIScreen.Camera, DISTANCE_OF_SCREEN);
            GetComponent<SimpleScreenSceneController>().SetOrigin(UIScreen.Camera);
        }

        public UIScreen UIScreen { get; set; }

        #region Constants
        // Use game unused layers.
        public const int UGUI_CAPTURE_LAYER = 15;
        public const int UI_SCREEN_LAYER = 31;
        // Distance(meter)
        public const float DISTANCE_OF_SCREEN = 1f;
        #endregion

        private UGUICapture UGUICapture { get; set; }
    }
}
