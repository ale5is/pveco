using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class DoomShroomZombie : PlantZombie
{
	private int BoomNum;

	private Grid endGrid;

	protected override GameObject Prefab => GameManager.Instance.GameConf.DoomShroomZombie;

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
			Doom();
		}
		if (PlacePlayer == null)
		{
			if (!base.IsCriticalState && BoomNum > 0 && Random.Range(BoomNum, 18) > 16)
			{
				Doom();
			}
			if (endGrid != null && !base.IsCriticalState && Vector2.Distance(base.transform.position, endGrid.Position) < 2f)
			{
				Doom();
			}
		}
	}

	protected void Doom()
	{
		EffectPanel.Instance.Spark(new Color(1f, 0.8f, 1f, 0.8f), 0.1f, base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Doomm, base.transform.position, isAll: true);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 5f, !isHypno, needCapsule: false);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 5f, isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(attackValue2 / aroundPlant.Count, Vector2.zero, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(attackValue2 / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(attackValue2);
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(attackValue2, Vector2.zero, null);
			}
		}
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(base.CurrGrid, 1);
		for (int m = 0; m < aroundGrid.Count; m++)
		{
			aroundGrid[m].ClearLadder();
			if (aroundGrid[m].snow != null)
			{
				aroundGrid[m].snow.DirctClear(6, synClient: false);
			}
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Doom).GetComponent<Doom>().CreateInit(base.CurrGrid.Position, GetBulletSortOrder());
		Grid grid = base.CurrGrid;
		CameraControl.Instance.ShakeCamera(base.transform.position);
		grid.HaveCrater = true;
		ServerSendSyn(1);
		DirectDead(canDropItem: false, 0f);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 1)
		{
			Doom();
		}
		if (syn.SynCode[1] == 0)
		{
			RandomSpeed = syn.Twofloat.x;
			base.Speed = DefSpeed;
		}
	}
}
