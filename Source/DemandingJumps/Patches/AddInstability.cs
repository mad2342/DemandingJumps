﻿using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using UnityEngine;
using UnityEngine.UI;

namespace DemandingJumps.Patches
{
    class AddInstability
    {
        //UI
        [HarmonyPatch(typeof(CombatHUDPipBar), "ShowValue")]
        public static class CombatHUDPipBar_ShowValue_Patch
        {
            static void Postfix(CombatHUDPipBar __instance, float current, float projected, Color shownColor, Color shownColorProjectedHigh)
            {
                try
                {
                    if (__instance.GetComponentInParent(typeof(CombatHUDStabilityDisplay)) != null)
                    {
                        Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] THIS CombatHUDPipBar is a child of CombatHUDStabilityDisplay");

                        Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] current: " + current);
                        Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] projected: " + projected);

                        if (projected > current)
                        {
                            Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] INSTABILITY RISES: Fixing pips-preview");

                            float ___pointsPerPip = (float)typeof(CombatHUDPipBar).GetProperty("PointsPerPip", AccessTools.all).GetValue(__instance, null);
                            int pipCount = __instance.PipCount;
                            Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] ___pointsPerPip: " + ___pointsPerPip);
                            Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] pipCount: " + pipCount);
                            float calculatedMaxStability = pipCount * ___pointsPerPip;
                            Logger.LogLine("[CombatHUDPipBar_ShowValue_POSTFIX] calculatedMaxStability: " + calculatedMaxStability);

                            Color increaseStabilityWarningColor = Color.Lerp(shownColor, shownColorProjectedHigh, Mathf.Sin(Time.realtimeSinceStartup * __instance.TimeSinFactor) * 0.5f + 0.5f);
                            Color c = LazySingletonBehavior<UIManager>.Instance.UIColorRefs.red;
                            Color maxStabilityWarningColor = Color.Lerp(c, shownColorProjectedHigh, Mathf.Sin(Time.realtimeSinceStartup * __instance.TimeSinFactor) * 0.5f + 0.5f);
                            Color stabilityWarningColor = projected >= calculatedMaxStability ? maxStabilityWarningColor : increaseStabilityWarningColor;


                            List <Graphic> ___pips = (List<Graphic>)typeof(CombatHUDPipBar).GetProperty("Pips", AccessTools.all).GetValue(__instance, null);
                            for (int i = 0; i < ___pips.Count; i++)
                            {
                                ___pips[i].transform.localScale = Vector3.one;

                                if ( ((i+1) * ___pointsPerPip > current) && ((i+1) * ___pointsPerPip <= projected) )
                                {
                                    UIHelpers.SetImageColor(___pips[i], stabilityWarningColor);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        // Reference
        /*
        [HarmonyPatch(typeof(CombatHUDMechTray), "Update")]
        public static class CombatHUDMechTray_Update_Patch
        {

        static void Prefix(CombatHUDMechTray __instance)
        {
            try
            {
                if (__instance.DisplayedActor == null)
                {
                    return;
                }

                CombatHUDStabilityDisplay stabilityDisplay = __instance.ActorInfo.StabilityDisplay;
                float predictedStabilityPercentage = stabilityDisplay.PredictedTargetLevel;
                Mech mech = __instance.DisplayedActor as Mech;
                float currentStabilityPercentage = mech.StabilityPercentage;

                // Instability rises
                if (currentStabilityPercentage < predictedStabilityPercentage)
                {
                    Fields.StabilityPreview = true;
                }
                else
                {
                    Fields.StabilityPreview = false;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        static void Postfix(CombatHUDMechTray __instance)
        {
            try
            {
                //AbstractActor ____actor = (AbstractActor)AccessTools.Field(typeof(CombatHUDActorInfo), "displayedActor").GetValue(__instance.ActorInfo);
                //Logger.LogLine("[CombatHUDMechTray_Update_POSTFIX] ____actor?.DisplayName: " + ____actor?.DisplayName);


                Logger.LogLine("[CombatHUDMechTray_Update_POSTFIX] __instance.DisplayedActor?.DisplayName: " + __instance.DisplayedActor?.DisplayName);

                if(__instance.DisplayedActor == null)
                {
                    return;
                }

                CombatHUDStabilityDisplay stabilityDisplay = __instance.ActorInfo.StabilityDisplay;
                float predictedStabilityPercentage = stabilityDisplay.PredictedTargetLevel;
                Logger.LogLine("[CombatHUDMechTray_Update_POSTFIX] predictedStabilityPercentage: " + predictedStabilityPercentage);


                Mech mech = __instance.DisplayedActor as Mech;
                float currentStabilityPercentage = mech.StabilityPercentage;
                Logger.LogLine("[CombatHUDMechTray_Update_POSTFIX] currentStabilityPercentage: " + currentStabilityPercentage);

                // Instability rises
                if (currentStabilityPercentage < predictedStabilityPercentage)
                {
                    Logger.LogLine("[CombatHUDMechTray_Update_POSTFIX] Try to fix pips");

                    CombatHUDStabilityBarPips pipBar = (CombatHUDStabilityBarPips)AccessTools.Property(typeof(CombatHUDStabilityDisplay), "PipBar").GetValue(stabilityDisplay, null);
                    List<Graphic> pipBarPips = (List<Graphic>)typeof(CombatHUDStabilityBarPips).GetProperty("Pips", AccessTools.all).GetValue(pipBar, null);

                    //Vector3 fullScale = (Vector3)AccessTools.Field(typeof(CombatHUDStabilityBarPips), "fullScale").GetValue(pipBar);

                    Logger.LogLine("[CombatHUDMechTray_Update_POSTFIX] pipBarPips.Count: " + pipBarPips.Count);

                    for (int i = 0; i < pipBarPips.Count; i++)
                    {
                        pipBarPips[i].color = new Color(0, 0, 1, 0);
                        pipBarPips[i].transform.localScale = Vector3.one;
                    } 
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        }
        */

        // Info
        [HarmonyPatch(typeof(Mech), "GetMinStability", new Type[] { typeof(float), typeof(int) })]
        public static class Mech_GetMinStability_Patch
        {
            static void Postfix(Mech __instance, float __result, float oldStability, int numLevelsToDump)
            {
                try
                {
                    //Logger.LogLine("[Mech_GetMinStability_POSTFIX] __instance.DisplayName: " + __instance.DisplayName);
                    //Logger.LogLine("[Mech_GetMinStability_POSTFIX] oldStability: " + oldStability);
                    //Logger.LogLine("[Mech_GetMinStability_POSTFIX] numLevelsToDump: " + numLevelsToDump);

                    float levelValue = __instance.MaxStability / (float)__instance.Combat.Constants.ResolutionConstants.StabilityLevels;
                    //Logger.LogLine("[Mech_GetMinStability_POSTFIX] levelValue: " + levelValue);

                    //Logger.LogLine("[Mech_GetMinStability_POSTFIX] __result: " + __result);
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
                    //Logger.LogLine("[Mech_AddInstability_POSTFIX] amt: " + amt);
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
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }
}
