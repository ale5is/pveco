using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BigGreenStar : BulletBase
{
	public Light2D light2;

	private float Scale;

	private float StartX;

	private int HitNum;

	private bool CanPass;

	private List<Grid> hitOverGrids = new List<Grid>();

	private List<ZombieBase> hitOverZombies = new List<ZombieBase>();

	public Transform Bullet;

	protected override bool canBlow => false;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirc, bool isHyp)
	{
		HitNum = 0;
		MoveSpeed = 0f;
		hitOverGrids.Clear();
		hitOverZombies.Clear();
		isHypno = isHyp;
		base.transform.position = pos;
		StartX = pos.x;
		Dirction = dirc;
		CurrLine = currLine;
		GetComponent<Collider2D>().enabled = false;
		if (base.transform.localScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		base.attackValue = attackValue;
		isHit = false;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		CanPass = true;
		Scale = 0f;
		base.transform.localScale = Vector3.zero;
		BaseInit();
	}

	public void GoMove()
	{
		MoveSpeed = 4f;
		GetComponent<Collider2D>().enabled = true;
	}

	protected override void UpdateThis()
	{
		Bullet.Rotate(new Vector3(0f, 0f, -200f * Time.deltaTime));
		if (Scale < 3f)
		{
			Scale += Time.deltaTime * 3f;
			base.transform.localScale = new Vector3(Scale, Scale);
		}
		else
		{
			if (isHit)
			{
				return;
			}
			if (CanPass && Mathf.Abs(base.transform.position.x - StartX) > 4.9f)
			{
				HitNum = 0;
				CanPass = false;
			}
			if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
			{
				DestoryBullet();
			}
			if (!hitOverGrids.Contains(base.CurrGrid) && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((!base.CurrGrid.CurrPlantBase.isHypno && isHypno) || (base.CurrGrid.CurrPlantBase.isHypno && !isHypno)) && Vector2.Distance(base.transform.position, base.CurrGrid.CurrPlantBase.transform.position) < 0.46f)
			{
				hitOverGrids.Add(base.CurrGrid);
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, Dirction, null);
				base.CurrGrid.CurrPlantBase.Dizzy(4);
				HitEff();
				if (UnityEngine.Random.Range(0, 3) == 0)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
				}
				else if (UnityEngine.Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
				}
				HitNum += 3;
				if (HitNum > 4 && !CanPass)
				{
					DestoryBullet();
				}
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
			if ((componentInParent.CurrLine == CurrLine || CurrLine == -1) && !hitOverZombies.Contains(componentInParent) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				hitOverZombies.Add(componentInParent);
				componentInParent.Hurt(attackValue, Dirction);
				componentInParent.Dizzy(4);
				HitNum++;
				HitEff();
				if (HitNum > 4 && !CanPass)
				{
					DestoryBullet();
				}
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			if (UnityEngine.Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (UnityEngine.Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
			HitEff();
			DestoryBullet();
		}
	}

	private void HitEff()
	{
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.StarParticle);
		obj.transform.position = base.transform.position + new Vector3(Dirction.normalized.x * UnityEngine.Random.Range(0.1f, 0.2f), Dirction.normalized.y * UnityEngine.Random.Range(0.1f, 0.2f));
		obj.transform.GetComponent<SortingGroup>().sortingOrder = Bullet.GetComponent<SpriteRenderer>().sortingOrder;
		if (!CanPass)
		{
			base.transform.localScale = new Vector3(3f - (float)HitNum * 0.3f, 3f - (float)HitNum * 0.3f);
		}
	}

	protected override void DestoryBullet()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.BigGreenStar, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
	}

	protected override void HitEvent()
	{
		throw new NotImplementedException();
	}
}
