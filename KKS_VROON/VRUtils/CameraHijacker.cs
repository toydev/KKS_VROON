using UnityEngine;

namespace KKS_VROON.VRUtils
{
    public class CameraHijacker : MonoBehaviour
    {
        public static void Hijack(Camera source, Camera destination = null, bool synchronization = true)
        {
            if (destination) destination.CopyFrom(source);
            var hijacker = source.gameObject.GetOrAddComponent<CameraHijacker>();
            hijacker.Synchronization = synchronization;
            if (destination && hijacker) hijacker.Destination = destination;
        }

        private bool Synchronization { get; set; }
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
                LastCullingMask = camera.cullingMask;
                LastClearFlags = camera.clearFlags;
                camera.cullingMask = 0;
                camera.clearFlags = CameraClearFlags.Nothing;
                if (Synchronization)
                {
                    Destination.cullingMask = LastCullingMask;
                    Destination.clearFlags = LastClearFlags;
                }
            }
        }

        void OnGUI()
        {
            if (Time.frameCount != LastOnPreCullFrameCount) return;

            // Enable camera before the rendering phase ends.
            var camera = GetComponent<Camera>();
            if (camera != null && Event.current.type == EventType.Repaint)
            {
                camera.cullingMask = LastCullingMask;
                camera.clearFlags = LastClearFlags;
            }
        }
    }
}
