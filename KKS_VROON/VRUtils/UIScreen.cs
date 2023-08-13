﻿using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class UIScreen : MonoBehaviour
    {
        #region Create
        public static UIScreen Create(GameObject gameObject, UGUICapture UGUICapture, int uiScreenLayer)
        {
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<UIScreen>();
            result.UGUICapture = UGUICapture;
            result.UIScreenLayer = uiScreenLayer;
            gameObject.SetActive(true);
            return result;
        }
        #endregion

        public VRCamera Camera { get; private set; }
        public bool MouseCursorVisible { get; set; } = true;

        #region Transform Screen <-> World
        public Vector2 GetScreenPositionFromWorld(Vector3 worldPositionOnScreen, Rect gameWindowRect)
        {
            var localHitPoint = ScreenObject.transform.InverseTransformPoint(worldPositionOnScreen);
            return new Vector2(
                (int)(gameWindowRect.x + (localHitPoint.x + 0.5f) * Screen.width),
                (int)(gameWindowRect.y + (0.5f - localHitPoint.y) * Screen.height));
        }

        public Vector3 GetWorldPositionFromScreen(float x, float y)
        {
            return ScreenObject.transform.TransformPoint(
                x / Screen.width - 0.5f,
                y / Screen.height - 0.5f, 0f);
        }
        #endregion

        #region Implementations
        private UGUICapture UGUICapture { get; set; }
        private int UIScreenLayer { get; set; } = 31;
        private GameObject ScreenObject { get; set; }
        private GameObject MouseCursor { get; set; }

        public Plane GetScreenPlane()
        {
            return new Plane(ScreenObject.transform.forward, ScreenObject.transform.position);
        }

        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            // Camera
            Camera = new GameObject(gameObject.name + nameof(Camera)).AddComponent<VRCamera>();
            Camera.Normal.cullingMask = 1 << UIScreenLayer;
            Camera.Normal.clearFlags = CameraClearFlags.Depth;
            Camera.Normal.nearClipPlane = 0.01f;  // 1cm

            // Screen
            ScreenObject = new GameObject(gameObject.name + nameof(Screen));
            ScreenObject.transform.SetParent(transform, false);
            ScreenObject.transform.localScale = new Vector3(UGUICapture.Texture.width / (float)UGUICapture.Texture.height, 1f, 0f);
            ScreenObject.layer = UIScreenLayer;
            var meshFilter = ScreenObject.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            var material = new Material(Shader.Find("Unlit/Transparent"))
            {
                mainTexture = UGUICapture.Texture,
            };
            var meshRenderer = ScreenObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            // MouseCursor
            MouseCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MouseCursor.transform.SetParent(GetComponent<UIScreen>().transform);
            MouseCursor.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            MouseCursor.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"))
            {
                color = new Color(0.9f, 0.9f, 0.9f, 0.5f),
            };
            var mouseCursorCollider = MouseCursor.GetComponent<Collider>();
            if (mouseCursorCollider) Destroy(mouseCursorCollider);
        }

        void Update()
        {
            if (0 <= Input.mousePosition.x && Input.mousePosition.x <= Screen.width
                && 0 <= Input.mousePosition.y && Input.mousePosition.y < Screen.height
                && MouseCursorVisible)
            {
                MouseCursor.SetActive(true);
                MouseCursor.gameObject.layer = UIScreenLayer;
                MouseCursor.transform.position = GetWorldPositionFromScreen(
                    Input.mousePosition.x, Input.mousePosition.y);
            }
            else
            {
                MouseCursor.SetActive(false);
            }
        }

        void OnDestroy()
        {
            PluginLog.Info($"OnDestroy: {name}");

            if (Camera) Destroy(Camera.gameObject);
        }
        #endregion
    }
}
