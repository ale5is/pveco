using System;
using System.Collections.Generic;

namespace SocketSave
{
	[Serializable]
	public class OnlinePlayerInfo
	{
		public PlayerInfo HostPlayer;

		public List<PlayerInfo> players = new List<PlayerInfo>();
	}
}
