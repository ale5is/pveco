using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class ZombieBase : MonoBehaviour
{
	public int OnlineId;

	public string PlacePlayer;

	public ZombieType zombieType;

	[SerializeField]
	private ZombieState state;

	protected Renderer REnderer;

	protected SwfClipController clipController;

	protected Animator animator;

	protected SortingGroup Sorting;

	protected List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	protected List<int> InMoreLines = new List<int>();

	private Grid currGrid;

	private Grid nextGrid;

	protected Grid lastGrid;

	[SerializeField]
	private bool inWater;

	private bool going;

	protected SpriteRenderer spriteRenderer;

	protected Grid AttackGrid;

	protected ZombieBase hypnoAttackTarget;

	public Vector2 MoveTarget;

	private bool isFacingLeft = true;

	private bool returnFirstClick;

	private MapBase currMap;

	protected bool IsOVer;

	private bool isDropArm;

	private bool isDropHead;

	public Vector2 SpawnPos;

	protected float TargetBaseY;

	protected bool BanAllInmobilize;

	private Coroutine changeLineCoroutine;

	protected bool needHypnoPurple;

	public bool isHypno;

	private int frozenLevel;

	private bool isFrozen;

	protected bool canFrozen = true;

	private Coroutine frozenCoroutine;

	protected bool canIce = true;

	private SpriteRenderer Icetrap;

	private Coroutine icetrapCoroutine;

	private bool isButter;

	public SpriteRenderer butter;

	protected bool canButter = true;

	private Coroutine butterCoroutine;

	private bool isDizzy;

	private int dizzyTime;

	private GameObject DizzyObject;

	protected bool canDizzy = true;

	private Coroutine dizzyCoroutine;

	private float addSpeed;

	protected SpriteRenderer Shadow;

	public bool invincible;

	public SpriteRenderer WhiteWater;

	public SpriteMask WaterMask;

	public Sprite normalArm;

	public Sprite lostArm;

	public SpriteRenderer UpArmRenderer;

	public SpriteRenderer MidArmRenderer;

	public SpriteRenderer LowArmRenderer;

	public List<SpriteRenderer> headSprites = new List<SpriteRenderer>();

	public Sprite normalHead;

	public Sprite yuckHead;

	public SpriteRenderer HeadRenderer;

	public SpriteRenderer JawRenderer;

	public bool anCanMove;

	protected bool dontCgStCanInwater;

	protected bool needInWater = true;

	protected bool needChangeLine = true;

	protected bool onlyBoomHurt;

	[SerializeField]
	private float speed;

	private float speedRate = 1f;

	private float customSpeed;

	private float customAttack;

	private int hp;

	private int doorHp;

	private int armorHp;

	protected bool CriticalHpEnable;

	public SpriteRenderer EquipRenderer;

	private int currHpState;

	protected List<int> HpState = new List<int>();

	protected List<int> HammerHpState = new List<int>();

	protected List<Sprite> E1HpStateSprite = new List<Sprite>();

	public SpriteRenderer DoorRenderer;

	private int currDoorHpState;

	protected List<int> DoorHpState = new List<int>();

	protected List<int> HammerDoorHpState = new List<int>();

	protected List<Sprite> DoorHpStateSprite = new List<Sprite>();

	public SpriteRenderer ArmorRenderer;

	protected int ArmorHpAbsorb;

	private int currArmorHpState;

	protected List<int> ArmorHpState = new List<int>();

	protected List<Sprite> ArmorHpStateSprite = new List<Sprite>();

	private Coroutine BrightCoroutine;

	private Coroutine DoorBrightCoroutine;

	private Vector3 animPos;

	protected Transform BaseTransform;

	public virtual int OnlineIdNum { get; } = 1;

	public Collider2D collider2d { get; private set; }

	public int SortOrder => Sorting.sortingOrder;

	public Transform AnimTranform => animator.transform;

	protected virtual Vector2 AttackDir { get; private set; } = new Vector2(-1f, 0f);

	protected virtual bool NeedDropHead { get; } = true;

	protected virtual bool CanEatDontEat { get; }

	protected virtual float inWaterDepth { get; } = 0.6f;

	public virtual Vector3 SplashScale { get; } = new Vector2(1.6f, 1.6f);

	public virtual Vector3 VaseScale { get; } = new Vector2(0.5f, 0.5f);

	public virtual Vector3 VaseOffset { get; } = new Vector2(0f, -0.3f);

	protected virtual float InWaterDistance { get; } = 0.5f;

	protected virtual float OutWaterDistance { get; } = 0.65f;

	protected virtual float NextGridAttackDis { get; } = 0.85f;

	public virtual bool CanEatByChomper { get; } = true;

	public virtual bool CanUseLadder { get; } = true;

	public virtual bool CanBlowBack { get; } = true;

	public virtual bool CanNoCollGet { get; }

	protected bool isIcetrap { get; private set; }

	protected abstract GameObject Prefab { get; }

	[SerializeField]
	public bool dontChangeState { get; protected set; }

	protected abstract float AnToSpeed { get; }

	protected abstract float DefSpeed { get; }

	protected abstract float attackValue { get; }

	public abstract int MaxHP { get; }

	protected virtual int CriticalHp { get; }

	protected bool IsCriticalState { get; private set; }

	protected float HpScale => Mathf.Pow(BodyScale, 2f);

	public float BodyScale
	{
		get
		{
			return Mathf.Abs(BaseTransform.localScale.x);
		}
		protected set
		{
			float num = value;
			if (LV.Instance.LvSpStates.Contains(LVSpState.SmallZombie))
			{
				num *= 0.5f;
			}
			BaseTransform.localScale = new Vector3(num, num);
			float num2 = -0.8f * num + 0.8f;
			TargetBaseY = 0f - num2;
			if (InWater)
			{
				BaseTransform.localPosition = new Vector3(0f, TargetBaseY - InWaterDepth);
			}
			else
			{
				BaseTransform.localPosition = new Vector3(0f, TargetBaseY);
			}
		}
	}

	public ZombieState State
	{
		get
		{
			return state;
		}
		set
		{
			if (state == ZombieState.Dead || ((IsCriticalState || dontChangeState) && value != ZombieState.Dead && LVManager.Instance.GameIsStart) || (GameManager.Instance.isClient && value == ZombieState.Dead))
			{
				return;
			}
			if (value == ZombieState.Dead)
			{
				dontChangeState = false;
				if (GameManager.Instance.isServer)
				{
					SynItem synItem = new SynItem();
					synItem.OnlineId = OnlineId;
					synItem.Type = SynItemType.Zombie;
					synItem.SynCode[0] = 1;
					synItem.SynCode[1] = 0;
					SocketServer.Instance.SendSynBag(synItem);
				}
			}
			state = value;
			ResetAnimationSpeed();
			CheckState();
		}
	}

	public float InWaterDepth
	{
		get
		{
			float num = inWaterDepth;
			float num2 = Mathf.Abs(BaseTransform.localScale.x);
			if (num2 < 1f)
			{
				num *= num2;
			}
			return num;
		}
	}

	public Grid CurrGrid
	{
		get
		{
			return currGrid;
		}
		protected set
		{
			if (value != currGrid && value != null)
			{
				currGrid = value;
				lastGrid = MapManager.Instance.GetNextGrid(currGrid, IsFacingLeft);
				NextGrid = MapManager.Instance.GetNextGrid(currGrid, !IsFacingLeft);
				ResetMoveTarget();
				CurrGridChangeEvent(lastGrid);
			}
		}
	}

	protected Grid NextGrid
	{
		get
		{
			Grid grid = nextGrid;
			if (grid != null)
			{
				float num = Mathf.Abs(nextGrid.Position.x - CurrGrid.Position.x);
				if (Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) > num)
				{
					grid = CurrGrid;
				}
			}
			return grid;
		}
		set
		{
			nextGrid = value;
		}
	}

	public MapBase CurrMap
	{
		get
		{
			return currMap;
		}
		private set
		{
			currMap = value;
		}
	}

	public int Hp
	{
		get
		{
			return hp;
		}
		protected set
		{
			hp = value;
			int num = -1;
			for (int i = 0; i < HpState.Count; i++)
			{
				if (hp <= HpState[i])
				{
					num = i;
				}
			}
			if (num != -1 && currHpState != num)
			{
				SpriteChangeEvent(E1HpStateSprite[num]);
				EquipRenderer.sprite = E1HpStateSprite[num];
				if (E1HpStateSprite[num] == null)
				{
					EquipRenderer.enabled = false;
				}
				currHpState = num;
			}
			if (hp == (int)((float)MaxHP * BaseTransform.localScale.x * BaseTransform.localScale.x))
			{
				if (E1HpStateSprite.Count != 0)
				{
					EquipRenderer.sprite = E1HpStateSprite[0];
					EquipRenderer.enabled = true;
				}
				else if (EquipRenderer != null)
				{
					EquipRenderer.enabled = false;
				}
			}
			if (hp <= 0)
			{
				if (!GameManager.Instance.isClient)
				{
					ReleaseAll();
					if (DoorHp > 0)
					{
						DoorHp = 0;
					}
					if (State != ZombieState.Dead)
					{
						State = ZombieState.Dead;
					}
				}
			}
			else if ((float)hp <= (float)CriticalHp * HpScale && !IsCriticalState)
			{
				DropHead();
				ReleaseAll();
				if (CriticalHpEnable)
				{
					StartCoroutine(CriticalState());
				}
				else
				{
					Hp = 0;
				}
			}
			CreatePanel.Instance.ChangeHp(null, this);
		}
	}

	protected int DoorHp
	{
		get
		{
			return doorHp;
		}
		set
		{
			if (ZombieManager.Instance.ZombieInvincible)
			{
				return;
			}
			if (value < 0)
			{
				Hp += value;
			}
			doorHp = value;
			int num = -1;
			for (int i = 0; i < DoorHpState.Count; i++)
			{
				if (doorHp <= DoorHpState[i])
				{
					num = i;
				}
			}
			if (num != -1 && currDoorHpState != num)
			{
				SpriteChangeEvent(DoorHpStateSprite[num]);
				DoorRenderer.sprite = DoorHpStateSprite[num];
				if (DoorHpStateSprite[num] == null)
				{
					DoorRenderer.enabled = false;
				}
				currDoorHpState = num;
			}
		}
	}

	protected int ArmorHp
	{
		get
		{
			return armorHp;
		}
		set
		{
			if (ZombieManager.Instance.ZombieInvincible)
			{
				return;
			}
			armorHp = value;
			int num = -1;
			for (int i = 0; i < ArmorHpState.Count; i++)
			{
				if (armorHp <= ArmorHpState[i])
				{
					num = i;
				}
			}
			if (num != -1 && currArmorHpState != num)
			{
				SpriteChangeEvent(ArmorHpStateSprite[num]);
				ArmorRenderer.sprite = ArmorHpStateSprite[num];
				if (ArmorHpStateSprite[num] == null)
				{
					ArmorRenderer.enabled = false;
				}
				currArmorHpState = num;
			}
		}
	}

	public bool InWater
	{
		get
		{
			return inWater;
		}
		protected set
		{
			inWater = value;
			anCanMove = true;
			if (WhiteWater != null)
			{
				WhiteWater.maskInteraction = SpriteMaskInteraction.None;
				WhiteWater.gameObject.SetActive(inWater);
			}
			WaterMask.enabled = InWater;
			InWaterChangeEvent();
		}
	}

	public int CurrLine
	{
		get
		{
			if (CurrGrid != null)
			{
				return CurrGrid.Point.y;
			}
			return -1;
		}
	}

	protected float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			if (value != 0f)
			{
				speed = ((speed >= 0f) ? Mathf.Abs(value) : (0f - Mathf.Abs(value)));
				ResetAnimationSpeed();
			}
		}
	}

	protected float SpeedRate => speedRate;

	protected float AddSpeed
	{
		get
		{
			return addSpeed;
		}
		set
		{
			addSpeed = value;
			ResetSpeedRate();
		}
	}

	public bool IsFacingLeft
	{
		get
		{
			return isFacingLeft;
		}
		set
		{
			if (isFacingLeft != value)
			{
				isFacingLeft = value;
				int num = 2;
				if (base.transform.localScale.x < 0f)
				{
					num = -2;
				}
				AttackDir = new Vector2(0f - AttackDir.x, AttackDir.y);
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
				if (State != ZombieState.Dead)
				{
					State = ZombieState.Walk;
				}
				base.transform.position += new Vector3(Shadow.transform.localPosition.x * (float)num, 0f);
				NextGrid = MapManager.Instance.GetNextGrid(currGrid, !IsFacingLeft);
				lastGrid = MapManager.Instance.GetNextGrid(currGrid, IsFacingLeft);
				ResetMoveTarget();
				ChangeFacingEvent();
			}
		}
	}

	public int FrozenLevel
	{
		get
		{
			return frozenLevel;
		}
		private set
		{
			frozenLevel = value;
			isFrozen = frozenLevel > 0;
			if (frozenLevel > 10)
			{
				frozenLevel = 10;
			}
			else if (frozenLevel < 0)
			{
				frozenLevel = 0;
			}
			if (frozenCoroutine != null)
			{
				StopCoroutine(frozenCoroutine);
			}
			float num = 20f;
			if (CurrMap != null)
			{
				num = CurrMap.CurrTempt;
			}
			float num2 = (num + 50f) / 70f;
			if (num2 > 2f)
			{
				num2 = 2f;
			}
			if (num2 < 0.2f)
			{
				num2 = 0.2f;
			}
			float num3 = 4.5f - (float)FrozenLevel * 0.4f * num2;
			if (FrozenLevel > 0)
			{
				if (num3 <= 0f)
				{
					UnFrozenOne();
				}
				else
				{
					frozenCoroutine = StartCoroutine(DoFuncWait(UnFrozenOne, num3));
				}
			}
			ResetSpeedRate();
			ResetColor();
		}
	}

	private void ReleaseAll()
	{
		DeadStateGetStaticBuff();
		if (isButter)
		{
			UnButter();
		}
		if (isIcetrap)
		{
			UnIce();
		}
		if (isDizzy)
		{
			UnDizzy();
		}
	}

	private IEnumerator CriticalState()
	{
		DoorHp = 0;
		IsCriticalState = true;
		int i = CriticalHp / 30;
		while (Hp > 0)
		{
			yield return new WaitForSeconds(0.1f);
			Hp -= i;
		}
	}

	public bool ContainLine(int line)
	{
		if (line == CurrLine)
		{
			return true;
		}
		return InMoreLines.Contains(line);
	}

	public void ResetSpeedRate()
	{
		float num = (float)(-FrozenLevel) * 0.05f;
		float num2 = 0f;
		if (GobalEffManager.Instance.IsHaveThisEff(GobalEffect.RainCool))
		{
			num2 = 0.1f;
		}
		float num3 = 1f + num + AddSpeed + num2;
		if (num3 <= 0f)
		{
			num3 = 0.1f;
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.QuickZombie))
		{
			num3 *= 2f;
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.SmallZombie))
		{
			float num4 = -1f * BodyScale + 1f;
			num3 *= 1f + num4;
		}
		speedRate = num3 * customSpeed;
		ResetAnimationSpeed();
	}

	protected void ResetMoveTarget()
	{
		if (nextGrid == null)
		{
			if (IsFacingLeft)
			{
				MoveTarget = new Vector2(-50f, currGrid.Position.y);
			}
			else
			{
				MoveTarget = new Vector2(50f, currGrid.Position.y);
			}
		}
		else if (currGrid.isSlope)
		{
			if (nextGrid.isSlope)
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, (currGrid.Position.y + nextGrid.Position.y) / 2f);
			}
			else
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
			}
		}
		else if (nextGrid.isSlope)
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, currGrid.Position.y);
		}
		else
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
		}
		if (IsFacingLeft)
		{
			MoveTarget -= new Vector2(0.05f, 0f);
		}
		else
		{
			MoveTarget += new Vector2(0.05f, 0f);
		}
	}

	protected void ResetAnimationSpeed()
	{
		if (IsOVer)
		{
			return;
		}
		if (State == ZombieState.Dead)
		{
			animator.speed = SpeedRate;
		}
		else if (IsStaticState())
		{
			animator.speed = 0f;
		}
		else if (State == ZombieState.Walk)
		{
			if (SpeedRate == 0f)
			{
				animator.speed = SpeedRate;
			}
			else
			{
				animator.speed = AnToSpeed / Mathf.Abs(Speed) * SpeedRate;
			}
		}
		else
		{
			animator.speed = SpeedRate;
		}
		AnimatorSpeedChange(animator.speed);
	}

	protected void ResetColor()
	{
		if (FrozenLevel > 0)
		{
			SetAllColor(new Color(1f - (float)FrozenLevel * 0.055f, 1f - (float)FrozenLevel * 0.05f, 1f - (float)FrozenLevel * 0.005f));
		}
		else if (needHypnoPurple)
		{
			SetAllColor(new Color(0.88f, 0.39f, 1f));
		}
		else
		{
			SetAllColor(new Color(1f, 1f, 1f));
		}
	}

	protected void SetAnimatorChange(int Change)
	{
		if (Change / 10 == 3 || State != ZombieState.Dead)
		{
			animator.SetInteger("Change", Change);
		}
	}

	protected void SetAllColor(Color color)
	{
		Color color2 = Color.white;
		if (WhiteWater != null)
		{
			color2 = new Color(WhiteWater.color.r, WhiteWater.color.g, WhiteWater.color.b, color.a);
		}
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].color = new Color(color.r, color.g, color.b, sprites[i].color.a);
		}
		if (WhiteWater != null)
		{
			WhiteWater.color = color2;
		}
	}

	protected void SetAlpha(float a)
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, a);
		}
	}

	protected void SetAllBrightness(float color, SpriteRenderer noThis = null)
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			if (sprites[i] != noThis)
			{
				sprites[i].material.SetFloat("_Brightness", color);
			}
		}
	}

	public void InitForAlmanac(Vector2 pos)
	{
		IsOVer = true;
		Init(0, 100, default);
		StartIdel();
		base.transform.position = pos;
		WaterMask.enabled = false;
		Shadow.sortingOrder = 0;
		AlmanacInitZombie();
	}

	public void CreateInit(bool inGrid, Grid grid, bool isRat, int Spcode = 0)
	{
		IsOVer = true;
		Init(0, 100, default);
		if (grid != null)
		{
			base.transform.position = grid.Position;
		}
		if (inGrid)
		{
			Sorting.sortingOrder = 2011;
			SetAlpha(0.7f);
		}
		else
		{
			Sorting.sortingOrder = 2012;
			if (Spcode != 1)
			{
				SetAlpha(1f);
			}
		}
		if (Spcode == 1)
		{
			Sorting.sortingOrder = grid.Point.y * 200 + 191;
		}
		Shadow.enabled = false;
		collider2d.enabled = false;
		anCanMove = false;
		if (isRat)
		{
			RatThis(synClient: true);
		}
		animator.speed = 0f;
		AnimatorSpeedChange(animator.speed);
		CreateInitZombie();
	}

	public void SetSortingOrder(int lineNum, int orderNum)
	{
		SetSortingOrder(FixedInfo.GetBaseSort(lineNum) + 100 + orderNum);
	}

	public void SetSortingOrder(int order)
	{
		Sorting.sortingOrder = order;
		Icetrap.sortingOrder = Sorting.sortingOrder + 1;
		WaterMask.frontSortingOrder = Sorting.sortingOrder;
		WaterMask.backSortingOrder = Sorting.sortingOrder - 1;
	}

	public void SetInsideVisible()
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		}
	}

	public void UpdateForCreate(Grid grid)
	{
		base.transform.position = grid.Position;
	}

	public void Init(int lineNum, int orderNum, Vector2 pos, bool IsFirst = true)
	{
		if (animator == null)
		{
			animator = base.transform.Find("Animation").GetComponent<Animator>();
			Sorting = animator.GetComponent<SortingGroup>();
			SpriteRenderer[] componentsInChildren = animator.transform.GetComponentsInChildren<SpriteRenderer>();
			sprites.AddRange(componentsInChildren);
			collider2d = GetComponent<Collider2D>();
			Icetrap = base.transform.Find("icetrap").GetComponent<SpriteRenderer>();
			Shadow = base.transform.Find("Shadow").GetComponent<SpriteRenderer>();
			List<Transform> list = new List<Transform>();
			foreach (Transform item in base.transform)
			{
				list.Add(item);
			}
			BaseTransform = new GameObject("Base").GetComponent<Transform>();
			BaseTransform.SetParent(base.transform);
			BaseTransform.localPosition = Vector3.zero;
			foreach (Transform item2 in list)
			{
				item2.SetParent(BaseTransform);
			}
			animPos = animator.transform.localPosition;
		}
		if (sprites[0].maskInteraction != SpriteMaskInteraction.VisibleOutsideMask)
		{
			for (int i = 0; i < sprites.Count; i++)
			{
				sprites[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
			}
		}
		isDropArm = false;
		isDropHead = false;
		if (MidArmRenderer != null)
		{
			MidArmRenderer.material.SetInt("_OpenDisplay", 1);
		}
		if (LowArmRenderer != null)
		{
			LowArmRenderer.material.SetInt("_OpenDisplay", 1);
		}
		if (normalArm != null)
		{
			UpArmRenderer.sprite = normalArm;
		}
		for (int j = 0; j < headSprites.Count; j++)
		{
			headSprites[j].gameObject.SetActive(value: true);
		}
		returnFirstClick = false;
		SpawnPos = pos;
		base.transform.position = pos;
		CurrMap = MapManager.Instance.GetNearestMap(base.transform.position);
		CurrGrid = MapManager.Instance.GetGridByWorldPos(CurrMap, pos, lineNum);
		IsFacingLeft = true;
		onlyBoomHurt = false;
		anCanMove = true;
		dontChangeState = false;
		dontCgStCanInwater = false;
		collider2d.enabled = true;
		InWater = false;
		going = false;
		FrozenLevel = 0;
		BanAllInmobilize = false;
		if (butter != null)
		{
			butter.enabled = false;
		}
		if (normalHead != null)
		{
			HeadRenderer.sprite = normalHead;
			JawRenderer.enabled = true;
		}
		invincible = false;
		IsCriticalState = false;
		isHypno = false;
		hypnoAttackTarget = null;
		needHypnoPurple = false;
		changeLineCoroutine = null;
		SetSortingOrder(lineNum, orderNum);
		WaterMask.enabled = false;
		Icetrap.enabled = false;
		Shadow.enabled = true;
		Shadow.sortingOrder = FixedInfo.GetBaseSort(CurrLine) + 10;
		Shadow.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		SetAllBrightness(1f);
		if (LV.Instance.LvSpStates.Contains(LVSpState.InvisibleZombie))
		{
			SetAlpha(0f);
			Shadow.color = new Color(1f, 1f, 1f, 0f);
		}
		else
		{
			SetAlpha(1f);
			Shadow.color = new Color(1f, 1f, 1f, 1f);
		}
		SetAllColor(new Color(1f, 1f, 1f));
		if (IsFirst && !GameManager.Instance.isClient)
		{
			NeedSynInit();
		}
		if (orderNum != 100)
		{
			IsOVer = false;
		}
		state = ZombieState.Walk;
		UnIce();
		UnFrozen(10);
		UnButter();
		UnDizzy();
		customAttack = 1f;
		customSpeed = 1f;
		AddSpeed = 0f;
		HpState.Clear();
		HammerHpState.Clear();
		E1HpStateSprite.Clear();
		DoorHpState.Clear();
		HammerDoorHpState.Clear();
		DoorHpStateSprite.Clear();
		ArmorHpState.Clear();
		ArmorHpStateSprite.Clear();
		TargetBaseY = 0f;
		BodyScale = 1f;
		BaseTransform.localPosition = Vector3.zero;
		animator.transform.localPosition = animPos;
		InitZombieHpState();
		CriticalHpEnable = true;
		currHpState = 0;
		if (HpScale != 1f)
		{
			for (int k = 0; k < HpState.Count; k++)
			{
				HpState[k] = (int)((float)HpState[k] * HpScale);
			}
			for (int l = 0; l < DoorHpState.Count; l++)
			{
				DoorHpState[l] = (int)((float)DoorHpState[l] * HpScale);
			}
			for (int m = 0; m < ArmorHpState.Count; m++)
			{
				ArmorHpState[m] = (int)((float)ArmorHpState[m] * HpScale);
			}
		}
		Hp = (int)((float)MaxHP * HpScale);
		if (HpState.Count > 0 && HammerHpState.Count > 0 && HammerHpState[0] != HpState[0])
		{
			HammerHpState.Insert(0, HpState[0]);
		}
		if (DoorHpState.Count > 0 && HammerDoorHpState.Count > 0 && HammerDoorHpState[0] != DoorHpState[0])
		{
			HammerDoorHpState.Insert(0, DoorHpState[0]);
		}
		doorHp = 0;
		if (DoorHpState.Count > 0)
		{
			DoorHp = DoorHpState[0];
			DoorRenderer.sprite = DoorHpStateSprite[0];
			DoorRenderer.enabled = true;
		}
		else if (DoorRenderer != null)
		{
			DoorRenderer.enabled = false;
		}
		armorHp = 0;
		if (ArmorHpState.Count > 0)
		{
			ArmorHp = ArmorHpState[0];
			ArmorRenderer.sprite = ArmorHpStateSprite[0];
			ArmorRenderer.enabled = true;
		}
		else if (ArmorRenderer != null)
		{
			ArmorRenderer.enabled = false;
		}
		CheckState();
		Speed = DefSpeed;
		if (CurrGrid != null && CurrMap.IsWaterShow && CurrGrid.isNoIceWater && Vector2.Distance(base.transform.position, CurrGrid.Position) < 2f)
		{
			DirctInWater();
		}
	}

	public bool GetDead()
	{
		if (Hp <= CriticalHp)
		{
			return true;
		}
		if (State == ZombieState.Dead)
		{
			return true;
		}
		return false;
	}

	public abstract void InitZombieHpState();

	protected virtual void AlmanacInitZombie()
	{
	}

	protected virtual void CreateInitZombie()
	{
	}

	protected virtual void NeedSynInit()
	{
	}

	protected virtual void AnimatorSpeedChange(float speed)
	{
	}

	public virtual void ServerInitInfo()
	{
	}

	public virtual void SummonInit()
	{
	}

	public virtual void ZombieOnDead(bool dropItem)
	{
	}

	private void Update()
	{
		UpdateThis();
		if (IsOVer || isIcetrap || isButter || isDizzy)
		{
			return;
		}
		switch (State)
		{
		case ZombieState.Walk:
			Move();
			if (!IsFacingLeft && base.transform.position.x > 8.6f)
			{
				DirectDead(canDropItem: false, 0f);
			}
			if (LV.Instance.CurrLVType == LVType.PvP && MapManager.Instance.GetCurrMap(base.transform.position) == null)
			{
				DirectDead(canDropItem: false, 0f);
			}
			break;
		case ZombieState.Attack:
			if (Hp > 0)
			{
				CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
			}
			EatOutCheck();
			if (CanUseLadder && !dontChangeState && AttackGrid != null && ((IsFacingLeft && AttackGrid.HaveRightLadder) || (!IsFacingLeft && AttackGrid.HaveLeftLadder)))
			{
				StartCoroutine(UpLadder());
			}
			break;
		}
	}

	protected virtual void CheckState()
	{
		switch (State)
		{
		case ZombieState.Idel:
			animator.Play("idle1", 0, Random.Range(0f, 1f));
			break;
		case ZombieState.Walk:
			animator.SetInteger("Change", 11);
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			collider2d.enabled = false;
			animator.SetInteger("Change", 31);
			break;
		}
	}

	private void Move()
	{
		if (Speed == 0f || SpeedRate == 0f || CurrGrid == null)
		{
			return;
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(CurrMap, base.transform.position, CurrLine);
		if (gridByWorldPos == null)
		{
			return;
		}
		CurrGrid = gridByWorldPos;
		if (needInWater && (!dontChangeState || (dontCgStCanInwater && dontChangeState)))
		{
			bool flag = false;
			bool flag2 = false;
			if (!going && (NextGrid == null || !NextGrid.isWaterGrid || NextGrid.IsIce))
			{
				if (IsFacingLeft && CurrGrid.Position.x - base.transform.position.x > OutWaterDistance)
				{
					flag = true;
				}
				else if (!IsFacingLeft && base.transform.position.x - CurrGrid.Position.x > OutWaterDistance)
				{
					flag = true;
				}
			}
			if (!flag && !going && CurrGrid.isWaterGrid && !CurrGrid.IsIce)
			{
				if (IsFacingLeft && base.transform.position.x - currGrid.Position.x < InWaterDistance)
				{
					if (InWaterDistance > 0f)
					{
						flag2 = true;
					}
					else if (NextGrid != null && NextGrid.isWaterGrid && !NextGrid.IsIce)
					{
						flag2 = true;
					}
				}
				else if (!IsFacingLeft && currGrid.Position.x - base.transform.position.x < InWaterDistance)
				{
					if (InWaterDistance > 0f)
					{
						flag2 = true;
					}
					else if (NextGrid != null && NextGrid.isWaterGrid && !NextGrid.IsIce)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					if (lastGrid != null && lastGrid.isWaterGrid && !lastGrid.IsIce && ((IsFacingLeft && base.transform.position.x > currGrid.Position.x) || (!IsFacingLeft && base.transform.position.x < currGrid.Position.x)))
					{
						flag2 = true;
					}
					else if (NextGrid != null && NextGrid.isWaterGrid && !NextGrid.IsIce && ((IsFacingLeft && base.transform.position.x < currGrid.Position.x) || (!IsFacingLeft && base.transform.position.x > currGrid.Position.x)))
					{
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				if (InWater && (NextGrid == null || !NextGrid.isOccupied))
				{
					StartCoroutine(MoveOutWater());
				}
			}
			else if (flag2 && !InWater)
			{
				StartCoroutine(MoveInWater());
			}
		}
		if (!going)
		{
			EatEnterCheck();
		}
		if (State == ZombieState.Attack && CanUseLadder && !dontChangeState && AttackGrid != null && ((IsFacingLeft && AttackGrid.HaveRightLadder) || (!IsFacingLeft && AttackGrid.HaveLeftLadder)))
		{
			StartCoroutine(UpLadder());
		}
		if (State != ZombieState.Walk)
		{
			return;
		}
		if (!dontChangeState && NextGrid != null && NextGrid.needChangeLine && changeLineCoroutine == null && Mathf.Abs(NextGrid.Position.x - base.transform.position.x) < 1.45f && needChangeLine)
		{
			ChangeLine();
		}
		if (IsFacingLeft && base.transform.position.x < CurrMap.EndLine)
		{
			if (!isHypno)
			{
				if (!GameManager.Instance.isClient && !IsCriticalState)
				{
					if (LV.Instance.CurrLVType == LVType.IZombie)
					{
						CleanerDead();
					}
					else
					{
						LVManager.Instance.ZombieGameOver(base.transform.position);
					}
				}
			}
			else if (IsFacingLeft)
			{
				GoBack();
			}
		}
		else
		{
			if (ZombieManager.Instance.ZombieDontMove)
			{
				return;
			}
			float num = 0f;
			if (SkyManager.Instance.WindScale > 0)
			{
				num = ((!SkyManager.Instance.WindTowardRight) ? (num + (float)SkyManager.Instance.WindScale * 0.01f * Time.deltaTime) : (num - (float)SkyManager.Instance.WindScale * 0.01f * Time.deltaTime));
				if (CurrMap.IsFacingLeft)
				{
					num *= -1f;
				}
			}
			if (!CanBlowBack)
			{
				num = 0f;
			}
			if (anCanMove)
			{
				float num2 = 1.33f * Time.deltaTime / (Speed / SpeedRate);
				num2 += num;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				base.transform.position = Vector2.MoveTowards(base.transform.position, MoveTarget, num2);
			}
			else if (SkyManager.Instance.WindScale > 0)
			{
				base.transform.position = Vector2.MoveTowards(base.transform.position, MoveTarget, num);
			}
		}
	}

	protected virtual void EatEnterCheck()
	{
		if (isHypno)
		{
			if (NextGrid != null && NextGrid.CurrPlantBase != null && (NextGrid.CurrPlantBase.ZombieCanEat || (!NextGrid.CurrPlantBase.ZombieCanEat && CanEatDontEat)) && NextGrid.Cage == null && NextGrid.CurrPlantBase.isHypno && Mathf.Abs(base.transform.position.x - NextGrid.Position.x) < NextGridAttackDis * BodyScale && Mathf.Abs(base.transform.position.y - NextGrid.Position.y) < 0.2f)
			{
				AttackGrid = NextGrid;
				State = ZombieState.Attack;
				return;
			}
			if (CurrGrid.CurrPlantBase != null && (CurrGrid.CurrPlantBase.ZombieCanEat || (!CurrGrid.CurrPlantBase.ZombieCanEat && CanEatDontEat)) && CurrGrid.Cage == null && CurrGrid.CurrPlantBase.isHypno && ((IsFacingLeft && base.transform.position.x - CurrGrid.Position.x >= 0f) || (!IsFacingLeft && base.transform.position.x - CurrGrid.Position.x <= 0f)) && Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) < 0.6f && Mathf.Abs(base.transform.position.y - CurrGrid.Position.y) < 0.2f)
			{
				AttackGrid = CurrGrid;
				State = ZombieState.Attack;
				return;
			}
			hypnoAttackTarget = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, IsFacingLeft, !isHypno);
			if (hypnoAttackTarget != null && Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) <= 0.4f)
			{
				AttackGrid = null;
				State = ZombieState.Attack;
			}
			else
			{
				hypnoAttackTarget = null;
			}
		}
		else if (NextGrid != null && NextGrid.CurrPlantBase != null && (NextGrid.CurrPlantBase.ZombieCanEat || (!NextGrid.CurrPlantBase.ZombieCanEat && CanEatDontEat)) && NextGrid.Cage == null && !NextGrid.CurrPlantBase.isHypno && Mathf.Abs(base.transform.position.x - NextGrid.Position.x) < NextGridAttackDis * BodyScale && Mathf.Abs(base.transform.position.y - NextGrid.Position.y) < 0.2f)
		{
			AttackGrid = NextGrid;
			State = ZombieState.Attack;
		}
		else if (CurrGrid.CurrPlantBase != null && (CurrGrid.CurrPlantBase.ZombieCanEat || (!CurrGrid.CurrPlantBase.ZombieCanEat && CanEatDontEat)) && CurrGrid.Cage == null && !CurrGrid.CurrPlantBase.isHypno && ((IsFacingLeft && base.transform.position.x - CurrGrid.Position.x >= 0f) || (!IsFacingLeft && base.transform.position.x - CurrGrid.Position.x <= 0f)) && Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) < 0.6f && Mathf.Abs(base.transform.position.y - CurrGrid.Position.y) < 0.2f)
		{
			AttackGrid = CurrGrid;
			State = ZombieState.Attack;
		}
		else
		{
			hypnoAttackTarget = ZombieManager.Instance.GetZombieByLineMinDistance(currGrid.Point.y, base.transform.position, IsFacingLeft, !isHypno);
			if (hypnoAttackTarget != null && Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) <= 0.4f)
			{
				AttackGrid = null;
				State = ZombieState.Attack;
			}
			else
			{
				hypnoAttackTarget = null;
			}
		}
	}

	protected virtual void EatOutCheck()
	{
		if (isHypno)
		{
			if (hypnoAttackTarget != null)
			{
				if (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.collider2d.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
				{
					State = ZombieState.Walk;
					hypnoAttackTarget = null;
				}
			}
			else if (AttackGrid != null && (AttackGrid.CurrPlantBase == null || AttackGrid.Cage != null || (AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.ZombieCanEat) || (AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.isHypno)))
			{
				State = ZombieState.Walk;
				AttackGrid = null;
			}
		}
		else if (hypnoAttackTarget != null)
		{
			if (hypnoAttackTarget.Hp <= 0 || !hypnoAttackTarget.collider2d.enabled || Vector2.Distance(hypnoAttackTarget.transform.position, base.transform.position) > 0.4f)
			{
				State = ZombieState.Walk;
				hypnoAttackTarget = null;
			}
		}
		else if (AttackGrid != null && (AttackGrid.CurrPlantBase == null || AttackGrid.Cage != null || (AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.ZombieCanEat) || (AttackGrid.CurrPlantBase != null && AttackGrid.CurrPlantBase.isHypno)))
		{
			State = ZombieState.Walk;
			AttackGrid = null;
		}
		if (State == ZombieState.Attack && AttackGrid != null && ((AttackGrid == NextGrid && Mathf.Abs(base.transform.position.x - NextGrid.Position.x) > 0.85f) || (AttackGrid == CurrGrid && NextGrid != CurrGrid && Mathf.Abs(base.transform.position.x - CurrGrid.Position.x) > 0.6f)))
		{
			State = ZombieState.Walk;
			AttackGrid = null;
		}
	}

	private IEnumerator MoveInWater()
	{
		going = true;
		Shadow.enabled = false;
		float Y = 0f - (TargetBaseY + InWaterDepth / 2f);
		while (BaseTransform.localPosition.y > Y)
		{
			yield return null;
			BaseTransform.Translate(new Vector2(0f, -5f) * Time.deltaTime);
		}
		WaterMask.enabled = true;
		while (BaseTransform.localPosition.y > TargetBaseY - InWaterDepth)
		{
			yield return null;
			BaseTransform.Translate(new Vector2(0f, -5f) * Time.deltaTime);
		}
		BaseTransform.localPosition = new Vector3(BaseTransform.localPosition.x, TargetBaseY - InWaterDepth);
		InWater = true;
		yield return new WaitForFixedUpdate();
		Splash component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>();
		component.CreateInit(new Vector2(base.transform.position.x, CurrGrid.Position.y), CurrGrid.Point.y);
		component.transform.localScale = SplashScale;
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
		going = false;
	}

	public void DirctInWater()
	{
		if (needInWater && !InWater)
		{
			Shadow.enabled = false;
			WaterMask.enabled = true;
			InWater = true;
			going = false;
			BaseTransform.transform.localPosition = new Vector3(BaseTransform.localPosition.x, TargetBaseY - InWaterDepth);
		}
	}

	public void DirctOutWater()
	{
		Shadow.enabled = true;
		WaterMask.enabled = false;
		InWater = false;
		going = false;
	}

	private IEnumerator MoveOutWater()
	{
		going = true;
		WaterMask.enabled = false;
		if (WhiteWater != null)
		{
			WhiteWater.gameObject.SetActive(value: false);
		}
		while (BaseTransform.localPosition.y < TargetBaseY)
		{
			yield return null;
			BaseTransform.Translate(new Vector2(0f, 5f) * Time.deltaTime);
		}
		BaseTransform.localPosition = new Vector3(BaseTransform.localPosition.x, TargetBaseY);
		Shadow.enabled = true;
		InWater = false;
		going = false;
	}

	private IEnumerator UpLadder()
	{
		State = ZombieState.Walk;
		dontChangeState = true;
		going = true;
		Shadow.enabled = false;
		if (InWater)
		{
			InWater = false;
		}
		Vector2 dir = (AttackGrid.Position + new Vector2(0f, 1.1f) - new Vector2(base.transform.position.x, base.transform.position.y)).normalized;
		while (base.transform.position.y < CurrGrid.Position.y + 1.1f)
		{
			yield return null;
			base.transform.Translate(2f * SpeedRate * Time.deltaTime * dir);
		}
		while (base.transform.position.y > CurrGrid.Position.y)
		{
			yield return null;
			base.transform.Translate(Vector2.down * Time.deltaTime * 2f * SpeedRate);
		}
		if (!InWater)
		{
			Shadow.enabled = true;
		}
		going = false;
		dontChangeState = false;
		ResetAnimationSpeed();
	}

	protected void GoBack()
	{
		IsFacingLeft = !IsFacingLeft;
	}

	public void AnimAttack(bool isFlat = false)
	{
		if (!isDropHead)
		{
			Attack(isFlat);
		}
	}

	public int GetAttack()
	{
		return (int)(attackValue * customAttack);
	}

	protected void Attack(bool isFlat = false)
	{
		if (CurrGrid == null)
		{
			return;
		}
		if (hypnoAttackTarget != null)
		{
			OnAttackEvent();
			AttackEvent(null, hypnoAttackTarget);
			hypnoAttackTarget.Hurt(GetAttack(), AttackDir, isHard: true, HitSound: false);
			if (!isFlat)
			{
				MyTool.RandomOne(new List<UnityAction>
				{
					() =>
					{
						AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant1, base.transform.position);
					},
					() =>
					{
						AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant2, base.transform.position);
					},
					() =>
					{
						AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant3, base.transform.position);
					}
				});
			}
		}
		else if (AttackGrid != null && AttackGrid.CurrPlantBase != null)
		{
			AttackEvent(AttackGrid.CurrPlantBase, null);
			AttackGrid.CurrPlantBase.Hurt(GetAttack(), AttackDir, this, isFlat);
			OnAttackEvent();
		}
	}

	public void Hurt(int attackValue, Vector2 dirction, bool isHard = true, bool HitSound = true)
	{
		if (invincible)
		{
			return;
		}
		if (isDizzy)
		{
			attackValue = (int)(1.5f * (float)attackValue);
		}
		attackValue = HandleHurt(attackValue, dirction);
		if (attackValue < 0)
		{
			attackValue = 0;
		}
		if (ZombieManager.Instance.ZombieInvincible)
		{
			attackValue = 0;
		}
		if (DoorHp > 0 && ((dirction.x > 0f && IsFacingLeft) || (dirction.x < 0f && !IsFacingLeft)))
		{
			if (attackValue > 0 || ZombieManager.Instance.ZombieInvincible)
			{
				if (DoorBrightCoroutine != null)
				{
					StopCoroutine(DoorBrightCoroutine);
				}
				DoorBrightCoroutine = StartCoroutine(DoorBrightnessEffect(1.5f, null));
			}
			DoorHp -= attackValue;
			if (attackValue > 0)
			{
				DoorHpReduceEvent();
			}
			return;
		}
		if (ArmorHp > 0)
		{
			int num = ArmorHpAbsorb;
			attackValue -= ArmorHpAbsorb;
			if (attackValue < 0)
			{
				num += attackValue;
				attackValue = 0;
			}
			ArmorHp -= num;
		}
		if (attackValue > 0 || ZombieManager.Instance.ZombieInvincible)
		{
			if (BrightCoroutine != null)
			{
				StopCoroutine(BrightCoroutine);
			}
			BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
		}
		Hp -= attackValue;
		if (attackValue >= 0)
		{
			HpReduceEvent(isHard, HitSound);
		}
	}

	public void BoomHurt(int attackValue, bool HitSound = false)
	{
		if (invincible)
		{
			return;
		}
		if (onlyBoomHurt)
		{
			Hurt(attackValue, Vector2.zero, isHard: true, HitSound: false);
			return;
		}
		int num = Hp;
		if (isDizzy)
		{
			attackValue = (int)(1.5f * (float)attackValue);
		}
		attackValue = HandleBoomHurt(attackValue);
		if (ZombieManager.Instance.ZombieInvincible)
		{
			attackValue = 0;
		}
		if (attackValue >= num && !ZombieManager.Instance.ZombieInvincible)
		{
			if (GameManager.Instance.isServer && GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
			if (!GameManager.Instance.isClient)
			{
				BoomDeadEvent();
				if (InWater || num <= 0)
				{
					Dead(canDropItem: true, 0f);
				}
				else
				{
					PlaceCharred();
				}
				num = 0;
				state = ZombieState.Dead;
			}
		}
		else if (attackValue >= 0)
		{
			Hp -= attackValue;
			HpReduceEvent(isHard: false, HitSound);
			if (BrightCoroutine != null)
			{
				StopCoroutine(BrightCoroutine);
			}
			BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
		}
	}

	protected virtual void BoomDeadEvent()
	{
	}

	public void HammerHurt(int NAttackValue, Vector2 dirction)
	{
		if (invincible)
		{
			return;
		}
		NAttackValue = HandleHurt(NAttackValue, dirction);
		if (ZombieManager.Instance.ZombieInvincible)
		{
			NAttackValue = 0;
		}
		if (DoorHp > 0 && ((dirction.x > 0f && IsFacingLeft) || (dirction.x < 0f && !IsFacingLeft)))
		{
			int num = 0;
			for (int i = 0; i < HammerDoorHpState.Count; i++)
			{
				if (DoorHp <= HammerDoorHpState[i])
				{
					num = ((HammerDoorHpState.Count <= i + 1) ? HammerDoorHpState[i] : (HammerDoorHpState[i] - HammerDoorHpState[i + 1]));
				}
			}
			if (num < 1)
			{
				num = 500;
			}
			if (num > 0 || ZombieManager.Instance.ZombieInvincible)
			{
				if (DoorBrightCoroutine != null)
				{
					StopCoroutine(DoorBrightCoroutine);
				}
				DoorBrightCoroutine = StartCoroutine(DoorBrightnessEffect(1.5f, null));
			}
			DoorHp -= num;
			if (num > 0)
			{
				DoorHpReduceEvent();
			}
			return;
		}
		int num2 = 0;
		for (int j = 0; j < HammerHpState.Count; j++)
		{
			if (Hp <= HammerHpState[j])
			{
				num2 = ((HammerHpState.Count <= j + 1) ? HammerHpState[j] : (HammerHpState[j] - HammerHpState[j + 1]));
			}
		}
		if (num2 < 1)
		{
			num2 = 500;
		}
		if (ArmorHp > 0)
		{
			int num3 = ArmorHpAbsorb;
			num2 -= ArmorHpAbsorb;
			if (num2 < 0)
			{
				num3 += num2;
				num2 = 0;
			}
			ArmorHp -= num3;
		}
		if (num2 > 0 || ZombieManager.Instance.ZombieInvincible)
		{
			if (BrightCoroutine != null)
			{
				StopCoroutine(BrightCoroutine);
			}
			BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
		}
		Hp -= num2;
		if (num2 > 0)
		{
			HpReduceEvent(isHard: true, HitSound: true);
		}
	}

	protected virtual void PlaceCharred()
	{
		Dead();
		animator.speed = 0f;
		AnimatorSpeedChange(animator.speed);
		SetAllColor(Color.black);
	}

	public void CleanerDead(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			if (GameManager.Instance.isServer && State != ZombieState.Dead && !synClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 3;
				SocketServer.Instance.SendSynBag(synItem);
			}
			Object.Instantiate(GameManager.Instance.GameConf.ImitaterParticle).transform.position = base.transform.position;
			Hp = 0;
			Dead(canDropItem: true, 0f);
		}
	}

	public void DirectDead(bool canDropItem = true, float delay = 1f, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			if (GameManager.Instance.isServer && !synClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 1;
				synItem.SynCode[1] = 2;
				synItem.SynCode[2] = (canDropItem ? 1 : 0);
				synItem.Twofloat = new Vector2(delay, 0f);
				SocketServer.Instance.SendSynBag(synItem);
			}
			Dead(canDropItem, delay);
		}
	}

	private void Dead(bool canDropItem = true, float delay = 1f)
	{
		CreatePanel.Instance.ZombieDeadEvent(this);
		hp = 0;
		collider2d.enabled = false;
		ZombieOnDead(canDropItem);
		if (canDropItem && LV.Instance.CurrLVType != LVType.IZombie)
		{
			LvItemManager.Instance.DropCoin(base.transform.position);
		}
		StopAllCoroutines();
		ZombieManager.Instance.RemoveZombie(this);
		if (delay == 0f)
		{
			PoolManager.Instance.PushObj(Prefab, base.gameObject);
		}
		else
		{
			StartCoroutine(DeadWait(delay));
		}
	}

	private IEnumerator DeadWait(float delay)
	{
		yield return new WaitForSeconds(delay);
		float currAlpha = sprites[0].color.a;
		while (currAlpha > 0f)
		{
			yield return null;
			currAlpha -= 8f * Time.deltaTime;
			SetAlpha(currAlpha);
		}
		PoolManager.Instance.PushObj(Prefab, base.gameObject);
	}

	protected IEnumerator BrightnessEffect(float targetBright, UnityAction fun)
	{
		float currBright = sprites[0].material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return null;
			currBright += 5f * Time.deltaTime;
			SetAllBrightness(currBright, DoorRenderer);
		}
		while (1f < targetBright)
		{
			yield return null;
			targetBright -= 5f * Time.deltaTime;
			SetAllBrightness(targetBright, DoorRenderer);
		}
		SetAllBrightness(1f, DoorRenderer);
		fun?.Invoke();
		BrightCoroutine = null;
	}

	protected IEnumerator DoorBrightnessEffect(float targetBright, UnityAction fun)
	{
		float currBright = DoorRenderer.material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return null;
			currBright += 5f * Time.deltaTime;
			DoorRenderer.material.SetFloat("_Brightness", currBright);
		}
		while (1f < targetBright)
		{
			yield return null;
			targetBright -= 5f * Time.deltaTime;
			DoorRenderer.material.SetFloat("_Brightness", targetBright);
		}
		DoorRenderer.material.SetFloat("_Brightness", 1f);
		fun?.Invoke();
		DoorBrightCoroutine = null;
	}

	protected void ServerSendSyn(int code1, int code2 = 0, int code3 = 0)
	{
		ServerSendSyn(code1, Vector2.zero, code2, code3);
	}

	protected void ServerSendSyn(int code1, Vector2 pos, int code2 = 0, int code3 = 0)
	{
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Zombie;
			synItem.Twofloat = pos;
			synItem.SynCode[0] = 2;
			synItem.SynCode[1] = code1;
			synItem.SynCode[2] = code2;
			synItem.SynCode[3] = code3;
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	protected virtual void OnlineSyn(SynItem syn)
	{
	}

	public void OnlineSynZombie(SynItem syn)
	{
		if (syn.SynCode[0] == 0)
		{
			if (syn.SynCode[1] == 0)
			{
				FrozenLevel = syn.SynCode[2];
			}
			else if (syn.SynCode[1] == 1)
			{
				Ice(isSyn: true);
			}
			else if (syn.SynCode[1] == 2)
			{
				Butter(isSyn: true);
			}
			else if (syn.SynCode[1] == 3)
			{
				if (changeLineCoroutine == null)
				{
					changeLineCoroutine = StartCoroutine(MovetoLine(syn.SynCode[2]));
				}
			}
			else if (syn.SynCode[1] == 4)
			{
				if (syn.SynCode[2] == 1)
				{
					Hypno(synClient: true);
				}
				else
				{
					RatThis(synClient: true);
				}
			}
			else if (syn.SynCode[1] == 5)
			{
				TeleportTo(syn.Twofloat, synClient: true);
			}
			else if (syn.SynCode[1] == 6)
			{
				Dizzy(syn.SynCode[2], isSyn: true);
			}
			else if (syn.SynCode[1] == 7)
			{
				CustomHp(syn.SynCode[2], synClient: true);
			}
			else if (syn.SynCode[1] == 8)
			{
				CustomSpeed(syn.Twofloat.x, synClient: true);
			}
			else if (syn.SynCode[1] == 9)
			{
				CustomAttack(syn.Twofloat.x, synClient: true);
			}
		}
		else if (syn.SynCode[0] == 1)
		{
			if (syn.SynCode[1] == 0)
			{
				if (isButter)
				{
					UnButter();
				}
				if (isIcetrap)
				{
					UnIce();
				}
				if (isDizzy)
				{
					UnDizzy();
				}
				Hp = 0;
				if (ArmorHp > 0)
				{
					armorHp = 0;
				}
				HpReduceEvent(isHard: false, HitSound: false);
				state = ZombieState.Dead;
				CheckState();
				ResetAnimationSpeed();
			}
			else if (syn.SynCode[1] == 1)
			{
				hp = 0;
				state = ZombieState.Dead;
				if (InWater)
				{
					Dead(canDropItem: true, 0f);
					return;
				}
				Dead();
				PlaceCharred();
			}
			else if (syn.SynCode[1] == 2)
			{
				DirectDead(syn.SynCode[2] == 1, syn.Twofloat.x, synClient: true);
			}
			else if (syn.SynCode[1] == 3)
			{
				CleanerDead(synClient: true);
			}
		}
		else if (syn.SynCode[0] == 2)
		{
			OnlineSyn(syn);
		}
		else if (syn.SynCode[0] == 3 && syn.SynCode[1] == 0)
		{
			ClickEvent(syn.AName);
		}
	}

	protected bool IsStaticState()
	{
		if (isIcetrap || isButter || isDizzy)
		{
			return true;
		}
		return false;
	}

	public void StartIdel()
	{
		animator.SetInteger("Change", 0);
		State = ZombieState.Idel;
	}

	public void PurpleZombie()
	{
		needHypnoPurple = true;
		ResetColor();
	}

	private void CheckChaosAcvment()
	{
		if (isHypno && needHypnoPurple && isDizzy && isIcetrap && isButter)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.ChaosZombie);
		}
	}

	public void Hypno(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = true;
			ResetColor();
			hypZombie(synClient);
			CheckChaosAcvment();
		}
	}

	public void RatThis(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = false;
			ResetColor();
			hypZombie(synClient);
		}
	}

	private void hypZombie(bool synClient)
	{
		GoBack();
		isHypno = !isHypno;
		HypnoEvent();
		ZombieManager.Instance.ZombieHypno(this);
		hypnoAttackTarget = null;
		if (GameManager.Instance.isServer && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Zombie;
			synItem.SynCode[0] = 0;
			synItem.SynCode[1] = 4;
			if (needHypnoPurple)
			{
				synItem.SynCode[2] = 1;
			}
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public void Frozen(Vector2 dirct, bool isAudio = true, int frozenLvl = 1)
	{
		if (!GameManager.Instance.isClient && State != ZombieState.Dead && canFrozen && (DoorHp <= 0 || !IsFacingLeft || !(dirct.x > 0f)) && (DoorHp <= 0 || IsFacingLeft || !(dirct.x < 0f)))
		{
			if (!isFrozen & isAudio)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
			}
			FrozenLevel += frozenLvl;
			int num = 9 - frozenLvl;
			if (FrozenLevel > 8 && Random.Range(0, 10) > num)
			{
				Ice();
			}
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 0;
				synItem.SynCode[2] = (byte)FrozenLevel;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void UnFrozen(int UnLvl)
	{
		if (!isIcetrap)
		{
			FrozenLevel -= UnLvl;
			if (FrozenLevel == 0)
			{
				isFrozen = false;
			}
		}
	}

	private void UnFrozenOne()
	{
		UnFrozen(1);
	}

	public void Ice(bool isSyn = false)
	{
		if ((isSyn || !GameManager.Instance.isClient) && State != ZombieState.Dead && !BanAllInmobilize && !IsCriticalState && canIce)
		{
			isIcetrap = true;
			ResetAnimationSpeed();
			if (icetrapCoroutine != null)
			{
				StopCoroutine(icetrapCoroutine);
			}
			CheckChaosAcvment();
			float num = ((CurrMap.CurrTempt < 0f) ? (0.007f * CurrMap.CurrTempt + 0.71f) : ((!(CurrMap.CurrTempt < 20f)) ? (0.1f * CurrMap.CurrTempt - 1f) : (0.0145f * CurrMap.CurrTempt + 0.71f)));
			if (num > 4f)
			{
				num = 4f;
			}
			if (num < 0.5f)
			{
				num = 0.5f;
			}
			float time = 4f / num;
			icetrapCoroutine = StartCoroutine(DoFuncWait(() =>
			{
				UnIce();
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.UnIceParticle).transform.position = base.transform.position;
			}, time));
			FrozenLevel = 10;
			if (!InWater)
			{
				Icetrap.enabled = true;
			}
			OnIceEvent();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void UnIce()
	{
		isIcetrap = false;
		if (icetrapCoroutine != null)
		{
			StopCoroutine(icetrapCoroutine);
		}
		ResetAnimationSpeed();
		ResetColor();
		Icetrap.enabled = false;
		UnFrozen(1);
	}

	public void Butter(bool isSyn = false)
	{
		if ((isSyn || !GameManager.Instance.isClient) && base.gameObject.activeInHierarchy && !BanAllInmobilize && canButter && !isDropHead)
		{
			isButter = true;
			ResetAnimationSpeed();
			if (butterCoroutine != null)
			{
				StopCoroutine(butterCoroutine);
			}
			butterCoroutine = StartCoroutine(DoFuncWait(UnButter, 4f));
			if (butter != null)
			{
				butter.enabled = true;
			}
			CheckChaosAcvment();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 2;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void UnButter()
	{
		isButter = false;
		ResetAnimationSpeed();
		if (butter != null)
		{
			butter.enabled = false;
		}
	}

	public void Yuck()
	{
		if (IsCriticalState || State == ZombieState.Dead)
		{
			return;
		}
		dontChangeState = true;
		animator.speed = 0f;
		if (yuckHead != null)
		{
			HeadRenderer.sprite = yuckHead;
			JawRenderer.enabled = false;
		}
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.yuck1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.yuck2, base.transform.position);
		}
		StartCoroutine(DoFuncWait(() =>
		{
			if (yuckHead != null)
			{
				HeadRenderer.sprite = normalHead;
				JawRenderer.enabled = true;
			}
			if (!isButter && !isIcetrap && !isDizzy)
			{
				if (Hp <= 0)
				{
					State = ZombieState.Dead;
				}
				if (State == ZombieState.Dead)
				{
					ResetAnimationSpeed();
				}
				dontChangeState = false;
				if (State != ZombieState.Dead)
				{
					ResetAnimationSpeed();
					ChangeLine();
				}
			}
		}, 0.4f));
	}

	public void ChangeLine()
	{
		if (!GameManager.Instance.isClient && changeLineCoroutine == null)
		{
			int num = ((MapManager.Instance.GetCurrMap(base.transform.position).MapGridNum.y - 1 == CurrLine) ? (CurrLine - 1) : ((CurrLine == 0) ? (CurrLine + 1) : ((Random.Range(0, 2) != 1) ? (CurrLine + 1) : (CurrLine - 1))));
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 3;
				synItem.SynCode[2] = (byte)num;
				SocketServer.Instance.SendSynBag(synItem);
			}
			changeLineCoroutine = StartCoroutine(MovetoLine(num));
		}
	}

	private IEnumerator MovetoLine(int line)
	{
		Grid grid = null;
		if (State != ZombieState.Dead)
		{
			grid = MapManager.Instance.GetGridByWorldPos(base.transform.position, line);
		}
		if (grid == null)
		{
			yield break;
		}
		State = ZombieState.Walk;
		collider2d.enabled = false;
		dontChangeState = true;
		int last2 = Sorting.sortingOrder % 100;
		if (InWater && !grid.isWaterGrid)
		{
			StartCoroutine(MoveOutWater());
		}
		if (InWater && !grid.isWaterGrid)
		{
			going = true;
			while (base.transform.position.y < grid.Position.y)
			{
				yield return null;
				base.transform.Translate(new Vector2(0f, 2f) * Time.deltaTime);
			}
			Shadow.enabled = true;
			going = false;
		}
		if (line > CurrLine)
		{
			SetSortingOrder(FixedInfo.GetBaseSort(line) + last2 + 100);
			Shadow.sortingOrder = FixedInfo.GetBaseSort(line) + 10;
			WaterMask.frontSortingOrder = Sorting.sortingOrder;
			WaterMask.backSortingOrder = Sorting.sortingOrder - 1;
			while (base.transform.position.y > grid.Position.y)
			{
				yield return null;
				base.transform.Translate(new Vector2(0f, -2f) * Time.deltaTime);
			}
		}
		else
		{
			while (base.transform.position.y < grid.Position.y)
			{
				yield return null;
				base.transform.Translate(new Vector2(0f, 2f) * Time.deltaTime);
			}
			SetSortingOrder(FixedInfo.GetBaseSort(line) + last2 + 100);
			Shadow.sortingOrder = FixedInfo.GetBaseSort(line) + 10;
			WaterMask.frontSortingOrder = Sorting.sortingOrder;
			WaterMask.backSortingOrder = Sorting.sortingOrder - 1;
		}
		dontChangeState = false;
		CurrGrid = grid;
		State = ZombieState.Walk;
		changeLineCoroutine = null;
		collider2d.enabled = true;
	}

	public void TeleportTo(Vector2 pos, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			base.transform.position = pos;
			CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
			CurrGrid = MapManager.Instance.GetGridByWorldPos(pos);
			int num = Sorting.sortingOrder % 100;
			SetSortingOrder(FixedInfo.GetBaseSort(CurrLine) + num + 100);
			Shadow.sortingOrder = FixedInfo.GetBaseSort(CurrLine) + 10;
			WaterMask.frontSortingOrder = Sorting.sortingOrder;
			WaterMask.backSortingOrder = Sorting.sortingOrder - 1;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 5;
				synItem.Twofloat = pos;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void Dizzy(int dizzyTime, bool isSyn = false)
	{
		if ((isSyn || !GameManager.Instance.isClient) && State != ZombieState.Dead && !(HeadRenderer == null) && !IsCriticalState && !BanAllInmobilize && canDizzy && !isDropHead)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.dizzy, base.transform.position);
			isDizzy = true;
			this.dizzyTime += dizzyTime;
			ResetAnimationSpeed();
			if (DizzyObject == null)
			{
				DizzyObject = Object.Instantiate(GameManager.Instance.GameConf.Dizzy);
				DizzyObject.transform.SetParent(base.transform);
			}
			DizzyObject.SetActive(value: true);
			DizzyObject.transform.position = HeadRenderer.transform.position + new Vector3(0f, 0.2f);
			DizzyObject.GetComponent<SortingGroup>().sortingOrder = Sorting.sortingOrder + 1;
			if (dizzyCoroutine != null)
			{
				StopCoroutine(dizzyCoroutine);
			}
			dizzyCoroutine = StartCoroutine(DeDizzy());
			CheckChaosAcvment();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 6;
				synItem.SynCode[2] = dizzyTime;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private IEnumerator DeDizzy()
	{
		while (dizzyTime > 0)
		{
			yield return new WaitForSeconds(1f);
			dizzyTime--;
		}
		if (dizzyTime <= 0)
		{
			dizzyTime = 0;
			UnDizzy();
		}
	}

	private void UnDizzy()
	{
		isDizzy = false;
		ResetAnimationSpeed();
		if (DizzyObject != null)
		{
			DizzyObject.SetActive(value: false);
		}
	}

	protected IEnumerator DoFuncWait(UnityAction fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun?.Invoke();
	}

	protected void DropEquip(SpriteRenderer renderer)
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EquipDropEF).GetComponent<EquipDropEF>().CreateInit(Sorting.sortingOrder, CurrGrid, renderer, new Vector3(animator.transform.localScale.x * BaseTransform.localScale.x * base.transform.localScale.x, animator.transform.localScale.y * BaseTransform.localScale.y * base.transform.localScale.y), IsFacingLeft);
	}

	protected void DropArm()
	{
		if (!isDropArm)
		{
			isDropArm = true;
			if (lostArm != null)
			{
				UpArmRenderer.sprite = lostArm;
			}
			if (MidArmRenderer != null)
			{
				MidArmRenderer.material.SetInt("_OpenDisplay", 0);
			}
			if (LowArmRenderer != null)
			{
				LowArmRenderer.material.SetInt("_OpenDisplay", 0);
			}
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ArmDropEF).GetComponent<ArmDropEF>().CreateInit(Sorting.sortingOrder, CurrGrid, MidArmRenderer, LowArmRenderer, new Vector3(animator.transform.localScale.x * BaseTransform.localScale.x * base.transform.localScale.x, animator.transform.localScale.y * BaseTransform.localScale.y * base.transform.localScale.y), IsFacingLeft);
		}
	}

	public void DropHead()
	{
		if (!isDropHead)
		{
			if (NeedDropHead)
			{
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.HeadDropEF).GetComponent<HeadDropEF>().CreateInit(Sorting.sortingOrder, CurrGrid, HeadRenderer, headSprites, new Vector3(animator.transform.localScale.x * BaseTransform.localScale.x * base.transform.localScale.x, animator.transform.localScale.y * BaseTransform.localScale.y * base.transform.localScale.y), IsFacingLeft);
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropHead, base.transform.position);
			}
			isDropHead = true;
			for (int i = 0; i < headSprites.Count; i++)
			{
				headSprites[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void TangkleStopAction()
	{
		animator.speed = 0f;
		anCanMove = false;
		TangkleStopActionVirtual();
		WhiteWater.gameObject.SetActive(value: false);
		AnimatorSpeedChange(animator.speed);
	}

	protected virtual void TangkleStopActionVirtual()
	{
	}

	public void GameOverFakeDeath()
	{
		animator.speed = 0f;
		AnimatorSpeedChange(0f);
		IsOVer = true;
		StopAllCoroutines();
	}

	public virtual void AnimFailSound()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail2, base.transform.position);
		}
	}

	public void BurnEvent()
	{
		UnIce();
		UnFrozen(10);
		OnBurnEvent();
	}

	public void CustomHp(int chp, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			Hp = chp;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 7;
				synItem.SynCode[2] = chp;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void CustomSpeed(float speed, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			customSpeed = speed;
			ResetSpeedRate();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 8;
				synItem.Twofloat = new Vector2(speed, 0f);
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void CustomAttack(float value, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			customAttack = value;
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 9;
				synItem.Twofloat = new Vector2(value, 0f);
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
		{
			if (!returnFirstClick)
			{
				returnFirstClick = true;
			}
			else if (GameManager.Instance.isClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Zombie;
				synItem.AName = GameManager.Instance.LocalPlayerSave.playerName;
				synItem.SynCode[0] = 3;
				synItem.SynCode[1] = 0;
				SocketClient.Instance.SendSynBag(synItem);
			}
			else
			{
				ClickEvent(GameManager.Instance.LocalPlayerSave.playerName);
			}
		}
	}

	protected virtual int HandleBoomHurt(int attackValue)
	{
		return attackValue;
	}

	protected virtual int HandleHurt(int attackValue, Vector2 dirction)
	{
		return attackValue;
	}

	public virtual SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		return null;
	}

	protected virtual void FrameChangeEvent(SwfClip swfClip)
	{
	}

	protected virtual void OnBurnEvent()
	{
	}

	protected virtual void OnAttackEvent()
	{
	}

	protected virtual void SpriteChangeEvent(Texture2D nextTexture)
	{
	}

	protected virtual void SpriteChangeEvent(Sprite nextSprite)
	{
	}

	protected virtual void CurrGridChangeEvent(Grid lastGrid)
	{
	}

	protected virtual void InWaterChangeEvent()
	{
	}

	public virtual void OutInWaterEvent()
	{
	}

	protected virtual void AttackEvent(PlantBase plant, ZombieBase zombie)
	{
	}

	protected virtual void HypnoEvent()
	{
	}

	protected virtual void OnIceEvent()
	{
	}

	protected virtual void ClickEvent(string clickPlayer)
	{
	}

	protected virtual void HpReduceEvent(bool isHard, bool HitSound)
	{
	}

	protected virtual void DoorHpReduceEvent()
	{
	}

	protected virtual void ChangeFacingEvent()
	{
	}

	protected virtual void DeadStateGetStaticBuff()
	{
	}

	protected virtual void UpdateThis()
	{
	}

	public virtual void SpecialAnimEvent1()
	{
	}

	public virtual void SpecialAnimEvent2()
	{
	}

	public virtual void SpecialAnimEvent3()
	{
	}

	public virtual void SpecialAnimEvent4()
	{
	}

	public virtual void SpecialAnimEvent5()
	{
	}

	public virtual void SpecialAnimEvent6()
	{
	}
}
