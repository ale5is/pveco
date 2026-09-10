using System.Collections;
using System.Collections.Generic;
using SaveClass;
using SocketSave;
using UnityEngine;

public abstract class MapBase : MonoBehaviour
{
	public Fog fog;

	public SpriteRenderer ChangeSprite;

	public Sprite Day;

	public Sprite Sunset;

	public Sprite Night;

	public Sprite GotoSprite;

	public int GraveStoneNum;

	public int GraveStoneLine;

	public Color puddleColor = new Color(1f, 1f, 1f);

	public List<GameObject> MapLights = new List<GameObject>();

	protected List<LawnMower> mowers = new List<LawnMower>();

	protected List<int> SpawnZombieLine = new List<int>();

	private List<GridSelector> gridSelectors = new List<GridSelector>();

	private float currTempt;

	[SerializeField]
	private float targetTempt;

	private float manmadeTempt;

	private float natureTempt;

	[SerializeField]
	private float fadeTempt;

	[SerializeField]
	private float entityTempt;

	private int mapTemptDiff = 15;

	protected List<CustomTile> tiles = new List<CustomTile>();

	public abstract Vector2Int MapGridNum { get; }

	public abstract Vector2 MapHalfLengthWidth { get; }

	public abstract List<Grid> GridList { get; }

	public virtual float EndLine { get; } = -7.6f;

	public virtual bool IsWaterShow { get; }

	public virtual bool IsFacingLeft { get; }

	public virtual Color32 SplashColor { get; } = Color.white;

	protected virtual int mapTempt { get; } = 10;

	protected virtual List<int> SpawnLimitLine { get; }

	public float CurrTempt
	{
		get
		{
			return currTempt;
		}
		private set
		{
			currTempt = value;
			if (currTempt > 50f)
			{
				currTempt = 50f;
			}
			if (currTempt < -50f)
			{
				currTempt = -50f;
			}
			Timetable.Instance.UpdateTempt(this);
		}
	}

	public float FadeTempt
	{
		get
		{
			return fadeTempt;
		}
		set
		{
			if (fadeTempt != value)
			{
				fadeTempt = value;
				if (Mathf.Abs(fadeTempt) < 0.2f)
				{
					fadeTempt = 0f;
				}
				if (fadeTempt < -30f)
				{
					fadeTempt = -30f;
				}
				if (fadeTempt > 30f)
				{
					fadeTempt = 30f;
				}
				RefreshManmadeTemperature();
			}
		}
	}

	public float EntityTempt
	{
		get
		{
			return entityTempt;
		}
		set
		{
			if (entityTempt != value)
			{
				entityTempt = value;
				if (entityTempt < -30f)
				{
					entityTempt = -30f;
				}
				if (entityTempt > 30f)
				{
					entityTempt = 30f;
				}
				RefreshManmadeTemperature();
			}
		}
	}

	public void BaseInitMap()
	{
		InitMap();
		RefreshNatureTemperature();
		RefreshManmadeTemperature();
		CurrTempt = targetTempt;
		StartCoroutine(GoTargetTempt());
	}

	public void BaseTimeChange(int time)
	{
		TimeChange(time);
		RefreshNatureTemperature();
	}

	protected abstract void InitMap();

	public abstract void TimeInitMap(int time);

	protected abstract void TimeChange(int time);

	public virtual void ZombieDeadEvent(ZombieBase zombie)
	{
	}

	public virtual void SpawnMower()
	{
	}

	public virtual List<Vector2> GetShowZombiePos(int PosNum)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < PosNum; i++)
		{
			list.Add(new Vector3(Random.Range(9.5f, 13f), Random.Range(-5f, 3.2f)) + base.transform.position);
		}
		list.Sort((Vector2 x, Vector2 y) => -x.y.CompareTo(y.y));
		return list;
	}

	public void LightOpenClose()
	{
		if (MapLights.Count == 0)
		{
			return;
		}
		if (!GetIsDay() || GobalLight.Instance.gobalLight.intensity < 0.5f)
		{
			if (!MapLights[0].activeSelf)
			{
				for (int i = 0; i < MapLights.Count; i++)
				{
					MapLights[i].SetActive(value: true);
				}
			}
		}
		else if (MapLights[0].activeSelf)
		{
			for (int j = 0; j < MapLights.Count; j++)
			{
				MapLights[j].SetActive(value: false);
			}
		}
	}

	public bool GetIsDay()
	{
		if (SkyManager.Instance.Time < 1140 && SkyManager.Instance.Time > 360)
		{
			return true;
		}
		return false;
	}

	public void SynMap(SynMap Syn)
	{
		if (Syn.SynCode[0] == 1)
		{
			for (int i = 0; i < mowers.Count; i++)
			{
				if (Syn.SynCode[1] == i)
				{
					mowers[i].Launch(synClient: true);
					break;
				}
			}
		}
		OwnerSynMap(Syn);
	}

	public virtual void SpawnAllGraveStone()
	{
	}

	public virtual void SnowInit()
	{
	}

	public virtual void WindScaleReset()
	{
	}

	public virtual void WindDirectionReset()
	{
	}

	public virtual void MapCloudClear(MapCloud cloud)
	{
	}

	protected virtual void OwnerSynMap(SynMap Syn)
	{
	}

	public virtual bool SpSpawnZombie(out Vector2 pos, out int SPCode)
	{
		pos = default;
		SPCode = 0;
		return false;
	}

	public int GetRandomLine(int spCode)
	{
		int num = 0;
		int num2 = 0;
		do
		{
			num2++;
			if (SpawnZombieLine.Count == 0)
			{
				for (int i = 0; i < MapGridNum.y; i++)
				{
					SpawnZombieLine.Add(i);
				}
				SpawnZombieLine.Shuffle();
			}
			num = SpawnZombieLine[0];
			if (spCode != 1 || SpawnLimitLine == null || !SpawnLimitLine.Contains(num))
			{
				break;
			}
			SpawnZombieLine.RemoveAt(0);
		}
		while (num2 <= 15);
		SpawnZombieLine.RemoveAt(0);
		return num;
	}

	public virtual void SetStripe(int lineX)
	{
	}

	private IEnumerator GoTargetTempt()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			float num = targetTempt - CurrTempt;
			if (num != 0f)
			{
				if (Mathf.Abs(num) < 0.2f)
				{
					CurrTempt = targetTempt;
				}
				else
				{
					if (num > 0f && num < 2f)
					{
						num = 2f;
					}
					if (num < 0f && num > -2f)
					{
						num = -2f;
					}
					CurrTempt += num / 20f;
				}
			}
			if (FadeTempt != 0f)
			{
				float num2 = ((!(FadeTempt > 0f)) ? ((CurrTempt + 50f) / 50f) : (2f - (CurrTempt + 50f) / 50f));
				if (num2 > 1.8f)
				{
					num2 = 1.8f;
				}
				if (num2 < 0.2f)
				{
					num2 = 0.2f;
				}
				if (FadeTempt > 0f)
				{
					num2 *= -1f;
				}
				FadeTempt += num2;
			}
		}
	}

	private float GetTimeTempt()
	{
		float num = 0f;
		float num2 = SkyManager.Instance.Time;
		if (num2 >= 0f && num2 < 360f)
		{
			num = 0.1f - num2 / 360f * 0.1f;
		}
		else if (num2 >= 360f && num2 < 600f)
		{
			num = (num2 - 360f) / 240f * 0.7f;
		}
		else if (num2 >= 600f && num2 < 840f)
		{
			num = (num2 - 600f) / 240f * 0.3f + 0.7f;
		}
		else if (num2 >= 840f && num2 < 1200f)
		{
			num = 1f - (num2 - 840f) / 360f * 0.6f;
		}
		else if (num2 >= 1200f && num2 < 1440f)
		{
			num = 0.4f - (num2 - 1200f) / 240f * 0.3f;
		}
		return (float)mapTemptDiff * num;
	}

	public void RefreshManmadeTemperature()
	{
		float num = 0f;
		float num2 = Mathf.Abs(FadeTempt) / 10f;
		float num3 = Mathf.Abs(FadeTempt) % 10f;
		if (num2 >= 3f)
		{
			num += 17f;
		}
		else if (num2 >= 2f)
		{
			num += 15f;
			num += num3 / 5f;
		}
		else if (num2 >= 1f)
		{
			num += 10f;
			num += num3 / 2f;
		}
		else if (num2 >= 0f)
		{
			num += num3;
		}
		if (FadeTempt < 0f)
		{
			num *= -1f;
		}
		float num4 = 0f;
		float num5 = Mathf.Abs(EntityTempt) / 10f;
		float num6 = Mathf.Abs(EntityTempt) % 10f;
		if (num2 >= 3f)
		{
			num4 += 17f;
		}
		else if (num5 >= 2f)
		{
			num4 += 15f;
			num4 += num6 / 5f;
		}
		else if (num5 >= 1f)
		{
			num4 += 10f;
			num4 += num6;
		}
		else if (num5 >= 0f)
		{
			num4 += num6 / 2f;
		}
		if (EntityTempt < 0f)
		{
			num4 *= -1f;
		}
		manmadeTempt = num + num4;
		targetTempt = manmadeTempt + natureTempt;
		if (targetTempt > 30f)
		{
			float num7 = natureTempt - 30f;
			if (num7 < 0f)
			{
				float num8 = manmadeTempt + num7;
				targetTempt = num8 * 0.6f + natureTempt;
			}
		}
		else if (targetTempt < -30f)
		{
			float num9 = natureTempt + 30f;
			if (num9 > 0f)
			{
				float num10 = manmadeTempt + num9;
				targetTempt = num10 * 0.6f + natureTempt;
			}
		}
	}

	public void RefreshNatureTemperature()
	{
		natureTempt = (float)SkyManager.Instance.GetWeatherTempt() + GetTimeTempt() + (float)mapTempt + (float)LV.Instance.LvTemperature;
		targetTempt = manmadeTempt + natureTempt;
	}

	protected bool SkySunCondition()
	{
		if (SeedBank.Instance.isNoCD && PlayerManager.Instance.SunInfinite)
		{
			return false;
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.LastStand))
		{
			return false;
		}
		return true;
	}

	public void DisplayGridSelector()
	{
		if (gridSelectors.Count == 0)
		{
			for (int i = 0; i < GridList.Count; i++)
			{
				GridSelector component = Object.Instantiate(GameManager.Instance.GameConf.GridSelector).GetComponent<GridSelector>();
				component.transform.position = GridList[i].Position;
				component.transform.SetParent(base.transform);
				component.grid = GridList[i];
				gridSelectors.Add(component);
			}
		}
		for (int j = 0; j < gridSelectors.Count; j++)
		{
			gridSelectors[j].gameObject.SetActive(value: true);
		}
	}

	public void HideGridSelector()
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].gameObject.SetActive(value: false);
		}
	}

	public List<GridSelector> GetGridSelector()
	{
		return gridSelectors;
	}

	public virtual void CustomMapEditInit(int vertical, int horizontal, CustomMapSave save)
	{
	}

	public List<CustomTile> GetAroundTile(Vector2Int point)
	{
		List<Vector2Int> list = new List<Vector2Int>
		{
			point + new Vector2Int(0, 1),
			point + new Vector2Int(1, 0),
			point + new Vector2Int(-1, 0),
			point + new Vector2Int(0, -1),
			point + new Vector2Int(1, 1),
			point + new Vector2Int(-1, -1),
			point + new Vector2Int(1, -1),
			point + new Vector2Int(-1, 1)
		};
		List<CustomTile> list2 = new List<CustomTile>();
		for (int i = 0; i < tiles.Count; i++)
		{
			if (list.Contains(tiles[i].CurrGrid.Point))
			{
				list2.Add(tiles[i]);
			}
			if (list2.Count >= list.Count)
			{
				break;
			}
		}
		return list2;
	}

	public virtual int ChangeDecoration(int decorType)
	{
		return 0;
	}
}
