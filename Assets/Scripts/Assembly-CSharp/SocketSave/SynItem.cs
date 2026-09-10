using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class SynItem
	{
		public int OnlineId;

		public SynItemType Type;

		public string AName;

		public Vector2 Twofloat;

		public int[] SynCode = new int[4];
	}
}
