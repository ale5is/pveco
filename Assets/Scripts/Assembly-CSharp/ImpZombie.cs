using System.Collections;
using UnityEngine;

public class ImpZombie : ZombieBase
{
	private Vector3 scale;

	protected override GameObject Prefab => GameManager.Instance.GameConf.ImpZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 50f;

	public override int MaxHP => 270;

	protected override int CriticalHp => 70;

	public override void InitZombieHpState()
	{
	}

	public void ThrowInit(Vector2 goal, bool haveDuckytube = false)
	{
		scale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
		canIce = false;
		base.dontChangeState = true;
		animator.Play("throw");
		base.collider2d.enabled = false;
		StartCoroutine(MoveToGround(goal));
	}

	private IEnumerator MoveToGround(Vector3 vector)
	{
		yield return new WaitForFixedUpdate();
		base.transform.localScale = scale;
		anCanMove = false;
		Vector3 vec = (vector - base.transform.position).normalized;
		while (base.transform.position.y > vector.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(vec * Time.deltaTime * 5f);
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

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombieImp).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override void SpecialAnimEvent1()
	{
		if (base.CurrGrid.isNoIceWater)
		{
			base.collider2d.enabled = true;
			base.dontChangeState = false;
			base.State = ZombieState.Walk;
			canIce = true;
		}
	}

	public override void SpecialAnimEvent2()
	{
		base.collider2d.enabled = true;
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
		SetAnimatorChange(41);
		canIce = true;
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (base.transform.localScale.x == 0f)
		{
			base.transform.localScale = scale;
		}
	}
}
