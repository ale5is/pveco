using System;
using System.Collections.Generic;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class PuddleSpawn
	{
		public int OnlineId;

		public Vector2 InitPos;

		public List<Vector2> MapPos = new List<Vector2>();
	}
}
