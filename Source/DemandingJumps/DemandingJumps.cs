﻿using Harmony;
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
        // BEN: DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        internal static int DebugLevel = 1;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;
            LogPath = Path.Combine(ModDirectory, "DemandingJumps.log");

            Logger.Initialize(LogPath, DebugLevel, ModDirectory, nameof(DemandingJumps));

            try
            {
                Settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch (Exception e)
            {
                Settings = new Settings();
                Logger.Error(e);
            }

            // Harmony calls need to go last here because their Prepare() methods directly check Settings...
            HarmonyInstance harmony = HarmonyInstance.Create("de.mad.DemandingJumps");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
