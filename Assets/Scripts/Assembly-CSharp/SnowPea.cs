using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SnowPea : BulletBase
{
	private int FrozenLvl;

	private int AoeLvl;

	public SortingGroup SnowSort;

	protected override bool canBounce => true;

	protected override bool canUpBounce => true;

	public void Init(int attackValue, Vector2 pos, int line, Vector2 dirction, int sortOrder, bool isHyp, int frozenLvl = 1, int aoeLvl = 0)
	{
		SetSortingOrder(sortOrder);
		FrozenLvl = frozenLvl;
		AoeLvl = aoeLvl;
		isHit = false;
		isHypno = isHyp;
		CurrLine = line;
		Dirction = dirction;
		base.transform.position = pos;
		MoveSpeed = 6f;
		MapBase nearestMap = MapManager.Instance.GetNearestMap(pos);
		float num = 1f;
		float currTempt = nearestMap.CurrTempt;
		if (currTempt < 0f)
		{
			num += Mathf.Abs(currTempt) / 50f;
		}
		base.attackValue = (int)((float)attackValue * num);
		base.transform.SetParent(nearestMap.transform);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FrozenPea, base.transform.position);
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
			base.CurrGrid.CurrPlantBase.Frozen(Dirction, isAudio: true, FrozenLvl);
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
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent == null)
			{
				return;
			}
			if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				collision.GetComponentInParent<ZombieBase>().Frozen(Dirction, isAudio: true, FrozenLvl);
				collision.GetComponentInParent<ZombieBase>().Hurt(base.FinalDamage, Dirction);
				if (AoeLvl > 0)
				{
					List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 0.5f, isHypno, needCapsule: true);
					for (int i = 0; i < zombies.Count; i++)
					{
						zombies[i].Frozen(Dirction, isAudio: true, AoeLvl);
					}
				}
				HitEff();
			}
		}
		if (collision.tag == "Torchwood" && !isPultTrack)
		{
			Invoke("melt", 0.2f);
		}
		if (collision.tag == "ZombieTorch" && !isPultTrack)
		{
			ZombieBase zombie = collision.GetComponent<PlantZombieAnimEvent>().zombie;
			if ((zombie.CurrLine == CurrLine && isHypno && !zombie.isHypno) || (!isHypno && zombie.isHypno))
			{
				Invoke("melt", 0.2f);
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
	}

	private void HitEff()
	{
		isHit = true;
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnowpeaParticle);
		obj.transform.position = base.transform.position + new Vector3(Dirction.normalized.x * Random.Range(0.1f, 0.2f), Dirction.normalized.y * Random.Range(0.1f, 0.2f));
		obj.transform.GetComponent<SortingGroup>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
		DestoryBullet();
	}

	private void melt()
	{
		Pea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
		component.transform.SetParent(null);
		component.Init(attackValue, base.transform.position, CurrLine, Dirction, GetComponent<SpriteRenderer>().sortingOrder, isHypno);
		DestoryBullet();
	}

	protected override void DestoryBullet()
	{
		CancelInvoke();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.SnowPea, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		SnowSort.sortingOrder = sorting;
		GetComponent<SpriteRenderer>().sortingOrder = sorting;
		base.transform.Find("SnowFlakeParticle").GetComponent<SortingGroup>().sortingOrder = sorting;
	}

	protected override void HitEvent()
	{
		HitEff();
	}

	protected override void SetPultRotation(Quaternion quaternion)
	{
	}
}
