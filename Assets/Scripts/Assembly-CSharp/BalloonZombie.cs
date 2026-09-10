using System.Collections;
using UnityEngine;

public class BalloonZombie : ZombieBase
{
	private Transform anim;

	private bool isFlying;

	private float ownerSpeed;

	private float ownerAnSpeed;

	protected override GameObject Prefab => GameManager.Instance.GameConf.BalloonZombie;

	protected override float AnToSpeed => ownerAnSpeed;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 350;

	protected override int CriticalHp => 70;

	public bool IsFly()
	{
		return isFlying;
	}

	public override void InitZombieHpState()
	{
		canIce = false;
		ownerAnSpeed = 2.2f;
		ownerSpeed = 2.2f;
		base.Speed = 2.2f;
		base.dontChangeState = true;
		isFlying = true;
		anim = animator.transform;
		anim.localPosition = new Vector3(-0.42f, 1f);
		base.collider2d.enabled = false;
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ballooninflate, base.transform.position);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (isFlying && base.Hp > 0)
		{
			anCanMove = false;
			ownerAnSpeed = 3.8f;
			ownerSpeed = 3.8f;
			base.Speed = 3.8f;
			isFlying = false;
			animator.Play("pop");
			StartCoroutine(MoveDown());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.balloon_pop, base.transform.position);
		}
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

	private IEnumerator MoveDown()
	{
		while (anim.localPosition.y > 0.84f)
		{
			yield return null;
			anim.Translate(new Vector2(0f, -1.33f) * Time.deltaTime);
		}
	}

	public void Blow()
	{
		StartCoroutine(BlowOut());
	}

	private IEnumerator BlowOut()
	{
		if (isFlying)
		{
			do
			{
				yield return null;
				base.transform.Translate(new Vector2(20f, 0f) * Time.deltaTime);
			}
			while (!(base.transform.position.x > 10f));
			DirectDead(canDropItem: false, 0f);
		}
	}

	public override void SpecialAnimEvent1()
	{
		canIce = true;
		base.collider2d.enabled = true;
		if (base.CurrGrid.isNoIceWater)
		{
			base.dontChangeState = false;
			base.State = ZombieState.Walk;
		}
		anCanMove = false;
	}

	public override void SpecialAnimEvent2()
	{
		anCanMove = true;
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
	}
}
