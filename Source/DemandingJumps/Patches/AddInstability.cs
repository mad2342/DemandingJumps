using System;
using BattleTech;
using Harmony;

namespace DemandingJumps.Patches
{
    class AddInstability
    {
        // Info
        [HarmonyPatch(typeof(Mech), "GetMinStability", new Type[] { typeof(float), typeof(int) })]
        public static class Mech_GetMinStability_Patch
        {
            static void Postfix(Mech __instance, float __result, float oldStability, int numLevelsToDump)
            {
                try
                {
                    Logger.LogLine("[Mech_GetMinStability_POSTFIX] __instance.DisplayName: " + __instance.DisplayName);
                    Logger.LogLine("[Mech_GetMinStability_POSTFIX] oldStability: " + oldStability);
                    Logger.LogLine("[Mech_GetMinStability_POSTFIX] numLevelsToDump: " + numLevelsToDump);

                    float levelValue = __instance.MaxStability / (float)__instance.Combat.Constants.ResolutionConstants.StabilityLevels;
                    Logger.LogLine("[Mech_GetMinStability_POSTFIX] levelValue: " + levelValue);

                    Logger.LogLine("[Mech_GetMinStability_POSTFIX] __result: " + __result);
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(Mech), "AddInstability")]
        public static class Mech_AddInstability_Patch
        {
            static void Postfix(Mech __instance, ref float amt, StabilityChangeSource source)
            {
                try
                {
                    Logger.LogLine("[Mech_AddInstability_POSTFIX] __instance.DisplayName: " + __instance.DisplayName);
                    Logger.LogLine("[Mech_AddInstability_POSTFIX] amt: " + amt);
                    Logger.LogLine("[Mech_AddInstability_POSTFIX] source: " + source);

                    if (source != StabilityChangeSource.Jumping)
                    {
                        return;
                    }

                    float maxStability = __instance.MaxStability;
                    Logger.LogLine("[Mech_AddInstability_PREFIX] maxStability: " + maxStability);
                    float currentStability = (float)AccessTools.Property(typeof(Mech), "_stability").GetValue(__instance, null);
                    Logger.LogLine("[Mech_AddInstability_PREFIX] currentStability: " + currentStability);
                    float unsteadyThreshold = __instance.UnsteadyThreshold;
                    Logger.LogLine("[Mech_AddInstability_PREFIX] unsteadyThreshold: " + unsteadyThreshold);
                    float percentStability = __instance.StabilityPercentage;
                    Logger.LogLine("[Mech_AddInstability_PREFIX] percentStability: " + percentStability);


                    __instance.NeedsInstabilityCheck = true;

                    // Fails if unit is already unsteady before the jump -> gains evasion even though unsteady
                    //__instance.CheckForInstability();
                    
                    if (currentStability > unsteadyThreshold)
                    {
                        // Add floatie again if unit WAS ALREADY unsteady and still is after jumping
                        if (__instance.IsUnsteady)
                        {
                            __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, "UNSTEADY", FloatieMessage.MessageNature.Debuff));
                        }

                        // Would make Mech fall if attacked by anything in this state
                        /*
                        if (__instance.IsUnsteady && percentStability >= 1f )
                        {
                            __instance.Combat.MessageCenter.PublishMessage(new FloatieMessage(__instance.GUID, __instance.GUID, "OFF BALANCE", FloatieMessage.MessageNature.Debuff));
                            __instance.FlagForKnockdown();
                        }
                        */

                        __instance.ApplyUnsteady();
                    }



                    // @ToDo: Fix UI: CombatHUDFilledBarBase.Update
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
