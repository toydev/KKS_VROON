using BepInEx.Configuration;
using UnityEngine;

namespace KKS_VROON
{
    public class PluginConfig : MonoBehaviour
    {
        public static ConfigEntry<bool> GameWindowTopMost;

        public static void Setup(ConfigFile config)
        {
            GameWindowTopMost = config.Bind(
                "General",
                nameof(GameWindowTopMost),
                true,
                "Set the game window to always be on top.");
        }
    }
}
