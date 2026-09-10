using SocketSave;
using UnityEngine;

public class Ghost : ZombieBase
{
	private bool isLight;

	private float OwnerSpeed;

	private int OwnerHp;

	public override int MaxHP => OwnerHp;

	protected override GameObject Prefab => GameManager.Instance.GameConf.GhostZombie;

	protected override float AnToSpeed => OwnerSpeed;

	protected override float DefSpeed => OwnerSpeed;

	protected override float attackValue => 5f;

	private void FixedUpdate()
	{
		if (base.State != ZombieState.Walk || base.CurrGrid == null || !(Vector2.Distance(base.transform.position, base.CurrGrid.Position) < 0.8f))
		{
			return;
		}
		if (!isLight)
		{
			if (base.CurrGrid.LightNum > 0)
			{
				isLight = true;
				base.collider2d.enabled = true;
				SetAlpha(1f);
			}
		}
		else if (base.CurrGrid.LightNum <= 0)
		{
			isLight = false;
			base.collider2d.enabled = false;
			SetAlpha(0.6f);
		}
	}

	public override void InitZombieHpState()
	{
		needInWater = false;
		needChangeLine = false;
		base.collider2d.enabled = false;
		canButter = false;
		canFrozen = false;
		base.dontChangeState = true;
		isLight = false;
		SetAlpha(0.6f);
		Shadow.enabled = false;
		animator.speed = 1f;
	}

	protected override void NeedSynInit()
	{
		OwnerSpeed = Random.Range(0.8f, 2.5f);
		OwnerHp = (int)(10f + OwnerSpeed * 20f);
		if (OwnerSpeed > 1.5f)
		{
			OwnerHp += 20;
		}
		if (OwnerSpeed > 2f)
		{
			OwnerHp += 40;
		}
	}

	public override void ServerInitInfo()
	{
		ServerSendSyn(0, new Vector2(OwnerSpeed, 0f));
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			OwnerSpeed = syn.Twofloat.x;
			OwnerHp = (int)(10f + OwnerSpeed * 20f);
			if (OwnerSpeed > 1.5f)
			{
				OwnerHp += 40;
			}
			if (OwnerSpeed > 2f)
			{
				OwnerHp += 40;
			}
			base.Speed = DefSpeed;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp <= 0 && base.State != ZombieState.Dead)
		{
			base.State = ZombieState.Dead;
		}
	}

	public override void SpecialAnimEvent1()
	{
		SetAlpha(0f);
	}
}
