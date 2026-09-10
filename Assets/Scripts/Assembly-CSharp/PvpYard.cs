using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvpYard : MapBase
{
	private List<Grid> gridList = new List<Grid>();

	public SpriteRenderer RightLine;

	public SpriteRenderer LeftLine;

	public override Vector2Int MapGridNum => new Vector2Int(11, 5);

	public override Vector2 MapHalfLengthWidth => new Vector2(11.4f, 6.2f);

	public override List<Grid> GridList => gridList;

	public override float EndLine => -15f;

	protected override void InitMap()
	{
		bool localIsBlueTeam = PvPSelector.Instance.LocalIsBlueTeam;
		Vector3 vector = base.transform.position + new Vector3(-6.8f, 2.5f);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				Grid grid = new Grid(new Vector2Int(j, i), vector + new Vector3(1.36f * (float)j, -1.63f * (float)i, 0f));
				if (grid.Point.x < 4)
				{
					if (localIsBlueTeam)
					{
						grid.CurrGridType = TeamType.BlueTeam;
					}
					else
					{
						grid.CurrGridType = TeamType.RedTeam;
					}
				}
				else if (grid.Point.x > 6)
				{
					if (localIsBlueTeam)
					{
						grid.CurrGridType = TeamType.RedTeam;
					}
					else
					{
						grid.CurrGridType = TeamType.BlueTeam;
					}
				}
				else
				{
					grid.CurrGridType = TeamType.AllTeam;
				}
				GridList.Add(grid);
			}
		}
		TimeInitMap(SkyManager.Instance.Time);
		TimeChange(SkyManager.Instance.Time);
		StartCoroutine(RenewRain());
		if (localIsBlueTeam || (GameManager.Instance.isClient && SpectatorList.Instance.LocalIsSpectator && PvPSelector.Instance.BlueTeamNames.Contains(GameManager.Instance.HostName)))
		{
			Sprite sprite = RightLine.sprite;
			RightLine.sprite = LeftLine.sprite;
			LeftLine.sprite = sprite;
		}
		StartCoroutine(PlaceTarget());
	}

	private IEnumerator PlaceTarget()
	{
		yield return new WaitForSeconds(0.1f);
		if (GameManager.Instance.isClient)
		{
			yield break;
		}
		for (int i = 0; i < GridList.Count; i++)
		{
			if (GridList[i].Point.x == 0)
			{
				ZombieBase newZombie = ZombieManager.Instance.GetNewZombie(ZombieType.PvPTarget);
				if (SpectatorList.Instance.LocalIsSpectator)
				{
					newZombie.PlacePlayer = PvPSelector.Instance.RedTeamNames[0];
				}
				else if (PvPSelector.Instance.LocalIsRedTeam)
				{
					newZombie.PlacePlayer = PvPSelector.Instance.RedTeamNames[0];
				}
				else
				{
					newZombie.PlacePlayer = PvPSelector.Instance.BlueTeamNames[0];
				}
				ZombieManager.Instance.UpdateZombie(ZombieType.PvPTarget, newZombie, GridList[i].Position - new Vector2(1.8f, 0f), GridList[i].Point.y);
				newZombie.RatThis(synClient: true);
			}
			if (GridList[i].Point.x == 10)
			{
				ZombieBase newZombie2 = ZombieManager.Instance.GetNewZombie(ZombieType.PvPTarget);
				if (PvPSelector.Instance.LocalIsBlueTeam)
				{
					newZombie2.PlacePlayer = PvPSelector.Instance.RedTeamNames[0];
				}
				else
				{
					newZombie2.PlacePlayer = PvPSelector.Instance.BlueTeamNames[0];
				}
				ZombieManager.Instance.UpdateZombie(ZombieType.PvPTarget, newZombie2, GridList[i].Position + new Vector2(1.8f, 0f), GridList[i].Point.y);
			}
		}
	}

	protected override void TimeChange(int time)
	{
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
		}
		for (int i = 0; i < MapManager.Instance.puddles.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(MapManager.Instance.puddles[i].transform.position) == this)
			{
				MapManager.Instance.puddles[i].spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, MapManager.Instance.puddles[i].spriteRenderer.color.a);
			}
		}
	}

	public override void TimeInitMap(int time)
	{
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
		}
		ChangeSprite.color = new Color(1f, 1f, 1f, 1f);
	}

	private IEnumerator RenewRain()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
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
					GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Rain_splash);
					obj.transform.SetParent(base.transform);
					obj.transform.position = new Vector3(Random.Range(-7.5f, 10f) + base.transform.position.x, Random.Range(3f, -5f) + base.transform.position.y);
				}
			}
		}
	}

	private IEnumerator SpawnSkySun()
	{
		while (true)
		{
			yield return new WaitForSeconds(16f);
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
}
