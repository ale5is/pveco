using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class ZombieSpawnApply
	{
		public int CardId;

		public PlantType plantType;

		public ZombieType Type;

		public Vector2 GridPos;

		public bool isRat;
	}
}
