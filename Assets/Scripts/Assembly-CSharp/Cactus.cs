using System.Collections.Generic;
using UnityEngine;

public class Cactus : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	private ZombieBase TargetZombie;

	private bool isHigh;

	public override float MaxHp => 300f;

	protected override int attackValue => 20;

	protected override void OnInitForPlace()
	{
		isHigh = false;
		StartActionCD(1.4f, isOver: true);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		FindMinDisBalloon();
		if (TargetZombie == null)
		{
			if (isHigh)
			{
				isHigh = false;
				SetAnimChange(14);
			}
			else
			{
				ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
				PlantBase plantBase = null;
				if (zombieByLineMinDistance == null)
				{
					plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
				}
				if (zombieByLineMinDistance != null || plantBase != null)
				{
					SetAnimChange(11);
					return true;
				}
			}
		}
		else
		{
			if (isHigh)
			{
				SetAnimChange(11);
				return true;
			}
			isHigh = true;
			SetAnimChange(13);
		}
		return false;
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreateThron(creatBulletOffsetPos);
		SetAnimChange(12);
	}

	public override void SpecialAnimEvent2()
	{
		CreateThron(new Vector3(1.15f, 1.3f));
		SetAnimChange(12);
	}

	private void FindMinDisBalloon()
	{
		TargetZombie = null;
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno, needCapsule: false);
		if (zombiesByLine.Count <= 0)
		{
			return;
		}
		List<ZombieBase> list = new List<ZombieBase>();
		for (int i = 0; i < zombiesByLine.Count; i++)
		{
			if (zombiesByLine[i] is BalloonZombie && zombiesByLine[i].GetComponent<BalloonZombie>().IsFly())
			{
				list.Add(zombiesByLine[i]);
			}
		}
		float num = 999999f;
		for (int j = 0; j < list.Count; j++)
		{
			if (Vector2.Distance(base.transform.position, list[j].transform.position) < num && list[j].Hp > 0)
			{
				num = Vector2.Distance(base.transform.position, list[j].transform.position);
				TargetZombie = list[j];
			}
		}
	}

	private void CreateThron(Vector3 pos)
	{
		if (base.currGrid != null)
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			Thron component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Thron).GetComponent<Thron>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(pos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno, TargetZombie);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + pos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno, TargetZombie);
			}
		}
	}
}
