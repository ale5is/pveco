using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Zombie : ZombieBase
{
	public NormalZombieType Type;

	public Sprite cone1;

	public Sprite cone2;

	public Sprite cone3;

	public Sprite bucket1;

	public Sprite bucket2;

	public Sprite bucket3;

	public Sprite door1;

	public Sprite door2;

	public Sprite door3;

	public Sprite tube1;

	public Sprite tube2;

	public SpriteRenderer WhiteWaterTube;

	public SpriteRenderer WhiteWaterDoor;

	public SpriteRenderer ConeRenderer;

	public SpriteRenderer BucketRenderer;

	public List<SpriteRenderer> LeftArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> RightArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> flagArmSprites = new List<SpriteRenderer>();

	public List<SpriteRenderer> doorArmSprites = new List<SpriteRenderer>();

	public List<Texture2D> DecoratesTex = new List<Texture2D>();

	public List<SpriteRenderer> Seaweeds = new List<SpriteRenderer>();

	private float RandomSpeed = 4f;

	private bool needDropHat;

	private bool needDropDoor;

	protected override float DefSpeed => RandomSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => GameManager.Instance.GameConf.Zombie;

	protected override float AnToSpeed => 5f;

	protected override int CriticalHp => 70;

	private int GetTypeHp()
	{
		int result = 270;
		if (Type == NormalZombieType.Cone || Type == NormalZombieType.DoorAndCone || Type == NormalZombieType.FlagCone || Type == NormalZombieType.TubeCone || Type == NormalZombieType.TubeDoorCone)
		{
			result = 640;
		}
		else if (Type == NormalZombieType.Bucket || Type == NormalZombieType.DoorAndBucket || Type == NormalZombieType.FlagBucket || Type == NormalZombieType.TubeBucket || Type == NormalZombieType.TubeDoorBucket)
		{
			result = 1370;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		needDropHat = true;
		needDropDoor = true;
		ArmorHpAbsorb = 5;
		ConeRenderer.enabled = false;
		BucketRenderer.enabled = false;
		bool isDoorArm = false;
		for (int i = 0; i < flagArmSprites.Count; i++)
		{
			flagArmSprites[i].enabled = false;
		}
		switch (Type)
		{
		case NormalZombieType.Cone:
			SetEquip(0);
			break;
		case NormalZombieType.Bucket:
			SetEquip(1);
			break;
		case NormalZombieType.Door:
			SetEquip(2);
			isDoorArm = true;
			break;
		case NormalZombieType.DoorAndCone:
			SetEquip(0);
			SetEquip(2);
			isDoorArm = true;
			break;
		case NormalZombieType.DoorAndBucket:
			SetEquip(1);
			SetEquip(2);
			isDoorArm = true;
			break;
		case NormalZombieType.Flag:
			RandomSpeed = 2.4f;
			break;
		case NormalZombieType.FlagCone:
			SetEquip(0);
			RandomSpeed = 2.3f;
			break;
		case NormalZombieType.FlagBucket:
			SetEquip(1);
			RandomSpeed = 2.2f;
			break;
		case NormalZombieType.Tube:
			SetEquip(3);
			break;
		case NormalZombieType.TubeCone:
			SetEquip(0);
			SetEquip(3);
			break;
		case NormalZombieType.TubeBucket:
			SetEquip(1);
			SetEquip(3);
			break;
		case NormalZombieType.TubeDoor:
			SetEquip(2);
			SetEquip(3);
			isDoorArm = true;
			break;
		case NormalZombieType.TubeDoorCone:
			SetEquip(0);
			SetEquip(2);
			SetEquip(3);
			isDoorArm = true;
			break;
		case NormalZombieType.TubeDoorBucket:
			SetEquip(1);
			SetEquip(2);
			SetEquip(3);
			isDoorArm = true;
			break;
		}
		IsDoorArm(isDoorArm);
		if (Type == NormalZombieType.Flag || Type == NormalZombieType.FlagCone || Type == NormalZombieType.FlagBucket)
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

	private void SetEquip(int type)
	{
		switch (type)
		{
		case 0:
			EquipRenderer = ConeRenderer;
			HammerHpState = new List<int> { 270 };
			HpState = new List<int> { 640, 520, 400, 270 };
			E1HpStateSprite = new List<Sprite> { cone1, cone2, cone3, null };
			break;
		case 1:
			EquipRenderer = BucketRenderer;
			HammerHpState = new List<int> { 640, 270 };
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { bucket1, bucket2, bucket3, null };
			break;
		case 2:
			HammerDoorHpState = new List<int> { 640, 270 };
			DoorHpState = new List<int> { 1100, 760, 360, 0 };
			DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
			break;
		case 3:
			ArmorHpState = new List<int> { 180, 80, 0 };
			ArmorHpStateSprite = new List<Sprite> { tube1, tube2, null };
			break;
		}
	}

	protected override void NeedSynInit()
	{
		if (Type != NormalZombieType.Flag && Type != NormalZombieType.FlagCone && Type != NormalZombieType.FlagBucket)
		{
			RandomSpeed = Random.Range(3.2f, 5.5f);
		}
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

	private void SetDecorate()
	{
		if (DecoratesTex.Count == 0)
		{
			return;
		}
		if (DecoratesTex.Count <= 2)
		{
			REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[0]);
			if (DecoratesTex.Count == 2)
			{
				REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[1]);
			}
			return;
		}
		int num = Random.Range(0, DecoratesTex.Count);
		int num2 = Random.Range(0, DecoratesTex.Count);
		while (num == num2)
		{
			num2 = Random.Range(0, DecoratesTex.Count);
		}
		REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[num]);
		REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[num2]);
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = EquipRenderer.sprite;
		Sprite sprite2 = DoorRenderer.sprite;
		Sprite sprite3 = ArmorRenderer.sprite;
		if (DoorRenderer.enabled && base.DoorHp <= 0 && (sprite2 == door1 || sprite2 == door2 || sprite2 == door3))
		{
			if (!(nextSprite == null))
			{
				return;
			}
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
				if (base.ArmorHp > 0)
				{
					WhiteWaterTube.gameObject.SetActive(value: true);
				}
				else
				{
					WhiteWater.gameObject.SetActive(value: true);
				}
			}
		}
		else if (ArmorRenderer.enabled && base.ArmorHp <= 0 && (sprite3 == tube1 || sprite3 == tube2 || sprite3 == door3))
		{
			if (!(nextSprite == null))
			{
				return;
			}
			DropEquip(ArmorRenderer);
			SetAnimatorChange(10 + Random.Range(1, 3));
			if (base.InWater)
			{
				WhiteWaterTube.gameObject.SetActive(value: false);
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
		else
		{
			if ((sprite == bucket1 || sprite == bucket2 || sprite == bucket3) && nextSprite == null && needDropHat)
			{
				DropEquip(EquipRenderer);
			}
			if ((sprite == cone1 || sprite == cone2 || sprite == cone3) && nextSprite == null && needDropHat)
			{
				DropEquip(EquipRenderer);
			}
		}
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
		if (!HitSound)
		{
			return;
		}
		if (isHard && (float)base.Hp > 270f * base.HpScale)
		{
			switch (Type)
			{
			case NormalZombieType.Cone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.FlagCone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.Bucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.FlagBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.DoorAndCone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.DoorAndBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.TubeCone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.TubeDoorCone:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case NormalZombieType.TubeBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.TubeDoorBucket:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
				}
				break;
			case NormalZombieType.Door:
			case NormalZombieType.Tube:
			case NormalZombieType.TubeDoor:
			case NormalZombieType.Flag:
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
		WhiteWaterTube.gameObject.SetActive(value: false);
		WhiteWaterDoor.maskInteraction = SpriteMaskInteraction.None;
		WhiteWaterTube.maskInteraction = SpriteMaskInteraction.None;
		if (base.InWater)
		{
			if (base.DoorHp > 0)
			{
				WhiteWater.gameObject.SetActive(value: false);
				WhiteWaterDoor.gameObject.SetActive(value: true);
			}
			else if (base.ArmorHp > 0)
			{
				WhiteWater.gameObject.SetActive(value: false);
				WhiteWaterTube.gameObject.SetActive(value: true);
			}
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
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
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
		if (base.IsCriticalState)
		{
			base.PlaceCharred();
			return;
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, 0f + TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		Sprite sprite = EquipRenderer.sprite;
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
		else if (base.Hp > 270 && (sprite == bucket1 || sprite == bucket2 || sprite == bucket3))
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

	public override void OutInWaterEvent()
	{
		WhiteWaterDoor.gameObject.SetActive(value: false);
		WhiteWaterTube.gameObject.SetActive(value: false);
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

	public override void SpecialAnimEvent1()
	{
		if (base.ArmorHp <= 0)
		{
			anCanMove = false;
		}
	}

	protected override void TangkleStopActionVirtual()
	{
		WhiteWaterDoor.gameObject.SetActive(value: false);
		WhiteWaterTube.gameObject.SetActive(value: false);
	}
}
