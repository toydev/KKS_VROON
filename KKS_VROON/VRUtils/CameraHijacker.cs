using KKS_VROON.Logging;
using UnityEngine;

namespace KKS_VROON.VRUtils
{
    public class CameraHijacker : MonoBehaviour
    {
        public static void Hijack(Camera src, Camera destination = null)
        {
            if (destination) destination.CopyFrom(src);
            var hijacker = src.gameObject.GetOrAddComponent<CameraHijacker>();
            if (destination && hijacker) hijacker.Destination = destination;
        }

        private Camera Destination { get; set; }
        private int LastOnPreCullFrameCount { get; set; }
        private LayerMask LastCullingMask { get; set; }
        private CameraClearFlags LastClearFlags { get; set; }

        void OnPreCull()
        {
            // Allow execution only once per frame. Because the VR camera has a run for both eyes.
            if (Time.frameCount == LastOnPreCullFrameCount) return;

            // Disable camera before the rendering phase begins.
            var camera = GetComponent<Camera>();
            if (camera != null)
            {
                LastOnPreCullFrameCount = Time.frameCount;
                PluginLog.Info($"{Time.frameCount}: camera.cullingMask Before(src={camera.name},dest={Destination.name}): {camera.cullingMask}");
                LastCullingMask = camera.cullingMask;
                LastClearFlags = camera.clearFlags;
                camera.cullingMask = 0;
                camera.clearFlags = CameraClearFlags.Nothing;
                Destination.cullingMask = LastCullingMask;
                Destination.clearFlags = LastClearFlags;
                PluginLog.Info($"{Time.frameCount}: Destination.cullingMask Before(src={camera.name},dest={Destination.name}): {Destination.cullingMask}");
            }
        }

        void OnGUI()
        {
            // Enable camera before the rendering phase ends.
            var camera = GetComponent<Camera>();
            if (camera != null && Event.current.type == EventType.Repaint)
            {
                camera.cullingMask = LastCullingMask;
                camera.clearFlags = LastClearFlags;
                PluginLog.Info($"{Time.frameCount}: camera.cullingMask After (src={camera.name},dest={Destination.name}): {camera.cullingMask}");
                PluginLog.Info($"{Time.frameCount}: Destination.cullingMask After (src={camera.name},dest={Destination.name}): {Destination.cullingMask}");
            }
        }
    }
}
