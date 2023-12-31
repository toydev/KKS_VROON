﻿using System;
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
        public enum CanvasUpdateType
        {
            CAPTURE,
            DISABLE,
            SKIP,
        }

        public static UGUICapture Create(GameObject parentGameObject, string name, int layer, Func<Canvas, CanvasUpdateType> getCanvasUpdateType = null)
        {
            var gameObject = new GameObject($"{parentGameObject.name}{name}");
            // Synchronized lifecycle
            gameObject.transform.parent = parentGameObject.transform;
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<UGUICapture>();
            result.Layer = layer;
            result.GetCanvasUpdateType = getCanvasUpdateType != null ? getCanvasUpdateType : (canvas) => CanvasUpdateType.CAPTURE;
            gameObject.SetActive(true);
            return result;
        }
        public RenderTexture Texture { get; private set; }

        private int Layer { get; set; }
        private Func<Canvas, CanvasUpdateType> GetCanvasUpdateType { get; set; }
        private IDictionary<Canvas, IndexedSet<Graphic>> CanvasGraphics { get; set; }
        private ISet<Canvas> ProcessedCanvas { get; set; } = new HashSet<Canvas>();

        void Awake()
        {
            PluginLog.Debug($"Awake: {name}");

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
            var disableCanvas = new List<Canvas>();
            foreach (var canvas in CanvasGraphics.Keys)
            {
                if (canvas.enabled && (!ProcessedCanvas.Contains(canvas) || (canvas.renderMode != RenderMode.ScreenSpaceCamera || canvas.worldCamera != camera))    )
                {
                    ProcessedCanvas.Add(canvas);
                    switch (GetCanvasUpdateType(canvas))
                    {
                        case CanvasUpdateType.CAPTURE:
                            PluginLog.Debug($"Capture canvas: {canvas.name} in {canvas.gameObject.layer}:{LayerMask.LayerToName(canvas.gameObject.layer)}");
                            canvas.renderMode = RenderMode.ScreenSpaceCamera;
                            canvas.worldCamera = camera;
                            foreach (var i in canvas.gameObject.GetComponentsInChildren<Transform>())
                                i.gameObject.layer = Layer;
                            break;
                        case CanvasUpdateType.DISABLE:
                            // Canvas cannot be disabled while processing CanvasGraphics.Keys
                            disableCanvas.Add(canvas);
                            break;
                        case CanvasUpdateType.SKIP:
                        default:
                            PluginLog.Debug($"Skip canvas: {canvas.name} in {canvas.gameObject.layer}:{LayerMask.LayerToName(canvas.gameObject.layer)}");
                            break;
                    }
                }
            }

            foreach (var canvas in disableCanvas)
            {
                PluginLog.Debug($"Disable canvas: {canvas.name} in {canvas.gameObject.layer}:{LayerMask.LayerToName(canvas.gameObject.layer)}");
                canvas.enabled = false;
            }
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
    }
}
