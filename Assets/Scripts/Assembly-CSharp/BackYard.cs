using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class BackYard : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public Transform PoolSpark;

	private bool SnowScenceOpen;

	private Coroutine SnowCoroutine;

	public SpriteRenderer SnowScence;

	public List<SpriteRenderer> SnowScences = new List<SpriteRenderer>();

	private List<Snow> SnowsList = new List<Snow>();

	public ParticleSystem FlyGrass;

	public ParticleSystem FlyGrassRight;

	public ParticleSystem FlyLeaf;

	public ParticleSystem FlyLeafRight;

	public SpriteRenderer HailSence1;

	public SpriteRenderer HailSence2;

	public SpriteRenderer HailSence3;

	public Animator LeafAnim;

	public List<SpriteRenderer> ScenceItem = new List<SpriteRenderer>();

	private List<float> PoolSparkRateOverTime = new List<float>();

	private List<MapCloud> mapClouds = new List<MapCloud>();

	public override Vector2Int MapGridNum => new Vector2Int(9, 6);

	public override List<Grid> GridList => gridList;

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	private List<Puddle> puddles => MapManager.Instance.puddles;

	protected override int mapTempt => 9;

	protected override List<int> SpawnLimitLine => new List<int> { 2, 3 };

	public override bool IsFacingLeft => true;

	private void ResetPoolSpark(bool open)
	{
		ParticleSystem[] componentsInChildren = PoolSpark.GetComponentsInChildren<ParticleSystem>();
		if (open)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.EmissionModule emission = componentsInChildren[i].emission;
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(PoolSparkRateOverTime[i]);
			}
		}
		else
		{
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				ParticleSystem.EmissionModule emission = componentsInChildren[j].emission;
				emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
			}
		}
	}

	protected override void InitMap()
	{
		ParticleSystem[] componentsInChildren = PoolSpark.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			ParticleSystem.EmissionModule emission = componentsInChildren[i].emission;
			PoolSparkRateOverTime.Add(emission.rateOverTime.constant);
		}
		for (int j = 0; j < 2; j++)
		{
			for (int k = 0; k < 9; k++)
			{
				GridList.Add(new Grid(new Vector2Int(k, j), base.transform.position + new Vector3(-5.9f, 2.7f) + new Vector3(1.33f * (float)k, -1.5f * (float)j)));
			}
		}
		for (int l = 0; l < 2; l++)
		{
			for (int m = 0; m < 9; m++)
			{
				GridList.Add(new Grid(new Vector2Int(m, l + 2), base.transform.position + new Vector3(-5.9f, -0.45f) + new Vector3(1.33f * (float)m, -1.2f * (float)l)));
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
		}
		for (int n = 0; n < 2; n++)
		{
			for (int num = 0; num < 9; num++)
			{
				GridList.Add(new Grid(new Vector2Int(num, n + 4), base.transform.position + new Vector3(-5.9f, -3.4f) + new Vector3(1.33f * (float)num, -1.2f * (float)n)));
			}
		}
		TimeInitMap(SkyManager.Instance.Time);
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewRain());
		StartCoroutine(SpawnSkySun());
		ParticleSystem.EmissionModule emission2 = FlyGrassRight.emission;
		ParticleSystem.EmissionModule emission3 = FlyLeafRight.emission;
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission3.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission2 = FlyGrass.emission;
		emission3 = FlyLeaf.emission;
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission3.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
	}

	public override void TimeInitMap(int time)
	{
		puddleColor = Color.white;
		SetSnowColor(new Color(1f, 1f, 1f, SnowScence.color.a));
		if (time > 300 && time < 900)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Night;
			ChangeSprite.sprite = Day;
		}
		else if (time > 900 && time < 1020)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Day;
			ChangeSprite.sprite = Sunset;
		}
		else
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Sunset;
			ChangeSprite.sprite = Night;
			puddleColor = new Color(0.7f, 0.7f, 1f);
			SetSnowColor(new Color(0.78f, 0.78f, 1f, SnowScence.color.a));
		}
		ResetPoolSpark(SkyManager.Instance.Time > 420 && SkyManager.Instance.Time < 1080 && SkyManager.Instance.RainScale < 4 && SkyManager.Instance.SnowScale == 0 && SkyManager.Instance.HailScale < 4);
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
	}

	private IEnumerator RenewRain()
	{
		int cloudi = 0;
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			cloudi++;
			if (CameraControl.Instance.CurrMap == this)
			{
				int num = SkyManager.Instance.RainScale;
				if (num == 0)
				{
					num = SkyManager.Instance.HailScale;
				}
				if (num > 0)
				{
					int num2 = num - 5;
					if (num2 < 0)
					{
						num2 = 0;
					}
					int num3 = Random.Range(num2, num) * 3;
					for (int i = 0; i < num3; i++)
					{
						int num4 = Random.Range(0, 4);
						Vector3 vector = Vector3.zero;
						switch (num4)
						{
						case 0:
							vector = new Vector3(Random.Range(-8f, 7f) + base.transform.position.x, Random.Range(3f, 0.3f) + base.transform.position.y);
							break;
						case 1:
							vector = new Vector3(Random.Range(-8f, 7f) + base.transform.position.x, Random.Range(-2.7f, -6f) + base.transform.position.y);
							break;
						case 2:
							vector = new Vector3(Random.Range(9f, 14f) + base.transform.position.x, Random.Range(-5f, 2f) + base.transform.position.y);
							break;
						case 3:
							vector = new Vector3(Random.Range(-6.3f, 5f) + base.transform.position.x, Random.Range(-0.5f, -2.2f) + base.transform.position.y);
							if (SkyManager.Instance.RainScale > 0)
							{
								GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_circle);
								obj.transform.SetParent(base.transform);
								obj.transform.position = vector;
							}
							else if (SkyManager.Instance.HailScale > 0 && Random.Range(0, 3) > 1)
							{
								Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(vector);
								if (Vector2.Distance(gridByWorldPos.Position, vector) > 0.8f || gridByWorldPos.CoverNum <= 0)
								{
									PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(vector + new Vector3(0f, 0.8f), -1, 0.4f);
								}
							}
							break;
						}
						if (num4 >= 3)
						{
							continue;
						}
						GameObject gameObject = null;
						if (SkyManager.Instance.RainScale > 0)
						{
							gameObject = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
						}
						else if (SkyManager.Instance.HailScale > 0)
						{
							Grid gridByWorldPos2 = MapManager.Instance.GetGridByWorldPos(vector);
							if (Vector2.Distance(gridByWorldPos2.Position, vector) > 0.8f || gridByWorldPos2.CoverNum <= 0)
							{
								gameObject = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.HailParticle);
							}
						}
						if (gameObject != null)
						{
							gameObject.transform.SetParent(base.transform);
							gameObject.transform.position = vector;
						}
					}
				}
			}
			if (GameManager.Instance.isClient || cloudi < 6)
			{
				continue;
			}
			cloudi = 0;
			if (mapClouds.Count < 3 && Random.Range(0, 10) > 8)
			{
				MapCloud component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.MapCloud).GetComponent<MapCloud>();
				int num5 = 1;
				if (!SkyManager.Instance.WindTowardRight)
				{
					num5 = -1;
				}
				bool flag = Random.Range(0, 3) > 0;
				if (flag)
				{
					component.transform.position = new Vector3(17 * num5, Random.Range(-5.6f, 2.7f));
				}
				else
				{
					component.transform.position = new Vector3(17 * num5, Random.Range(4.6f, 5.7f));
				}
				component.CreateInit(flag, this);
				component.transform.SetParent(base.transform);
				mapClouds.Add(component);
				if (GameManager.Instance.isServer)
				{
					SynMap synMap = new SynMap();
					synMap.SynCode[0] = 3;
					synMap.SynCode[1] = (flag ? 1 : 2);
					synMap.SynCode[2] = component.GetSpriteId();
					synMap.mapPos = base.transform.position;
					synMap.pos = component.transform.position;
					synMap.TwoFloat = new Vector2(component.GetBaseSpeed(), 0f);
					SocketServer.Instance.SendMapSyn(synMap);
				}
			}
		}
	}

	private IEnumerator SpawnSkySun()
	{
		while (true)
		{
			yield return new WaitForSeconds(8f);
			if (!SkySunCondition())
			{
				break;
			}
			if (LVManager.Instance.GameIsStart && SkyManager.Instance.RainScale < 6 && SkyManager.Instance.SnowScale < 6 && SkyManager.Instance.HailScale < 3)
			{
				if (SkyManager.Instance.GetIsDay() && SeedBank.Instance.NeedSummonSun)
				{
					float downY = Random.Range(-4.4f, 2.6f) + base.transform.position.y;
					float x = Random.Range(-6f, 4.5f);
					SkyManager.Instance.CreateSkySun(new Vector3(x, 7.2f + base.transform.position.y), downY, SunType.Normal);
				}
				else if (!SkyManager.Instance.GetIsDay() && SeedBank.Instance.NeedSummonMoon)
				{
					float downY2 = Random.Range(-4.4f, 2.6f) + base.transform.position.y;
					float x2 = Random.Range(-6f, 4.5f);
					SkyManager.Instance.CreateSkySun(new Vector3(x2, 7.2f + base.transform.position.y), downY2, SunType.Moon);
				}
			}
		}
	}

	private void SetSnowColor(Color color)
	{
		SnowScence.color = color;
		for (int i = 0; i < SnowScences.Count; i++)
		{
			SnowScences[i].color = color;
		}
	}

	protected override void TimeChange(int time)
	{
		LightOpenClose();
		if (time == 300)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Night;
			ChangeSprite.sprite = Day;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
		}
		if (time >= 300 && time <= 420)
		{
			float num = (float)(time - 300) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num);
			puddleColor = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			SetSnowColor(new Color(0.78f + 0.22f * num, 0.78f + 0.22f * num, 1f, SnowScence.color.a));
		}
		if (time == 900)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Day;
			ChangeSprite.sprite = Sunset;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
		}
		if (time >= 900 && time <= 1020)
		{
			float num2 = (float)(time - 900) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num2);
			puddleColor = new Color(1f, 1f, 1f - 0.3f * num2);
		}
		if (time == 1020)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Sunset;
			ChangeSprite.sprite = Night;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
		}
		if (time >= 1020 && time <= 1140)
		{
			float num3 = (float)(time - 1020) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num3);
			puddleColor = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
			SetSnowColor(new Color(1f - 0.22f * num3, 1f - 0.22f * num3, 1f, SnowScence.color.a));
		}
		if (SkyManager.Instance.Time > 420 && SkyManager.Instance.Time < 1080 && SkyManager.Instance.RainScale < 5 && SkyManager.Instance.SnowScale == 0 && SkyManager.Instance.HailScale < 4)
		{
			ResetPoolSpark(open: true);
		}
		if (time > 1080 || time < 420 || SkyManager.Instance.RainScale > 4 || SkyManager.Instance.SnowScale > 0 || SkyManager.Instance.HailScale > 3)
		{
			ResetPoolSpark(open: false);
		}
		for (int i = 0; i < MapManager.Instance.puddles.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(MapManager.Instance.puddles[i].transform.position) == this)
			{
				MapManager.Instance.puddles[i].spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, MapManager.Instance.puddles[i].spriteRenderer.color.a);
			}
		}
		for (int j = 0; j < ScenceItem.Count; j++)
		{
			ScenceItem[j].color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, 1f);
		}
		for (int k = 0; k < mapClouds.Count; k++)
		{
			mapClouds[k].SetTimeColor(SnowScence.color);
		}
		for (int l = 0; l < SnowsList.Count; l++)
		{
			SnowsList[l].TimeChange();
			SnowsList[l].ChangeColor(SnowScence.color);
		}
		int hailScale = SkyManager.Instance.HailScale;
		if (!HailSence1.enabled && hailScale > 0)
		{
			StartCoroutine(HailDisplay(HailSence1, isOpen: true));
		}
		if (!HailSence2.enabled && hailScale > 4)
		{
			StartCoroutine(HailDisplay(HailSence2, isOpen: true));
		}
		if (!HailSence3.enabled && hailScale > 7)
		{
			StartCoroutine(HailDisplay(HailSence3, isOpen: true));
		}
		if (hailScale <= 0)
		{
			if (Mathf.Approximately(HailSence1.color.a, 1f))
			{
				StartCoroutine(HailDisplay(HailSence1, isOpen: false));
			}
			if (Mathf.Approximately(HailSence2.color.a, 1f))
			{
				StartCoroutine(HailDisplay(HailSence2, isOpen: false));
			}
			if (Mathf.Approximately(HailSence3.color.a, 1f))
			{
				StartCoroutine(HailDisplay(HailSence3, isOpen: false));
			}
		}
		if (!LVManager.Instance.GameIsStart || GameManager.Instance.isClient)
		{
			return;
		}
		if (!SnowScenceOpen && SkyManager.Instance.SnowScale > 0)
		{
			SnowScenceOpen = true;
			if (SnowCoroutine != null)
			{
				StopCoroutine(SnowCoroutine);
			}
			SnowCoroutine = StartCoroutine(SnowScenceSt());
		}
		if (SnowScenceOpen && SkyManager.Instance.SnowScale <= 0)
		{
			SnowScenceOpen = false;
			if (SnowCoroutine != null)
			{
				StopCoroutine(SnowCoroutine);
			}
			SnowCoroutine = StartCoroutine(SnowScenceSt());
		}
		if (GraveStoneNum > 0 && Random.Range(0, 10) >= 8)
		{
			List<Grid> list = new List<Grid>();
			for (int m = 0; m < gridList.Count; m++)
			{
				if (gridList[m].Point.x >= GraveStoneLine && !gridList[m].isWaterGrid && !gridList[m].HaveGraveStone)
				{
					list.Add(gridList[m]);
				}
			}
			if (list.Count > 0)
			{
				GraveStoneNum--;
				list[Random.Range(0, list.Count)].HaveGraveStone = true;
			}
		}
		if (SkyManager.Instance.RainScale == 0 && SkyManager.Instance.HailScale == 0 && puddles.Count > 0 && Random.Range(0, 15) > 10)
		{
			int index = Random.Range(0, puddles.Count);
			if (MapManager.Instance.GetCurrMap(puddles[index].transform.position) == this)
			{
				puddles[index].StartDisappear();
			}
		}
		if (SkyManager.Instance.RainScale <= 2)
		{
			return;
		}
		int num4 = 80 - SkyManager.Instance.RainScale * 5;
		if (num4 < 10)
		{
			num4 = 10;
		}
		if (Random.Range(0, num4) <= num4 - 2)
		{
			return;
		}
		int num5 = Random.Range(0, gridList.Count);
		if (gridList[num5].isHavePuddle || gridList[num5].isWaterGrid || puddles.Count >= 8)
		{
			return;
		}
		bool flag = false;
		Vector2 vector = Vector2.zero;
		List<Grid> list2 = new List<Grid>();
		switch (Random.Range(0, 3))
		{
		case 0:
			if (num5 > 0 && num5 < gridList.Count - 1)
			{
				if ((!gridList[num5 - 1].isHavePuddle || gridList[num5 - 1].Point.y != gridList[num5].Point.y) && (!gridList[num5 + 1].isHavePuddle || gridList[num5 + 1].Point.y != gridList[num5].Point.y))
				{
					flag = true;
				}
			}
			else if (num5 == 0)
			{
				if (!gridList[1].isHavePuddle || gridList[1].Point.y != gridList[num5].Point.y)
				{
					flag = true;
				}
			}
			else if (num5 == gridList.Count - 1 && (!gridList[num5 - 1].isHavePuddle || gridList[num5 - 1].Point.y != gridList[num5].Point.y))
			{
				flag = true;
			}
			if (flag)
			{
				list2.Add(gridList[num5]);
				gridList[num5].isHavePuddle = true;
				vector = gridList[num5].Position;
			}
			break;
		case 1:
			if (num5 > 1 && num5 < gridList.Count - 1 && !gridList[num5 - 1].isHavePuddle && gridList[num5 - 1].Point.y == gridList[num5].Point.y && (!gridList[num5 - 2].isHavePuddle || gridList[num5 - 2].Point.y != gridList[num5].Point.y) && (!gridList[num5 + 1].isHavePuddle || gridList[num5 + 1].Point.y != gridList[num5].Point.y))
			{
				flag = true;
				list2.Add(gridList[num5]);
				list2.Add(gridList[num5 - 1]);
				gridList[num5].isHavePuddle = true;
				gridList[num5 - 1].isHavePuddle = true;
				vector = (gridList[num5 - 1].Position + gridList[num5].Position) * 0.5f;
			}
			break;
		case 2:
			if (num5 > 1 && num5 < gridList.Count - 2 && !gridList[num5 - 1].isHavePuddle && gridList[num5 - 1].Point.y == gridList[num5].Point.y && (!gridList[num5 - 2].isHavePuddle || gridList[num5 - 2].Point.y != gridList[num5].Point.y) && !gridList[num5 + 1].isHavePuddle && gridList[num5 + 1].Point.y == gridList[num5].Point.y && (!gridList[num5 + 2].isHavePuddle || gridList[num5 + 2].Point.y != gridList[num5].Point.y))
			{
				flag = true;
				list2.Add(gridList[num5]);
				list2.Add(gridList[num5 + 1]);
				list2.Add(gridList[num5 - 1]);
				gridList[num5 + 1].isHavePuddle = true;
				gridList[num5 - 1].isHavePuddle = true;
				gridList[num5].isHavePuddle = true;
				vector = gridList[num5].Position;
			}
			break;
		}
		if (!flag)
		{
			return;
		}
		PuddleSpawn puddleSpawn = new PuddleSpawn();
		if (GameManager.Instance.isServer)
		{
			puddleSpawn.OnlineId = SocketServer.Instance.ItemId;
			for (int n = 0; n < list2.Count; n++)
			{
				puddleSpawn.MapPos.Add(list2[n].Position);
			}
			puddleSpawn.InitPos = vector + new Vector2(0f, -0.3f);
			SocketServer.Instance.SpawnPuddle(puddleSpawn);
		}
		Puddle component = Object.Instantiate(GameManager.Instance.GameConf.Puddle).GetComponent<Puddle>();
		component.CreateInit(list2, vector + new Vector2(0f, -0.2f), puddleSpawn.OnlineId);
		component.transform.SetParent(base.transform);
		MapManager.Instance.puddles.Add(component);
	}

	public override void SpawnMower()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		SpMower(GameManager.Instance.LocalPlayerSave.SpItems.Contains(SpItem.PoolCleaner));
		if (GameManager.Instance.isServer)
		{
			SynMap synMap = new SynMap();
			synMap.SynCode[0] = 2;
			if (GameManager.Instance.LocalPlayerSave.SpItems.Contains(SpItem.PoolCleaner))
			{
				synMap.SynCode[1] = 2;
			}
			else
			{
				synMap.SynCode[1] = 1;
			}
			synMap.mapPos = base.transform.position;
			SocketServer.Instance.SendMapSyn(synMap);
		}
	}

	private IEnumerator SnowScenceSt()
	{
		if (SnowScenceOpen)
		{
			yield return new WaitForSeconds(30 - SkyManager.Instance.SnowScale * 2);
			SpawnSnow(Fade: true);
			for (int i = 0; i < gridList.Count; i++)
			{
				if (gridList[i].Point.y == 2 || gridList[i].Point.y == 3)
				{
					gridList[i].IsIce = true;
				}
			}
			float a = SnowScence.color.a;
			while (a < 1f)
			{
				a += 0.1f;
				yield return new WaitForSeconds(0.05f);
				SetSnowColor(new Color(SnowScence.color.r, SnowScence.color.g, SnowScence.color.b, a));
			}
		}
		else
		{
			yield return new WaitForSeconds(30f);
			for (int j = 0; j < SnowsList.Count; j++)
			{
				SnowsList[j].SnowOver();
			}
			for (int k = 0; k < gridList.Count; k++)
			{
				if (gridList[k].Point.y == 2 || gridList[k].Point.y == 3)
				{
					gridList[k].IsIce = false;
				}
			}
			float a = SnowScence.color.a;
			while (a > 0f)
			{
				a -= 0.1f;
				yield return new WaitForSeconds(0.05f);
				SetSnowColor(new Color(SnowScence.color.r, SnowScence.color.g, SnowScence.color.b, a));
			}
		}
		SnowCoroutine = null;
	}

	public override void SnowInit()
	{
		if (SkyManager.Instance.SnowScale > 0)
		{
			SpawnSnow(Fade: false);
			SnowScenceOpen = true;
			SetSnowColor(new Color(1f, 1f, 1f, 1f));
			for (int i = 0; i < gridList.Count; i++)
			{
				if (gridList[i].Point.y == 2 || gridList[i].Point.y == 3)
				{
					gridList[i].IsIce = true;
				}
			}
			ResetPoolSpark(open: false);
		}
		else
		{
			SnowScenceOpen = false;
			SetSnowColor(new Color(1f, 1f, 1f, 0f));
		}
		int hailScale = SkyManager.Instance.HailScale;
		if (hailScale > 0)
		{
			HailSence1.enabled = true;
			HailSence1.color = new Color(1f, 1f, 1f, 1f);
			if (hailScale > 4)
			{
				HailSence2.enabled = true;
				HailSence2.color = new Color(1f, 1f, 1f, 1f);
			}
			if (hailScale > 7)
			{
				HailSence3.enabled = true;
				HailSence3.color = new Color(1f, 1f, 1f, 1f);
			}
		}
		else
		{
			HailSence1.enabled = false;
			HailSence2.enabled = false;
			HailSence3.enabled = false;
		}
	}

	private void SpawnSnow(bool Fade)
	{
		if (SnowsList.Count > 0)
		{
			for (int i = 0; i < SnowsList.Count; i++)
			{
				SnowsList[i].SnowOpen(fade: true);
			}
			return;
		}
		for (int j = 0; j < gridList.Count; j++)
		{
			if (gridList[j].Point.y != 2 && gridList[j].Point.y != 3)
			{
				Snow component = Object.Instantiate(GameManager.Instance.GameConf.Snow).GetComponent<Snow>();
				component.transform.position = gridList[j].Position;
				if (gridList[j].Point.y == 0)
				{
					component.BigSnow.transform.localScale = new Vector3(1f, 0.8f);
					component.transform.position += new Vector3(0f, -0.1f);
					component.LowSnow.transform.localPosition += new Vector3(0f, 0.23f);
				}
				else if (gridList[j].Point.y == 1)
				{
					component.BigSnow.transform.position += new Vector3(0f, -0.1f);
				}
				else if (gridList[j].Point.y == 4)
				{
					component.BigSnow.transform.localScale = new Vector3(1f, 0.9f);
					component.LowSnow.transform.localPosition += new Vector3(0f, 0.11f);
				}
				else if (gridList[j].Point.y == 5)
				{
					component.BigSnow.transform.localScale = new Vector3(1f, 0.8f);
					component.transform.position += new Vector3(0f, -0.1f);
				}
				component.CreateInit(gridList[j], Fade);
				component.transform.SetParent(base.transform);
				gridList[j].snow = component;
				SnowsList.Add(component);
			}
		}
	}

	private IEnumerator HailDisplay(SpriteRenderer sprite, bool isOpen)
	{
		if (isOpen)
		{
			sprite.enabled = isOpen;
		}
		else
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.99f);
		}
		yield return new WaitForSeconds(20f);
		float a;
		if (isOpen)
		{
			a = 0f;
			while (a < 1f)
			{
				a += 0.1f;
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
				yield return new WaitForSeconds(0.05f);
			}
			yield break;
		}
		a = sprite.color.a;
		while (a > 0f)
		{
			a -= 0.1f;
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
			yield return new WaitForSeconds(0.05f);
		}
		sprite.enabled = isOpen;
	}

	public override void SpawnAllGraveStone()
	{
		if (GameManager.Instance.isClient || GraveStoneNum <= 0)
		{
			return;
		}
		List<Grid> list = new List<Grid>();
		for (int i = 0; i < gridList.Count; i++)
		{
			if (gridList[i].Point.x >= GraveStoneLine && !gridList[i].isWaterGrid && !gridList[i].HaveGraveStone)
			{
				list.Add(gridList[i]);
			}
		}
		int num = 0;
		for (int j = 0; j < GraveStoneNum; j++)
		{
			if (list.Count == 0)
			{
				break;
			}
			int index = Random.Range(0, list.Count);
			list[index].HaveGraveStone = true;
			list.RemoveAt(index);
			num++;
		}
		GraveStoneNum -= num;
	}

	protected override void OwnerSynMap(SynMap Syn)
	{
		if (Syn.SynCode[0] == 2)
		{
			SpMower(Syn.SynCode[1] == 2);
		}
		else if (Syn.SynCode[0] == 3)
		{
			MapCloud component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.MapCloud).GetComponent<MapCloud>();
			component.transform.position = Syn.pos;
			component.ClientInit(Syn.SynCode[1] == 1, this, Syn.TwoFloat.x, Syn.SynCode[2]);
			component.transform.SetParent(base.transform);
			mapClouds.Add(component);
		}
	}

	private void SpMower(bool HavePoolCleaner)
	{
		for (int i = 0; i < mowers.Count; i++)
		{
			if (!mowers[i].IsRun)
			{
				mowers[i].DestroyMower();
			}
		}
		for (int j = 0; j < MapGridNum.y; j++)
		{
			LawnMower lawnMower = null;
			Grid farestGrid = MapManager.Instance.GetFarestGrid(base.transform.position, getLeft: true, j);
			lawnMower = ((!(farestGrid.isWaterGrid & HavePoolCleaner)) ? PoolManager.Instance.GetObj(GameManager.Instance.GameConf.LawnMover).GetComponent<LawnMower>() : PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PoolCleaner).GetComponent<LawnMower>());
			lawnMower.Init(farestGrid, new Vector2(farestGrid.Position.x - 1.2f, farestGrid.Position.y - 0.2f));
			lawnMower.transform.SetParent(base.transform);
			lawnMower.OnlineID = j;
			mowers.Add(lawnMower);
		}
	}

	public override void WindScaleReset()
	{
		int windScale = SkyManager.Instance.WindScale;
		ParticleSystem.EmissionModule emission;
		ParticleSystem.EmissionModule emission2;
		if (!SkyManager.Instance.WindTowardRight)
		{
			emission = FlyGrassRight.emission;
			emission2 = FlyLeafRight.emission;
		}
		else
		{
			emission = FlyGrass.emission;
			emission2 = FlyLeaf.emission;
		}
		float num = windScale * 5;
		float num2 = (windScale - 2) * 5;
		if (num > 15f)
		{
			num = 15f;
		}
		if (num2 > 15f)
		{
			num2 = 15f;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(num);
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(num2);
		LeafAnim.speed = 0.8f + (float)windScale * 0.2f;
		if (windScale == 0)
		{
			LeafAnim.SetInteger("Change", 0);
		}
		else if (!SkyManager.Instance.WindTowardRight)
		{
			LeafAnim.SetInteger("Change", 1);
		}
		else
		{
			LeafAnim.SetInteger("Change", 2);
		}
	}

	public override void WindDirectionReset()
	{
		WindScaleReset();
		ParticleSystem.EmissionModule emission;
		ParticleSystem.EmissionModule emission2;
		if (!SkyManager.Instance.WindTowardRight)
		{
			emission = FlyGrass.emission;
			emission2 = FlyLeaf.emission;
		}
		else
		{
			emission = FlyGrassRight.emission;
			emission2 = FlyLeafRight.emission;
		}
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
	}

	public override void MapCloudClear(MapCloud cloud)
	{
		mapClouds.Remove(cloud);
	}
}
