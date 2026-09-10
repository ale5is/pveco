using UnityEngine;

public class Cabbagepult : PlantBase
{
	private bool canAttack;

	public override float MaxHp => 300f;

	protected override int attackValue => 40;

	protected override void OnInitForPlace()
	{
		StartActionCD(2.9f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreateCabbage();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		canAttack = false;
		if (ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y + 1, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			canAttack = true;
		}
		if (!canAttack && ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			canAttack = true;
		}
		if (!canAttack && ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y - 1, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			canAttack = true;
		}
		if (!canAttack)
		{
			if (MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno) != null)
			{
				canAttack = true;
			}
			if (!canAttack && MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y + 1, base.IsFacingLeft, !isHypno) != null)
			{
				canAttack = true;
			}
			if (!canAttack && MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y - 1, base.IsFacingLeft, !isHypno) != null)
			{
				canAttack = true;
			}
		}
		if (canAttack)
		{
			SetAnimChange(11);
		}
		return canAttack;
	}

	private void CreateCabbage()
	{
		if (NormalDontAttackCondition())
		{
			return;
		}
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
		}
		int num = 0;
		PlantBase plantBase = null;
		ZombieBase zombieBase = null;
		zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieBase == null)
		{
			num = 1;
			zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y + 1, base.transform.position, base.IsFacingLeft, isHypno);
		}
		if (zombieBase == null)
		{
			num = -1;
			zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y - 1, base.transform.position, base.IsFacingLeft, isHypno);
		}
		if (zombieBase == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if (plantBase == null)
			{
				num = 1;
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y + 1, base.IsFacingLeft, !isHypno);
			}
			if (plantBase == null)
			{
				num = -1;
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y - 1, base.IsFacingLeft, !isHypno);
			}
		}
		if (zombieBase != null || plantBase != null)
		{
			Cabbage component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Cabbage).GetComponent<Cabbage>();
			component.transform.SetParent(null);
			Vector3 vector;
			if (base.IsFacingLeft)
			{
				vector = MyTool.ReverseX(new Vector3(-0.27f, 0.75f, 0f));
				component.Bullet.rotation = Quaternion.Euler(0f, 0f, 123f);
			}
			else
			{
				vector = new Vector3(-0.27f, 0.75f, 0f);
				component.Bullet.rotation = Quaternion.Euler(0f, 0f, -57f);
			}
			component.Init(plantBase, zombieBase, base.transform.position + vector, GetAttackValue(), base.currGrid.Point.y + num, isHypno);
		}
	}
}
