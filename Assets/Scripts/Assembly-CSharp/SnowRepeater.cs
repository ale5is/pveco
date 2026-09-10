using UnityEngine;

public class SnowRepeater : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	private int ShootNum;

	public override float MaxHp => 300f;

	protected override int attackValue => 20;

	public override PlantType BasePlant => PlantType.SnowPea;

	public override int BasePlantSunNum => 175;

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

	protected override void OnInitForAlmanac()
	{
		base.BaseAnimSpeed = 1f;
	}

	protected override void OnInitForAll()
	{
		canFrozen = false;
		canIce = false;
	}

	protected override void OnInitForPlace()
	{
		StartActionCD(1.6f, isOver: true);
	}

	private void CreatePea()
	{
		if (!isSleeping && base.currGrid != null)
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			SnowPea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnowPea).GetComponent<SnowPea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno, ShootNum + 1, ShootNum);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + creatBulletOffsetPos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno, ShootNum + 1, ShootNum);
			}
		}
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
}
