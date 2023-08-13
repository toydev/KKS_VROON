using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class CameraHijacker : MonoBehaviour
    {
        public static void Hijack(Camera src, Camera dest = null)
        {
            if (dest) dest.CopyFrom(src);
            var hijacker = src.gameObject.GetOrAddComponent<CameraHijacker>();
            if (dest && hijacker)
            {
                dest.cullingMask = hijacker.LastCullingMask;
                dest.clearFlags = hijacker.LastClearFlags;
            }
        }

        private LayerMask LastCullingMask { get; set; }
        private CameraClearFlags LastClearFlags { get; set; }

        void Awake()
        {
            var camera = GetComponent<Camera>();
            if (camera != null)
            {
                PluginLog.Info($"Hijack camera: {camera.name}");
                LastCullingMask = camera.cullingMask;
                LastClearFlags = camera.clearFlags;
                camera.cullingMask = 0;
                camera.clearFlags = CameraClearFlags.Nothing;
            }
        }

        void OnPreCull()
        {
            // Disable camera before the rendering phase begins.
            var camera = GetComponent<Camera>();
            if (camera != null)
            {
                camera.enabled = false;
                camera.cullingMask = 0;
                camera.clearFlags = CameraClearFlags.Nothing;
            }
        }

        void OnGUI()
        {
            // Enable camera before the rendering phase ends.
            var camera = GetComponent<Camera>();
            if (camera != null && Event.current.type == EventType.Repaint) camera.enabled = true;
        }
    }
}
