using System.Collections.Generic;
using UnityEngine;

public class LV2 : MonoBehaviour
{
	public static LV2 Instance;

	private void Awake()
	{
		Instance = this;
	}

	public List<SubLv> GetLvSubs()
	{
		List<SubLv> list = new List<SubLv>();
		SubLv subLv = new SubLv();
		list.Add(subLv);
		subLv.CardNum = 10;
		subLv.CurrBankType = BankType.ConveryorBelt;
		subLv.GeneralCardPool = new List<CardType>
		{
			new CardType(PlantType.PeaShooter),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.Repeater),
			new CardType(PlantType.Spike),
			new CardType(PlantType.PotatoMine),
			new CardType(PlantType.PotatoMine),
			new CardType(PlantType.PeaShooter),
			new CardType(PlantType.Repeater)
		};
		subLv.BigWaveFixedZombie = new List<ZombieType>
		{
			ZombieType.FlagZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie
		};
		subLv.BigWaveNum = new List<int> { 1 };
		subLv.Weights = new List<List<int>>
		{
			new List<int> { 1, 2 },
			new List<int> { 1, 2 }
		};
		subLv.ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.ConeZombie,
				ZombieType.PeaShooterZombie,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.ConeZombie,
				ZombieType.DolphinriderZombie,
				ZombieType.SnorkleZombieHelmet,
				ZombieType.Zomboni,
				ZombieType.PaperZombie
			}
		};
		SubLv subLv2 = new SubLv();
		list.Add(subLv2);
		subLv2.CardNum = 10;
		subLv2.BigWaveFixedZombie = new List<ZombieType>
		{
			ZombieType.FlagZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie
		};
		subLv2.BigWaveNum = new List<int> { 1 };
		subLv2.Weights = new List<List<int>>
		{
			new List<int> { 1, 8 },
			new List<int> { 1, 1 }
		};
		subLv2.ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.JacksonZombie,
				ZombieType.ConeZombie,
				ZombieType.FootballZombie,
				ZombieType.RepeaterZombie,
				ZombieType.LadderZombie,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.ConeZombie,
				ZombieType.DolphinriderZombie,
				ZombieType.SnorkleZombieHelmet,
				ZombieType.Zomboni,
				ZombieType.PaperZombie,
				ZombieType.FootballZombie,
				ZombieType.HeavyBungiZombie
			}
		};
		return list;
	}

	public List<SubLv> GetLvSubs2()
	{
		List<SubLv> list = new List<SubLv>();
		SubLv subLv = new SubLv();
		list.Add(subLv);
		subLv.CardNum = 10;
		subLv.CurrBankType = BankType.ConveryorBelt;
		subLv.GeneralCardPool = new List<CardType>
		{
			new CardType(PlantType.PeaShooter),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.Repeater),
			new CardType(PlantType.Spike),
			new CardType(PlantType.PotatoMine),
			new CardType(PlantType.PotatoMine),
			new CardType(PlantType.PeaShooter),
			new CardType(PlantType.Repeater)
		};
		subLv.BigWaveFixedZombie = new List<ZombieType>
		{
			ZombieType.FlagZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie
		};
		subLv.BigWaveNum = new List<int> { 1, 3 };
		subLv.Weights = new List<List<int>>
		{
			new List<int> { 16, 28, 26, 30 },
			new List<int> { 16, 28, 22, 35 }
		};
		subLv.ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.ConeZombie,
				ZombieType.PeaShooterZombie,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.ConeZombie,
				ZombieType.DolphinriderZombie,
				ZombieType.SnorkleZombieHelmet,
				ZombieType.Zomboni,
				ZombieType.PaperZombie
			}
		};
		SubLv subLv2 = new SubLv();
		list.Add(subLv2);
		subLv2.CardNum = 10;
		subLv2.BigWaveFixedZombie = new List<ZombieType>
		{
			ZombieType.FlagZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie,
			ZombieType.NormalZombie
		};
		subLv2.BigWaveNum = new List<int> { 3, 5 };
		subLv2.Weights = new List<List<int>>
		{
			new List<int> { 15, 18, 26, 34, 26, 40 },
			new List<int> { 15, 16, 24, 32, 22, 35 }
		};
		subLv2.ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.JacksonZombie,
				ZombieType.ConeZombie,
				ZombieType.FootballZombie,
				ZombieType.RepeaterZombie,
				ZombieType.LadderZombie,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.ConeZombie,
				ZombieType.DolphinriderZombie,
				ZombieType.SnorkleZombieHelmet,
				ZombieType.Zomboni,
				ZombieType.PaperZombie,
				ZombieType.FootballZombie,
				ZombieType.HeavyBungiZombie
			}
		};
		return list;
	}
}
