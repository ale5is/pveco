using System;
using UnityEngine;

namespace SocketSave
{
	[Serializable]
	public class PlantPreview
	{
		public string PlayerName;

		public Vector2 GridPos;

		public PlantType plantType;

		public bool isImtor;
	}
}
