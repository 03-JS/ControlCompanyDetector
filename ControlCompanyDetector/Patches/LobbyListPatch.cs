using System;
using System.Collections.Generic;
using System.Linq;
using ControlCompany.Core.Config;
using HarmonyLib;
using Steamworks.Data;

namespace ControlCompany.Patches
{
	// Token: 0x02000008 RID: 8
	internal class LobbyListPatch
	{
		// Token: 0x06000012 RID: 18 RVA: 0x0000251C File Offset: 0x0000071C
		[HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")]
		[HarmonyPrefix]
		private static void FilterLobbyList(ref Lobby[] lobbyList, ref Lobby[] ___currentLobbyList)
		{
			if (ConfigManager.hideControlCompanyEnabledServers.Value)
			{
				List<Lobby> list = ___currentLobbyList.ToList<Lobby>();
				list.RemoveAll(delegate(Lobby lobby)
				{
					string data = lobby.GetData("name");
					bool flag = data.Contains('\u200b');
					bool flag2 = flag;
					if (flag2)
					{
					}
					return flag;
				});
				Lobby[] array = list.ToArray();
				___currentLobbyList = array;
				lobbyList = array;
			}
		}
		// Token: 0x04000009 RID: 9
		public const char SERVER_NAME_FILTER = '\u200b';
	}
}
