using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using LobbyCompatibility.Features;
using LobbyCompatibility.Models;
using Steamworks.Data;

namespace ControlCompanyDetector.Patches
{
    internal class LobbyListPatch
    {
        [HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")]
        [HarmonyPrefix]
        private static void FilterLobbyList(ref Lobby[] lobbyList, ref Lobby[] ___currentLobbyList)
        {
            Plugin.clientHasCCFilter = Plugin.UserHasMod("ControlCompany.ControlCompanyFilter");
            Plugin.clientHasRBL = Plugin.UserHasMod("Ryokune.BetterLobbies");
            
            if (Plugin.hideControlCompanyLobbies.Value)
            {
                List<Lobby> list = ___currentLobbyList.ToList<Lobby>();
                list.RemoveAll(delegate (Lobby lobby)
                {
                    LobbyDiff lobbyDiff = LobbyHelper.GetLobbyDiff(lobby);
                    bool serverHasBMX = lobbyDiff.PluginDiffs.Any(diff => diff.ServerVersion != null); 
                    if (serverHasBMX)
                    {
                        return lobbyDiff.PluginDiffs.Any(diff => diff.GUID == "ControlCompany.ControlCompany");
                    }

                    string lobbyName = lobby.GetData("name");
                    bool flag = lobbyName.Contains('\u200b');
                    return flag;
                });
                Lobby[] array = list.ToArray();
                ___currentLobbyList = array;
                lobbyList = array;
                return;
            }
            if (Plugin.showControlCompanyLobbiesOnly.Value)
            {
                List<Lobby> list = ___currentLobbyList.ToList<Lobby>();
                list.RemoveAll(delegate (Lobby lobby)
                {
                    LobbyDiff lobbyDiff = LobbyHelper.GetLobbyDiff(lobby);
                    bool serverHasBMX = lobbyDiff.PluginDiffs.Any(diff => diff.ServerVersion != null);
                    if (serverHasBMX)
                    {
                        return !lobbyDiff.PluginDiffs.Any(diff => diff.GUID == "ControlCompany.ControlCompany");
                    }

                    string lobbyName = lobby.GetData("name");
                    bool flag = lobbyName.Contains('\u200b');
                    return !flag;
                });
                Lobby[] array = list.ToArray();
                ___currentLobbyList = array;
                lobbyList = array;
            }
        }
    }
}
