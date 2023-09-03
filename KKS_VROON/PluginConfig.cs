using BepInEx.Configuration;
using UnityEngine;

namespace KKS_VROON
{
    public class PluginConfig : MonoBehaviour
    {
        public static ConfigEntry<bool> GameWindowTopMost { get; private set; }
        public static ConfigEntry<bool> EnableMirror { get; private set; }
        public static ConfigEntry<float> MainGameJoystickViewSpeed { get; private set; }
        public static ConfigEntry<float> CharaStudioJoystickViewSpeed { get; private set; }
        public static ConfigEntry<int> MouseWheelScalingFactor { get; private set; }

        public static float JoystickViewSpeed { get => Plugin.GameMode == KKAPI.GameMode.Studio ? CharaStudioJoystickViewSpeed.Value : MainGameJoystickViewSpeed.Value; }

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
            MainGameJoystickViewSpeed = config.Bind(
                "Controls",
                nameof(MainGameJoystickViewSpeed),
                30.0f,
                "Scaling factor for joystick-controlled view movement speed in MainGame");
            CharaStudioJoystickViewSpeed = config.Bind(
                "Controls",
                nameof(CharaStudioJoystickViewSpeed),
                50.0f,
                "Scaling factor for joystick-controlled view movement speed in CharaStudio");
            MouseWheelScalingFactor = config.Bind(
                "Controls",
                nameof(MouseWheelScalingFactor),
                120,
                "Scaling factor for mouse wheel");
        }
    }
}
