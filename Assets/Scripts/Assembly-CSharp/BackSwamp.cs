using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class BackSwamp : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public SpriteRenderer Tree1;

	public SpriteRenderer Tree2;

	public SpriteRenderer Fence;

	public SpriteRenderer TombStone;

	private int SunNum;

	public List<SpriteRenderer> StoneSuns = new List<SpriteRenderer>();

	public override Vector2Int MapGridNum => new Vector2Int(11, 6);

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	public override List<Grid> GridList => gridList;

	private List<Puddle> puddles => MapManager.Instance.puddles;

	public override Color32 SplashColor => new Color32(90, 214, 160, byte.MaxValue);

	public override float EndLine => -8.5f;

	public override bool IsWaterShow => true;

	protected override int mapTempt => 15;

	protected override void InitMap()
	{
		Vector3 vector = base.transform.position + new Vector3(-7.1f, 2.7f);
		for (int i = -2; i < 0; i++)
		{
			GridList.Add(new Grid(new Vector2Int(i, 0), vector + new Vector3(1.33f * (float)i, 0f, 0f)));
			if (i == -2)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (i == -1)
			{
				GridList[GridList.Count - 1].isOccupied = true;
			}
		}
		for (int j = 0; j < 11; j++)
		{
			GridList.Add(new Grid(new Vector2Int(j, 0), vector + new Vector3(1.33f * (float)j, 0f, 0f)));
			if (j > 1)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			GridList[GridList.Count - 1].isShadow = true;
		}
		for (int k = 0; k < 11; k++)
		{
			GridList.Add(new Grid(new Vector2Int(k, 1), vector + new Vector3(1.33f * (float)k, -1.35f, 0f)));
			if (k > 0)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (k > 1 && k < 6)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
		}
		for (int l = 0; l < 11; l++)
		{
			GridList.Add(new Grid(new Vector2Int(l, 2), vector + new Vector3(1.33f * (float)l, -2.7f, 0f)));
			if (l < 4 || l > 7)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (l > 2 && l < 6)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
			if (l == 4)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].needChangeLine = true;
			}
		}
		for (int m = 0; m < 11; m++)
		{
			GridList.Add(new Grid(new Vector2Int(m, 3), vector + new Vector3(1.33f * (float)m, -4.05f, 0f)));
			if (m < 3 || m > 7)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (m != 0 && m != 1 && m != 2 && m != 6 && m != 7)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
			if (m == 7)
			{
				GridList[GridList.Count - 1].needChangeLine = true;
			}
			if (m == 6 || m == 7)
			{
				GridList[GridList.Count - 1].isOccupied = true;
			}
		}
		for (int n = 0; n < 11; n++)
		{
			GridList.Add(new Grid(new Vector2Int(n, 4), vector + new Vector3(1.33f * (float)n, -5.4f, 0f)));
			if (n < 4 || n > 6)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (n > 6)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
			if (n == 9)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].needChangeLine = true;
			}
		}
		for (int num = -2; num < 0; num++)
		{
			GridList.Add(new Grid(new Vector2Int(num, 5), vector + new Vector3(1.33f * (float)num, -6.75f, 0f)));
			if (num == -2)
			{
				GridList[GridList.Count - 1].isShadow = true;
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (num == -1)
			{
				GridList[GridList.Count - 1].isOccupied = true;
			}
		}
		for (int num2 = 0; num2 < 11; num2++)
		{
			GridList.Add(new Grid(new Vector2Int(num2, 5), vector + new Vector3(1.33f * (float)num2, -6.75f, 0f)));
			if (num2 > 0)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (num2 > 6)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
		}
		TimeInitMap(SkyManager.Instance.Time);
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewRain());
		if (!GameManager.Instance.isClient)
		{
			StartCoroutine(RunTombStone());
		}
		for (int num3 = 0; num3 < StoneSuns.Count; num3++)
		{
			StoneSuns[num3].enabled = false;
		}
	}

	public override void TimeInitMap(int time)
	{
		puddleColor = Color.white;
		Tree1.color = new Color(1f, 1f, 1f);
		Tree2.color = new Color(1f, 1f, 1f);
		Fence.color = new Color(1f, 1f, 1f);
		TombStone.color = new Color(1f, 1f, 1f);
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
			Tree1.color = new Color(0.7f, 0.7f, 1f);
			Tree2.color = new Color(0.7f, 0.7f, 1f);
			Fence.color = new Color(0.7f, 0.7f, 1f);
			TombStone.color = new Color(0.7f, 0.7f, 1f);
		}
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
	}

	private IEnumerator RenewRain()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			if (!(CameraControl.Instance.CurrMap == this) || SkyManager.Instance.RainScale <= 0)
			{
				continue;
			}
			int num = SkyManager.Instance.RainScale - 5;
			if (num < 0)
			{
				num = 0;
			}
			int num2 = Random.Range(num, SkyManager.Instance.RainScale);
			for (int i = 0; i < num2; i++)
			{
				int num3 = Random.Range(0, 14);
				Vector3 position = Vector3.zero;
				switch (num3)
				{
				case 0:
					position = new Vector3(Random.Range(3.2f, 12.8f), Random.Range(-4.7f, 2.9f)) + base.transform.position;
					break;
				case 1:
					position = new Vector3(Random.Range(-4.7f, 6.6f), Random.Range(0.7f, 3f)) + base.transform.position;
					break;
				case 2:
					position = new Vector3(Random.Range(-6f, -4f), Random.Range(-5.3f, 1.5f)) + base.transform.position;
					break;
				case 3:
					position = new Vector3(Random.Range(-7.3f, -6.5f), Random.Range(-3f, 0f)) + base.transform.position;
					break;
				case 4:
					position = new Vector3(Random.Range(-11.7f, -9.3f), Random.Range(2.3f, 3.3f)) + base.transform.position;
					break;
				case 5:
					position = new Vector3(Random.Range(-4f, 2.8f), Random.Range(-5.3f, -3.8f)) + base.transform.position;
					break;
				case 6:
					position = new Vector3(Random.Range(-14f, -11.8f), Random.Range(-6.1f, -4f)) + base.transform.position;
					break;
				case 7:
					position = new Vector3(Random.Range(-8.6f, -5.3f), Random.Range(2f, 3.2f)) + base.transform.position;
					break;
				case 8:
					position = new Vector3(Random.Range(-9.3f, -6.7f), Random.Range(0.7f, 1.6f)) + base.transform.position;
					break;
				case 9:
					position = new Vector3(Random.Range(-1f, 2.7f), Random.Range(-1.2f, 0.5f)) + base.transform.position;
					break;
				case 10:
					position = new Vector3(Random.Range(-3.5f, 0.2f), Random.Range(-2.2f, -1f)) + base.transform.position;
					break;
				case 11:
					position = new Vector3(Random.Range(-2.3f, 1.2f), Random.Range(-3.4f, -2.2f)) + base.transform.position;
					break;
				case 12:
					position = new Vector3(Random.Range(-8.4f, -6.5f), Random.Range(-3.4f, -4.9f)) + base.transform.position;
					break;
				case 13:
					position = new Vector3(Random.Range(-9f, -8f), Random.Range(-3.3f, 0.4f)) + base.transform.position;
					break;
				}
				if (num3 < 7)
				{
					GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_circle);
					obj.transform.SetParent(base.transform);
					obj.transform.position = position;
				}
				else
				{
					GameObject obj2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
					obj2.transform.SetParent(base.transform);
					obj2.transform.position = position;
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
		}
		if (time >= 300 && time <= 420)
		{
			float num = (float)(time - 300) / 120f;
			ChangeSprite.color = new Color(1f, 1f, 1f, num);
			puddleColor = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			Tree1.color = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			Tree2.color = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			Fence.color = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			TombStone.color = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
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
			Tree1.color = new Color(1f, 1f, 1f - 0.3f * num2);
			Tree2.color = new Color(1f, 1f, 1f - 0.3f * num2);
			Fence.color = new Color(1f, 1f, 1f - 0.3f * num2);
			TombStone.color = new Color(1f, 1f, 1f - 0.3f * num2);
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
			Tree1.color = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
			Tree2.color = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
			Fence.color = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
			TombStone.color = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
		}
		for (int i = 0; i < MapManager.Instance.puddles.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(MapManager.Instance.puddles[i].transform.position) == this)
			{
				MapManager.Instance.puddles[i].spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, MapManager.Instance.puddles[i].spriteRenderer.color.a);
			}
		}
		if (!LVManager.Instance.GameIsStart || GameManager.Instance.isClient)
		{
			return;
		}
		if (Random.Range(0, 10) >= 8)
		{
			int num4 = 0;
			List<Grid> list = new List<Grid>();
			for (int j = 0; j < gridList.Count; j++)
			{
				if (gridList[j].HaveGraveStone)
				{
					num4++;
				}
				if (gridList[j].Point.x >= GraveStoneLine && !gridList[j].HaveGraveStone && !gridList[j].isWaterGrid && !gridList[j].isOccupied)
				{
					list.Add(gridList[j]);
				}
			}
			if (num4 < GraveStoneNum && list.Count > 0)
			{
				list[Random.Range(0, list.Count)].HaveGraveStone = true;
			}
		}
		if (SkyManager.Instance.RainScale == 0 && puddles.Count > 0 && Random.Range(0, 15) > 10)
		{
			int index = Random.Range(0, puddles.Count);
			if (MapManager.Instance.GetCurrMap(puddles[index].transform.position) == this)
			{
				puddles[index].StartDisappear();
			}
		}
	}

	private IEnumerator RunTombStone()
	{
		int time = 5;
		int notNum = 0;
		while (true)
		{
			yield return new WaitForSeconds(time);
			if (!LVManager.Instance.GameIsStart)
			{
				continue;
			}
			Sun aRandomSun = SkyManager.Instance.GetARandomSun(this);
			if (notNum > 12)
			{
				notNum = 0;
				if (SkyManager.Instance.GetIsDay())
				{
					float downY = -1.9f + base.transform.position.y;
					float x = Random.Range(0.8f, 2f);
					SkyManager.Instance.CreateSkySun(new Vector3(x, 7.2f + base.transform.position.y), downY, SunType.Normal);
				}
			}
			if (aRandomSun == null)
			{
				time = 1;
				notNum++;
				continue;
			}
			aRandomSun.TombFlyToPosDes(new Vector3(1.58f, 0.5f) + base.transform.position, () =>
			{
				SunNum++;
				for (int i = 0; i < StoneSuns.Count; i++)
				{
					StoneSuns[i].enabled = i < SunNum;
				}
				if (GameManager.Instance.isServer && SunNum < 5)
				{
					SynMap synMap = new SynMap();
					synMap.SynCode[0] = SunNum;
					synMap.mapPos = base.transform.position;
					SocketServer.Instance.SendMapSyn(synMap);
				}
			});
			time = 10 + SunNum * 3;
			if (time > 22)
			{
				time = 22;
			}
		}
	}

	public override bool SpSpawnZombie(out Vector2 pos, out int SPCode)
	{
		pos = new Vector3(1.2f, -1.8f) + base.transform.position;
		SPCode = 1;
		bool flag = false;
		if (SunNum == 0)
		{
			flag = Random.Range(0, 50) > 48;
		}
		else if (SunNum == 1)
		{
			flag = Random.Range(0, 20) > 18;
		}
		else if (SunNum == 3)
		{
			flag = Random.Range(0, 10) > 8;
		}
		else if (SunNum >= 4)
		{
			flag = true;
		}
		if (flag)
		{
			SunNum--;
			for (int i = 0; i < StoneSuns.Count; i++)
			{
				StoneSuns[i].enabled = i < SunNum;
			}
			if (GameManager.Instance.isServer && SunNum < 5)
			{
				SynMap synMap = new SynMap();
				synMap.SynCode[0] = SunNum;
				synMap.mapPos = base.transform.position;
				SocketServer.Instance.SendMapSyn(synMap);
			}
		}
		if (SunNum < 0)
		{
			SunNum = 0;
		}
		return flag;
	}

	protected override void OwnerSynMap(SynMap Syn)
	{
		for (int i = 0; i < StoneSuns.Count; i++)
		{
			StoneSuns[i].enabled = i < Syn.SynCode[0];
		}
	}
}
