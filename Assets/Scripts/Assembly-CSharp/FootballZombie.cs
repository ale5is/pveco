using System.Collections.Generic;
using UnityEngine;

public class FootballZombie : ZombieBase
{
	public FootballZombieType Type;

	public Sprite helmet1;

	public Sprite helmet2;

	public Sprite helmet3;

	private bool needDropHat;

	private GameObject prefab;

	protected override float DefSpeed => 2.5f;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => prefab;

	protected override float AnToSpeed => 5f;

	protected override int CriticalHp => 70;

	private int GetTypeHp()
	{
		int result = 270;
		switch (Type)
		{
		case FootballZombieType.Normal:
			result = 1670;
			break;
		case FootballZombieType.Black:
			result = 3070;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		switch (Type)
		{
		case FootballZombieType.Normal:
			prefab = GameManager.Instance.GameConf.Zombie_Football;
			HammerHpState = new List<int> { 940, 270 };
			HpState = new List<int> { 1670, 1200, 740, 270 };
			break;
		case FootballZombieType.Black:
			prefab = GameManager.Instance.GameConf.Zombie_BlackFootball;
			HammerHpState = new List<int> { 2200, 1670, 940, 270 };
			HpState = new List<int> { 3070, 1660, 1200, 270 };
			break;
		}
		E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
	}

	protected override void InWaterChangeEvent()
	{
		if (base.State != ZombieState.Dead)
		{
			if (base.InWater)
			{
				animator.SetInteger("Change", 12);
			}
			else
			{
				animator.SetInteger("Change", 11);
			}
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 180f * base.HpScale)
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

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (base.InWater)
			{
				animator.SetInteger("Change", 12);
			}
			else
			{
				animator.SetInteger("Change", 11);
			}
			break;
		case ZombieState.Dead:
			if (base.InWater)
			{
				animator.SetInteger("Change", 32);
			}
			else
			{
				animator.SetInteger("Change", 31);
			}
			break;
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 270 && Type == FootballZombieType.Normal)
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

	protected override void CreateInitZombie()
	{
		StartIdel();
	}
}
