using System;
using BattleTech;
using BattleTech.UI;
using DemandingJumps.Extensions;
using Harmony;

namespace DemandingJumps.Patches
{
    class DisableCalledShot
    {
        // Append text to tooltip depending on mod's settings
        [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "Init")]
        public static class CombatHUDMechwarriorTray_Init_Patch
        {
            public static bool Prepare()
            {
                return DemandingJumps.Settings.DisableCalledShots;
            }

            public static void Prefix(CombatHUDMechwarriorTray __instance, CombatGameState Combat)
            {
                try
                {
                    Logger.Debug($"[CombatHUDMechwarriorTray_Init_PREFIX] Overriding CombatGameState.Constants.CombatUIConstants.MoraleAttackDescription.Details");

                    string org = Combat.Constants.CombatUIConstants.MoraleAttackDescription.Details;
                    string ovr = $"{org}Cannot take Called Shots after jumping.\n";

                    Combat.Constants.CombatUIConstants.MoraleAttackDescription.SetDetails(ovr);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }



        [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetMechwarriorButtons")]
        public static class CombatHUDMechwarriorTray_ResetMechwarriorButtons_Patch
        {
            public static bool Prepare()
            {
                return DemandingJumps.Settings.DisableCalledShots;
            }

            public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor, CombatGameState ___Combat)
            {
                try
                {
                    if (actor == null)
                    {
                        return;
                    }

                    Logger.Debug($"[CombatHUDMechwarriorTray_ResetMechwarriorButtons_POSTFIX] actor.DisplayName: {actor.DisplayName}");
                    Logger.Debug($"[CombatHUDMechwarriorTray_ResetMechwarriorButtons_POSTFIX] actor.HasJumpedThisRound: {actor.HasJumpedThisRound}");
                    Logger.Debug($"[CombatHUDMechwarriorTray_ResetMechwarriorButtons_POSTFIX] actor.JumpedLastRound: {actor.JumpedLastRound}");

                    CombatHUDActionButton[] ___MoraleButtons = (CombatHUDActionButton[])AccessTools.Property(typeof(CombatHUDMechwarriorTray), "MoraleButtons").GetValue(__instance, null);

                    if (actor.HasJumpedThisRound || actor.JumpedLastRound)
                    {
                        ___MoraleButtons[0].DisableButton();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}
