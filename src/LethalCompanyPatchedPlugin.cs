using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OoLunar.LethalCompanyPatched.Patches;

namespace OoLunar.LethalCompanyPatched
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public sealed class LethalCompanyPatchedPlugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

        internal static ManualLogSource StaticLogger = null!;


        [SuppressMessage("Roslyn", "IDE0051", Justification = "Unity will call this method through reflection. Should've been an interface method but w/e.")]
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogDebug($"{MyPluginInfo.PLUGIN_NAME} started loading!");
            StaticLogger = Logger;
            // Find all types in this assembly that have the LethalPatchAttribute and patch them.
            foreach (Type type in typeof(LethalCompanyPatchedPlugin).Assembly.GetTypes())
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
