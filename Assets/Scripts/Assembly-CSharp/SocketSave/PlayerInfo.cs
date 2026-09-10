using System;

namespace SocketSave
{
	[Serializable]
	public class PlayerInfo
	{
		public string VersionCode;

		public string Name;

		public bool CmdEnable;

		public bool Heartbeat;

		public int ReCntCode;

		public string Password;
	}
}
