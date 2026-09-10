using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public abstract class PlantZombie : ZombieBase
{
	private float baseAnimSpeed = 1f;

	public Animator PlantHeadAnimator;

	public Sprite door1;

	public Sprite door2;

	public Sprite door3;

	public Sprite armor1;

	public Sprite armor2;

	public SpriteRenderer WhiteWaterDoor;

	public List<SpriteRenderer> LeftArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> RightArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> flagArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> doorArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> Seaweeds = new List<SpriteRenderer>();

	protected float RandomSpeed = 4f;

	private bool needDropDoor;

	private Coroutine ActionCdCoroutine;

	private float ActionCDTime;

	protected float CDTimeScale = 1f;

	protected bool CDTimeOver;

	protected override float DefSpeed => RandomSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override float AnToSpeed => 5f;

	protected override int CriticalHp => 70;

	protected virtual int OwnerHp { get; } = 270;

	protected virtual int attackValue2 { get; }

	protected virtual Vector2 SpeedRange { get; } = new Vector2(3.2f, 5.5f);

	protected override bool NeedDropHead => false;

	public float BaseAnimSpeed
	{
		get
		{
			return baseAnimSpeed;
		}
		set
		{
			baseAnimSpeed = value;
			if (IsStaticState())
			{
				CDTimeScale = 0f;
				PlantHeadAnimator.speed = 0f;
			}
			else
			{
				CDTimeScale = base.SpeedRate;
				PlantHeadAnimator.speed = base.SpeedRate * baseAnimSpeed;
			}
		}
	}

	protected override void AnimatorSpeedChange(float speed)
	{
		PlantHeadAnimator.speed = speed * baseAnimSpeed;
	}

	private int GetTypeHp()
	{
		return OwnerHp;
	}

	public override void InitZombieHpState()
	{
		needDropDoor = true;
		ArmorHpAbsorb = 20;
		PlantZombieInit();
		bool isDoorArm = DoorHpState.Count > 0;
		IsDoorArm(isDoorArm);
		PlantHeadAnimator.GetComponent<PlantZombieAnimEvent>().zombie = this;
	}

	protected abstract void PlantZombieInit();

	protected virtual bool DoAction()
	{
		return false;
	}

	protected void StartActionCD(float cd, bool isOver)
	{
		ActionCDTime = cd;
		CDTimeOver = isOver;
		if (ActionCdCoroutine != null)
		{
			StopCoroutine(ActionCdCoroutine);
		}
		ActionCdCoroutine = StartCoroutine(ActionCd());
	}

	protected IEnumerator ActionCd()
	{
		yield return new WaitForSeconds(0.5f);
		while (true)
		{
			if (CDTimeOver)
			{
				float num = ActionCDTime / 5f;
				if (num < 1f)
				{
					num = 1f;
				}
				CDTimeOver = !DoAction();
				if (CDTimeOver)
				{
					yield return new WaitForSeconds(num);
				}
			}
			else
			{
				float CurrCDTime = ActionCDTime;
				do
				{
					float time = ActionCDTime / 10f;
					yield return new WaitForSeconds(time);
					float num2 = 1f - (float)base.FrozenLevel * 0.05f;
					CurrCDTime -= time * CDTimeScale * num2;
				}
				while (!(CurrCDTime <= 0f));
				CDTimeOver = true;
			}
		}
	}

	protected void SetAnimChange(int Integ)
	{
		PlantHeadAnimator.SetInteger("Change", Integ);
	}

	protected int GetBulletSortOrder(int line = 0)
	{
		return (Sorting.sortingOrder / 200 + line) * 200 + FixedInfo.BulletSort;
	}

	protected override void NeedSynInit()
	{
		RandomSpeed = Random.Range(SpeedRange.x, SpeedRange.y);
	}

	public override void ServerInitInfo()
	{
		ServerSendSyn(0, new Vector2(RandomSpeed, 0f));
	}

	protected override void OnlineSyn(SynItem syn)
	{
		RandomSpeed = syn.Twofloat.x;
		base.Speed = DefSpeed;
	}

	private void IsDoorArm(bool isDoorArm)
	{
		for (int i = 0; i < doorArmSprites.Count; i++)
		{
			doorArmSprites[i].enabled = isDoorArm;
		}
		for (int j = 0; j < LeftArmSprites.Count; j++)
		{
			LeftArmSprites[j].enabled = !isDoorArm;
		}
		for (int k = 0; k < RightArmSprites.Count; k++)
		{
			RightArmSprites[k].enabled = !isDoorArm;
		}
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = DoorRenderer.sprite;
		Sprite sprite2 = ArmorRenderer.sprite;
		if (DoorRenderer.enabled && base.DoorHp <= 0 && (sprite == door1 || sprite == door2 || sprite == door3))
		{
			if (nextSprite == null)
			{
				if (needDropDoor)
				{
					DropEquip(DoorRenderer);
				}
				IsDoorArm(isDoorArm: false);
				if ((float)base.Hp < 180f * base.HpScale)
				{
					DropArm();
				}
				if (base.InWater)
				{
					WhiteWaterDoor.gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			if (!ArmorRenderer.enabled || base.ArmorHp > 0 || (!(sprite2 == armor1) && !(sprite2 == armor2) && !(sprite2 == door3)) || !(nextSprite == null))
			{
				return;
			}
			DropEquip(ArmorRenderer);
			if (base.InWater)
			{
				if (base.DoorHp > 0)
				{
					WhiteWaterDoor.gameObject.SetActive(value: true);
				}
				else
				{
					WhiteWater.gameObject.SetActive(value: true);
				}
			}
		}
	}

	protected virtual void HpReduceEventPZombie()
	{
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp <= 70)
		{
			for (int i = 0; i < flagArmSprites.Count; i++)
			{
				flagArmSprites[i].enabled = false;
			}
			IsDoorArm(isDoorArm: false);
		}
		if ((float)base.Hp < 180f * base.HpScale && base.DoorHp <= 0)
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
		HpReduceEventPZombie();
	}

	protected override void InWaterChangeEvent()
	{
		SetAnimatorChange(10 + Random.Range(1, 3));
	}

	protected bool NormalDontAttackCondition()
	{
		if (LVManager.Instance.GameIsStart)
		{
			return false;
		}
		return true;
	}

	protected bool NormalProduceCondition()
	{
		if (!LVManager.Instance.GameIsStart)
		{
			return false;
		}
		return true;
	}

	protected override void DoorHpReduceEvent()
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

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("idle" + Random.Range(1, 3), 0, Random.Range(0f, 1f));
			break;
		case ZombieState.Walk:
			SetAnimatorChange(10 + Random.Range(1, 3));
			if (base.DoorHp > 0)
			{
				IsDoorArm(isDoorArm: true);
			}
			break;
		case ZombieState.Attack:
			SetAnimatorChange(21);
			if (base.DoorHp > 0)
			{
				IsDoorArm(isDoorArm: false);
			}
			break;
		case ZombieState.Dead:
			base.DoorHp = 0;
			SetAnimatorChange(30 + Random.Range(1, 3));
			break;
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		_ = EquipRenderer.sprite;
		SpriteRenderer result = null;
		if (base.DoorHp > 0)
		{
			result = DoorRenderer;
			if (needClearEquip)
			{
				needDropDoor = false;
				base.DoorHp = 0;
			}
		}
		return result;
	}

	public override void OutInWaterEvent()
	{
		WhiteWaterDoor.gameObject.SetActive(value: false);
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

	public override void ZombieOnDead(bool dropItem)
	{
		for (int i = 0; i < Seaweeds.Count; i++)
		{
			Seaweeds[i].enabled = false;
		}
	}

	protected override void TangkleStopActionVirtual()
	{
		if (WhiteWaterDoor != null)
		{
			WhiteWaterDoor.gameObject.SetActive(value: false);
		}
	}
}
