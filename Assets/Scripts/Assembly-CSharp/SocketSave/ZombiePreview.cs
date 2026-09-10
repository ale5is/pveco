using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class ZombiePreview
	{
		public string PlayerName;

		public Vector2 GridPos;

		public ZombieType zombieType;

		public bool isRat;
	}
}
