using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Roof : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	private bool SnowScenceOpen;

	private Coroutine SnowCoroutine;

	public SpriteRenderer SnowScence;

	public SpriteRenderer HailSence1;

	public SpriteRenderer HailSence2;

	public SpriteRenderer HailSence3;

	public ParticleSystem FlyGrass;

	public ParticleSystem FlyGrassRight;

	public ParticleSystem FlyLeaf;

	public ParticleSystem FlyLeafRight;

	private List<Snow> SnowsList = new List<Snow>();

	private List<MapCloud> mapClouds = new List<MapCloud>();

	public override Vector2Int MapGridNum => new Vector2Int(9, 5);

	public override List<Grid> GridList => gridList;

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	public override float EndLine => -7.1f;

	protected override void InitMap()
	{
		Vector3 vector = base.transform.position + new Vector3(-5.6f, 1.55f);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				GridList.Add(new Grid(new Vector2Int(j, i), vector + new Vector3(1.31f * (float)j, -1.36f * (float)i, 0f)));
				if (j < 5)
				{
					GridList[gridList.Count - 1].Position += new Vector2(0f, 0.33f * (float)(j - 1));
					GridList[gridList.Count - 1].isSlope = true;
				}
				else
				{
					GridList[gridList.Count - 1].Position += new Vector2(0f, 1.25f);
				}
				GridList[gridList.Count - 1].isHardGrid = true;
			}
		}
		TimeInitMap(SkyManager.Instance.Time);
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewWeatherEff());
		StartCoroutine(SpawnSkySun());
		if (!GameManager.Instance.isClient)
		{
			for (int k = 0; k < GridList.Count; k++)
			{
				if (GridList[k].Point.x == 0 || GridList[k].Point.x == 1 || GridList[k].Point.x == 2)
				{
					PlantBase newPlant = PlantManager.Instance.GetNewPlant(PlantType.Pot);
					SeedBank.Instance.PlantConfirm(newPlant, GridList[k], 0, 3, null);
				}
			}
		}
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
		SnowScence.color = new Color(1f, 1f, 1f, SnowScence.color.a);
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
			SnowScence.color = new Color(0.78f, 0.78f, 1f, SnowScence.color.a);
		}
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
	}

	protected override void TimeChange(int time)
	{
		if (time == 300)
		{
			base.transform.GetComponent<SpriteRenderer>().sprite = Night;
			ChangeSprite.sprite = Day;
			ChangeSprite.color = new Color(1f, 1f, 1f, 0f);
			SnowScence.color = new Color(1f, 1f, 1f, SnowScence.color.a);
		}
		if (time >= 300 && time <= 420)
		{
			float num = (float)(time - 300) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num);
			puddleColor = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			SnowScence.color = new Color(0.78f + 0.22f * num, 0.78f + 0.22f * num, 1f, SnowScence.color.a);
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
			SnowScence.color = new Color(1f - 0.22f * num3, 1f - 0.22f * num3, 1f, SnowScence.color.a);
		}
		for (int i = 0; i < SnowsList.Count; i++)
		{
			SnowsList[i].TimeChange();
			SnowsList[i].ChangeColor(SnowScence.color);
		}
		for (int j = 0; j < mapClouds.Count; j++)
		{
			mapClouds[j].SetTimeColor(SnowScence.color);
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
	}

	private IEnumerator RenewWeatherEff()
	{
		int cloudi = 0;
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			cloudi++;
			if (CameraControl.Instance.CurrMap == this && SkyManager.Instance.RainScale > 0)
			{
				int num = SkyManager.Instance.RainScale - 5;
				if (num < 0)
				{
					num = 0;
				}
				int num2 = Random.Range(num, SkyManager.Instance.RainScale);
				for (int i = 0; i < num2; i++)
				{
					Vector2 vector;
					switch (Random.Range(0, 3))
					{
					case 0:
						vector = new Vector3(Random.Range(0.5f, 3.3f), Random.Range(5.2f, -3.4f)) + base.transform.position;
						break;
					case 1:
					{
						float num4 = Random.Range(-6.5f, 5.25f);
						float maxInclusive2 = (num4 - 0.2f) * 0.31f + 3.2f;
						float minInclusive2 = num4 * 0.21f - 3.6f;
						vector = new Vector3(num4, Random.Range(minInclusive2, maxInclusive2)) + base.transform.position;
						break;
					}
					default:
					{
						float num3 = Random.Range(5.7f, 12f);
						float maxInclusive = (0.2f - num3) * 0.3f + 3.2f;
						float minInclusive = num3 * 0.23f - 4.8f;
						vector = new Vector3(num3, Random.Range(minInclusive, maxInclusive)) + base.transform.position;
						break;
					}
					}
					GameObject gameObject = null;
					if (SkyManager.Instance.RainScale > 0)
					{
						gameObject = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
					}
					else if (SkyManager.Instance.HailScale > 0)
					{
						gameObject = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.HailParticle);
					}
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.position = vector;
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
				if (SkyManager.Instance.WindTowardRight)
				{
					num5 = -1;
				}
				bool flag = Random.Range(0, 3) > 0;
				if (flag)
				{
					component.transform.position = new Vector3(17 * num5, Random.Range(-4.3f, 3f));
				}
				else
				{
					component.transform.position = new Vector3(17 * num5, Random.Range(0.9f, 5.7f));
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
			if (LVManager.Instance.GameIsStart && SkyManager.Instance.RainScale < 6)
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

	public override List<Vector2> GetShowZombiePos(int PosNum)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < PosNum; i++)
		{
			float num = Random.Range(7.5f, 11f);
			float maxInclusive = -0.3f * (num - 7.5f) + 2.3f;
			float minInclusive = -0.3f * (num - 7.5f) - 3.3f;
			list.Add(new Vector3(num, Random.Range(minInclusive, maxInclusive)) + base.transform.position);
		}
		list.Sort((Vector2 x, Vector2 y) => -x.y.CompareTo(y.y));
		return list;
	}

	public override void SpawnMower()
	{
		if (!GameManager.Instance.isClient && GameManager.Instance.LocalPlayerSave.SpItems.Contains(SpItem.RoofCleaner))
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
				SnowScence.color = new Color(SnowScence.color.r, SnowScence.color.g, SnowScence.color.b, a);
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
				SnowScence.color = new Color(SnowScence.color.r, SnowScence.color.g, SnowScence.color.b, a);
			}
		}
		SnowCoroutine = null;
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
			if (gridList[j].Point.x < 5)
			{
				component.transform.rotation = Quaternion.Euler(0f, 0f, 8f);
			}
			if (gridList[j].Point.y == 0)
			{
				component.BigSnow.transform.localScale = new Vector3(1f, 0.85f, 1f);
			}
			gridList[j].snow = component;
			SnowsList.Add(component);
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
			LawnMower component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.RoofCleaner).GetComponent<LawnMower>();
			component.Init(farestGrid, new Vector2(farestGrid.Position.x - 1.1f - (float)j * 0.08f, farestGrid.Position.y - 0.2f));
			component.transform.SetParent(base.transform);
			component.OnlineID = j;
			mowers.Add(component);
		}
	}

	public override void SnowInit()
	{
		if (SkyManager.Instance.SnowScale > 0)
		{
			SpawnSnow(Fade: false);
			SnowScenceOpen = true;
			SnowScence.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			SnowScenceOpen = false;
			SnowScence.color = new Color(1f, 1f, 1f, 0f);
		}
	}

	public override void MapCloudClear(MapCloud cloud)
	{
		mapClouds.Remove(cloud);
	}
}
