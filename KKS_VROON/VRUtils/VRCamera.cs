using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using Valve.VR;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class VRCamera : MonoBehaviour
    {
        #region Base head position / rotation
        public static Vector3 BaseHeadLocalPosition { get; private set; }
        public static Quaternion BaseHeadLocalRotation { get; private set; }

        // Set the HMD's position / rotation at runtime as a base
        public static void UpdateBaseHeadLocalValues()
        {
            if (VRUtils.VR.Initialized)
            {
                var devices = new List<InputDevice>();
                var characteristics = InputDeviceCharacteristics.HeadMounted;
                InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

                if (0 < devices.Count)
                {
                    var device = devices[0];
                    if (device.TryGetFeatureValue(CommonUsages.devicePosition, out var position))
                    {
                        BaseHeadLocalPosition = position;
                    }

                    if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation))
                    {
                        BaseHeadLocalRotation = rotation;
                    }
                }
            }
        }
        #endregion

        public static VRCamera Create(GameObject parentGameObject, string name, int depth, bool withCurtain = true)
        {
            var gameObject = new GameObject($"{parentGameObject.name}{name}");
            // Synchronized lifecycle
            gameObject.transform.parent = parentGameObject.transform;
            gameObject.SetActive(false);
            var result = gameObject.AddComponent<VRCamera>();
            result.Depth = depth;
            result.WithCurtain = withCurtain;
            gameObject.SetActive(true);
            return result;
        }

        public void Hijack(Camera parentCamera)
        {
            Setup();

            CameraHijacker.Hijack(parentCamera, Normal);
            Normal.depth = Depth;

            if (VRUtils.VR.Initialized)
            {
                VR.origin.rotation = parentCamera.transform.rotation * Quaternion.Inverse(BaseHeadLocalRotation);
                VR.origin.position = parentCamera.transform.position - VR.origin.rotation * BaseHeadLocalPosition;
                VR.origin.SetParent(parentCamera.transform);
            }
            else
            {
                Normal.transform.SetParent(parentCamera.transform);
                Normal.transform.position = parentCamera.transform.position;
                Normal.transform.rotation = parentCamera.transform.rotation;
            }
        }

        public Camera Normal { get; private set; }
        public SteamVR_Camera VR { get; private set; }

        #region Implementations
        private int Depth { get; set; }
        private bool WithCurtain { get; set; }
        private GameObject CameraObject { get; set; }
        private void Setup()
        {
            if (!Normal)
            {
                PluginLog.Info($"Setup: {name}");

                CameraObject = new GameObject($"{name}Internal");
                Normal = CameraObject.AddComponent<Camera>();
                Normal.depth = Depth;
                if (VRUtils.VR.Initialized)
                {
                    VR = CameraObject.AddComponent<SteamVR_Camera>();
                    var trackedPose = CameraObject.AddComponent<TrackedPoseDriver>();
                    trackedPose.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Center);
                    trackedPose.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
                    trackedPose.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                }

                if (WithCurtain)
                {
                    PluginLog.Info("Add CameraCurtain");
                    CameraObject.GetOrAddComponent<CameraCurtain>();
                }
                else
                {
                    PluginLog.Info("No CameraCurtain");
                }
            }
        }

        void Awake()
        {
            PluginLog.Info($"Awake: {name}");
            Setup();
        }

        void OnDestroy()
        {
            PluginLog.Info($"OnDestroy: {name}");
            if (CameraObject) Destroy(CameraObject);
        }
        #endregion
    }
}
