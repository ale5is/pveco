using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class PlantSpawn
	{
		public int OnlineId;

		public string PlacePlayer;

		public Vector2 GridPos;

		public PlantType plantType;

		public int SPcode;

		public int CardId;
	}
}
