using System;
using System.Collections.Generic;

namespace SaveClass
{
	[Serializable]
	public class CustomMapSave
	{
		[NonSerialized]
		public string SavePath;

		public string MapName;

		public string MapBrief;

		public MapType mapType;

		public int VerticalNum;

		public int HorizontalNum;

		public int HouseType;

		public int FenceType;

		public int FenceBackType;

		public List<TileType> tileTypes = new List<TileType>();
	}
}
