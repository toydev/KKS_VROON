using BepInEx;
using KKAPI;

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
            Logger.LogInfo("Awake");
        }
    }
}
