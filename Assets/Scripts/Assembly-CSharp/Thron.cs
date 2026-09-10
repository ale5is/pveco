using System.Collections.Generic;
using UnityEngine;

public class Thron : BulletBase
{
	private BalloonZombie TargetZombie;

	private int HitNum;

	private List<Grid> hitOverGrids = new List<Grid>();

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, int sortOrder, bool isHyp, ZombieBase zombie = null)
	{
		SetSortingOrder(sortOrder);
		isHypno = isHyp;
		base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		if (dirct.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		HitNum = 0;
		hitOverGrids.Clear();
		TargetZombie = null;
		if (zombie != null)
		{
			TargetZombie = zombie.GetComponent<BalloonZombie>();
		}
		base.transform.position = pos;
		Dirction = dirct;
		CurrLine = currLine;
		MoveSpeed = 6f;
		base.attackValue = attackValue;
		isHit = false;
		BaseInit();
	}

	protected override void UpdateThis()
	{
		if (isHit)
		{
			return;
		}
		if (base.transform.position.x > 15f)
		{
			DestoryBullet();
		}
		else if (base.transform.position.x < -15f)
		{
			DestoryBullet();
		}
		else if (TargetZombie != null && TargetZombie.CurrLine == CurrLine && TargetZombie.IsFly() && TargetZombie.Hp > 0 && Mathf.Abs(TargetZombie.transform.position.x - base.transform.position.x) < 0.1f)
		{
			TargetZombie.Hurt(base.FinalDamage, Vector2.zero);
			DestoryBullet();
		}
		else if (!hitOverGrids.Contains(base.CurrGrid) && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((!base.CurrGrid.CurrPlantBase.isHypno && isHypno) || (base.CurrGrid.CurrPlantBase.isHypno && !isHypno)) && (!base.CurrGrid.CurrPlantBase.IsLowPlant || (base.CurrGrid.CurrPlantBase.IsLowPlant && (isLow || base.CurrGrid.CurrPlantBase.CarryPlant != null || base.CurrGrid.CurrPlantBase.ProtectPlant != null))) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.2f)
		{
			hitOverGrids.Add(base.CurrGrid);
			base.CurrGrid.CurrPlantBase.Hurt(base.FinalDamage, Dirction, null);
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
			HitNum += 3;
			if (HitNum > 4)
			{
				DestoryBullet();
			}
		}
	}

	protected override void TriggerEnter(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				componentInParent.Hurt(base.FinalDamage, Vector2.zero);
				HitNum++;
				if (HitNum > 4)
				{
					DestoryBullet();
				}
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			isHit = true;
			DestoryBullet();
		}
	}

	protected override void DestoryBullet()
	{
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Thron, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		GetComponent<SpriteRenderer>().sortingOrder = sorting;
	}

	protected override void HitEvent()
	{
	}

	protected override void CurrGridChange()
	{
		hitOverGrids.Clear();
	}
}
