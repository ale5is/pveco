using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeltPea : BulletBase
{
	public SpriteRenderer Pea;

	public Animator fireAnimator;

	public Animator animator;

	private float StartX;

	private float Gravity;

	private int meltValue;

	protected override bool canBounce => true;

	protected override bool canUpBounce => true;

	public void Init(int meltValue, Vector2 pos, int attackvalue, int line, Vector2 dirct, int sortOrder, bool ishyp)
	{
		SetSortingOrder(sortOrder);
		Pea.gameObject.SetActive(value: true);
		fireAnimator.gameObject.SetActive(value: false);
		Gravity = 0f;
		StartX = pos.x;
		isHit = false;
		isHypno = ishyp;
		CurrLine = line;
		Dirction = dirct;
		this.meltValue = meltValue - 4;
		animator.enabled = false;
		base.transform.position = pos;
		attackValue = attackvalue * 3;
		MoveSpeed = 6f;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		BaseInit();
		Shadow.gameObject.SetActive(value: true);
		if (this.meltValue <= 0)
		{
			StartCoroutine(Fire());
		}
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
		if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((!base.CurrGrid.CurrPlantBase.isHypno && isHypno) || (base.CurrGrid.CurrPlantBase.isHypno && !isHypno)) && (!base.CurrGrid.CurrPlantBase.IsLowPlant || (base.CurrGrid.CurrPlantBase.IsLowPlant && (isLow || (base.CurrGrid.CurrPlantBase.CarryPlant != null && !base.CurrGrid.CurrPlantBase.CarryPlant.IsLowPlant) || base.CurrGrid.CurrPlantBase.ProtectPlant != null))) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.2f && Mathf.Abs(base.transform.position.y - base.CurrGrid.Position.y) < 1.2f)
		{
			isHit = true;
			base.CurrGrid.CurrPlantBase.UnFrozen(1);
			base.CurrGrid.CurrPlantBase.Hurt(base.FinalDamage, Dirction, null);
			StartCoroutine(Fire());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
		}
		Pea.transform.Rotate(new Vector3(0f, 0f, -100f * Time.deltaTime));
		if (Mathf.Abs(StartX - base.transform.position.x) > 6.4f - (float)meltValue * 0.5f)
		{
			Gravity += Time.deltaTime;
		}
		base.transform.position += new Vector3(0f, -0.1f * Gravity);
		if (Gravity > 0f && base.transform.position.y < base.CurrGrid.Position.y - 0.5f)
		{
			SpawnMelt();
			StartCoroutine(Fire());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
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
			if (componentInParent.ContainLine(CurrLine) && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				isHit = true;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
				float currTempt = MapManager.Instance.GetNearestMap(base.transform.position).CurrTempt;
				float num = 1f + (currTempt - 30f) / 40f;
				if (num < 1f)
				{
					num = 1f;
				}
				if (num > 1.5f)
				{
					num = 1.5f;
				}
				componentInParent.Hurt((int)((float)base.FinalDamage * num), Dirction);
				SpawnMelt();
				StartCoroutine(Fire());
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			isHit = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
			StartCoroutine(Fire());
		}
		if (!(collision.tag == "Obstacle"))
		{
			return;
		}
		Obstacle component = collision.transform.GetComponent<Obstacle>();
		if (component.ContainLine(CurrLine))
		{
			component.HurtThis(attackValue);
			if (component is SteelWheel)
			{
				component.GetComponent<SteelWheel>().PushForce(0.8f, base.transform.position);
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
		HitEff();
		MoveSpeed = 0f;
		animator.enabled = true;
		animator.Play("FirePeaDead");
		Pea.gameObject.SetActive(value: false);
		Shadow.gameObject.SetActive(value: false);
		fireAnimator.gameObject.SetActive(value: true);
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		yield return new WaitForSeconds(1f);
		DestoryBullet();
	}

	private void HitEff()
	{
		isHit = true;
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.MeltPeaParticle);
		obj.transform.position = base.transform.position;
		obj.transform.GetComponent<SortingGroup>().sortingOrder = Pea.sortingOrder;
	}

	private void SpawnMelt()
	{
		if (!base.CurrGrid.isWaterGrid && Random.Range(0, 10) > 8)
		{
			LvItemManager.Instance.SpawnMelt(CurrLine, base.transform.position);
		}
	}

	protected override void DestoryBullet()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.MeltPea, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		Pea.sortingOrder = sorting;
		fireAnimator.GetComponent<SortingGroup>().sortingOrder = sorting;
	}

	protected override void HitEvent()
	{
		StartCoroutine(Fire());
	}
}
