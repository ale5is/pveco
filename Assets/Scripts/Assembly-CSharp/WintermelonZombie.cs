using System.Collections.Generic;
using UnityEngine;

public class WintermelonZombie : PlantZombie
{
	public Transform creatBulletPos;

	protected override GameObject Prefab => GameManager.Instance.GameConf.WinterMelonZombie;

	protected override int attackValue2 => 80;

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	protected override void PlantZombieInit()
	{
		StartActionCD(2.9f, isOver: true);
		ArmorHpState = new List<int> { 500, 250, 0 };
		ArmorHpStateSprite = new List<Sprite> { armor1, armor2, null };
	}

	public override void SpecialAnimEvent1()
	{
		CreateMelon();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		Grid lastPlantGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		PlantBase plantBase = null;
		ZombieBase zombieBase = null;
		if (lastPlantGrid != null)
		{
			plantBase = lastPlantGrid.CurrPlantBase;
		}
		if (plantBase == null)
		{
			zombieBase = ZombieManager.Instance.GetLastZombieByLine(base.CurrLine, base.transform.position, !base.IsFacingLeft, !isHypno);
		}
		if (zombieBase != null || plantBase != null)
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreateMelon()
	{
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
		}
		Grid lastPlantGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		PlantBase plantBase = null;
		if (lastPlantGrid != null)
		{
			plantBase = lastPlantGrid.CurrPlantBase;
		}
		ZombieBase zombieBase = ZombieManager.Instance.GetLastZombieByLine(base.CurrLine, base.transform.position, !base.IsFacingLeft, !isHypno);
		if (zombieBase != null && plantBase != null)
		{
			if (Mathf.Abs(plantBase.transform.position.x - base.transform.position.x) >= Mathf.Abs(zombieBase.transform.position.x - base.transform.position.x))
			{
				plantBase = null;
			}
			else
			{
				zombieBase = null;
			}
		}
		if (zombieBase != null || plantBase != null)
		{
			Wintermelon component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Wintermelon).GetComponent<Wintermelon>();
			component.transform.SetParent(null);
			Vector3 position;
			if (base.IsFacingLeft)
			{
				position = creatBulletPos.position;
				component.Bullet.rotation = Quaternion.Euler(0f, 0f, 93f);
			}
			else
			{
				position = creatBulletPos.position;
				component.Bullet.rotation = Quaternion.Euler(0f, 0f, 273f);
			}
			component.Init(plantBase, zombieBase, position, attackValue2, base.CurrGrid.Point.y, !isHypno);
		}
	}
}
