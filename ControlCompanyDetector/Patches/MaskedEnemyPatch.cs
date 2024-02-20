using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(MaskedPlayerEnemy))]
    internal class MaskedEnemyPatch
    {
        [HarmonyPatch("CreateMimicClientRpc")]
        [HarmonyPostfix]
        static void CreateMimicPatch()
        {
            RoundManagerPatch.maskDeaths++;
            // Plugin.LogInfoMLS("Mimic is being created!!!");
        }
    }
}
