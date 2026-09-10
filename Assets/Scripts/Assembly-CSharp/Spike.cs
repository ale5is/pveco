using System.Collections.Generic;
using UnityEngine;

public class Spike : PlantBase
{
	public override float MaxHp => 300f;

	public override bool ZombieCanEat => false;

	protected override Vector2 offSet => new Vector2(0f, -0.1f);

	protected override int attackValue => 20;

	protected override bool HaveShadow => false;

	public override bool CanCarryed => false;

	public override bool CanProtect => false;

	public override bool IsLowPlant => true;

	protected override void OnInitForPlace()
	{
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		Attack();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		if (ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 0.66f, isHypno, needCapsule: true).Count > 0)
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void Attack()
	{
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 0.66f, isHypno, needCapsule: true);
		for (int i = 0; i < zombiesByLine.Count; i++)
		{
			zombiesByLine[i].Hurt(GetAttackValue(), Vector2.up, isHard: false);
		}
	}
}
