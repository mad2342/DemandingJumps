using System;
using BattleTech;
using Harmony;

namespace DemandingJumps.Patches
{
    class Instability
    {
        [HarmonyPatch(typeof(Mech), "AddInstability")]
        public static class Mech_AddInstability_Patch
        {
            static void Postfix(Mech __instance, float amt, StabilityChangeSource source, string sourceGuid)
            {
                try
                {
                    Logger.LogLine("[Mech_AddInstability_POSTFIX] __instance.DisplayName: " + __instance.DisplayName);
                    Logger.LogLine("[Mech_AddInstability_POSTFIX] amt: " + amt);
                    Logger.LogLine("[Mech_AddInstability_POSTFIX] source: " + source.ToString());

                    if (source == StabilityChangeSource.Jumping)
                    {
                        __instance.NeedsInstabilityCheck = true;
                        __instance.CheckForInstability();
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
