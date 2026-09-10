using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : BulletBase
{
	public SpriteRenderer MidBall;

	private List<Grid> hitOverGrids = new List<Grid>();

	private List<ZombieBase> hitOverZombies = new List<ZombieBase>();

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, int sortOrder, bool isHyp)
	{
		SetSortingOrder(sortOrder);
		base.transform.position = pos;
		isHypno = isHyp;
		Dirction = dirct;
		CurrLine = currLine;
		MoveSpeed = 3f;
		base.attackValue = attackValue;
		isHit = false;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		hitOverGrids.Clear();
		hitOverZombies.Clear();
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
			return;
		}
		if (base.transform.position.x < -15f)
		{
			DestoryBullet();
			return;
		}
		if (!hitOverGrids.Contains(base.CurrGrid) && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((!base.CurrGrid.CurrPlantBase.isHypno && isHypno) || (base.CurrGrid.CurrPlantBase.isHypno && !isHypno)) && (!base.CurrGrid.CurrPlantBase.IsLowPlant || (base.CurrGrid.CurrPlantBase.IsLowPlant && (isLow || base.CurrGrid.CurrPlantBase.CarryPlant != null || base.CurrGrid.CurrPlantBase.ProtectPlant != null))) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.2f)
		{
			base.CurrGrid.CurrPlantBase.Hurt(attackValue, Dirction, null);
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
		}
		base.transform.Rotate(new Vector3(0f, 0f, -1.5f));
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
			if (componentInParent.CurrLine == CurrLine && !hitOverZombies.Contains(componentInParent) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				if (Random.Range(0, 2) == 0)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bilibili1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bilibili2, base.transform.position);
				}
				hitOverZombies.Add(componentInParent);
				componentInParent.BoomHurt(attackValue, HitSound: true);
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
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.EnergyBall, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		MidBall.sortingOrder = sorting + 1;
		GetComponent<SpriteRenderer>().sortingOrder = sorting;
	}

	protected override void HitEvent()
	{
	}
}
