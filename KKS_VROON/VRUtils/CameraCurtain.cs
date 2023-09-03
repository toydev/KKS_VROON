using UnityEngine;
using Valve.VR;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class CameraCurtain : MonoBehaviour
    {
        public void Update()
        {
            var cameraEnabled =
                !Manager.Scene.IsFadeNow
                && !Manager.Scene.IsNowLoading
                && !Manager.Scene.IsNowLoadingFade
                ;

            var camera = GetComponent<Camera>();
            if (camera) camera.enabled = cameraEnabled;

            var vrCamera = GetComponent<SteamVR_Camera>();
            if (vrCamera) vrCamera.enabled = cameraEnabled;
        }
    }
}
