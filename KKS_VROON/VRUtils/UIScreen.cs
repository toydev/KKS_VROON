using UnityEngine;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class UIScreen : MonoBehaviour
    {
        #region Create
        public static UIScreen Create(
            GameObject parentGameObject,
            string name,
            int cameraDepth,
            int screenLayer,
            UIScreenPanel[] panels,
            bool withCurtain = true,
            CameraClearFlags clearFlags = CameraClearFlags.Depth,
            bool mouseCursorVisible = true
        )
        {
            var gameObject = new GameObject($"{parentGameObject.name}{name}");
            // Synchronized lifecycle
            gameObject.transform.parent = parentGameObject.transform;
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<UIScreen>();
            result.Panels = panels;
            result.CameraDepth = cameraDepth;
            result.ScreenLayer = screenLayer;
            result.WithCurtain = withCurtain;
            result.ClearFlags = clearFlags;
            result.MouseCursorVisible = mouseCursorVisible;
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
                for (var i = 0; i < Panels.Length; ++i)
                {
                    var screen = Screens[i];
                    screen.transform.SetParent(targetCamera.VR.origin);
                    screen.transform.localPosition = VRCamera.BaseHeadLocalPosition + VRCamera.BaseHeadLocalRotation * (distance * Vector3.forward + Panels[i].Offset);
                    screen.transform.localRotation = VRCamera.BaseHeadLocalRotation;
                }
            }
            else
            {
                // Set as normal 2D screen.
                Camera.Normal.orthographic = true;
                Camera.Normal.orthographicSize = 0.5f;
                for (var i = 0; i < Panels.Length; ++i)
                {
                    var screen = Screens[i];
                    screen.transform.localPosition = Vector3.forward + Panels[i].Offset;
                    screen.transform.localRotation = Quaternion.identity;
                }
            }
        }

        public VRCamera Camera { get; private set; }
        public bool MouseCursorVisible { get; set; } = true;

        #region Transform Screen <-> World
        public Vector2 GetScreenPositionFromWorld(Vector3 worldPositionOnScreen, Rect gameWindowRect)
        {
            var localHitPoint = MainScreen.transform.InverseTransformPoint(worldPositionOnScreen);
            var actualWidth = gameWindowRect.height * 16f / 9f;
            return new Vector2(
                (int)(gameWindowRect.x + (gameWindowRect.width - actualWidth) / 2f + (localHitPoint.x + 0.5f) * actualWidth),
                (int)(gameWindowRect.y + (0.5f - localHitPoint.y) * gameWindowRect.height));
        }

        public Vector3? GetWorldPositionFromScreen(float x, float y)
        {
            return MainScreen
                ? (Vector3?)MainScreen.transform.TransformPoint(x / Screen.width - 0.5f, y / Screen.height - 0.5f, 0f)
                : null;
        }
        #endregion

        #region Implementations
        private int CameraDepth { get; set; }
        private int ScreenLayer { get; set; }
        private UIScreenPanel[] Panels { get; set; }
        private bool WithCurtain { get; set; }
        private CameraClearFlags ClearFlags { get; set; }
        private GameObject MainScreen { get; set; }
        private GameObject[] Screens { get; set; }
        private GameObject MouseCursor { get; set; }

        public Plane GetScreenPlane()
        {
            return new Plane(MainScreen.transform.forward, MainScreen.transform.position);
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

            for (var i = 0; i < Panels.Length; ++i)
            {
                if (!Screens[i])
                {
                    PluginLog.Info($"Setup Screen: {name}[{i}]");
                    var screen = new GameObject($"{gameObject.name}Screen{i}");
                    var panel = Panels[i];
                    screen.transform.parent = transform;
                    screen.transform.localPosition = panel.Offset;
                    screen.transform.localScale = new Vector3(panel.Texture.width / (float)panel.Texture.height * panel.Scale.x, 1f * panel.Scale.y, 1f * panel.Scale.z);
                    screen.layer = ScreenLayer;
                    var meshFilter = screen.AddComponent<MeshFilter>();
                    meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
                    var material = new Material(Shader.Find("Unlit/Transparent"))
                    {
                        mainTexture = panel.Texture,
                    };
                    var meshRenderer = screen.AddComponent<MeshRenderer>();
                    meshRenderer.material = material;

                    if (i == 0) MainScreen = screen;
                    Screens[i] = screen;
                }
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
            Screens = new GameObject[Panels.Length];
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
                var newPosition = GetWorldPositionFromScreen(Input.mousePosition.x, Input.mousePosition.y);
                if (newPosition != null) MouseCursor.transform.position = newPosition.Value;
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
            foreach (var screen in Screens) if (screen) Destroy(screen);
            if (MouseCursor) Destroy(MouseCursor);
        }
        #endregion
    }
}
