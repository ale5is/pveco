using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FirePea : BulletBase
{
	public Animator peaAnimator;

	public Animator fireAnimator;

	public Animator animator;

	protected override bool canBounce => true;

	protected override bool canUpBounce => true;

	public void Init(Vector2 pos, int attackvalue, int line, Vector2 dirct, int sortOrder, bool ishyp)
	{
		SetSortingOrder(sortOrder);
		peaAnimator.gameObject.SetActive(value: true);
		fireAnimator.gameObject.SetActive(value: false);
		isHit = false;
		isHypno = ishyp;
		CurrLine = line;
		Dirction = dirct;
		animator.Play("FirePeaShark");
		base.transform.position = pos;
		attackValue = attackvalue;
		MoveSpeed = 6f;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		BaseInit();
		Shadow.gameObject.SetActive(value: true);
	}

	protected override void UpdateThis()
	{
		if (!isHit)
		{
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
				isHit = true;
				base.CurrGrid.CurrPlantBase.UnFrozen(1);
				base.CurrGrid.CurrPlantBase.Hurt(base.FinalDamage * 2, Dirction, null);
				StartCoroutine(Fire());
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
			}
		}
	}

	protected override void TriggerEnter(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Torchwood" && !isPultTrack)
		{
			MeltTorch component = collision.GetComponent<MeltTorch>();
			if (component != null && component.lineNum == CurrLine)
			{
				MeltPea component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.MeltPea).GetComponent<MeltPea>();
				component2.transform.SetParent(null);
				int num = component.GetMeltValue();
				if (num < 6)
				{
					num += 5;
				}
				component2.Init(num, base.transform.position, attackValue * 2, CurrLine, Dirction, peaAnimator.GetComponent<SortingGroup>().sortingOrder, isHypno);
				DestoryBullet();
			}
		}
		else if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent.ContainLine(CurrLine) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				isHit = true;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
				float currTempt = MapManager.Instance.GetNearestMap(base.transform.position).CurrTempt;
				float num2 = currTempt / 30f + 1f / 3f;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (num2 > 2f)
				{
					num2 = 2f;
				}
				List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 0.65f * num2, isHypno, needCapsule: true);
				float num3 = 1f + (currTempt - 30f) / 40f;
				if (num3 < 1f)
				{
					num3 = 1f;
				}
				if (num3 > 1.5f)
				{
					num3 = 1.5f;
				}
				componentInParent.Hurt((int)((float)(base.FinalDamage * 2) * num3), Dirction, isHard: false, HitSound: false);
				for (int i = 0; i < zombies.Count; i++)
				{
					zombies[i].BurnEvent();
					if (componentInParent != zombies[i])
					{
						zombies[i].Hurt((int)((float)(base.FinalDamage * 2 / 3) * num3), Dirction, isHard: false);
					}
				}
				StartCoroutine(Fire());
			}
		}
		else if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			isHit = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
			StartCoroutine(Fire());
		}
		if (!(collision.tag == "Obstacle"))
		{
			return;
		}
		Obstacle component3 = collision.transform.GetComponent<Obstacle>();
		if (component3.ContainLine(CurrLine))
		{
			component3.HurtThis(base.FinalDamage);
			if (component3 is SteelWheel)
			{
				component3.GetComponent<SteelWheel>().PushForce(0.6f, base.transform.position);
			}
			AudioManager.Instance.RandomPlayEFAudio(new List<AudioClip>
			{
				GameManager.Instance.AudioConf.splat1,
				GameManager.Instance.AudioConf.splat2,
				GameManager.Instance.AudioConf.splat3
			}, base.transform.position);
			StartCoroutine(Fire());
		}
	}

	private IEnumerator Fire()
	{
		animator.Play("FirePeaDead");
		Shadow.gameObject.SetActive(value: false);
		peaAnimator.gameObject.SetActive(value: false);
		fireAnimator.gameObject.SetActive(value: true);
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		yield return new WaitForSeconds(1f);
		DestoryBullet();
	}

	protected override void DestoryBullet()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.FirePea, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		fireAnimator.GetComponent<SortingGroup>().sortingOrder = sorting;
		peaAnimator.GetComponent<SortingGroup>().sortingOrder = sorting;
	}

	protected override void HitEvent()
	{
		StartCoroutine(Fire());
	}

	protected override void SetPultRotation(Quaternion quaternion)
	{
		peaAnimator.transform.rotation = quaternion;
	}
}
