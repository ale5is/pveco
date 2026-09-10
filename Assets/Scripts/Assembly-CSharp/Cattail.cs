using System.Collections.Generic;
using UnityEngine;

public class Cattail : PlantBase
{
	private int HitNum;

	private Vector3 creatBulletOffsetPos = new Vector2(0f, 0.86f);

	public override float MaxHp => 300f;

	public override PlantType BasePlant => PlantType.Lilypad;

	public override int BasePlantSunNum => 100;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => true;

	public override bool CanPlaceOnPuddle => true;

	protected override int attackValue => 20;

	protected override void OnInitForPlace()
	{
		StartActionCD(1.6f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreateBullet();
		if (HitNum == 0)
		{
			HitNum++;
			SetAnimChange(0);
		}
		else
		{
			HitNum = 0;
			SetAnimChange(12);
		}
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		bool flag = false;
		for (int i = 0; i < allZombies.Count; i++)
		{
			if (allZombies[i].collider2d.enabled || allZombies[i] is BalloonZombie)
			{
				flag = true;
				break;
			}
		}
		List<PlantBase> list = null;
		if (!flag)
		{
			list = MapManager.Instance.GetAllPlant(base.transform.position, !isHypno);
		}
		if (flag || list.Count > 0)
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreateBullet()
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
			TrackBullet component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CattailBullet).GetComponent<TrackBullet>();
			component.transform.SetParent(null);
			component.Init(base.transform.position + creatBulletOffsetPos, GetAttackValue(), isHypno);
		}
	}
}
