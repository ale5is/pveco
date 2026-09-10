using UnityEngine;

public class WinterMelonpult : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.26f, 1.13f);

	public override float MaxHp => 300f;

	protected override int attackValue => 80;

	public override PlantType BasePlant => PlantType.Melonpult;

	public override int BasePlantSunNum => 300;

	public override float Temperature => -1f;

	protected override void OnInitForAll()
	{
		canFrozen = false;
		canIce = false;
	}

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
		CreateMelon();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		PlantBase plantBase = null;
		ZombieBase zombieByLineMinDisCanNoCollGet = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieByLineMinDisCanNoCollGet == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		}
		if (zombieByLineMinDisCanNoCollGet != null || plantBase != null)
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreateMelon()
	{
		if (base.currGrid == null)
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
		PlantBase plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		ZombieBase zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
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
			Vector3 vector;
			if (base.IsFacingLeft)
			{
				vector = MyTool.ReverseX(creatBulletOffsetPos);
				component.Bullet.rotation = Quaternion.Euler(0f, 0f, 78f);
			}
			else
			{
				vector = creatBulletOffsetPos;
				component.Bullet.rotation = Quaternion.Euler(0f, 0f, 258f);
			}
			component.Init(plantBase, zombieBase, base.transform.position + vector, GetAttackValue(), base.currGrid.Point.y, isHypno);
		}
	}
}
