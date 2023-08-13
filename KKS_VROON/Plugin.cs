using System;
using System.Diagnostics;
using System.Linq;

using BepInEx;
using KKAPI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;
using Valve.VR;

using KKS_VROON.Logging;
using KKS_VROON.VRUtils;

namespace KKS_VROON
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    [BepInProcess(KoikatuAPI.StudioProcessName)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = nameof(KKS_VROON);
        private const string Name = nameof(KKS_VROON);
        private const string Version = "0.01";

        void Awake()
        {
            PluginLog.Setup(Logger);

            try
            {
                if (!KoikatuAPI.IsVR() && IsSteamVRRunning) VR.Initialize(() => { });
            }
            catch (Exception e)
            {
                PluginLog.Error(e);
            }
        }

        private bool IsSteamVRRunning => Process.GetProcesses().Any(i => i.ProcessName == "vrcompositor");

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            // Scene switching calms down with Title
            if (scene.name == "Title")
            {
                if (VR.Initialized)
                {
                    // Try VR Camera
                    var cameraObject = new GameObject("TryVRCamera");
                    cameraObject.AddComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
                    cameraObject.AddComponent<SteamVR_Camera>();
                    var trackedPose = cameraObject.gameObject.AddComponent<TrackedPoseDriver>();
                    trackedPose.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Center);
                    trackedPose.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
                    trackedPose.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                }
                else
                {
                    PluginLog.Warning("Not initialized VR.");
                }
            }
        }

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
