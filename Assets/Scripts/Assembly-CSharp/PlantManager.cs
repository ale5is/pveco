using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
	public static PlantManager Instance;

	public List<PlantBase> plants = new List<PlantBase>();

	public bool PlantInvincible;

	public bool PlantDontSleep;

	private List<PlantType> AllType = new List<PlantType>();

	public List<WallNut> RollNuts = new List<WallNut>();

	private List<PlantType> ZombiePlantType = new List<PlantType> { PlantType.MoonTombStone };

	private void Start()
	{
		foreach (PlantType value in Enum.GetValues(typeof(PlantType)))
		{
			AllType.Add(value);
		}
		AllType.Remove(PlantType.Nope);
		AllType.Remove(PlantType.Hepatica);
		for (int i = 0; i < ZombiePlantType.Count; i++)
		{
			AllType.Remove(ZombiePlantType[i]);
		}
	}

	public void LvReset()
	{
		RollNuts.Clear();
		List<PlantBase> list = new List<PlantBase>(plants);
		for (int i = 0; i < list.Count; i++)
		{
			UnityEngine.Object.Destroy(list[i].gameObject);
		}
		plants.Clear();
	}

	public void PlantDeadRemove(PlantBase plant)
	{
		plants.Remove(plant);
		PoolManager.Instance.PushObj(GetPlantByType(plant.GetPlantType()), plant.gameObject);
	}

	public void ResetAllSpeedRate()
	{
		for (int i = 0; i < plants.Count; i++)
		{
			plants[i].ResetSpeedRate();
		}
	}

	public void AllCheckWeather()
	{
		for (int i = 0; i < plants.Count; i++)
		{
			plants[i].WeatherChangeEvent();
		}
	}

	public void GameOverPause()
	{
		for (int i = 0; i < plants.Count; i++)
		{
			plants[i].GameOverFakeDeath();
		}
	}

	public PlantType GetRandomType()
	{
		return AllType[UnityEngine.Random.Range(0, AllType.Count)];
	}

	public void KillAllPlant()
	{
		List<PlantBase> list = new List<PlantBase>(plants);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
		}
		plants.Clear();
	}

	public int ClearMapPlant(MapBase map)
	{
		int num = 0;
		List<PlantBase> list = new List<PlantBase>(plants);
		for (int i = 0; i < list.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(list[i].transform.position) == map)
			{
				num++;
				list[i].Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			}
		}
		return num;
	}

	private void Awake()
	{
		Instance = this;
	}

	public PlantBase OnlineGetPlant(int onlineId)
	{
		for (int i = 0; i < plants.Count; i++)
		{
			if (plants[i].OnlineId == onlineId)
			{
				return plants[i];
			}
		}
		return null;
	}

	public bool IsZombiePlant(PlantType type)
	{
		return ZombiePlantType.Contains(type);
	}

	public PlantBase GetNewPlant(PlantType type)
	{
		if (type == PlantType.Nope)
		{
			return null;
		}
		PlantBase component = PoolManager.Instance.GetObj(GetPlantByType(type)).GetComponent<PlantBase>();
		if (type == PlantType.ExplodeNut || type == PlantType.HugeNut || type == PlantType.WallNut)
		{
			component.GetComponent<WallNut>().SetType(type);
		}
		if (type == PlantType.Repeater || type == PlantType.RepeaterReverse)
		{
			component.GetComponent<Repeater>().SetType(type);
		}
		component.PlacePlayer = null;
		component.transform.SetParent(base.transform);
		component.SetPlantType(type);
		return component;
	}

	private GameObject GetPlantByType(PlantType type)
	{
		return type switch
		{
			PlantType.SunFlower => GameManager.Instance.GameConf.SunFlower, 
			PlantType.PeaShooter => GameManager.Instance.GameConf.PeaShooter, 
			PlantType.Cherry => GameManager.Instance.GameConf.Cherry, 
			PlantType.WallNut => GameManager.Instance.GameConf.WallNut, 
			PlantType.Tallnut => GameManager.Instance.GameConf.Tallnut, 
			PlantType.Lilypad => GameManager.Instance.GameConf.Lilypad, 
			PlantType.Spike => GameManager.Instance.GameConf.Spike, 
			PlantType.Repeater => GameManager.Instance.GameConf.Repeater, 
			PlantType.Torchwood => GameManager.Instance.GameConf.Torchwood, 
			PlantType.Jalapeno => GameManager.Instance.GameConf.Jalapeno, 
			PlantType.Chomper => GameManager.Instance.GameConf.Chomper, 
			PlantType.SnowPea => GameManager.Instance.GameConf.SnowpeaShooter, 
			PlantType.PotatoMine => GameManager.Instance.GameConf.PotatoMine, 
			PlantType.Squash => GameManager.Instance.GameConf.Squash, 
			PlantType.Tanglekelp => GameManager.Instance.GameConf.Tanglekelp, 
			PlantType.ThreePeater => GameManager.Instance.GameConf.ThreePeater, 
			PlantType.PuffShroom => GameManager.Instance.GameConf.PuffShroom, 
			PlantType.SunShroom => GameManager.Instance.GameConf.SunShroom, 
			PlantType.FumeShroom => GameManager.Instance.GameConf.FumeShroom, 
			PlantType.Gravebuster => GameManager.Instance.GameConf.Gravebuster, 
			PlantType.HypnoShroom => GameManager.Instance.GameConf.HypnoShroom, 
			PlantType.ScaredyShroom => GameManager.Instance.GameConf.ScaredyShroom, 
			PlantType.IceShroom => GameManager.Instance.GameConf.IceShroom, 
			PlantType.DoomShroom => GameManager.Instance.GameConf.DoomShroom, 
			PlantType.Blover => GameManager.Instance.GameConf.Blover, 
			PlantType.SeaShroom => GameManager.Instance.GameConf.SeaShroom, 
			PlantType.Pot => GameManager.Instance.GameConf.Pot, 
			PlantType.Plantern => GameManager.Instance.GameConf.Plantern, 
			PlantType.GatlingPea => GameManager.Instance.GameConf.GatlingPea, 
			PlantType.TwinSunflower => GameManager.Instance.GameConf.TwinSunflower, 
			PlantType.SpikeRock => GameManager.Instance.GameConf.SpikeRock, 
			PlantType.Marigold => GameManager.Instance.GameConf.Marigold, 
			PlantType.SplitPea => GameManager.Instance.GameConf.SplitPea, 
			PlantType.Cabbagepult => GameManager.Instance.GameConf.Cabbagepult, 
			PlantType.Cornpult => GameManager.Instance.GameConf.Cornpult, 
			PlantType.Melonpult => GameManager.Instance.GameConf.Melonpult, 
			PlantType.Wintermelonpult => GameManager.Instance.GameConf.Wintermelonpult, 
			PlantType.GloomShroom => GameManager.Instance.GameConf.GloomShroom, 
			PlantType.Magnetshroom => GameManager.Instance.GameConf.Magnetshroom, 
			PlantType.GoldMagnet => GameManager.Instance.GameConf.GoldMagnet, 
			PlantType.Coffeebean => GameManager.Instance.GameConf.Coffeebean, 
			PlantType.Pumpkin => GameManager.Instance.GameConf.Pumpkin, 
			PlantType.Cactus => GameManager.Instance.GameConf.Cactus, 
			PlantType.Starfruit => GameManager.Instance.GameConf.Starfruit, 
			PlantType.Umbrellaleaf => GameManager.Instance.GameConf.Umbrellaleaf, 
			PlantType.Garlic => GameManager.Instance.GameConf.Garlic, 
			PlantType.Cattail => GameManager.Instance.GameConf.Cattail, 
			PlantType.CobCannon => GameManager.Instance.GameConf.CobCannon, 
			PlantType.Mint => GameManager.Instance.GameConf.Mint, 
			PlantType.Heronsbill => GameManager.Instance.GameConf.Heronsbill, 
			PlantType.SnowRepeater => GameManager.Instance.GameConf.SnowRepeater, 
			PlantType.Hepatica => GameManager.Instance.GameConf.Hepatica, 
			PlantType.Imitater => GameManager.Instance.GameConf.Imitater, 
			PlantType.Clematis => GameManager.Instance.GameConf.Clematis, 
			PlantType.Glowstarfruit => GameManager.Instance.GameConf.Glowstarfruit, 
			PlantType.RottenImitater => GameManager.Instance.GameConf.RottenImitater, 
			PlantType.ExplodeNut => GameManager.Instance.GameConf.WallNut, 
			PlantType.HugeNut => GameManager.Instance.GameConf.WallNut, 
			PlantType.RepeaterReverse => GameManager.Instance.GameConf.Repeater, 
			PlantType.MeltTorch => GameManager.Instance.GameConf.MeltTorch, 
			PlantType.JellyShroom => GameManager.Instance.GameConf.JellyShroom, 
			PlantType.MoonTombStone => GameManager.Instance.GameConf.MoonTombStone, 
			_ => null, 
		};
	}

	public void LoadLvStartPlant()
	{
		if (GameManager.Instance.isClient || LV.Instance.StartPlants.Count == 0)
		{
			return;
		}
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapBase map = MapManager.Instance.mapList[i];
			if (LV.Instance.StartPlants.Count <= i)
			{
				break;
			}
			List<List<PlantType>> list = new List<List<PlantType>>(LV.Instance.StartPlants[i]);
			if (LV.Instance.CurrLVType == LVType.IZombie)
			{
				list.Shuffle();
			}
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = 0; k < list[j].Count; k++)
				{
					Grid thisGrid = MapManager.Instance.GetThisGrid(map, k, j);
					if (thisGrid == null)
					{
						continue;
					}
					PlantBase newPlant = GetNewPlant(list[j][k]);
					if (newPlant != null)
					{
						if (SeedBank.Instance.CheckPlant(newPlant, thisGrid, -2, null))
						{
							SeedBank.Instance.PlantConfirm(newPlant, thisGrid, -2, 3, null);
						}
						else
						{
							UnityEngine.Object.Destroy(newPlant.gameObject);
						}
					}
				}
			}
		}
	}

	public string GetPlantName(PlantType plantType)
	{
		string result = "???";
		switch (plantType)
		{
		case PlantType.SunFlower:
			result = "向日葵";
			break;
		case PlantType.PeaShooter:
			result = "豌豆射手";
			break;
		case PlantType.WallNut:
			result = "坚果墙";
			break;
		case PlantType.Cherry:
			result = "樱桃炸弹";
			break;
		case PlantType.Tallnut:
			result = "高坚果";
			break;
		case PlantType.Lilypad:
			result = "睡莲";
			break;
		case PlantType.Spike:
			result = "地刺";
			break;
		case PlantType.Repeater:
			result = "双发射手";
			break;
		case PlantType.Torchwood:
			result = "火炬树桩";
			break;
		case PlantType.Jalapeno:
			result = "火爆辣椒";
			break;
		case PlantType.SnowPea:
			result = "寒冰射手";
			break;
		case PlantType.PotatoMine:
			result = "土豆雷";
			break;
		case PlantType.Squash:
			result = "窝瓜";
			break;
		case PlantType.Chomper:
			result = "大嘴花";
			break;
		case PlantType.Tanglekelp:
			result = "缠绕海草";
			break;
		case PlantType.ThreePeater:
			result = "三线射手";
			break;
		case PlantType.PuffShroom:
			result = "小喷菇";
			break;
		case PlantType.SunShroom:
			result = "阳光菇";
			break;
		case PlantType.FumeShroom:
			result = "大喷菇";
			break;
		case PlantType.Gravebuster:
			result = "墓碑吞噬者";
			break;
		case PlantType.HypnoShroom:
			result = "魅惑菇";
			break;
		case PlantType.ScaredyShroom:
			result = "胆小菇";
			break;
		case PlantType.IceShroom:
			result = "寒冰菇";
			break;
		case PlantType.DoomShroom:
			result = "毁灭菇";
			break;
		case PlantType.Blover:
			result = "三叶草";
			break;
		case PlantType.SeaShroom:
			result = "海兵菇";
			break;
		case PlantType.Pot:
			result = "花盆";
			break;
		case PlantType.Plantern:
			result = "路灯花";
			break;
		case PlantType.GatlingPea:
			result = "机枪射手";
			break;
		case PlantType.TwinSunflower:
			result = "双子向日葵";
			break;
		case PlantType.SpikeRock:
			result = "地刺王";
			break;
		case PlantType.Marigold:
			result = "金盏花";
			break;
		case PlantType.SplitPea:
			result = "裂荚射手";
			break;
		case PlantType.Cabbagepult:
			result = "卷心菜投手";
			break;
		case PlantType.Cornpult:
			result = "玉米投手";
			break;
		case PlantType.Melonpult:
			result = "西瓜投手";
			break;
		case PlantType.Wintermelonpult:
			result = "冰瓜投手";
			break;
		case PlantType.GloomShroom:
			result = "忧郁菇";
			break;
		case PlantType.Magnetshroom:
			result = "磁力菇";
			break;
		case PlantType.GoldMagnet:
			result = "吸金磁";
			break;
		case PlantType.Coffeebean:
			result = "咖啡豆";
			break;
		case PlantType.Pumpkin:
			result = "南瓜头";
			break;
		case PlantType.Cactus:
			result = "仙人掌";
			break;
		case PlantType.Starfruit:
			result = "杨桃";
			break;
		case PlantType.Umbrellaleaf:
			result = "叶子保护伞";
			break;
		case PlantType.Garlic:
			result = "大蒜";
			break;
		case PlantType.Cattail:
			result = "猫尾草";
			break;
		case PlantType.CobCannon:
			result = "玉米加农炮";
			break;
		case PlantType.Mint:
			result = "薄荷";
			break;
		case PlantType.Heronsbill:
			result = "太阳花";
			break;
		case PlantType.SnowRepeater:
			result = "极冻射手";
			break;
		case PlantType.Hepatica:
			result = "雪割草";
			break;
		case PlantType.Imitater:
			result = "模仿者";
			break;
		case PlantType.MoonTombStone:
			result = "月光墓碑";
			break;
		case PlantType.Clematis:
			result = "铁线莲";
			break;
		case PlantType.Glowstarfruit:
			result = "荧光杨桃";
			break;
		case PlantType.RottenImitater:
			result = "腐烂模仿者";
			break;
		case PlantType.ExplodeNut:
			result = "爆炸坚果";
			break;
		case PlantType.HugeNut:
			result = "巨型坚果";
			break;
		case PlantType.RepeaterReverse:
			result = "反向双发射手";
			break;
		case PlantType.MeltTorch:
			result = "熔融树桩";
			break;
		case PlantType.JellyShroom:
			result = "冻冻菇";
			break;
		}
		return result;
	}
}
