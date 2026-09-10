using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class JalapenoZombie : PlantZombie
{
	private int BoomNum;

	private Grid endGrid;

	protected override GameObject Prefab => GameManager.Instance.GameConf.JalapenoZombie;

	protected override int attackValue2 => 1800;

	protected override int OwnerHp => 500;

	protected override bool DoAction()
	{
		CheckBoom();
		return false;
	}

	protected override void PlantZombieInit()
	{
		BoomNum = 0;
		StartActionCD(1f, isOver: true);
		endGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		BoomNum++;
	}

	private void CheckBoom()
	{
		if (LV.Instance.CurrLVType == LVType.VaseBreaker)
		{
			Boom();
		}
		if (PlacePlayer == null)
		{
			if (!base.IsCriticalState && BoomNum > 0 && Random.Range(BoomNum, 18) > 16)
			{
				Boom();
			}
			if (endGrid != null && !base.IsCriticalState && Vector2.Distance(base.transform.position, endGrid.Position) < 2f)
			{
				Boom();
			}
		}
	}

	private void Boom()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Jalapenoboom, base.transform.position);
		base.CurrMap.FadeTempt += 5f;
		List<Iceroad> list = new List<Iceroad>();
		for (int i = 0; i < MapManager.Instance.iceroads.Count; i++)
		{
			MapBase mapBase = MapManager.Instance.GetCurrMap(MapManager.Instance.iceroads[i].transform.position);
			if (!(base.CurrMap != mapBase) && MapManager.Instance.iceroads[i].CurrLine == base.CurrGrid.Point.y)
			{
				list.Add(MapManager.Instance.iceroads[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].Dead();
		}
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.CurrGrid.Point.y, base.transform.position, 15f, !isHypno, needCapsule: false);
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.CurrGrid.Point.y, 15f, isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int k = 0; k < linePlant.Count; k++)
			{
				linePlant[k].BurnEvent();
				linePlant[k].Hurt(attackValue2 / linePlant.Count, Vector2.zero, null);
			}
			for (int l = 0; l < zombiesByLine.Count; l++)
			{
				zombiesByLine[l].BurnEvent();
				zombiesByLine[l].BoomHurt(attackValue2 / zombiesByLine.Count);
			}
		}
		else
		{
			for (int m = 0; m < zombiesByLine.Count; m++)
			{
				zombiesByLine[m].BurnEvent();
				zombiesByLine[m].BoomHurt(attackValue2);
			}
			for (int n = 0; n < linePlant.Count; n++)
			{
				linePlant[n].BurnEvent();
				linePlant[n].Hurt(attackValue2, Vector2.zero, null);
			}
		}
		zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.CurrGrid.Point.y, base.transform.position, 15f, isHypno, needCapsule: false);
		linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.CurrGrid.Point.y, 15f, !isHypno);
		for (int num = 0; num < zombiesByLine.Count; num++)
		{
			zombiesByLine[num].BurnEvent();
		}
		for (int num2 = 0; num2 < linePlant.Count; num2++)
		{
			linePlant[num2].BurnEvent();
		}
		List<Grid> lineAllGrid = MapManager.Instance.GetLineAllGrid(base.CurrGrid.Position, base.CurrGrid.Point.y);
		for (int num3 = 0; num3 < lineAllGrid.Count; num3++)
		{
			lineAllGrid[num3].ClearLadder();
			if (lineAllGrid[num3].snow != null)
			{
				lineAllGrid[num3].snow.DirctClear(6, synClient: false);
			}
		}
		Object.Instantiate(GameManager.Instance.GameConf.JalapenoBoom).GetComponent<JalapenoBoom>().CreateInit(base.CurrGrid, GetBulletSortOrder());
		CameraControl.Instance.ShakeCamera(base.transform.position);
		ServerSendSyn(1);
		DirectDead(canDropItem: false, 0f);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			RandomSpeed = syn.Twofloat.x;
			base.Speed = DefSpeed;
		}
		if (syn.SynCode[1] == 1)
		{
			Boom();
		}
	}
}
