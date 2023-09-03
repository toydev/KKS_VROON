using System;

using BepInEx;
using KKAPI;
using HarmonyLib;
using UnityEngine;

using KKS_VROON.Logging;
using KKS_VROON.VRUtils;
using KKS_VROON.ScenePlugins;
using KKS_VROON.WindowNativeUtils;

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
            SupportNativeWindow();
        }

        private void SupportNativeWindow()
        {
            var dialogCapture = gameObject.AddComponent<WindowCapture>();
            dialogCapture.TargetWindowHandle = hWnd => WindowUtils.GetWindowClassName(hWnd) == "#32770" && WindowUtils.NativeMethods.IsWindowVisible(hWnd);
            dialogCapture.TargetWindowRect = hWnd =>
            {
                var windowRect = WindowUtils.GetWindowRect(hWnd);
                var clientRect = WindowUtils.GetClientRect(hWnd);
                return new Rect(clientRect.xMin, windowRect.yMin, clientRect.xMax - clientRect.xMin, clientRect.yMax - windowRect.yMin);
            };
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
