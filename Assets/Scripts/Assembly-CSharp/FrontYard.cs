using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class FrontYard : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public SpriteRenderer RedStripe;

	public SpriteRenderer SnowScence;

	public List<SpriteRenderer> SnowScences = new List<SpriteRenderer>();

	public ParticleSystem FlyGrass;

	public ParticleSystem FlyGrassRight;

	public ParticleSystem FlyLeaf;

	public ParticleSystem FlyLeafRight;

	public SpriteRenderer HailSence1;

	public SpriteRenderer HailSence2;

	public SpriteRenderer HailSence3;

	public Animator LeafAnim;

	public List<SpriteRenderer> ScenceItem = new List<SpriteRenderer>();

	private bool SnowScenceOpen;

	private Coroutine SnowCoroutine;

	private List<Snow> SnowsList = new List<Snow>();

	private List<MapCloud> mapClouds = new List<MapCloud>();

	public override Vector2Int MapGridNum => new Vector2Int(9, 5);

	public override List<Grid> GridList => gridList;

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	private List<Puddle> puddles => MapManager.Instance.puddles;

	protected override void InitMap()
	{
		Vector3 vector = base.transform.position + new Vector3(-6f, 2.5f);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GridList.Add(new Grid(new Vector2Int(j, i), vector + new Vector3(1.36f * (float)j, -1.63f * (float)i, 0f)));
			}
		}
		TimeInitMap(SkyManager.Instance.Time);
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewWeatherEff());
		StartCoroutine(SpawnSkySun());
		RedStripe.enabled = false;
		ParticleSystem.EmissionModule emission = FlyGrassRight.emission;
		ParticleSystem.EmissionModule emission2 = FlyLeafRight.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission = FlyGrass.emission;
		emission2 = FlyLeaf.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
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
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
	}

	private IEnumerator RenewWeatherEff()
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
						Vector3 vector = new Vector3(Random.Range(-7.5f, 10f) + base.transform.position.x, Random.Range(3f, -5f) + base.transform.position.y);
						GameObject gameObject = null;
						if (SkyManager.Instance.RainScale > 0)
						{
							gameObject = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
						}
						else if (SkyManager.Instance.HailScale > 0)
						{
							Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(vector);
							if (Vector2.Distance(gridByWorldPos.Position, vector) > 0.8f || gridByWorldPos.CoverNum <= 0)
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
				int num4 = 1;
				if (SkyManager.Instance.WindTowardRight)
				{
					num4 = -1;
				}
				bool flag = Random.Range(0, 3) > 0;
				if (flag)
				{
					component.transform.position = new Vector3(17 * num4, Random.Range(-6f, 2.7f));
				}
				else
				{
					component.transform.position = new Vector3(17 * num4, Random.Range(4.5f, 6f));
				}
				component.CreateInit(flag, this);
				component.SetTimeColor(SnowScence.color);
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

	private void SetSnowColor(Color color)
	{
		SnowScence.color = color;
		for (int i = 0; i < SnowScences.Count; i++)
		{
			SnowScences[i].color = color;
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

	protected override void TimeChange(int time)
	{
		LightOpenClose();
		if (time == 300)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Night;
			ChangeSprite.sprite = Day;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
			SetSnowColor(new Color(1f, 1f, 1f, SnowScence.color.a));
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
		for (int i = 0; i < MapManager.Instance.puddles.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(MapManager.Instance.puddles[i].transform.position) == this)
			{
				MapManager.Instance.puddles[i].spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, MapManager.Instance.puddles[i].spriteRenderer.color.a);
			}
		}
		for (int j = 0; j < SnowsList.Count; j++)
		{
			SnowsList[j].TimeChange();
			SnowsList[j].ChangeColor(SnowScence.color);
		}
		for (int k = 0; k < ScenceItem.Count; k++)
		{
			ScenceItem[k].color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, 1f);
		}
		for (int l = 0; l < mapClouds.Count; l++)
		{
			mapClouds[l].SetTimeColor(SnowScence.color);
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
		if (GameManager.Instance.isClient || !LVManager.Instance.GameIsStart)
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
				if (gridList[m].Point.x >= GraveStoneLine && !gridList[m].HaveGraveStone)
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
		int num4 = 60 - SkyManager.Instance.RainScale * 5;
		if (num4 < 10)
		{
			num4 = 10;
		}
		if (Random.Range(0, num4) <= num4 - 2)
		{
			return;
		}
		int num5 = Random.Range(0, gridList.Count);
		if (gridList[num5].isHavePuddle || puddles.Count >= 5)
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
				vector = gridList[num5].Position;
			}
			break;
		case 1:
			if (num5 > 1 && num5 < gridList.Count - 1 && !gridList[num5 - 1].isHavePuddle && gridList[num5 - 1].Point.y == gridList[num5].Point.y && (!gridList[num5 - 2].isHavePuddle || gridList[num5 - 2].Point.y != gridList[num5].Point.y) && (!gridList[num5 + 1].isHavePuddle || gridList[num5 + 1].Point.y != gridList[num5].Point.y))
			{
				flag = true;
				list2.Add(gridList[num5]);
				list2.Add(gridList[num5 - 1]);
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
		component.CreateInit(list2, vector + new Vector2(0f, -0.3f), puddleSpawn.OnlineId);
		component.transform.SetParent(base.transform);
		MapManager.Instance.puddles.Add(component);
	}

	public override List<Vector2> GetShowZombiePos(int PosNum)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < PosNum; i++)
		{
			float num = Random.Range(-5f, 3.2f);
			float maxInclusive = (num - 3.2f) / -2.56f + 9.4f;
			list.Add(new Vector3(Random.Range(8.5f, maxInclusive), num) + base.transform.position);
		}
		list.Sort((Vector2 x, Vector2 y) => -x.y.CompareTo(y.y));
		return list;
	}

	public override void SpawnMower()
	{
		if (!GameManager.Instance.isClient)
		{
			SpMower();
			if (GameManager.Instance.isServer)
			{
				SynMap synMap = new SynMap();
				synMap.SynCode[0] = 2;
				synMap.mapPos = base.transform.position;
				SocketServer.Instance.SendMapSyn(synMap);
			}
		}
	}

	private IEnumerator SnowScenceSt()
	{
		if (SnowScenceOpen)
		{
			yield return new WaitForSeconds(30 - SkyManager.Instance.SnowScale * 2);
			SpawnSnow(Fade: true);
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
			for (int i = 0; i < SnowsList.Count; i++)
			{
				SnowsList[i].SnowOver();
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

	public override void SnowInit()
	{
		if (SkyManager.Instance.SnowScale > 0)
		{
			SpawnSnow(Fade: false);
			SnowScenceOpen = true;
			SetSnowColor(new Color(1f, 1f, 1f, 1f));
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
			Snow component = Object.Instantiate(GameManager.Instance.GameConf.Snow).GetComponent<Snow>();
			component.transform.position = gridList[j].Position;
			component.CreateInit(gridList[j], Fade);
			component.transform.SetParent(base.transform);
			if (gridList[j].Point.y == 0)
			{
				if (gridList[j].Point.x == 0 || gridList[j].Point.x == 7)
				{
					component.BigSnow.transform.localScale = new Vector3(1f, 0.94f);
				}
				if (gridList[j].Point.x == 8)
				{
					component.BigSnow.transform.localScale = new Vector3(1f, 0.88f);
				}
			}
			gridList[j].snow = component;
			SnowsList.Add(component);
		}
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
			if (gridList[i].Point.x >= GraveStoneLine && !gridList[i].HaveGraveStone)
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
			SpMower();
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

	private void SpMower()
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
			Grid farestGrid = MapManager.Instance.GetFarestGrid(base.transform.position, getLeft: true, j);
			LawnMower component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.LawnMover).GetComponent<LawnMower>();
			component.Init(farestGrid, new Vector2(farestGrid.Position.x - 1.2f, farestGrid.Position.y - 0.2f));
			component.transform.SetParent(base.transform);
			component.OnlineID = j;
			mowers.Add(component);
		}
	}

	public override void WindScaleReset()
	{
		int windScale = SkyManager.Instance.WindScale;
		ParticleSystem.EmissionModule emission;
		ParticleSystem.EmissionModule emission2;
		if (SkyManager.Instance.WindTowardRight)
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
		else if (SkyManager.Instance.WindTowardRight)
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
		if (SkyManager.Instance.WindTowardRight)
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

	public override void SetStripe(int lineX)
	{
		float num = -5.36f;
		num = lineX switch
		{
			1 => -4.12f, 
			2 => -2.56f, 
			3 => -1.33f, 
			4 => 0f, 
			5 => 1.36f, 
			6 => 2.64f, 
			7 => 4.04f, 
			8 => 5.36f, 
			_ => -5.36f, 
		};
		RedStripe.enabled = true;
		RedStripe.transform.position = new Vector3(num, -0.7f);
	}

	public override void MapCloudClear(MapCloud cloud)
	{
		mapClouds.Remove(cloud);
	}
}
