using System;
using System.Collections;

using Unity.XR.OpenVR;
using UnityEngine;
using Valve.VR;

using KKS_VROON.Logging;

namespace KKS_VROON.VRUtils
{
    public class VR : MonoBehaviour
    {
        #region Initialize
        public static bool Initializing { get; private set; } = false;
        public static bool Initialized { get; private set; } = false;

        public static void Initialize(Action actionAfterInitialization, bool force = false)
        {
            if (!Initializing && (force || !Initialized))
            {
                Initializing = true;
                Initialized = false;
                ActionAfterInitialization = actionAfterInitialization;
                new GameObject(nameof(VR)).AddComponent<VR>();
            }
        }
        #endregion

        private static Action ActionAfterInitialization { get; set; }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            PluginLog.Debug("Start Setup");

            try
            {
                var vrSettings = OpenVRSettings.GetSettings();
                vrSettings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;
                vrSettings.InitializationType = OpenVRSettings.InitializationTypes.Scene;
                SteamVR_Settings.instance.autoEnableVR = true;

                var vrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();
                if (vrLoader.Initialize())
                {
                    PluginLog.Debug("OpenVR initialization succeeded.");
                }
                else
                {
                    PluginLog.Debug("OpenVR initialization failed.");
                    yield break;
                }

                if (vrLoader.Start())
                {
                    PluginLog.Debug("OpenVR started.");
                }
                else
                {
                    PluginLog.Error("Could not start OpenVR.");
                    yield break;
                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e);
            }

            SteamVR_Behaviour.Initialize(false);
            while (true)
            {
                switch (SteamVR.initializedState)
                {
                    case SteamVR.InitializedStates.InitializeSuccess:
                        PluginLog.Debug("SteamVR initialization succeeded.");
                        break;
                    case SteamVR.InitializedStates.InitializeFailure:
                        PluginLog.Error("SteamVR initialization failed.");
                        yield break;
                    default:
                        yield return new WaitForSeconds(0.1f);
                        continue;
                }

                break;
            }

            // Wait a moment for HMD tracking.
            yield return new WaitForSeconds(0.1f);

            Initialized = true;
            ActionAfterInitialization?.Invoke();

            PluginLog.Debug("Finish Setup");
            Destroy(gameObject);
        }
    }
}
