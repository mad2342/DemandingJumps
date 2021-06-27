using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace DemandingJumps.Patches
{
    class DisableCalledShot
    {
        [HarmonyPatch(typeof(CombatHUDMechwarriorTray), "ResetMechwarriorButtons")]
        public static class CombatHUDMechwarriorTray_ResetMechwarriorButtons_Patch
        {
            public static void Postfix(CombatHUDMechwarriorTray __instance, AbstractActor actor)
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
