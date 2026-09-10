using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	public static MapManager Instance;

	private List<Vector2> pointList = new List<Vector2>();

	public List<MapBase> mapList = new List<MapBase>();

	public List<Iceroad> iceroads = new List<Iceroad>();

	public List<Puddle> puddles = new List<Puddle>();

	public List<PortalController> portalCs = new List<PortalController>();

	private List<Obstacle> obstacles = new List<Obstacle>();

	private void Awake()
	{
		Instance = this;
	}

	public string MapSeriesName(MapSeries type)
	{
		string result = "";
		switch (type)
		{
		case MapSeries.Yard:
			result = "庭院";
			break;
		case MapSeries.Swamp:
			result = "沼泽";
			break;
		case MapSeries.CustomYard:
			result = "自定义庭院";
			break;
		}
		return result;
	}

	public string MapTypeName(MapType type)
	{
		string result = "";
		switch (type)
		{
		case MapType.FrontYard:
			result = "前院";
			break;
		case MapType.BackYard:
			result = "后院";
			break;
		case MapType.Roof:
			result = "屋顶";
			break;
		case MapType.FrontSwamp:
			result = "沼泽前院";
			break;
		case MapType.BackSwamp:
			result = "沼泽后院";
			break;
		case MapType.Basement:
			result = "沼泽地下";
			break;
		case MapType.PvPYard:
			result = "对战前院";
			break;
		case MapType.CustomYard:
			result = "自定义庭院";
			break;
		}
		return result;
	}

	public GameObject GetMapPrefab(MapType type)
	{
		GameObject result = null;
		switch (type)
		{
		case MapType.FrontYard:
			result = GameManager.Instance.GameConf.FrontYard;
			break;
		case MapType.BackYard:
			result = GameManager.Instance.GameConf.BackYard;
			break;
		case MapType.Roof:
			result = GameManager.Instance.GameConf.Roof;
			break;
		case MapType.FrontSwamp:
			result = GameManager.Instance.GameConf.FrontSwamp;
			break;
		case MapType.BackSwamp:
			result = GameManager.Instance.GameConf.BackSwamp;
			break;
		case MapType.Basement:
			result = GameManager.Instance.GameConf.BackSwamp;
			break;
		case MapType.PvPYard:
			result = GameManager.Instance.GameConf.PvPYard;
			break;
		case MapType.CustomYard:
			result = GameManager.Instance.GameConf.CustomYard;
			break;
		}
		return result;
	}

	public void ResetScence()
	{
		List<Iceroad> list = new List<Iceroad>(iceroads);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Dead();
		}
		iceroads.Clear();
		for (int j = 0; j < puddles.Count; j++)
		{
			puddles[j].Destroy();
		}
		puddles.Clear();
		for (int k = 0; k < mapList.Count; k++)
		{
			List<Grid> gridList = mapList[k].GridList;
			for (int l = 0; l < gridList.Count; l++)
			{
				gridList[l].DesToryGrid();
			}
			mapList[k].StopAllCoroutines();
			Object.Destroy(mapList[k].gameObject);
		}
		mapList.Clear();
		List<Obstacle> list2 = new List<Obstacle>(obstacles);
		for (int m = 0; m < list2.Count; m++)
		{
			list2[m].DestroyThis();
		}
		obstacles.Clear();
	}

	public void CreateMap(List<MapType> types)
	{
		for (int i = 0; i < types.Count; i++)
		{
			GameObject mapPrefab = GetMapPrefab(types[i]);
			if (mapPrefab != null)
			{
				MapBase component = Object.Instantiate(mapPrefab).GetComponent<MapBase>();
				component.transform.position = new Vector3(0f, 30 * i);
				mapList.Add(component);
			}
		}
		for (int j = 0; j < mapList.Count; j++)
		{
			mapList[j].BaseInitMap();
		}
	}

	public void CreateAllMower()
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			mapList[i].SpawnMower();
		}
	}

	public void CreateAllBrain()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		List<IZombieBrain> list = new List<IZombieBrain>();
		for (int i = 0; i < mapList.Count; i++)
		{
			for (int j = 0; j < mapList[i].GridList.Count; j++)
			{
				if (mapList[i].GridList[j].Point.x == 0)
				{
					IZombieBrain component = ZombieManager.Instance.GetNewZombie(ZombieType.IZombieBrain).GetComponent<IZombieBrain>();
					ZombieManager.Instance.UpdateZombie(ZombieType.IZombieBrain, component, mapList[i].GridList[j].Position - new Vector2(1f, 0f), mapList[i].GridList[j].Point.y);
					component.RatThis();
					list.Add(component);
				}
			}
		}
		LvItemManager.Instance.InitIZBrains(list);
	}

	public void LoadIZombieMap()
	{
		if (LV.Instance.StartPlants.Count == 0)
		{
			return;
		}
		for (int i = 0; i < mapList.Count; i++)
		{
			int num = 0;
			List<Grid> gridList = mapList[i].GridList;
			List<List<PlantType>> list = LV.Instance.StartPlants[i];
			if (list.Count > 0)
			{
				num = list[0].Count - 1;
			}
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].Point.x <= num)
				{
					gridList[j].CanPlaceZombie = false;
				}
				else
				{
					gridList[j].CanPlaceZombie = true;
				}
			}
			mapList[i].SetStripe(num);
		}
	}

	public void LoadVaseBreaker()
	{
		if (GameManager.Instance.isClient || LV.Instance.VaseBreakerVase.Count == 0)
		{
			return;
		}
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Vase> list = new List<Vase>();
			List<Vase> list2 = new List<Vase>();
			MapBase map = mapList[i];
			List<List<VaseType>> list3 = new List<List<VaseType>>(LV.Instance.VaseBreakerVase[i]);
			list3.Shuffle();
			for (int j = 0; j < list3.Count; j++)
			{
				List<int> list4 = new List<int>();
				for (int k = 0; k < list3[j].Count; k++)
				{
					list4.Add(list3[j][k].PlaceX);
				}
				list4.Shuffle();
				for (int l = 0; l < list3[j].Count; l++)
				{
					Grid thisGrid = GetThisGrid(map, list4[l], j);
					if (thisGrid != null)
					{
						Vase item = LvItemManager.Instance.CreateVase(thisGrid, list3[j][l]);
						if (list3[j][l].plantType != PlantType.Nope)
						{
							list.Add(item);
						}
						else if (list3[j][l].zombieType != ZombieType.Nope)
						{
							list2.Add(item);
						}
					}
				}
			}
			if (LV.Instance.PlantVaseNum > 0)
			{
				list.Shuffle();
				for (int m = 0; m < LV.Instance.PlantVaseNum; m++)
				{
					if (list.Count > m)
					{
						list[m].SetVaseType(1);
					}
				}
			}
			if (LV.Instance.ZombieVaseNum <= 0)
			{
				continue;
			}
			list2.Shuffle();
			for (int n = 0; n < LV.Instance.ZombieVaseNum; n++)
			{
				if (list2.Count > n)
				{
					list2[n].SetVaseType(2);
				}
			}
		}
	}

	public void GameOverPause()
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			mapList[i].StopAllCoroutines();
		}
	}

	private void Start()
	{
	}

	public void CreatePortal(int num, int type)
	{
		if (!GameManager.Instance.isClient)
		{
			PortalController component = Object.Instantiate(GameManager.Instance.GameConf.PortalController).GetComponent<PortalController>();
			component.InitThis(num, type);
			component.transform.SetParent(mapList[0].transform);
			portalCs.Add(component);
		}
	}

	public void ClientCreatePortal(PortalSpawn spawn)
	{
		PortalController component = Object.Instantiate(GameManager.Instance.GameConf.PortalController).GetComponent<PortalController>();
		component.ClientInit(spawn);
		component.transform.SetParent(mapList[0].transform);
		portalCs.Add(component);
	}

	public void AddObstacle(Obstacle obstacle)
	{
		if (!obstacles.Contains(obstacle))
		{
			obstacles.Add(obstacle);
		}
	}

	public void RemoveObstacle(Obstacle obstacle)
	{
		obstacles.Remove(obstacle);
	}

	public List<Obstacle> GetAllObstacle(MapBase map)
	{
		List<Obstacle> list = new List<Obstacle>();
		for (int i = 0; i < obstacles.Count; i++)
		{
			if (obstacles[i].CurrMap == map)
			{
				list.Add(obstacles[i]);
			}
		}
		return list;
	}

	public List<Obstacle> GetAroundObstacle(Vector2 pos, float dis)
	{
		List<Obstacle> list = new List<Obstacle>();
		for (int i = 0; i < obstacles.Count; i++)
		{
			if (Vector2.Distance(pos, obstacles[i].transform.position) < dis)
			{
				list.Add(obstacles[i]);
			}
		}
		return list;
	}

	public MapBase GetCurrMap(Vector3 pos)
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			float num = mapList[i].transform.position.x - mapList[i].MapHalfLengthWidth.x;
			float num2 = mapList[i].transform.position.y - mapList[i].MapHalfLengthWidth.y;
			float num3 = mapList[i].transform.position.x + mapList[i].MapHalfLengthWidth.x;
			float num4 = mapList[i].transform.position.y + mapList[i].MapHalfLengthWidth.y;
			if (pos.x > num && pos.x < num3 && pos.y > num2 && pos.y < num4)
			{
				return mapList[i];
			}
		}
		return null;
	}

	public MapBase GetNearestMap(Vector3 pos)
	{
		float num = float.MaxValue;
		MapBase result = null;
		for (int i = 0; i < mapList.Count; i++)
		{
			float num2 = Vector2.Distance(mapList[i].transform.position, pos);
			if (num2 < num)
			{
				num = num2;
				result = mapList[i];
			}
		}
		return result;
	}

	public Grid GetGridPointByMouse()
	{
		return GetGridByWorldPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	public Grid GetGridByWorldPos(Vector2 worldPos)
	{
		MapBase nearestMap = GetNearestMap(worldPos);
		if (nearestMap == null)
		{
			return null;
		}
		return GetGridByWorldPos(nearestMap, worldPos);
	}

	public Grid GetGridByWorldPos(MapBase map, Vector2 worldPos)
	{
		if (map == null)
		{
			return null;
		}
		List<Grid> gridList = map.GridList;
		float num = 99999f;
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				result = gridList[i];
			}
		}
		return result;
	}

	public Grid GetGridByWorldPos(Vector2 worldPos, int line)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		return GetGridByWorldPos(currMap, worldPos, line);
	}

	public Grid GetGridByWorldPos(MapBase map, Vector2 worldPos, int line)
	{
		if (map == null)
		{
			return null;
		}
		List<Grid> gridList = map.GridList;
		float num = float.MaxValue;
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == line)
			{
				float num2 = Mathf.Abs(worldPos.x - gridList[i].Position.x);
				if (num2 < num)
				{
					num = num2;
					result = gridList[i];
				}
			}
		}
		return result;
	}

	public float GetLineY(Vector2 pos, int line)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return 0f;
		}
		if (line < 0 || line >= currMap.MapGridNum.y)
		{
			return 0f;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == line)
			{
				return gridList[i].Position.y;
			}
		}
		return 0f;
	}

	public List<Grid> GetColumnGrids(Vector2 gridpos)
	{
		MapBase nearestMap = GetNearestMap(gridpos);
		return GetColumnGrids(nearestMap, gridpos.x);
	}

	public List<Grid> GetColumnGrids(MapBase map, float x)
	{
		List<Grid> list = new List<Grid>();
		Grid gridByWorldPos = GetGridByWorldPos(map, new Vector2(x, map.GridList[0].Position.y));
		for (int i = 0; i < map.GridList.Count; i++)
		{
			if (map.GridList[i].Point.x == gridByWorldPos.Point.x)
			{
				list.Add(map.GridList[i]);
			}
		}
		return list;
	}

	public Grid GetThisGrid(MapBase map, int x, int y)
	{
		if (map == null)
		{
			return null;
		}
		Grid result = null;
		for (int i = 0; i < map.GridList.Count; i++)
		{
			if (map.GridList[i].Point.x == x && map.GridList[i].Point.y == y)
			{
				result = map.GridList[i];
				break;
			}
		}
		return result;
	}

	public Grid GetUpperGrid(Grid grid)
	{
		if (grid == null)
		{
			return null;
		}
		if (grid.Point.y == 0)
		{
			return null;
		}
		return GetThisGrid(GetCurrMap(grid.Position), grid.Point.x, grid.Point.y - 1);
	}

	public void LightGrid(Vector2 pos, Vector2Int Gridpos, int x, int y, bool isLight, bool noCorner, int lvl = 1)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return;
		}
		Vector2Int vector2Int = Gridpos - new Vector2Int(x, y);
		int num = 2 * x + 1;
		int num2 = 2 * y + 1;
		for (int i = 0; i < currMap.GridList.Count; i++)
		{
			if (currMap.GridList[i].Point.x >= vector2Int.x && currMap.GridList[i].Point.x < vector2Int.x + num && currMap.GridList[i].Point.y >= vector2Int.y && currMap.GridList[i].Point.y < vector2Int.y + num2 && (!noCorner || ((currMap.GridList[i].Point.x != vector2Int.x || currMap.GridList[i].Point.y != vector2Int.y) && (currMap.GridList[i].Point.x != vector2Int.x || currMap.GridList[i].Point.y != vector2Int.y + num2 - 1) && (currMap.GridList[i].Point.x != vector2Int.x + num - 1 || currMap.GridList[i].Point.y != vector2Int.y) && (currMap.GridList[i].Point.x != vector2Int.x + num - 1 || currMap.GridList[i].Point.y != vector2Int.y + num2 - 1))))
			{
				if (isLight)
				{
					currMap.GridList[i].LightNum += lvl;
				}
				else
				{
					currMap.GridList[i].LightNum -= lvl;
				}
			}
		}
	}

	public void WarmGrid(Vector2 pos, Vector2Int Gridpos, int x, int y, bool isAdd)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return;
		}
		Vector2Int vector2Int = Gridpos - new Vector2Int(x, y);
		int num = 2 * x + 1;
		int num2 = 2 * y + 1;
		for (int i = 0; i < currMap.GridList.Count; i++)
		{
			if (currMap.GridList[i].Point.x >= vector2Int.x && currMap.GridList[i].Point.x < vector2Int.x + num && currMap.GridList[i].Point.y >= vector2Int.y && currMap.GridList[i].Point.y < vector2Int.y + num2)
			{
				if (isAdd)
				{
					currMap.GridList[i].HotNum++;
				}
				else
				{
					currMap.GridList[i].HotNum--;
				}
			}
		}
	}

	public void CoverGrid(Vector2 pos, Vector2Int Gridpos, int x, int y, bool isCover, bool noCorner)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return;
		}
		Vector2Int vector2Int = Gridpos - new Vector2Int(x, y);
		int num = 2 * x + 1;
		int num2 = 2 * y + 1;
		for (int i = 0; i < currMap.GridList.Count; i++)
		{
			if (currMap.GridList[i].Point.x >= vector2Int.x && currMap.GridList[i].Point.x < vector2Int.x + num && currMap.GridList[i].Point.y >= vector2Int.y && currMap.GridList[i].Point.y < vector2Int.y + num2 && (!noCorner || ((currMap.GridList[i].Point.x != vector2Int.x || currMap.GridList[i].Point.y != vector2Int.y) && (currMap.GridList[i].Point.x != vector2Int.x || currMap.GridList[i].Point.y != vector2Int.y + num2 - 1) && (currMap.GridList[i].Point.x != vector2Int.x + num - 1 || currMap.GridList[i].Point.y != vector2Int.y) && (currMap.GridList[i].Point.x != vector2Int.x + num - 1 || currMap.GridList[i].Point.y != vector2Int.y + num2 - 1))))
			{
				if (isCover)
				{
					currMap.GridList[i].CoverNum++;
				}
				else
				{
					currMap.GridList[i].CoverNum--;
				}
			}
		}
	}

	public Grid GetFarestGrid(Vector2 worldPos, bool getLeft, int line)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = float.MinValue;
		if (getLeft)
		{
			num = float.MaxValue;
		}
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getLeft)
			{
				if (gridList[i].Position.x < num && gridList[i].Point.x >= 0)
				{
					num = gridList[i].Position.x;
					result = gridList[i];
				}
			}
			else if (gridList[i].Position.x > num && gridList[i].Point.x >= 0)
			{
				num = gridList[i].Position.x;
				result = gridList[i];
			}
		}
		return result;
	}

	public Grid GetRandomGrid()
	{
		List<Grid> gridList = mapList[Random.Range(0, mapList.Count)].GridList;
		return gridList[Random.Range(0, gridList.Count)];
	}

	public Grid GetRandomNoWaterGrid(int outLine)
	{
		List<Grid> list = new List<Grid>();
		for (int i = 0; i < mapList.Count; i++)
		{
			list.AddRange(mapList[i].GridList);
		}
		List<Grid> list2 = new List<Grid>(list);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].Point.x < outLine)
			{
				list2.Remove(list[j]);
			}
			else if (list[j].isWaterGrid)
			{
				list2.Remove(list[j]);
			}
		}
		return list2[Random.Range(0, list2.Count)];
	}

	public PlantBase GetMinDisPlant(Vector2 worldPos, int line, bool getHypno)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = float.MaxValue;
		Grid grid = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getHypno)
			{
				if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && gridList[i].CurrPlantBase.isHypno && Vector2.Distance(worldPos, gridList[i].Position) < num)
				{
					num = Vector2.Distance(worldPos, gridList[i].Position);
					grid = gridList[i];
				}
			}
			else if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && !gridList[i].CurrPlantBase.isHypno && Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				grid = gridList[i];
			}
		}
		return grid?.CurrPlantBase;
	}

	public PlantBase GetMinDisPlant(Vector2 worldPos, int line, bool getLeft, bool getHypno)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = float.MaxValue;
		Grid grid = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getHypno)
			{
				if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && gridList[i].CurrPlantBase.isHypno && (((gridList[i].Position.x < worldPos.x) & getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) < num)
				{
					num = Vector2.Distance(worldPos, gridList[i].Position);
					grid = gridList[i];
				}
			}
			else if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && !gridList[i].CurrPlantBase.isHypno && (((gridList[i].Position.x < worldPos.x) & getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) < num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				grid = gridList[i];
			}
		}
		return grid?.CurrPlantBase;
	}

	public Grid GetLastPlantGrid(Vector2 worldPos, int line, bool getLeft, bool getHyp)
	{
		MapBase currMap = GetCurrMap(worldPos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		float num = 0f;
		Grid result = null;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y != line)
			{
				continue;
			}
			if (getHyp)
			{
				if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && gridList[i].CurrPlantBase.isHypno && (((gridList[i].Position.x < worldPos.x) & getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) > num)
				{
					num = Vector2.Distance(worldPos, gridList[i].Position);
					result = gridList[i];
				}
			}
			else if (gridList[i].CurrPlantBase != null && gridList[i].CurrPlantBase.ZombieCanEat && !gridList[i].CurrPlantBase.isHypno && (((gridList[i].Position.x < worldPos.x) & getLeft) || (gridList[i].Position.x >= worldPos.x && !getLeft)) && Vector2.Distance(worldPos, gridList[i].Position) > num)
			{
				num = Vector2.Distance(worldPos, gridList[i].Position);
				result = gridList[i];
			}
		}
		return result;
	}

	public Grid GetNextGrid(Grid grid, bool isRight = false)
	{
		if (grid == null)
		{
			return null;
		}
		MapBase currMap = GetCurrMap(grid.Position);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		if (grid == null)
		{
			return null;
		}
		int num = gridList.IndexOf(grid);
		int num2 = 1;
		if (!isRight)
		{
			num2 = -1;
		}
		if (num + num2 < gridList.Count && num + num2 >= 0 && gridList[num + num2].Point.y == grid.Point.y)
		{
			return gridList[num + num2];
		}
		return null;
	}

	public List<Grid> GetAroundGrid(Grid grid, int radius)
	{
		List<Grid> list = new List<Grid>();
		MapBase currMap = GetCurrMap(grid.Position);
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		Vector2 vector = grid.Point - new Vector2Int(radius, radius);
		int num = 2 * radius + 1;
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				list2.Add(vector + new Vector2Int(i, j));
			}
		}
		for (int k = 0; k < gridList.Count; k++)
		{
			if (list2.Contains(gridList[k].Point))
			{
				list.Add(gridList[k]);
				list2.Remove(gridList[k].Point);
			}
		}
		return list;
	}

	public List<Grid> GetLineAllGrid(Vector2 pos, int lineNum)
	{
		List<Grid> list = new List<Grid>();
		List<Grid> gridList = GetCurrMap(pos).GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == lineNum)
			{
				list.Add(gridList[i]);
			}
		}
		return list;
	}

	public List<PlantBase> GetAllPlant(Vector3 pos, bool getHyp)
	{
		MapBase currMap = GetCurrMap(pos);
		List<PlantBase> list = new List<PlantBase>();
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].CurrPlantBase != null)
			{
				if (getHyp && gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
				else if (!getHyp && !gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
			}
		}
		return list;
	}

	public List<PlantBase> GetAroundPlant(Vector3 pos, float dis, bool getHyp)
	{
		List<PlantBase> list = new List<PlantBase>();
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (Vector2.Distance(gridList[i].Position, pos) < dis && (bool)gridList[i].CurrPlantBase)
			{
				if (getHyp && gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
				else if (!getHyp && !gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
			}
		}
		return list;
	}

	public List<PlantBase> GetLinePlant(Vector3 pos, int CurrLine, float dis, bool getHyp)
	{
		MapBase currMap = GetCurrMap(pos);
		List<PlantBase> list = new List<PlantBase>();
		if (currMap == null)
		{
			return list;
		}
		List<Grid> gridList = currMap.GridList;
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.y == CurrLine && Vector2.Distance(gridList[i].Position, pos) < dis && (bool)gridList[i].CurrPlantBase)
			{
				if (getHyp && gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
				else if (!getHyp && !gridList[i].CurrPlantBase.isHypno)
				{
					list.Add(gridList[i].CurrPlantBase);
				}
			}
		}
		return list;
	}

	public Grid GetHaveIceFirstGrid(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		Grid farestGrid = GetFarestGrid(pos, getLeft: false, 0);
		List<Grid> list = new List<Grid>();
		List<Grid> columnGrids = GetColumnGrids(currMap, farestGrid.Position.x);
		for (int i = 0; i < columnGrids.Count; i++)
		{
			if (columnGrids[i].IceRoadNum > 0 || columnGrids[i].SnowLvl > 0 || columnGrids[i].IsIce)
			{
				list.Add(columnGrids[i]);
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return null;
	}

	public Grid GetWaterGrid()
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> gridList = mapList[i].GridList;
			List<Grid> list = new List<Grid>();
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].isWaterGrid)
				{
					list.Add(gridList[j]);
				}
			}
			if (list.Count > 0)
			{
				int index = Random.Range(0, list.Count);
				if (list.Count <= 0)
				{
					return null;
				}
				return list[index];
			}
		}
		return null;
	}

	public Grid GetWaterGrid(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		List<Grid> list = new List<Grid>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].isWaterGrid)
			{
				list.Add(gridList[i]);
			}
		}
		int index = Random.Range(0, list.Count);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[index];
	}

	public int GetNoWaterGrid(Vector3 pos, bool moveOnIce)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return -1;
		}
		List<Grid> gridList = currMap.GridList;
		List<int> list = new List<int>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].isWaterGrid)
			{
				if (moveOnIce && gridList[i].IsIce)
				{
					break;
				}
				if (!list.Contains(gridList[i].Point.y))
				{
					list.Add(gridList[i].Point.y);
				}
			}
		}
		int y = currMap.MapGridNum.y;
		List<int> list2 = new List<int>();
		for (int j = 0; j < y; j++)
		{
			if (!list.Contains(j))
			{
				list2.Add(j);
			}
		}
		int index = Random.Range(0, list2.Count);
		if (list2.Count <= 0)
		{
			return -1;
		}
		return list2[index];
	}

	public Grid GetHavePlantGrid(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return null;
		}
		List<Grid> gridList = currMap.GridList;
		List<Grid> list = new List<Grid>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].CurrPlantBase != null)
			{
				list.Add(gridList[i]);
			}
		}
		int index = Random.Range(0, list.Count);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[index];
	}

	public float GetMapYHighest(Vector2 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return -1f;
		}
		return currMap.transform.position.y + currMap.MapHalfLengthWidth.y;
	}

	public Vector2 GetMapPos(Vector3 pos)
	{
		MapBase currMap = GetCurrMap(pos);
		if (currMap == null)
		{
			return new Vector2(0f, 0f);
		}
		return currMap.transform.position;
	}

	public GraveStone GraveStoneUp(Grid grid)
	{
		GraveStone graveStone = Object.Instantiate(GameManager.Instance.GameConf.GraveStone.GetComponent<GraveStone>());
		graveStone.CreateInit(grid);
		graveStone.transform.SetParent(GetCurrMap(grid.Position).transform);
		return graveStone;
	}

	public GameObject CreateCrater(Grid grid)
	{
		GameObject obj = Object.Instantiate(GameManager.Instance.GameConf.Crater);
		obj.transform.position = grid.Position + new Vector2(0f, -0.3f);
		obj.GetComponent<Crater>().CreateInit(grid.isWaterGrid, grid);
		return obj;
	}

	private IEnumerator MoveUp(GameObject gameObject, float Y)
	{
		while (gameObject.transform.position.y < Y)
		{
			yield return new WaitForSeconds(0.02f);
			gameObject.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
		}
	}

	public void PlantFlash(PlantType plantType)
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> gridList = mapList[i].GridList;
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].CurrPlantBase != null)
				{
					if (!gridList[j].CurrPlantBase.isHypno && gridList[j].CurrPlantBase.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.StartFlash();
					}
					else if (gridList[j].CurrPlantBase.CarryPlant != null && !gridList[j].CurrPlantBase.CarryPlant.isHypno && gridList[j].CurrPlantBase.CarryPlant.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.CarryPlant.StartFlash();
					}
				}
			}
		}
	}

	public void PlantnoFlash(PlantType plantType)
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> gridList = mapList[i].GridList;
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].CurrPlantBase != null)
				{
					if (gridList[j].CurrPlantBase.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.StopFlash();
					}
					else if (gridList[j].CurrPlantBase.CarryPlant != null && gridList[j].CurrPlantBase.CarryPlant.GetPlantType() == plantType)
					{
						gridList[j].CurrPlantBase.CarryPlant.StopFlash();
					}
				}
			}
		}
	}

	public void AllGraveOutZombie()
	{
		for (int i = 0; i < mapList.Count; i++)
		{
			for (int j = 0; j < mapList[i].GridList.Count; j++)
			{
				if (mapList[i].GridList[j].HaveGraveStone)
				{
					ZombieManager.Instance.OutGround(LV.Instance.GraveZombie[Random.Range(0, LV.Instance.GraveZombie.Count)], mapList[i].GridList[j].Position, null, needArm: true, isHyp: false, purple: false);
				}
			}
		}
	}

	public void AllMapWaterOutZombie()
	{
		if (LV.Instance.WaterZombieNum <= 0 || LV.Instance.WaterZombie.Count == 0)
		{
			return;
		}
		for (int i = 0; i < mapList.Count; i++)
		{
			List<Grid> list = new List<Grid>();
			for (int j = 0; j < mapList[i].GridList.Count; j++)
			{
				if (mapList[i].GridList[j].isNoIceWater && mapList[i].GridList[j].Point.x > 3)
				{
					list.Add(mapList[i].GridList[j]);
				}
			}
			if (list.Count > 0)
			{
				for (int k = 0; k < LV.Instance.WaterZombieNum; k++)
				{
					ZombieManager.Instance.OutGround(LV.Instance.WaterZombie[Random.Range(0, LV.Instance.WaterZombie.Count)], list[Random.Range(0, list.Count)].Position, null, needArm: true, isHyp: false, purple: false);
				}
			}
		}
	}
}
