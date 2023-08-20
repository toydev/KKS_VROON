using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Collections;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class UGUICapture : MonoBehaviour
    {
        public static UGUICapture Create(GameObject gameObject, int layer, Func<Canvas, bool> isTarget = null)
        {
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<UGUICapture>();
            result.Layer = layer;
            result.IsTarget = isTarget != null ? isTarget : (canvas) => true;
            gameObject.SetActive(true);
            return result;
        }
        public RenderTexture Texture { get; private set; }

        private int Layer { get; set; }
        private Func<Canvas, bool> IsTarget { get; set; }
        private IDictionary<Canvas, IndexedSet<Graphic>> CanvasGraphics { get; set; }
        private ISet<Canvas> ProcessedCanvas { get; set; } = new HashSet<Canvas>();

        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            Texture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

            var camera = gameObject.AddComponent<Camera>();
            camera.cullingMask = 1 << Layer;
            camera.depth = float.MaxValue;
            camera.nearClipPlane = -1000f;
            camera.farClipPlane = 1000f;
            camera.targetTexture = Texture;
            camera.backgroundColor = Color.clear;
            camera.clearFlags = CameraClearFlags.Color;
            camera.orthographic = true;
            camera.useOcclusionCulling = false;
            var graphicsField = typeof(GraphicRegistry).GetField("m_Graphics", BindingFlags.Instance | BindingFlags.NonPublic);
            CanvasGraphics = graphicsField.GetValue(GraphicRegistry.instance) as IDictionary<Canvas, IndexedSet<Graphic>>;
        }

        void Update()
        {
            var camera = GetComponent<Camera>();
            foreach (var canvas in CanvasGraphics.Keys)
            {
                if (!ProcessedCanvas.Contains(canvas))
                {
                    ProcessedCanvas.Add(canvas);
                    if (canvas.enabled && (canvas.renderMode != RenderMode.ScreenSpaceCamera || canvas.worldCamera != camera))
                    {
                        if (IsTarget(canvas))
                        {
                            PluginLog.Info($"Capture canvas: {canvas.name} in {LayerMask.LayerToName(canvas.gameObject.layer)}");
                            canvas.renderMode = RenderMode.ScreenSpaceCamera;
                            canvas.worldCamera = camera;
                            foreach (var i in canvas.gameObject.GetComponentsInChildren<Transform>())
                                i.gameObject.layer = Layer;
                        }
                        else
                        {
                            PluginLog.Info($"Disable canvas: {canvas.name} in {LayerMask.LayerToName(canvas.gameObject.layer)}");
                            canvas.enabled = false;
                        }
                    }
                }
            }
        }

        void OnDestroy()
        {
            PluginLog.Info($"OnDestroy: {name}");

            if (Texture != null)
            {
                Texture.Release();
                Texture = null;
            }
        }
    }
}
