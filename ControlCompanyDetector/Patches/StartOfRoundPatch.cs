﻿using ControlCompanyDetector.Logic;
using HarmonyLib;
using UnityEngine;

namespace ControlCompanyDetector.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch : MonoBehaviour
    {
        [HarmonyPatch("OnPlayerConnectedClientRpc")]
        [HarmonyPostfix]
        static void PatchOnPlayerConnected()
        {
            if (HUDManager.Instance != null /*&& Player.HostPlayer != null*/)
            {
                CoroutineManager.StartCoroutine(Detector.StartDetection());
            }
        }
    }
}
