using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampFront : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public SpriteRenderer Tree;

	public SpriteRenderer SunStone;

	public SwampStone swampx;

	public override Vector2Int MapGridNum => new Vector2Int(10, 7);

	public override Vector2 MapHalfLengthWidth => new Vector2(14.3f, 6.2f);

	public override List<Grid> GridList => gridList;

	private List<Puddle> puddles => MapManager.Instance.puddles;

	public override Color32 SplashColor => new Color32(90, 214, 160, byte.MaxValue);

	public override float EndLine => -7.5f;

	protected override int mapTempt => 15;

	protected override void InitMap()
	{
		swampx.StartInit();
		Vector3 vector = base.transform.position + new Vector3(-5.75f, 2.8f);
		for (int i = 0; i < 10; i++)
		{
			float y = 0f;
			if (i > 4)
			{
				y = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(i, 0), vector + new Vector3(1.33f * (float)i, y, 0f)));
			if (i == 0 || i == 1 || i == 2 || i == 8 || i == 9)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (i == 4)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
			GridList[GridList.Count - 1].isShadow = true;
		}
		for (int j = 0; j < 10; j++)
		{
			float num = 0f;
			if (j > 4)
			{
				num = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(j, 1), vector + new Vector3(1.33f * (float)j, num - 1.15f, 0f)));
			if (j == 2 || j == 3)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (j == 4)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
			if (j != 2 && j != 3)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
		}
		for (int k = 0; k < 10; k++)
		{
			float num2 = 0f;
			if (k > 5)
			{
				num2 = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(k, 2), vector + new Vector3(1.33f * (float)k, num2 - 2.5f, 0f)));
			if (k == 5)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
			if (k == 6)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].needChangeLine = true;
			}
			if (k != 0 && k != 1 && k != 2 && k != 3)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
		}
		for (int l = 0; l < 10; l++)
		{
			float num3 = 0f;
			if (l > 5)
			{
				num3 = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(l, 3), vector + new Vector3(1.33f * (float)l, num3 - 3.6f, 0f)));
			if (l == 2 || l == 3)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (l == 5)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
			if (l != 0 && l != 1 && l != 2 && l != 3)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
		}
		for (int m = 0; m < 10; m++)
		{
			float num4 = 0f;
			if (m > 6)
			{
				num4 = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(m, 4), vector + new Vector3(1.33f * (float)m, num4 - 4.7f, 0f)));
			if (m != 0)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (m == 6)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
		}
		for (int n = 0; n < 10; n++)
		{
			float num5 = 0f;
			if (n > 5)
			{
				num5 = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(n, 5), vector + new Vector3(1.33f * (float)n, num5 - 5.95f, 0f)));
			if (n == 0 || n == 1 || n == 2 || n == 3 || n == 8 || n == 9)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (n == 5)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
			if (n == 6 || n == 7)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				if (n == 7)
				{
					GridList[GridList.Count - 1].needChangeLine = true;
				}
			}
			if (n == 0 || n == 1 || n == 2)
			{
				GridList[GridList.Count - 1].isShadow = true;
			}
		}
		for (int num6 = 0; num6 < 10; num6++)
		{
			float num7 = 0f;
			if (num6 > 4)
			{
				num7 = 0.5f;
			}
			GridList.Add(new Grid(new Vector2Int(num6, 6), vector + new Vector3(1.33f * (float)num6, num7 - 7.3f, 0f)));
			if (num6 == 0 || num6 == 1 || num6 == 2 || num6 == 7 || num6 == 8 || num6 == 9)
			{
				GridList[GridList.Count - 1].isWaterGrid = true;
			}
			if (num6 == 4)
			{
				GridList[GridList.Count - 1].isOccupied = true;
				GridList[GridList.Count - 1].isSlope = true;
				GridList[GridList.Count - 1].Position += new Vector2(0f, 0.25f);
			}
			GridList[GridList.Count - 1].isShadow = true;
		}
		TimeInitMap(SkyManager.Instance.Time);
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewRain());
	}

	public override void TimeInitMap(int time)
	{
		puddleColor = Color.white;
		Tree.color = new Color(1f, 1f, 1f);
		SunStone.color = new Color(1f, 1f, 1f);
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
			Tree.color = new Color(0.7f, 0.7f, 1f);
			SunStone.color = new Color(0.7f, 0.7f, 1f);
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
				int num3 = Random.Range(0, 8);
				Vector3 position = Vector3.zero;
				switch (num3)
				{
				case 0:
					position = new Vector3(Random.Range(-7.9f, -1f) + base.transform.position.x, Random.Range(-0.4f, 0.7f) + base.transform.position.y);
					break;
				case 1:
					position = new Vector3(Random.Range(-7.8f, -4f) + base.transform.position.x, Random.Range(-1.5f, -0.4f) + base.transform.position.y);
					break;
				case 2:
					position = new Vector3(Random.Range(-8.1f, -5.4f) + base.transform.position.x, Random.Range(-2.7f, -1.7f) + base.transform.position.y);
					break;
				case 3:
					position = new Vector3(Random.Range(9.3f, 13.5f) + base.transform.position.x, Random.Range(-4.2f, 1.7f) + base.transform.position.y);
					break;
				case 4:
					position = new Vector3(Random.Range(-3.6f, -1.7f) + base.transform.position.x, Random.Range(1.2f, 1.7f) + base.transform.position.y);
					break;
				case 5:
					position = new Vector3(Random.Range(-3.6f, -1.7f) + base.transform.position.x, Random.Range(-1.4f, -0.7f) + base.transform.position.y);
					break;
				case 6:
					position = new Vector3(Random.Range(-5f, 0.1f) + base.transform.position.x, Random.Range(-2.3f, -1.7f) + base.transform.position.y);
					break;
				case 7:
					position = new Vector3(Random.Range(4.2f, 6.3f) + base.transform.position.x, Random.Range(-3.5f, -1.3f) + base.transform.position.y);
					break;
				}
				if (num3 > 3)
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
		swampx.TimeChange();
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
			Tree.color = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
			SunStone.color = new Color(0.7f + 0.3f * num, 0.7f + 0.3f * num, 1f);
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
			Tree.color = new Color(1f, 1f, 1f - 0.3f * num2);
			SunStone.color = new Color(1f, 1f, 1f - 0.3f * num2);
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
			Tree.color = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
			SunStone.color = new Color(1f - 0.3f * num3, 1f - 0.3f * num3, 0.7f + 0.3f * num3);
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

	public override void ZombieDeadEvent(ZombieBase zombie)
	{
		if (zombie.CurrMap == this)
		{
			swampx.ZombieDead(zombie);
		}
	}

	public override bool SpSpawnZombie(out Vector2 pos, out int SPCode)
	{
		pos = new Vector3(2.8f, -8.7f) + base.transform.position;
		SPCode = 0;
		return Random.Range(0, 12) > 10;
	}
}
