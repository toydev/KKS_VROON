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

            UGUICapture = UGUICapture.Create(gameObject, nameof(UGUICapture), CustomLayers.UGUI_CAPTURE_LAYER);
            UIScreen = UIScreen.Create(gameObject, nameof(UIScreen), 100, CustomLayers.UI_SCREEN_LAYER, UGUICapture, clearFlags: CameraClearFlags.Skybox);
            gameObject.AddComponent<SimpleScreenSceneController>().SetLayer(CustomLayers.UI_SCREEN_LAYER);

            UpdateCamera(false);
        }

        // Correspond to the following camera updates.
        //
        // - initial construction
        // - user request
        public void UpdateCamera(bool updateBaseHead)
        {
            if (updateBaseHead) VRCamera.UpdateBaseHeadLocalValues();

            UIScreen.LinkToFront(UIScreen.Camera, 1.0f);
            GetComponent<SimpleScreenSceneController>().SetOrigin(UIScreen.Camera);
        }

        public UIScreen UIScreen { get; set; }
        private UGUICapture UGUICapture { get; set; }
    }
}
