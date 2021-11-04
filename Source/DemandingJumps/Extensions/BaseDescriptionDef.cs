using BattleTech;
using Harmony;

namespace DemandingJumps.Extensions
{
    internal static class BaseDescriptionDefExtensions
    {
        public static void SetDetails(this BaseDescriptionDef baseDescriptionDef, string details)
        {
            new Traverse(baseDescriptionDef).Property("Details").SetValue(details);
        }
    }
}
