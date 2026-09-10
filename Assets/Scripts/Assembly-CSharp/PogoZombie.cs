using System.Collections;
using SocketSave;
using UnityEngine;

public class PogoZombie : ZombieBase
{
	private Transform anim;

	private float animY;

	private bool isGoUp;

	public int jumpNum;

	private float jumpHigh;

	private float ownerSpeed;

	public SpriteRenderer Pogo;

	public Sprite Pogo1;

	public Sprite Pogo2;

	public Sprite Pogo3;

	protected override GameObject Prefab => GameManager.Instance.GameConf.PogoZombie;

	protected override float AnToSpeed => ownerSpeed;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 80f;

	public override int MaxHP => 500;

	protected override int CriticalHp => 167;

	public override void InitZombieHpState()
	{
		jumpHigh = 0.8f;
		jumpNum = 0;
		anim = animator.transform;
		BanAllInmobilize = true;
		anim.localPosition = new Vector3(-6f, 4f);
		animY = anim.localPosition.y;
		Pogo.sprite = Pogo1;
		LowArmRenderer.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
		isGoUp = true;
		SetAnimatorChange(0);
		animator.Play("pogo");
		ownerSpeed = 2f;
		if (!IsOVer)
		{
			animator.speed = 1f;
		}
		base.dontChangeState = true;
		dontCgStCanInwater = true;
	}

	protected override void UpdateThis()
	{
		if (IsOVer || !base.dontChangeState || base.State == ZombieState.Dead)
		{
			return;
		}
		if (jumpNum <= 2 && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.ZombieCanEat && ((isHypno && base.CurrGrid.CurrPlantBase.isHypno) || (!isHypno && !base.CurrGrid.CurrPlantBase.isHypno)) && ((base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x >= 0f) || (!base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x <= 0f)) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.7f)
		{
			anCanMove = false;
		}
		float num = 1.2f + (float)jumpNum * 0.7f;
		if (isGoUp && Pogo.enabled)
		{
			if (anim.localPosition.y < animY + jumpHigh * num)
			{
				anim.Translate(new Vector2(0f, 1f) * Time.deltaTime * num);
			}
		}
		else if (anim.localPosition.y > animY)
		{
			anim.Translate(new Vector2(0f, -1.2f) * Time.deltaTime * num);
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			if (syn.SynCode[1] == 0)
			{
				anCanMove = false;
			}
			else if (syn.SynCode[1] == 1)
			{
				jumpNum++;
				base.collider2d.enabled = false;
				anCanMove = true;
				base.Speed = 1f;
				animator.speed = 1f;
			}
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 0 && Pogo.enabled)
		{
			result = Pogo;
			if (needClearEquip)
			{
				ClearPogo();
			}
		}
		return result;
	}

	protected override void CheckState()
	{
		switch (base.State)
		{
		case ZombieState.Idel:
			animator.Play("pogo", 0, Random.Range(0f, 1f));
			break;
		case ZombieState.Walk:
			if (!base.dontChangeState)
			{
				SetAnimatorChange(11);
			}
			break;
		case ZombieState.Attack:
			SetAnimatorChange(21);
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			base.collider2d.enabled = false;
			StartCoroutine(MoveDown());
			SetAnimatorChange(31);
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 320f * base.HpScale)
		{
			DropArm();
			LowArmRenderer.transform.localScale = new Vector3(0f, 0f, 1f);
		}
		if ((float)base.Hp < 240f * base.HpScale && Pogo.sprite == Pogo2)
		{
			Pogo.sprite = Pogo3;
		}
		else if ((float)base.Hp < 380f * base.HpScale && Pogo.sprite == Pogo1)
		{
			Pogo.sprite = Pogo2;
		}
		if (base.Hp <= 0)
		{
			isGoUp = false;
			jumpNum = 2;
			base.dontChangeState = false;
			base.State = ZombieState.Dead;
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
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override void SpecialAnimEvent1()
	{
		if (!anCanMove)
		{
			jumpNum++;
		}
		animator.speed = 1f;
		if (!IsOVer)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.pogo_jump, base.transform.position);
		}
	}

	public override void SpecialAnimEvent2()
	{
		isGoUp = false;
	}

	public override void SpecialAnimEvent3()
	{
		isGoUp = true;
		if (jumpNum > 2)
		{
			if (jumpNum == 4)
			{
				jumpNum = 0;
				base.collider2d.enabled = true;
				base.Speed = DefSpeed;
			}
			if (jumpNum == 3)
			{
				jumpNum++;
				base.collider2d.enabled = false;
				anCanMove = true;
				base.Speed = 1f;
				animator.speed = 1f;
			}
		}
	}

	public override void SpecialAnimEvent4()
	{
		if (base.CurrGrid != null && jumpNum == 4 && ((base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.CarryPlant != null && base.CurrGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut)))
		{
			DropEquip(Pogo);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bonk, base.transform.position);
			ClearPogo();
		}
	}

	private void ClearPogo()
	{
		Pogo.enabled = false;
		base.Speed = 4.5f;
		ownerSpeed = 4.5f;
		canIce = true;
		base.dontChangeState = false;
		anim.localPosition = new Vector3(-6.3f, anim.localPosition.y);
		base.collider2d.enabled = true;
		base.State = ZombieState.Walk;
		animator.Play("walk1");
		if (!base.InWater)
		{
			Shadow.enabled = true;
		}
		BanAllInmobilize = false;
		StartCoroutine(MoveDown());
	}

	private IEnumerator MoveDown()
	{
		while (anim.localPosition.y > 4f)
		{
			yield return null;
			anim.Translate(new Vector2(0f, -3f) * Time.deltaTime);
		}
		anim.localPosition = new Vector3(anim.localPosition.x, 4f);
	}

	protected override void AlmanacInitZombie()
	{
		IsOVer = false;
	}

	protected override void InWaterChangeEvent()
	{
		if (base.InWater && Pogo.enabled)
		{
			ClearPogo();
		}
	}
}
