using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobsledZombie : ZombieBase
{
	public BobsledType Type;

	public Sprite helmet1;

	public Sprite helmet2;

	public Sprite helmet3;

	private int onlineIdNum;

	private bool OnSled;

	private float AnTospeed;

	private float defSpeed;

	private int walkGridNum;

	private Vector2 jumpPos;

	private Sled sled;

	private BobsledZombie MainZombie;

	private List<BobsledZombie> bobsleds = new List<BobsledZombie>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.BobsledZombie;

	protected override float AnToSpeed => AnTospeed;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => GetTypeHp();

	protected override int CriticalHp => 70;

	public override int OnlineIdNum => onlineIdNum;

	public override bool CanBlowBack => false;

	private int GetTypeHp()
	{
		int result = 270;
		switch (Type)
		{
		case BobsledType.Helmet:
			result = 1370;
			break;
		case BobsledType.HelmetAndSled:
			result = 1370;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		canIce = false;
		sled = null;
		OnSled = false;
		MainZombie = null;
		defSpeed = 4f;
		AnTospeed = 6f;
		onlineIdNum = 1;
		walkGridNum = 0;
		bobsleds.Clear();
		base.collider2d.GetComponent<CapsuleCollider2D>().size = new Vector2(0.3f, 1.4f);
		base.collider2d.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
		HpState.Clear();
		E1HpStateSprite.Clear();
		switch (Type)
		{
		case BobsledType.Helmet:
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
			break;
		case BobsledType.Sled:
			xxx(BobsledType.Normal);
			BanAllInmobilize = true;
			break;
		case BobsledType.HelmetAndSled:
			HpState = new List<int> { 1370, 1000, 640, 270 };
			E1HpStateSprite = new List<Sprite> { helmet1, helmet2, helmet3, null };
			xxx(BobsledType.Helmet);
			BanAllInmobilize = true;
			break;
		case BobsledType.Normal:
			break;
		}
	}

	private void xxx(BobsledType type)
	{
		OnSled = true;
		base.dontChangeState = true;
		onlineIdNum = 4;
		defSpeed = 1.5f;
		AnTospeed = 3f;
		if (LVManager.Instance.CurrLVState != LVState.Fighting)
		{
			return;
		}
		sled = Object.Instantiate(GameManager.Instance.GameConf.Sled).GetComponent<Sled>();
		sled.transform.SetParent(base.transform);
		sled.transform.localPosition = new Vector3(0f, 0.05f);
		sled.CreateInit(base.SortOrder);
		animator.Play("push", 0, 0f);
		base.collider2d.GetComponent<CapsuleCollider2D>().size = new Vector2(4.16f, 1.4f);
		base.collider2d.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
		for (int i = 0; i < 3; i++)
		{
			int num = base.SortOrder % 100 + 1;
			_ = ZombieManager.Instance.CurrOrderNum;
			Vector2 pos = base.transform.position + new Vector3(1.92f, 0f);
			Vector2 target = new Vector2(1.4f, 0f);
			switch (i)
			{
			case 1:
				num += 4;
				target = new Vector2(-0.7f, 0f);
				pos = base.transform.position + new Vector3(-0.58f, -0.2f);
				break;
			case 2:
				num += 5;
				target = new Vector2(0.7f, 0f);
				pos = base.transform.position + new Vector3(1.14f, -0.2f);
				break;
			}
			BobsledZombie component = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.BobsledZombie).GetComponent<BobsledZombie>();
			component.Type = type;
			component.Init(base.CurrLine, num, pos);
			component.transform.SetParent(base.transform);
			component.OnlineId = OnlineId - i - 1;
			component.SetPassenger(target, this);
			bobsleds.Add(component);
		}
	}

	protected override void UpdateThis()
	{
		if (!IsOVer && OnSled && (base.State == ZombieState.Walk || base.State == ZombieState.Attack))
		{
			if (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.ZombieCanEat && Vector2.Distance(base.NextGrid.Position, base.transform.position) < 1.5f)
			{
				HurtPlant(base.NextGrid);
			}
			if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.ZombieCanEat && Vector2.Distance(base.CurrGrid.Position, base.transform.position) < 0.7f)
			{
				HurtPlant(base.CurrGrid);
			}
		}
	}

	private void HurtPlant(Grid grid)
	{
		if (isHypno && grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, AttackDir, this, isFlat: true);
		}
		else if (!isHypno && !grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, AttackDir, this, isFlat: true);
		}
	}

	private void SetPassenger(Vector2 target, BobsledZombie zombie)
	{
		MainZombie = zombie;
		jumpPos = target;
		defSpeed = 1.5f;
		AnTospeed = 3f;
		base.Speed = defSpeed;
		anCanMove = false;
		animator.Play("push", 0, 0f);
	}

	private void JumpInSled()
	{
		if (base.State != ZombieState.Dead)
		{
			Shadow.enabled = false;
			SetAnimatorChange(41);
		}
	}

	private void LeaveSled()
	{
		if (base.State != ZombieState.Dead)
		{
			defSpeed = 4f;
			AnTospeed = 6f;
			base.Speed = defSpeed;
			base.dontChangeState = false;
			OnSled = false;
			anCanMove = true;
			Shadow.enabled = true;
			SetAnimatorChange(42);
		}
	}

	private IEnumerator MoveDown()
	{
		while (base.transform.localPosition.x != jumpPos.x)
		{
			yield return null;
			base.transform.localPosition = Vector2.MoveTowards(base.transform.localPosition, jumpPos, Time.deltaTime * 5f);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 180f * base.HpScale)
		{
			DropArm();
		}
		if (!HitSound || !HitSound)
		{
			return;
		}
		if (isHard && (float)base.Hp > 270f * base.HpScale)
		{
			switch (Type)
			{
			case BobsledType.Helmet:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
				break;
			case BobsledType.HelmetAndSled:
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlasticHit2, base.transform.position);
				}
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

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		if (!sled)
		{
			return;
		}
		walkGridNum++;
		if (walkGridNum == 1)
		{
			JumpInSled();
			for (int i = 0; i < bobsleds.Count; i++)
			{
				bobsleds[i].JumpInSled();
			}
		}
		if (base.CurrGrid.IceRoadNum == 0 && base.CurrGrid.SnowLvl == 0 && !base.CurrGrid.IsIce && (bool)sled)
		{
			sled.Hp -= 200;
			if (sled.Hp <= 0)
			{
				SledDead(isBoom: false);
			}
		}
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		if (nextSprite == null)
		{
			DropEquip(EquipRenderer);
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (sled != null)
		{
			defSpeed = 1.2f;
			AnTospeed = 1.2f;
			base.Speed = defSpeed;
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (sled != null)
		{
			sled.Jump();
		}
		else
		{
			StartCoroutine(MoveDown());
		}
	}

	private void SledDead(bool isBoom)
	{
		if ((bool)sled)
		{
			for (int i = 0; i < bobsleds.Count; i++)
			{
				bobsleds[i].transform.SetParent(base.transform.parent);
				bobsleds[i].LeaveSled();
			}
			bobsleds.Clear();
			LeaveSled();
			base.Speed = DefSpeed;
			sled.SledDead(isBoom);
			sled = null;
			BanAllInmobilize = false;
			base.collider2d.GetComponent<CapsuleCollider2D>().size = new Vector2(0.3f, 1.4f);
			base.collider2d.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
		}
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (sled != null)
		{
			for (int i = 0; i < bobsleds.Count; i++)
			{
				bobsleds[i].transform.SetParent(base.transform.parent);
				bobsleds[i].LeaveSled();
			}
			bobsleds.Clear();
			Object.Destroy(sled.gameObject);
		}
		if (MainZombie != null)
		{
			MainZombie.bobsleds.Remove(this);
		}
	}

	protected override int HandleHurt(int attackValue, Vector2 dirction)
	{
		if ((bool)sled && sled.Hp > 0)
		{
			int num = sled.Hp - attackValue;
			sled.Hp -= attackValue;
			if (sled.Hp <= 0)
			{
				SledDead(isBoom: false);
			}
			if (attackValue > 0)
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
			if (num < 0)
			{
				return -num;
			}
			return 0;
		}
		return base.HandleHurt(attackValue, dirction);
	}

	protected override int HandleBoomHurt(int attackValue)
	{
		if ((bool)sled && sled.Hp > 0)
		{
			sled.Hp -= attackValue;
			if (sled.Hp <= 0)
			{
				SledDead(isBoom: true);
			}
		}
		return base.HandleBoomHurt(attackValue);
	}
}
