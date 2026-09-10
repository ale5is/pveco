using System;
using System.Collections.Generic;

namespace SaveClass
{
	public class CustomLevelSave
	{
		[NonSerialized]
		public string SavePath;

		public string LvName;

		public string LvBrief;

		public LVType lVType;

		public MapSeries series;

		public List<MapType> mapTypes;
	}
}
