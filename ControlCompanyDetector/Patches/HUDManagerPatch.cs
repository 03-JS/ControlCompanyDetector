using HarmonyLib;

namespace ControlCompanyDetector.Patches
{
    internal class HUDManagerPatch
    {
        public static bool displayTip;

        [HarmonyPatch(typeof(HUDManager), "CanTipDisplay")]
        [HarmonyPrefix]
        public static bool CanTipDisplay(string prefsKey, ref bool __result)
        {
            if (displayTip)
            {
                return true;
            }
            __result = false;
            return false;
        }
    }
}
