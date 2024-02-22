using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch
    {
        [HarmonyPatch("ConfirmHostButton")]
        [HarmonyPrefix]
        static void CheckIfHostCanDetectEnemySpawning()
        {
            if (Plugin.detectEnemySpawningAsHost.Value)
            {
                Plugin.GetLoadedMods();
            }
        }
    }
}
