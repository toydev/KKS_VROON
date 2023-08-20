using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class UIScreen : MonoBehaviour
    {
        #region Create
        public static UIScreen Create(GameObject gameObject, Texture[] textures, int uiScreenLayer)
        {
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<UIScreen>();
            result.Textures = textures;
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
            var localHitPoint = MainScreenObject.transform.InverseTransformPoint(worldPositionOnScreen);
            var actualWidth = gameWindowRect.height * 16f / 9f;
            return new Vector2(
                (int)(gameWindowRect.x + (gameWindowRect.width - actualWidth) / 2f + (localHitPoint.x + 0.5f) * actualWidth),
                (int)(gameWindowRect.y + (0.5f - localHitPoint.y) * gameWindowRect.height));
        }

        public Vector3 GetWorldPositionFromScreen(float x, float y)
        {
            return MainScreenObject.transform.TransformPoint(
                x / Screen.width - 0.5f,
                y / Screen.height - 0.5f, 0f);
        }
        #endregion

        #region Implementations
        private Texture[] Textures { get; set; }
        private int UIScreenLayer { get; set; } = 31;
        private GameObject MainScreenObject { get; set; }
        private GameObject MouseCursor { get; set; }

        public Plane GetScreenPlane()
        {
            return new Plane(MainScreenObject.transform.forward, MainScreenObject.transform.position);
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
            for (var i = 0; i < Textures.Length; ++i)
            {
                var screenObject = new GameObject(gameObject.name + $"ScreenObject{i}");
                screenObject.transform.SetParent(transform, false);
                screenObject.transform.localPosition = new Vector3(0.0f, 0.0f, -0.001f * i);
                screenObject.transform.localScale = new Vector3(Textures[i].width / (float)Textures[i].height, 1f, 1f);
                screenObject.layer = UIScreenLayer;
                var meshFilter = screenObject.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
                var material = new Material(Shader.Find("Unlit/Transparent"))
                {
                    mainTexture = Textures[i],
                };
                var meshRenderer = screenObject.AddComponent<MeshRenderer>();
                meshRenderer.material = material;

                if (i == 0) MainScreenObject = screenObject;
            }

            // MouseCursor
            MouseCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MouseCursor.transform.SetParent(GetComponent<UIScreen>().transform);
            MouseCursor.transform.localPosition = new Vector3(0.0f, 0.0f, -0.001f * Textures.Length);
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
