using UnityEngine;
using Valve.VR;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class VRHandController : MonoBehaviour
    {
        #region Create
        public static VRHandController Create(GameObject parentGameObject, string name, int screenLayer)
        {
            var gameObject = new GameObject($"{parentGameObject.name}{name}");
            // Synchronized lifecycle
            gameObject.transform.parent = parentGameObject.transform;
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<VRHandController>();
            result.ScreenLayer = screenLayer;
            gameObject.SetActive(true);
            return result;
        }
        #endregion

        public static bool IsLeftHandActive { get; set; } = false;
        public static bool IsRightHandActive { get; set; } = false;

        public VRHandControllerState State { get { UpdateState(); return _state; } }

        public Ray? GetRay()
        {
            if (enabled)
            {
                UpdateState();
                if (LLaserPointer.gameObject.activeInHierarchy) return new Ray(State.LeftLaserPosition, State.LeftLaserRotation * Vector3.forward);
                else if (RLaserPointer.gameObject.activeInHierarchy) return new Ray(State.RightLaserPosition, State.RightLaserRotation * Vector3.forward);
            }
            return null;
        }

        public bool RayCast(Plane plane, out RaycastHit hit)
        {
            var ray = GetRay();
            if (ray != null)
            {
                if (plane.Raycast(ray.Value, out var enter))
                {
                    hit = default;
                    hit.point = ray.Value.GetPoint(enter);
                    return true;
                }
            }

            hit = default;
            return false;
        }

        public RaycastHit? WideCast(Collider targetCollider, float width, int xnum, int ynum, float maxDistance)
        {
            if (enabled)
            {
                UpdateState();
                if (LLaserPointer.gameObject.activeInHierarchy)
                    return WideCast(
                        State.LeftLaserPosition,
                        State.LeftLaserRotation * Vector3.forward,
                        State.LeftLaserRotation * Vector3.right,
                        State.LeftLaserRotation * Vector3.up,
                        targetCollider, width, xnum, ynum, maxDistance);
                else if (RLaserPointer.gameObject.activeInHierarchy)
                    return WideCast(
                        State.RightLaserPosition,
                        State.RightLaserRotation * Vector3.forward,
                        State.RightLaserRotation * Vector3.right,
                        State.RightLaserRotation * Vector3.up,
                        targetCollider, width, xnum, ynum, maxDistance);
            }
            return null;
        }

        public void Link(VRCamera targetCamera)
        {
            if (enabled && targetCamera.VR)
            {
                LControllerVisual.origin = targetCamera.VR.origin;
                RControllerVisual.origin = targetCamera.VR.origin;
                LControllerPose.origin = targetCamera.VR.origin;
                RControllerPose.origin = targetCamera.VR.origin;
            }
        }

        public void SetLayer(int layer)
        {
            if (enabled)
            {
                LControllerVisual.gameObject.layer = layer;
                RControllerVisual.gameObject.layer = layer;
                LControllerPose.gameObject.layer = layer;
                RControllerPose.gameObject.layer = layer;
                LLaserPointer.gameObject.layer = layer;
                RLaserPointer.gameObject.layer = layer;
                LHandIcon.gameObject.layer = layer;
                RHandIcon.gameObject.layer = layer;
            }
        }

        public void SetHandIcon(Texture2D texture)
        {
            if (enabled)
            {
                if (texture)
                {
                    LHandIcon.transform.localScale = 0.1f * new Vector3(texture.width / (float)texture.height, 1f, 0f);
                    RHandIcon.transform.localScale = 0.1f * new Vector3(texture.width / (float)texture.height, 1f, 0f);
                    LHandIcon.material.mainTexture = texture;
                    RHandIcon.material.mainTexture = texture;
                    LHandIcon.gameObject.SetActive(true);
                    RHandIcon.gameObject.SetActive(true);
                }
                else
                {
                    LHandIcon.gameObject.SetActive(false);
                    RHandIcon.gameObject.SetActive(false);
                }
            }
        }

        private int ScreenLayer { get; set; }
        private SteamVR_Behaviour_Pose LControllerVisual { get; set; }
        private SteamVR_Behaviour_Pose RControllerVisual { get; set; }
        private SteamVR_Behaviour_Pose LControllerPose { get; set; }
        private SteamVR_Behaviour_Pose RControllerPose { get; set; }
        private Transform LLaserPointer { get; set; }
        private Transform RLaserPointer { get; set; }
        private MeshRenderer LHandIcon { get; set; }
        private MeshRenderer RHandIcon { get; set; }

        void Awake()
        {
            PluginLog.Info("Awake");

            if (VR.Initialized)
            {
                // Separate SteamVR_RenderModel,
                // Because it rebuilds related elements when the model is loaded.
                LControllerVisual = CreatePose(SteamVR_Input_Sources.LeftHand, true);
                RControllerVisual = CreatePose(SteamVR_Input_Sources.RightHand, true);

                LControllerPose = CreatePose(SteamVR_Input_Sources.LeftHand, false);
                RControllerPose = CreatePose(SteamVR_Input_Sources.RightHand, false);
                LLaserPointer = CreateLarserPointer(LControllerPose);
                RLaserPointer = CreateLarserPointer(RControllerPose);
                LHandIcon = CreateHandIcon(LControllerPose);
                RHandIcon = CreateHandIcon(RControllerPose);
                LLaserPointer.gameObject.SetActive(IsLeftHandActive);
                RLaserPointer.gameObject.SetActive(IsRightHandActive);
                LHandIcon.enabled = IsLeftHandActive;
                RHandIcon.enabled = IsRightHandActive;
                SetLayer(ScreenLayer);
            }
            else
            {
                enabled = false;
            }
        }

        private SteamVR_Behaviour_Pose CreatePose(SteamVR_Input_Sources inputSource, bool withRenderModel)
        {
            var poseGameObject = new GameObject(gameObject.name + inputSource);
            // Synchronized lifecycle
            poseGameObject.transform.parent = gameObject.transform;
            poseGameObject.SetActive(false);
            var result = poseGameObject.AddComponent<SteamVR_Behaviour_Pose>();
            result.inputSource = inputSource;
            if (withRenderModel)
            {
                result.gameObject.AddComponent<SteamVR_RenderModel>();
            }
            poseGameObject.SetActive(true);
            return result;
        }

        private Transform CreateLarserPointer(SteamVR_Behaviour_Pose parentPose)
        {
            var result = new GameObject(gameObject.name + nameof(LineRenderer)).AddComponent<LineRenderer>();
            result.transform.SetParent(parentPose.transform);
            result.transform.localPosition = Vector3.zero;
            result.transform.localRotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);
            result.startWidth = 0.01f;
            result.endWidth = 0.001f;
            result.positionCount = 2;
            result.useWorldSpace = false;
            result.SetPosition(0, Vector3.zero);
            result.SetPosition(1, 0.5f * Vector3.forward);
            result.material = new Material(Shader.Find("Unlit/Color"))
            {
                color = new Color(1.0f, 1.0f, 1.0f, 0.5f),
            };
            return result.transform;
        }

        private MeshRenderer CreateHandIcon(SteamVR_Behaviour_Pose parentPose)
        {
            var result = new GameObject(gameObject.name + nameof(MeshRenderer)).AddComponent<MeshRenderer>();
            result.transform.SetParent(parentPose.transform);
            result.transform.localPosition = Vector3.forward * 0.05f;
            result.transform.localRotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);
            result.transform.localScale = 0.1f * Vector3.one;
            var meshFilter = result.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            var material = new Material(Shader.Find("Unlit/Transparent"))
            {
                mainTexture = null,
            };
            result.material = material;
            result.gameObject.SetActive(false);
            return result;
        }

        private float LastUpdateStateFrameCount { get; set; }
        private VRHandControllerState _state = new VRHandControllerState();
        private VRHandControllerPoseSmoother LLaserSmoother { get; set; } = new VRHandControllerPoseSmoother(2);
        private VRHandControllerPoseSmoother RLaserSmoother { get; set; } = new VRHandControllerPoseSmoother(2);

        private void UpdateState()
        {
            if (!enabled) return;
            if (LastUpdateStateFrameCount == Time.frameCount) return;
            LastUpdateStateFrameCount = Time.frameCount;

            _state.LeftPosition = SteamVR_Actions.default_Pose.GetLocalPosition(SteamVR_Input_Sources.LeftHand);
            _state.LeftPositionDelta = _state.LeftPosition - SteamVR_Actions.default_Pose.GetLastLocalPosition(SteamVR_Input_Sources.LeftHand);
            LLaserSmoother.AddTransform(LLaserPointer.transform);
            _state.LeftLaserPosition = LLaserSmoother.GetPosition();
            _state.LeftLaserRotation = LLaserSmoother.GetRotation();
            _state.LeftJoystickAxis = SteamVR_Actions.default_Move.GetAxis(SteamVR_Input_Sources.LeftHand);
            _state.LeftJoystickAxisDelta = SteamVR_Actions.default_Move.GetAxisDelta(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftTriggerOn = SteamVR_Actions.default_TriggerOn.GetState(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftTriggerDown = SteamVR_Actions.default_TriggerOn.GetStateDown(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftTriggerUp = SteamVR_Actions.default_TriggerOn.GetStateUp(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftGripOn = SteamVR_Actions.default_GripOn.GetState(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftGripDown = SteamVR_Actions.default_GripOn.GetStateDown(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftGripUp = SteamVR_Actions.default_GripOn.GetStateUp(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftJoystickClick = SteamVR_Actions.default_JoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftJoystickDown = SteamVR_Actions.default_JoystickClick.GetStateDown(SteamVR_Input_Sources.LeftHand);
            _state.IsLeftJoystickUp = SteamVR_Actions.default_JoystickClick.GetStateUp(SteamVR_Input_Sources.LeftHand);

            _state.RightPosition = SteamVR_Actions.default_Pose.GetLocalPosition(SteamVR_Input_Sources.RightHand);
            _state.RightPositionDelta = _state.RightPosition - SteamVR_Actions.default_Pose.GetLastLocalPosition(SteamVR_Input_Sources.RightHand);
            RLaserSmoother.AddTransform(RLaserPointer.transform);
            _state.RightLaserPosition = RLaserSmoother.GetPosition();
            _state.RightLaserRotation = RLaserSmoother.GetRotation();
            _state.RightJoystickAxis = SteamVR_Actions.default_Move.GetAxis(SteamVR_Input_Sources.RightHand);
            _state.RightJoystickAxisDelta = SteamVR_Actions.default_Move.GetAxisDelta(SteamVR_Input_Sources.RightHand);
            _state.IsRightTriggerOn = SteamVR_Actions.default_TriggerOn.GetState(SteamVR_Input_Sources.RightHand);
            _state.IsRightTriggerDown = SteamVR_Actions.default_TriggerOn.GetStateDown(SteamVR_Input_Sources.RightHand);
            _state.IsRightTriggerUp = SteamVR_Actions.default_TriggerOn.GetStateUp(SteamVR_Input_Sources.RightHand);
            _state.IsRightGripOn = SteamVR_Actions.default_GripOn.GetState(SteamVR_Input_Sources.RightHand);
            _state.IsRightGripDown = SteamVR_Actions.default_GripOn.GetStateDown(SteamVR_Input_Sources.RightHand);
            _state.IsRightGripUp = SteamVR_Actions.default_GripOn.GetStateUp(SteamVR_Input_Sources.RightHand);
            _state.IsRightJoystickClick = SteamVR_Actions.default_JoystickClick.GetState(SteamVR_Input_Sources.RightHand);
            _state.IsRightJoystickDown = SteamVR_Actions.default_JoystickClick.GetStateDown(SteamVR_Input_Sources.RightHand);
            _state.IsRightJoystickUp = SteamVR_Actions.default_JoystickClick.GetStateUp(SteamVR_Input_Sources.RightHand);

            _state.IsButtonAOn = SteamVR_Actions.default_A.GetState(SteamVR_Input_Sources.Any);
            _state.IsButtonADown = SteamVR_Actions.default_A.GetStateDown(SteamVR_Input_Sources.Any);
            _state.IsButtonAUp = SteamVR_Actions.default_A.GetStateUp(SteamVR_Input_Sources.Any);
            _state.IsButtonBOn = SteamVR_Actions.default_B.GetState(SteamVR_Input_Sources.Any);
            _state.IsButtonBDown = SteamVR_Actions.default_B.GetStateDown(SteamVR_Input_Sources.Any);
            _state.IsButtonBUp = SteamVR_Actions.default_B.GetStateUp(SteamVR_Input_Sources.Any);
            _state.IsButtonXOn = SteamVR_Actions.default_X.GetState(SteamVR_Input_Sources.Any);
            _state.IsButtonXDown = SteamVR_Actions.default_X.GetStateDown(SteamVR_Input_Sources.Any);
            _state.IsButtonXUp = SteamVR_Actions.default_X.GetStateUp(SteamVR_Input_Sources.Any);
            _state.IsButtonYOn = SteamVR_Actions.default_Y.GetState(SteamVR_Input_Sources.Any);
            _state.IsButtonYDown = SteamVR_Actions.default_Y.GetStateDown(SteamVR_Input_Sources.Any);
            _state.IsButtonYUp = SteamVR_Actions.default_Y.GetStateUp(SteamVR_Input_Sources.Any);

            var leftPositionDeltaDistance = _state.LeftPositionDelta.magnitude;
            var rightPositionDeltaDistance = _state.RightPositionDelta.magnitude;
            if (0.01f < leftPositionDeltaDistance || 0.01f < rightPositionDeltaDistance)
            {
                if (leftPositionDeltaDistance != rightPositionDeltaDistance)
                {
                    IsLeftHandActive = rightPositionDeltaDistance < leftPositionDeltaDistance;
                    LLaserPointer.gameObject.SetActive(IsLeftHandActive);
                    LHandIcon.enabled = IsLeftHandActive;

                    IsRightHandActive = leftPositionDeltaDistance < rightPositionDeltaDistance;
                    RLaserPointer.gameObject.SetActive(IsRightHandActive);
                    RHandIcon.enabled = IsRightHandActive;
                }
            }

            var leftJoystickAxisDeltaDistance = _state.LeftJoystickAxisDelta.magnitude;
            var rightJoystickAxisDeltaDistance = _state.RightJoystickAxisDelta.magnitude;
            if (0f < leftJoystickAxisDeltaDistance || 0f < rightJoystickAxisDeltaDistance)
                _state.JoystickAxis = (leftJoystickAxisDeltaDistance < rightJoystickAxisDeltaDistance) ? _state.RightJoystickAxis : _state.LeftJoystickAxis;
        }

        private RaycastHit? WideCast(Vector3 position, Vector3 forward, Vector3 right, Vector3 up, Collider targetCollider, float width, int xnum, int ynum, float maxDistance)
        {
            float stepX = width / (xnum - 1);
            float stepY = width / (ynum - 1);

            var closestDistance = maxDistance;
            RaycastHit? result = null;
            for (var x = 0; x < xnum; x++)
            {
                for (var y = 0; y < ynum; y++)
                {
                    var offset = right * (x * stepX - width * 0.5f) + up * (y * stepY - width * 0.5f);
                    var ray = new Ray(position + offset, forward);
                    if (targetCollider.Raycast(ray, out var hit, maxDistance) && hit.distance < closestDistance)
                    {
                        result = hit;
                        closestDistance = hit.distance;
                    }
                }
            }

            return result;
        }
    }
}
