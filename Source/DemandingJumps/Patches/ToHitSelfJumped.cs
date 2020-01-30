using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace DemandingJumps.Patches
{
    class ToHitSelfJumped
    {
        /*
        [HarmonyPatch(typeof(AbstractActor), "OnMovePhaseComplete")]
        public static class AbstractActor_OnMovePhaseComplete_Patch
        {
            static void Postfix(AbstractActor __instance)
            {
                try
                {
                    if (__instance.Combat.LocalPlayerTeam.IsActive)
                    {
                        //Logger.Debug("[AbstractActor_OnMovePhaseComplete_POSTFIX] __instance.Combat.LocalPlayerTeam.IsActive: " + __instance.Combat.LocalPlayerTeam.IsActive);

                        Fields.JumpPreview = false;
                        Logger.Debug("[AbstractActor_OnMovePhaseComplete_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(CombatSelectionHandler), "TrySelectActor")]
        public static class CombatSelectionHandler_TrySelectActor_Patch
        {
            // NOTE: This is not called for AI
            static void Postfix(CombatSelectionHandler __instance, AbstractActor actor)
            {
                try
                {
                    //CombatGameState Combat = (CombatGameState)AccessTools.Property(typeof(CombatSelectionHandler), "Combat").GetValue(__instance, null);
                    //if (Combat.LocalPlayerTeam.IsActive)
                    //{
                        Fields.JumpPreview = false;
                        Logger.Debug("[CombatSelectionHandler_TrySelectActor_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                    //}
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(SelectionState), "GetNewSelectionStateByType")]
        public static class SelectionState_GetNewSelectionStateByType_Patch
        {
            // NOTE: This is not called for AI
            static void Postfix(SelectionState __instance, SelectionType type, AbstractActor actor)
            {
                try
                {
                    if (type == SelectionType.Jump)
                    {
                        Fields.JumpPreview = true;
                    }
                    else
                    {
                        Fields.JumpPreview = false;
                    }
                    Logger.Debug("[SelectionState_GetNewSelectionStateByType_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        */

        [HarmonyPatch(typeof(CombatSelectionHandler), "addNewState", new Type[] { typeof(SelectionState) })]
        public static class CombatSelectionHandler_addNewState_Patch
        {
            // NOTE: This is not called for AI
            static void Postfix(CombatSelectionHandler __instance, SelectionState newState)
            {
                try
                {
                    Logger.Info("[CombatSelectionHandler_addNewState_POSTFIX] newState.SelectionType: " + newState.SelectionType);
                    Fields.JumpPreview = newState.SelectionType == SelectionType.Jump ? true : false;
                    Logger.Debug("[CombatSelectionHandler_addNewState_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        // Needed for right click during jump preview which falls back to a normal move...
        [HarmonyPatch(typeof(CombatSelectionHandler), "RemoveTopState", new Type[] {})]
        public static class CombatSelectionHandler_RemoveTopState_Patch
        {
            // NOTE: This is not called for AI
            static void Prefix(CombatSelectionHandler __instance)
            {
                try
                {
                    Logger.Info("[CombatSelectionHandler_RemoveTopState_PREFIX] __instance.ActiveState.SelectionType: " + __instance.ActiveState.SelectionType);
                    if (__instance.ActiveState.SelectionType == SelectionType.Jump)
                    {
                        Fields.JumpPreview = false;
                    }
                    Logger.Debug("[CombatSelectionHandler_RemoveTopState_PREFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ToHit), "GetAllModifiers")]
        public static class ToHit_GetAllModifiers_Patch
        {
            public static void Postfix(ToHit __instance, ref float __result, CombatGameState ___combat, AbstractActor attacker, Weapon weapon, ICombatant target)
            {
                try
                {
                    if (DemandingJumps.Settings.ToHitSelfJumpedSpareAI && !___combat.LocalPlayerTeam.IsActive)
                    {
                        return;
                    }
                    Logger.Info("[ToHit_GetAllModifiers_POSTFIX] CombatGameState.LocalPlayerTeam.IsActive: " + ___combat.LocalPlayerTeam.IsActive);



                    bool AttackerJumpedThisRound = attacker.HasMovedThisRound && attacker.JumpedLastRound;
                    Logger.Info("[ToHit_GetAllModifiers_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                    Logger.Info("[ToHit_GetAllModifiers_POSTFIX] AttackerJumpedThisRound: " + AttackerJumpedThisRound);

                    if (Fields.JumpPreview || AttackerJumpedThisRound)
                    {
                        int ToHitSelfJumpedModifier = Utilities.GetAttackerJumpedAccuracyModifier(attacker);
                        Logger.Info("[ToHit_GetAllModifiers_POSTFIX] Unit previews jump or already jumped. Applying ToHit penalty.");
                        __result = __result + (float)ToHitSelfJumpedModifier;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(ToHit), "GetAllModifiersDescription")]
        public static class ToHit_GetAllModifiersDescription_Patch
        {
            public static void Postfix(ToHit __instance, ref string __result, CombatGameState ___combat, AbstractActor attacker, Weapon weapon, ICombatant target)
            {
                try
                {
                    if (DemandingJumps.Settings.ToHitSelfJumpedSpareAI && !___combat.LocalPlayerTeam.IsActive)
                    {
                        return;
                    }
                    Logger.Info("[ToHit_GetAllModifiersDescription_POSTFIX] CombatGameState.LocalPlayerTeam.IsActive: " + ___combat.LocalPlayerTeam.IsActive);



                    bool AttackerJumpedThisRound = attacker.HasMovedThisRound && attacker.JumpedLastRound;
                    Logger.Info("[ToHit_GetAllModifiersDescription_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                    Logger.Info("[ToHit_GetAllModifiersDescription_POSTFIX] AttackerJumpedThisRound: " + AttackerJumpedThisRound);

                    if (AttackerJumpedThisRound)
                    {
                        int ToHitSelfJumpedModifier = Utilities.GetAttackerJumpedAccuracyModifier(attacker);
                        Logger.Debug("[ToHit_GetAllModifiersDescription_POSTFIX] Add description for ToHitSelfJumped: " + ToHitSelfJumpedModifier);
                        __result = string.Format("{0}JUMPED {1:+#;-#}; ", __result, ToHitSelfJumpedModifier);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        [HarmonyPatch(typeof(CombatHUDWeaponSlot), "SetHitChance", new Type[] { typeof(ICombatant) })]
        public static class CombatHUDWeaponSlot_SetHitChance_Patch
        {
            public static void Postfix(CombatHUDWeaponSlot __instance, CombatGameState ___Combat, ICombatant target)
            {
                try
                {
                    if (DemandingJumps.Settings.ToHitSelfJumpedSpareAI && !___Combat.LocalPlayerTeam.IsActive)
                    {
                        return;
                    }
                    Logger.Info("[CombatHUDWeaponSlot_SetHitChance_POSTFIX] CombatGameState.LocalPlayerTeam.IsActive: " + ___Combat.LocalPlayerTeam.IsActive);



                    AbstractActor attacker = __instance.DisplayedWeapon.parent;
                    bool AttackerJumpedThisRound = attacker.HasMovedThisRound && attacker.JumpedLastRound;
                    Logger.Info("[CombatHUDWeaponSlot_SetHitChance_POSTFIX] Fields.JumpPreview: " + Fields.JumpPreview);
                    Logger.Info("[CombatHUDWeaponSlot_SetHitChance_POSTFIX] AttackerJumpedThisRound: " + AttackerJumpedThisRound);

                    if (AttackerJumpedThisRound)
                    {
                        int ToHitSelfJumpedModifier = Utilities.GetAttackerJumpedAccuracyModifier(attacker);
                        Traverse AddToolTipDetail = Traverse.Create(__instance).Method("AddToolTipDetail", "SELF JUMPED", ToHitSelfJumpedModifier);
                        Logger.Info($"[CombatHUDWeaponSlot_SetHitChance_POSTFIX] Invoking AddToolTipDetail for ToHitSelfJumped: {ToHitSelfJumpedModifier}");
                        AddToolTipDetail.GetValue();
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
