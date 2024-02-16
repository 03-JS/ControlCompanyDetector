using ControlCompanyDetector.Logic;
using HarmonyLib;
using Steamworks.Data;
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
            if (HUDManager.Instance != null && !StartOfRound.Instance.IsHost)
            {
                CoroutineManager.StartCoroutine(Detector.StartDetection());
            }
        }
    }
}
