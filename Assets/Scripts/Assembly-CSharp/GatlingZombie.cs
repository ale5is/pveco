using System.Collections.Generic;
using UnityEngine;

public class GatlingZombie : PlantZombie
{
	public Transform creatBulletPos;

	private int ShootNum;

	protected override GameObject Prefab => GameManager.Instance.GameConf.GatlingZombie;

	protected override int attackValue2 => 20;

	protected override int OwnerHp => 500;

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	protected override void PlantZombieInit()
	{
		ShootNum = 0;
		base.BaseAnimSpeed = 1f;
		StartActionCD(1.4f, isOver: true);
		HammerDoorHpState = new List<int> { 640, 270 };
		DoorHpState = new List<int> { 1100, 760, 360, 0 };
		DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
		ArmorHpState = new List<int> { 500, 250, 0 };
		ArmorHpStateSprite = new List<Sprite> { armor1, armor2, null };
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
			base.BaseAnimSpeed = 2.5f;
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreatePea()
	{
		if (!NormalDontAttackCondition())
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
	}

	public override void SpecialAnimEvent1()
	{
		CreatePea();
		if (ShootNum < 5)
		{
			ShootNum++;
			SetAnimChange(0);
		}
		else
		{
			ShootNum = 0;
			SetAnimChange(12);
			base.BaseAnimSpeed = 1f;
		}
	}
}
