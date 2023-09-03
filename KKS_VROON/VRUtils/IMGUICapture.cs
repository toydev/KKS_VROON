using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class IMGUICapture : MonoBehaviour
    {
        public static IMGUICapture Create(GameObject gameObject)
        {
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<IMGUICapture>();
            gameObject.AddComponent<FirstGUIEventProcessor>();
            gameObject.AddComponent<LastGUIEventProcessor>();
            gameObject.SetActive(true);
            return result;
        }

        public RenderTexture PreviousTexture { get; private set; }
        public RenderTexture Texture { get; private set; }

        void Awake()
        {
            PluginLog.Debug($"Awake: {name}");

            Texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        }

        void OnDestroy()
        {
            PluginLog.Debug($"OnDestroy: {name}");

            if (Texture != null)
            {
                Texture.Release();
                Texture = null;
            }
        }

        private class FirstGUIEventProcessor : MonoBehaviour
        {
            void OnGUI()
            {
                GUI.depth = int.MaxValue;
                if (Event.current.type == EventType.Repaint)
                {
                    var capture = GetComponent<IMGUICapture>();
                    if (capture)
                    {
                        capture.PreviousTexture = RenderTexture.active;
                        RenderTexture.active = capture.Texture;
                        GL.Clear(true, true, Color.clear);
                    }
                }
            }
        }

        private class LastGUIEventProcessor : MonoBehaviour
        {
            void OnGUI()
            {
                GUI.depth = int.MinValue;
                if (Event.current.type == EventType.Repaint)
                {
                    var capture = GetComponent<IMGUICapture>();
                    if (capture) RenderTexture.active = capture.PreviousTexture;
                }
            }
        }
    }
}
