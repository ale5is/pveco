using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pea : BulletBase
{
	protected override bool canBounce => true;

	protected override bool canUpBounce => true;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, int sortOrder, bool isHyp)
	{
		SetSortingOrder(sortOrder);
		base.transform.position = pos;
		isHypno = isHyp;
		Dirction = dirct;
		CurrLine = currLine;
		MoveSpeed = 6f;
		base.attackValue = attackValue;
		isHit = false;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
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
		else if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((!base.CurrGrid.CurrPlantBase.isHypno && isHypno) || (base.CurrGrid.CurrPlantBase.isHypno && !isHypno)) && (!base.CurrGrid.CurrPlantBase.IsLowPlant || (base.CurrGrid.CurrPlantBase.IsLowPlant && (isLow || (base.CurrGrid.CurrPlantBase.CarryPlant != null && !base.CurrGrid.CurrPlantBase.CarryPlant.IsLowPlant) || base.CurrGrid.CurrPlantBase.ProtectPlant != null))) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.2f && Mathf.Abs(base.transform.position.y - base.CurrGrid.Position.y) < 1.2f)
		{
			base.CurrGrid.CurrPlantBase.Hurt(base.FinalDamage, Dirction, null);
			HitEff();
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
	}

	protected override void TriggerEnter(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Zombie")
		{
			ZombieBase component = collision.GetComponent<ZombieBase>();
			if (component.ContainLine(CurrLine) && ((component.isHypno && isHypno) || (!component.isHypno && !isHypno)))
			{
				component.Hurt(base.FinalDamage, Dirction);
				HitEff();
			}
		}
		if (collision.tag == "Torchwood" && !isPultTrack)
		{
			Torchwood component2 = collision.GetComponent<Torchwood>();
			if (component2 != null)
			{
				if (component2.lineNum == CurrLine)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
					FirePea component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FirePea).GetComponent<FirePea>();
					component3.transform.SetParent(null);
					component3.Init(base.transform.position, attackValue, CurrLine, Dirction, GetComponent<SpriteRenderer>().sortingOrder, isHypno);
					DestoryBullet();
				}
			}
			else
			{
				MeltTorch component4 = collision.GetComponent<MeltTorch>();
				if (component4.lineNum == CurrLine)
				{
					MeltPea component5 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.MeltPea).GetComponent<MeltPea>();
					component5.transform.SetParent(null);
					component5.Init(component4.GetMeltValue(), base.transform.position, attackValue, CurrLine, Dirction, GetComponent<SpriteRenderer>().sortingOrder, isHypno);
					DestoryBullet();
				}
			}
		}
		if (collision.tag == "ZombieTorch" && !isPultTrack)
		{
			ZombieBase zombie = collision.GetComponent<PlantZombieAnimEvent>().zombie;
			if ((zombie.CurrLine == CurrLine && isHypno && !zombie.isHypno) || (!isHypno && zombie.isHypno))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
				FirePea component6 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FirePea).GetComponent<FirePea>();
				component6.transform.SetParent(null);
				component6.Init(base.transform.position, base.FinalDamage, CurrLine, Dirction, GetComponent<SpriteRenderer>().sortingOrder, isHypno);
				DestoryBullet();
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
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
			HitEff();
		}
		if (!(collision.tag == "Obstacle"))
		{
			return;
		}
		Obstacle component7 = collision.transform.GetComponent<Obstacle>();
		if (component7.ContainLine(CurrLine))
		{
			component7.HurtThis(attackValue);
			if (component7 is SteelWheel)
			{
				component7.GetComponent<SteelWheel>().PushForce(0.6f, base.transform.position);
			}
			AudioManager.Instance.RandomPlayEFAudio(new List<AudioClip>
			{
				GameManager.Instance.AudioConf.splat1,
				GameManager.Instance.AudioConf.splat2,
				GameManager.Instance.AudioConf.splat3
			}, base.transform.position);
			HitEff(offset: false);
		}
	}

	protected override void HitEvent()
	{
		HitEff();
	}

	private void HitEff(bool offset = true)
	{
		isHit = true;
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PeaParticle);
		if (offset)
		{
			obj.transform.position = base.transform.position + new Vector3(Dirction.normalized.x * Random.Range(0.1f, 0.2f), Dirction.normalized.y * Random.Range(0.1f, 0.2f));
		}
		else
		{
			obj.transform.position = base.transform.position;
		}
		obj.transform.GetComponent<SortingGroup>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
		DestoryBullet();
	}

	public void StartVerticalMove(float distance)
	{
		StartCoroutine(VerticalMove(distance));
	}

	private IEnumerator VerticalMove(float distance)
	{
		float goal = base.transform.position.y + distance;
		if (distance >= 0f)
		{
			while (base.transform.position.y < goal)
			{
				yield return null;
				base.transform.position += new Vector3(0f, 5f * Time.deltaTime, 0f);
			}
		}
		else
		{
			while (base.transform.position.y > goal)
			{
				yield return null;
				base.transform.position += new Vector3(0f, -5f * Time.deltaTime, 0f);
			}
		}
	}

	protected override void DestoryBullet()
	{
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Pea, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		GetComponent<SpriteRenderer>().sortingOrder = sorting;
	}
}
