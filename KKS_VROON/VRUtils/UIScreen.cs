using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class UIScreen : MonoBehaviour
    {
        #region Create
        public static UIScreen Create(GameObject parentGameObject, string name, int cameraDepth, int screenLayer, UGUICapture UGUICapture, bool withCurtain = true, CameraClearFlags clearFlags = CameraClearFlags.Depth)
        {
            var gameObject = new GameObject($"{parentGameObject.name}{name}");
            // Synchronized lifecycle
            gameObject.transform.parent = parentGameObject.transform;
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<UIScreen>();
            result.UGUICapture = UGUICapture;
            result.CameraDepth = cameraDepth;
            result.ScreenLayer = screenLayer;
            result.WithCurtain = withCurtain;
            result.ClearFlags = clearFlags;
            gameObject.SetActive(true);
            return result;
        }
        #endregion

        public void LinkToFront(VRCamera targetCamera, float distance)
        {
            Setup();

            if (VR.Initialized)
            {
                // Put the screen in front.
                if (Camera != targetCamera)
                {
                    Camera.VR.origin.SetParent(targetCamera.VR.origin);
                    Camera.VR.origin.localPosition = Vector3.zero;
                    Camera.VR.origin.localRotation = Quaternion.identity;
                }
                Screen.transform.SetParent(targetCamera.VR.origin);
                Screen.transform.localPosition = VRCamera.BaseHeadLocalPosition + VRCamera.BaseHeadLocalRotation * (distance * Vector3.forward);
                Screen.transform.localRotation = VRCamera.BaseHeadLocalRotation;
            }
            else
            {
                // Set as normal 2D screen.
                Camera.Normal.orthographic = true;
                Camera.Normal.orthographicSize = 0.5f;
                Screen.transform.localPosition = Vector3.forward;
                Screen.transform.localRotation = Quaternion.identity;
            }
        }

        public VRCamera Camera { get; private set; }
        public bool MouseCursorVisible { get; set; } = true;

        #region Transform Screen <-> World
        public Vector2 GetScreenPositionFromWorld(Vector3 worldPositionOnScreen, Rect gameWindowRect)
        {
            var localHitPoint = Screen.transform.InverseTransformPoint(worldPositionOnScreen);
            var actualWidth = gameWindowRect.height * 16f / 9f;
            return new Vector2(
                (int)(gameWindowRect.x + (gameWindowRect.width - actualWidth) / 2f + (localHitPoint.x + 0.5f) * actualWidth),
                (int)(gameWindowRect.y + (0.5f - localHitPoint.y) * gameWindowRect.height));
        }

        public Vector3 GetWorldPositionFromScreen(float x, float y)
        {
            return Screen.transform.TransformPoint(
                x / UnityEngine.Screen.width - 0.5f,
                y / UnityEngine.Screen.height - 0.5f, 0f);
        }
        #endregion

        #region Implementations
        private int CameraDepth { get; set; }
        private int ScreenLayer { get; set; }
        private UGUICapture UGUICapture { get; set; }
        private bool WithCurtain { get; set; }
        private CameraClearFlags ClearFlags { get; set; }
        private GameObject Screen { get; set; }
        private GameObject MouseCursor { get; set; }

        public Plane GetScreenPlane()
        {
            return new Plane(Screen.transform.forward, Screen.transform.position);
        }

        private void Setup()
        {
            if (!Camera || !Camera.Normal)
            {
                PluginLog.Info($"Setup Camera: {name}");
                Camera = VRCamera.Create(gameObject, nameof(Camera), CameraDepth, WithCurtain);
                Camera.Normal.cullingMask = 1 << ScreenLayer;
                Camera.Normal.clearFlags = ClearFlags;
                Camera.Normal.nearClipPlane = 0.01f;  // 1cm
            }

            if (!Screen)
            {
                PluginLog.Info($"Setup Screen: {name}");
                Screen = new GameObject(gameObject.name + nameof(Screen));
                Screen.transform.localScale = new Vector3(UGUICapture.Texture.width / (float)UGUICapture.Texture.height, 1f, 1f);
                Screen.layer = ScreenLayer;
                var meshFilter = Screen.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
                var material = new Material(Shader.Find("Unlit/Transparent"))
                {
                    mainTexture = UGUICapture.Texture,
                };
                var meshRenderer = Screen.AddComponent<MeshRenderer>();
                meshRenderer.material = material;
            }

            if (!MouseCursor)
            {
                PluginLog.Info($"Setup MouseCursor: {name}");
                MouseCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                MouseCursor.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                MouseCursor.layer = ScreenLayer;
                MouseCursor.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"))
                {
                    color = new Color(0.9f, 0.9f, 0.9f, 0.5f),
                };
                var mouseCursorCollider = MouseCursor.GetComponent<Collider>();
                if (mouseCursorCollider) Destroy(mouseCursorCollider);
            }
        }

        void Awake()
        {
            PluginLog.Info($"Awake: {name}");
            Setup();
        }

        void Update()
        {
            if (0 <= Input.mousePosition.x && Input.mousePosition.x <= UnityEngine.Screen.width
                && 0 <= Input.mousePosition.y && Input.mousePosition.y <= UnityEngine.Screen.height
                && MouseCursorVisible
            )
            {
                MouseCursor.SetActive(true);
                MouseCursor.transform.position = GetWorldPositionFromScreen(Input.mousePosition.x, Input.mousePosition.y);
            }
            else
            {
                MouseCursor.SetActive(false);
            }
        }

        void OnDestroy()
        {
            PluginLog.Info($"OnDestroy: {name}");
            if (Camera) Destroy(Camera);
            if (Screen) Destroy(Screen);
            if (MouseCursor) Destroy(MouseCursor);
        }
        #endregion
    }
}
