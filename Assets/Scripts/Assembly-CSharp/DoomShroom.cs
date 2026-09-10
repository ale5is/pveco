using System.Collections.Generic;
using UnityEngine;

public class DoomShroom : PlantBase
{
	public SpriteRenderer SleepHead;

	public override float MaxHp => 300f;

	protected override int attackValue => 1800;

	protected override bool isShroom => true;

	protected override void OnInitForPlace()
	{
		SetAnimChange(11);
		needFlatDead = false;
		SleepHead.enabled = false;
	}

	protected override void DeadrattleEvent()
	{
		if (isSleeping)
		{
			return;
		}
		EffectPanel.Instance.Spark(new Color(1f, 0.8f, 1f, 0.8f), 0.1f, base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Doomm, base.transform.position, isAll: true);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 5f, isHypno, needCapsule: false);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 5f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(GetAttackValue() / aroundPlant.Count, Vector2.zero, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(GetAttackValue() / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(GetAttackValue());
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(GetAttackValue(), Vector2.zero, null);
			}
		}
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(base.currGrid, 1);
		for (int m = 0; m < aroundGrid.Count; m++)
		{
			aroundGrid[m].ClearLadder();
			if (aroundGrid[m].snow != null)
			{
				aroundGrid[m].snow.DirctClear(6, synClient: false);
			}
		}
		List<Obstacle> aroundObstacle = MapManager.Instance.GetAroundObstacle(base.transform.position, 3.6f);
		for (int n = 0; n < aroundObstacle.Count; n++)
		{
			aroundObstacle[n].HurtThis(GetAttackValue());
			if (aroundObstacle[n] is SteelWheel)
			{
				aroundObstacle[n].GetComponent<SteelWheel>().PushForce(8f, base.transform.position);
			}
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Doom).GetComponent<Doom>().CreateInit(base.currGrid.Position, GetBulletSortOrder());
		Grid grid = base.currGrid;
		CameraControl.Instance.ShakeCamera(base.transform.position);
		grid.HaveCrater = true;
	}

	protected override void GoAwakeSpecial()
	{
		SleepHead.enabled = false;
		SetAnimChange(11);
		needFlatDead = false;
	}

	protected override void GoSleepSpecial()
	{
		SleepHead.enabled = true;
		SetAnimChange(21);
		needFlatDead = true;
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.reverse_explosion, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		Dead(isFlat: false, 0f, synClient: true);
	}
}
