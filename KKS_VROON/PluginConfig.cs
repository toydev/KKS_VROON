using BepInEx.Configuration;
using UnityEngine;

namespace KKS_VROON
{
    public class PluginConfig : MonoBehaviour
    {
        public static ConfigEntry<bool> GameWindowTopMost { get; private set; }
        public static ConfigEntry<bool> EnableMirror { get; private set; }
        public static ConfigEntry<float> MainGameAxisScalingFactor { get; private set; }
        public static ConfigEntry<float> CharaStudioAxisScalingFactor { get; private set; }

        public static float AxisScalingFactor { get => Plugin.GameMode == KKAPI.GameMode.Studio ? CharaStudioAxisScalingFactor.Value : MainGameAxisScalingFactor.Value; }

        public static void Setup(ConfigFile config)
        {
            GameWindowTopMost = config.Bind(
                "General",
                nameof(GameWindowTopMost),
                true,
                "Set the game window to always be on top.");
            EnableMirror = config.Bind(
                "General",
                nameof(EnableMirror),
                false,
                "Enable or disable mirror in VR");
            MainGameAxisScalingFactor = config.Bind(
                "MainGame",
                "AxisScalingFactor",
                30.0f,
                "Scaling factor for hand controller axis in MainGame");
            CharaStudioAxisScalingFactor = config.Bind(
                "Studio",
                "AxisScalingFactor",
                50.0f,
                "Scaling factor for hand controller axis in CharaStudio");
        }
    }
}
