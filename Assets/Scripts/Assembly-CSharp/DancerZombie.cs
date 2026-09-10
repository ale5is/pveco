using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class DancerZombie : ZombieBase
{
	private Transform anim;

	private JacksonZombie theKing;

	private bool KingEating;

	private int walkNum;

	private int armRaiseNum;

	public List<SpriteRenderer> Seaweeds = new List<SpriteRenderer>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.DancerZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 270;

	protected override int CriticalHp => 70;

	public override void InitZombieHpState()
	{
		if (theKing != null && theKing.Hp <= 0)
		{
			theKing = null;
		}
		KingEating = false;
		anim = animator.transform;
		ToTurn(isLeft: true);
	}

	public void KingInit(JacksonZombie jackson)
	{
		theKing = jackson;
	}

	private void ToTurn()
	{
		anim.localScale = new Vector3(0f - anim.localScale.x, anim.localScale.y);
		anim.localPosition = new Vector3(0f - anim.localPosition.x, anim.localPosition.y);
	}

	private void ToTurn(bool isLeft)
	{
		float num = Mathf.Abs(anim.localScale.x);
		float num2 = Mathf.Abs(anim.localPosition.x);
		if (isLeft)
		{
			num2 = 0f - num2;
		}
		else
		{
			num = 0f - num;
		}
		anim.localScale = new Vector3(num, anim.localScale.y);
		anim.localPosition = new Vector3(num2, anim.localPosition.y);
	}

	protected override void CheckState()
	{
		base.CheckState();
		if (!(theKing == null))
		{
			switch (base.State)
			{
			case ZombieState.Walk:
				theKing.DancerEat(isEat: false, this);
				break;
			case ZombieState.Attack:
				ToTurn(isLeft: true);
				theKing.DancerEat(isEat: true, this);
				break;
			}
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public void LeaveKing()
	{
		KingEat(isEat: false);
		theKing = null;
	}

	private void DismissKing()
	{
		if (theKing != null)
		{
			theKing.DancerDead(this);
		}
		KingEat(isEat: false);
		theKing = null;
	}

	public override void ZombieOnDead(bool dropItem)
	{
		DismissKing();
		for (int i = 0; i < Seaweeds.Count; i++)
		{
			Seaweeds[i].enabled = false;
		}
	}

	protected override void HypnoEvent()
	{
		if (!(theKing == null) && theKing.isHypno != isHypno)
		{
			DismissKing();
		}
	}

	public void KingDanceSyn(bool isArmraise)
	{
		if (base.State == ZombieState.Walk)
		{
			if (isArmraise)
			{
				KingEating = true;
				ToArmRaise();
			}
			else
			{
				KingEating = false;
				ToWalk(isPassive: true);
			}
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			KingEating = syn.SynCode[2] == 0;
			anCanMove = !KingEating;
		}
		else if (syn.SynCode[1] == 1)
		{
			ToWalk(isPassive: true);
		}
		else if (syn.SynCode[1] == 2)
		{
			ToArmRaise();
		}
	}

	public void KingEat(bool isEat)
	{
		KingEating = isEat;
		anCanMove = !isEat;
		if (base.State != ZombieState.Attack)
		{
			ServerSendSyn(0, (!isEat) ? 1 : 0);
		}
	}

	private void ToWalk(bool isPassive)
	{
		armRaiseNum = 0;
		if (!isPassive)
		{
			ToTurn(isLeft: true);
		}
		base.State = ZombieState.Walk;
		ServerSendSyn(1);
	}

	private void ToArmRaise()
	{
		anCanMove = false;
		SetAnimatorChange(41);
		ServerSendSyn(2);
	}

	public override void SpecialAnimEvent1()
	{
		ToTurn();
		if (!GameManager.Instance.isClient && !(theKing != null))
		{
			armRaiseNum++;
			anCanMove = false;
			if (armRaiseNum >= 4)
			{
				ToWalk(isPassive: false);
			}
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (!GameManager.Instance.isClient && !(theKing != null))
		{
			walkNum++;
			ToTurn(isLeft: true);
			if (walkNum >= 6)
			{
				walkNum = 0;
				ToArmRaise();
			}
		}
	}

	public override void SpecialAnimEvent3()
	{
		if (!KingEating)
		{
			anCanMove = true;
		}
	}

	public override void SpecialAnimEvent4()
	{
		ToTurn(isLeft: true);
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

	public override void OutInWaterEvent()
	{
		int num = Random.Range(0, Seaweeds.Count);
		for (int i = 0; i < Seaweeds.Count; i++)
		{
			if (i != num)
			{
				Seaweeds[i].enabled = true;
			}
			Seaweeds[i].sprite = NormalSprite.Instance.SeaweedSprites[Random.Range(0, NormalSprite.Instance.SeaweedSprites.Count)];
		}
	}
}
