using System;

using BepInEx;
using KKAPI;

using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins;
using KKS_VROON.WindowNativeUtils;
using HarmonyLib;

namespace KKS_VROON
{
    [BepInPlugin(nameof(KKS_VROON), nameof(KKS_VROON), "1.1")]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    [BepInProcess(KoikatuAPI.StudioProcessName)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class Plugin : BaseUnityPlugin
    {
        private void Setup()
        {
            new Harmony(nameof(KKS_VROON)).PatchAll();
            VRCamera.UpdateBaseHeadLocalValues();
            ScenePluginManager.Initialize();
        }

        void Awake()
        {
            PluginLog.Setup(Logger);

            if (WindowUtils.InitializeGameWindowHandle())
            {
                PluginLog.Debug($"WindowRect: {WindowUtils.GetGameWindowRect()}");

                try
                {
                    // Debug vroon mode ... Apply plugin while on the 2D screen
                    if (Environment.CommandLine.Contains("--debug-vroon")) Setup();
                    else if (!KoikatuAPI.IsVR() && WindowUtils.IsSteamVRRunning()) VR.Initialize(() => Setup());
                }
                catch (Exception e)
                {
                    PluginLog.Error(e);
                }
            }
        }
    }
}
