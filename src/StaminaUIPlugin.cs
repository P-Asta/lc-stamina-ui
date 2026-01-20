using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using LethalConfig;
using OoLunar.StaminaUI.Patches;

namespace OoLunar.StaminaUI
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class StaminaUIPlugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

        internal static ManualLogSource StaticLogger = null!;
        internal static ConfigEntry<string>? TextColorHex;

        [SuppressMessage("Roslyn", "IDE0051", Justification = "Unity will call this method through reflection. Should've been an interface method but w/e.")]
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogDebug($"{MyPluginInfo.PLUGIN_NAME} started loading!");
            StaticLogger = Logger;

            // Config setup
            TextColorHex = Config.Bind(
                "UI",
                "TextColorHex",
                "FF0000",
                "Text color in hex format (e.g., FF0000 for red, 00FF00 for green, 0000FF for blue). Supports 6-digit hex (RRGGBB) or 8-digit hex (RRGGBBAA) for alpha."
            );

            // LethalConfig integration
            try
            {
                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ainavt.lc.lethalconfig"))
                {
                    LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(TextColorHex, requiresRestart: false));
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogWarning($"Failed to initialize LethalConfig integration: {ex.Message}");
            }

            // Find all types in this assembly that have the LethalPatchAttribute and patch them.
            foreach (Type type in typeof(StaminaUIPlugin).Assembly.GetTypes())
            {
                if (type.GetCustomAttribute<LethalPatchAttribute>() is not null)
                {
                    _harmony.PatchAll(type);
                }
            }
            Logger.LogDebug($"|\\_/|\n`o.o'\n=(_)=\n  U");
            Logger.LogInfo($"ᓚᘏᗢ --- N O Nyang E --- ᓚᘏᗢ");
        }
    }
}
