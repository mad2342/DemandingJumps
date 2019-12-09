using BattleTech;

namespace DemandingJumps
{
    public static class Utilities
    {
        public static int GetAttackerJumpedAccuracyModifier(AbstractActor attacker)
        {
            if (attacker != null)
            {
                if (attacker.UnitType == UnitType.Mech)
                {
                    WeightClass weightClass = ((Mech)attacker).weightClass;

                    switch (weightClass)
                    {
                        case WeightClass.LIGHT:
                            return DemandingJumps.Settings.ToHitSelfJumpedPenalties[0];

                        case WeightClass.MEDIUM:
                            return DemandingJumps.Settings.ToHitSelfJumpedPenalties[1];

                        case WeightClass.HEAVY:
                            return DemandingJumps.Settings.ToHitSelfJumpedPenalties[2];

                        case WeightClass.ASSAULT:
                            return DemandingJumps.Settings.ToHitSelfJumpedPenalties[3];

                        default:
                            break;
                    }
                }
            }
            return 0;
        }
    }
}
