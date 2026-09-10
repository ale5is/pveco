using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class LvItemManager : MonoBehaviour
{
	public static LvItemManager Instance;

	private int IZBrainNum;

	public List<Vase> AllVase = new List<Vase>();

	private List<IZombieBrain> IZbrains = new List<IZombieBrain>();

	private float MeltZSort;

	private bool vaseAlwaysLight;

	public bool VaseAlwaysLight
	{
		get
		{
			return vaseAlwaysLight;
		}
		set
		{
			vaseAlwaysLight = value;
			for (int i = 0; i < AllVase.Count; i++)
			{
				AllVase[i].CheckLight();
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private float GetMeltZ()
	{
		MeltZSort += 0.01f;
		if (MeltZSort > 1f)
		{
			MeltZSort = 0f;
		}
		return 0f - MeltZSort;
	}

	public void LvReset()
	{
		MeltZSort = 0f;
		AllVase.Clear();
		IZbrains.Clear();
	}

	public int GetVaseNum()
	{
		return AllVase.Count;
	}

	public int GetIZbrainNum()
	{
		return IZbrains.Count;
	}

	public void InitIZBrains(List<IZombieBrain> brains)
	{
		IZbrains = brains;
		IZBrainNum = IZbrains.Count;
		FlagMeter.Instance.IZUpdate(IZBrainNum - IZbrains.Count, IZBrainNum);
	}

	public void IZBrainDead(IZombieBrain brain)
	{
		if (IZbrains.Remove(brain))
		{
			FlagMeter.Instance.IZUpdate(IZBrainNum - IZbrains.Count, IZBrainNum);
			if (IZbrains.Count == 0)
			{
				LVManager.Instance.SettleLv(brain.transform.position);
			}
		}
	}

	public Vase CreateVase(Grid grid, VaseType type)
	{
		Vase component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Vase).GetComponent<Vase>();
		component.CreateInit(grid, type);
		component.transform.SetParent(base.transform);
		AllVase.Add(component);
		if (GameManager.Instance.isServer)
		{
			component.OnlineId = SocketServer.Instance.ItemId;
			VaseSpawn vaseSpawn = new VaseSpawn();
			vaseSpawn.OnlineId = component.OnlineId;
			vaseSpawn.vaseType = type;
			vaseSpawn.GridPos = grid.Position;
			SocketServer.Instance.SpawnVase(vaseSpawn);
		}
		return component;
	}

	public void ClientCreateVase(VaseSpawn vaseSpawn)
	{
		CreateVase(MapManager.Instance.GetGridByWorldPos(vaseSpawn.GridPos), vaseSpawn.vaseType).OnlineId = vaseSpawn.OnlineId;
	}

	public void SynVase(SynItem syn)
	{
		for (int i = 0; i < AllVase.Count; i++)
		{
			if (AllVase[i].OnlineId == syn.OnlineId)
			{
				AllVase[i].OnlineSyn(syn);
			}
		}
	}

	public void DestoryVase(Vase vase)
	{
		if (AllVase.Remove(vase) && AllVase.Count == 0 && ZombieManager.Instance.GetZombieNum() == 0 && LV.Instance.CurrLVType == LVType.VaseBreaker)
		{
			LVManager.Instance.SettleLv(vase.transform.position);
		}
		Object.Destroy(vase.gameObject);
	}

	public void SpawnMelt(int line, Vector3 pos)
	{
		if (!GameManager.Instance.isClient)
		{
			int type = Random.Range(1, 3);
			Vector3 vector = pos + new Vector3(Random.Range(-0.5f, 0.5f), 0f, GetMeltZ());
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Melt).GetComponent<Melt>().CreateInit(type, line, vector);
			if (GameManager.Instance.isServer)
			{
				MeltSpawn meltSpawn = new MeltSpawn();
				meltSpawn.Type = type;
				meltSpawn.line = line;
				meltSpawn.Pos = vector;
				SocketServer.Instance.SpawnMelt(meltSpawn);
			}
		}
	}

	public void SpawnMelt(MeltSpawn spawn)
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Melt).GetComponent<Melt>().CreateInit(spawn.Type, spawn.line, new Vector3(spawn.Pos.x, spawn.Pos.y, GetMeltZ()));
	}

	public void SpawnFallHail(Grid grid, float scale)
	{
		if (!GameManager.Instance.isClient)
		{
			int type = Random.Range(0, 3);
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FallHail).GetComponent<FallHail>().CreateInit(grid, scale, type);
			if (GameManager.Instance.isServer)
			{
				FallHailSpawn fallHailSpawn = new FallHailSpawn();
				fallHailSpawn.type = type;
				fallHailSpawn.scale = scale;
				fallHailSpawn.pos = grid.Position;
				SocketServer.Instance.SpawnFallHail(fallHailSpawn);
			}
		}
	}

	public void SpawnFallHail(FallHailSpawn spawn)
	{
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(spawn.pos);
		if (gridByWorldPos != null)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FallHail).GetComponent<FallHail>().CreateInit(gridByWorldPos, spawn.scale, spawn.type);
		}
	}

	public void DropCoin(Vector2 pos, bool AlwaysDrop = false, bool notDiamond = false)
	{
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			return;
		}
		int minInclusive = 1;
		if (AlwaysDrop)
		{
			minInclusive = 167;
		}
		int num = Random.Range(minInclusive, 200);
		if (num > 198)
		{
			if (!notDiamond)
			{
				InstantiateItem(GameManager.Instance.GameConf.Diamond, pos);
			}
			else
			{
				InstantiateItem(GameManager.Instance.GameConf.Silvercoin, pos);
			}
		}
		else if (num > 186)
		{
			InstantiateItem(GameManager.Instance.GameConf.Goldcoin, pos);
		}
		else if (num > 166)
		{
			InstantiateItem(GameManager.Instance.GameConf.Silvercoin, pos);
		}
	}

	public void SummonDiamond(Vector2 pos)
	{
		InstantiateItem(GameManager.Instance.GameConf.Diamond, pos);
	}

	public void SummonSilverCoin(Vector2 pos)
	{
		InstantiateItem(GameManager.Instance.GameConf.Silvercoin, pos);
	}

	public void SummonGoldCoin(Vector2 pos)
	{
		InstantiateItem(GameManager.Instance.GameConf.Goldcoin, pos);
	}

	private void InstantiateItem(GameObject prefab, Vector2 pos)
	{
		Allcoin component = PoolManager.Instance.GetObj(prefab).GetComponent<Allcoin>();
		component.transform.SetParent(null);
		component.InitForItem(pos);
	}
}
