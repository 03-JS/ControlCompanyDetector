using BepInEx;
using BepInEx.Bootstrap;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ControlCompanyDetector.Patches;
using static UnityEngine.Scripting.GarbageCollector;
using System;
using System.Linq;
using Unity.Netcode;

namespace ControlCompanyDetector.Logic
{
    internal static class Detector
    {
        // private static Dictionary<string, PluginInfo> Mods = new Dictionary<string, PluginInfo>();
        // private static int previousLastCCLine;
        public static bool hostHasCC;
        public static bool clientIsFriendsWithHost;

        public static IEnumerator StartDetection()
        {
            //HUDManagerPatch.displayTip = false;
            //if (Player.LocalPlayer == null)
            //{
            //    Mods = Chainloader.PluginInfos;
            //    Plugin.LogInfoMLS("Starting detection...");
            //    Network.Broadcast("LC_API_ReqGUID");
            //    CoroutineManager.StartCoroutine(ReadBepinLog());
            //}

            Plugin.LogInfoMLS("Checking if host and client are friends...");

            yield return new WaitForSeconds(3.5f);

            clientIsFriendsWithHost = true;
            if (!StartOfRound.Instance.IsClientFriendsWithHost())
            {
                clientIsFriendsWithHost = false;
                Detector.Detect();
            }
            else if (!Plugin.ignoreFriendlyLobbies.Value)
            {
                Detector.Detect();
            }
        }

        internal static void Detect()
        {
            Plugin.LogInfoMLS("Detection started");
            Plugin.LogInfoMLS("Lobby name: " + GameNetworkManager.Instance.steamLobbyName);
            if (GameNetworkManager.Instance != null)
            {
                if (GameNetworkManager.Instance.steamLobbyName.Contains('\u200b'))
                {
                    hostHasCC = true;
                    Plugin.LogWarnMLS("Control Company has been detected");
                    Detector.SendUITip("WARNING:", "The host is using Control Company", true);
                }
                else
                {
                    hostHasCC = false;
                }
            }
        }

        internal static void SendUITip(string header, string message, bool warning)
        {
            // Player player = Player.Get(senderId);
            // HUDManagerPatch.displayTip = true;
            Plugin.LogInfoMLS("Displaying HUD message");
            HUDManager.Instance.DisplayTip(header, message, warning, false, "LC_Tip1");
        }

        public static IEnumerator SendDelayedUITip(string header, string message, bool warning, float delay)
        {
            Plugin.LogInfoMLS("Sending a delayed message...");

            yield return new WaitForSeconds(delay);

            Plugin.LogInfoMLS("Displaying HUD message");
            HUDManager.Instance.DisplayTip(header, message, warning, false, "LC_Tip1");
        }
    }
}
