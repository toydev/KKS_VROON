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
    [BepInPlugin(nameof(KKS_VROON), "VROON", "1.1")]
    [BepInProcess(KoikatuAPI.GameProcessName)]
    [BepInProcess(KoikatuAPI.StudioProcessName)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class Plugin : BaseUnityPlugin
    {
        private void Setup()
        {
            if (PluginConfig.GameWindowTopMost.Value) WindowUtils.MakeWindowTopMost();
            new Harmony(nameof(KKS_VROON)).PatchAll();
            VRCamera.UpdateBaseHeadLocalValues();
            ScenePluginManager.Initialize();
        }

        void Awake()
        {
            PluginLog.Setup(Logger);
            PluginConfig.Setup(Config);

            if (WindowUtils.InitializeGameWindowHandle())
            {
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
