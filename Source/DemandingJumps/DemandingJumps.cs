using Harmony;
using System.Reflection;
using System;
using Newtonsoft.Json;
using System.IO;

namespace DemandingJumps
{
    public static class DemandingJumps
    {
        internal static string LogPath;
        internal static string ModDirectory;
        internal static Settings Settings;

        // BEN: Debug (0: nothing, 1: errors, 2:all)
        internal static int DebugLevel = 2;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;

            LogPath = Path.Combine(ModDirectory, "DemandingJumps.log");
            File.CreateText(DemandingJumps.LogPath);

            try
            {
                Settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch (Exception)
            {
                Settings = new Settings();
            }

            // Harmony calls need to go last here because their Prepare() methods directly check Settings...
            HarmonyInstance harmony = HarmonyInstance.Create("de.mad.DemandingJumps");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
