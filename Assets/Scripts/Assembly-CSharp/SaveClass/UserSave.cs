using System.Collections.Generic;

namespace SaveClass
{
	public class UserSave
	{
		public string playerName;

		public string VersionCode;

		public bool CmdEnable;

		public int LastAdventureId;

		public int LastMiniGameId;

		public int LastPuzzleId;

		public int MoneyNum;

		public int CardSlotNum;

		public bool ShovelUnLock;

		public bool AlmanacUnLock;

		public int StoreLvl;

		public List<SpItem> SpItems = new List<SpItem>();

		public bool SwampOpen;

		public List<PlantType> UnlockedPlants = new List<PlantType>();

		public List<bool> MoreOptions = new List<bool>();

		public List<CardType> LastSelectedCard = new List<CardType>();

		public bool OpenQuickChat;

		public List<string> QuickChat = new List<string>();
	}
}
