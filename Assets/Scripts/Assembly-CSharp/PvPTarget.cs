using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPTarget : ZombieBase
{
	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	private List<int> OnlineID = new List<int>();

	public override int MaxHP => 600;

	public override bool CanEatByChomper => false;

	protected override GameObject Prefab => GameManager.Instance.GameConf.PvPTarget;

	protected override float AnToSpeed => 1f;

	protected override float DefSpeed => 1f;

	protected override float attackValue => 0f;

	public override bool CanBlowBack => false;

	public override void InitZombieHpState()
	{
		anCanMove = false;
		canIce = false;
		canButter = false;
		canFrozen = false;
		canDizzy = false;
		onlyBoomHurt = true;
		LowArmRenderer.sprite = State1;
		StartCoroutine(GetHurt());
	}

	private IEnumerator GetHurt()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.3f);
			if (base.Hp <= 0)
			{
				continue;
			}
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.CurrLine, base.transform.position, !base.IsFacingLeft, !isHypno, needCapsule: false);
			if (zombieByLineMinDistance != null)
			{
				float num = Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x);
				if (num > 0.6f && num < 1.2f)
				{
					Hurt(200, Vector2.zero);
				}
			}
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (GameManager.Instance.isServer)
		{
			PvPSelector.Instance.TargetDead(PlacePlayer);
		}
	}

	protected override int HandleHurt(int attackValue, Vector2 dirction)
	{
		int result = attackValue;
		if (attackValue > 100)
		{
			result = 100;
		}
		return result;
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 400f * base.HpScale && (float)base.Hp > 200f * base.HpScale)
		{
			LowArmRenderer.sprite = State2;
		}
		else if ((float)base.Hp < 200f * base.HpScale)
		{
			LowArmRenderer.sprite = State3;
		}
		if (HitSound)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
			}
		}
	}
}
