using UnityEngine;
using System.Collections;
using LobbyCompatibility.Models;
using LobbyCompatibility.Features;
using System.Linq;
using HarmonyLib;
using Steamworks;

namespace ControlCompanyDetector.Logic
{
    internal static class Detector
    {
        public static bool hostHasCC;
        public static bool clientIsFriendsWithHost;
        public static bool canClientDetectEnemySpawning;

        public static IEnumerator StartDetection()
        {
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
            hostHasCC = false;
            canClientDetectEnemySpawning = true;
            if (GameNetworkManager.Instance != null)
            {
                bool lobbyHasValue = GameNetworkManager.Instance.currentLobby.HasValue;
                Plugin.LogInfoMLS("Is the lobby a steam lobby? -> " + lobbyHasValue);
                if (lobbyHasValue)
                {
                    LobbyDiff lobbyDiff = LobbyHelper.GetLobbyDiff(GameNetworkManager.Instance.currentLobby.GetValueOrDefault());
                    if (lobbyDiff.PluginDiffs.Any(diff => diff.ServerVersion != null))
                    {
                        if (lobbyDiff.PluginDiffs.Any(diff => diff.GUID == "ControlCompany.ControlCompany"))
                        {
                            DisplayWarning();
                        }
                        foreach (var diff in lobbyDiff.PluginDiffs)
                        {
                            foreach (var key in Plugin.keywords)
                            {
                                if (diff.GUID.Contains(key))
                                {
                                    canClientDetectEnemySpawning = false;
                                    Plugin.LogWarnMLS("The host is using a mod that alters enemy spawning!");
                                    Detector.SendUITip("Control Company Detector:", "<size=15>Detect enemy spawning has been disabled because the host has the following mod installed:</size>\n" + diff.GUID, false);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GameNetworkManager.Instance.steamLobbyName.Contains('\u200b'))
                        {
                            DisplayWarning();
                        }
                    }
                }
            }
        }

        internal static void DisplayWarning()
        {
            hostHasCC = true;
            Plugin.LogWarnMLS("Control Company has been detected!");
            Detector.SendUITip("WARNING:", "The host is using Control Company", true);
            if (Plugin.sendChatMessage.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FF0000>" + "Control Company Detector" + "</color>:" + "<color=#FFFF00> Hey! " + RoundManager.Instance.playersManager.allPlayerScripts[0].playerUsername + " is using Control Company!" + "</color>");
            }
        }

        internal static void SendUITip(string header, string message, bool warning)
        {
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
