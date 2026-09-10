using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Star : BulletBase
{
	public Light2D light2;

	private Starfruit Starfruit;

	private bool isCheck;

	public SpriteRenderer Bullet;

	protected override bool canBounce => true;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirc, Starfruit starfruit, bool isCheck, bool isHyp)
	{
		isHypno = isHyp;
		Starfruit = starfruit;
		this.isCheck = isCheck;
		base.transform.position = pos;
		Dirction = dirc;
		CurrLine = currLine;
		if (base.transform.localScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		MoveSpeed = 6f;
		base.attackValue = attackValue;
		isHit = false;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		BaseInit();
		if (Shadow != null)
		{
			Shadow.transform.localPosition = new Vector3(0f, -0.2f);
		}
		light2.enabled = !isCheck;
		Bullet.enabled = !isCheck;
		Shadow.enabled = !isCheck;
	}

	protected override void UpdateThis()
	{
		if (isHit)
		{
			return;
		}
		Bullet.transform.Rotate(new Vector3(0f, 0f, -200f * Time.deltaTime));
		if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			DestoryBullet();
		}
		if (base.CurrGrid == null || !(base.CurrGrid.CurrPlantBase != null) || ((base.CurrGrid.CurrPlantBase.isHypno || !isHypno) && (!base.CurrGrid.CurrPlantBase.isHypno || isHypno)) || !(Vector2.Distance(base.transform.position, base.CurrGrid.CurrPlantBase.transform.position) < 0.46f))
		{
			return;
		}
		if (Starfruit != null)
		{
			Starfruit.CheckZombieResult();
		}
		if (!isCheck)
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
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if ((componentInParent.CurrLine == CurrLine || CurrLine == -1) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				if (Starfruit != null)
				{
					Starfruit.CheckZombieResult();
				}
				if (!isCheck)
				{
					componentInParent.Hurt(base.FinalDamage, Dirction);
					HitEff();
				}
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			if (!isCheck)
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
			}
			HitEff();
		}
		if (!(collision.tag == "Obstacle") || isCheck)
		{
			return;
		}
		Obstacle component = collision.transform.GetComponent<Obstacle>();
		if (component.ContainLine(CurrLine) || CurrLine == -1)
		{
			component.HurtThis(base.FinalDamage);
			if (component is SteelWheel)
			{
				component.GetComponent<SteelWheel>().PushForce(0.6f, base.transform.position);
			}
			AudioManager.Instance.RandomPlayEFAudio(new List<AudioClip>
			{
				GameManager.Instance.AudioConf.splat1,
				GameManager.Instance.AudioConf.splat2,
				GameManager.Instance.AudioConf.splat3
			}, base.transform.position);
			HitEff();
		}
	}

	private void HitEff()
	{
		isHit = true;
		if (!isCheck)
		{
			GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.StarParticle);
			obj.transform.position = base.transform.position + new Vector3(Dirction.normalized.x * Random.Range(0.1f, 0.2f), Dirction.normalized.y * Random.Range(0.1f, 0.2f));
			obj.transform.GetComponent<SortingGroup>().sortingOrder = Bullet.GetComponent<SpriteRenderer>().sortingOrder;
		}
		DestoryBullet();
	}

	protected override void DestoryBullet()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Star, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
	}

	protected override void HitEvent()
	{
		HitEff();
	}
}
