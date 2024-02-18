using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(EnemyVent))]
    internal class EnemyVentPatch
    {
        public static int openVentCount;

        [HarmonyPatch("OpenVentClientRpc")]
        [HarmonyPostfix]
        static void GetOpenVentCount()
        {
            openVentCount++;
        }
    }
}
