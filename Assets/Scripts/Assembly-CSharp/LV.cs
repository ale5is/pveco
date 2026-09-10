using System.Collections.Generic;
using SaveClass;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class LV : MonoBehaviour
{
	public static LV Instance;

	public bool IsEasy;

	private bool OnlyInfo;

	public LvSave CurrLvInfo;

	public int CurrLvId;

	public int StartTime;

	public int LvNormalSunNum;

	public string LvName;

	public LVType CurrLVType;

	public int LvTemperature;

	public bool WeightLimit;

	public int NextWaveLossTime;

	public Vector2 SetupTime = new Vector2(10f, 12f);

	public List<FutureWeather> LoadWeathers = new List<FutureWeather>();

	public BootySprite BootySprite;

	public PlantType BootyPlant;

	public UnityAction BootyEvent;

	public UnityAction FirstBootyEvent;

	private BgmType dayBgm;

	private BgmType nightBgm;

	public List<MapType> LoadMapTypes;

	public UnityAction MapOverAction;

	public int ProphaseLimitWave = 3;

	public List<ZombieType> ProphaseLimitZombie = new List<ZombieType>();

	public List<SubLv> SubLvs = new List<SubLv>();

	public bool EnableShovel;

	public bool EnablePlantGlove;

	public bool EnableZombieGlove;

	public float BungiSpRate;

	public List<LVSpState> LvSpStates = new List<LVSpState>();

	public int CardNum;

	public bool StartCardCd;

	public bool BanMultyCardAdd;

	public int BeltTimeAdd;

	public BankType CurrBankType;

	public SeedBankType CurrSeedBankType;

	public List<CardType> GeneralCardPool = new List<CardType>();

	public List<CardType> FixedCard = new List<CardType>();

	public List<int> BigWaveNum = new List<int>();

	public List<List<int>> Weights = new List<List<int>>();

	public List<List<ZombieType>> ZombieTypes = new List<List<ZombieType>>();

	public List<ZombieType> GraveZombie = new List<ZombieType>();

	public int WaterZombieNum;

	public List<ZombieType> WaterZombie = new List<ZombieType>();

	public List<ZombieType> BigWaveFixedZombie = new List<ZombieType>();

	public UnityAction StartAction;

	public UnityAction BigWaveAction;

	public Dictionary<int, UnityAction> TimeAction = new Dictionary<int, UnityAction>();

	public List<List<List<PlantType>>> StartPlants = new List<List<List<PlantType>>>();

	public int PlantVaseNum;

	public int ZombieVaseNum;

	public List<List<List<VaseType>>> VaseBreakerVase = new List<List<List<VaseType>>>();

	public BgmType DayBgm
	{
		get
		{
			if (dayBgm != BgmType.Nope)
			{
				return dayBgm;
			}
			return BgmType.GrassWalk;
		}
	}

	public BgmType NightBgm
	{
		get
		{
			if (nightBgm != BgmType.Nope)
			{
				return nightBgm;
			}
			return BgmType.MoonGrains;
		}
	}

	public void ReadSubLv(SubLv lv)
	{
		BungiSpRate = lv.BungiSpRate;
		LvSpStates = lv.LvSpStates;
		CardNum = lv.CardNum;
		StartCardCd = lv.StartCardCd;
		BanMultyCardAdd = lv.BanMultyCardAdd;
		BeltTimeAdd = lv.BeltTimeAdd;
		CurrBankType = lv.CurrBankType;
		CurrSeedBankType = lv.CurrSeedBankType;
		GeneralCardPool = lv.GeneralCardPool;
		FixedCard = lv.FixedCard;
		BigWaveNum = lv.BigWaveNum;
		Weights = lv.Weights;
		ZombieTypes = lv.ZombieTypes;
		GraveZombie = lv.GraveZombie;
		WaterZombieNum = lv.WaterZombieNum;
		WaterZombie = lv.WaterZombie;
		BigWaveFixedZombie = lv.BigWaveFixedZombie;
		StartAction = lv.StartAction;
		BigWaveAction = lv.BigWaveAction;
		TimeAction = lv.TimeAction;
		StartPlants = lv.StartPlants;
		PlantVaseNum = lv.PlantVaseNum;
		ZombieVaseNum = lv.ZombieVaseNum;
		VaseBreakerVase = lv.VaseBreakerVase;
	}

	private void Awake()
	{
		Instance = this;
	}

	public void LoadLV(int LVId, bool isEasy, bool onlyInfo, bool isRun)
	{
		if (LVManager.Instance.InGame)
		{
			Debug.Log("错误加载地图" + onlyInfo);
			return;
		}
		CurrLvId = LVId;
		OnlyInfo = onlyInfo;
		IsEasy = isEasy;
		if (CurrLvId > 10000)
		{
			CurrLvInfo = GameManager.Instance.GetLvSave(CurrLvId);
		}
		else
		{
			CurrLvInfo = null;
		}
		ResetData();
		if (CurrLvId > 10000 && CurrLvId % 10000 < 1000)
		{
			LvName = "关卡" + CurrLvId / 10000 + "-" + CurrLvId % 10000;
		}
		LoadLocalLevel(LVId);
		if (!onlyInfo & isRun)
		{
			RunLv();
		}
	}

	private void LoadLocalLevel(int LVId)
	{
		switch (LVId)
		{
		case 1:
			LVPvP();
			break;
		case 10001:
			if (PlayerManager.Instance.IsDebug)
			{
				LVTest();
			}
			else
			{
				LV10001();
			}
			break;
		case 10002:
			if (PlayerManager.Instance.IsDebug)
			{
				LVTest2();
			}
			else
			{
				LV10002();
			}
			break;
		case 10003:
			LV10003();
			break;
		case 10004:
			LV10004();
			break;
		case 10005:
			LV10005();
			break;
		case 10006:
			LV10006();
			break;
		case 10007:
			LV10007();
			break;
		case 10008:
			LV10008();
			break;
		case 10009:
			LV10009();
			break;
		case 10010:
			LV10010();
			break;
		case 10011:
			LV10011();
			break;
		case 10012:
			LV10012();
			break;
		case 10013:
			LV10013();
			break;
		case 10014:
			LV10014();
			break;
		case 10015:
			LV10015();
			break;
		case 10016:
			LV10016();
			break;
		case 10017:
			LV10017();
			break;
		case 10018:
			LV10018();
			break;
		case 10019:
			LV10019();
			break;
		case 10020:
			LV10020();
			break;
		case 10021:
			LV10021();
			break;
		case 10022:
			LV10022();
			break;
		case 10023:
			LV10023();
			break;
		case 10024:
			LV10024();
			break;
		case 10025:
			LV10025();
			break;
		case 10026:
			LV10026();
			break;
		case 10027:
			LV10027();
			break;
		case 10028:
			LV10028();
			break;
		case 10029:
			LV10029();
			break;
		case 10030:
			LV10030();
			break;
		case 10031:
			LV10031();
			break;
		case 10032:
			LV10032();
			break;
		case 10033:
			LV10033();
			break;
		case 10034:
			LV10034();
			break;
		case 10035:
			LV10035();
			break;
		case 10036:
			LV10036();
			break;
		case 10037:
			LV10037();
			break;
		case 10038:
			LV10038();
			break;
		case 10039:
			LV10039();
			break;
		case 10040:
			LV10040();
			break;
		case 10041:
			LV10041();
			break;
		case 10042:
			LV10042();
			break;
		case 10043:
			LV10043();
			break;
		case 10044:
			LV10044();
			break;
		case 10045:
			LV10045();
			break;
		case 10046:
			LV10046();
			break;
		case 10047:
			LV10047();
			break;
		case 10048:
			LV10048();
			break;
		case 10049:
			LV10049();
			break;
		case 10050:
			LV10050();
			break;
		case 10051:
			LV10051();
			break;
		case 10052:
			LV10052();
			break;
		case 10053:
			LV10053();
			break;
		case 10054:
			LV10054();
			break;
		case 10055:
			LV10055();
			break;
		case 11001:
			LV11001();
			break;
		case 11002:
			LV11002();
			break;
		case 11003:
			LV11003();
			break;
		case 11004:
			LV11004();
			break;
		case 11005:
			LV11005();
			break;
		case 11006:
			LV11006();
			break;
		case 11007:
			LV11007();
			break;
		case 11008:
			LV11008();
			break;
		case 11009:
			LV11009();
			break;
		case 11010:
			LV11010();
			break;
		case 11011:
			LV11011();
			break;
		case 11012:
			LV11012();
			break;
		case 11013:
			LV11013();
			break;
		case 11014:
			LV11014();
			break;
		case 11015:
			LV11015();
			break;
		case 11016:
			LV11016();
			break;
		case 11017:
			LV11017();
			break;
		case 11018:
			LV11018();
			break;
		case 11019:
			LV11019();
			break;
		case 11020:
			LV11020();
			break;
		case 11021:
			LV11021();
			break;
		case 11022:
			LV11022();
			break;
		case 11023:
			LV11023();
			break;
		case 11024:
			LV11024();
			break;
		case 11025:
			LV11025();
			break;
		case 11026:
			LV11026();
			break;
		case 11027:
			LV11027();
			break;
		case 11028:
			LV11028();
			break;
		case 11029:
			LV11029();
			break;
		case 11030:
			LV11030();
			break;
		case 11031:
			LV11031();
			break;
		case 11032:
			LV11032();
			break;
		case 11033:
			LV11033();
			break;
		case 11034:
			LV11034();
			break;
		case 11035:
			LV11035();
			break;
		case 12001:
			LV12001();
			break;
		case 12002:
			LV12002();
			break;
		case 12003:
			LV12003();
			break;
		case 12004:
			LV12004();
			break;
		case 12005:
			LV12005();
			break;
		case 12006:
			LV12006();
			break;
		case 12007:
			LV12007();
			break;
		case 12008:
			LV12008();
			break;
		case 12009:
			LV12009();
			break;
		case 12010:
			LV12010();
			break;
		case 20001:
			LV20001();
			break;
		case 20002:
			LV20002();
			break;
		case 20003:
			LV20003();
			break;
		case 20004:
			LV20004();
			break;
		case 20005:
			LV20005();
			break;
		case 20006:
			LV20006();
			break;
		case 20007:
			LV20007();
			break;
		case 20008:
			LV20008();
			break;
		case 20009:
			LV20009();
			break;
		case 20010:
			LV20010();
			break;
		case 20011:
			LV20011();
			break;
		case 20012:
			LV20012();
			break;
		case 20013:
			LV20013();
			break;
		case 20014:
			LV20014();
			break;
		case 20015:
			LV20015();
			break;
		case 20016:
			LV20016();
			break;
		case 20017:
			LV20017();
			break;
		case 20018:
			LV20018();
			break;
		case 20019:
			LV20019();
			break;
		case 20020:
			LV20020();
			break;
		case 20021:
			LV20021();
			break;
		case 20022:
			LV20022();
			break;
		case 20023:
			LV20023();
			break;
		case 20024:
			LV20024();
			break;
		case 20025:
			LV20025();
			break;
		default:
			LV10001();
			break;
		}
	}

	private void RunLv()
	{
		if (CurrLVType == LVType.IZombie)
		{
			StartTime = 0;
		}
		SkyManager.Instance.DirectSetTime(StartTime);
		MapManager.Instance.CreateMap(LoadMapTypes);
		if (LoadWeathers.Count > 0 && LoadWeathers[0].appearTime <= 0)
		{
			SkyManager.Instance.PlayWeather(LoadWeathers[0]);
			MyTool.MoveFirstToLast(LoadWeathers);
		}
		Timetable.Instance.UpdateWeatherReport();
		if (MapOverAction != null)
		{
			MapOverAction();
		}
	}

	public void ClientLoadLv(LoadLVBag loadBag)
	{
		ResetData();
		CurrLvId = loadBag.LvId;
		if (CurrLvId > 10000)
		{
			CurrLvInfo = GameManager.Instance.GetLvSave(CurrLvId);
		}
		else
		{
			CurrLvInfo = null;
		}
		OnlyInfo = false;
		LoadLocalLevel(loadBag.LvId);
		LvName = loadBag.LvName;
		dayBgm = loadBag.dayBgm;
		nightBgm = loadBag.nightBgm;
		CurrBankType = loadBag.BankType;
		CurrSeedBankType = loadBag.SeedBankType;
		LoadMapTypes = loadBag.LoadMapTypes;
		LvSpStates = loadBag.LvSpStates;
		IsEasy = loadBag.BoolTypes[0];
		EnableShovel = loadBag.BoolTypes[1];
		EnablePlantGlove = loadBag.BoolTypes[2];
		EnableZombieGlove = loadBag.BoolTypes[3];
		ZombieTypes = new List<List<ZombieType>>();
		List<List<ZombieType>> list = new List<List<ZombieType>>();
		int num = 0;
		foreach (int item in loadBag.ZTypesSplit)
		{
			List<ZombieType> range = loadBag.ZombieTypes.GetRange(num, item);
			list.Add(range);
			num += item;
		}
		ZombieTypes = list;
		if (GameManager.Instance.isClient)
		{
			LoadWeathers.Clear();
		}
		RunLv();
	}

	private void ResetData()
	{
		LoadMapTypes.Clear();
		LvName = "关卡???";
		BungiSpRate = 0f;
		StartCardCd = true;
		LvTemperature = 0;
		StartTime = 480;
		BeltTimeAdd = 0;
		WeightLimit = true;
		dayBgm = BgmType.Nope;
		nightBgm = BgmType.Nope;
		LvNormalSunNum = 50;
		NextWaveLossTime = 0;
		StartAction = null;
		MapOverAction = null;
		BigWaveAction = null;
		LvSpStates.Clear();
		CurrLVType = LVType.Normal;
		BootyEvent = null;
		BootyPlant = PlantType.Nope;
		BootySprite = BootySprite.MoneyBag;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "???", "???");
		ProphaseLimitWave = 3;
		ProphaseLimitZombie = new List<ZombieType>
		{
			ZombieType.Ghost,
			ZombieType.RoadrollerZombie
		};
		EnableShovel = true;
		EnablePlantGlove = false;
		EnableZombieGlove = false;
		Weights.Clear();
		BigWaveNum.Clear();
		ZombieTypes.Clear();
		TimeAction.Clear();
		LoadWeathers.Clear();
		CardNum = -1;
		BanMultyCardAdd = false;
		CurrBankType = BankType.Normal;
		CurrSeedBankType = SeedBankType.SunBank;
		WaterZombieNum = 0;
		WaterZombie.Clear();
		StartPlants.Clear();
		PlantVaseNum = 0;
		ZombieVaseNum = 0;
		VaseBreakerVase.Clear();
		FixedCard.Clear();
		GraveZombie = new List<ZombieType>
		{
			ZombieType.NormalZombie,
			ZombieType.ConeZombie,
			ZombieType.BucketZombie
		};
		if (IsEasy)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie
			};
		}
		else
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagBucketZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.BucketZombie
			};
		}
	}

	public Sprite GetLvSprite()
	{
		Sprite result = NormalSprite.Instance.YardDay;
		switch (CurrLvId / 10000)
		{
		case 1:
			result = ((!SkyManager.Instance.GetIsDay(StartTime)) ? NormalSprite.Instance.YardNight : NormalSprite.Instance.YardDay);
			break;
		case 2:
			result = ((!SkyManager.Instance.GetIsDay(StartTime)) ? NormalSprite.Instance.SwampNight : NormalSprite.Instance.SwampDay);
			break;
		}
		return result;
	}

	public void SpSeedBankMove()
	{
		SeedBank.Instance.StartMove(CurrSeedBankType);
	}

	private void LVTest()
	{
		CardNum = 15;
		LvNormalSunNum = 5000;
		LvName = "关卡?-?";
		StartTime = 480;
		CurrSeedBankType = SeedBankType.SunAndMoonBank;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Wind, 5, 0),
			new FutureWeather(WeatherType.Wind, 2, 10),
			new FutureWeather(WeatherType.Wind, 4, 40),
			new FutureWeather(WeatherType.Rain, 4, 50)
		};
		if (!OnlyInfo)
		{
			GeneralCardPool = new List<CardType>
			{
				new CardType(ZombieType.ConeZombie)
			};
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.NormalZombie
			};
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			LvNormalSunNum = 50;
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1 }
			};
			SetupTime = new Vector2(1000f, 1500f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType> { ZombieType.NormalZombie }
			};
			BigWaveNum = new List<int> { 1 };
			TimeAction.Add(485, () =>
			{
			});
			MapOverAction = () =>
			{
			};
			StartAction = () =>
			{
			};
		}
	}

	private void LVTest2()
	{
		if (IsEasy)
		{
			StartTime = 680;
		}
		else
		{
			StartTime = 1080;
		}
		LvName = "关卡?-?";
		CardNum = 15;
		CurrSeedBankType = SeedBankType.SunAndMoonBank;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(0f, 2f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2 },
					new List<int> { 1, 1 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.ConeZombie,
						ZombieType.SnorkleZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie
					}
				};
			}
			BigWaveNum = new List<int> { 1 };
			SubLvs = LV2.Instance.GetLvSubs();
		}
	}

	private void LVTest2652()
	{
		if (IsEasy)
		{
			StartTime = 1000;
		}
		else
		{
			StartTime = 1080;
		}
		LvName = "关卡?-?";
		StartCardCd = false;
		WeightLimit = false;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Rain, 10, 280),
			new FutureWeather(WeatherType.Clear, 0, 710)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 2500;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 10, 12, 20, 26, 38, 30, 40, 54 },
					new List<int> { 2, 3, 8, 8, 10, 8, 10, 16 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.RepeaterZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmet,
						ZombieType.BalloonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.ConeZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.PaperZombie,
						ZombieType.SnorkleZombie,
						ZombieType.GargantuarRedeye
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.Repeater,
						PlantType.SunFlower,
						PlantType.Nope,
						PlantType.Nope
					},
					new List<PlantType>
					{
						PlantType.Repeater,
						PlantType.TwinSunflower,
						PlantType.Nope,
						PlantType.Torchwood
					},
					new List<PlantType>
					{
						PlantType.Repeater,
						PlantType.SunFlower,
						PlantType.Nope,
						PlantType.Torchwood
					},
					new List<PlantType>
					{
						PlantType.Repeater,
						PlantType.TwinSunflower,
						PlantType.Nope,
						PlantType.Torchwood
					},
					new List<PlantType>
					{
						PlantType.Repeater,
						PlantType.SunFlower,
						PlantType.Nope,
						PlantType.Nope
					}
				}
			};
		}
	}

	private void LVTest2xdw()
	{
		if (IsEasy)
		{
			StartTime = 600;
		}
		else
		{
			StartTime = 1080;
		}
		LvName = "关卡?-?";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 30),
			new FutureWeather(WeatherType.Rain, 10, 280),
			new FutureWeather(WeatherType.Clear, 0, 710)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 1500;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 3, 6, 8, 8, 12, 22, 20, 36, 44 },
					new List<int> { 3, 6, 6, 12, 18, 20, 16, 24, 42 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.TubeDoorConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndBucket,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.PaperZombie,
						ZombieType.LadderZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LVPvP()
	{
		LvName = "玩家对战模式";
		CurrLVType = LVType.PvP;
		CurrSeedBankType = PvPSelector.Instance.GetSeedBankType();
		LoadMapTypes = new List<MapType> { MapType.PvPYard };
		if (!OnlyInfo)
		{
			if (!int.TryParse(PvPSelector.Instance.StartTime, out var result))
			{
				result = 480;
			}
			if (!int.TryParse(PvPSelector.Instance.StartSunNum, out var result2))
			{
				LvNormalSunNum = 50;
			}
			LvNormalSunNum = result2;
			StartTime = result;
		}
	}

	private void LV10001()
	{
		StartTime = 480;
		LvNormalSunNum = 50;
		BootyPlant = PlantType.WallNut;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "坚果", " 阻挡僵尸的前进，\n并保护你的其他植物");
			if (IsEasy)
			{
				SetupTime = new Vector2(14f, 16f);
				BigWaveNum = new List<int> { 3 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 10 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.NormalZombie
					}
				};
			}
			else
			{
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 4 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 16 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.PeaShooterZombie
					}
				};
			}
		}
	}

	private void LV10002()
	{
		LvNormalSunNum = 50;
		StartTime = 500;
		BootyPlant = PlantType.PotatoMine;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "土豆雷", "接触就会爆炸，\n但是需要一段时间准备");
			if (IsEasy)
			{
				SetupTime = new Vector2(14f, 16f);
				BigWaveNum = new List<int> { 4 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 2, 4, 10 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie
					}
				};
			}
			else
			{
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 4 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 4, 8, 14 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie
					}
				};
			}
		}
	}

	private void LV10003()
	{
		LvNormalSunNum = 50;
		StartTime = 520;
		BootyPlant = PlantType.Lilypad;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (IsEasy)
		{
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 3, 0)
			};
		}
		else
		{
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 3, 0)
			};
		}
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "荷叶", "使你将非水生植物种在上面");
			if (IsEasy)
			{
				SetupTime = new Vector2(14f, 16f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 3, 4, 8, 15 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter
					}
				};
			}
			else
			{
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Polevaulter,
						ZombieType.WallNutZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5 };
		}
	}

	private void LV10004()
	{
		LvNormalSunNum = 50;
		StartTime = 540;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Shovel;
			AwardScence.Instance.LoadText("你获得了一个新的道具!", "铲子", "让你可以铲掉一株植物，\n为别的植物腾出空间");
			BootyEvent = () =>
			{
				GameManager.Instance.LocalPlayerSave.ShovelUnLock = true;
			};
			if (IsEasy)
			{
				SetupTime = new Vector2(14f, 16f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 2, 4, 10, 18 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.TubeZombie
					}
				};
			}
			else
			{
				SetupTime = new Vector2(10f, 14f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 8, 12, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.TubeZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5 };
		}
	}

	private void LV10005()
	{
		LvNormalSunNum = 50;
		if (IsEasy)
		{
			StartTime = 550;
		}
		else
		{
			StartTime = 570;
		}
		CurrBankType = BankType.ConveryorBelt;
		BootyPlant = PlantType.Repeater;
		LvSpStates.Add(LVSpState.NutBowling);
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.Loonboon;
		nightBgm = BgmType.Loonboon;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "双发射手", "一次发射两颗豌豆");
		GeneralCardPool = new List<CardType>
		{
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.ExplodeNut)
		};
		if (IsEasy)
		{
			SetupTime = new Vector2(2f, 4f);
			Weights = new List<List<int>>
			{
				new List<int> { 10, 12, 10, 12, 25, 25, 30, 30, 40 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.Polevaulter
				}
			};
		}
		else
		{
			SetupTime = new Vector2(2f, 4f);
			Weights = new List<List<int>>
			{
				new List<int> { 15, 15, 25, 25, 45, 35, 35, 40, 45 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.Polevaulter,
					ZombieType.BucketZombie,
					ZombieType.Polevaulter
				}
			};
		}
		BigWaveNum = new List<int> { 4, 8 };
		MapOverAction = () =>
		{
			List<Grid> gridList = MapManager.Instance.mapList[0].GridList;
			for (int i = 0; i < gridList.Count; i++)
			{
				if (gridList[i].Point.x <= 2)
				{
					gridList[i].isOccupied = false;
				}
				else
				{
					gridList[i].isOccupied = true;
				}
			}
			MapManager.Instance.mapList[0].SetStripe(2);
		};
	}

	private void LV10006()
	{
		if (IsEasy)
		{
			StartTime = 740;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 6, 0),
				new FutureWeather(WeatherType.Rain, 0, 242)
			};
		}
		else
		{
			StartTime = 800;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 8, 0),
				new FutureWeather(WeatherType.Rain, 3, 380)
			};
		}
		BootyPlant = PlantType.Tanglekelp;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "缠绕海草", "可以将僵尸直接拖入水底");
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(8f, 12f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Polevaulter,
						ZombieType.PeaShooterZombie
					}
				};
			}
		}
	}

	private void LV10007()
	{
		if (IsEasy)
		{
			StartTime = 570;
		}
		else
		{
			StartTime = 660;
		}
		BootyPlant = PlantType.Spike;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "地刺", "扎破轮胎并伤害踩在上面的僵尸");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 6 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 18, 24 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.Zomboni
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(10f, 14f);
				BigWaveNum = new List<int> { 4, 7 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 8, 12, 26, 15, 22, 30 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni
					}
				};
			}
		}
	}

	private void LV10008()
	{
		LvNormalSunNum = 50;
		if (IsEasy)
		{
			StartTime = 590;
		}
		else
		{
			StartTime = 720;
		}
		BootyPlant = PlantType.Coffeebean;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "咖啡豆", "将它种在植物上来唤醒植物");
			if (IsEasy)
			{
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 4, 7 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 4, 18, 10, 15, 24 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni
					}
				};
			}
			else
			{
				SetupTime = new Vector2(8f, 12f);
				BigWaveNum = new List<int> { 4, 7 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 8, 12, 18, 15, 18, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni,
						ZombieType.FootballZombie
					}
				};
			}
		}
	}

	private void LV10009()
	{
		LvNormalSunNum = 50;
		if (IsEasy)
		{
			StartTime = 660;
		}
		else
		{
			StartTime = 840;
		}
		CurrBankType = BankType.ConveryorBelt;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.UltimateBattle;
		nightBgm = BgmType.UltimateBattle;
		BootySprite = BootySprite.ZombieNote;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "双面夹击！");
		BootyEvent = () =>
		{
			if (GameManager.Instance.LocalPlayerSave.CardSlotNum < 7)
			{
				GameManager.Instance.LocalPlayerSave.CardSlotNum = 7;
			}
		};
		GeneralCardPool = new List<CardType>
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
		if (IsEasy)
		{
			SetupTime = new Vector2(12f, 15f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 3, 8, 10, 12, 27, 16, 28, 45 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.Polevaulter,
					ZombieType.Zomboni,
					ZombieType.FootballZombie
				}
			};
		}
		else
		{
			SetupTime = new Vector2(8f, 12f);
			Weights = new List<List<int>>
			{
				new List<int> { 2, 4, 8, 12, 25, 38, 32, 45, 55 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.Polevaulter,
					ZombieType.Zomboni,
					ZombieType.FootballZombie,
					ZombieType.PeaShooterZombie,
					ZombieType.PeaShooterZombie
				}
			};
		}
		BigWaveNum = new List<int> { 5, 8 };
	}

	private void LV10010()
	{
		if (IsEasy)
		{
			StartTime = 700;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 3, 100),
				new FutureWeather(WeatherType.Clear, 0, 272)
			};
		}
		else
		{
			StartTime = 890;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 4, 0),
				new FutureWeather(WeatherType.Clear, 0, 372)
			};
		}
		BootyPlant = PlantType.Cherry;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "樱桃炸弹", "炸死一个区域内的全部僵尸");
			if (IsEasy)
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(12f, 16f);
				Weights = new List<List<int>>
				{
					new List<int> { 0, 1, 2, 3, 12, 8, 10, 16 },
					new List<int> { 1, 2, 3, 6, 10, 8, 12, 20 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 250;
				SetupTime = new Vector2(10f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 1, 4, 8, 8, 14, 22 },
					new List<int> { 1, 1, 2, 4, 10, 8, 14, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.PeaShooterZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.FootballZombie,
						ZombieType.PeaShooterZombie
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV10011()
	{
		if (IsEasy)
		{
			StartTime = 860;
		}
		else
		{
			StartTime = 1000;
		}
		BootyPlant = PlantType.SunShroom;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "阳光菇", "开始提供少量阳光\n一段时间后提供大量阳光");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 6, 16, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(8f, 12f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 10, 24, 45 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie
					}
				};
			}
		}
	}

	private void LV10012()
	{
		if (IsEasy)
		{
			StartTime = 1080;
		}
		else
		{
			StartTime = 1000;
		}
		BootyPlant = PlantType.PuffShroom;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "小喷菇", "免费种植，但是射程很近。");
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 4, 6, 8, 16 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.PaperZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(8f, 10f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 8, 14, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie
					}
				};
			}
		}
	}

	private void LV10013()
	{
		if (IsEasy)
		{
			StartTime = 1110;
		}
		else
		{
			StartTime = 1030;
		}
		BootyPlant = PlantType.FumeShroom;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "大喷菇", "喷射可以穿过门板的气液。");
			if (IsEasy)
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(13f, 16f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 12, 16, 24 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.PaperZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(9f, 12f);
				BigWaveNum = new List<int> { 5 };
				Weights = new List<List<int>>
				{
					new List<int> { 3, 5, 10, 14, 24, 45 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.DoorZombie,
						ZombieType.DoorAndBucket,
						ZombieType.PeaShooterZombie
					}
				};
			}
		}
	}

	private void LV10014()
	{
		if (IsEasy)
		{
			StartTime = 1130;
		}
		else
		{
			StartTime = 1050;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		BootySprite = BootySprite.Almanac;
		GraveZombie = new List<ZombieType>
		{
			ZombieType.NormalZombie,
			ZombieType.ConeZombie
		};
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "图鉴", "记载你所有遇到的植物和僵尸");
		BootyEvent = () =>
		{
			GameManager.Instance.LocalPlayerSave.AlmanacUnLock = true;
		};
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(12f, 16f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 3, 6, 8, 10, 14, 22 },
				new List<int> { 0, 2, 3, 6, 8, 12, 10, 14, 22 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.TubeConeZombie,
					ZombieType.PaperZombie,
					ZombieType.DoorZombie
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 4;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			return;
		}
		LvNormalSunNum = 250;
		SetupTime = new Vector2(8f, 12f);
		BigWaveNum = new List<int> { 4, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 4, 6, 8, 10, 16, 16, 24, 33 },
			new List<int> { 1, 2, 4, 8, 10, 16, 16, 24, 33 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.Polevaulter,
				ZombieType.DoorAndCone,
				ZombieType.PaperZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeConeZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone
			}
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].GraveStoneNum += 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
			MapManager.Instance.mapList[1].GraveStoneNum += 8;
			MapManager.Instance.mapList[1].GraveStoneLine = 6;
			MapManager.Instance.mapList[1].SpawnAllGraveStone();
		};
	}

	private void LV10015()
	{
		if (IsEasy)
		{
			StartTime = 1140;
		}
		else
		{
			StartTime = 1140;
		}
		BootyPlant = PlantType.Gravebuster;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "墓碑吞噬者", "将它种在墓碑上用来吞噬墓碑");
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(18f, 22f);
			BigWaveNum = new List<int> { 6, 10 };
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 0, 2, 2, 4, 10, 12, 12, 14, 18,
					22
				},
				new List<int>
				{
					0, 2, 2, 4, 4, 9, 10, 12, 12, 14,
					20
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.BucketZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.DoorZombie
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.CreatePortal(2, 0);
				MapManager.Instance.CreatePortal(2, 2);
			};
		}
		else
		{
			LvNormalSunNum = 75;
			SetupTime = new Vector2(15f, 20f);
			BigWaveNum = new List<int> { 6, 10 };
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 1, 4, 6, 8, 12, 18, 12, 20, 15,
					25
				},
				new List<int>
				{
					1, 2, 2, 4, 4, 12, 25, 16, 18, 20,
					28
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.FootballZombie,
					ZombieType.PaperZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.PaperZombie,
					ZombieType.DoorZombie
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.CreatePortal(2, 0);
				MapManager.Instance.CreatePortal(3, 1);
				MapManager.Instance.CreatePortal(3, 2);
			};
		}
	}

	private void LV10016()
	{
		if (IsEasy)
		{
			StartTime = 1170;
		}
		else
		{
			StartTime = 1140;
		}
		BootyPlant = PlantType.Chomper;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "大嘴花", "能一次吞下一只僵尸，\n且咀嚼状态非常脆弱");
		if (IsEasy)
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 6 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 8, 10, 16, 24 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.TubeConeZombie,
					ZombieType.DoorZombie,
					ZombieType.BlackFootball
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 4;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			return;
		}
		LvNormalSunNum = 125;
		SetupTime = new Vector2(8f, 12f);
		BigWaveNum = new List<int> { 4, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 2, 2, 4, 6, 18, 16, 20, 24, 32 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.TubeDoorBucketZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.DoorZombie,
				ZombieType.BlackFootball
			}
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].GraveStoneNum += 8;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
	}

	private void LV10017()
	{
		if (IsEasy)
		{
			StartTime = 1260;
		}
		else
		{
			StartTime = 1230;
		}
		BootyPlant = PlantType.Torchwood;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (OnlyInfo)
		{
			return;
		}
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "火炬树桩", "通过火炬树桩的豌豆将变为火球");
		if (IsEasy)
		{
			LvNormalSunNum = 50;
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 5 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 3, 6, 12, 20 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.BlackFootball,
					ZombieType.Zomboni
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(2);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 6;
				MapManager.Instance.mapList[0].GraveStoneLine = 5;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(363, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 75;
		SetupTime = new Vector2(10f, 14f);
		BigWaveNum = new List<int> { 5 };
		Weights = new List<List<int>>
		{
			new List<int> { 2, 3, 6, 10, 16, 40 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.BlackFootball,
				ZombieType.Zomboni,
				ZombieType.RepeaterZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(4);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 8;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(363, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10018()
	{
		if (IsEasy)
		{
			StartTime = 1300;
		}
		else
		{
			StartTime = 1260;
		}
		BootyPlant = PlantType.Plantern;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "路灯花", "照亮一片区域，清除迷雾，\n并且为白天产光植物增产");
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 4, 7 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 6, 10, 8, 18, 24 },
				new List<int> { 1, 2, 2, 6, 12, 10, 18, 24 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.FootballZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.PaperZombie,
					ZombieType.Zomboni,
					ZombieType.SnorkleZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(2);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(363, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 200;
		SetupTime = new Vector2(12f, 16f);
		BigWaveNum = new List<int> { 4, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 2, 2, 3, 8, 22, 14, 18, 24, 26 },
			new List<int> { 2, 2, 4, 8, 16, 14, 16, 22, 32 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.PeaShooterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.BlackFootball,
				ZombieType.SnorkleZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(4);
			MapManager.Instance.mapList[1].fog.CreateFog(4);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 5;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
			MapManager.Instance.mapList[1].fog.MoveIn();
			MapManager.Instance.mapList[1].GraveStoneNum += 4;
			MapManager.Instance.mapList[1].GraveStoneLine = 6;
			MapManager.Instance.mapList[1].SpawnAllGraveStone();
		};
		TimeAction.Add(363, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV10019()
	{
		if (IsEasy)
		{
			StartTime = 1260;
		}
		else
		{
			StartTime = 1230;
		}
		CurrBankType = BankType.ConveryorBelt;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.UltimateBattle;
		nightBgm = BgmType.UltimateBattle;
		BootySprite = BootySprite.ZombieNote;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "雾中决战！");
		BootyEvent = () =>
		{
			if (GameManager.Instance.LocalPlayerSave.CardSlotNum < 8)
			{
				GameManager.Instance.LocalPlayerSave.CardSlotNum = 8;
			}
		};
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		GeneralCardPool = new List<CardType>
		{
			new CardType(PlantType.Chomper),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.Repeater),
			new CardType(PlantType.Cherry),
			new CardType(PlantType.Coffeebean),
			new CardType(PlantType.PuffShroom),
			new CardType(PlantType.PuffShroom),
			new CardType(PlantType.PuffShroom),
			new CardType(PlantType.FumeShroom),
			new CardType(PlantType.PotatoMine),
			new CardType(PlantType.FumeShroom),
			new CardType(PlantType.Repeater),
			new CardType(PlantType.Lilypad),
			new CardType(PlantType.Lilypad),
			new CardType(PlantType.Tanglekelp)
		};
		if (IsEasy)
		{
			SetupTime = new Vector2(4f, 8f);
			BigWaveNum = new List<int> { 4, 7 };
			Weights = new List<List<int>>
			{
				new List<int> { 4, 6, 12, 18, 24, 28, 22, 44 },
				new List<int> { 4, 8, 12, 14, 18, 22, 28, 42 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.BucketZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.DoorZombie,
					ZombieType.PaperZombie,
					ZombieType.FootballZombie
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			return;
		}
		SetupTime = new Vector2(4f, 8f);
		BigWaveNum = new List<int> { 4, 7 };
		Weights = new List<List<int>>
		{
			new List<int> { 5, 8, 16, 22, 28, 38, 38, 54 },
			new List<int> { 4, 6, 18, 18, 34, 28, 38, 54 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.BlackFootball,
				ZombieType.PaperZombie,
				ZombieType.Zomboni,
				ZombieType.PeaShooterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndBucket,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.PeaShooterZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(3);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 3;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(376, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10020()
	{
		if (IsEasy)
		{
			StartTime = 1320;
		}
		else
		{
			StartTime = 1260;
		}
		BootyPlant = PlantType.SeaShroom;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "水兵菇", "可以直接种植在水中攻击僵尸");
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(12f, 16f);
			BigWaveNum = new List<int> { 5, 8, 11 };
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 0, 2, 6, 6, 12, 12, 18, 24, 18,
					26, 34
				},
				new List<int>
				{
					0, 2, 3, 8, 8, 14, 12, 18, 28, 20,
					26, 32
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.BlackFootball
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.TubeConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.Zomboni,
					ZombieType.PaperZombie,
					ZombieType.SnorkleZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(2);
				MapManager.Instance.mapList[1].fog.CreateFog(4);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[1].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 4;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(376, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
				MapManager.Instance.mapList[1].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 100;
		SetupTime = new Vector2(8f, 12f);
		BigWaveNum = new List<int> { 5, 8, 11 };
		Weights = new List<List<int>>
		{
			new List<int>
			{
				1, 1, 3, 6, 10, 16, 12, 22, 28, 20,
				26, 44
			},
			new List<int>
			{
				1, 2, 2, 6, 10, 16, 12, 24, 30, 20,
				26, 35
			}
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.Polevaulter,
				ZombieType.BlackFootball,
				ZombieType.PaperZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.DoorAndCone,
				ZombieType.SnorkleZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(4);
			MapManager.Instance.mapList[1].fog.CreateFog(6);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[1].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 8;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
			MapManager.Instance.mapList[1].GraveStoneNum += 3;
			MapManager.Instance.mapList[1].GraveStoneLine = 5;
			MapManager.Instance.mapList[1].SpawnAllGraveStone();
		};
		TimeAction.Add(396, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV10021()
	{
		if (IsEasy)
		{
			StartTime = 1340;
		}
		else
		{
			StartTime = 1250;
		}
		BootyPlant = PlantType.SplitPea;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (OnlyInfo)
		{
			return;
		}
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "裂荚射手", "前后双向发射豌豆");
		if (IsEasy)
		{
			LvNormalSunNum = 50;
			SetupTime = new Vector2(18f, 24f);
			BigWaveNum = new List<int> { 6 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 3, 6, 10, 14, 22 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DiggerZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].GraveStoneLine = 5;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(371, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(16f, 22f);
		BigWaveNum = new List<int> { 5, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 3, 6, 8, 18, 12, 20, 34 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.DiggerZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(6);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(371, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10022()
	{
		if (IsEasy)
		{
			StartTime = 1370;
		}
		else
		{
			StartTime = 1260;
		}
		BootyPlant = PlantType.Cactus;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "仙人掌", "能发射穿透多个僵尸的子弹\n且能攻击气球僵尸");
		if (IsEasy)
		{
			LvNormalSunNum = 50;
			SetupTime = new Vector2(18f, 24f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 3, 6, 10, 16, 14, 18, 26 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DiggerZombie,
					ZombieType.PaperZombie,
					ZombieType.DoorAndCone,
					ZombieType.DolphinriderZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 5;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(376, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(16f, 22f);
		BigWaveNum = new List<int> { 5, 9 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 2, 6, 8, 24, 16, 20, 26, 35 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.DiggerZombie,
				ZombieType.DoorAndCone,
				ZombieType.DolphinriderZombie,
				ZombieType.RepeaterZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(5);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 7;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(376, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10023()
	{
		if (IsEasy)
		{
			StartTime = 1370;
		}
		else
		{
			StartTime = 1260;
		}
		BootyPlant = PlantType.IceShroom;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "寒冰菇", "暂时使所有敌人无法移动");
		if (IsEasy)
		{
			LvNormalSunNum = 50;
			SetupTime = new Vector2(18f, 24f);
			BigWaveNum = new List<int> { 6 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 3, 4, 8, 14, 28 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.BalloonZombie,
					ZombieType.DiggerZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 4;
				MapManager.Instance.mapList[0].GraveStoneLine = 5;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(376, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(16f, 22f);
		BigWaveNum = new List<int> { 5, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 2, 2, 4, 14, 16, 28, 14, 26, 38 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.PaperZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.BalloonZombie,
				ZombieType.DiggerZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(6);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(376, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10024()
	{
		if (IsEasy)
		{
			StartTime = 1400;
		}
		else
		{
			StartTime = 1300;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		BootySprite = BootySprite.Store;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "商店钥匙", "现在你可以访问疯狂戴夫的商店！");
		BootyEvent = () =>
		{
			if (GameManager.Instance.LocalPlayerSave.StoreLvl < 1)
			{
				GameManager.Instance.LocalPlayerSave.StoreLvl = 1;
			}
		};
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 0, 2, 4, 10, 10, 14, 18, 24, 28 },
				new List<int> { 1, 2, 4, 6, 10, 16, 16, 22, 28 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.Polevaulter,
					ZombieType.BalloonZombie,
					ZombieType.FootballZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BlackFootball,
					ZombieType.PaperZombie,
					ZombieType.DiggerZombie,
					ZombieType.SnorkleZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(4);
				MapManager.Instance.mapList[1].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[1].fog.MoveIn();
				MapManager.Instance.mapList[1].GraveStoneNum += 3;
				MapManager.Instance.mapList[1].GraveStoneLine = 6;
				MapManager.Instance.mapList[1].SpawnAllGraveStone();
			};
			TimeAction.Add(372, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
				MapManager.Instance.mapList[1].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 100;
		SetupTime = new Vector2(8f, 12f);
		BigWaveNum = new List<int> { 5, 8, 11 };
		Weights = new List<List<int>>
		{
			new List<int>
			{
				1, 2, 4, 6, 8, 12, 18, 16, 20, 25,
				30, 45
			},
			new List<int>
			{
				1, 1, 4, 6, 8, 12, 18, 16, 20, 25,
				30, 45
			}
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.Polevaulter,
				ZombieType.BlackFootball,
				ZombieType.PaperZombie,
				ZombieType.BalloonZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.DiggerZombie,
				ZombieType.Zomboni,
				ZombieType.SnorkleZombie,
				ZombieType.DolphinriderZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(6);
			MapManager.Instance.mapList[1].fog.CreateFog(3);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[1].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 4;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
			MapManager.Instance.mapList[1].GraveStoneNum += 3;
			MapManager.Instance.mapList[1].GraveStoneLine = 6;
			MapManager.Instance.mapList[1].SpawnAllGraveStone();
		};
		TimeAction.Add(376, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV10025()
	{
		if (IsEasy)
		{
			StartTime = 1412;
		}
		else
		{
			StartTime = 1310;
		}
		BootyPlant = PlantType.Pot;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		LvSpStates.Add(LVSpState.SmallZombie);
		dayBgm = BgmType.WateryGraves;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "花盆", "可以让你在屋顶上种植植物");
		if (IsEasy)
		{
			LvNormalSunNum = 250;
			SetupTime = new Vector2(25f, 30f);
			BigWaveNum = new List<int> { 5, 9 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 6, 12, 18, 12, 22, 28, 32 },
				new List<int> { 1, 2, 4, 8, 10, 18, 14, 26, 28, 32 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.PaperZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.Zomboni,
					ZombieType.PaperZombie,
					ZombieType.SnorkleZombie
				}
			};
			return;
		}
		LvNormalSunNum = 100;
		SetupTime = new Vector2(18f, 22f);
		BigWaveNum = new List<int> { 5, 9 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 4, 6, 12, 22, 20, 28, 34, 45 },
			new List<int> { 2, 2, 4, 6, 15, 20, 18, 26, 28, 42 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.Polevaulter,
				ZombieType.BlackFootball,
				ZombieType.PaperZombie,
				ZombieType.SnowpeaShooterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.SnorkleZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(2);
			MapManager.Instance.mapList[1].fog.CreateFog(3);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[1].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 7;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(381, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV10026()
	{
		if (IsEasy)
		{
			StartTime = 360;
		}
		else
		{
			StartTime = 280;
		}
		BootyPlant = PlantType.Cabbagepult;
		LoadMapTypes = new List<MapType> { MapType.Roof };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "卷心菜投手", "投掷卷心菜来攻击僵尸");
			if (IsEasy)
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 6 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 6, 10, 12, 20 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(10f, 14f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 6, 8, 10, 22, 16, 20, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PaperZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.LadderZombie,
						ZombieType.DoorAndCone
					}
				};
			}
		}
	}

	private void LV10027()
	{
		if (IsEasy)
		{
			StartTime = 392;
		}
		else
		{
			StartTime = 290;
		}
		BootyPlant = PlantType.HypnoShroom;
		LoadMapTypes = new List<MapType> { MapType.Roof };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "魅惑菇", "让一只僵尸为你作战");
			if (IsEasy)
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 10, 12, 16, 12, 22, 30 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.BungiZombie,
						ZombieType.Polevaulter
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(8f, 12f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 6, 12, 24, 20, 28, 42 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PaperZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.BungiZombie,
						ZombieType.CabbagepultZombie
					}
				};
			}
		}
	}

	private void LV10028()
	{
		if (IsEasy)
		{
			StartTime = 437;
		}
		else
		{
			StartTime = 312;
		}
		BootyPlant = PlantType.Tallnut;
		LoadMapTypes = new List<MapType> { MapType.Roof };
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "高坚果", "不会被跳过的坚实堡垒");
			if (IsEasy)
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 6 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 2, 6, 10, 12, 20 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.Polevaulter,
						ZombieType.PogoZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(16f, 22f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 14, 24, 16, 25, 38 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PogoZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.PogoZombie,
						ZombieType.BungiZombie
					}
				};
			}
		}
	}

	private void LV10029()
	{
		if (IsEasy)
		{
			StartTime = 454;
		}
		else
		{
			StartTime = 369;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.GrazeTheRoof;
		BootySprite = BootySprite.ZombieNote;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "大战将要到来");
		BootyEvent = () =>
		{
			if (GameManager.Instance.LocalPlayerSave.CardSlotNum < 9)
			{
				GameManager.Instance.LocalPlayerSave.CardSlotNum = 9;
			}
		};
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 2, 4, 10, 14, 12, 20, 30 },
				new List<int> { 0, 2, 2, 4, 6, 14, 10, 16, 20 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.Polevaulter,
					ZombieType.FootballZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.PogoZombie,
					ZombieType.Polevaulter,
					ZombieType.DoorZombie
				}
			};
		}
		else
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(8f, 12f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 4, 6, 10, 22, 15, 28, 45 },
				new List<int> { 2, 2, 4, 6, 8, 18, 15, 28, 45 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.Zomboni,
					ZombieType.Polevaulter,
					ZombieType.BlackFootball,
					ZombieType.PaperZombie,
					ZombieType.PogoZombie,
					ZombieType.Zomboni,
					ZombieType.SnowpeaShooterZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.PaperZombie,
					ZombieType.Polevaulter,
					ZombieType.FootballZombie,
					ZombieType.PogoZombie
				}
			};
		}
	}

	private void LV10030()
	{
		if (IsEasy)
		{
			StartTime = 491;
		}
		else
		{
			StartTime = 372;
		}
		BootyPlant = PlantType.Jalapeno;
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Wind, 2, 30)
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.GrazeTheRoof;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "火爆辣椒", "消灭整行的敌人");
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 6, 11 };
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 0, 2, 4, 12, 14, 18, 12, 18, 22,
					20, 34
				},
				new List<int>
				{
					0, 2, 4, 4, 8, 12, 18, 10, 18, 22,
					20, 28
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.FootballZombie,
					ZombieType.DolphinriderZombie,
					ZombieType.SnorkleZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.PogoZombie,
					ZombieType.BungiZombie,
					ZombieType.PaperZombie
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			return;
		}
		LvNormalSunNum = 100;
		SetupTime = new Vector2(8f, 12f);
		BigWaveNum = new List<int> { 5, 8, 11 };
		Weights = new List<List<int>>
		{
			new List<int>
			{
				1, 2, 6, 8, 12, 18, 16, 20, 25, 20,
				20, 42
			},
			new List<int>
			{
				1, 3, 4, 8, 12, 18, 16, 20, 25, 18,
				25, 42
			}
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeZombie,
				ZombieType.BucketZombie,
				ZombieType.DolphinriderZombie,
				ZombieType.Polevaulter,
				ZombieType.BlackFootball,
				ZombieType.PaperZombie,
				ZombieType.Gargantuar,
				ZombieType.DiggerZombie,
				ZombieType.SnorkleZombie,
				ZombieType.PeaShooterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.PogoZombie,
				ZombieType.BungiZombie,
				ZombieType.DoorAndBucket,
				ZombieType.SunflowerZombie
			}
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].GraveStoneNum += 8;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
	}

	private void LV10031()
	{
		if (IsEasy)
		{
			StartTime = 542;
		}
		else
		{
			StartTime = 450;
		}
		BootyPlant = PlantType.Umbrellaleaf;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "萝卜伞", "保护周围植物不被投石车伤害");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 6 };
				Weights = new List<List<int>>
				{
					new List<int> { 0, 1, 2, 4, 12, 14, 18 },
					new List<int> { 1, 2, 4, 4, 8, 12, 18 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.CatapultZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(8f, 12f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 8, 12, 22, 16, 20, 38 },
					new List<int> { 1, 2, 6, 8, 12, 18, 16, 26, 38 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.PaperZombie,
						ZombieType.CatapultZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.PaperZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.CatapultZombie
					}
				};
			}
		}
	}

	private void LV10032()
	{
		if (IsEasy)
		{
			StartTime = 620;
		}
		else
		{
			StartTime = 563;
		}
		BootyPlant = PlantType.Cornpult;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 5),
			new FutureWeather(WeatherType.Hail, 3, 30)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "玉米投手", "投掷玉米粒和黄油");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 12, 14, 18, 12, 16, 22 },
					new List<int> { 0, 1, 2, 8, 12, 18, 10, 15, 20 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni,
						ZombieType.CatapultZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.CatapultZombie,
						ZombieType.BungiZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(8f, 12f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 6, 8, 18, 22, 16, 28, 30 },
					new List<int> { 1, 3, 6, 8, 18, 25, 16, 22, 38 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.PaperZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.PaperZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.PogoZombie
					}
				};
			}
		}
	}

	private void LV10033()
	{
		if (IsEasy)
		{
			StartTime = 674;
		}
		else
		{
			StartTime = 592;
		}
		BootyPlant = PlantType.ScaredyShroom;
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "胆小菇", "攻击整行的僵尸");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(18f, 20f);
				BigWaveNum = new List<int> { 7 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 2, 2, 4, 12, 14, 28 },
					new List<int> { 0, 1, 2, 4, 4, 8, 12, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Gargantuar
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(15f, 18f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 8, 12, 22, 16, 28, 30 },
					new List<int> { 1, 4, 6, 8, 12, 22, 16, 22, 38 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.Gargantuar,
						ZombieType.BungiZombie,
						ZombieType.SnorkleZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.PaperZombie,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar,
						ZombieType.GargantuarInjured,
						ZombieType.CatapultZombie
					}
				};
			}
		}
	}

	private void LV10034()
	{
		if (IsEasy)
		{
			StartTime = 722;
		}
		else
		{
			StartTime = 699;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.GrazeTheRoof;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "奖杯", "沼泽关卡已开启");
		BootyEvent = () =>
		{
			GameManager.Instance.LocalPlayerSave.SwampOpen = true;
		};
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 5, 9 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 2, 4, 12, 18, 10, 12, 16, 22 },
				new List<int> { 0, 1, 4, 4, 8, 18, 12, 10, 15, 25 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.BalloonZombie,
					ZombieType.Gargantuar,
					ZombieType.SnorkleZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.Gargantuar
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
			};
			TimeAction.Add(367, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(15f, 20f);
		BigWaveNum = new List<int> { 5, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 4, 8, 12, 22, 16, 28, 35 },
			new List<int> { 2, 2, 4, 8, 12, 18, 16, 22, 35 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeZombie,
				ZombieType.BucketZombie,
				ZombieType.Polevaulter,
				ZombieType.Zomboni,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.BalloonZombie,
				ZombieType.SnorkleZombie,
				ZombieType.DolphinriderZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.GargantuarHelmet,
				ZombieType.BungiZombie,
				ZombieType.Gargantuar
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(5);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
		};
		TimeAction.Add(367, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10035()
	{
		if (IsEasy)
		{
			StartTime = 762;
		}
		else
		{
			StartTime = 739;
		}
		BootyPlant = PlantType.Starfruit;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "杨桃", "朝五个方向发射发光的星星");
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				SetupTime = new Vector2(16f, 20f);
				BigWaveNum = new List<int> { 6 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 2, 8, 12, 14, 25 },
					new List<int> { 0, 1, 2, 4, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PogoZombie,
						ZombieType.JacksonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PogoZombie,
						ZombieType.PogoZombie,
						ZombieType.PogoZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 5, 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 22, 16, 22, 35 },
					new List<int> { 1, 2, 4, 6, 12, 18, 16, 22, 40 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PogoZombie,
						ZombieType.PogoZombie,
						ZombieType.BungiZombie,
						ZombieType.JacksonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.PogoZombie,
						ZombieType.BungiZombie,
						ZombieType.PogoZombie
					}
				};
			}
		}
	}

	private void LV10036()
	{
		if (IsEasy)
		{
			StartTime = 830;
		}
		else
		{
			StartTime = 720;
		}
		BootyPlant = PlantType.Pumpkin;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 10, 0)
		};
		if (!OnlyInfo)
		{
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "南瓜头", "可以用来保护其他植物");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(16f, 20f);
				BigWaveNum = new List<int> { 5, 9 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 2, 4, 12, 18, 12, 12, 10, 22 },
					new List<int> { 0, 1, 2, 4, 8, 16, 12, 10, 15, 28 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.JacksonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombie,
						ZombieType.GargantuarInjured
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 5, 8, 11 };
				WaterZombieNum = 4;
				WaterZombie = new List<ZombieType>
				{
					ZombieType.TubeZombie,
					ZombieType.TubeConeZombie,
					ZombieType.TubeBucketZombie
				};
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 4, 8, 12, 22, 16, 22, 28, 15,
						25, 40
					},
					new List<int>
					{
						1, 2, 4, 8, 12, 18, 16, 22, 30, 20,
						25, 35
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar,
						ZombieType.PaperZombie,
						ZombieType.DiggerZombie,
						ZombieType.JacksonZombie,
						ZombieType.PeaShooterZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombie,
						ZombieType.GargantuarHelmet,
						ZombieType.CatapultZombie,
						ZombieType.JacksonZombie
					}
				};
			}
		}
	}

	private void LV10037()
	{
		if (IsEasy)
		{
			StartTime = 1020;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 3, 0),
				new FutureWeather(WeatherType.Thunder, 10, 240)
			};
		}
		else
		{
			StartTime = 912;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Rain, 3, 0),
				new FutureWeather(WeatherType.Thunder, 10, 182)
			};
		}
		BootyPlant = PlantType.Clematis;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "铁线莲", "大范围吸收雷电");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(16f, 20f);
				BigWaveNum = new List<int> { 8 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 2, 2, 4, 12, 14, 14, 18 },
					new List<int> { 0, 1, 2, 4, 4, 8, 12, 18, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.Polevaulter
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 6, 9 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 8, 12, 22, 16, 22, 35 },
					new List<int> { 0, 2, 4, 6, 8, 12, 18, 16, 22, 40 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.Polevaulter,
						ZombieType.BalloonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.SnorkleZombie
					}
				};
			}
		}
	}

	private void LV10038()
	{
		if (IsEasy)
		{
			StartTime = 1021;
		}
		else
		{
			StartTime = 915;
		}
		BootyPlant = PlantType.Magnetshroom;
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.GrazeTheRoof;
		nightBgm = BgmType.RigorMormist;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "磁力菇", "吸取僵尸的金属装备");
		if (IsEasy)
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 6 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 2, 4, 12, 14, 18 },
				new List<int> { 0, 2, 4, 8, 10, 14, 22 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.TubeConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.DolphinriderZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.JackboxZombie,
					ZombieType.Zomboni
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
			};
			TimeAction.Add(367, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(10f, 15f);
		BigWaveNum = new List<int> { 5, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 4, 8, 12, 22, 16, 22, 35 },
			new List<int> { 2, 3, 6, 8, 12, 22, 16, 22, 40 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.TubeConeZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.DolphinriderZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.JackboxZombie,
				ZombieType.Zomboni,
				ZombieType.Gargantuar
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(6);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
		};
		TimeAction.Add(375, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
		});
	}

	private void LV10039()
	{
		if (IsEasy)
		{
			StartTime = 1045;
		}
		else
		{
			StartTime = 930;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Wind, 1, 0),
			new FutureWeather(WeatherType.Wind, 3, 36),
			new FutureWeather(WeatherType.Wind, 5, 140)
		};
		if (OnlyInfo)
		{
			return;
		}
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		dayBgm = BgmType.GrazeTheRoof;
		nightBgm = BgmType.RigorMormist;
		BootySprite = BootySprite.ZombieNote;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "规模更大的战斗即将来袭");
		if (IsEasy)
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 5, 9 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 4, 12, 16, 12, 20, 18, 28 },
				new List<int> { 0, 1, 3, 4, 8, 22, 17, 20, 12, 34 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.SnorkleZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.JackboxZombie,
					ZombieType.Zomboni,
					ZombieType.GargantuarInjured
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(5);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 4;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(343, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
		}
		else
		{
			LvNormalSunNum = 50;
			SetupTime = new Vector2(10f, 15f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 4, 12, 14, 26, 22, 28, 45 },
				new List<int> { 0, 2, 6, 10, 18, 26, 22, 28, 38 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.Gargantuar,
					ZombieType.SnorkleZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.JackboxZombie,
					ZombieType.Zomboni,
					ZombieType.Gargantuar
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(6);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 8;
				MapManager.Instance.mapList[0].GraveStoneLine = 5;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(353, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
			});
		}
	}

	private void LV10040()
	{
		if (IsEasy)
		{
			StartTime = 1048;
		}
		else
		{
			StartTime = 947;
		}
		BootyPlant = PlantType.DoomShroom;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "毁灭菇", "摧毁大范围的僵尸");
		if (IsEasy)
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 7 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 2, 4, 12, 14, 22 },
				new List<int> { 1, 0, 1, 2, 4, 12, 14, 18 },
				new List<int> { 0, 1, 2, 4, 4, 8, 12, 22 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.TubeConeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.Zomboni
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.CatapultZombie
				}
			};
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(10f, 15f);
		BigWaveNum = new List<int> { 6, 9 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 4, 6, 8, 12, 22, 16, 22, 45 },
			new List<int> { 1, 2, 4, 6, 8, 12, 22, 16, 22, 35 },
			new List<int> { 0, 2, 4, 6, 8, 12, 18, 16, 22, 30 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.GargantuarInjured,
				ZombieType.FootballZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.Zomboni,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.CatapultZombie,
				ZombieType.BlackFootball,
				ZombieType.Gargantuar
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(3);
			MapManager.Instance.mapList[1].fog.CreateFog(5);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[1].fog.MoveIn();
		};
		TimeAction.Add(389, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV10041()
	{
		if (IsEasy)
		{
			StartTime = 1100;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 2, 0),
				new FutureWeather(WeatherType.Snow, 5, 225)
			};
		}
		else
		{
			StartTime = 950;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 3, 0),
				new FutureWeather(WeatherType.Snow, 7, 225)
			};
		}
		BootyPlant = PlantType.SnowPea;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "寒冰射手", "降低僵尸移动速度");
			if (IsEasy)
			{
				LvNormalSunNum = 75;
				SetupTime = new Vector2(16f, 20f);
				BigWaveNum = new List<int> { 7 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 2, 2, 4, 12, 14, 18 },
					new List<int> { 0, 1, 2, 4, 4, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.TubeConeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndCone
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 6, 9 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 8, 12, 22, 16, 22, 35 },
					new List<int> { 1, 2, 4, 6, 8, 12, 18, 16, 22, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndBucket,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.PaperZombie,
						ZombieType.Zomboni
					}
				};
			}
		}
	}

	private void LV10042()
	{
		if (IsEasy)
		{
			StartTime = 1165;
		}
		else
		{
			StartTime = 970;
		}
		BootyPlant = PlantType.ThreePeater;
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "三线射手", "同时向3条线发射豌豆");
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(12f, 15f);
				BigWaveNum = new List<int> { 7 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 2, 2, 4, 12, 14, 18 },
					new List<int> { 0, 1, 2, 4, 4, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndBucket,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndCone,
						ZombieType.LadderZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 6, 9 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 8, 12, 24, 20, 32, 35 },
					new List<int> { 2, 3, 4, 8, 10, 12, 28, 16, 32, 45 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.TubeDoorConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndBucket,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.BalloonZombie,
						ZombieType.Zomboni
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndBucket,
						ZombieType.LadderZombie,
						ZombieType.Gargantuar,
						ZombieType.JackboxZombie
					}
				};
			}
		}
	}

	private void LV10043()
	{
		if (IsEasy)
		{
			StartTime = 1045;
		}
		else
		{
			StartTime = 930;
		}
		BootyPlant = PlantType.Garlic;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "大蒜", "使吃它的僵尸换行");
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 2, 4, 12, 18, 12, 18, 35 },
				new List<int> { 0, 1, 2, 4, 8, 12, 12, 20, 28 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.LadderZombie,
					ZombieType.BlackFootball
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorAndCone,
					ZombieType.LadderZombie,
					ZombieType.Zomboni,
					ZombieType.Gargantuar,
					ZombieType.JacksonZombie,
					ZombieType.DolphinriderZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(5);
				MapManager.Instance.mapList[1].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[1].fog.MoveIn();
				MapManager.Instance.mapList[0].GraveStoneNum += 6;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
			TimeAction.Add(349, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
				MapManager.Instance.mapList[1].fog.CloseFog();
			});
			return;
		}
		LvNormalSunNum = 50;
		SetupTime = new Vector2(10f, 15f);
		BigWaveNum = new List<int> { 5, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 4, 8, 12, 22, 16, 22, 35 },
			new List<int> { 0, 1, 2, 6, 8, 18, 16, 22, 45 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.LadderZombie,
				ZombieType.DiggerZombie,
				ZombieType.SnowRepeaterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.TubeConeZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorAndBucket,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.Zomboni,
				ZombieType.Gargantuar,
				ZombieType.LadderZombie,
				ZombieType.DiggerZombie,
				ZombieType.JacksonZombie,
				ZombieType.DolphinriderZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(7);
			MapManager.Instance.mapList[1].fog.CreateFog(3);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[1].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 8;
			MapManager.Instance.mapList[0].GraveStoneLine = 5;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(357, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV10044()
	{
		StartTime = 1148;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		BootySprite = BootySprite.Taco;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "玉米卷", "戴夫商店已出现新的货物");
		FirstBootyEvent = () =>
		{
			Coinbank.Instance.ShowCoinbank();
			PlayerManager.Instance.Money += 1000;
			if (GameManager.Instance.LocalPlayerSave.StoreLvl < 2)
			{
				GameManager.Instance.LocalPlayerSave.StoreLvl = 2;
			}
		};
		BootyEvent = () =>
		{
			if (GameManager.Instance.LocalPlayerSave.StoreLvl < 2)
			{
				GameManager.Instance.LocalPlayerSave.StoreLvl = 2;
			}
		};
		dayBgm = BgmType.WateryGraves;
		nightBgm = BgmType.RigorMormist;
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(20f, 22f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 3, 8, 8, 12, 26, 18, 25, 35 },
				new List<int> { 0, 1, 3, 8, 6, 12, 20, 20, 28, 32 },
				new List<int> { 0, 1, 2, 8, 10, 20, 25, 15, 25, 28 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.GargantuarInjured,
					ZombieType.BucketZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.Gargantuar,
					ZombieType.FootballZombie,
					ZombieType.SnorkleZombie,
					ZombieType.DolphinriderZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.CatapultZombie,
					ZombieType.Gargantuar,
					ZombieType.LadderZombie
				}
			};
			BigWaveNum = new List<int> { 3, 6, 9 };
		}
		else
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(15f, 18f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 3, 8, 8, 12, 22, 18, 35, 45 },
				new List<int> { 1, 1, 3, 8, 6, 12, 20, 25, 28, 38 },
				new List<int> { 1, 2, 3, 12, 10, 20, 25, 15, 30, 48 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.GargantuarInjured,
					ZombieType.BlackFootball,
					ZombieType.CatapultZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.Gargantuar,
					ZombieType.FootballZombie,
					ZombieType.GargantuarHelmet,
					ZombieType.SnorkleZombie,
					ZombieType.DolphinriderZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.CatapultZombie,
					ZombieType.Gargantuar,
					ZombieType.BlackFootball,
					ZombieType.LadderZombie
				}
			};
			BigWaveNum = new List<int> { 3, 6, 9 };
		}
	}

	private void LV10045()
	{
		dayBgm = BgmType.GrazeTheRoof;
		nightBgm = BgmType.MoonGrains;
		if (IsEasy)
		{
			StartTime = 1045;
		}
		else
		{
			StartTime = 930;
		}
		BootyPlant = PlantType.Blover;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		CurrSeedBankType = SeedBankType.MoonBank;
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "三叶草", "吹走浓雾且推动僵尸");
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 200;
			SetupTime = new Vector2(22f, 25f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 12, 8, 12, 18, 15, 18, 22 },
				new List<int> { 0, 1, 3, 8, 10, 18, 18, 15, 20, 20 }
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
		}
		else
		{
			LvNormalSunNum = 100;
			SetupTime = new Vector2(18f, 22f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 2, 6, 4, 8, 12, 22, 20, 25, 40 },
				new List<int> { 1, 2, 4, 10, 10, 18, 18, 15, 20, 30 }
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 5;
				MapManager.Instance.mapList[0].GraveStoneLine = 8;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
		}
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.Zomboni,
				ZombieType.ConeZombie,
				ZombieType.GargantuarInjured,
				ZombieType.JacksonZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
	}

	private void LV10046()
	{
		LvNormalSunNum = 75;
		StartTime = 880;
		BootyPlant = PlantType.Squash;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (IsEasy)
		{
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 2, 0),
				new FutureWeather(WeatherType.Snow, 5, 200),
				new FutureWeather(WeatherType.Snow, 8, 403)
			};
		}
		else
		{
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 2, 0),
				new FutureWeather(WeatherType.Snow, 7, 125),
				new FutureWeather(WeatherType.Snow, 10, 340)
			};
		}
		if (OnlyInfo)
		{
			return;
		}
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "窝瓜", "压扁第一个靠近它的僵尸");
		if (IsEasy)
		{
			SetupTime = new Vector2(15f, 20f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					0, 1, 2, 3, 4, 8, 20, 12, 20, 22,
					28
				},
				new List<int>
				{
					1, 2, 2, 2, 4, 8, 16, 10, 18, 16,
					20, 30
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.BucketZombie,
					ZombieType.Polevaulter,
					ZombieType.GargantuarHelmet,
					ZombieType.Zomboni,
					ZombieType.SnowpeaShooterZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.GargantuarRedeye,
					ZombieType.PaperZombie,
					ZombieType.LadderZombie,
					ZombieType.Zomboni
				}
			};
			BigWaveNum = new List<int> { 6, 10 };
			TimeAction.Add(1283, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum = 6;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[1].GraveStoneNum = 3;
				MapManager.Instance.mapList[1].GraveStoneLine = 6;
			});
			return;
		}
		SetupTime = new Vector2(15f, 20f);
		Weights = new List<List<int>>
		{
			new List<int>
			{
				1, 1, 2, 3, 4, 8, 20, 12, 20, 26,
				38
			},
			new List<int>
			{
				1, 2, 2, 2, 4, 8, 16, 10, 18, 16,
				26, 44
			}
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.BucketZombie,
				ZombieType.LadderZombie,
				ZombieType.Polevaulter,
				ZombieType.GargantuarHelmet,
				ZombieType.Zomboni,
				ZombieType.JacksonZombie,
				ZombieType.SnowRepeaterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.GargantuarRedeye,
				ZombieType.PaperZombie,
				ZombieType.LadderZombie,
				ZombieType.Zomboni,
				ZombieType.SnowpeaShooterZombie
			}
		};
		BigWaveNum = new List<int> { 6, 10 };
		TimeAction.Add(1223, () =>
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 8;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[1].GraveStoneNum = 7;
			MapManager.Instance.mapList[1].GraveStoneLine = 6;
		});
	}

	private void LV10047()
	{
		if (IsEasy)
		{
			StartTime = 1048;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 2, 0),
				new FutureWeather(WeatherType.Snow, 5, 192),
				new FutureWeather(WeatherType.Snow, 7, 380)
			};
		}
		else
		{
			StartTime = 947;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 3, 0),
				new FutureWeather(WeatherType.Snow, 5, 133),
				new FutureWeather(WeatherType.Snow, 10, 340)
			};
		}
		BootyPlant = PlantType.Melonpult;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		AwardScence.Instance.LoadText("你获得了一株新的植物!", "西瓜投手", "造成巨大伤害且范围溅射");
		if (IsEasy)
		{
			LvNormalSunNum = 250;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 7 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 2, 4, 12, 14, 22 },
				new List<int> { 1, 0, 1, 2, 4, 12, 14, 18 },
				new List<int> { 0, 1, 2, 4, 4, 8, 12, 22 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.Gargantuar,
					ZombieType.Zomboni,
					ZombieType.SnowpeaShooterZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.JackboxZombie,
					ZombieType.GargantuarHelmet,
					ZombieType.Zomboni,
					ZombieType.JacksonZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.FootballZombie,
					ZombieType.Polevaulter,
					ZombieType.LadderZombie,
					ZombieType.IceShroomZombie
				}
			};
			return;
		}
		LvNormalSunNum = 150;
		SetupTime = new Vector2(10f, 15f);
		BigWaveNum = new List<int> { 6, 9, 12 };
		Weights = new List<List<int>>
		{
			new List<int>
			{
				1, 1, 2, 4, 6, 8, 22, 16, 22, 35,
				18, 22, 45
			},
			new List<int>
			{
				1, 1, 3, 6, 8, 8, 18, 16, 22, 25,
				16, 22, 45
			},
			new List<int>
			{
				0, 1, 3, 6, 8, 6, 22, 16, 22, 22,
				18, 22, 45
			}
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.PogoZombie,
				ZombieType.FootballZombie,
				ZombieType.Zomboni,
				ZombieType.GargantuarHelmet,
				ZombieType.JacksonZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.Zomboni,
				ZombieType.GargantuarHelmet,
				ZombieType.JacksonZombie,
				ZombieType.SnowRepeaterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.JackboxZombie,
				ZombieType.Gargantuar,
				ZombieType.BungiZombie,
				ZombieType.LadderZombie,
				ZombieType.IceShroomZombie
			}
		};
		TimeAction.Add(1283, () =>
		{
			MapManager.Instance.mapList[0].GraveStoneNum = 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[1].GraveStoneNum = 3;
			MapManager.Instance.mapList[1].GraveStoneLine = 6;
		});
	}

	private void LV10048()
	{
		if (IsEasy)
		{
			StartTime = 1154;
		}
		else
		{
			StartTime = 1001;
		}
		BootyPlant = PlantType.Heronsbill;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0)
		};
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "太阳花", "生产少量阳光且不会被吃");
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.TubeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.TubeBucketZombie
			};
			if (IsEasy)
			{
				LvNormalSunNum = 350;
				SetupTime = new Vector2(16f, 20f);
				BigWaveNum = new List<int> { 7, 11 };
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 1, 2, 2, 4, 12, 14, 22, 14, 22,
						20, 35
					},
					new List<int>
					{
						1, 0, 1, 2, 4, 12, 14, 20, 14, 18,
						25, 30
					},
					new List<int>
					{
						0, 1, 2, 4, 4, 8, 12, 22, 18, 18,
						22, 35
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.DiggerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.Gargantuar,
						ZombieType.Zomboni,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.FootballZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.CabbagepultZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 250;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 6, 9 };
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 8, 12, 18, 22, 16, 22, 45 },
					new List<int> { 1, 2, 6, 8, 8, 12, 22, 16, 22, 35 },
					new List<int> { 1, 2, 4, 6, 8, 18, 20, 18, 24, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmet,
						ZombieType.BungiZombie,
						ZombieType.DiggerZombie,
						ZombieType.UmbrellaZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.DiggerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.GargantuarRedeye,
						ZombieType.JacksonZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.FootballZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.MelonpultZombie,
						ZombieType.UmbrellaZombie
					}
				};
			}
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 6;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
				MapManager.Instance.mapList[1].GraveStoneNum += 8;
				MapManager.Instance.mapList[1].GraveStoneLine = 6;
				MapManager.Instance.mapList[1].SpawnAllGraveStone();
			};
		}
	}

	private void LV10049()
	{
		if (IsEasy)
		{
			StartTime = 1135;
		}
		else
		{
			StartTime = 847;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 5, 0)
		};
		if (OnlyInfo)
		{
			return;
		}
		BootySprite = BootySprite.ZombieNote;
		AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "终极大战来袭");
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 450;
			SetupTime = new Vector2(16f, 20f);
			BigWaveNum = new List<int> { 6, 9, 12 };
			Weights = new List<List<int>>
			{
				new List<int>
				{
					1, 1, 2, 2, 4, 12, 22, 18, 14, 25,
					18, 24, 35
				},
				new List<int>
				{
					1, 0, 1, 2, 4, 12, 14, 18, 12, 20,
					28, 24, 22, 30
				},
				new List<int>
				{
					0, 1, 2, 4, 4, 8, 12, 22, 20, 18,
					24, 24, 17, 28
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.Zomboni,
					ZombieType.ConeZombie,
					ZombieType.Gargantuar,
					ZombieType.GargantuarHelmet,
					ZombieType.BungiZombie,
					ZombieType.JacksonZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.DiggerZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.JackboxZombie,
					ZombieType.Gargantuar,
					ZombieType.CatapultZombie,
					ZombieType.Zomboni,
					ZombieType.SquashZombie,
					ZombieType.SnorkleZombie,
					ZombieType.DolphinriderZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.DoorAndCone,
					ZombieType.JackboxZombie,
					ZombieType.FootballZombie,
					ZombieType.BungiZombie,
					ZombieType.CatapultZombie,
					ZombieType.GargantuarRedeye,
					ZombieType.LadderZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(5);
				MapManager.Instance.mapList[2].fog.CreateFog(3);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[2].fog.MoveIn();
			};
			TimeAction.Add(347, () =>
			{
				MapManager.Instance.mapList[0].fog.CloseFog();
				MapManager.Instance.mapList[2].fog.CloseFog();
			});
		}
		else
		{
			LvNormalSunNum = 350;
			SetupTime = new Vector2(10f, 15f);
			BigWaveNum = new List<int> { 5, 8, 11 };
			Weights = new List<List<int>>
			{
				new List<int>
				{
					2, 2, 4, 6, 8, 18, 22, 16, 30, 28,
					32, 45
				},
				new List<int>
				{
					2, 2, 4, 6, 8, 22, 18, 22, 28, 23,
					30, 40
				},
				new List<int>
				{
					1, 2, 4, 6, 8, 12, 14, 18, 22, 25,
					35, 40
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.BalloonZombie,
					ZombieType.Gargantuar,
					ZombieType.GargantuarHelmet,
					ZombieType.BungiZombie,
					ZombieType.PogoZombie,
					ZombieType.DiggerZombie,
					ZombieType.LadderZombie,
					ZombieType.JacksonZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.DiggerZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.JackboxZombie,
					ZombieType.Gargantuar,
					ZombieType.CatapultZombie,
					ZombieType.Zomboni,
					ZombieType.GargantuarRedeye,
					ZombieType.LadderZombie,
					ZombieType.SnorkleZombie,
					ZombieType.DolphinriderZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.DoorAndCone,
					ZombieType.JackboxZombie,
					ZombieType.FootballZombie,
					ZombieType.BungiZombie,
					ZombieType.CatapultZombie,
					ZombieType.Gargantuar,
					ZombieType.GargantuarHelmet,
					ZombieType.GargantuarRedeye,
					ZombieType.LadderZombie,
					ZombieType.UmbrellaZombie,
					ZombieType.MelonpultZombie
				}
			};
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(5);
				MapManager.Instance.mapList[1].fog.CreateFog(5);
				MapManager.Instance.mapList[2].fog.CreateFog(4);
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[1].fog.MoveIn();
				MapManager.Instance.mapList[2].fog.MoveIn();
			};
		}
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].GraveStoneNum += 6;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
			MapManager.Instance.mapList[1].GraveStoneNum += 8;
			MapManager.Instance.mapList[1].GraveStoneLine = 6;
			MapManager.Instance.mapList[1].SpawnAllGraveStone();
		};
	}

	private void LV10050()
	{
		StartTime = 634;
		BootyPlant = PlantType.Mint;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 6, 366),
			new FutureWeather(WeatherType.Thunder, 10, 486)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "薄荷", "给下方的植物清除冷却");
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.TubeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.TubeBucketZombie
			};
			if (IsEasy)
			{
				LvNormalSunNum = 1200;
				SetupTime = new Vector2(25f, 30f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 3, 10, 6, 8, 12, 16, 20, 30,
						20, 23, 40, 25, 35, 35
					},
					new List<int>
					{
						1, 2, 3, 8, 10, 16, 30, 15, 18, 28,
						30, 33, 40, 30, 30, 45
					},
					new List<int>
					{
						0, 1, 3, 8, 10, 18, 10, 15, 15, 20,
						20, 23, 40, 30, 30, 45
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmet,
						ZombieType.BungiZombie,
						ZombieType.LadderZombie,
						ZombieType.JacksonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.DiggerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.GargantuarRedeye,
						ZombieType.SnorkleZombie,
						ZombieType.DolphinriderZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.FootballZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.LadderZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 1000;
				SetupTime = new Vector2(20f, 25f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 16, 12, 12, 22, 26, 30, 40,
						30, 33, 50, 40, 50, 70
					},
					new List<int>
					{
						2, 2, 3, 12, 10, 20, 30, 15, 25, 28,
						30, 33, 60, 40, 50, 65
					},
					new List<int>
					{
						2, 2, 3, 8, 10, 18, 18, 15, 20, 30,
						30, 33, 50, 40, 50, 65
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.ConeZombie,
						ZombieType.BalloonZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmet,
						ZombieType.BungiZombie,
						ZombieType.PogoZombie,
						ZombieType.DiggerZombie,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.LadderZombie,
						ZombieType.JacksonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.DiggerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet,
						ZombieType.LadderZombie,
						ZombieType.JacksonZombie,
						ZombieType.SnorkleZombie,
						ZombieType.DolphinriderZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.FootballZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmet,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.LadderZombie,
						ZombieType.UmbrellaZombie
					}
				};
			}
			BigWaveNum = new List<int> { 3, 6, 9, 12, 15 };
			TimeAction.Add(1000, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum = 10;
				MapManager.Instance.mapList[0].GraveStoneLine = 4;
				MapManager.Instance.mapList[1].GraveStoneNum = 8;
				MapManager.Instance.mapList[1].GraveStoneLine = 4;
			});
		}
	}

	private void LV10051()
	{
		LvNormalSunNum = 250;
		StartTime = 761;
		BootyPlant = PlantType.Marigold;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 6, 221),
			new FutureWeather(WeatherType.Thunder, 10, 452)
		};
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", "金盏花", "为你生产一些金币");
			GraveZombie = new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie
			};
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.TubeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.TubeBucketZombie
			};
			if (IsEasy)
			{
				SetupTime = new Vector2(10f, 12f);
				BigWaveNum = new List<int> { 5, 10 };
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 3, 4, 5, 16, 8, 12, 10, 12,
						20
					},
					new List<int>
					{
						1, 2, 4, 6, 8, 10, 10, 14, 12, 10,
						28
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.JacksonZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter,
						ZombieType.DiggerZombie,
						ZombieType.CatapultZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.JacksonZombie,
						ZombieType.BungiZombie,
						ZombieType.Gargantuar
					}
				};
			}
			else
			{
				SetupTime = new Vector2(10f, 12f);
				BigWaveNum = new List<int> { 5, 10 };
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 4, 5, 22, 8, 12, 10, 22,
						32
					},
					new List<int>
					{
						2, 3, 4, 6, 8, 18, 10, 14, 12, 28,
						36
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.JacksonZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter,
						ZombieType.DiggerZombie,
						ZombieType.CatapultZombie,
						ZombieType.GargantuarRedeye
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.JacksonZombie,
						ZombieType.BungiZombie,
						ZombieType.Gargantuar,
						ZombieType.Polevaulter,
						ZombieType.DiggerZombie
					}
				};
			}
			TimeAction.Add(1203, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			});
			TimeAction.Add(982, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 2;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			});
			TimeAction.Add(922, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 4;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
				MapManager.Instance.mapList[1].GraveStoneNum += 4;
				MapManager.Instance.mapList[1].SpawnAllGraveStone();
			});
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum = 2;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
				MapManager.Instance.mapList[1].GraveStoneNum = 2;
				MapManager.Instance.mapList[1].GraveStoneLine = 7;
				MapManager.Instance.mapList[1].SpawnAllGraveStone();
			};
		}
	}

	private void LV10052()
	{
		if (IsEasy)
		{
			StartTime = 846;
		}
		else
		{
			StartTime = 1080;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 5),
			new FutureWeather(WeatherType.Hail, 3, 72),
			new FutureWeather(WeatherType.Hail, 6, 160),
			new FutureWeather(WeatherType.Hail, 10, 420),
			new FutureWeather(WeatherType.Rain, 3, 720)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 350;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 4, 6, 10, 18, 8, 20, 30 },
					new List<int> { 1, 1, 3, 8, 10, 14, 8, 18, 20 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.RepeaterZombie,
						ZombieType.RockCatapultZombie,
						ZombieType.MelonpultZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Gargantuar,
						ZombieType.DoomShroomZombie,
						ZombieType.BucketZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.RockCatapultZombie,
						ZombieType.PaperZombie,
						ZombieType.SnorkleZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 150;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 4, 8, 10, 24, 8, 20, 34 },
					new List<int> { 2, 3, 6, 8, 10, 18, 8, 21, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.RepeaterZombie,
						ZombieType.RockCatapultZombie,
						ZombieType.MelonpultZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Gargantuar,
						ZombieType.DoomShroomZombie,
						ZombieType.BucketZombie,
						ZombieType.LadderZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.RockCatapultZombie,
						ZombieType.PaperZombie,
						ZombieType.SnorkleZombie,
						ZombieType.IceShroomZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.Nope,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.Nope,
						PlantType.Umbrellaleaf
					},
					new List<PlantType>
					{
						PlantType.Nope,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.Nope,
						PlantType.Umbrellaleaf
					},
					new List<PlantType>
					{
						PlantType.Nope,
						PlantType.SunFlower
					}
				}
			};
		}
	}

	private void LV10053()
	{
		if (IsEasy)
		{
			StartTime = 600;
		}
		else
		{
			StartTime = 1080;
		}
		StartCardCd = false;
		WeightLimit = false;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Rain, 10, 280),
			new FutureWeather(WeatherType.Clear, 0, 710)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 2200;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 5, 10, 16, 15, 24, 20, 38, 44 },
					new List<int> { 0, 0, 0, 4, 8, 4, 6, 12 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.BucketZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Polevaulter,
						ZombieType.CatapultZombie,
						ZombieType.PaperZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 1800;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 5, 16, 16, 22, 28, 20, 46, 54 },
					new List<int> { 0, 0, 0, 4, 8, 4, 10, 18 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Polevaulter,
						ZombieType.CatapultZombie,
						ZombieType.PaperZombie
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV10054()
	{
		if (IsEasy)
		{
			StartTime = 852;
		}
		else
		{
			StartTime = 1080;
		}
		CardNum = 12;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Wind, 1, 0),
			new FutureWeather(WeatherType.Wind, 3, 30),
			new FutureWeather(WeatherType.Rain, 6, 60),
			new FutureWeather(WeatherType.Wind, 5, 100)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 500;
				SetupTime = new Vector2(3f, 5f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 6, 12, 6, 6, 16 },
					new List<int> { 1, 3, 8, 10, 6, 10, 20 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.JacksonZombie,
						ZombieType.ConeZombie,
						ZombieType.Zomboni,
						ZombieType.RepeaterZombie,
						ZombieType.SnowRepeaterZombie,
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
			}
			else
			{
				LvNormalSunNum = 450;
				SetupTime = new Vector2(3f, 5f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 6, 8, 15, 8, 10, 22 },
					new List<int> { 3, 6, 8, 15, 8, 15, 28 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.JacksonZombie,
						ZombieType.ConeZombie,
						ZombieType.Zomboni,
						ZombieType.RepeaterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.Gargantuar,
						ZombieType.PeaShooterZombie
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
			}
			BigWaveNum = new List<int> { 3, 6 };
		}
	}

	private void LV10055()
	{
		if (IsEasy)
		{
			StartTime = 823;
		}
		else
		{
			StartTime = 1080;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Rain, 6, 160),
			new FutureWeather(WeatherType.Rain, 10, 420),
			new FutureWeather(WeatherType.Rain, 5, 720)
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.LastStand);
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 9500;
				SetupTime = new Vector2(1f, 2f);
				Weights = new List<List<int>>
				{
					new List<int> { 35, 35, 45, 40, 70 },
					new List<int> { 30, 40, 50, 50, 55 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.JacksonZombie,
						ZombieType.DiggerZombie,
						ZombieType.BlackFootball,
						ZombieType.BucketZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Zomboni,
						ZombieType.RepeaterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.GargantuarHelmetRedeye
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DolphinriderZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.Zomboni,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.HeavyBungiZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 9000;
				SetupTime = new Vector2(1f, 2f);
				Weights = new List<List<int>>
				{
					new List<int> { 45, 44, 55, 50, 90 },
					new List<int> { 35, 40, 65, 60, 75 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.JacksonZombie,
						ZombieType.DiggerZombie,
						ZombieType.BlackFootball,
						ZombieType.BucketZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Zomboni,
						ZombieType.RepeaterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.GargantuarHelmetRedeye
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DolphinriderZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.SnorkleZombieHelmet,
						ZombieType.Zomboni,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.HeavyBungiZombie
					}
				};
			}
			BigWaveNum = new List<int> { 2, 4 };
		}
	}

	private void LV11001()
	{
		if (IsEasy)
		{
			StartTime = 480;
		}
		else
		{
			StartTime = 300;
		}
		LvName = "植物僵尸1";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Rain, 6, 360),
			new FutureWeather(WeatherType.Rain, 10, 1010)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 16, 12, 16, 26 },
					new List<int> { 1, 2, 4, 8, 14, 12, 18, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.WallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.WallNutZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(8f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 8, 10, 22, 14, 24, 35 },
					new List<int> { 2, 3, 6, 8, 22, 14, 28, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.WallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.WallNutZombie
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV11002()
	{
		LvNormalSunNum = 50;
		if (IsEasy)
		{
			StartTime = 550;
		}
		else
		{
			StartTime = 570;
		}
		CurrBankType = BankType.ConveryorBelt;
		LvName = "坚果保龄球";
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (OnlyInfo)
		{
			return;
		}
		LvSpStates.Add(LVSpState.NutBowling);
		dayBgm = BgmType.Loonboon;
		nightBgm = BgmType.Loonboon;
		BootySprite = BootySprite.Trophy;
		AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
		FirstBootyEvent = () =>
		{
			PlayerManager.Instance.Money += 1000;
		};
		NextWaveLossTime = -25;
		GeneralCardPool = new List<CardType>
		{
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.ExplodeNut)
		};
		if (IsEasy)
		{
			SetupTime = new Vector2(2f, 4f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					10, 12, 10, 12, 25, 25, 30, 35, 25, 20,
					45
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.DoorZombie
				}
			};
		}
		else
		{
			SetupTime = new Vector2(2f, 4f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					15, 15, 25, 25, 45, 35, 35, 45, 35, 35,
					55
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.Polevaulter,
					ZombieType.BucketZombie,
					ZombieType.DoorAndBucket,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.FootballZombie
				}
			};
		}
		BigWaveNum = new List<int> { 4, 7, 10 };
		MapOverAction = () =>
		{
			List<Grid> gridList = MapManager.Instance.mapList[0].GridList;
			for (int i = 0; i < gridList.Count; i++)
			{
				if (gridList[i].Point.x <= 2)
				{
					gridList[i].isOccupied = false;
				}
				else
				{
					gridList[i].isOccupied = true;
				}
			}
			MapManager.Instance.mapList[0].SetStripe(2);
		};
	}

	private void LV11003()
	{
		if (IsEasy)
		{
			StartTime = 495;
		}
		else
		{
			StartTime = 750;
		}
		CardNum = 1;
		LvName = "老虎机";
		CurrBankType = BankType.SlotMachine;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(PlantType.Cherry)
			};
			if (IsEasy)
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 14, 12, 12, 22 },
					new List<int> { 1, 2, 4, 8, 14, 10, 18, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie
					}
				};
				GeneralCardPool = new List<CardType>
				{
					new CardType(PlantType.Tanglekelp),
					new CardType(PlantType.ThreePeater),
					new CardType(PlantType.PeaShooter),
					new CardType(PlantType.ThreePeater),
					new CardType(PlantType.Torchwood),
					new CardType(PlantType.Cherry),
					new CardType(PlantType.SunFlower),
					new CardType(PlantType.SunFlower),
					new CardType(PlantType.SunFlower),
					new CardType(PlantType.WallNut),
					new CardType(PlantType.SnowPea),
					new CardType(PlantType.Cherry),
					new CardType(PlantType.Squash),
					new CardType(ZombieType.BucketZombie),
					new CardType(ZombieType.Polevaulter)
				};
			}
			else
			{
				LvNormalSunNum = 25;
				SetupTime = new Vector2(8f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 6, 10, 16, 12, 20, 25 },
					new List<int> { 1, 3, 6, 8, 18, 10, 18, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.PeaShooterZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.PaperZombie
					}
				};
				GeneralCardPool = new List<CardType>
				{
					new CardType(PlantType.Tanglekelp),
					new CardType(PlantType.ThreePeater),
					new CardType(PlantType.PeaShooter),
					new CardType(PlantType.ThreePeater),
					new CardType(PlantType.Torchwood),
					new CardType(PlantType.Cherry),
					new CardType(PlantType.SunFlower),
					new CardType(PlantType.SunFlower),
					new CardType(PlantType.SunFlower),
					new CardType(PlantType.WallNut),
					new CardType(PlantType.SnowPea),
					new CardType(PlantType.Squash),
					new CardType(PlantType.Cherry),
					new CardType(ZombieType.BucketZombie),
					new CardType(ZombieType.Zomboni)
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV11004()
	{
		LvName = "大睡天";
		StartTime = 500;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.SleepDay);
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 8, 10, 18, 15, 20, 30 },
					new List<int> { 0, 1, 3, 6, 8, 18, 15, 20, 28 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.FootballZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
			else
			{
				LvNormalSunNum = 200;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 8, 10, 24, 20, 28, 40 },
					new List<int> { 1, 2, 3, 6, 8, 22, 20, 25, 35 }
				};
				SetupTime = new Vector2(8f, 12f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni,
						ZombieType.LadderZombie,
						ZombieType.Gargantuar,
						ZombieType.SnowpeaShooterZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.FootballZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
		}
	}

	private void LV11005()
	{
		if (IsEasy)
		{
			StartTime = 1140;
		}
		else
		{
			StartTime = 1140;
		}
		LvName = "雨中植物";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 4, 0),
			new FutureWeather(WeatherType.Rain, 7, 260),
			new FutureWeather(WeatherType.Rain, 10, 1010)
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.RainPlant);
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(PlantType.Cherry)
			};
			GeneralCardPool = new List<CardType>
			{
				new CardType(PlantType.Tanglekelp),
				new CardType(PlantType.ThreePeater),
				new CardType(PlantType.PeaShooter),
				new CardType(PlantType.SplitPea),
				new CardType(PlantType.Torchwood),
				new CardType(PlantType.Cactus),
				new CardType(PlantType.Melonpult),
				new CardType(PlantType.Marigold),
				new CardType(PlantType.WallNut),
				new CardType(PlantType.SnowPea),
				new CardType(PlantType.Cherry),
				new CardType(PlantType.Lilypad),
				new CardType(PlantType.Lilypad),
				new CardType(PlantType.Lilypad),
				new CardType(PlantType.Cabbagepult),
				new CardType(PlantType.Cornpult),
				new CardType(PlantType.Jalapeno),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.Pumpkin),
				new CardType(ZombieType.ConeZombie)
			};
			if (IsEasy)
			{
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 14, 12, 18, 25 },
					new List<int> { 1, 2, 4, 8, 14, 12, 18, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter,
						ZombieType.BalloonZombie,
						ZombieType.LadderZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie,
						ZombieType.DiggerZombie,
						ZombieType.SnorkleZombie
					}
				};
			}
			else
			{
				GeneralCardPool.Add(new CardType(ZombieType.Polevaulter));
				SetupTime = new Vector2(8f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 22, 14, 28, 35 },
					new List<int> { 2, 3, 6, 8, 22, 14, 28, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.PeaShooterZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorZombie
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV11006()
	{
		if (IsEasy)
		{
			StartTime = 1100;
		}
		else
		{
			StartTime = 1100;
		}
		LvName = "隐形食脑者";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		LvSpStates.Add(LVSpState.InvisibleZombie);
		dayBgm = BgmType.WateryGraves;
		BootySprite = BootySprite.Trophy;
		AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
		FirstBootyEvent = () =>
		{
			PlayerManager.Instance.Money += 1000;
		};
		if (IsEasy)
		{
			LvNormalSunNum = 350;
			SetupTime = new Vector2(25f, 30f);
			BigWaveNum = new List<int> { 5, 8 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 2, 4, 6, 18, 12, 18, 22 },
				new List<int> { 0, 1, 2, 4, 4, 15, 10, 18, 22 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.DoorZombie,
					ZombieType.PaperZombie,
					ZombieType.Polevaulter
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.TubeZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.ConeZombie,
					ZombieType.Zomboni,
					ZombieType.PaperZombie
				}
			};
			return;
		}
		LvNormalSunNum = 200;
		SetupTime = new Vector2(15f, 20f);
		BigWaveNum = new List<int> { 5, 8 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 1, 2, 4, 6, 18, 14, 18, 35 },
			new List<int> { 1, 2, 2, 4, 10, 15, 18, 28, 32 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.Zomboni,
				ZombieType.Polevaulter,
				ZombieType.BlackFootball,
				ZombieType.PaperZombie,
				ZombieType.Gargantuar
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.BucketZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.RepeaterZombie
			}
		};
		MapOverAction = () =>
		{
			MapManager.Instance.mapList[0].fog.CreateFog(2);
			MapManager.Instance.mapList[1].fog.CreateFog(3);
		};
		StartAction = () =>
		{
			MapManager.Instance.mapList[0].fog.MoveIn();
			MapManager.Instance.mapList[1].fog.MoveIn();
			MapManager.Instance.mapList[0].GraveStoneNum += 7;
			MapManager.Instance.mapList[0].GraveStoneLine = 6;
			MapManager.Instance.mapList[0].SpawnAllGraveStone();
		};
		TimeAction.Add(381, () =>
		{
			MapManager.Instance.mapList[0].fog.CloseFog();
			MapManager.Instance.mapList[1].fog.CloseFog();
		});
	}

	private void LV11007()
	{
		if (IsEasy)
		{
			StartTime = 540;
		}
		else
		{
			StartTime = 360;
		}
		LvName = "小僵尸大麻烦";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 5, 360),
			new FutureWeather(WeatherType.Rain, 10, 510)
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.SmallZombie);
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 10, 10, 12, 20, 16, 20, 30 },
					new List<int> { 1, 2, 4, 10, 10, 14, 18, 20, 20, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Zomboni,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndCone,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(8f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 14, 12, 14, 28, 25, 30, 45 },
					new List<int> { 2, 3, 6, 14, 22, 14, 28, 22, 35, 40 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Zomboni,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.Gargantuar,
						ZombieType.TallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.CatapultZombie,
						ZombieType.GargantuarHelmetRedeye
					}
				};
			}
			BigWaveNum = new List<int> { 3, 6, 9 };
		}
	}

	private void LV11008()
	{
		if (IsEasy)
		{
			StartTime = 1140;
		}
		else
		{
			StartTime = 1040;
		}
		LvName = "传送门";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.WateryGraves;
		BootySprite = BootySprite.Trophy;
		AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
		FirstBootyEvent = () =>
		{
			PlayerManager.Instance.Money += 1000;
		};
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(18f, 22f);
			BigWaveNum = new List<int> { 5, 9 };
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 2, 2, 10, 12, 12, 14, 18, 22 },
				new List<int> { 0, 2, 2, 4, 9, 10, 12, 18, 22, 30 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorZombie,
					ZombieType.Polevaulter,
					ZombieType.BucketZombie,
					ZombieType.DiggerZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.CatapultZombie,
					ZombieType.DoorZombie
				}
			};
			StartAction = () =>
			{
				MapManager.Instance.CreatePortal(2, 0);
				MapManager.Instance.CreatePortal(2, 2);
			};
			return;
		}
		LvNormalSunNum = 75;
		SetupTime = new Vector2(15f, 20f);
		BigWaveNum = new List<int> { 5, 9 };
		Weights = new List<List<int>>
		{
			new List<int> { 1, 1, 4, 6, 8, 18, 18, 20, 25, 35 },
			new List<int> { 1, 2, 2, 4, 8, 25, 18, 22, 24, 38 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.BucketZombie,
				ZombieType.DoorZombie,
				ZombieType.Polevaulter,
				ZombieType.FootballZombie,
				ZombieType.DiggerZombie,
				ZombieType.PaperZombie,
				ZombieType.SnowpeaShooterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.ConeZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie,
				ZombieType.RepeaterZombie
			}
		};
		StartAction = () =>
		{
			MapManager.Instance.CreatePortal(2, 0);
			MapManager.Instance.CreatePortal(3, 1);
			MapManager.Instance.CreatePortal(3, 2);
		};
	}

	private void LV11009()
	{
		if (IsEasy)
		{
			StartTime = 480;
		}
		else
		{
			StartTime = 300;
		}
		LvName = "排山倒海";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			CurrBankType = BankType.ConveryorBelt;
			LvSpStates.Add(LVSpState.PlantLikeColumn);
			dayBgm = BgmType.UltimateBattle;
			nightBgm = BgmType.UltimateBattle;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			GeneralCardPool = new List<CardType>
			{
				new CardType(PlantType.Melonpult),
				new CardType(PlantType.Pot),
				new CardType(PlantType.Pot),
				new CardType(PlantType.Tallnut),
				new CardType(PlantType.Torchwood),
				new CardType(PlantType.Pumpkin),
				new CardType(PlantType.PotatoMine),
				new CardType(PlantType.Repeater),
				new CardType(PlantType.Jalapeno),
				new CardType(PlantType.Squash)
			};
			NextWaveLossTime = -40;
			BeltTimeAdd = 4;
			if (IsEasy)
			{
				SetupTime = new Vector2(20f, 25f);
				Weights = new List<List<int>>
				{
					new List<int> { 20, 40, 40, 50, 60, 70 },
					new List<int> { 30, 40, 40, 50, 50, 70 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndCone,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie,
						ZombieType.Gargantuar,
						ZombieType.JackboxZombie,
						ZombieType.LadderZombie
					}
				};
			}
			else
			{
				SetupTime = new Vector2(20f, 25f);
				Weights = new List<List<int>>
				{
					new List<int> { 30, 40, 50, 70, 80, 100 },
					new List<int> { 30, 40, 40, 50, 80, 100 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndCone,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.PaperZombie,
						ZombieType.FootballZombie,
						ZombieType.Gargantuar,
						ZombieType.JackboxZombie,
						ZombieType.LadderZombie
					}
				};
			}
			BigWaveNum = new List<int> { 3, 5 };
		}
	}

	private void LV11010()
	{
		if (IsEasy)
		{
			StartTime = 730;
		}
		else
		{
			StartTime = 1080;
		}
		LvName = "变变变";
		CardNum = 14;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 1, 0),
			new FutureWeather(WeatherType.Rain, 6, 200),
			new FutureWeather(WeatherType.Clear, 0, 710)
		};
		if (!OnlyInfo)
		{
			BanMultyCardAdd = true;
			FixedCard = new List<CardType>
			{
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater),
				new CardType(PlantType.RottenImitater)
			};
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(15f, 18f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 3, 6, 8, 16, 10, 18, 24 },
					new List<int> { 0, 1, 2, 6, 10, 14, 10, 14, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BungiZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.TubeDoorConeZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie,
						ZombieType.LadderZombie,
						ZombieType.BungiZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 5, 6, 14, 22, 10, 24, 32 },
					new List<int> { 1, 2, 4, 6, 14, 24, 10, 14, 34 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BungiZombie,
						ZombieType.FootballZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.TubeDoorConeZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie,
						ZombieType.LadderZombie,
						ZombieType.BungiZombie,
						ZombieType.PeaShooterZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LV11011()
	{
		if (IsEasy)
		{
			StartTime = 960;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 2, 0),
				new FutureWeather(WeatherType.Snow, 5, 258),
				new FutureWeather(WeatherType.Snow, 8, 448)
			};
		}
		else
		{
			StartTime = 1080;
			LoadWeathers = new List<FutureWeather>
			{
				new FutureWeather(WeatherType.Snow, 2, 0),
				new FutureWeather(WeatherType.Snow, 6, 198),
				new FutureWeather(WeatherType.Snow, 10, 388)
			};
		}
		LvName = "雪橇区";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.WateryGraves;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 750;
				SetupTime = new Vector2(20f, 25f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 8, 12, 18, 12, 20, 28 },
					new List<int> { 1, 2, 3, 8, 10, 18, 16, 18, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.BobsledZombie,
						ZombieType.BobsledZombie,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.Zomboni
					},
					new List<ZombieType>
					{
						ZombieType.BobsledZombie,
						ZombieType.BobsledZombieSled,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.BobsledZombieHelmetSled
					}
				};
			}
			else
			{
				LvNormalSunNum = 550;
				SetupTime = new Vector2(15f, 20f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 8, 8, 12, 24, 18, 28, 34 },
					new List<int> { 2, 3, 3, 8, 10, 24, 22, 28, 42 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.BobsledZombie,
						ZombieType.BobsledZombie,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.Zomboni
					},
					new List<ZombieType>
					{
						ZombieType.BobsledZombie,
						ZombieType.BobsledZombieSled,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.BobsledZombieHelmetSled
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
			BigWaveAction = () =>
			{
				ZombieManager.Instance.UpdateZombieOnRandomLine(ZombieType.Yeti, MapManager.Instance.mapList[Random.Range(0, 2)].transform.position);
			};
		}
	}

	private void LV11012()
	{
		if (IsEasy)
		{
			StartTime = 1145;
		}
		else
		{
			StartTime = 930;
		}
		LvName = "僵尸大战僵尸";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.GrazeTheRoof;
		nightBgm = BgmType.MoonGrains;
		BootySprite = BootySprite.Trophy;
		AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
		FirstBootyEvent = () =>
		{
			PlayerManager.Instance.Money += 1000;
		};
		CurrSeedBankType = SeedBankType.MoonBank;
		WaterZombieNum = 4;
		WaterZombie = new List<ZombieType>
		{
			ZombieType.TubeZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeBucketZombie
		};
		if (IsEasy)
		{
			LvNormalSunNum = 400;
			SetupTime = new Vector2(22f, 25f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 2, 6, 4, 12, 18, 10, 18, 22 },
				new List<int> { 0, 1, 2, 8, 4, 10, 16, 8, 12, 22 },
				new List<int> { 0, 1, 2, 6, 10, 12, 14, 10, 15, 20 }
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 3;
				MapManager.Instance.mapList[0].GraveStoneLine = 6;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
			};
		}
		else
		{
			LvNormalSunNum = 350;
			SetupTime = new Vector2(18f, 22f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 1, 3, 8, 8, 12, 22, 20, 25, 34 },
				new List<int> { 0, 1, 2, 8, 10, 12, 18, 8, 15, 30 },
				new List<int> { 1, 2, 3, 8, 14, 14, 18, 15, 20, 30 }
			};
			StartAction = () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum += 5;
				MapManager.Instance.mapList[0].GraveStoneLine = 8;
				MapManager.Instance.mapList[0].SpawnAllGraveStone();
				MapManager.Instance.mapList[1].GraveStoneNum += 5;
				MapManager.Instance.mapList[1].GraveStoneLine = 8;
				MapManager.Instance.mapList[1].SpawnAllGraveStone();
			};
		}
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.PaperZombie,
				ZombieType.Polevaulter,
				ZombieType.Zomboni,
				ZombieType.ConeZombie,
				ZombieType.GargantuarInjured,
				ZombieType.JacksonZombie,
				ZombieType.RepeaterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.BucketZombie,
				ZombieType.DiggerZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.BucketZombie,
				ZombieType.RepeaterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.PaperZombie,
				ZombieType.DoorAndCone,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.CatapultZombie
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
	}

	private void LV11013()
	{
		LvName = "僵尸快跑";
		StartTime = 420;
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.QuickZombie);
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 200;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 4, 8, 10, 12, 10, 18, 24 },
					new List<int> { 0, 1, 3, 6, 8, 12, 10, 15, 20 }
				};
				SetupTime = new Vector2(20f, 25f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.SnorkleZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.CatapultZombie,
						ZombieType.LadderZombie
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
			else
			{
				LvNormalSunNum = 150;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 10, 18, 14, 18, 24 },
					new List<int> { 1, 1, 3, 6, 8, 20, 15, 22, 30 }
				};
				SetupTime = new Vector2(15f, 20f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.SquashZombie,
						ZombieType.BlackFootball,
						ZombieType.SnorkleZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.LadderZombie,
						ZombieType.SquashZombie
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
		}
	}

	private void LV11014()
	{
		if (IsEasy)
		{
			StartTime = 980;
		}
		else
		{
			StartTime = 1200;
		}
		LvName = "植物僵尸2";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(14f, 18f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 18, 16, 20, 25 },
					new List<int> { 1, 1, 4, 8, 10, 18, 12, 18, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.GatlingZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.JalapenoZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(10f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 8, 10, 14, 22, 16, 24, 35 },
					new List<int> { 2, 3, 6, 8, 14, 22, 16, 28, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.GatlingZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.RepeaterZombie,
						ZombieType.GatlingZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.SquashZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.JalapenoZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LV11015()
	{
		LvNormalSunNum = 50;
		if (IsEasy)
		{
			StartTime = 550;
		}
		else
		{
			StartTime = 570;
		}
		CurrBankType = BankType.ConveryorBelt;
		LvSpStates.Add(LVSpState.NutBowling);
		LvName = "坚果保龄球2";
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (OnlyInfo)
		{
			return;
		}
		dayBgm = BgmType.Loonboon;
		nightBgm = BgmType.Loonboon;
		BootySprite = BootySprite.Trophy;
		AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
		NextWaveLossTime = -35;
		GeneralCardPool = new List<CardType>
		{
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.WallNut),
			new CardType(PlantType.ExplodeNut),
			new CardType(PlantType.ExplodeNut),
			new CardType(PlantType.ExplodeNut),
			new CardType(PlantType.ExplodeNut),
			new CardType(PlantType.HugeNut)
		};
		if (IsEasy)
		{
			SetupTime = new Vector2(2f, 4f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					8, 10, 15, 20, 22, 25, 25, 35, 30, 35,
					45
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.BucketZombie,
					ZombieType.Polevaulter,
					ZombieType.DoorAndCone,
					ZombieType.PaperZombie,
					ZombieType.DoorZombie,
					ZombieType.JacksonZombie,
					ZombieType.BlackFootball,
					ZombieType.Gargantuar
				}
			};
		}
		else
		{
			SetupTime = new Vector2(2f, 4f);
			Weights = new List<List<int>>
			{
				new List<int>
				{
					8, 15, 20, 25, 30, 35, 45, 45, 35, 45,
					60
				}
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.Polevaulter,
					ZombieType.BucketZombie,
					ZombieType.DoorAndBucket,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.FootballZombie,
					ZombieType.BlackFootball,
					ZombieType.JacksonZombie,
					ZombieType.Gargantuar
				}
			};
		}
		BigWaveNum = new List<int> { 4, 7, 10 };
		MapOverAction = () =>
		{
			List<Grid> gridList = MapManager.Instance.mapList[0].GridList;
			for (int i = 0; i < gridList.Count; i++)
			{
				if (gridList[i].Point.x <= 2)
				{
					gridList[i].isOccupied = false;
				}
				else
				{
					gridList[i].isOccupied = true;
				}
			}
			MapManager.Instance.mapList[0].SetStripe(2);
		};
	}

	private void LV11016()
	{
		LvName = "向后看";
		StartTime = 920;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.PlantReverse);
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 200;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 10, 22, 18, 24, 34 },
					new List<int> { 1, 2, 3, 6, 8, 18, 16, 20, 34 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.DiggerZombie,
						ZombieType.PaperZombie,
						ZombieType.HyponShroomZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 4, 8, 18, 28, 28, 34, 44 },
					new List<int> { 2, 2, 6, 8, 18, 28, 22, 30, 44 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.DiggerZombie,
						ZombieType.PaperZombie,
						ZombieType.HyponShroomZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.SnorkleZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.Gargantuar
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
		}
	}

	private void LV11017()
	{
		LvName = "巨型狂潮";
		StartTime = 1200;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			WeightLimit = false;
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			BigWaveFixedZombie = new List<ZombieType> { ZombieType.FlagZombie };
			NextWaveLossTime = 15;
			if (IsEasy)
			{
				LvNormalSunNum = 1000;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 0, 1, 14, 2, 18, 18, 25, 38 },
					new List<int> { 0, 1, 2, 2, 14, 14, 18, 25, 30 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.GargantuarInjured,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet
					},
					new List<ZombieType>
					{
						ZombieType.GargantuarInjured,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
			else
			{
				LvNormalSunNum = 600;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 1, 14, 2, 28, 24, 35, 58 },
					new List<int> { 1, 1, 2, 2, 14, 28, 24, 45, 54 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.GargantuarInjured,
						ZombieType.SwampGargantuarBlueEye,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet
					},
					new List<ZombieType>
					{
						ZombieType.GargantuarInjured,
						ZombieType.SwampGargantuar,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
		}
	}

	private void LV11018()
	{
		StartTime = 1148;
		LvName = "超混沌传送门";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		if (OnlyInfo)
		{
			return;
		}
		BootySprite = BootySprite.Trophy;
		AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
		FirstBootyEvent = () =>
		{
			PlayerManager.Instance.Money += 1000;
		};
		dayBgm = BgmType.WateryGraves;
		nightBgm = BgmType.RigorMormist;
		if (IsEasy)
		{
			LvNormalSunNum = 150;
			SetupTime = new Vector2(20f, 22f);
			Weights = new List<List<int>>
			{
				new List<int> { 1, 0, 3, 8, 8, 12, 26, 18, 25, 35 },
				new List<int> { 0, 1, 3, 8, 6, 12, 20, 20, 28, 32 },
				new List<int> { 0, 1, 2, 8, 10, 20, 25, 15, 25, 28 }
			};
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.GargantuarInjured
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.Gargantuar,
					ZombieType.SnorkleZombie,
					ZombieType.FootballZombie
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.CatapultZombie,
					ZombieType.Gargantuar,
					ZombieType.LadderZombie
				}
			};
			BigWaveNum = new List<int> { 3, 6, 9 };
			StartAction = () =>
			{
				MapManager.Instance.CreatePortal(3, 0);
				MapManager.Instance.CreatePortal(2, 1);
				MapManager.Instance.CreatePortal(2, 2);
			};
			return;
		}
		LvNormalSunNum = 100;
		SetupTime = new Vector2(15f, 18f);
		Weights = new List<List<int>>
		{
			new List<int> { 1, 2, 3, 8, 8, 12, 22, 18, 35, 45 },
			new List<int> { 1, 1, 3, 8, 6, 12, 20, 25, 28, 38 },
			new List<int> { 1, 2, 3, 12, 10, 20, 25, 15, 30, 48 }
		};
		ZombieTypes = new List<List<ZombieType>>
		{
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.PaperZombie,
				ZombieType.ConeZombie,
				ZombieType.GargantuarInjured,
				ZombieType.BlackFootball,
				ZombieType.CatapultZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.ConeZombie,
				ZombieType.DoorAndCone,
				ZombieType.Gargantuar,
				ZombieType.FootballZombie,
				ZombieType.SnorkleZombie,
				ZombieType.GargantuarHelmet,
				ZombieType.PeaShooterZombie
			},
			new List<ZombieType>
			{
				ZombieType.NormalZombie,
				ZombieType.Polevaulter,
				ZombieType.JackboxZombie,
				ZombieType.CatapultZombie,
				ZombieType.Gargantuar,
				ZombieType.BlackFootball,
				ZombieType.LadderZombie
			}
		};
		BigWaveNum = new List<int> { 3, 6, 9 };
		StartAction = () =>
		{
			MapManager.Instance.CreatePortal(3, 0);
			MapManager.Instance.CreatePortal(4, 1);
			MapManager.Instance.CreatePortal(2, 2);
		};
	}

	private void LV11019()
	{
		if (IsEasy)
		{
			StartTime = 1260;
		}
		else
		{
			StartTime = 1100;
		}
		LvName = "植物僵尸3";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Rain, 6, 360)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 8, 10, 14, 18, 12, 20, 28 },
					new List<int> { 1, 3, 6, 8, 10, 18, 12, 18, 27 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.GloomShroomZombie,
						ZombieType.FumeShroomZombie,
						ZombieType.JalapenoZombie,
						ZombieType.SquashZombie,
						ZombieType.WallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.IceShroomZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.FumeShroomZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(15f, 18f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 8, 10, 14, 24, 18, 20, 38 },
					new List<int> { 2, 4, 8, 12, 16, 24, 18, 20, 37 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.IceShroomZombie,
						ZombieType.GloomShroomZombie,
						ZombieType.FumeShroomZombie,
						ZombieType.JalapenoZombie,
						ZombieType.SquashZombie,
						ZombieType.WallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.IceShroomZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.GloomShroomZombie,
						ZombieType.FumeShroomZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LV11020()
	{
		LvName = "免费日";
		StartTime = 420;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 100),
			new FutureWeather(WeatherType.Rain, 6, 360),
			new FutureWeather(WeatherType.Thunder, 10, 610)
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.FreeDay);
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 4, 8, 12, 28, 20, 28, 40, 28, 34, 64 },
					new List<int> { 2, 4, 8, 20, 18, 24, 38, 30, 40, 60 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.BlackFootball,
						ZombieType.GargantuarRedeye
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar,
						ZombieType.DolphinriderZombie,
						ZombieType.GatlingZombie
					}
				};
				BigWaveNum = new List<int> { 3, 6, 9 };
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int> { 8, 8, 24, 38, 30, 48, 58, 48, 64, 84 },
					new List<int> { 4, 8, 22, 34, 25, 34, 58, 48, 60, 78 }
				};
				SetupTime = new Vector2(8f, 12f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DiggerZombie,
						ZombieType.BucketZombie,
						ZombieType.CatapultZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.BlackFootball,
						ZombieType.SnowRepeaterZombie,
						ZombieType.SwampGargantuarBlueEye,
						ZombieType.GargantuarHelmet,
						ZombieType.GatlingZombie,
						ZombieType.UmbrellaZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.TubeConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.Polevaulter,
						ZombieType.DiggerZombie,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.SnorkleZombie,
						ZombieType.Polevaulter,
						ZombieType.DolphinriderZombie,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.GatlingZombie,
						ZombieType.UmbrellaZombie
					}
				};
				BigWaveNum = new List<int> { 3, 6, 9 };
			}
		}
	}

	private void LV11021()
	{
		if (IsEasy)
		{
			StartTime = 762;
		}
		else
		{
			StartTime = 739;
		}
		LvName = "跳跳舞会";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				SetupTime = new Vector2(16f, 20f);
				BigWaveNum = new List<int> { 5, 8, 11 };
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 4, 6, 8, 14, 12, 16, 24, 18,
						24, 30
					},
					new List<int>
					{
						0, 2, 4, 6, 8, 12, 10, 16, 22, 18,
						24, 33
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.PogoZombie,
						ZombieType.JacksonZombie,
						ZombieType.Polevaulter,
						ZombieType.DolphinriderZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.PogoZombie,
						ZombieType.PogoZombie,
						ZombieType.Polevaulter,
						ZombieType.BungiZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(10f, 15f);
				BigWaveNum = new List<int> { 5, 8, 11 };
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 4, 6, 8, 22, 18, 26, 30, 22,
						28, 35
					},
					new List<int>
					{
						2, 2, 4, 6, 8, 22, 18, 20, 24, 20,
						28, 40
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.Polevaulter,
						ZombieType.PogoZombie,
						ZombieType.JacksonZombie,
						ZombieType.DolphinriderZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PogoZombie,
						ZombieType.BungiZombie,
						ZombieType.PogoZombie
					}
				};
			}
		}
	}

	private void LV11022()
	{
		if (IsEasy)
		{
			StartTime = 1145;
		}
		else
		{
			StartTime = 930;
		}
		LvName = "僵尸大战植物";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.MoonGrains;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			CurrSeedBankType = SeedBankType.MoonBank;
			if (IsEasy)
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(22f, 25f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 3, 12, 8, 12, 18, 15, 18, 22 },
					new List<int> { 0, 1, 4, 8, 10, 12, 16, 8, 12, 18 },
					new List<int> { 0, 1, 3, 8, 10, 18, 18, 15, 20, 20 }
				};
			}
			else
			{
				LvNormalSunNum = 100;
				SetupTime = new Vector2(18f, 22f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 4, 8, 12, 22, 20, 25, 40 },
					new List<int> { 0, 1, 4, 8, 10, 12, 16, 8, 12, 28 },
					new List<int> { 1, 2, 4, 10, 10, 18, 18, 15, 20, 30 }
				};
			}
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.PeaShooterZombie,
					ZombieType.SunflowerZombie,
					ZombieType.WallNutZombie,
					ZombieType.RepeaterZombie,
					ZombieType.TorchwoodZombie,
					ZombieType.GatlingZombie,
					ZombieType.JalapenoZombie,
					ZombieType.DoomShroomZombie
				},
				new List<ZombieType>
				{
					ZombieType.PeaShooterZombie,
					ZombieType.SunflowerZombie,
					ZombieType.TallNutZombie,
					ZombieType.IceShroomZombie,
					ZombieType.SnowpeaShooterZombie,
					ZombieType.SnowRepeaterZombie,
					ZombieType.WinterMelonZombie
				},
				new List<ZombieType>
				{
					ZombieType.PeaShooterZombie,
					ZombieType.SunflowerZombie,
					ZombieType.TallNutZombie,
					ZombieType.CabbagepultZombie,
					ZombieType.JalapenoZombie,
					ZombieType.GloomShroomZombie,
					ZombieType.MelonpultZombie
				}
			};
			BigWaveNum = new List<int> { 3, 6, 9 };
		}
	}

	private void LV11023()
	{
		LvName = "炸弹工厂";
		StartTime = 500;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			CardNum = 0;
			StartCardCd = false;
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BanMultyCardAdd = true;
			WeightLimit = false;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(PlantType.SunFlower),
				new CardType(PlantType.Pot),
				new CardType(PlantType.Coffeebean),
				new CardType(PlantType.WallNut),
				new CardType(PlantType.Cherry),
				new CardType(PlantType.Cherry),
				new CardType(PlantType.PotatoMine),
				new CardType(PlantType.PotatoMine),
				new CardType(PlantType.Jalapeno),
				new CardType(PlantType.Jalapeno),
				new CardType(PlantType.Squash),
				new CardType(PlantType.IceShroom),
				new CardType(PlantType.DoomShroom)
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 10, 16, 12, 18, 28 },
					new List<int> { 1, 2, 6, 6, 12, 18, 10, 20, 28 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.BucketZombie,
						ZombieType.BlackFootball,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.Gargantuar,
						ZombieType.PaperZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.GargantuarHelmet
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
			else
			{
				LvNormalSunNum = 200;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 8, 10, 12, 22, 18, 26, 44 },
					new List<int> { 1, 2, 6, 12, 12, 28, 18, 30, 38 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar,
						ZombieType.BucketZombie,
						ZombieType.BlackFootball,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.DiggerZombie,
						ZombieType.SwampGargantuarBlueEye
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.GargantuarHelmet,
						ZombieType.Gargantuar
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
		}
	}

	private void LV11024()
	{
		StartTime = 771;
		LvName = "排山倒海2";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			LvSpStates.Add(LVSpState.PlantLikeColumn);
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			WeightLimit = false;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 500;
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 8, 18, 12, 24, 28, 28, 44 },
					new List<int> { 2, 4, 6, 8, 12, 28, 28, 38, 48 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorZombie,
						ZombieType.BucketZombie,
						ZombieType.BlackFootball,
						ZombieType.JacksonZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.PaperZombie,
						ZombieType.JackboxZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.JackboxZombie,
						ZombieType.BlackFootball,
						ZombieType.Gargantuar,
						ZombieType.PaperZombie,
						ZombieType.CabbagepultZombie
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
			else
			{
				LvNormalSunNum = 300;
				Weights = new List<List<int>>
				{
					new List<int> { 4, 4, 8, 18, 28, 34, 28, 44, 64 },
					new List<int> { 4, 8, 8, 12, 18, 48, 36, 48, 64 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar,
						ZombieType.BucketZombie,
						ZombieType.BlackFootball,
						ZombieType.JacksonZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.JackboxZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.MelonpultZombie,
						ZombieType.PaperZombie,
						ZombieType.CabbagepultZombie
					}
				};
				BigWaveNum = new List<int> { 5, 8 };
			}
		}
	}

	private void LV11025()
	{
		if (IsEasy)
		{
			StartTime = 1240;
		}
		else
		{
			StartTime = 1360;
		}
		LvName = "小僵尸大麻烦2";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Rain, 10, 210)
		};
		if (!OnlyInfo)
		{
			WeightLimit = false;
			LvSpStates.Add(LVSpState.SmallZombie);
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 10, 10, 12, 20, 16, 20, 30 },
					new List<int> { 1, 2, 4, 10, 10, 14, 18, 20, 20, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.BucketZombie,
						ZombieType.Zomboni,
						ZombieType.SnorkleZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.GargantuarRedeye
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.BucketZombie,
						ZombieType.BlackFootball,
						ZombieType.DoorAndCone,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.TallNutZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(8f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 14, 12, 14, 28, 25, 30, 45 },
					new List<int> { 2, 3, 6, 14, 22, 14, 28, 22, 35, 48 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.BucketZombie,
						ZombieType.Zomboni,
						ZombieType.SnorkleZombie,
						ZombieType.Polevaulter,
						ZombieType.BlackFootball,
						ZombieType.SwampGargantuarBlueEye,
						ZombieType.TallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.BucketZombie,
						ZombieType.BlackFootball,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmetRedeye
					}
				};
			}
			BigWaveNum = new List<int> { 3, 6, 9 };
		}
	}

	private void LV11026()
	{
		if (IsEasy)
		{
			StartTime = 666;
		}
		else
		{
			StartTime = 777;
		}
		LvName = "植物僵尸4";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 500;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 14, 18, 16, 28, 30 },
					new List<int> { 1, 1, 3, 8, 14, 18, 16, 24, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.StarfruitZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.FumeShroomZombie,
						ZombieType.TallNutZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.CabbagepultZombie,
						ZombieType.MelonpultZombie,
						ZombieType.WinterMelonZombie,
						ZombieType.UmbrellaZombie,
						ZombieType.JalapenoZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SunflowerZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 4, 6, 14, 26, 18, 28, 35 },
					new List<int> { 2, 2, 4, 8, 14, 24, 22, 28, 38 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.StarfruitZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.FumeShroomZombie,
						ZombieType.GloomShroomZombie,
						ZombieType.TallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.CabbagepultZombie,
						ZombieType.MelonpultZombie,
						ZombieType.WinterMelonZombie,
						ZombieType.UmbrellaZombie,
						ZombieType.JalapenoZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SunflowerZombie,
						ZombieType.SunflowerZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LV11027()
	{
		if (IsEasy)
		{
			StartTime = 600;
		}
		else
		{
			StartTime = 600;
		}
		LvName = "极致炎热";
		LoadMapTypes = new List<MapType>
		{
			MapType.BackYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			LvTemperature = 25;
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 500;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 6, 8, 12, 22, 20, 36, 44 },
					new List<int> { 1, 2, 6, 12, 18, 20, 12, 24, 42 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.NormalZombie,
						ZombieType.TubeDoorConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar,
						ZombieType.RepeaterZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.JalapenoZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndBucket,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.PaperZombie,
						ZombieType.LadderZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.GatlingZombie,
						ZombieType.JalapenoZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 500;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 12, 22, 20, 36, 44 },
					new List<int> { 2, 2, 6, 12, 18, 28, 18, 34, 42 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.TubeDoorConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar,
						ZombieType.TorchwoodZombie,
						ZombieType.GatlingZombie,
						ZombieType.JalapenoZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndBucket,
						ZombieType.Polevaulter,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.PaperZombie,
						ZombieType.LadderZombie,
						ZombieType.RepeaterZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.GatlingZombie,
						ZombieType.JalapenoZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LV11028()
	{
		if (IsEasy)
		{
			StartTime = 711;
		}
		else
		{
			StartTime = 821;
		}
		LvName = "大混战";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 100),
			new FutureWeather(WeatherType.Rain, 8, 500)
		};
		if (!OnlyInfo)
		{
			CurrSeedBankType = SeedBankType.SunAndMoonBank;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 800;
				SetupTime = new Vector2(25f, 30f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 3, 10, 6, 8, 12, 16, 20, 30,
						20, 23, 40, 25, 35, 55
					},
					new List<int>
					{
						1, 2, 3, 8, 10, 16, 30, 15, 18, 28,
						30, 33, 40, 30, 30, 55
					},
					new List<int>
					{
						1, 2, 5, 8, 10, 18, 10, 15, 15, 20,
						20, 23, 40, 30, 30, 55
					}
				};
			}
			else
			{
				LvNormalSunNum = 600;
				SetupTime = new Vector2(18f, 22f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 16, 12, 12, 22, 26, 30, 40,
						30, 43, 50, 50, 60, 80
					},
					new List<int>
					{
						2, 5, 8, 12, 10, 20, 30, 15, 25, 28,
						30, 33, 60, 50, 60, 75
					},
					new List<int>
					{
						1, 3, 6, 8, 10, 18, 18, 15, 20, 30,
						30, 33, 60, 50, 60, 75
					}
				};
			}
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.Zomboni,
					ZombieType.BlackFootball,
					ZombieType.ConeZombie,
					ZombieType.RepeaterZombie,
					ZombieType.Gargantuar,
					ZombieType.GargantuarHelmet,
					ZombieType.BungiZombie,
					ZombieType.PogoZombie,
					ZombieType.DiggerZombie,
					ZombieType.GargantuarHelmetRedeye
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.DiggerZombie,
					ZombieType.Polevaulter,
					ZombieType.PaperZombie,
					ZombieType.ConeZombie,
					ZombieType.DoorAndCone,
					ZombieType.JackboxZombie,
					ZombieType.Gargantuar,
					ZombieType.CatapultZombie,
					ZombieType.Zomboni,
					ZombieType.GargantuarRedeye,
					ZombieType.GargantuarHelmet
				},
				new List<ZombieType>
				{
					ZombieType.NormalZombie,
					ZombieType.Polevaulter,
					ZombieType.JackboxZombie,
					ZombieType.FootballZombie,
					ZombieType.RepeaterZombie,
					ZombieType.CatapultZombie,
					ZombieType.Gargantuar,
					ZombieType.GargantuarHelmet,
					ZombieType.MelonpultZombie,
					ZombieType.GargantuarHelmetRedeye,
					ZombieType.UmbrellaZombie,
					ZombieType.CabbagepultZombie
				}
			};
			BigWaveNum = new List<int> { 3, 6, 9, 12, 15 };
			TimeAction.Add(1100, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum = 6;
				MapManager.Instance.mapList[0].GraveStoneLine = 4;
				MapManager.Instance.mapList[1].GraveStoneNum = 6;
				MapManager.Instance.mapList[1].GraveStoneLine = 4;
			});
		}
	}

	private void LV11029()
	{
		if (IsEasy)
		{
			StartTime = 1140;
		}
		else
		{
			StartTime = 1200;
		}
		LvName = "极致寒冷";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Snow, 10, 0)
		};
		if (!OnlyInfo)
		{
			LvTemperature = -40;
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 700;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 8, 12, 18, 16, 20, 28 },
					new List<int> { 1, 2, 3, 8, 10, 18, 12, 18, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.Polevaulter,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.Gargantuar
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.PaperZombie,
						ZombieType.BucketZombie,
						ZombieType.DoorAndCone,
						ZombieType.IceShroomZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 500;
				SetupTime = new Vector2(8f, 12f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 6, 8, 12, 28, 22, 28, 34 },
					new List<int> { 1, 2, 3, 6, 10, 28, 18, 28, 42 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.BucketZombie,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.Gargantuar,
						ZombieType.Polevaulter,
						ZombieType.SnowRepeaterZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni,
						ZombieType.BobsledZombieSled,
						ZombieType.DoorAndCone,
						ZombieType.GargantuarRedeye,
						ZombieType.IceShroomZombie,
						ZombieType.WinterMelonZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
			BigWaveAction = () =>
			{
				ZombieManager.Instance.UpdateZombieOnRandomLine(ZombieType.Yeti, MapManager.Instance.mapList[Random.Range(0, 2)].transform.position);
			};
		}
	}

	private void LV11030()
	{
		if (IsEasy)
		{
			StartTime = 480;
		}
		else
		{
			StartTime = 1200;
		}
		LvName = "植物僵尸5";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 320),
			new FutureWeather(WeatherType.Rain, 8, 999)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 600;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 18, 10, 14, 28 },
					new List<int> { 1, 1, 3, 4, 14, 16, 12, 20, 30 },
					new List<int> { 1, 1, 3, 8, 14, 22, 12, 22, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.IceShroomZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.RepeaterZombie,
						ZombieType.GatlingZombie,
						ZombieType.JalapenoZombie,
						ZombieType.SquashZombie,
						ZombieType.WallNutZombie,
						ZombieType.SunflowerZombie,
						ZombieType.SunflowerZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.StarfruitZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.FumeShroomZombie,
						ZombieType.GloomShroomZombie,
						ZombieType.TallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.CabbagepultZombie,
						ZombieType.MelonpultZombie,
						ZombieType.WinterMelonZombie,
						ZombieType.UmbrellaZombie,
						ZombieType.JalapenoZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SunflowerZombie,
						ZombieType.SunflowerZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 400;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 8, 10, 14, 24, 16, 24, 38 },
					new List<int> { 2, 2, 3, 4, 14, 16, 18, 28, 35 },
					new List<int> { 1, 3, 3, 8, 14, 22, 18, 28, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.HyponShroomZombie,
						ZombieType.IceShroomZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.RepeaterZombie,
						ZombieType.GatlingZombie,
						ZombieType.JalapenoZombie,
						ZombieType.SquashZombie,
						ZombieType.WallNutZombie,
						ZombieType.SunflowerZombie,
						ZombieType.SunflowerZombie
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.StarfruitZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.FumeShroomZombie,
						ZombieType.GloomShroomZombie,
						ZombieType.TallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SquashZombie
					},
					new List<ZombieType>
					{
						ZombieType.CabbagepultZombie,
						ZombieType.MelonpultZombie,
						ZombieType.WinterMelonZombie,
						ZombieType.UmbrellaZombie,
						ZombieType.JalapenoZombie,
						ZombieType.TallNutZombie,
						ZombieType.WallNutZombie,
						ZombieType.SquashZombie,
						ZombieType.SunflowerZombie,
						ZombieType.SunflowerZombie
					}
				};
			}
			BigWaveNum = new List<int> { 5, 8 };
		}
	}

	private void LV11031()
	{
		LvName = "小游戏派对";
		StartTime = 480;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 100),
			new FutureWeather(WeatherType.Rain, 6, 360),
			new FutureWeather(WeatherType.Thunder, 10, 610)
		};
		if (!OnlyInfo)
		{
			StartCardCd = false;
			LvSpStates.Add(LVSpState.FreeDay);
			LvSpStates.Add(LVSpState.NutBowling);
			LvSpStates.Add(LVSpState.PlantReverse);
			LvSpStates.Add(LVSpState.SleepDay);
			dayBgm = BgmType.WateryGraves;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			if (IsEasy)
			{
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie,
					ZombieType.NormalZombie
				};
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 8, 18, 20, 28, 40, 28, 34, 54 },
					new List<int> { 2, 4, 8, 20, 18, 24, 38, 30, 40, 50 }
				};
				SetupTime = new Vector2(12f, 15f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.BlackFootball,
						ZombieType.Zomboni,
						ZombieType.GargantuarHelmet
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.TubeConeZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.BlackFootball,
						ZombieType.SnorkleZombie,
						ZombieType.Gargantuar,
						ZombieType.PeaShooterZombie
					}
				};
				BigWaveNum = new List<int> { 3, 6, 9 };
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int> { 4, 8, 8, 28, 20, 38, 50, 48, 54, 74 },
					new List<int> { 3, 8, 8, 28, 25, 34, 58, 40, 60, 70 }
				};
				SetupTime = new Vector2(8f, 12f);
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.DiggerZombie,
						ZombieType.BucketZombie,
						ZombieType.CatapultZombie,
						ZombieType.Polevaulter,
						ZombieType.FootballZombie,
						ZombieType.BlackFootball,
						ZombieType.SnowRepeaterZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet
					},
					new List<ZombieType>
					{
						ZombieType.PeaShooterZombie,
						ZombieType.TubeConeZombie,
						ZombieType.TubeBucketZombie,
						ZombieType.Polevaulter,
						ZombieType.DiggerZombie,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.SnorkleZombie,
						ZombieType.Polevaulter,
						ZombieType.DolphinriderZombie,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.RepeaterZombie
					}
				};
				BigWaveNum = new List<int> { 3, 6, 9 };
			}
			StartAction = () =>
			{
				MapManager.Instance.CreatePortal(2, 0);
				MapManager.Instance.CreatePortal(2, 2);
			};
		}
	}

	private void LV11032()
	{
		StartTime = 434;
		LvName = "终极决战";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard,
			MapType.Roof
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 6, 466),
			new FutureWeather(WeatherType.Thunder, 10, 586)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.TubeZombie,
				ZombieType.TubeConeZombie,
				ZombieType.TubeBucketZombie
			};
			if (IsEasy)
			{
				LvNormalSunNum = 1200;
				SetupTime = new Vector2(25f, 30f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 3, 10, 6, 8, 12, 16, 20, 30,
						20, 23, 40, 25, 35, 35, 30, 40, 55
					},
					new List<int>
					{
						1, 2, 3, 8, 10, 16, 30, 15, 18, 28,
						30, 33, 40, 30, 30, 45, 30, 40, 55
					},
					new List<int>
					{
						0, 1, 3, 8, 10, 18, 10, 15, 15, 20,
						20, 23, 40, 30, 30, 45, 30, 45, 55
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar,
						ZombieType.GatlingZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.GargantuarHelmet,
						ZombieType.BungiZombie,
						ZombieType.LadderZombie,
						ZombieType.JacksonZombie,
						ZombieType.RepeaterZombie,
						ZombieType.RoadrollerZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.DiggerZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.GargantuarRedeye,
						ZombieType.JalapenoZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.HeavyBungiZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.FootballZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarRedeye,
						ZombieType.LadderZombie,
						ZombieType.WinterMelonZombie,
						ZombieType.CabbagepultZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 1000;
				SetupTime = new Vector2(20f, 25f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 16, 12, 12, 22, 26, 30, 40,
						30, 33, 50, 40, 50, 70, 40, 50, 85
					},
					new List<int>
					{
						2, 2, 3, 12, 10, 20, 30, 15, 25, 28,
						30, 33, 60, 40, 50, 65, 40, 50, 85
					},
					new List<int>
					{
						2, 2, 3, 8, 10, 18, 18, 15, 20, 30,
						30, 33, 50, 40, 50, 65, 40, 50, 85
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.Zomboni,
						ZombieType.BlackFootball,
						ZombieType.ConeZombie,
						ZombieType.BalloonZombie,
						ZombieType.Gargantuar,
						ZombieType.GatlingZombie,
						ZombieType.TorchwoodZombie,
						ZombieType.GargantuarHelmet,
						ZombieType.BungiZombie,
						ZombieType.PogoZombie,
						ZombieType.DiggerZombie,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.LadderZombie,
						ZombieType.JacksonZombie,
						ZombieType.DoomShroomZombie,
						ZombieType.RepeaterZombie,
						ZombieType.UmbrellaZombie,
						ZombieType.RoadrollerZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.DiggerZombie,
						ZombieType.Polevaulter,
						ZombieType.SnorkleZombie,
						ZombieType.DolphinriderZombie,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.Gargantuar,
						ZombieType.CatapultZombie,
						ZombieType.Zomboni,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmet,
						ZombieType.LadderZombie,
						ZombieType.JacksonZombie,
						ZombieType.JalapenoZombie,
						ZombieType.SnowRepeaterZombie,
						ZombieType.TallNutZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.DoorAndCone,
						ZombieType.JackboxZombie,
						ZombieType.FootballZombie,
						ZombieType.BungiZombie,
						ZombieType.CatapultZombie,
						ZombieType.Gargantuar,
						ZombieType.GargantuarHelmet,
						ZombieType.GargantuarRedeye,
						ZombieType.GargantuarHelmetRedeye,
						ZombieType.LadderZombie,
						ZombieType.WinterMelonZombie,
						ZombieType.IceShroomZombie,
						ZombieType.UmbrellaZombie
					}
				};
			}
			BigWaveNum = new List<int> { 3, 6, 9, 12, 15, 18 };
			TimeAction.Add(1000, () =>
			{
				MapManager.Instance.mapList[0].GraveStoneNum = 10;
				MapManager.Instance.mapList[0].GraveStoneLine = 4;
				MapManager.Instance.mapList[1].GraveStoneNum = 8;
				MapManager.Instance.mapList[1].GraveStoneLine = 4;
			});
		}
	}

	private void LV11033()
	{
		if (IsEasy)
		{
			StartTime = 790;
		}
		else
		{
			StartTime = 880;
		}
		LvName = "蹦极闪电战";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.Roof
		};
		if (!OnlyInfo)
		{
			BungiSpRate = 0.9f;
			LvSpStates.Add(LVSpState.BungiMode);
			dayBgm = BgmType.GrazeTheRoof;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			if (IsEasy)
			{
				LvNormalSunNum = 450;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 3, 6, 10, 18, 8, 15, 25 },
					new List<int> { 1, 3, 8, 6, 15, 8, 15, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Zomboni,
						ZombieType.RepeaterZombie,
						ZombieType.HeavyBungiZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.BucketZombie,
						ZombieType.ConeZombie,
						ZombieType.Gargantuar
					}
				};
			}
			else
			{
				LvNormalSunNum = 550;
				SetupTime = new Vector2(12f, 15f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 6, 14, 23, 14, 22, 35 },
					new List<int> { 2, 3, 8, 10, 18, 12, 21, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.ConeZombie,
						ZombieType.BucketZombie,
						ZombieType.RoadrollerZombie,
						ZombieType.Zomboni,
						ZombieType.RepeaterZombie,
						ZombieType.HeavyBungiZombie,
						ZombieType.JacksonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.BucketZombie,
						ZombieType.ConeZombie,
						ZombieType.LadderZombie,
						ZombieType.GargantuarRedeye,
						ZombieType.JackboxZombie,
						ZombieType.JalapenoZombie
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV11034()
	{
		if (IsEasy)
		{
			StartTime = 480;
		}
		else
		{
			StartTime = 720;
		}
		LvName = "手套战争1";
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			CardNum = 4;
			BanMultyCardAdd = true;
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			EnablePlantGlove = true;
			FixedCard = new List<CardType>
			{
				new CardType(PlantType.Lilypad),
				new CardType(PlantType.Cherry),
				new CardType(PlantType.PotatoMine),
				new CardType(PlantType.Squash)
			};
			if (IsEasy)
			{
				LvNormalSunNum = 50;
				SetupTime = new Vector2(3f, 5f);
				Weights = new List<List<int>>
				{
					new List<int> { 3, 3, 8, 12, 16, 8, 12, 18 },
					new List<int> { 3, 8, 6, 12, 14, 8, 10, 16 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.DoorZombie,
						ZombieType.BucketZombie,
						ZombieType.BalloonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.DoorZombie,
						ZombieType.ConeZombie
					}
				};
				StartPlants = new List<List<List<PlantType>>>
				{
					new List<List<PlantType>>
					{
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.WallNut,
							PlantType.GatlingPea
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.ThreePeater,
							PlantType.SnowPea
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.Melonpult,
							PlantType.Cactus
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.Tallnut,
							PlantType.Torchwood
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.TwinSunflower,
							PlantType.Cabbagepult
						}
					}
				};
			}
			else
			{
				LvNormalSunNum = 0;
				SetupTime = new Vector2(1f, 3f);
				Weights = new List<List<int>>
				{
					new List<int> { 6, 8, 10, 10, 22, 14, 15, 25 },
					new List<int> { 6, 8, 10, 10, 22, 14, 18, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.DoorZombie,
						ZombieType.BucketZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.BalloonZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.Polevaulter,
						ZombieType.PaperZombie,
						ZombieType.DoorZombie,
						ZombieType.ConeZombie,
						ZombieType.RepeaterZombie,
						ZombieType.CatapultZombie
					}
				};
				StartPlants = new List<List<List<PlantType>>>
				{
					new List<List<PlantType>>
					{
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.SunFlower,
							PlantType.GatlingPea
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.ThreePeater,
							PlantType.SnowPea
						},
						new List<PlantType>
						{
							PlantType.Umbrellaleaf,
							PlantType.Melonpult,
							PlantType.Cactus
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.Tallnut,
							PlantType.Torchwood
						},
						new List<PlantType>
						{
							PlantType.Nope,
							PlantType.SunFlower,
							PlantType.Cabbagepult
						}
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV11035()
	{
		if (IsEasy)
		{
			StartTime = 1200;
		}
		else
		{
			StartTime = 1280;
		}
		LvName = "手套战争2";
		CurrSeedBankType = SeedBankType.MoonBank;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontYard,
			MapType.BackYard
		};
		if (!OnlyInfo)
		{
			CardNum = 6;
			BanMultyCardAdd = true;
			dayBgm = BgmType.GrassWalk;
			nightBgm = BgmType.RigorMormist;
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			EnableZombieGlove = true;
			FixedCard = new List<CardType>
			{
				new CardType(PlantType.MoonTombStone),
				new CardType(ZombieType.PeaShooterZombie),
				new CardType(ZombieType.SnowpeaShooterZombie),
				new CardType(ZombieType.FumeShroomZombie),
				new CardType(ZombieType.CabbagepultZombie),
				new CardType(ZombieType.BucketZombie),
				new CardType(ZombieType.DoorAndBucket)
			};
			if (IsEasy)
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(3f, 5f);
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 10, 16, 15, 16, 26 },
					new List<int> { 1, 2, 6, 8, 20, 15, 18, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.PeaShooterZombie,
						ZombieType.ConeZombie,
						ZombieType.Polevaulter,
						ZombieType.BucketZombie,
						ZombieType.CatapultZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.PaperZombie,
						ZombieType.ConeZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.BucketZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 200;
				SetupTime = new Vector2(1f, 3f);
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 10, 15, 22, 18, 24, 35 },
					new List<int> { 2, 3, 14, 15, 22, 20, 28, 35 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.RepeaterZombie,
						ZombieType.SnowpeaShooterZombie,
						ZombieType.BucketZombie,
						ZombieType.Polevaulter,
						ZombieType.CatapultZombie
					},
					new List<ZombieType>
					{
						ZombieType.NormalZombie,
						ZombieType.ConeZombie,
						ZombieType.RepeaterZombie,
						ZombieType.PaperZombie,
						ZombieType.BucketZombie,
						ZombieType.FootballZombie,
						ZombieType.Polevaulter,
						ZombieType.Zomboni
					}
				};
			}
			BigWaveNum = new List<int> { 4, 7 };
		}
	}

	private void LV12001()
	{
		CardNum = 0;
		LvNormalSunNum = 0;
		LvName = "破罐者";
		StartTime = 0;
		CurrLVType = LVType.VaseBreaker;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			PlantVaseNum = 2;
			ZombieVaseNum = 2;
			VaseBreakerVase = new List<List<List<VaseType>>>
			{
				new List<List<VaseType>>
				{
					new List<VaseType>
					{
						new VaseType(4, ZombieType.BucketZombie),
						new VaseType(5, PlantType.RepeaterReverse),
						new VaseType(6, PlantType.WallNut),
						new VaseType(7, PlantType.Squash)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.JackboxZombie),
						new VaseType(5, ZombieType.Gargantuar),
						new VaseType(6, PlantType.RepeaterReverse),
						new VaseType(7, ZombieType.BucketZombie)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.Gargantuar),
						new VaseType(5, PlantType.SnowPea),
						new VaseType(6, PlantType.Squash),
						new VaseType(7, PlantType.RepeaterReverse)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.NormalZombie),
						new VaseType(5, PlantType.RepeaterReverse),
						new VaseType(6, ZombieType.Polevaulter),
						new VaseType(7, PlantType.ThreePeater)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.JackboxZombie),
						new VaseType(5, PlantType.RepeaterReverse),
						new VaseType(6, PlantType.SnowPea),
						new VaseType(7, PlantType.Squash)
					}
				}
			};
		}
	}

	private void LV12002()
	{
		CardNum = 0;
		LvNormalSunNum = 150;
		LvName = "我是僵尸";
		StartTime = 0;
		CurrLVType = LVType.IZombie;
		CurrSeedBankType = SeedBankType.MoonBank;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(ZombieType.NormalZombie),
				new CardType(ZombieType.BucketZombie),
				new CardType(ZombieType.FootballZombie)
			};
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.Squash,
						PlantType.SnowPea,
						PlantType.PeaShooter,
						PlantType.PeaShooter
					},
					new List<PlantType>
					{
						PlantType.PeaShooter,
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.PeaShooter,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.GatlingPea,
						PlantType.PeaShooter,
						PlantType.SnowPea,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.Squash,
						PlantType.SunFlower,
						PlantType.Squash
					}
				}
			};
		}
	}

	private void LV12003()
	{
		CardNum = 0;
		LvNormalSunNum = 0;
		LvName = "连锁反应";
		StartTime = 0;
		CurrLVType = LVType.VaseBreaker;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			PlantVaseNum = 2;
			ZombieVaseNum = 2;
			VaseBreakerVase = new List<List<List<VaseType>>>
			{
				new List<List<VaseType>>
				{
					new List<VaseType>
					{
						new VaseType(2, ZombieType.PogoZombie),
						new VaseType(3, PlantType.PuffShroom),
						new VaseType(4, PlantType.RepeaterReverse),
						new VaseType(5, ZombieType.JackboxZombie),
						new VaseType(6, PlantType.Tallnut),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, PlantType.PuffShroom)
					},
					new List<VaseType>
					{
						new VaseType(2, PlantType.PuffShroom),
						new VaseType(3, ZombieType.NormalZombie),
						new VaseType(4, PlantType.Tallnut),
						new VaseType(5, PlantType.PuffShroom),
						new VaseType(6, PlantType.Squash),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, ZombieType.PogoZombie)
					},
					new List<VaseType>
					{
						new VaseType(2, ZombieType.JackboxZombie),
						new VaseType(3, ZombieType.JackboxZombie),
						new VaseType(4, PlantType.Squash),
						new VaseType(5, ZombieType.PogoZombie),
						new VaseType(6, PlantType.Squash),
						new VaseType(7, ZombieType.JackboxZombie),
						new VaseType(8, PlantType.PuffShroom)
					},
					new List<VaseType>
					{
						new VaseType(2, PlantType.PuffShroom),
						new VaseType(3, PlantType.PuffShroom),
						new VaseType(4, PlantType.Squash),
						new VaseType(5, ZombieType.NormalZombie),
						new VaseType(6, ZombieType.NormalZombie),
						new VaseType(7, ZombieType.JackboxZombie),
						new VaseType(8, ZombieType.PogoZombie)
					},
					new List<VaseType>
					{
						new VaseType(2, PlantType.Tallnut),
						new VaseType(3, PlantType.Squash),
						new VaseType(4, ZombieType.JackboxZombie),
						new VaseType(5, ZombieType.NormalZombie),
						new VaseType(6, ZombieType.JackboxZombie),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, ZombieType.JackboxZombie)
					}
				}
			};
		}
	}

	private void LV12004()
	{
		CardNum = 0;
		LvNormalSunNum = 150;
		LvName = "力拔山河";
		StartTime = 0;
		CurrLVType = LVType.IZombie;
		CurrSeedBankType = SeedBankType.MoonBank;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(ZombieType.NormalZombie),
				new CardType(ZombieType.Polevaulter),
				new CardType(ZombieType.BucketZombie),
				new CardType(ZombieType.Gargantuar)
			};
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.Squash,
						PlantType.Spike,
						PlantType.Torchwood,
						PlantType.SunFlower,
						PlantType.Spike
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.Spike,
						PlantType.SunFlower,
						PlantType.Garlic
					},
					new List<PlantType>
					{
						PlantType.PeaShooter,
						PlantType.PeaShooter,
						PlantType.PeaShooter,
						PlantType.SnowPea,
						PlantType.PeaShooter
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.Cornpult,
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.Garlic
					},
					new List<PlantType>
					{
						PlantType.PeaShooter,
						PlantType.SunFlower,
						PlantType.Cornpult,
						PlantType.Torchwood,
						PlantType.Squash
					}
				}
			};
		}
	}

	private void LV12005()
	{
		CardNum = 0;
		LvNormalSunNum = 0;
		LvName = "破坏之王";
		StartTime = 0;
		CurrLVType = LVType.VaseBreaker;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			PlantVaseNum = 2;
			ZombieVaseNum = 2;
			VaseBreakerVase = new List<List<List<VaseType>>>
			{
				new List<List<VaseType>>
				{
					new List<VaseType>
					{
						new VaseType(2, PlantType.RepeaterReverse),
						new VaseType(3, ZombieType.NormalZombie),
						new VaseType(4, PlantType.RepeaterReverse),
						new VaseType(5, PlantType.ThreePeater),
						new VaseType(6, PlantType.RepeaterReverse),
						new VaseType(7, ZombieType.BucketZombie),
						new VaseType(8, PlantType.RepeaterReverse)
					},
					new List<VaseType>
					{
						new VaseType(2, ZombieType.Gargantuar),
						new VaseType(3, PlantType.SnowPea),
						new VaseType(4, PlantType.RepeaterReverse),
						new VaseType(5, ZombieType.NormalZombie),
						new VaseType(6, ZombieType.NormalZombie),
						new VaseType(7, PlantType.ThreePeater),
						new VaseType(8, ZombieType.BucketZombie)
					},
					new List<VaseType>
					{
						new VaseType(2, PlantType.Squash),
						new VaseType(3, PlantType.Squash),
						new VaseType(4, ZombieType.NormalZombie),
						new VaseType(5, PlantType.SnowPea),
						new VaseType(6, PlantType.PotatoMine),
						new VaseType(7, PlantType.Plantern),
						new VaseType(8, PlantType.Squash)
					},
					new List<VaseType>
					{
						new VaseType(2, PlantType.RepeaterReverse),
						new VaseType(3, PlantType.PeaShooter),
						new VaseType(4, PlantType.Squash),
						new VaseType(5, ZombieType.BucketZombie),
						new VaseType(6, ZombieType.NormalZombie),
						new VaseType(7, ZombieType.Gargantuar),
						new VaseType(8, ZombieType.NormalZombie)
					},
					new List<VaseType>
					{
						new VaseType(2, ZombieType.NormalZombie),
						new VaseType(3, PlantType.WallNut),
						new VaseType(4, ZombieType.JackboxZombie),
						new VaseType(5, ZombieType.BucketZombie),
						new VaseType(6, ZombieType.NormalZombie),
						new VaseType(7, ZombieType.BucketZombie),
						new VaseType(8, PlantType.Squash)
					}
				}
			};
		}
	}

	private void LV12006()
	{
		CardNum = 0;
		LvNormalSunNum = 150;
		LvName = "地雷区";
		StartTime = 0;
		CurrLVType = LVType.IZombie;
		CurrSeedBankType = SeedBankType.MoonBank;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(ZombieType.NormalZombie),
				new CardType(ZombieType.Polevaulter)
			};
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.PotatoMine,
						PlantType.SunFlower,
						PlantType.Chomper
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.Chomper,
						PlantType.PotatoMine,
						PlantType.SunFlower,
						PlantType.PotatoMine
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.Chomper,
						PlantType.Chomper,
						PlantType.PotatoMine,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.PotatoMine,
						PlantType.Chomper,
						PlantType.PotatoMine,
						PlantType.PotatoMine,
						PlantType.PotatoMine
					},
					new List<PlantType>
					{
						PlantType.Chomper,
						PlantType.Chomper,
						PlantType.Chomper,
						PlantType.PotatoMine,
						PlantType.SunFlower
					}
				}
			};
		}
	}

	private void LV12007()
	{
		CardNum = 0;
		LvNormalSunNum = 0;
		LvName = "水族罐";
		StartTime = 0;
		CurrLVType = LVType.VaseBreaker;
		LoadMapTypes = new List<MapType> { MapType.BackYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			PlantVaseNum = 2;
			ZombieVaseNum = 2;
			VaseBreakerVase = new List<List<List<VaseType>>>
			{
				new List<List<VaseType>>
				{
					new List<VaseType>
					{
						new VaseType(4, PlantType.Jalapeno),
						new VaseType(5, ZombieType.JackboxZombie),
						new VaseType(6, PlantType.Tallnut),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, PlantType.Lilypad)
					},
					new List<VaseType>
					{
						new VaseType(4, PlantType.Tallnut),
						new VaseType(5, PlantType.Lilypad),
						new VaseType(6, PlantType.Squash),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, ZombieType.Polevaulter)
					},
					new List<VaseType>
					{
						new VaseType(4, PlantType.Lilypad),
						new VaseType(5, ZombieType.Polevaulter),
						new VaseType(6, PlantType.Squash),
						new VaseType(7, ZombieType.Gargantuar),
						new VaseType(8, PlantType.RepeaterReverse)
					},
					new List<VaseType>
					{
						new VaseType(4, PlantType.Squash),
						new VaseType(5, ZombieType.NormalZombie),
						new VaseType(6, ZombieType.NormalZombie),
						new VaseType(7, ZombieType.JackboxZombie),
						new VaseType(8, ZombieType.Polevaulter)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.JackboxZombie),
						new VaseType(5, ZombieType.BucketZombie),
						new VaseType(6, PlantType.Lilypad),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, ZombieType.JacksonZombie)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.NormalZombie),
						new VaseType(5, ZombieType.BucketZombie),
						new VaseType(6, ZombieType.JackboxZombie),
						new VaseType(7, PlantType.RepeaterReverse),
						new VaseType(8, PlantType.Lilypad)
					}
				}
			};
		}
	}

	private void LV12008()
	{
		CardNum = 0;
		LvNormalSunNum = 150;
		LvName = "飞檐走壁";
		StartTime = 0;
		CurrLVType = LVType.IZombie;
		CurrSeedBankType = SeedBankType.MoonBank;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(ZombieType.NormalZombie),
				new CardType(ZombieType.ConeZombie),
				new CardType(ZombieType.BucketZombie),
				new CardType(ZombieType.BungiZombie),
				new CardType(ZombieType.DiggerZombie),
				new CardType(ZombieType.LadderZombie)
			};
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.Squash,
						PlantType.Squash,
						PlantType.PeaShooter
					},
					new List<PlantType>
					{
						PlantType.PotatoMine,
						PlantType.PeaShooter,
						PlantType.WallNut,
						PlantType.PeaShooter,
						PlantType.WallNut
					},
					new List<PlantType>
					{
						PlantType.PeaShooter,
						PlantType.SunFlower,
						PlantType.PotatoMine,
						PlantType.Magnetshroom,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.Magnetshroom,
						PlantType.WallNut,
						PlantType.PeaShooter,
						PlantType.PeaShooter
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.PeaShooter,
						PlantType.PeaShooter
					}
				}
			};
		}
	}

	private void LV12009()
	{
		CardNum = 0;
		LvNormalSunNum = 0;
		LvName = "屋顶罐";
		StartTime = 0;
		CurrLVType = LVType.VaseBreaker;
		LoadMapTypes = new List<MapType> { MapType.Roof };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			PlantVaseNum = 2;
			ZombieVaseNum = 2;
			VaseBreakerVase = new List<List<List<VaseType>>>
			{
				new List<List<VaseType>>
				{
					new List<VaseType>
					{
						new VaseType(4, ZombieType.BucketZombie),
						new VaseType(5, PlantType.Cabbagepult),
						new VaseType(6, PlantType.WallNut),
						new VaseType(7, PlantType.Squash),
						new VaseType(8, PlantType.Squash)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.NormalZombie),
						new VaseType(5, ZombieType.Gargantuar),
						new VaseType(6, ZombieType.CatapultZombie),
						new VaseType(7, ZombieType.BucketZombie),
						new VaseType(8, PlantType.Pot)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.Gargantuar),
						new VaseType(5, PlantType.Cornpult),
						new VaseType(6, PlantType.Squash),
						new VaseType(7, PlantType.Pot),
						new VaseType(8, PlantType.Pot)
					},
					new List<VaseType>
					{
						new VaseType(4, ZombieType.NormalZombie),
						new VaseType(5, PlantType.Cabbagepult),
						new VaseType(6, ZombieType.Polevaulter),
						new VaseType(7, PlantType.Jalapeno),
						new VaseType(8, PlantType.Pot)
					},
					new List<VaseType>
					{
						new VaseType(4, PlantType.Melonpult),
						new VaseType(5, PlantType.Melonpult),
						new VaseType(6, PlantType.WallNut),
						new VaseType(7, PlantType.Squash),
						new VaseType(8, PlantType.Melonpult)
					}
				}
			};
		}
	}

	private void LV12010()
	{
		CardNum = 0;
		LvNormalSunNum = 150;
		LvName = "大混战";
		StartTime = 0;
		CurrLVType = LVType.IZombie;
		CurrSeedBankType = SeedBankType.MoonBank;
		LoadMapTypes = new List<MapType> { MapType.FrontYard };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.Trophy;
			AwardScence.Instance.LoadText("你获得了一个奖杯!", "奖杯", "继续解锁更多奖杯吧！");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1000;
			};
			FixedCard = new List<CardType>
			{
				new CardType(ZombieType.NormalZombie),
				new CardType(ZombieType.ConeZombie),
				new CardType(ZombieType.Polevaulter),
				new CardType(ZombieType.BucketZombie),
				new CardType(ZombieType.BungiZombie),
				new CardType(ZombieType.DiggerZombie),
				new CardType(ZombieType.LadderZombie),
				new CardType(ZombieType.FootballZombie)
			};
			StartPlants = new List<List<List<PlantType>>>
			{
				new List<List<PlantType>>
				{
					new List<PlantType>
					{
						PlantType.PotatoMine,
						PlantType.SunFlower,
						PlantType.PotatoMine,
						PlantType.SunFlower,
						PlantType.PotatoMine,
						PlantType.PotatoMine
					},
					new List<PlantType>
					{
						PlantType.ThreePeater,
						PlantType.SunFlower,
						PlantType.SunFlower,
						PlantType.SnowPea,
						PlantType.SplitPea,
						PlantType.Tallnut
					},
					new List<PlantType>
					{
						PlantType.Chomper,
						PlantType.Chomper,
						PlantType.SunFlower,
						PlantType.Squash,
						PlantType.Chomper,
						PlantType.SunFlower
					},
					new List<PlantType>
					{
						PlantType.SunFlower,
						PlantType.PeaShooter,
						PlantType.PeaShooter,
						PlantType.PeaShooter,
						PlantType.SunFlower,
						PlantType.Torchwood
					},
					new List<PlantType>
					{
						PlantType.Magnetshroom,
						PlantType.SplitPea,
						PlantType.SunFlower,
						PlantType.Starfruit,
						PlantType.FumeShroom,
						PlantType.ScaredyShroom
					}
				}
			};
		}
	}

	private void LV20001()
	{
		StartTime = 900;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 3, 6, 18, 12, 20 }
				};
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 22, 18, 28 }
				};
			}
			LvNormalSunNum = 100;
			SetupTime = new Vector2(15f, 18f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor
				}
			};
			BigWaveNum = new List<int> { 4, 6 };
		}
	}

	private void LV20002()
	{
		StartTime = 700;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 2, 4, 10, 6, 8, 12, 20 }
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 3, 6, 14, 8, 10, 18, 28 }
				};
			}
			SetupTime = new Vector2(13f, 15f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie
				}
			};
			BigWaveNum = new List<int> { 4, 8 };
		}
	}

	private void LV20003()
	{
		StartTime = 700;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 2, 6, 10, 6, 8, 12, 20, 18,
						25
					}
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 6, 14, 8, 10, 18, 28, 20,
						35
					}
				};
			}
			SetupTime = new Vector2(13f, 15f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.SwampDoorAndStool,
					ZombieType.StoolZombie
				}
			};
			BigWaveNum = new List<int> { 4, 8, 10 };
		}
	}

	private void LV20004()
	{
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		StartTime = 12;
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 18, 12, 22, 10, 16, 26 }
				};
			}
			else
			{
				LvNormalSunNum = 50;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 8, 8, 24, 18, 28, 14, 20, 30 }
				};
			}
			CardNum = 15;
			SetupTime = new Vector2(10f, 12f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.SwampDoorAndStool,
					ZombieType.SwampDoorAndBucket,
					ZombieType.StoolZombie
				}
			};
			BigWaveNum = new List<int> { 4, 6, 9 };
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			TimeAction.Add(120, () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
			});
		}
	}

	private void LV20005()
	{
		StartTime = 45;
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 200;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 5, 12, 16, 12, 16, 20, 24 }
				};
			}
			else
			{
				LvNormalSunNum = 75;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 12, 20, 16, 20, 24, 36 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket
				};
			}
			SetupTime = new Vector2(13f, 15f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.SwampDoorAndBucket
				}
			};
			BigWaveNum = new List<int> { 5, 9 };
		}
	}

	private void LV20006()
	{
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		StartTime = 60;
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 4, 10, 10, 15 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.Ghost,
						ZombieType.Ghost
					}
				};
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 3, 6, 18, 16, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.Ghost,
						ZombieType.Ghost
					}
				};
			}
			LvNormalSunNum = 100;
			SetupTime = new Vector2(13f, 15f);
			BigWaveNum = new List<int> { 6 };
		}
	}

	private void LV20007()
	{
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		StartTime = 100;
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 3, 6, 12, 8, 10, 12, 22, 25,
						35
					}
				};
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 6, 18, 10, 12, 18, 28, 35,
						48
					}
				};
			}
			LvNormalSunNum = 100;
			SetupTime = new Vector2(13f, 15f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.Ghost
				}
			};
			BigWaveNum = new List<int> { 4, 8, 10 };
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(3);
			};
			TimeAction.Add(150, () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
			});
		}
	}

	private void LV20008()
	{
		LvNormalSunNum = 100;
		StartTime = 420;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 125;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 5, 12, 12, 18 }
				};
			}
			else
			{
				LvNormalSunNum = 75;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 12, 16, 26 }
				};
			}
			SetupTime = new Vector2(13f, 15f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.SwampDoorAndStool,
					ZombieType.Crocodile,
					ZombieType.Crocodile,
					ZombieType.Crocodile
				}
			};
			BigWaveNum = new List<int> { 6 };
		}
	}

	private void LV20009()
	{
		StartTime = 466;
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 4, 0)
		};
		if (!OnlyInfo)
		{
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.ZombieNote;
			AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "两面夹击");
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 5, 12, 16, 12, 12, 16, 24 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.SwampDoorAndStool,
						ZombieType.Crocodile,
						ZombieType.Crocodile
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 12, 22, 18, 20, 24, 36 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.SwampDoorAndStool,
						ZombieType.Crocodile,
						ZombieType.Crocodile,
						ZombieType.Crocodile,
						ZombieType.Ghost
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket
				};
			}
			SetupTime = new Vector2(13f, 15f);
			BigWaveNum = new List<int> { 5, 9 };
		}
	}

	private void LV20010()
	{
		StartTime = 480;
		BootyPlant = PlantType.JellyShroom;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 4, 0),
			new FutureWeather(WeatherType.Rain, 10, 344)
		};
		if (!OnlyInfo)
		{
			AwardScence.Instance.LoadText("你获得了一株新的植物!", PlantManager.Instance.GetPlantName(BootyPlant), "改变子弹方向或弹飞子弹，\n并提升子弹伤害");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			BigWaveNum = new List<int> { 4, 7 };
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.Crocodile,
					ZombieType.SwampDoor
				},
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.Crocodile
				}
			};
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 2, 4, 12, 8, 12, 20 },
					new List<int> { 0, 1, 3, 4, 10, 10, 14, 16 }
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 2, 4, 18, 8, 12, 26 },
					new List<int> { 1, 1, 3, 4, 16, 10, 14, 26 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket
				};
			}
		}
	}

	private void LV20011()
	{
		LvNormalSunNum = 100;
		StartTime = 524;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 2, 4, 12, 10, 12, 24 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.SwampDoorAndStool,
						ZombieType.SwampGargantuar
					}
				};
			}
			else
			{
				LvNormalSunNum = 75;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 3, 6, 16, 10, 20, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.SwampDoorAndStool,
						ZombieType.Crocodile,
						ZombieType.Ghost,
						ZombieType.SwampGargantuar
					}
				};
			}
			SetupTime = new Vector2(10f, 15f);
			BigWaveNum = new List<int> { 7 };
		}
	}

	private void LV20012()
	{
		StartTime = 622;
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 8, 0)
		};
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 1, 2, 4, 4, 8, 20, 12, 16, 20,
						26
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.SwampDoorAndStool,
						ZombieType.SwampGargantuar,
						ZombieType.SwampGargantuarNoCro
					}
				};
			}
			else
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 3, 6, 8, 12, 28, 16, 20, 24,
						35
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.SwampDoorAndStool,
						ZombieType.Crocodile,
						ZombieType.SwampGargantuar,
						ZombieType.Ghost,
						ZombieType.SwampGargantuarNoCro
					}
				};
			}
			SetupTime = new Vector2(12f, 15f);
			BigWaveNum = new List<int> { 6, 10 };
		}
	}

	private void LV20013()
	{
		StartTime = 666;
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 600;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 200;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 3, 6, 14, 12, 14, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.Crocodile
					}
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 6, 8, 24, 18, 16, 32 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.Crocodile,
						ZombieType.SwampGargantuar
					}
				};
			}
			SetupTime = new Vector2(10f, 12f);
			BigWaveNum = new List<int> { 4, 7 };
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(4);
			};
			TimeAction.Add(734, () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
			});
		}
	}

	private void LV20014()
	{
		StartTime = 722;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 6, 0),
			new FutureWeather(WeatherType.Thunder, 10, 40)
		};
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 500;
				SetupTime = new Vector2(14f, 16f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						1, 2, 4, 6, 18, 12, 20, 12, 16, 24,
						20, 25, 25, 30, 35
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.SwampGargantuar,
						ZombieType.Crocodile
					}
				};
			}
			else
			{
				LvNormalSunNum = 300;
				SetupTime = new Vector2(10f, 12f);
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 8, 8, 24, 18, 28, 14, 20, 30,
						20, 25, 45, 38, 55
					}
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampBucket,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.Crocodile,
						ZombieType.SwampGargantuar
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket
				};
			}
			BigWaveNum = new List<int> { 4, 6, 9, 12, 14 };
		}
	}

	private void LV20015()
	{
		StartTime = 860;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			SetupTime = new Vector2(13f, 15f);
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			BigWaveNum = new List<int> { 4, 7 };
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.Crocodile,
					ZombieType.SwampDoor,
					ZombieType.SwampGargantuar
				},
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.Crocodile,
					ZombieType.SwampGargantuar
				}
			};
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 2, 4, 12, 8, 12, 16 },
					new List<int> { 0, 1, 3, 4, 12, 8, 12, 16 }
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 2, 4, 12, 12, 16, 26 },
					new List<int> { 1, 2, 3, 4, 18, 14, 18, 28 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket
				};
			}
			MapOverAction = () =>
			{
				MapManager.Instance.mapList[0].fog.CreateFog(4);
				MapManager.Instance.mapList[1].fog.CreateFog(4);
			};
			TimeAction.Add(982, () =>
			{
				MapManager.Instance.mapList[0].fog.MoveIn();
				MapManager.Instance.mapList[1].fog.MoveIn();
			});
		}
	}

	private void LV20016()
	{
		if (IsEasy)
		{
			StartTime = 900;
		}
		else
		{
			StartTime = 1000;
		}
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 2, 0)
		};
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			SetupTime = new Vector2(13f, 15f);
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SlimeZombie
			};
			BigWaveNum = new List<int> { 5 };
			if (IsEasy)
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 4, 8, 18 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.SwampBucket,
						ZombieType.SlimeZombie,
						ZombieType.SwampDoorAndBucket
					}
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 5, 8, 10, 24 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SlimeZombie,
					ZombieType.SlimeZombie,
					ZombieType.SwampBucket
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.SwampBucket,
						ZombieType.SlimeZombie,
						ZombieType.SwampGargantuar,
						ZombieType.Ghost
					}
				};
			}
		}
	}

	private void LV20017()
	{
		if (IsEasy)
		{
			StartTime = 980;
		}
		else
		{
			StartTime = 1080;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 2, 0),
			new FutureWeather(WeatherType.Rain, 6, 280),
			new FutureWeather(WeatherType.Clear, 0, 710)
		};
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			WaterZombieNum = 2;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SlimeZombie,
				ZombieType.SlimeZombie
			};
			BigWaveNum = new List<int> { 3, 7 };
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 3, 8, 6, 10, 12, 24 },
					new List<int> { 1, 2, 3, 8, 8, 8, 12, 22 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.Ghost,
						ZombieType.SlimeZombie,
						ZombieType.SwampGargantuar
					},
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.StoolZombie,
						ZombieType.SwampBucket,
						ZombieType.Crocodile,
						ZombieType.SlimeZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 10, 8, 10, 16, 28 },
					new List<int> { 2, 4, 6, 10, 8, 12, 18, 30 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampBucket
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.Ghost,
						ZombieType.SlimeZombie,
						ZombieType.SwampGargantuar
					},
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.StoolZombie,
						ZombieType.SwampBucket,
						ZombieType.Ghost,
						ZombieType.Crocodile,
						ZombieType.SlimeZombie,
						ZombieType.SwampGargantuar
					}
				};
			}
		}
	}

	private void LV20018()
	{
		if (IsEasy)
		{
			StartTime = 1020;
		}
		else
		{
			StartTime = 1099;
		}
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			WaterZombieNum = 2;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveNum = new List<int> { 5 };
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 8, 12 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagSwampZombie,
					ZombieType.StarveZombie,
					ZombieType.SwampNormal
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.BucketZombie,
						ZombieType.StarveZombie
					}
				};
			}
			else
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 4, 8, 12, 22 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagSwampZombie,
					ZombieType.StarveZombie,
					ZombieType.StarveZombie
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.StarveZombie,
						ZombieType.SlimeZombie
					}
				};
			}
		}
	}

	private void LV20019()
	{
		if (IsEasy)
		{
			StartTime = 960;
		}
		else
		{
			StartTime = 1080;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 1, 0),
			new FutureWeather(WeatherType.Rain, 10, 280),
			new FutureWeather(WeatherType.Clear, 0, 710)
		};
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			WaterZombieNum = 2;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveNum = new List<int> { 4, 7 };
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 4, 10, 6, 8, 20 },
					new List<int> { 1, 1, 3, 4, 12, 8, 10, 16 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.Ghost,
						ZombieType.StarveZombie,
						ZombieType.SwampBucket
					},
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.StoolZombie,
						ZombieType.SwampGargantuar,
						ZombieType.SlimeZombie,
						ZombieType.StarveZombie
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.StarveZombie
				};
			}
			else
			{
				LvNormalSunNum = 150;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 8, 14, 26 },
					new List<int> { 2, 2, 4, 8, 15, 8, 15, 26 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.Ghost,
						ZombieType.StarveZombie,
						ZombieType.SlimeZombie,
						ZombieType.SwampGargantuar
					},
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.StoolZombie,
						ZombieType.SwampGargantuar,
						ZombieType.Ghost,
						ZombieType.SlimeZombie,
						ZombieType.StarveZombie
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.StarveZombie,
					ZombieType.StarveZombie
				};
			}
		}
	}

	private void LV20020()
	{
		if (IsEasy)
		{
			StartTime = 960;
		}
		else
		{
			StartTime = 1080;
		}
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 6, 0),
			new FutureWeather(WeatherType.Rain, 10, 280)
		};
		if (!OnlyInfo)
		{
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			WaterZombieNum = 2;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveNum = new List<int> { 4, 7 };
			if (IsEasy)
			{
				LvNormalSunNum = 1050;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 3, 6, 8, 18, 14, 20, 25 },
					new List<int> { 2, 3, 4, 8, 16, 12, 20, 25 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.StarveZombie,
						ZombieType.SlimeZombie,
						ZombieType.SwampBucket
					},
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.SwampDoor,
						ZombieType.SlimeZombie,
						ZombieType.StarveZombie,
						ZombieType.SwampGargantuar
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.StarveZombie
				};
			}
			else
			{
				LvNormalSunNum = 650;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 4, 8, 12, 22, 18, 24, 36 },
					new List<int> { 2, 4, 6, 12, 25, 18, 25, 36 }
				};
				ZombieTypes = new List<List<ZombieType>>
				{
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.SwampDoor,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.StarveZombie,
						ZombieType.Crocodile,
						ZombieType.SlimeZombie,
						ZombieType.SwampGargantuar
					},
					new List<ZombieType>
					{
						ZombieType.SwampNormal,
						ZombieType.StoolZombie,
						ZombieType.Ghost,
						ZombieType.SlimeZombie,
						ZombieType.Crocodile,
						ZombieType.StarveZombie,
						ZombieType.SwampGargantuar
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.StarveZombie,
					ZombieType.StarveZombie
				};
			}
		}
	}

	private void LV20021()
	{
		StartTime = 900;
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 3, 0),
			new FutureWeather(WeatherType.Thunder, 10, 335)
		};
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			if (IsEasy)
			{
				LvNormalSunNum = 400;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 18, 12, 24, 12, 16, 24 }
				};
			}
			else
			{
				LvNormalSunNum = 300;
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 8, 8, 24, 18, 28, 14, 20, 30 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal
				};
			}
			SetupTime = new Vector2(10f, 12f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StarveZombie,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.Crocodile,
					ZombieType.SwampGargantuar
				}
			};
			BigWaveNum = new List<int> { 4, 6, 9 };
		}
	}

	private void LV20022()
	{
		StartTime = 920;
		LoadMapTypes = new List<MapType> { MapType.FrontSwamp };
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			if (IsEasy)
			{
				Weights = new List<List<int>>
				{
					new List<int> { 1, 1, 4, 6, 8, 12, 20, 28 }
				};
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 8, 12, 18, 20, 32 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal
				};
			}
			LvNormalSunNum = 300;
			SetupTime = new Vector2(10f, 12f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.SlimeZombie,
					ZombieType.Crocodile,
					ZombieType.SwampGargantuarBlueEye
				}
			};
			BigWaveNum = new List<int> { 7 };
		}
	}

	private void LV20023()
	{
		StartTime = 940;
		LoadMapTypes = new List<MapType> { MapType.BackSwamp };
		if (!OnlyInfo)
		{
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			if (IsEasy)
			{
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 4, 6, 18, 12, 24, 12, 12, 26 }
				};
			}
			else
			{
				Weights = new List<List<int>>
				{
					new List<int> { 2, 2, 8, 8, 24, 18, 28, 14, 26, 35 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal
				};
			}
			LvNormalSunNum = 300;
			SetupTime = new Vector2(10f, 12f);
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampDoorAndStool,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.StarveZombie,
					ZombieType.SlimeZombie,
					ZombieType.Ghost,
					ZombieType.SwampGargantuarBlueEye
				}
			};
			BigWaveNum = new List<int> { 4, 6, 9 };
		}
	}

	private void LV20024()
	{
		StartTime = 860;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 10, 0)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.ZombieNote;
			AwardScence.Instance.LoadText("你获得了一个新的道具!", "纸条", "规模更大的战斗即将来袭");
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			BigWaveNum = new List<int> { 5, 8 };
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.Crocodile,
					ZombieType.Ghost,
					ZombieType.Crocodile,
					ZombieType.StarveZombie,
					ZombieType.SlimeZombie,
					ZombieType.SwampGargantuar,
					ZombieType.SwampGargantuarBlueEye
				},
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.Crocodile,
					ZombieType.StarveZombie,
					ZombieType.SlimeZombie,
					ZombieType.SwampGargantuar,
					ZombieType.SwampGargantuarBlueEye
				}
			};
			if (IsEasy)
			{
				LvNormalSunNum = 250;
				Weights = new List<List<int>>
				{
					new List<int> { 0, 1, 2, 4, 6, 18, 8, 12, 22 },
					new List<int> { 1, 1, 3, 4, 6, 16, 10, 14, 22 }
				};
			}
			else
			{
				LvNormalSunNum = 100;
				Weights = new List<List<int>>
				{
					new List<int> { 1, 2, 2, 4, 8, 20, 14, 26, 36 },
					new List<int> { 2, 2, 3, 4, 8, 18, 12, 18, 36 }
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal
				};
			}
		}
	}

	private void LV20025()
	{
		StartTime = 960;
		LoadMapTypes = new List<MapType>
		{
			MapType.FrontSwamp,
			MapType.BackSwamp
		};
		LoadWeathers = new List<FutureWeather>
		{
			new FutureWeather(WeatherType.Rain, 5, 0),
			new FutureWeather(WeatherType.Thunder, 10, 275)
		};
		if (!OnlyInfo)
		{
			dayBgm = BgmType.SwampDay;
			nightBgm = BgmType.SwampNight;
			BootySprite = BootySprite.MoneyBag;
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
			FirstBootyEvent = () =>
			{
				PlayerManager.Instance.Money += 1500;
			};
			WaterZombieNum = 4;
			WaterZombie = new List<ZombieType>
			{
				ZombieType.SwampNormal,
				ZombieType.SwampBucket,
				ZombieType.StoolZombie
			};
			SetupTime = new Vector2(13f, 15f);
			BigWaveFixedZombie = new List<ZombieType>
			{
				ZombieType.FlagSwampZombie,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal,
				ZombieType.SwampNormal
			};
			BigWaveNum = new List<int> { 5, 8, 11 };
			ZombieTypes = new List<List<ZombieType>>
			{
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.SwampDoorAndStool,
					ZombieType.Crocodile,
					ZombieType.SwampDoor,
					ZombieType.Ghost,
					ZombieType.StarveZombie,
					ZombieType.SlimeZombie,
					ZombieType.SwampGargantuar,
					ZombieType.SwampGargantuarBlueEye
				},
				new List<ZombieType>
				{
					ZombieType.SwampNormal,
					ZombieType.SwampBucket,
					ZombieType.SwampDoor,
					ZombieType.SwampDoorAndBucket,
					ZombieType.StoolZombie,
					ZombieType.Ghost,
					ZombieType.StarveZombie,
					ZombieType.SlimeZombie,
					ZombieType.Crocodile,
					ZombieType.SwampGargantuar,
					ZombieType.SwampGargantuarBlueEye
				}
			};
			if (IsEasy)
			{
				LvNormalSunNum = 700;
				Weights = new List<List<int>>
				{
					new List<int>
					{
						0, 1, 2, 4, 6, 12, 8, 16, 20, 16,
						18, 34
					},
					new List<int>
					{
						1, 1, 3, 4, 6, 12, 8, 12, 18, 16,
						24, 32
					}
				};
			}
			else
			{
				LvNormalSunNum = 500;
				Weights = new List<List<int>>
				{
					new List<int>
					{
						2, 2, 4, 4, 12, 20, 14, 18, 24, 26,
						24, 40
					},
					new List<int>
					{
						1, 2, 3, 4, 10, 18, 14, 16, 26, 26,
						34, 48
					}
				};
				BigWaveFixedZombie = new List<ZombieType>
				{
					ZombieType.FlagBucketSwampZombie,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal,
					ZombieType.SwampNormal
				};
			}
		}
	}
}
