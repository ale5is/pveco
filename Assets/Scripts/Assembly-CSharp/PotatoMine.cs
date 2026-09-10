using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoMine : PlantBase
{
	private bool isGrowOver;

	private int GlowSeconds = 15;

	private Coroutine GrowCoroutine;

	private ZombieBase zombieBoom;

	public override float MaxHp => 300f;

	protected override int attackValue => 1800;

	public override bool IsLowPlant => true;

	public override bool CanPlaceOnWaterCarry => false;

	protected override List<string> DontPlayAnim => new List<string> { "anim_armed", "anim_idle" };

	protected override void OnInitForPlace()
	{
		GlowSeconds = 15;
		if (LV.Instance.CurrLVType == LVType.IZombie)
		{
			isGrowOver = true;
			StartActionCD(2f, isOver: true);
		}
		else
		{
			isGrowOver = false;
			PlayAnim("anim_idle", 0);
			StartActionCD(GlowSeconds, isOver: false);
		}
		zombieBoom = null;
	}

	protected override bool DoAction()
	{
		if (isGrowOver)
		{
			if (Attack())
			{
				Dead();
				return true;
			}
		}
		else
		{
			isGrowOver = true;
			SetAnimChange(11);
			StartActionCD(2f, isOver: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
		}
		return false;
	}

	private IEnumerator WaitGrow()
	{
		while (GlowSeconds > 0)
		{
			yield return new WaitForSeconds(1f);
			GlowSeconds--;
		}
		isGrowOver = true;
		SetAnimChange(11);
	}

	private bool Attack()
	{
		if (base.currGrid == null)
		{
			return false;
		}
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(base.currGrid.Point.y, base.transform.position, isHypno);
		if (zombieByLineMinDisNoDir == null)
		{
			return false;
		}
		if (Mathf.Abs(zombieByLineMinDisNoDir.transform.position.x - base.transform.position.x) < 0.7f)
		{
			return true;
		}
		return false;
	}

	protected override void DeadrattleEvent()
	{
		if (isGrowOver)
		{
			Boom();
		}
	}

	private void Boom()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PotatoParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PotatoMineboom, base.transform.position);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 1.2f, isHypno, needCapsule: false);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i] != zombieBoom)
			{
				zombies[i].BoomHurt(GetAttackValue());
			}
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		if (GameManager.Instance.isClient)
		{
			return;
		}
		int num = 0;
		for (int j = 0; j < zombies.Count; j++)
		{
			if (zombies[j].GetDead())
			{
				num++;
			}
		}
		if (num >= 5)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.Potato5, PlacePlayer);
		}
	}

	protected override void GoAwakeSpecial()
	{
	}

	protected override void GoSleepSpecial()
	{
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (zombie != null && isGrowOver && zombie.Hp > 0)
		{
			zombieBoom = zombie;
			zombie.BoomHurt(GetAttackValue());
			Dead();
		}
	}
}
