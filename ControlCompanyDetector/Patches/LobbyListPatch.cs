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
            }
            else
            {
                for (int i = 0; i < ___currentLobbyList.Length; i++)
                {
                    Lobby lobby = ___currentLobbyList[i];
                    string lobbyName = lobby.GetData("name");
                    if (lobbyName.Contains('\u200b'))
                    {
                        if (lobbyName.Contains("[CC]"))
                        {
                            lobbyName.Replace("[CC]", "[Control Company]" + lobbyName);
                        }
                        if (!lobbyName.Contains("[Control Company]"))
                        {
                            lobbyName = "[Control Company] " + lobbyName;
                        }
                        lobby.SetData("name", lobbyName);
                    }
                }
            }
        }
    }
}
