using UnityEngine;

public class Repeater : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	private int ShootNum;

	private PlantType ownerType;

	protected override int attackValue => 20;

	public override float MaxHp => 300f;

	public override PlantType FrozenEvolution => PlantType.SnowRepeater;

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		PlantBase plantBase = null;
		if (zombieByLineMinDistance == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		}
		if (zombieByLineMinDistance != null || plantBase != null)
		{
			base.BaseAnimSpeed = 2f;
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
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + creatBulletOffsetPos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
		}
	}

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
		CreatePea();
		if (ShootNum == 0)
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

	public void SetType(PlantType plantType)
	{
		ownerType = plantType;
	}

	protected override void OnInitForAll()
	{
		if (ownerType == PlantType.RepeaterReverse)
		{
			GoBack();
		}
	}
}
