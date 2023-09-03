using BepInEx.Configuration;
using UnityEngine;

namespace KKS_VROON
{
    public class PluginConfig : MonoBehaviour
    {
        public static ConfigEntry<bool> GameWindowTopMost { get; private set; }
        public static ConfigEntry<bool> EnableMirror { get; private set; }
        public static ConfigEntry<float> AxisScalingFactor { get; private set; }

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
            AxisScalingFactor = config.Bind(
                "Controls",
                "AxisScalingFactor",
                20.0f,
                "Scaling factor for hand controller axis");
        }
    }
}
