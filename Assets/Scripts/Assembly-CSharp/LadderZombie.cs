using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class LadderZombie : ZombieBase
{
	public Sprite ladder1;

	public Sprite ladder2;

	public Sprite ladder3;

	public SpriteRenderer placeLadderRender;

	private float defSpeed = 4f;

	private bool canLadder;

	private bool needDropDoor;

	protected override GameObject Prefab => GameManager.Instance.GameConf.LadderZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 500;

	protected override int CriticalHp => 167;

	protected override void UpdateThis()
	{
		if (!IsOVer && !base.IsCriticalState && !GameManager.Instance.isClient && canLadder && base.State == ZombieState.Attack && base.DoorHp > 0 && AttackGrid != null && ((!base.IsFacingLeft && !AttackGrid.HaveLeftLadder) || (base.IsFacingLeft && !AttackGrid.HaveRightLadder)) && AttackGrid.CurrPlantBase != null && (AttackGrid.CurrPlantBase.MaxHp >= FixedInfo.LadderPlantHp || (AttackGrid.CurrPlantBase.ProtectPlant != null && AttackGrid.CurrPlantBase.ProtectPlant.MaxHp >= FixedInfo.LadderPlantHp) || (AttackGrid.CurrPlantBase.CarryPlant != null && AttackGrid.CurrPlantBase.CarryPlant.MaxHp >= FixedInfo.LadderPlantHp)))
		{
			canLadder = false;
			base.dontChangeState = true;
			animator.SetInteger("Change", 41);
			placeLadderRender.enabled = true;
			DoorRenderer.enabled = false;
			ServerSendSyn(0);
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			if (syn.SynCode[1] == 0)
			{
				base.State = ZombieState.Attack;
				base.dontChangeState = true;
				placeLadderRender.enabled = true;
				DoorRenderer.enabled = false;
				animator.SetInteger("Change", 41);
			}
			else if (syn.SynCode[1] == 1)
			{
				canLadder = false;
				needDropDoor = false;
				base.DoorHp = 0;
				defSpeed = 4f;
				base.Speed = defSpeed;
				base.State = ZombieState.Walk;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.placeLadder, base.transform.position);
			}
		}
	}

	public override void InitZombieHpState()
	{
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			base.Speed = 1.8f;
		}
		placeLadderRender.enabled = false;
		defSpeed = 1.8f;
		canLadder = true;
		needDropDoor = true;
		DoorHpState = new List<int> { 500, 300, 150, 0 };
		DoorHpStateSprite = new List<Sprite> { ladder1, ladder2, ladder3, null };
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 320f * base.HpScale && base.DoorHp <= 0)
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

	protected override void DoorHpReduceEvent()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
		}
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = DoorRenderer.sprite;
		if (base.DoorHp <= 0 && (sprite == ladder1 || sprite == ladder2 || sprite == ladder3) && nextSprite == null)
		{
			if (needDropDoor)
			{
				DropEquip(DoorRenderer);
			}
			LowArmRenderer.enabled = true;
			defSpeed = 4f;
			base.Speed = defSpeed;
			if (base.State == ZombieState.Walk)
			{
				animator.SetInteger("Change", 12);
			}
			if (base.State == ZombieState.Attack)
			{
				animator.SetInteger("Change", 22);
			}
			if ((float)base.Hp < 320f * base.HpScale && LowArmRenderer.enabled)
			{
				DropArm();
			}
		}
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (base.DoorHp > 0)
			{
				animator.SetInteger("Change", 11);
			}
			else
			{
				animator.SetInteger("Change", 12);
			}
			break;
		case ZombieState.Attack:
			if (base.DoorHp > 0)
			{
				animator.SetInteger("Change", 21);
			}
			else
			{
				animator.SetInteger("Change", 22);
			}
			break;
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		_ = DoorRenderer.sprite;
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

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override void SpecialAnimEvent1()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		canLadder = true;
		placeLadderRender.enabled = false;
		if (AttackGrid != null && AttackGrid.CurrPlantBase != null && (AttackGrid.CurrPlantBase.MaxHp >= FixedInfo.LadderPlantHp || (AttackGrid.CurrPlantBase.ProtectPlant != null && AttackGrid.CurrPlantBase.ProtectPlant.MaxHp >= FixedInfo.LadderPlantHp) || (AttackGrid.CurrPlantBase.CarryPlant != null && AttackGrid.CurrPlantBase.CarryPlant.MaxHp >= FixedInfo.LadderPlantHp)))
		{
			if (base.IsFacingLeft)
			{
				AttackGrid.SetLadder(isLeft: false, isHave: true);
			}
			else
			{
				AttackGrid.SetLadder(isLeft: true, isHave: true);
			}
			canLadder = false;
			needDropDoor = false;
			base.DoorHp = 0;
			defSpeed = 4f;
			base.Speed = defSpeed;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.placeLadder, base.transform.position);
			ServerSendSyn(1);
		}
		base.dontChangeState = false;
		if (canLadder)
		{
			placeLadderRender.enabled = false;
			DoorRenderer.enabled = true;
		}
		base.State = ZombieState.Walk;
	}
}
