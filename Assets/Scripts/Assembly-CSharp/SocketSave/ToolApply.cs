using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class ToolApply
	{
		public ToolType type;

		public string User;

		public Vector2 GridPos;

		public int Sound;

		public int OnlineId;
	}
}
