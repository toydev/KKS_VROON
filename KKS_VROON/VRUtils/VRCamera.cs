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

        public void Link(Camera parentCamera)
        {
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

        void Awake()
        {
            PluginLog.Info($"Awake: {name}");

            Normal = gameObject.AddComponent<Camera>();
            if (VRUtils.VR.Initialized)
            {
                VR = gameObject.AddComponent<SteamVR_Camera>();
                var trackedPose = gameObject.AddComponent<TrackedPoseDriver>();
                trackedPose.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Center);
                trackedPose.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
                trackedPose.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
            }
        }

        void OnDestroy()
        {
            PluginLog.Info($"OnDestroy: {name}");
        }
    }
}
