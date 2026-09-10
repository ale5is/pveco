using System;
using System.Collections.Generic;

namespace SocketSave
{
	[Serializable]
	public class LoadLVBag
	{
		public int LoadType;

		public string LvName;

		public int LvId;

		public int LvSeed;

		public int ReCntCode;

		public List<int> CardNumList;

		public List<string> NameList;

		public BgmType dayBgm;

		public BgmType nightBgm;

		public List<bool> BoolTypes;

		public BankType BankType;

		public SeedBankType SeedBankType;

		public List<MapType> LoadMapTypes;

		public List<LVSpState> LvSpStates;

		public List<int> ZTypesSplit;

		public List<ZombieType> ZombieTypes;
	}
}
