using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Steamworks.Data;

namespace ControlCompanyDetector.Patches
{
    internal class LobbyListPatch
    {
        [HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")]
        [HarmonyPrefix]
        private static void FilterLobbyList(ref Lobby[] lobbyList, ref Lobby[] ___currentLobbyList)
        {
            Plugin.clientHasCC = Plugin.UserHasMod("ControlCompany.ControlCompany");
            Plugin.clientHasRBL = Plugin.UserHasMod("Ryokune.BetterLobbies");
            
            if (Plugin.hideControlCompanyLobbies.Value)
            {
                List<Lobby> list = ___currentLobbyList.ToList<Lobby>();
                list.RemoveAll(delegate (Lobby lobby)
                {
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
