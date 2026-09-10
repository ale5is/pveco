using System.Collections.Generic;
using UnityEngine;

public class GloomShroom : PlantBase
{
	public override float MaxHp => 300f;

	public override PlantType BasePlant => PlantType.FumeShroom;

	public override int BasePlantSunNum => 75;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override void OnInitForPlace()
	{
		StartActionCD(1.9f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Fume, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		HurtZombie();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, isHypno, needCapsule: true);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
		if (zombies.Count > 0 || aroundPlant.Count > 0)
		{
			SetAnimChange(11);
		}
		return false;
	}

	private void HurtZombie()
	{
		if (base.currGrid != null)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CircleFumeParticle).transform.position = base.transform.position;
			List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, isHypno, needCapsule: true);
			for (int i = 0; i < zombies.Count; i++)
			{
				zombies[i].Hurt(GetAttackValue(), Vector2.zero, isHard: false);
			}
			List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
			for (int j = 0; j < aroundPlant.Count; j++)
			{
				aroundPlant[j].Hurt(GetAttackValue(), Vector2.zero, null);
			}
		}
	}
}
