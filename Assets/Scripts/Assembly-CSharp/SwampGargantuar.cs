using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SwampGargantuar : ZombieBase
{
	public SwampGargantuarType Type;

	public Sprite BungeeHead;

	public Sprite NormalBlueHead;

	public Sprite BungeeBlueHead;

	public Sprite NormalArm;

	public Sprite BungeeArm;

	public Sprite hat1;

	public Sprite hat2;

	public Sprite hat3;

	public Sprite Body1;

	public Sprite Body2;

	public Sprite Body3;

	public Texture2D CharredImp;

	public SpriteRenderer WeaponRenderer;

	public SpriteRenderer BodyRenderer;

	public List<Sprite> DecoratesTex = new List<Sprite>();

	public List<SpriteRenderer> CrocodileSprites = new List<SpriteRenderer>();

	public SpriteRenderer CrocodileRope;

	private bool isNoCrocodile;

	private float defSpeed;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 2000f;

	public override int MaxHP => GetTypeHp();

	protected override GameObject Prefab => GameManager.Instance.GameConf.SwampGargantuar;

	protected override float AnToSpeed => 2.8f;

	public override int OnlineIdNum => 2;

	protected override Vector2 AttackDir => new Vector2(0f, -1f);

	public override bool CanEatByChomper => false;

	public override bool CanUseLadder => false;

	protected override float InWaterDistance => -0.4f;

	protected override float OutWaterDistance => 0f;

	protected override float inWaterDepth => 0.8f;

	protected override float NextGridAttackDis => 1.5f;

	public override bool CanBlowBack => false;

	public override Vector3 SplashScale => new Vector3(2.5f, 2f);

	public override Vector3 VaseScale => new Vector3(0.3f, 0.3f);

	public override Vector3 VaseOffset => new Vector3(0f, -0.25f);

	protected override bool CanEatDontEat => true;

	private int GetTypeHp()
	{
		int result = 3000;
		switch (Type)
		{
		case SwampGargantuarType.Normal:
			result = 4800;
			break;
		case SwampGargantuarType.NoCrocodile:
			result = 4800;
			break;
		case SwampGargantuarType.Blueeye:
			result = 7800;
			break;
		case SwampGargantuarType.BlueeyeNoCrocodile:
			result = 7800;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		defSpeed = 4.2f;
		canButter = true;
		canIce = true;
		canFrozen = true;
		isNoCrocodile = false;
		HpState.Clear();
		E1HpStateSprite.Clear();
		GiveWeapon();
		CrocodileRope.enabled = true;
		for (int i = 0; i < CrocodileSprites.Count; i++)
		{
			CrocodileSprites[i].enabled = true;
		}
		HeadRenderer.sprite = normalHead;
		MidArmRenderer.sprite = NormalArm;
		BodyRenderer.sprite = Body1;
		switch (Type)
		{
		case SwampGargantuarType.Normal:
			HpState = new List<int> { 4800, 4200, 3600, 3000 };
			E1HpStateSprite = new List<Sprite> { hat1, hat2, hat3, null };
			break;
		case SwampGargantuarType.NoCrocodile:
		{
			isNoCrocodile = true;
			CrocodileRope.enabled = false;
			for (int k = 0; k < CrocodileSprites.Count; k++)
			{
				CrocodileSprites[k].enabled = false;
			}
			HpState = new List<int> { 4800, 4200, 3600, 3000 };
			E1HpStateSprite = new List<Sprite> { hat1, hat2, hat3, null };
			break;
		}
		case SwampGargantuarType.Blueeye:
			defSpeed = 3f;
			base.AddSpeed = 0.5f;
			HeadRenderer.sprite = NormalBlueHead;
			HpState = new List<int> { 7800, 7200, 6600, 4500 };
			E1HpStateSprite = new List<Sprite> { hat1, hat2, hat3, null };
			break;
		case SwampGargantuarType.BlueeyeNoCrocodile:
		{
			defSpeed = 3f;
			base.AddSpeed = 0.5f;
			HeadRenderer.sprite = NormalBlueHead;
			HpState = new List<int> { 7800, 7200, 6600, 4500 };
			E1HpStateSprite = new List<Sprite> { hat1, hat2, hat3, null };
			isNoCrocodile = true;
			CrocodileRope.enabled = false;
			for (int j = 0; j < CrocodileSprites.Count; j++)
			{
				CrocodileSprites[j].enabled = false;
			}
			break;
		}
		}
	}

	private void GiveWeapon()
	{
		WeaponRenderer.sprite = DecoratesTex[Random.Range(0, DecoratesTex.Count)];
	}

	private void ownerAttack()
	{
		bool flag = false;
		if (hypnoAttackTarget != null && hypnoAttackTarget.Hp <= 0)
		{
			hypnoAttackTarget = null;
		}
		if (isHypno)
		{
			if (hypnoAttackTarget != null)
			{
				hypnoAttackTarget.Hurt((int)attackValue, Vector2.down);
			}
			else if (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno)
			{
				CreateWaterSplah(base.CurrGrid);
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, Vector2.down, this, isFlat: true);
			}
			else if (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.isHypno)
			{
				CreateWaterSplah(base.NextGrid);
				base.NextGrid.CurrPlantBase.Hurt(attackValue, Vector2.down, this, isFlat: true);
			}
			return;
		}
		if (hypnoAttackTarget != null)
		{
			hypnoAttackTarget.Hurt((int)attackValue, Vector2.down);
		}
		else if (base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno)
		{
			flag = true;
			CreateWaterSplah(base.CurrGrid);
			base.CurrGrid.CurrPlantBase.Hurt(attackValue, Vector2.down, this, isFlat: true);
		}
		else if (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && !base.NextGrid.CurrPlantBase.isHypno)
		{
			flag = true;
			CreateWaterSplah(base.NextGrid);
			base.NextGrid.CurrPlantBase.Hurt(attackValue, Vector2.down, this, isFlat: true);
		}
		if (flag)
		{
			return;
		}
		if (base.CurrGrid.Vases.Count > 0)
		{
			for (int i = 0; i < base.CurrGrid.Vases.Count; i++)
			{
				base.CurrGrid.Vases[i].BreakEvent();
			}
		}
		else if (base.NextGrid.Vases.Count > 0)
		{
			for (int j = 0; j < base.NextGrid.Vases.Count; j++)
			{
				base.NextGrid.Vases[j].BreakEvent();
			}
		}
	}

	private void CreateWaterSplah(Grid grid)
	{
		if (grid.isNoIceWater)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(grid.Position, base.CurrGrid.Point.y);
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
			}
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		ThrowCrocodile();
		if (base.Hp <= MaxHP / 4)
		{
			MidArmRenderer.sprite = BungeeArm;
			BodyRenderer.sprite = Body3;
		}
		else if (base.Hp <= MaxHP / 2)
		{
			if (Type == SwampGargantuarType.Normal || Type == SwampGargantuarType.NoCrocodile)
			{
				HeadRenderer.sprite = BungeeHead;
			}
			else
			{
				HeadRenderer.sprite = BungeeBlueHead;
			}
			BodyRenderer.sprite = Body2;
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

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = EquipRenderer.sprite;
		if ((sprite == hat1 || sprite == hat2 || sprite == hat3) && nextSprite == null)
		{
			DropEquip(EquipRenderer);
		}
	}

	private void ThrowCrocodile(bool synClient = false)
	{
		if (GameManager.Instance.isClient && !synClient)
		{
			return;
		}
		if (synClient)
		{
			base.State = ZombieState.Attack;
			SetAnimatorChange(41);
			anCanMove = false;
		}
		else if (base.Hp <= MaxHP / 2 && !isNoCrocodile)
		{
			Grid farestGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
			if (farestGrid != null && Mathf.Abs(farestGrid.Position.x - base.transform.position.x) > 6f)
			{
				base.State = ZombieState.Attack;
				SetAnimatorChange(41);
				anCanMove = false;
				ServerSendSyn(0);
			}
		}
	}

	protected override void EatOutCheck()
	{
		if (isHypno)
		{
			if (!(hypnoAttackTarget == null) && (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.collider2d.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f))
			{
				hypnoAttackTarget = null;
			}
		}
		else if (hypnoAttackTarget != null && (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.collider2d.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f))
		{
			hypnoAttackTarget = null;
		}
	}

	private void OwnerEatCheck()
	{
		if (isHypno)
		{
			if (hypnoAttackTarget == null)
			{
				base.State = ZombieState.Walk;
			}
			else if (hypnoAttackTarget.Hp <= 0 || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
			{
				base.State = ZombieState.Walk;
				hypnoAttackTarget = null;
			}
			else if (base.CurrGrid.CurrPlantBase == null)
			{
				base.State = ZombieState.Walk;
			}
			else if (!base.CurrGrid.CurrPlantBase.isHypno)
			{
				base.State = ZombieState.Walk;
			}
		}
		else if (hypnoAttackTarget != null)
		{
			if (hypnoAttackTarget.Hp <= 0 || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
			{
				base.State = ZombieState.Walk;
				hypnoAttackTarget = null;
			}
		}
		else if (base.CurrGrid.CurrPlantBase == null)
		{
			base.State = ZombieState.Walk;
		}
		else if (base.CurrGrid.CurrPlantBase.isHypno)
		{
			base.State = ZombieState.Walk;
		}
		if (base.State == ZombieState.Walk)
		{
			ServerSendSyn(1);
		}
	}

	protected override void EatEnterCheck()
	{
		base.EatEnterCheck();
		if (base.NextGrid != null && base.NextGrid.Vases.Count > 0 && Mathf.Abs(base.transform.position.x - base.NextGrid.Position.x) < NextGridAttackDis && Mathf.Abs(base.transform.position.y - base.NextGrid.Position.y) < 0.2f)
		{
			AttackGrid = base.NextGrid;
			base.State = ZombieState.Attack;
		}
		else if (base.CurrGrid.Vases.Count > 0 && ((base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x >= 0f) || (!base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x <= 0f)) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.6f && Mathf.Abs(base.transform.position.y - base.CurrGrid.Position.y) < 0.2f)
		{
			AttackGrid = base.CurrGrid;
			base.State = ZombieState.Attack;
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombieGargantuar).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale, (!isNoCrocodile) ? 1 : 0);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			if (syn.SynCode[1] == 0)
			{
				ThrowCrocodile(synClient: true);
			}
			else if (syn.SynCode[1] == 1)
			{
				base.State = ZombieState.Walk;
			}
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (base.State == ZombieState.Attack)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.lowgroan1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.lowgroan2, base.transform.position);
			}
		}
		else if (base.State == ZombieState.Dead)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuardeath, base.transform.position);
		}
	}

	public override void SpecialAnimEvent2()
	{
		ownerAttack();
		CameraControl.Instance.ShakeCamera(base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
	}

	public override void SpecialAnimEvent3()
	{
		OwnerEatCheck();
	}

	public override void SpecialAnimEvent4()
	{
		if (isNoCrocodile)
		{
			return;
		}
		int num = 1;
		if (!base.IsFacingLeft)
		{
			num = -1;
		}
		Crocodile component = ZombieManager.Instance.CreateOneZombie(GameManager.Instance.GameConf.Crocodile, base.CurrLine, base.transform.position + new Vector3(-2.26f * (float)num, 2.56f)).GetComponent<Crocodile>();
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position + new Vector3(-5.5f * (float)num, 0f), base.CurrLine);
		Vector2 goal = new Vector2(base.transform.position.x - 5.5f * (float)num, gridByWorldPos.Position.y);
		if (isHypno)
		{
			if (needHypnoPurple)
			{
				component.Hypno();
			}
			else
			{
				component.RatThis();
			}
		}
		component.ThrowInit(goal);
		component.OnlineId = OnlineId - 1;
		for (int i = 0; i < CrocodileSprites.Count; i++)
		{
			CrocodileSprites[i].enabled = false;
		}
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.crocodileYell1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.crocodileYell2, base.transform.position);
		}
		base.dontChangeState = false;
		CrocodileRope.enabled = false;
		isNoCrocodile = true;
	}

	public override void SpecialAnimEvent5()
	{
		base.State = ZombieState.Walk;
		anCanMove = true;
	}

	public override void AnimFailSound()
	{
		CameraControl.Instance.ShakeCamera(base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
	}
}
