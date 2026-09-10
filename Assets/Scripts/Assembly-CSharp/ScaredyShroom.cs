using System.Collections.Generic;
using UnityEngine;

public class ScaredyShroom : PlantBase
{
	private bool isScared;

	private Vector3 creatBulletOffsetPos = new Vector2(0.48f, -0.14f);

	public override float MaxHp => 300f;

	protected override int attackValue => 20;

	protected override bool isShroom => true;

	protected override bool haveSpEye => true;

	protected override void OnInitForPlace()
	{
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		bool result = false;
		if (isScared)
		{
			CheackNoScare();
		}
		else if (!CheackScare())
		{
			result = CheckAttack();
		}
		return result;
	}

	public override void SpecialAnimEvent1()
	{
		CreatePuff();
		SetAnimChange(12);
		base.BaseAnimSpeed = 1f;
	}

	private bool CheackScare()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.25f, isHypno, needCapsule: false);
		List<PlantBase> list = null;
		if (zombies.Count == 0)
		{
			list = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, !isHypno);
		}
		if (zombies.Count > 0 || list.Count > 0)
		{
			isScared = true;
			SetAnimChange(13);
			return true;
		}
		return false;
	}

	private bool CheackNoScare()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.25f, isHypno, needCapsule: false);
		List<PlantBase> list = null;
		if (zombies.Count == 0)
		{
			list = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, !isHypno);
		}
		if (zombies.Count == 0 && list.Count == 0)
		{
			isScared = false;
			SetAnimChange(14);
			return true;
		}
		return false;
	}

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

	private void CreatePuff()
	{
		if (base.currGrid != null)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Puff, base.transform.position);
			ShroomPuff component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomPuff).GetComponent<ShroomPuff>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), NeedDis: false, isHypno, IsLow: false);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + creatBulletOffsetPos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), NeedDis: false, isHypno, IsLow: false);
			}
		}
	}

	protected override void GoSleepSpecial()
	{
		SetAnimChange(15);
	}

	protected override void GoAwakeSpecial()
	{
		SetAnimChange(16);
	}

	protected override void SetEyeTex(int EyeType)
	{
		if (EyeType == 100)
		{
			PlayAnim("anim_blink", 1);
		}
	}
}
