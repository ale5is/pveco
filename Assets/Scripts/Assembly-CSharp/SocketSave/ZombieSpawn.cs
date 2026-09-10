using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class ZombieSpawn
	{
		public int OnlineId;

		public string PlacePlayer;

		public ZombieType Type;

		public Vector2 SpawnPos;

		public int UpdateLine;

		public int[] SpCode = new int[4];

		public bool UpLv;
	}
}
