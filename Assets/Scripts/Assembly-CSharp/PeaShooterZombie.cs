using UnityEngine;

public class PeaShooterZombie : PlantZombie
{
	public Transform creatBulletPos;

	protected override GameObject Prefab => GameManager.Instance.GameConf.PeaShooterZombie;

	protected override int attackValue2 => 20;

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	protected override void PlantZombieInit()
	{
		base.BaseAnimSpeed = 1f;
		StartActionCD(1.4f, isOver: true);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.CurrGrid.Point.y, base.transform.position, base.IsFacingLeft, !isHypno);
		PlantBase plantBase = null;
		if (zombieByLineMinDistance == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.CurrGrid.Point.y, base.IsFacingLeft, isHypno);
		}
		if (zombieByLineMinDistance != null || plantBase != null)
		{
			base.BaseAnimSpeed = 3f;
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreatePea()
	{
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
		}
		Pea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
		component.transform.SetParent(null);
		if (base.IsFacingLeft)
		{
			component.Init(attackValue2, creatBulletPos.position, base.CurrGrid.Point.y, Vector2.left, GetBulletSortOrder(), !isHypno);
		}
		else
		{
			component.Init(attackValue2, creatBulletPos.position, base.CurrGrid.Point.y, Vector2.right, GetBulletSortOrder(), !isHypno);
		}
	}

	public override void SpecialAnimEvent1()
	{
		CreatePea();
		base.BaseAnimSpeed = 1f;
		SetAnimChange(12);
	}
}
