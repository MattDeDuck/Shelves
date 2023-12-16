using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;

namespace Shelves
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, "1.1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        // Logging source
        public static ManualLogSource Log { get; set; }

        // Get the plugin location
        public static string pluginLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Log = this.Logger;

            // Make sure Harmony patches
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Harmony.CreateAndPatchAll(typeof(Functions));
            Harmony.CreateAndPatchAll(typeof(Storage));
            Harmony.CreateAndPatchAll(typeof(GamePatchManager));
        }
    }   
}