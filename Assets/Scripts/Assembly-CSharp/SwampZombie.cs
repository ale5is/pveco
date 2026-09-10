using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SwampZombie : ZombieBase
{
	public SwampZombieType Type;

	public Sprite strawHat1;

	public Sprite strawHat2;

	public Sprite cone1;

	public Sprite cone2;

	public Sprite cone3;

	public Sprite bucket1;

	public Sprite bucket2;

	public Sprite bucket3;

	public Sprite door1;

	public Sprite door2;

	public Sprite door3;

	public SpriteRenderer WhiteWaterDoor;

	public SpriteRenderer ConeRenderer;

	public SpriteRenderer BucketRenderer;

	private float RandomSpeed = 4f;

	public List<SpriteRenderer> LeftArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> RightArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> flagArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> doorArmSprites = new List<SpriteRenderer>();

	private bool needDropHat;

	private bool needDropDoor;

	public List<SpriteRenderer> Seaweeds = new List<SpriteRenderer>();

	protected override float DefSpeed => RandomSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => GameManager.Instance.GameConf.SwampZombie;

	protected override float AnToSpeed => 5f;

	protected override int CriticalHp => 70;

	private int GetTypeHp()
	{
		int result = 270;
		switch (Type)
		{
		case SwampZombieType.Normal:
			result = 350;
			break;
		case SwampZombieType.Stool:
			result = 640;
			break;
		case SwampZombieType.Bucket:
			result = 1370;
			break;
		case SwampZombieType.Door:
			result = 350;
			break;
		case SwampZombieType.DoorAndStool:
			result = 640;
			break;
		case SwampZombieType.DoorAndBucket:
			result = 1370;
			break;
		case SwampZombieType.Flag:
			result = 350;
			break;
		case SwampZombieType.FlagStool:
			result = 640;
			break;
		case SwampZombieType.FlagBucket:
			result = 1370;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		needDropDoor = true;
		canButter = true;
		ConeRenderer.enabled = false;
		BucketRenderer.enabled = false;
		HpState.Clear();
		DoorHpState.Clear();
		E1HpStateSprite.Clear();
		DoorHpStateSprite.Clear();
		for (int i = 0; i < flagArmSprites.Count; i++)
		{
			flagArmSprites[i].enabled = false;
		}
		bool isDoorArm = false;
		switch (Type)
		{
		case SwampZombieType.Normal:
			EquipRenderer = ConeRenderer;
			HpState = new List<int> { 350, 310, 270 };
			E1HpStateSprite = new List<Sprite> { strawHat1, strawHat2, null };
			break;
		case SwampZombieType.Stool:
			EquipRenderer = BucketRenderer;
			HpState = new List<int> { 840, 720, 500, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			break;
		case SwampZombieType.Bucket:
			EquipRenderer = BucketRenderer;
			canButter = false;
			HpState = new List<int> { 1470, 1100, 740, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			break;
		case SwampZombieType.Door:
			EquipRenderer = ConeRenderer;
			isDoorArm = true;
			HpState = new List<int> { 350, 310, 270 };
			E1HpStateSprite = new List<Sprite> { strawHat1, strawHat2, null };
			DoorHpState = new List<int> { 600, 400, 200, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			break;
		case SwampZombieType.DoorAndStool:
			EquipRenderer = BucketRenderer;
			isDoorArm = true;
			HpState = new List<int> { 640, 520, 400, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			break;
		case SwampZombieType.DoorAndBucket:
			EquipRenderer = BucketRenderer;
			isDoorArm = true;
			canButter = false;
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			break;
		case SwampZombieType.Flag:
			RandomSpeed = 2.4f;
			EquipRenderer = ConeRenderer;
			HpState = new List<int> { 350, 310, 270 };
			E1HpStateSprite = new List<Sprite> { strawHat1, strawHat2, null };
			break;
		case SwampZombieType.FlagStool:
			RandomSpeed = 2.3f;
			EquipRenderer = ConeRenderer;
			HpState = new List<int> { 640, 520, 400, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			break;
		case SwampZombieType.FlagBucket:
			RandomSpeed = 2.2f;
			EquipRenderer = BucketRenderer;
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			break;
		}
		IsDoorArm(isDoorArm);
		if (Type == SwampZombieType.Flag || Type == SwampZombieType.FlagStool || Type == SwampZombieType.FlagBucket)
		{
			for (int j = 0; j < flagArmSprites.Count; j++)
			{
				flagArmSprites[j].enabled = true;
			}
			for (int k = 0; k < LeftArmSprites.Count; k++)
			{
				LeftArmSprites[k].enabled = true;
			}
			for (int l = 0; l < RightArmSprites.Count; l++)
			{
				RightArmSprites[l].enabled = false;
			}
		}
	}

	protected override void NeedSynInit()
	{
		RandomSpeed = Random.Range(3.2f, 5.5f);
	}

	public override void ServerInitInfo()
	{
		ServerSendSyn(0, new Vector2(RandomSpeed, 0f));
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			RandomSpeed = syn.Twofloat.x;
			base.Speed = DefSpeed;
		}
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
		Sprite sprite = EquipRenderer.sprite;
		Sprite sprite2 = DoorRenderer.sprite;
		if (DoorRenderer.enabled && base.DoorHp <= 0 && (sprite2 == door1 || sprite2 == door2 || sprite2 == door3))
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
					WhiteWater.gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			if ((sprite == bucket1 || sprite == bucket2 || sprite == bucket3) && nextSprite == null && needDropHat)
			{
				DropEquip(EquipRenderer);
				canButter = true;
			}
			if ((sprite == cone1 || sprite == cone2 || sprite == cone3) && nextSprite == null && needDropHat)
			{
				DropEquip(EquipRenderer);
			}
			if ((sprite == strawHat1 || sprite == strawHat2) && nextSprite == null && needDropHat)
			{
				DropEquip(EquipRenderer);
			}
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (base.Hp <= 0)
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
		if (Type == SwampZombieType.Bucket && base.Hp <= 270 && !canButter)
		{
			canButter = true;
			canIce = true;
			canFrozen = true;
		}
		if (!HitSound)
		{
			return;
		}
		if (isHard && (float)base.Hp > 270f * base.HpScale && Type != SwampZombieType.Normal && Type != SwampZombieType.Flag && Type != SwampZombieType.Door)
		{
			switch (Type)
			{
			case SwampZombieType.Stool:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
				}
				break;
			case SwampZombieType.FlagStool:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
				}
				break;
			case SwampZombieType.Bucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case SwampZombieType.FlagBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case SwampZombieType.DoorAndStool:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodHit2, base.transform.position);
				}
				break;
			case SwampZombieType.DoorAndBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case SwampZombieType.Door:
			case SwampZombieType.Flag:
				break;
			}
		}
		else if (Random.Range(0, 3) == 0)
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

	protected override void InWaterChangeEvent()
	{
		WhiteWaterDoor.gameObject.SetActive(value: false);
		WhiteWaterDoor.maskInteraction = SpriteMaskInteraction.None;
		if (base.InWater && base.DoorHp > 0)
		{
			WhiteWater.gameObject.SetActive(value: false);
			WhiteWaterDoor.gameObject.SetActive(value: true);
		}
		if (base.InWater && base.ArmorHp > 0)
		{
			SetAnimatorChange(13);
		}
		else
		{
			SetAnimatorChange(10 + Random.Range(1, 3));
		}
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
			if (base.InWater && base.ArmorHp > 0)
			{
				SetAnimatorChange(13);
			}
			else
			{
				SetAnimatorChange(10 + Random.Range(1, 3));
			}
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

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		Sprite sprite = EquipRenderer.sprite;
		SpriteRenderer result = null;
		if (base.Hp > 270 && (sprite == bucket1 || sprite == bucket2 || sprite == bucket3))
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

	protected override int HandleHurt(int attackValue, Vector2 dirction)
	{
		if (base.Hp > 270 && (Type == SwampZombieType.Stool || Type == SwampZombieType.DoorAndStool) && dirction.y < 0f)
		{
			attackValue /= 20;
		}
		return attackValue;
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

	public override void ZombieOnDead(bool dropItem)
	{
		for (int i = 0; i < Seaweeds.Count; i++)
		{
			Seaweeds[i].enabled = false;
		}
	}

	protected override void TangkleStopActionVirtual()
	{
		WhiteWaterDoor.gameObject.SetActive(value: false);
	}
}
