using System;
using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class ZombieManager : MonoBehaviour
{
	public static ZombieManager Instance;

	[SerializeField]
	private List<ZombieBase> zombies = new List<ZombieBase>();

	[SerializeField]
	private List<ZombieBase> hypnoZombies = new List<ZombieBase>();

	private int currOrderNum = 1;

	private UnityAction AllZombieDeadAction;

	private float CreateX = 8.2f;

	public bool ZombieInvincible;

	public bool ZombieDontMove;

	public List<ZombieType> CantBungiSky = new List<ZombieType>
	{
		ZombieType.DiggerZombie,
		ZombieType.BungiZombie,
		ZombieType.HeavyBungiZombie
	};

	private List<ZombieType> Gargantuar = new List<ZombieType>
	{
		ZombieType.GargantuarInjured,
		ZombieType.Gargantuar,
		ZombieType.GargantuarHelmet,
		ZombieType.GargantuarRedeye,
		ZombieType.GargantuarHelmetRedeye,
		ZombieType.SwampGargantuarBlueEye,
		ZombieType.SwampGargantuar,
		ZombieType.SwampGargantuar,
		ZombieType.SwampGargantuarNoCro,
		ZombieType.SwampGargantuarBlueEyeNoCro
	};

	private List<ZombieType> RottenZombieType = new List<ZombieType>();

	public int CurrOrderNum
	{
		get
		{
			currOrderNum++;
			if (currOrderNum > 90)
			{
				currOrderNum = 1;
			}
			return currOrderNum;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Yell();
		foreach (ZombieType value in Enum.GetValues(typeof(ZombieType)))
		{
			RottenZombieType.Add(value);
		}
		List<ZombieType> list = new List<ZombieType>
		{
			ZombieType.SwampGargantuarBlueEyeNoCro,
			ZombieType.SwampGargantuarNoCro,
			ZombieType.GargantuarHelmetRedeye,
			ZombieType.TubeBucketZombie,
			ZombieType.TubeConeZombie,
			ZombieType.TubeDoorZombie,
			ZombieType.BobsledZombieHelmetSled,
			ZombieType.BobsledZombieSled,
			ZombieType.BungiZombie,
			ZombieType.FlagBucketSwampZombie
		};
		RottenZombieType.Remove(ZombieType.Nope);
		RottenZombieType.Remove(ZombieType.PvPTarget);
		RottenZombieType.Remove(ZombieType.IZombieBrain);
		for (int i = 0; i < list.Count; i++)
		{
			RottenZombieType.Remove(list[i]);
		}
	}

	public ZombieBase OnlineGetZombie(int onlineId)
	{
		List<ZombieBase> list = new List<ZombieBase>(GetAllZombies());
		list.AddRange(GetAllHypZombies());
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].OnlineId == onlineId)
			{
				return list[i];
			}
		}
		return null;
	}

	public int GetZombieNum()
	{
		return zombies.Count;
	}

	public bool IsGargantuar(ZombieType type)
	{
		return Gargantuar.Contains(type);
	}

	public ZombieType RottenGetZombieType()
	{
		return RottenZombieType[UnityEngine.Random.Range(0, RottenZombieType.Count)];
	}

	public void ResetAllSpeedRate()
	{
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].ResetSpeedRate();
		}
		for (int j = 0; j < hypnoZombies.Count; j++)
		{
			hypnoZombies[j].ResetSpeedRate();
		}
	}

	public void GameOverPause()
	{
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].GameOverFakeDeath();
		}
		for (int j = 0; j < hypnoZombies.Count; j++)
		{
			hypnoZombies[j].GameOverFakeDeath();
		}
	}

	public void ClearAllZombie()
	{
		AllZombieDeadAction = null;
		while (zombies.Count > 0)
		{
			zombies[0].DirectDead(canDropItem: false, 0f, synClient: true);
		}
		while (hypnoZombies.Count > 0)
		{
			hypnoZombies[0].DirectDead(canDropItem: false, 0f, synClient: true);
		}
		zombies.Clear();
		hypnoZombies.Clear();
	}

	public void LvReset()
	{
		AllZombieDeadAction = null;
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i] != null)
			{
				UnityEngine.Object.Destroy(zombies[i].gameObject);
			}
		}
		for (int j = 0; j < hypnoZombies.Count; j++)
		{
			if (hypnoZombies[j] != null)
			{
				UnityEngine.Object.Destroy(hypnoZombies[j].gameObject);
			}
		}
		zombies.Clear();
		hypnoZombies.Clear();
	}

	public int BigHurtAllZombie()
	{
		List<ZombieBase> list = new List<ZombieBase>(zombies);
		int count = zombies.Count;
		for (int i = 0; i < list.Count; i++)
		{
			zombies[i].Hurt(1000000, Vector2.zero, isHard: false, HitSound: false);
		}
		return count;
	}

	public int BigHurtMapZombie(MapBase map)
	{
		List<ZombieBase> list = new List<ZombieBase>(zombies);
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(zombies[i].transform.position) == map)
			{
				num++;
				zombies[i].Hurt(1000000, Vector2.zero, isHard: false, HitSound: false);
			}
		}
		return num;
	}

	public void ShowZombie()
	{
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			List<Vector2> showZombiePos = MapManager.Instance.mapList[i].GetShowZombiePos(LV.Instance.ZombieTypes[i].Count);
			for (int j = 0; j < LV.Instance.ZombieTypes[i].Count; j++)
			{
				ZombieBase summonZombie = GetSummonZombie(LV.Instance.ZombieTypes[i][j], showZombiePos[j], needSpawnLimit: false);
				if (summonZombie != null)
				{
					AddZombie(summonZombie);
					summonZombie.transform.SetParent(base.transform);
					summonZombie.Init(0, j, showZombiePos[j], MapManager.Instance.mapList[i]);
					if (MapManager.Instance.mapList[i].IsWaterShow)
					{
						summonZombie.DirctInWater();
					}
				}
			}
		}
		ZombieStartIdel();
	}

	public void SummonZombie(int id1, Grid grid)
	{
	}

	public void SummonZombie(int id1, int lineNum, Vector2 mapPos)
	{
		GetPosByGridVertical(mapPos, lineNum, CreateX);
	}

	public bool MapSPZombie(ZombieType Type, Vector2 spawnPos, MapBase map)
	{
		ZombieBase summonZombie = GetSummonZombie(Type, spawnPos, needSpawnLimit: true);
		if (summonZombie != null)
		{
			Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(map, spawnPos);
			if (gridByWorldPos == null)
			{
				return false;
			}
			UpdateZombie(Type, summonZombie, spawnPos, gridByWorldPos.Point.y);
		}
		return summonZombie != null;
	}

	public ZombieBase OutGround(ZombieType Type, Vector3 pos, string placePlayer, bool needArm, bool isHyp, bool purple, bool synClient = false)
	{
		if (!synClient && GameManager.Instance.isClient)
		{
			return null;
		}
		Grid grid = MapManager.Instance.GetGridByWorldPos(pos);
		ZombieBase Azombie = null;
		if (grid != null)
		{
			Azombie = GetNewZombie(Type);
		}
		if (Azombie != null)
		{
			Azombie.PlacePlayer = placePlayer;
			Azombie.transform.SetParent(base.transform);
			Azombie.CreateInit(inGrid: false, grid, isRat: false, 1);
			ZombieOutGround component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZombieOutGround).GetComponent<ZombieOutGround>();
			component.transform.position = new Vector3(pos.x, grid.Position.y);
			AddZombie(Azombie);
			if (isHyp)
			{
				if (purple)
				{
					Azombie.Hypno(synClient: true);
				}
				else
				{
					Azombie.RatThis(synClient: true);
				}
			}
			else if (purple)
			{
				Azombie.PurpleZombie();
			}
			Azombie.transform.SetParent(base.transform);
			if (grid.isNoIceWater)
			{
				Azombie.DirctInWater();
				Azombie.OutInWaterEvent();
				Azombie.WhiteWater.gameObject.SetActive(value: false);
			}
			component.CreateInit(grid, Azombie, () =>
			{
				Azombie.SpawnPos = new Vector3(pos.x, grid.Position.y);
				Azombie.Init(grid.Point.y, CurrOrderNum, new Vector3(pos.x, grid.Position.y), IsFirst: false);
				if (grid.isNoIceWater)
				{
					Azombie.DirctInWater();
				}
				if (isHyp)
				{
					hypnoZombies.Remove(Azombie);
					AddZombie(Azombie);
					if (purple)
					{
						Azombie.Hypno(synClient: true);
					}
					else
					{
						Azombie.RatThis(synClient: true);
					}
				}
				else if (purple)
				{
					Azombie.PurpleZombie();
				}
			}, needArm);
			component.transform.SetParent(base.transform);
			int[] array = new int[4] { 1, 0, 0, 0 };
			if (needArm)
			{
				array[1] = 1;
			}
			if (isHyp)
			{
				array[2] = 1;
			}
			if (purple)
			{
				array[3] = 1;
			}
			ServerSendZombie(Azombie, Type, pos, grid.Point.y, array);
		}
		return Azombie;
	}

	public ZombieBase OutBungiSky(ZombieType Type, Vector3 pos, string placePlayer, bool isHyp, bool purple, int onlineId = 0, bool synClient = false)
	{
		if (!synClient && GameManager.Instance.isClient)
		{
			return null;
		}
		ZombieBase azombie = null;
		if (MapManager.Instance.GetGridByWorldPos(pos) != null)
		{
			azombie = GetNewZombie(Type);
		}
		return OutBungiSky(Type, azombie, pos, placePlayer, isHyp, purple, onlineId, synClient);
	}

	public ZombieBase OutBungiSky(ZombieType Type, ZombieBase Azombie, Vector3 pos, string placePlayer, bool isHyp, bool purple, int onlineId = 0, bool synClient = false)
	{
		if (!synClient && GameManager.Instance.isClient)
		{
			return null;
		}
		if (Azombie != null)
		{
			Grid grid = MapManager.Instance.GetGridByWorldPos(pos);
			Azombie.PlacePlayer = placePlayer;
			Azombie.CreateInit(inGrid: false, grid, isRat: false, 1);
			AddZombie(Azombie);
			List<ZombieType> obj = new List<ZombieType>
			{
				ZombieType.Gargantuar,
				ZombieType.GargantuarHelmet,
				ZombieType.GargantuarHelmetRedeye,
				ZombieType.GargantuarRedeye,
				ZombieType.GargantuarInjured,
				ZombieType.RoadrollerZombie
			};
			BungiZombie component = GetSummonZombie(ZombieType.BungiZombie, grid.Position, needSpawnLimit: false).GetComponent<BungiZombie>();
			if (obj.Contains(Type))
			{
				component.Type = BungiZombieType.Heavy;
			}
			else
			{
				component.Type = BungiZombieType.Normal;
			}
			component.Init(grid.Point.y, CurrOrderNum, grid.Position);
			AddZombie(component);
			if (GameManager.Instance.isClient)
			{
				component.OnlineId = onlineId - Azombie.OnlineIdNum;
				Azombie.OnlineId = onlineId;
			}
			else if (GameManager.Instance.isServer)
			{
				component.OnlineId = SocketServer.Instance.ItemId;
			}
			if (isHyp)
			{
				if (purple)
				{
					Azombie.Hypno(synClient: true);
				}
				else
				{
					Azombie.RatThis(synClient: true);
				}
			}
			else if (purple)
			{
				Azombie.PurpleZombie();
			}
			Azombie.transform.SetParent(base.transform);
			Azombie.transform.position = new Vector3(component.transform.position.x + 0.25f, component.transform.position.y - 0.25f);
			component.DropInit(Azombie, () =>
			{
				Azombie.SpawnPos = new Vector3(pos.x, grid.Position.y);
				Azombie.Init(grid.Point.y, CurrOrderNum, new Vector2(Azombie.transform.position.x, grid.Position.y), IsFirst: false);
				if (isHyp)
				{
					hypnoZombies.Remove(Azombie);
					AddZombie(Azombie);
					if (purple)
					{
						Azombie.Hypno(synClient: true);
					}
					else
					{
						Azombie.RatThis(synClient: true);
					}
				}
				else if (purple)
				{
					Azombie.PurpleZombie();
				}
			});
			int[] array = new int[4] { 2, 0, 0, 0 };
			if (isHyp)
			{
				array[2] = 1;
			}
			if (purple)
			{
				array[3] = 1;
			}
			ServerSendZombie(Azombie, Type, pos, grid.Point.y, array);
			if (GameManager.Instance.isServer)
			{
				component.ServerInitInfo();
			}
		}
		return Azombie;
	}

	private void ServerSendZombie(ZombieBase zombie, ZombieType zombieType, Vector2 pos, int UpdateLine, int[] SPcode)
	{
		if (GameManager.Instance.isServer)
		{
			ZombieSpawn zombieSpawn = new ZombieSpawn();
			zombieSpawn.SpawnPos = pos;
			zombieSpawn.SpCode = SPcode;
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(zombie.PlacePlayer))
			{
				zombieSpawn.SpawnPos = new Vector2(0f - zombieSpawn.SpawnPos.x, zombieSpawn.SpawnPos.y);
			}
			for (int i = 0; i < zombie.OnlineIdNum; i++)
			{
				zombieSpawn.OnlineId = SocketServer.Instance.ItemId;
			}
			zombie.OnlineId = zombieSpawn.OnlineId;
			zombieSpawn.PlacePlayer = zombie.PlacePlayer;
			zombieSpawn.Type = zombieType;
			zombieSpawn.UpdateLine = UpdateLine;
			SocketServer.Instance.SpawnZombie(zombieSpawn);
			zombie.ServerInitInfo();
		}
	}

	public bool UpdateBungiZombieOnRandomLine(ZombieType Type, Vector3 mapPos)
	{
		if (GameManager.Instance.isClient)
		{
			return false;
		}
		int updateLine = MapManager.Instance.GetCurrMap(mapPos).GetRandomLine(0);
		ZombieBase summonZombie = GetSummonZombie(Type, mapPos, needSpawnLimit: true, ref updateLine);
		if (summonZombie != null)
		{
			Grid posByGridVertical = GetPosByGridVertical(mapPos, updateLine, CreateX);
			if (posByGridVertical == null)
			{
				return false;
			}
			List<Grid> lineAllGrid = MapManager.Instance.GetLineAllGrid(mapPos, posByGridVertical.Point.y);
			lineAllGrid.Remove(posByGridVertical);
			List<Grid> list = new List<Grid>(lineAllGrid);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Point.x < 4)
				{
					lineAllGrid.Remove(list[i]);
				}
			}
			OutBungiSky(Type, summonZombie, lineAllGrid[UnityEngine.Random.Range(0, lineAllGrid.Count)].Position, "", isHyp: false, purple: false);
		}
		return true;
	}

	public bool UpdateZombieOnRandomLine(ZombieType Type, Vector3 mapPos, int spawnCode = 0)
	{
		if (GameManager.Instance.isClient)
		{
			return false;
		}
		int updateLine = MapManager.Instance.GetCurrMap(mapPos).GetRandomLine(spawnCode);
		int num = updateLine;
		ZombieBase zombieBase = GetSummonZombie(Type, mapPos, needSpawnLimit: true, ref updateLine);
		if (spawnCode == 1 && updateLine != num && zombieBase != null)
		{
			UnityEngine.Object.Destroy(zombieBase.gameObject);
			zombieBase = null;
		}
		if (zombieBase != null)
		{
			Grid posByGridVertical = GetPosByGridVertical(mapPos, updateLine, CreateX);
			if (posByGridVertical == null)
			{
				return false;
			}
			UpdateZombie(Type, zombieBase, new Vector2(CreateX, posByGridVertical.Position.y), updateLine);
		}
		return zombieBase != null;
	}

	public ZombieBase UpdateZombie(ZombieType Type, Vector2 pos)
	{
		if (GameManager.Instance.isClient)
		{
			return null;
		}
		ZombieBase newZombie = GetNewZombie(Type);
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(pos);
		if (newZombie != null)
		{
			UpdateZombie(Type, newZombie, pos, gridByWorldPos.Point.y);
		}
		return newZombie;
	}

	public bool UpdateZombie(ZombieType Type, ZombieBase zombie, Vector2 pos, int UpdateLine)
	{
		if (GameManager.Instance.isClient)
		{
			return false;
		}
		if (zombie != null)
		{
			AddZombie(zombie);
			zombie.transform.SetParent(base.transform);
			zombie.Init(UpdateLine, CurrOrderNum, pos);
			ServerSendZombie(zombie, Type, pos, UpdateLine, new int[4] { 0, 0, 0, 0 });
		}
		return zombie != null;
	}

	public bool UpdateZombie(ZombieSpawn spawnInfo)
	{
		ZombieBase zombieBase = null;
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(spawnInfo.PlacePlayer))
		{
			spawnInfo.SpawnPos = MyTool.ReverseX(spawnInfo.SpawnPos);
		}
		if (spawnInfo.SpCode[0] == 0)
		{
			int updateLine = 0;
			zombieBase = GetSummonZombie(spawnInfo.Type, spawnInfo.SpawnPos, needSpawnLimit: false, ref updateLine);
			if (zombieBase != null)
			{
				AddZombie(zombieBase);
				zombieBase.transform.SetParent(base.transform);
				zombieBase.Init(spawnInfo.UpdateLine, CurrOrderNum, spawnInfo.SpawnPos);
				if (LV.Instance.CurrLVType == LVType.PvP && PvPSelector.Instance.IsSameTeam(spawnInfo.PlacePlayer))
				{
					zombieBase.RatThis(synClient: true);
				}
			}
		}
		else if (spawnInfo.SpCode[0] == 1)
		{
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
			{
				if (spawnInfo.SpCode[2] == 1)
				{
					spawnInfo.SpCode[2] = 0;
				}
				else if (spawnInfo.SpCode[2] == 0)
				{
					spawnInfo.SpCode[2] = 1;
				}
			}
			zombieBase = OutGround(spawnInfo.Type, spawnInfo.SpawnPos, spawnInfo.PlacePlayer, spawnInfo.SpCode[1] == 1, spawnInfo.SpCode[2] == 1, spawnInfo.SpCode[3] == 1, synClient: true);
		}
		else if (spawnInfo.SpCode[0] == 2)
		{
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
			{
				if (spawnInfo.SpCode[2] == 1)
				{
					spawnInfo.SpCode[2] = 0;
				}
				else if (spawnInfo.SpCode[2] == 0)
				{
					spawnInfo.SpCode[2] = 1;
				}
			}
			zombieBase = OutBungiSky(spawnInfo.Type, spawnInfo.SpawnPos, spawnInfo.PlacePlayer, spawnInfo.SpCode[2] == 1, spawnInfo.SpCode[3] == 1, spawnInfo.OnlineId, synClient: true);
		}
		if (zombieBase != null)
		{
			zombieBase.OnlineId = spawnInfo.OnlineId;
			zombieBase.PlacePlayer = spawnInfo.PlacePlayer;
		}
		return zombieBase != null;
	}

	public GameObject CreateOneZombie(GameObject prefab)
	{
		if (prefab == null)
		{
			return null;
		}
		_ = CurrOrderNum;
		ZombieBase component = PoolManager.Instance.GetObj(prefab).GetComponent<ZombieBase>();
		AddZombie(component);
		component.transform.SetParent(base.transform);
		return component.gameObject;
	}

	public GameObject CreateOneZombie(GameObject prefab, int lineNum, Vector2 vector)
	{
		if (prefab == null)
		{
			return null;
		}
		if (MapManager.Instance.GetCurrMap(vector).MapGridNum.y <= lineNum || lineNum < 0)
		{
			return null;
		}
		ZombieBase component = PoolManager.Instance.GetObj(prefab).GetComponent<ZombieBase>();
		AddZombie(component);
		component.transform.SetParent(base.transform);
		component.Init(lineNum, CurrOrderNum, vector);
		return component.gameObject;
	}

	public ZombieBase GetNewZombie(ZombieType Type)
	{
		return GetSummonZombie(Type, default, needSpawnLimit: false);
	}

	private ZombieBase GetSummonZombie(ZombieType Type, Vector2 mapPos, bool needSpawnLimit)
	{
		int updateLine = 0;
		return GetSummonZombie(Type, mapPos, needSpawnLimit, ref updateLine);
	}

	private ZombieBase GetSummonZombie(ZombieType Type, Vector2 mapPos, bool needSpawnLimit, ref int updateLine)
	{
		ZombieBase zombieBase = null;
		switch (Type)
		{
		case ZombieType.PvPTarget:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PvPTarget).GetComponent<ZombieBase>();
			break;
		case ZombieType.IZombieBrain:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.IZombieBrain).GetComponent<ZombieBase>();
			break;
		case ZombieType.NormalZombie:
		{
			Zombie component35 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component35.Type = NormalZombieType.Normal;
			zombieBase = component35;
			break;
		}
		case ZombieType.FlagZombie:
		{
			Zombie component32 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component32.Type = NormalZombieType.Flag;
			zombieBase = component32;
			break;
		}
		case ZombieType.FlagBucketZombie:
		{
			Zombie component24 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component24.Type = NormalZombieType.FlagBucket;
			zombieBase = component24;
			break;
		}
		case ZombieType.TubeZombie:
		{
			Grid waterGrid8 = MapManager.Instance.GetWaterGrid(mapPos);
			Zombie component34 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component34.Type = NormalZombieType.Tube;
			zombieBase = component34;
			if (waterGrid8 != null)
			{
				updateLine = waterGrid8.Point.y;
			}
			break;
		}
		case ZombieType.TubeConeZombie:
		{
			Grid waterGrid6 = MapManager.Instance.GetWaterGrid(mapPos);
			Zombie component33 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component33.Type = NormalZombieType.TubeCone;
			zombieBase = component33;
			if (waterGrid6 != null)
			{
				updateLine = waterGrid6.Point.y;
			}
			break;
		}
		case ZombieType.TubeBucketZombie:
		{
			Grid waterGrid9 = MapManager.Instance.GetWaterGrid(mapPos);
			Zombie component41 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component41.Type = NormalZombieType.TubeBucket;
			zombieBase = component41;
			if (waterGrid9 != null)
			{
				updateLine = waterGrid9.Point.y;
			}
			break;
		}
		case ZombieType.TubeDoorZombie:
		{
			Grid waterGrid4 = MapManager.Instance.GetWaterGrid(mapPos);
			Zombie component27 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component27.Type = NormalZombieType.TubeDoor;
			zombieBase = component27;
			if (waterGrid4 != null)
			{
				updateLine = waterGrid4.Point.y;
			}
			break;
		}
		case ZombieType.TubeDoorConeZombie:
		{
			Grid waterGrid = MapManager.Instance.GetWaterGrid(mapPos);
			Zombie component14 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component14.Type = NormalZombieType.TubeDoorCone;
			zombieBase = component14;
			if (waterGrid != null)
			{
				updateLine = waterGrid.Point.y;
			}
			break;
		}
		case ZombieType.TubeDoorBucketZombie:
		{
			Grid waterGrid3 = MapManager.Instance.GetWaterGrid(mapPos);
			Zombie component26 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component26.Type = NormalZombieType.TubeDoorBucket;
			zombieBase = component26;
			if (waterGrid3 != null)
			{
				updateLine = waterGrid3.Point.y;
			}
			break;
		}
		case ZombieType.BobsledZombie:
		{
			BobsledZombie component43 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
			component43.Type = BobsledType.Normal;
			zombieBase = component43;
			break;
		}
		case ZombieType.BobsledZombieHelmet:
		{
			BobsledZombie component46 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
			component46.Type = BobsledType.Helmet;
			zombieBase = component46;
			break;
		}
		case ZombieType.BobsledZombieSled:
			if (LVManager.Instance.CurrLVState == LVState.Fighting)
			{
				Grid haveIceFirstGrid = MapManager.Instance.GetHaveIceFirstGrid(mapPos);
				if (haveIceFirstGrid != null || !needSpawnLimit)
				{
					if (haveIceFirstGrid != null)
					{
						updateLine = haveIceFirstGrid.Point.y;
					}
					BobsledZombie component36 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
					component36.Type = BobsledType.Sled;
					zombieBase = component36;
				}
			}
			else
			{
				BobsledZombie component37 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
				component37.Type = BobsledType.Sled;
				zombieBase = component37;
			}
			break;
		case ZombieType.BobsledZombieHelmetSled:
			if (LVManager.Instance.CurrLVState == LVState.Fighting)
			{
				Grid haveIceFirstGrid2 = MapManager.Instance.GetHaveIceFirstGrid(mapPos);
				if (haveIceFirstGrid2 != null || !needSpawnLimit)
				{
					if (haveIceFirstGrid2 != null)
					{
						updateLine = haveIceFirstGrid2.Point.y;
					}
					BobsledZombie component44 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
					component44.Type = BobsledType.HelmetAndSled;
					zombieBase = component44;
				}
			}
			else
			{
				BobsledZombie component45 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
				component45.Type = BobsledType.Helmet;
				zombieBase = component45;
			}
			break;
		case ZombieType.ImpZpmbie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ImpZombie).GetComponent<ImpZombie>();
			break;
		case ZombieType.ConeZombie:
		{
			Zombie component42 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component42.Type = NormalZombieType.Cone;
			zombieBase = component42;
			break;
		}
		case ZombieType.Polevaulter:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Polevaulter).GetComponent<Polevaulter>();
			break;
		case ZombieType.PaperZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PaperZombie).GetComponent<PaperZombie>();
			break;
		case ZombieType.DiggerZombie:
		{
			int noWaterGrid7 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: false);
			if (noWaterGrid7 != -1 || !needSpawnLimit)
			{
				DiggerZombie component40 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.DiggerZombie).GetComponent<DiggerZombie>();
				updateLine = noWaterGrid7;
				zombieBase = component40;
			}
			break;
		}
		case ZombieType.BalloonZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BalloonZombie).GetComponent<BalloonZombie>();
			break;
		case ZombieType.JacksonZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.JacksonZombie).GetComponent<JacksonZombie>();
			break;
		case ZombieType.DancerZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.DancerZombie).GetComponent<DancerZombie>();
			break;
		case ZombieType.SnorkleZombie:
		{
			Grid waterGrid7 = MapManager.Instance.GetWaterGrid(mapPos);
			if (waterGrid7 != null || !needSpawnLimit)
			{
				zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnorkleZombie).GetComponent<SnorkleZombie>();
				if (waterGrid7 != null)
				{
					updateLine = waterGrid7.Point.y;
				}
			}
			break;
		}
		case ZombieType.SnorkleZombieHelmet:
		{
			Grid waterGrid5 = MapManager.Instance.GetWaterGrid(mapPos);
			if (waterGrid5 != null || !needSpawnLimit)
			{
				SnorkleZombie component29 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnorkleZombie).GetComponent<SnorkleZombie>();
				component29.Type = SnorkleType.Helmet;
				zombieBase = component29;
				if (waterGrid5 != null)
				{
					updateLine = waterGrid5.Point.y;
				}
			}
			break;
		}
		case ZombieType.LadderZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.LadderZombie).GetComponent<LadderZombie>();
			break;
		case ZombieType.DolphinriderZombie:
		{
			Grid waterGrid2 = MapManager.Instance.GetWaterGrid(mapPos);
			if (waterGrid2 != null || !needSpawnLimit)
			{
				zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.DolphinriderZombie).GetComponent<DolphinriderZombie>();
				if (waterGrid2 != null)
				{
					updateLine = waterGrid2.Point.y;
				}
			}
			break;
		}
		case ZombieType.BucketZombie:
		{
			Zombie component21 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component21.Type = NormalZombieType.Bucket;
			zombieBase = component21;
			break;
		}
		case ZombieType.DoorZombie:
		{
			Zombie component22 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component22.Type = NormalZombieType.Door;
			zombieBase = component22;
			break;
		}
		case ZombieType.Yeti:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Yeti).GetComponent<Yeti>();
			break;
		case ZombieType.JackboxZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.JackboxZombie).GetComponent<JackboxZombie>();
			break;
		case ZombieType.PogoZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PogoZombie).GetComponent<PogoZombie>();
			break;
		case ZombieType.DoorAndCone:
		{
			Zombie component16 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component16.Type = NormalZombieType.DoorAndCone;
			zombieBase = component16;
			break;
		}
		case ZombieType.FootballZombie:
		{
			FootballZombie component13 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie_Football).GetComponent<FootballZombie>();
			component13.Type = FootballZombieType.Normal;
			zombieBase = component13;
			break;
		}
		case ZombieType.Zomboni:
		{
			int noWaterGrid8 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid8 != -1 || !needSpawnLimit)
			{
				zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ZamboniZombie).GetComponent<Zamboni>();
				updateLine = noWaterGrid8;
			}
			break;
		}
		case ZombieType.BungiZombie:
			if (needSpawnLimit)
			{
				Grid havePlantGrid2 = MapManager.Instance.GetHavePlantGrid(mapPos);
				if (havePlantGrid2 != null && !havePlantGrid2.isZombieSigned && havePlantGrid2.CurrPlantBase != null && havePlantGrid2.CurrPlantBase.GetPlantType() != PlantType.CobCannon && havePlantGrid2.Cage == null && (!(havePlantGrid2.CurrPlantBase.CarryPlant != null) || havePlantGrid2.CurrPlantBase.CarryPlant.GetPlantType() != PlantType.CobCannon))
				{
					BungiZombie component38 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BungiZombie).GetComponent<BungiZombie>();
					component38.SpawnGrid = havePlantGrid2;
					component38.Type = BungiZombieType.Normal;
					zombieBase = component38;
				}
			}
			else
			{
				BungiZombie component39 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BungiZombie).GetComponent<BungiZombie>();
				component39.Type = BungiZombieType.Normal;
				updateLine = 0;
				zombieBase = component39;
			}
			break;
		case ZombieType.HeavyBungiZombie:
			if (needSpawnLimit)
			{
				Grid havePlantGrid = MapManager.Instance.GetHavePlantGrid(mapPos);
				if (havePlantGrid != null && !havePlantGrid.isZombieSigned && !havePlantGrid.isNoIceWater && havePlantGrid.CurrPlantBase != null && havePlantGrid.CurrPlantBase.GetPlantType() != PlantType.CobCannon && havePlantGrid.Cage == null && (!(havePlantGrid.CurrPlantBase.CarryPlant != null) || havePlantGrid.CurrPlantBase.CarryPlant.GetPlantType() != PlantType.CobCannon))
				{
					BungiZombie component30 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BungiZombie).GetComponent<BungiZombie>();
					component30.Type = BungiZombieType.Heavy;
					component30.SpawnGrid = havePlantGrid;
					zombieBase = component30;
				}
			}
			else
			{
				BungiZombie component31 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BungiZombie).GetComponent<BungiZombie>();
				component31.Type = BungiZombieType.Heavy;
				updateLine = 0;
				zombieBase = component31;
			}
			break;
		case ZombieType.GargantuarInjured:
		{
			int noWaterGrid6 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid6 != -1 || !needSpawnLimit)
			{
				Gargantuar component28 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component28.Type = GargantuarType.Injured;
				zombieBase = component28;
				updateLine = noWaterGrid6;
			}
			break;
		}
		case ZombieType.CatapultZombie:
		{
			int noWaterGrid5 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid5 != -1 || !needSpawnLimit)
			{
				CatapultZombie component25 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CatapultZombie).GetComponent<CatapultZombie>();
				component25.Type = CatapultZombieType.Normal;
				zombieBase = component25;
				updateLine = noWaterGrid5;
			}
			break;
		}
		case ZombieType.RockCatapultZombie:
		{
			int noWaterGrid4 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid4 != -1 || !needSpawnLimit)
			{
				CatapultZombie component23 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CatapultZombie).GetComponent<CatapultZombie>();
				component23.Type = CatapultZombieType.Rockpult;
				zombieBase = component23;
				updateLine = noWaterGrid4;
			}
			break;
		}
		case ZombieType.RoadrollerZombie:
			if (updateLine > 0 || !needSpawnLimit)
			{
				zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.RoadrollerZombie).GetComponent<RoadrollerZombie>();
			}
			break;
		case ZombieType.DoorAndBucket:
		{
			Zombie component20 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie).GetComponent<Zombie>();
			component20.Type = NormalZombieType.DoorAndBucket;
			zombieBase = component20;
			break;
		}
		case ZombieType.BlackFootball:
		{
			FootballZombie component19 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Zombie_BlackFootball).GetComponent<FootballZombie>();
			component19.Type = FootballZombieType.Black;
			zombieBase = component19;
			break;
		}
		case ZombieType.Gargantuar:
		{
			Gargantuar component18 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
			component18.Type = GargantuarType.Normal;
			zombieBase = component18;
			break;
		}
		case ZombieType.GargantuarRedeye:
		{
			int noWaterGrid3 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid3 != -1 || !needSpawnLimit)
			{
				Gargantuar component17 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component17.Type = GargantuarType.Redeye;
				zombieBase = component17;
				updateLine = noWaterGrid3;
			}
			break;
		}
		case ZombieType.GargantuarHelmet:
		{
			int noWaterGrid2 = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid2 != -1 || !needSpawnLimit)
			{
				Gargantuar component15 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component15.Type = GargantuarType.Helmet;
				zombieBase = component15;
				updateLine = noWaterGrid2;
			}
			break;
		}
		case ZombieType.GargantuarHelmetRedeye:
		{
			int noWaterGrid = MapManager.Instance.GetNoWaterGrid(mapPos, moveOnIce: true);
			if (noWaterGrid != -1 || !needSpawnLimit)
			{
				Gargantuar component12 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Gargantuar).GetComponent<Gargantuar>();
				component12.Type = GargantuarType.RedeyeAndHelmet;
				zombieBase = component12;
				updateLine = noWaterGrid;
			}
			break;
		}
		case ZombieType.SwampNormal:
		{
			SwampZombie component11 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component11.Type = SwampZombieType.Normal;
			zombieBase = component11;
			break;
		}
		case ZombieType.FlagSwampZombie:
		{
			SwampZombie component10 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component10.Type = SwampZombieType.Flag;
			zombieBase = component10;
			break;
		}
		case ZombieType.StoolZombie:
		{
			SwampZombie component9 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component9.Type = SwampZombieType.Stool;
			zombieBase = component9;
			break;
		}
		case ZombieType.SwampBucket:
		{
			SwampZombie component8 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component8.Type = SwampZombieType.Bucket;
			zombieBase = component8;
			break;
		}
		case ZombieType.SwampDoor:
		{
			SwampZombie component7 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component7.Type = SwampZombieType.Door;
			zombieBase = component7;
			break;
		}
		case ZombieType.SwampDoorAndStool:
		{
			SwampZombie component6 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component6.Type = SwampZombieType.DoorAndStool;
			zombieBase = component6;
			break;
		}
		case ZombieType.SwampDoorAndBucket:
		{
			SwampZombie component5 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampZombie).GetComponent<SwampZombie>();
			component5.Type = SwampZombieType.DoorAndBucket;
			zombieBase = component5;
			break;
		}
		case ZombieType.Ghost:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GhostZombie).GetComponent<Ghost>();
			break;
		case ZombieType.Crocodile:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Crocodile).GetComponent<Crocodile>();
			break;
		case ZombieType.StarveZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.StarveZombie).GetComponent<StarveZombie>();
			break;
		case ZombieType.SlimeZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SlimeZombie).GetComponent<SlimeZombie>();
			break;
		case ZombieType.SwampGargantuar:
		{
			SwampGargantuar component4 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampGargantuar).GetComponent<SwampGargantuar>();
			component4.Type = SwampGargantuarType.Normal;
			zombieBase = component4;
			break;
		}
		case ZombieType.SwampGargantuarNoCro:
		{
			SwampGargantuar component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampGargantuar).GetComponent<SwampGargantuar>();
			component3.Type = SwampGargantuarType.NoCrocodile;
			zombieBase = component3;
			break;
		}
		case ZombieType.SwampGargantuarBlueEye:
		{
			SwampGargantuar component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampGargantuar).GetComponent<SwampGargantuar>();
			component2.Type = SwampGargantuarType.Blueeye;
			zombieBase = component2;
			break;
		}
		case ZombieType.SwampGargantuarBlueEyeNoCro:
		{
			SwampGargantuar component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SwampGargantuar).GetComponent<SwampGargantuar>();
			component.Type = SwampGargantuarType.BlueeyeNoCrocodile;
			zombieBase = component;
			break;
		}
		case ZombieType.PeaShooterZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PeaShooterZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.SnowpeaShooterZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnowpeaShooterZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.TorchwoodZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.TorchwoodZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.HyponShroomZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.HyponShroomZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.WallNutZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.WallnutZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.TallNutZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.TallnutZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.RepeaterZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.RepeaterZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.GatlingZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GatlingZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.JalapenoZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.JalapenoZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.IceShroomZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.IceShroomZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.DoomShroomZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.DoomShroomZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.SnowRepeaterZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnowRepeaterZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.FumeShroomZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FumeShroomZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.GloomShroomZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GloomShroomZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.StarfruitZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.StarfruitZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.SunflowerZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SunflowerZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.SquashZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SquashZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.UmbrellaZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.UmbrallaZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.MelonpultZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.WaterMelonZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.WinterMelonZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.WinterMelonZombie).GetComponent<PlantZombie>();
			break;
		case ZombieType.CabbagepultZombie:
			zombieBase = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CabbagepultZombie).GetComponent<PlantZombie>();
			break;
		default:
			Debug.Log("无该类型僵尸:" + Type);
			break;
		}
		if (zombieBase != null)
		{
			zombieBase.SummonInit();
			zombieBase.PlacePlayer = null;
			zombieBase.zombieType = Type;
			zombieBase.transform.SetParent(base.transform);
		}
		return zombieBase;
	}

	public int GetZombieWeight(ZombieType type)
	{
		int num = -1;
		return type switch
		{
			ZombieType.NormalZombie => 1, 
			ZombieType.ConeZombie => 2, 
			ZombieType.BucketZombie => 4, 
			ZombieType.DoorZombie => 4, 
			ZombieType.DoorAndCone => 5, 
			ZombieType.DoorAndBucket => 6, 
			ZombieType.Polevaulter => 2, 
			ZombieType.PaperZombie => 2, 
			ZombieType.JacksonZombie => 5, 
			ZombieType.DancerZombie => 1, 
			ZombieType.FootballZombie => 5, 
			ZombieType.BlackFootball => 6, 
			ZombieType.Zomboni => 5, 
			ZombieType.JackboxZombie => 4, 
			ZombieType.BalloonZombie => 3, 
			ZombieType.DiggerZombie => 3, 
			ZombieType.PogoZombie => 4, 
			ZombieType.BungiZombie => 4, 
			ZombieType.HeavyBungiZombie => 6, 
			ZombieType.CatapultZombie => 5, 
			ZombieType.RockCatapultZombie => 7, 
			ZombieType.RoadrollerZombie => 10, 
			ZombieType.Gargantuar => 9, 
			ZombieType.GargantuarHelmet => 14, 
			ZombieType.GargantuarRedeye => 12, 
			ZombieType.GargantuarHelmetRedeye => 19, 
			ZombieType.GargantuarInjured => 5, 
			ZombieType.ImpZpmbie => 1, 
			ZombieType.LadderZombie => 4, 
			ZombieType.Yeti => 4, 
			ZombieType.DolphinriderZombie => 3, 
			ZombieType.SnorkleZombie => 2, 
			ZombieType.SnorkleZombieHelmet => 5, 
			ZombieType.BobsledZombie => 1, 
			ZombieType.BobsledZombieHelmet => 4, 
			ZombieType.BobsledZombieSled => 7, 
			ZombieType.BobsledZombieHelmetSled => 12, 
			ZombieType.DiscoZombie => 3, 
			ZombieType.BackupZombie => 1, 
			ZombieType.TubeZombie => 2, 
			ZombieType.TubeConeZombie => 3, 
			ZombieType.TubeBucketZombie => 5, 
			ZombieType.TubeDoorZombie => 5, 
			ZombieType.TubeDoorConeZombie => 6, 
			ZombieType.TubeDoorBucketZombie => 7, 
			ZombieType.SwampNormal => 1, 
			ZombieType.StoolZombie => 2, 
			ZombieType.SwampBucket => 4, 
			ZombieType.SwampDoor => 3, 
			ZombieType.SwampDoorAndStool => 4, 
			ZombieType.SwampDoorAndBucket => 5, 
			ZombieType.Ghost => 3, 
			ZombieType.Crocodile => 4, 
			ZombieType.StarveZombie => 5, 
			ZombieType.SlimeZombie => 4, 
			ZombieType.SwampGargantuar => 10, 
			ZombieType.SwampGargantuarNoCro => 7, 
			ZombieType.SwampGargantuarBlueEye => 15, 
			ZombieType.SwampGargantuarBlueEyeNoCro => 12, 
			ZombieType.PeaShooterZombie => 1, 
			ZombieType.SnowpeaShooterZombie => 1, 
			ZombieType.RepeaterZombie => 1, 
			ZombieType.GatlingZombie => 3, 
			ZombieType.TorchwoodZombie => 1, 
			ZombieType.WallNutZombie => 3, 
			ZombieType.TallNutZombie => 6, 
			ZombieType.HyponShroomZombie => 1, 
			ZombieType.IceShroomZombie => 3, 
			ZombieType.DoomShroomZombie => 3, 
			ZombieType.JalapenoZombie => 3, 
			ZombieType.SquashZombie => 3, 
			ZombieType.SnowRepeaterZombie => 4, 
			ZombieType.FumeShroomZombie => 1, 
			ZombieType.GloomShroomZombie => 3, 
			ZombieType.StarfruitZombie => 1, 
			ZombieType.SunflowerZombie => 1, 
			ZombieType.BloverZombie => 3, 
			ZombieType.UmbrellaZombie => 1, 
			ZombieType.CabbagepultZombie => 2, 
			ZombieType.CornpultZombie => 2, 
			ZombieType.MelonpultZombie => 3, 
			ZombieType.WinterMelonZombie => 3, 
			_ => 1, 
		};
	}

	private Grid GetPosByGridVertical(Vector3 pos, int verticalNum, float x)
	{
		return MapManager.Instance.GetGridByWorldPos(pos, verticalNum);
	}

	private void AddZombie(ZombieBase zombie)
	{
		if (!zombies.Contains(zombie))
		{
			zombies.Add(zombie);
			LVManager.Instance.ZombieNumChange(zombies.Count);
		}
	}

	public void RemoveZombie(ZombieBase zombie)
	{
		if (zombie.isHypno)
		{
			hypnoZombies.Remove(zombie);
		}
		else if (zombies.Remove(zombie))
		{
			CheckAllZombieDeadForLV(zombie);
			LVManager.Instance.ZombieNumChange(zombies.Count);
		}
	}

	public void ZombieHypno(ZombieBase zombie)
	{
		if (zombies.Remove(zombie))
		{
			if (!hypnoZombies.Contains(zombie))
			{
				hypnoZombies.Add(zombie);
			}
			CheckAllZombieDeadForLV(zombie);
		}
		else if (hypnoZombies.Remove(zombie) && !zombies.Contains(zombie))
		{
			zombies.Add(zombie);
		}
	}

	public ZombieBase GetLastZombieByLine(int lineNum, Vector3 pos, bool getRight = false, bool getHyp = false)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2)
			{
				continue;
			}
			if (getRight)
			{
				if (list[i].ContainLine(lineNum) && list[i].collider2d.enabled && list[i].transform.position.x >= pos.x && Vector2.Distance(pos, list[i].transform.position) >= num && list[i].Hp > 0)
				{
					num = Vector2.Distance(pos, list[i].transform.position);
					result = list[i];
				}
			}
			else if (list[i].ContainLine(lineNum) && list[i].collider2d.enabled && list[i].transform.position.x <= pos.x && Vector2.Distance(pos, list[i].transform.position) >= num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public ZombieBase GetZombieByLineMinDistance(int lineNum, Vector3 pos, bool getLeft, bool getHyp, bool needCapsule = true)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 10000f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2 || (needCapsule && !list[i].collider2d.enabled))
			{
				continue;
			}
			if (getLeft)
			{
				if (list[i].transform.position.x > pos.x)
				{
					continue;
				}
			}
			else if (list[i].transform.position.x < pos.x)
			{
				continue;
			}
			if (list[i].ContainLine(lineNum) && Vector2.Distance(pos, list[i].transform.position) < num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public ZombieBase GetZombieByLineMinDisCanNoCollGet(int lineNum, Vector3 pos, bool getLeft, bool getHyp)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 10000f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2 || (!list[i].CanNoCollGet && !list[i].collider2d.enabled))
			{
				continue;
			}
			if (getLeft)
			{
				if (list[i].transform.position.x > pos.x)
				{
					continue;
				}
			}
			else if (list[i].transform.position.x < pos.x)
			{
				continue;
			}
			if (list[i].ContainLine(lineNum) && Vector2.Distance(pos, list[i].transform.position) < num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public ZombieBase GetZombieByLineMinDisNoDir(int lineNum, Vector3 pos, bool getHyp, bool needCapsule = true)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		ZombieBase result = null;
		float num = 10000f;
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (!(currMap != currMap2) && (!needCapsule || list[i].collider2d.enabled) && list[i].ContainLine(lineNum) && Vector2.Distance(pos, list[i].transform.position) < num && list[i].Hp > 0)
			{
				num = Vector2.Distance(pos, list[i].transform.position);
				result = list[i];
			}
		}
		return result;
	}

	public List<ZombieBase> GetZombiesByLine(int lineNum, Vector3 pos, bool getLeft, bool getHyp, bool needCapsule = true)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		List<ZombieBase> list2 = new List<ZombieBase>();
		for (int i = 0; i < list.Count; i++)
		{
			MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
			if (currMap != currMap2 || (needCapsule && !list[i].collider2d.enabled))
			{
				continue;
			}
			if (getLeft)
			{
				if (list[i].transform.position.x > pos.x)
				{
					continue;
				}
			}
			else if (list[i].transform.position.x < pos.x)
			{
				continue;
			}
			if (list[i].ContainLine(lineNum) && list[i].Hp > 0)
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public List<ZombieBase> GetZombiesByLine(int lineNum, Vector2 targetPos, float dis, bool needCapsule)
	{
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		List<ZombieBase> list = new List<ZombieBase>();
		for (int i = 0; i < zombies.Count; i++)
		{
			if (!needCapsule || zombies[i].collider2d.enabled)
			{
				MapBase currMap2 = MapManager.Instance.GetCurrMap(zombies[i].transform.position);
				if (currMap == currMap2 && zombies[i].ContainLine(lineNum) && Mathf.Abs(targetPos.x - zombies[i].transform.position.x) < dis)
				{
					list.Add(zombies[i]);
				}
			}
		}
		for (int j = 0; j < hypnoZombies.Count; j++)
		{
			if (!needCapsule || hypnoZombies[j].collider2d.enabled)
			{
				MapBase currMap3 = MapManager.Instance.GetCurrMap(hypnoZombies[j].transform.position);
				if (currMap == currMap3 && hypnoZombies[j].ContainLine(lineNum) && Mathf.Abs(targetPos.x - hypnoZombies[j].transform.position.x) < dis)
				{
					list.Add(hypnoZombies[j]);
				}
			}
		}
		return list;
	}

	public List<ZombieBase> GetZombiesByLine(int lineNum, Vector2 targetPos, float dis, bool getHyp, bool needCapsule)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		List<ZombieBase> list2 = new List<ZombieBase>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!needCapsule || list[i].collider2d.enabled)
			{
				MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
				if (currMap == currMap2 && list[i].ContainLine(lineNum) && Mathf.Abs(targetPos.x - list[i].transform.position.x) < dis)
				{
					list2.Add(list[i]);
				}
			}
		}
		return list2;
	}

	public List<ZombieBase> GetZombies(Vector2 targetPos, float dis, bool getHyp, bool needCapsule)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		List<ZombieBase> list2 = new List<ZombieBase>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!needCapsule || list[i].collider2d.enabled)
			{
				MapBase currMap2 = MapManager.Instance.GetCurrMap(list[i].transform.position);
				if (currMap == currMap2 && Vector2.Distance(targetPos, list[i].transform.position) < dis)
				{
					list2.Add(list[i]);
				}
			}
		}
		return list2;
	}

	public List<ZombieBase> GetAllZombies()
	{
		return zombies;
	}

	public List<ZombieBase> GetAllHypZombies()
	{
		return hypnoZombies;
	}

	public List<ZombieBase> GetAllZombies(Vector2 pos, bool getHyp, bool needCapsule = false)
	{
		List<ZombieBase> list = ((!getHyp) ? zombies : hypnoZombies);
		List<ZombieBase> list2 = new List<ZombieBase>();
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		for (int i = 0; i < list.Count; i++)
		{
			if ((!needCapsule || list[i].collider2d.enabled) && MapManager.Instance.GetCurrMap(list[i].transform.position) == currMap && list[i].Hp > 0)
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public void ZombieStartIdel()
	{
		if (zombies.Count != 0)
		{
			for (int i = 0; i < zombies.Count; i++)
			{
				zombies[i].StartIdel();
			}
		}
	}

	private void CheckAllZombieDeadForLV(ZombieBase zombie)
	{
		if (GameManager.Instance.isClient || !LVManager.Instance.InGame)
		{
			return;
		}
		if (LV.Instance.CurrLVType == LVType.IZombie && zombies.Count == 0 && LvItemManager.Instance.GetIZbrainNum() > 0 && SeedBank.Instance.AllNoSunPlace())
		{
			LVManager.Instance.GameOver2();
		}
		LVManager.Instance.OnZombieDeadEvent();
		if (zombies.Count == 0)
		{
			if (AllZombieDeadAction != null)
			{
				AllZombieDeadAction();
			}
			if (LVManager.Instance.LvSpawnisOver)
			{
				LVManager.Instance.SettleLv(zombie.transform.position);
			}
			if (LV.Instance.CurrLVType == LVType.VaseBreaker && LvItemManager.Instance.GetVaseNum() == 0)
			{
				LVManager.Instance.SettleLv(zombie.transform.position);
			}
		}
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapManager.Instance.mapList[i].ZombieDeadEvent(zombie);
		}
	}

	public void AddAllZombieDeadAction(UnityAction action)
	{
		AllZombieDeadAction = (UnityAction)Delegate.Combine(AllZombieDeadAction, action);
	}

	public void RemoveAllZombieDeadAction(UnityAction action)
	{
		AllZombieDeadAction = (UnityAction)Delegate.Remove(AllZombieDeadAction, action);
	}

	private void Yell()
	{
	}

	private IEnumerator DoYell()
	{
		while (true)
		{
			if (zombies.Count > 0 && UnityEngine.Random.Range(0, 10) > 8)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieYell, base.transform.position, isAll: true);
			}
			yield return new WaitForSeconds(5f);
		}
	}

	public string GetZombieName(ZombieType zombieType)
	{
		string result = "???";
		switch (zombieType)
		{
		case ZombieType.NormalZombie:
			result = "普通僵尸";
			break;
		case ZombieType.ConeZombie:
			result = "路障僵尸";
			break;
		case ZombieType.BucketZombie:
			result = "铁桶僵尸";
			break;
		case ZombieType.DoorZombie:
			result = "铁门僵尸";
			break;
		case ZombieType.DoorAndCone:
			result = "铁门路障僵尸";
			break;
		case ZombieType.DoorAndBucket:
			result = "铁门铁桶僵尸";
			break;
		case ZombieType.Polevaulter:
			result = "撑杆僵尸";
			break;
		case ZombieType.PaperZombie:
			result = "读报僵尸";
			break;
		case ZombieType.FootballZombie:
			result = "橄榄球僵尸";
			break;
		case ZombieType.BlackFootball:
			result = "黑橄榄球僵尸";
			break;
		case ZombieType.Zomboni:
			result = "冰车僵尸";
			break;
		case ZombieType.JackboxZombie:
			result = "小丑僵尸";
			break;
		case ZombieType.BalloonZombie:
			result = "气球僵尸";
			break;
		case ZombieType.DiggerZombie:
			result = "矿工僵尸";
			break;
		case ZombieType.PogoZombie:
			result = "跳跳僵尸";
			break;
		case ZombieType.BungiZombie:
			result = "蹦极僵尸";
			break;
		case ZombieType.CatapultZombie:
			result = "投篮车僵尸";
			break;
		case ZombieType.Gargantuar:
			result = "巨人僵尸";
			break;
		case ZombieType.GargantuarHelmet:
			result = "头盔巨人僵尸";
			break;
		case ZombieType.GargantuarRedeye:
			result = "红眼巨人僵尸";
			break;
		case ZombieType.GargantuarHelmetRedeye:
			result = "头盔红眼巨人僵尸";
			break;
		case ZombieType.GargantuarInjured:
			result = "战损巨人僵尸";
			break;
		case ZombieType.ImpZpmbie:
			result = "小鬼僵尸";
			break;
		case ZombieType.LadderZombie:
			result = "梯子僵尸";
			break;
		case ZombieType.Yeti:
			result = "雪人僵尸";
			break;
		case ZombieType.SwampNormal:
			result = "沼泽普通僵尸";
			break;
		case ZombieType.StoolZombie:
			result = "凳子僵尸";
			break;
		case ZombieType.SwampBucket:
			result = "沼泽铁桶僵尸";
			break;
		case ZombieType.SwampDoor:
			result = "木门僵尸";
			break;
		case ZombieType.SwampDoorAndStool:
			result = "木门凳子僵尸";
			break;
		case ZombieType.SwampDoorAndBucket:
			result = "木门铁桶僵尸";
			break;
		case ZombieType.Ghost:
			result = "幽灵僵尸";
			break;
		case ZombieType.PvPTarget:
			result = "对战标靶";
			break;
		case ZombieType.FlagZombie:
			result = "旗帜僵尸";
			break;
		case ZombieType.FlagConeZombie:
			result = "旗帜路障僵尸";
			break;
		case ZombieType.FlagBucketZombie:
			result = "旗帜铁桶僵尸";
			break;
		case ZombieType.FlagSwampZombie:
			result = "沼泽旗帜僵尸";
			break;
		case ZombieType.FlagStoolZombie:
			result = "旗帜木凳僵尸";
			break;
		case ZombieType.FlagBucketSwampZombie:
			result = "旗帜铁桶僵尸";
			break;
		case ZombieType.Crocodile:
			result = "僵尸鳄鱼";
			break;
		case ZombieType.SwampGargantuar:
			result = "沼泽巨人僵尸";
			break;
		case ZombieType.SwampGargantuarNoCro:
			result = "沼泽巨人僵尸";
			break;
		case ZombieType.SwampGargantuarBlueEye:
			result = "蓝眼沼泽巨人僵尸";
			break;
		case ZombieType.SwampGargantuarBlueEyeNoCro:
			result = "蓝眼沼泽巨人僵尸";
			break;
		case ZombieType.JacksonZombie:
			result = "舞王僵尸";
			break;
		case ZombieType.DancerZombie:
			result = "伴舞僵尸";
			break;
		case ZombieType.DiscoZombie:
			result = "迪斯科僵尸";
			break;
		case ZombieType.BackupZombie:
			result = "舞伴僵尸";
			break;
		case ZombieType.SnorkleZombie:
			result = "潜水僵尸";
			break;
		case ZombieType.SnorkleZombieHelmet:
			result = "头盔潜水僵尸";
			break;
		case ZombieType.DolphinriderZombie:
			result = "海豚僵尸";
			break;
		case ZombieType.IZombieBrain:
			result = "我是僵尸大脑";
			break;
		case ZombieType.BobsledZombie:
			result = "雪橇车僵尸";
			break;
		case ZombieType.BobsledZombieHelmet:
			result = "头盔雪橇车僵尸";
			break;
		case ZombieType.BobsledZombieSled:
			result = "雪橇车僵尸";
			break;
		case ZombieType.BobsledZombieHelmetSled:
			result = "头盔雪橇车僵尸";
			break;
		case ZombieType.TubeZombie:
			result = "鸭子泳圈僵尸";
			break;
		case ZombieType.TubeConeZombie:
			result = "泳圈路障僵尸";
			break;
		case ZombieType.TubeBucketZombie:
			result = "泳圈铁桶僵尸";
			break;
		case ZombieType.TubeDoorZombie:
			result = "泳圈铁门僵尸";
			break;
		case ZombieType.TubeDoorConeZombie:
			result = "泳圈铁门路障僵尸";
			break;
		case ZombieType.TubeDoorBucketZombie:
			result = "泳圈铁门铁桶僵尸";
			break;
		case ZombieType.PeaShooterZombie:
			result = "豌豆射手僵尸";
			break;
		case ZombieType.SnowpeaShooterZombie:
			result = "寒冰射手僵尸";
			break;
		case ZombieType.RepeaterZombie:
			result = "双发射手僵尸";
			break;
		case ZombieType.GatlingZombie:
			result = "机枪射手僵尸";
			break;
		case ZombieType.TorchwoodZombie:
			result = "火炬树桩僵尸";
			break;
		case ZombieType.WallNutZombie:
			result = "坚果墙僵尸";
			break;
		case ZombieType.TallNutZombie:
			result = "高坚果僵尸";
			break;
		case ZombieType.HyponShroomZombie:
			result = "魅惑菇僵尸";
			break;
		case ZombieType.IceShroomZombie:
			result = "寒冰菇僵尸";
			break;
		case ZombieType.DoomShroomZombie:
			result = "毁灭菇僵尸";
			break;
		case ZombieType.JalapenoZombie:
			result = "火爆辣椒僵尸";
			break;
		case ZombieType.SquashZombie:
			result = "窝瓜僵尸";
			break;
		case ZombieType.SnowRepeaterZombie:
			result = "极冻射手僵尸";
			break;
		case ZombieType.FumeShroomZombie:
			result = "大喷菇僵尸";
			break;
		case ZombieType.GloomShroomZombie:
			result = "忧郁菇僵尸";
			break;
		case ZombieType.StarfruitZombie:
			result = "杨桃僵尸";
			break;
		case ZombieType.SunflowerZombie:
			result = "向日葵僵尸";
			break;
		case ZombieType.BloverZombie:
			result = "三叶草僵尸";
			break;
		case ZombieType.UmbrellaZombie:
			result = "保护伞僵尸";
			break;
		case ZombieType.CabbagepultZombie:
			result = "卷心菜投手僵尸";
			break;
		case ZombieType.CornpultZombie:
			result = "玉米投手僵尸";
			break;
		case ZombieType.MelonpultZombie:
			result = "西瓜投手僵尸";
			break;
		case ZombieType.WinterMelonZombie:
			result = "冰瓜投手僵尸";
			break;
		case ZombieType.RoadrollerZombie:
			result = "压路机僵尸";
			break;
		case ZombieType.StarveZombie:
			result = "饿殍僵尸";
			break;
		case ZombieType.RockCatapultZombie:
			result = "投石车僵尸";
			break;
		case ZombieType.HeavyBungiZombie:
			result = "重型蹦极僵尸";
			break;
		case ZombieType.SlimeZombie:
			result = "史莱姆僵尸";
			break;
		}
		return result;
	}
}
