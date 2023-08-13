using System;
using System.Diagnostics;
using System.Linq;

using BepInEx;
using KKAPI;

using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins;

namespace KKS_VROON
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    [BepInProcess(KoikatuAPI.StudioProcessName)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class Plugin : BaseUnityPlugin
    {
        private void Setup()
        {
            VRCamera.UpdateBaseHeadLocalValues();
            ScenePluginManager.Initialize();
        }

        private const string GUID = nameof(KKS_VROON);
        private const string Name = nameof(KKS_VROON);
        private const string Version = "0.01";

        void Awake()
        {
            PluginLog.Setup(Logger);

            try
            {
                if (!KoikatuAPI.IsVR() && IsSteamVRRunning) VR.Initialize(() => Setup());
            }
            catch (Exception e)
            {
                PluginLog.Error(e);
            }
        }

        private bool IsSteamVRRunning => Process.GetProcesses().Any(i => i.ProcessName == "vrcompositor");
    }
}
