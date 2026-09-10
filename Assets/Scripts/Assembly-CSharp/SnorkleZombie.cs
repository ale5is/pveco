using System.Collections.Generic;
using UnityEngine;

public class SnorkleZombie : ZombieBase
{
	public SnorkleType Type;

	public Sprite helmet1;

	public Sprite helmet2;

	public Sprite helmet3;

	public SpriteMask JumpMask;

	public SpriteRenderer EatWhiteWater;

	private float AnTospeed;

	private float defSpeed;

	private bool needDropHat;

	protected override GameObject Prefab => GameManager.Instance.GameConf.SnorkleZombie;

	protected override float AnToSpeed => AnTospeed;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 80f;

	public override int MaxHP => GetTypeHp();

	protected override int CriticalHp => 70;

	protected override float inWaterDepth => 0.9f;

	public override bool CanNoCollGet => true;

	private int GetTypeHp()
	{
		int result = 270;
		if (Type == SnorkleType.Helmet)
		{
			result = 1670;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		defSpeed = 2f;
		AnTospeed = 3f;
		JumpMask.enabled = false;
		JumpMask.frontSortingOrder = Sorting.sortingOrder;
		JumpMask.backSortingOrder = Sorting.sortingOrder - 1;
		if (Type == SnorkleType.Helmet)
		{
			HammerHpState = new List<int> { 940, 270 };
			HpState = new List<int> { 1670, 1200, 740, 270 };
			E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
		}
	}

	protected override void UpdateThis()
	{
		if (!base.InWater && base.State == ZombieState.Walk && (lastGrid == null || !lastGrid.isNoIceWater))
		{
			float num = Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x);
			if (base.CurrGrid != null && base.CurrGrid.isNoIceWater && num < 0.9f && num > 0.8f)
			{
				base.State = ZombieState.Attack;
				base.dontChangeState = true;
				SetAnimatorChange(41);
				base.collider2d.enabled = false;
			}
		}
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (base.InWater)
			{
				CriticalHpEnable = false;
				anCanMove = false;
				SetAnimatorChange(12);
			}
			else
			{
				SetAnimatorChange(11);
			}
			break;
		case ZombieState.Attack:
			if (base.InWater)
			{
				CriticalHpEnable = true;
			}
			break;
		case ZombieState.Dead:
			if (base.InWater)
			{
				SetAnimatorChange(32);
			}
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 180f * base.HpScale && (!base.InWater || base.State != ZombieState.Walk) && base.State != ZombieState.Dead)
		{
			DropArm();
		}
		if (HitSound)
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
	}

	protected override void InWaterChangeEvent()
	{
		if (base.State != ZombieState.Dead)
		{
			if (base.InWater)
			{
				SetAnimatorChange(12);
				CriticalHpEnable = false;
				base.collider2d.enabled = false;
				EatWhiteWater.material.SetInt("_OpenDisplay", 1);
			}
			else
			{
				SetAnimatorChange(11);
				CriticalHpEnable = true;
				base.collider2d.enabled = true;
				EatWhiteWater.material.SetInt("_OpenDisplay", 0);
			}
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 270 && Type == SnorkleType.Helmet)
		{
			result = EquipRenderer;
			if (needClearEquip)
			{
				needDropHat = false;
				base.Hp = 270;
			}
		}
		return result;
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		if (nextSprite == null && needDropHat)
		{
			DropEquip(EquipRenderer);
		}
	}

	public override void SpecialAnimEvent1()
	{
		DirctInWater();
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
		JumpMask.enabled = false;
		if (base.IsFacingLeft)
		{
			base.transform.position += new Vector3(-0.48f, 0f);
		}
		else
		{
			base.transform.position -= new Vector3(-0.48f, 0f);
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(base.transform.position + new Vector3(0f, 0f), base.CurrGrid.Point.y);
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
		if (base.IsCriticalState)
		{
			SetAnimatorChange(32);
		}
	}

	public override void SpecialAnimEvent2()
	{
		base.collider2d.enabled = true;
	}

	public override void SpecialAnimEvent3()
	{
		base.collider2d.enabled = false;
	}

	public override void SpecialAnimEvent4()
	{
		Shadow.enabled = false;
		JumpMask.enabled = true;
	}
}
