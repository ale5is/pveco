using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class PlantBase : MonoBehaviour
{
	public int OnlineId;

	public string PlacePlayer;

	protected SpriteRenderer Shadow;

	protected Transform ZZZ;

	private MapBase currMap;

	public Sprite eye1;

	public Sprite eye2;

	public SpriteRenderer EyeREnderer;

	protected bool isClosingEye;

	protected int closeEyeFrame;

	[SerializeField]
	private float speedRate = 1f;

	private float customSpeed;

	private float customAttack;

	public bool isSleeping;

	private float hp;

	private bool isFlash;

	public PlantBase CarryPlant;

	public PlantBase ProtectPlant;

	protected bool needFlatDead;

	private Coroutine FlashCoroutine;

	private Coroutine BrightCoroutine;

	private bool isFacingLeft;

	protected bool needHypnoPurple;

	public bool isHypno;

	private int frozenLevel;

	private bool isFrozen;

	protected bool canFrozen = true;

	private Coroutine frozenCoroutine;

	private float baseAnimSpeed;

	protected bool isIcetrap;

	protected bool canIce = true;

	protected SpriteRenderer Icetrap;

	private Coroutine icetrapCoroutine;

	protected bool isButter;

	protected bool canButter = true;

	protected SpriteRenderer butter;

	private Coroutine butterCoroutine;

	private bool isDizzy;

	private int dizzyTime;

	private GameObject DizzyObject;

	protected bool canDizzy = true;

	private Coroutine dizzyCoroutine;

	private Animator animator;

	protected SortingGroup animatorSorting;

	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	private bool isStopAnim;

	private Coroutine WaitAnimChange;

	private List<Animator> PaperSwfClips = new List<Animator>();

	private float ActionCDTime;

	protected float CDTimeScale = 1f;

	protected bool CDTimeOver;

	private Coroutine ActionCdCoroutine;

	public Grid currGrid { get; private set; }

	protected virtual int attackValue { get; }

	protected virtual bool isShroom { get; }

	public virtual bool isFloatPlant { get; }

	protected virtual bool haveSpEye { get; }

	public virtual bool isHaveSpecialCheck { get; }

	public virtual bool isProtectPlant { get; }

	public virtual PlantType BasePlant { get; }

	public virtual LikeTmptType TemptType { get; }

	public virtual float FavoriteTempt { get; } = 20f;

	public virtual int BasePlantSunNum { get; } = -1;

	public virtual float Temperature { get; }

	protected virtual List<string> DontPlayAnim { get; } = new List<string> { "anim_idle" };

	public PlantType plantType { get; private set; }

	protected virtual Vector2 offSet { get; } = Vector2.zero;

	public virtual bool ZombieCanEat { get; } = true;

	public virtual bool CanPlaceOnWaterCarry { get; } = true;

	public virtual bool CanPlaceOnGrass { get; } = true;

	public virtual bool CanPlaceOnWater { get; }

	public virtual bool CanPlaceOnHardGround { get; }

	public virtual bool CanPlaceOnPuddle { get; }

	public virtual bool CanCarryed { get; } = true;

	public virtual bool CanCarryOtherPlant { get; }

	public virtual bool CanProtect { get; } = true;

	public virtual Vector2 CarryOffset { get; } = Vector2.zero;

	public virtual bool IsZombiePlant { get; }

	public virtual bool SnowHurt { get; } = true;

	public virtual PlantType FrozenEvolution { get; }

	public virtual bool IsLowPlant { get; }

	public int SortingOrder => animatorSorting.sortingOrder;

	public abstract float MaxHp { get; }

	protected virtual bool HaveShadow { get; } = true;

	public float Hp
	{
		get
		{
			return hp;
		}
		private set
		{
			if (value <= hp)
			{
				if (BrightCoroutine != null)
				{
					StopCoroutine(BrightCoroutine);
				}
				BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f, null));
			}
			hp = value;
			CreatePanel.Instance.ChangeHp(this, null);
		}
	}

	public int CurrLine
	{
		get
		{
			if (currGrid != null)
			{
				return currGrid.Point.y;
			}
			return -1;
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

	public float BaseAnimSpeed
	{
		get
		{
			return baseAnimSpeed;
		}
		protected set
		{
			baseAnimSpeed = value;
			ResetAnimationSpeed();
		}
	}

	protected float SpeedRate
	{
		get
		{
			return speedRate;
		}
		set
		{
			if (value != 0f)
			{
				speedRate = value;
				ResetAnimationSpeed();
			}
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
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
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
			if (FrozenLevel > 0)
			{
				frozenCoroutine = StartCoroutine(DoFuncWait(UnFrozenOne, 4.5f - (float)FrozenLevel * 0.4f));
			}
			ResetSpeedRate();
			ResetColor();
		}
	}

	protected void InitForAll()
	{
		needFlatDead = true;
		isFlash = false;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		if (animator == null)
		{
			animator = base.transform.Find("Animation").GetComponent<Animator>();
			animatorSorting = animator.GetComponent<SortingGroup>();
			SpriteRenderer[] componentsInChildren = animator.transform.GetComponentsInChildren<SpriteRenderer>();
			renderers.AddRange(componentsInChildren);
			Icetrap = base.transform.Find("icetrap").GetComponent<SpriteRenderer>();
			Shadow = base.transform.Find("Shadow").GetComponent<SpriteRenderer>();
		}
		Icetrap.sortingOrder = animatorSorting.sortingOrder + 1;
		Icetrap.enabled = false;
		isIcetrap = false;
		if (HaveShadow)
		{
			Shadow.enabled = false;
		}
		ZZZ = base.transform.Find("ZZZ");
		ZZZ.gameObject.SetActive(value: false);
		isSleeping = false;
		if (EyeREnderer != null)
		{
			EyeREnderer.material.SetTexture("_EyeTex", null);
		}
		customAttack = 1f;
		customSpeed = 1f;
		SpeedRate = 1f;
		BaseAnimSpeed = 1f;
		isFacingLeft = false;
		base.transform.localScale = new Vector3(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		isHypno = false;
		needHypnoPurple = false;
		FrozenLevel = 0;
		if (LV.Instance.CurrLVType == LVType.IZombie)
		{
			SetPaper();
		}
		else if (PaperSwfClips.Count > 0)
		{
			List<Animator> list = new List<Animator>(PaperSwfClips);
			PaperSwfClips.Clear();
			foreach (Animator item in list)
			{
				if (item != null)
				{
					Object.Destroy(item.gameObject);
				}
			}
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.PlantReverse))
		{
			GoBack();
		}
		SetAllBrightness(1f);
		OnInitForAll();
	}

	public void SetPlantType(PlantType plantType)
	{
		this.plantType = plantType;
	}

	public void UpdateForCreate(Grid grid)
	{
		if (grid.CurrPlantBase != null)
		{
			base.transform.position = grid.Position + offSet + grid.CurrPlantBase.CarryOffset;
		}
		else
		{
			base.transform.position = grid.Position + offSet;
		}
	}

	public void InitForAlmanac(Vector2 pos)
	{
		InitForAll();
		if (HaveShadow)
		{
			Shadow.enabled = true;
		}
		base.transform.position = pos;
		currGrid = null;
		SetColor(Color.white);
		OnInitForAlmanac();
	}

	public void InitForCreate(bool inGrid, Grid grid, bool isBlcWhi)
	{
		InitForAll();
		if (grid != null)
		{
			if (grid.CurrPlantBase != null)
			{
				base.transform.position = grid.Position + offSet + grid.CurrPlantBase.CarryOffset;
			}
			else
			{
				base.transform.position = grid.Position + offSet;
			}
		}
		SetAnimSpeed(0f);
		if (inGrid)
		{
			SetAlpha(0.7f);
			animatorSorting.sortingOrder = 2011;
		}
		else
		{
			SetAlpha(1f);
			animatorSorting.sortingOrder = 2013;
		}
		OpenBlackAndWhite(isBlcWhi);
		OnInitForCreate();
	}

	public void InitForPlace(Grid grid, int orderNum, bool PlaceEff)
	{
		InitForAll();
		Hp = MaxHp;
		SetAlpha(1f);
		ResetColor();
		CDTimeScale = SpeedRate;
		PlaceOn(grid, orderNum, PlaceEff);
		if (GameManager.Instance.isClient)
		{
			OnInitForCreate();
		}
		OnInitForPlace();
		if (LV.Instance.CurrLVType != LVType.IZombie && LV.Instance.CurrLVType != LVType.VaseBreaker)
		{
			if (isShroom)
			{
				if (SkyManager.Instance.GetIsDay())
				{
					GoSleep();
				}
				else if (isSleeping)
				{
					GoAwake();
				}
			}
			else if (!SkyManager.Instance.GetIsDay() && Random.Range(0, 8) > 6 && !LV.Instance.LvSpStates.Contains(LVSpState.RainPlant))
			{
				GoSleep();
			}
			else if (isSleeping)
			{
				GoAwake();
			}
			if (!isSleeping && LV.Instance.LvSpStates.Contains(LVSpState.SleepDay))
			{
				GoSleep();
			}
		}
		isStopAnim = false;
		ResetAnimationSpeed(s: true);
		if ((EyeREnderer != null || haveSpEye) && PaperSwfClips.Count == 0)
		{
			StartCoroutine(CloseEyes());
		}
		WeatherChangeEvent();
	}

	public virtual void SpCheckInitPlace()
	{
	}

	private void PlaceOn(Grid grid, int orderNum, bool PlaceEff)
	{
		currGrid = grid;
		CurrMap = MapManager.Instance.GetCurrMap(grid.Position);
		CurrMap.EntityTempt += Temperature;
		if (grid.CurrPlantBase != null)
		{
			base.transform.position = grid.Position + offSet + grid.CurrPlantBase.CarryOffset;
		}
		else
		{
			base.transform.position = grid.Position + offSet;
		}
		if (CanCarryOtherPlant)
		{
			animatorSorting.sortingOrder = currGrid.Point.y * 200 + FixedInfo.BasePlant;
		}
		else
		{
			animatorSorting.sortingOrder = currGrid.Point.y * 200 + orderNum;
		}
		ProtectPlant = null;
		if (PlaceEff)
		{
			if (grid.isWaterGrid && !grid.IsIce)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.PlantWater, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant2, base.transform.position);
			}
		}
		if (HaveShadow)
		{
			Shadow.enabled = true;
			Shadow.sortingOrder = animatorSorting.sortingOrder / 200 * 200 + FixedInfo.Shadow;
			Shadow.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
			if (Shadow.sortingOrder > animatorSorting.sortingOrder)
			{
				Shadow.sortingOrder = animatorSorting.sortingOrder - 1;
			}
			if (PlaceEff)
			{
				if (grid.isNoIceWater)
				{
					PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SmallWaterParticle).transform.position = grid.Position + new Vector2(0f, -0.3f);
				}
				else
				{
					PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SmallDirtParticle).transform.position = grid.Position + new Vector2(0f, -0.3f);
				}
			}
		}
		else
		{
			Shadow.enabled = false;
		}
		Icetrap.sortingOrder = animatorSorting.sortingOrder + 1;
	}

	protected void ResetColor()
	{
		float a = renderers[0].color.a;
		if (FrozenLevel > 0)
		{
			SetColor(new Color(1f - (float)FrozenLevel * 0.055f, 1f - (float)FrozenLevel * 0.05f, 1f - (float)FrozenLevel * 0.005f, a));
		}
		else if (needHypnoPurple)
		{
			SetColor(new Color(0.88f, 0.39f, 1f, a));
		}
		else if (LV.Instance.CurrLVType == LVType.IZombie)
		{
			SetColor(new Color(1f, 0.745f, 0.51f, a));
		}
		else
		{
			SetColor(new Color(1f, 1f, 1f, a));
		}
	}

	public void ResetSpeedRate()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)(-FrozenLevel) * 0.05f;
		GobalEffManager.Instance.IsHaveThisEff(GobalEffect.RainCool);
		if (CurrMap != null)
		{
			if (CurrMap.CurrTempt > FavoriteTempt + 10f)
			{
				num2 = (0f - Mathf.Abs(CurrMap.CurrTempt - FavoriteTempt)) / 100f;
				if (TemptType == LikeTmptType.Hot)
				{
					num2 = 0f;
				}
			}
			else if (CurrMap.CurrTempt < FavoriteTempt - 10f)
			{
				num2 = (0f - Mathf.Abs(CurrMap.CurrTempt - FavoriteTempt)) / 100f;
				if (TemptType == LikeTmptType.Cold)
				{
					num2 = 0f;
				}
			}
			if (num2 < -0.4f)
			{
				num2 = -0.4f;
			}
		}
		float num4 = 1f + num3 + num2 + num;
		num4 *= customSpeed;
		if (num4 < 0.1f)
		{
			num4 = 0.1f;
		}
		speedRate = num4;
		ResetAnimationSpeed();
	}

	private void SetAlpha(float a)
	{
		Color color = renderers[0].color;
		SetColor(new Color(color.r, color.g, color.b, a));
	}

	private void SetColor(Color color)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].color = color;
		}
		OwnerSetColor(color);
	}

	private void SetAllBrightness(float value)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].material.SetFloat("_Brightness", value);
		}
		OwnerSetBrightness(value);
	}

	protected void SetSpriteEnable(string name, bool enable)
	{
		for (int i = 0; i < PaperSwfClips.Count; i++)
		{
			PaperSwfClips[i].transform.Find(name).GetComponent<SpriteRenderer>().enabled = enable;
		}
	}

	protected void SetSprite(string name, Sprite sprite)
	{
		for (int i = 0; i < PaperSwfClips.Count; i++)
		{
			PaperSwfClips[i].transform.Find(name).GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}

	protected void SetAnimChange(int Integ)
	{
		animator.SetInteger("Change", Integ);
		for (int i = 0; i < PaperSwfClips.Count; i++)
		{
			PaperSwfClips[i].SetInteger("Change", Integ);
		}
		WaitAnim();
	}

	public void WaitAnim()
	{
		if (LV.Instance.CurrLVType == LVType.IZombie)
		{
			if (WaitAnimChange != null)
			{
				StopCoroutine(WaitAnimChange);
			}
			WaitAnimChange = StartCoroutine(sss());
		}
	}

	private IEnumerator sss()
	{
		AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
		string lastAnim = currentAnimatorClipInfo[0].clip.name;
		while (true)
		{
			if (Time.timeScale > 0f)
			{
				currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
				if (currentAnimatorClipInfo[0].clip.name != lastAnim || animator.IsInTransition(0))
				{
					break;
				}
			}
			yield return null;
		}
		ResetAnimationSpeed(s: true);
	}

	protected void ResetAnimationSpeed(bool s = false)
	{
		if (Hp <= 0f)
		{
			return;
		}
		float speed = 0f;
		if (IsStaticState() || BaseAnimSpeed == 0f)
		{
			CDTimeScale = 0f;
		}
		else
		{
			CDTimeScale = SpeedRate;
			speed = SpeedRate * BaseAnimSpeed;
		}
		animator.speed = speed;
		if ((PaperSwfClips.Count > 0) & s)
		{
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
			if (currentAnimatorClipInfo.Length != 0)
			{
				string item = currentAnimatorClipInfo[0].clip.name;
				if (animator.IsInTransition(0))
				{
					currentAnimatorClipInfo = animator.GetNextAnimatorClipInfo(0);
					item = currentAnimatorClipInfo[0].clip.name;
				}
				if (!isStopAnim)
				{
					if (DontPlayAnim.Contains(item))
					{
						animator.speed = 0f;
						isStopAnim = true;
					}
					else
					{
						animator.speed = speed;
					}
				}
				else
				{
					isStopAnim = false;
					animator.speed = speed;
				}
			}
		}
		SetAnimSpeed(animator.speed);
	}

	private void SetAnimSpeed(float speed)
	{
		animator.speed = speed;
		for (int i = 0; i < PaperSwfClips.Count; i++)
		{
			PaperSwfClips[i].speed = speed;
		}
		ResetAnimSpeedEvent(speed);
	}

	private void SetPaper()
	{
		if (PaperSwfClips.Count > 0)
		{
			return;
		}
		Animator component = Object.Instantiate(animator.transform.gameObject).GetComponent<Animator>();
		component.transform.position = animator.transform.position + new Vector3(-0.03f, 0.03f);
		PaperSwfClips.Add(component);
		Animator component2 = Object.Instantiate(animator.transform.gameObject).GetComponent<Animator>();
		component2.transform.position = animator.transform.position + new Vector3(0.02f, -0.02f);
		PaperSwfClips.Add(component2);
		Animator component3 = Object.Instantiate(animator.transform.gameObject).GetComponent<Animator>();
		component3.transform.position = animator.transform.position + new Vector3(0.04f, -0.04f);
		PaperSwfClips.Add(component3);
		for (int i = 0; i < PaperSwfClips.Count; i++)
		{
			Color32 color = new Color32(184, 152, 126, byte.MaxValue);
			PaperSwfClips[i].transform.SetParent(animator.transform);
			PaperSwfClips[i].GetComponent<SortingGroup>().sortingOrder = -1;
			PaperSwfClips[i].GetComponent<PlantAnimEvent>().EventDisable = true;
			if (i == 2)
			{
				color = new Color32(124, 88, 60, byte.MaxValue);
				PaperSwfClips[i].GetComponent<SortingGroup>().sortingOrder = -2;
			}
			SpriteRenderer[] componentsInChildren = PaperSwfClips[i].transform.GetComponentsInChildren<SpriteRenderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].GetComponent<Renderer>().material.SetInt("_OpenSolid", 1);
				componentsInChildren[j].GetComponent<Renderer>().material.SetColor("_SolideColor", color);
			}
		}
		ResetAnimationSpeed();
	}

	protected virtual bool DoAction()
	{
		return false;
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
				if (!isSleeping && !IsStaticState())
				{
					CDTimeOver = !DoAction();
				}
				if (CDTimeOver)
				{
					yield return new WaitForSeconds(num);
				}
				continue;
			}
			float CurrCDTime = ActionCDTime;
			do
			{
				float time = ActionCDTime / 10f;
				yield return new WaitForSeconds(time);
				if (!isSleeping && !IsStaticState())
				{
					float num2 = 1f - (float)FrozenLevel * 0.05f;
					CurrCDTime -= time * CDTimeScale * num2;
				}
			}
			while (!(CurrCDTime <= 0f));
			CDTimeOver = true;
		}
	}

	protected void ServerSendSyn(int code1, int code2 = 0, int code3 = 0)
	{
		ServerSendSyn(code1, Vector2.zero, code2, code3);
	}

	protected void ServerSendSyn(int code1, Vector2 pos, int code2 = 0, int code3 = 0, string AName = "")
	{
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Plant;
			synItem.Twofloat = pos;
			synItem.SynCode[0] = 1;
			synItem.SynCode[1] = code1;
			synItem.SynCode[2] = code2;
			synItem.SynCode[3] = code3;
			synItem.AName = AName;
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	protected virtual void OnlineSyn(SynItem syn)
	{
	}

	public virtual void OnlineSynPlant(SynItem syn)
	{
		if (syn.SynCode[0] == 0 && GameManager.Instance.isClient)
		{
			if (syn.SynCode[1] == 0)
			{
				Dead(syn.SynCode[2] == 1, syn.Twofloat.x, synClient: true, syn.SynCode[2] != 2);
			}
			else if (syn.SynCode[1] == 1)
			{
				GoSleep(synClient: true);
			}
			else if (syn.SynCode[1] == 2)
			{
				GoAwake(synClient: true);
			}
			else if (syn.SynCode[1] == 3)
			{
				FrozenLevel = syn.SynCode[2];
			}
			else if (syn.SynCode[1] == 4)
			{
				Ice(isSyn: true);
			}
			else if (syn.SynCode[1] == 5)
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
			else if (syn.SynCode[1] == 6)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FrozenEvolutionParticle).transform.position = base.transform.position + new Vector3(0f, 0.1f);
			}
			else if (syn.SynCode[1] == 7)
			{
				Butter(synClient: true);
			}
			else if (syn.SynCode[1] == 8)
			{
				Dizzy(syn.SynCode[2], isSyn: true);
			}
			else if (syn.SynCode[1] == 9)
			{
				CustomHp(syn.SynCode[2], synClient: true);
			}
			else if (syn.SynCode[1] == 10)
			{
				CustomSpeed(syn.Twofloat.x, synClient: true);
			}
			else if (syn.SynCode[1] == 11)
			{
				CustomAttack(syn.Twofloat.x, synClient: true);
			}
			else if (syn.SynCode[1] == 12)
			{
				Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(syn.Twofloat);
				if (gridByWorldPos != null)
				{
					MoveToGrid(gridByWorldPos, synClient: true);
				}
			}
		}
		else if (syn.SynCode[0] == 1)
		{
			OnlineSyn(syn);
		}
	}

	public void Hurt(float hurtValue, Vector2 dirction, ZombieBase zombie, bool isFlat = false)
	{
		if (!isFlat)
		{
			if (ProtectPlant != null && dirction.x != 0f)
			{
				ProtectPlant.Hurt(hurtValue, dirction, zombie, isFlat);
				return;
			}
			if (CarryPlant != null)
			{
				CarryPlant.Hurt(hurtValue, dirction, zombie, isFlat);
				return;
			}
			if (CanCarryOtherPlant && ProtectPlant != null)
			{
				ProtectPlant.Hurt(hurtValue, dirction, zombie, isFlat);
				return;
			}
			hurtValue = ((!PlantManager.Instance.PlantInvincible) ? HandleHurt(hurtValue, isFlat) : 0f);
			if (isDizzy)
			{
				hurtValue = (int)(1.5f * hurtValue);
			}
			Hp -= hurtValue;
			if (zombie != null)
			{
				if (Hp <= 0f)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gulp, base.transform.position);
				}
				else
				{
					HurtAudio();
				}
			}
			if (Hp <= 0f)
			{
				HpUpdateEvents(zombie, isFlat);
				Dead();
			}
			else
			{
				HpUpdateEvents(zombie, isFlat);
			}
			return;
		}
		hurtValue = ((!PlantManager.Instance.PlantInvincible) ? HandleHurt(hurtValue, isFlat) : 0f);
		if (hurtValue > 0f)
		{
			if (ProtectPlant != null)
			{
				ProtectPlant.Hurt(hurtValue, dirction, zombie, isFlat);
			}
			if (CarryPlant != null)
			{
				CarryPlant.Hurt(hurtValue, dirction, zombie, isFlat);
			}
			HpUpdateEvents(zombie, isFlat);
			Dead(isFlat: true);
		}
	}

	protected void GoBack()
	{
		IsFacingLeft = !IsFacingLeft;
	}

	protected bool IsStaticState()
	{
		if (isIcetrap || isButter || isDizzy)
		{
			return true;
		}
		return false;
	}

	public void Hypno(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			if (CarryPlant != null)
			{
				CarryPlant.Hypno();
				CarryPlant.GoBack();
			}
			if (ProtectPlant != null)
			{
				ProtectPlant.Hypno();
				ProtectPlant.GoBack();
			}
			needHypnoPurple = true;
			ResetColor();
			hypPlant(synClient);
		}
	}

	public void RatThis(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			needHypnoPurple = false;
			ResetColor();
			hypPlant(synClient);
		}
	}

	private void hypPlant(bool synClient)
	{
		GoBack();
		isHypno = !isHypno;
		if (GameManager.Instance.isServer && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Plant;
			synItem.SynCode[0] = 0;
			synItem.SynCode[1] = 5;
			if (needHypnoPurple)
			{
				synItem.SynCode[2] = 1;
			}
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public void Frozen(Vector2 dirction, bool isAudio = true, int frozenLvl = 1)
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		if (ProtectPlant != null && !ProtectPlant.isIcetrap && (dirction.x != 0f || isIcetrap))
		{
			ProtectPlant.Frozen(dirction, isAudio, frozenLvl);
		}
		else if (CarryPlant != null && (dirction.y <= 0f || isIcetrap))
		{
			CarryPlant.Frozen(dirction, isAudio, frozenLvl);
		}
		else
		{
			if (!canFrozen)
			{
				return;
			}
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
			if (FrozenEvolution != PlantType.Nope && FrozenLevel > 6 && FrozenLevel < 10 && Random.Range(0, 2) > 0)
			{
				if (GameManager.Instance.isServer)
				{
					SynItem synItem = new SynItem();
					synItem.OnlineId = OnlineId;
					synItem.Type = SynItemType.Plant;
					synItem.SynCode[0] = 0;
					synItem.SynCode[1] = 6;
					SocketServer.Instance.SendSynBag(synItem);
				}
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.FrozenEvolutionParticle).transform.position = base.transform.position + new Vector3(0f, 0.1f);
				SeedBank.Instance.ReplacePlant(this, FrozenEvolution);
			}
			if (GameManager.Instance.isServer)
			{
				SynItem synItem2 = new SynItem();
				synItem2.OnlineId = OnlineId;
				synItem2.Type = SynItemType.Plant;
				synItem2.SynCode[0] = 0;
				synItem2.SynCode[1] = 3;
				synItem2.SynCode[2] = (byte)FrozenLevel;
				SocketServer.Instance.SendSynBag(synItem2);
			}
		}
	}

	public void UnFrozen(int UnLvl)
	{
		if (isIcetrap)
		{
			return;
		}
		if (SkyManager.Instance.SnowScale > 0 && currGrid.HotNum == 0 && UnLvl <= 1)
		{
			FrozenLevel = FrozenLevel;
			return;
		}
		FrozenLevel -= UnLvl;
		if (FrozenLevel == 0)
		{
			isFrozen = false;
		}
	}

	private void UnFrozenOne()
	{
		UnFrozen(1);
	}

	public void Ice(bool isSyn = false)
	{
		if ((isSyn || !GameManager.Instance.isClient) && canIce)
		{
			isIcetrap = true;
			ResetAnimationSpeed();
			UnIceTimer();
			FrozenLevel = 10;
			Icetrap.enabled = true;
			OnIceEvent();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 4;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	private void UnIce()
	{
		if (SkyManager.Instance.SnowScale > 0 && currGrid.HotNum == 0)
		{
			UnIceTimer();
		}
		else
		{
			ClearIce();
		}
	}

	private void UnIceTimer()
	{
		if (!(CurrMap == null))
		{
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
			if (icetrapCoroutine != null)
			{
				StopCoroutine(icetrapCoroutine);
			}
			icetrapCoroutine = StartCoroutine(DoFuncWait(() =>
			{
				UnIce();
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.UnIceParticle).transform.position = base.transform.position;
			}, time));
		}
	}

	private void ClearIce()
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

	public void MoveToGrid(Grid grid, bool synClient = false)
	{
		if ((synClient || !GameManager.Instance.isClient) && (GameManager.Instance.isClient || SeedBank.Instance.CheckPlant(this, grid, -2, PlacePlayer)))
		{
			ClearGrid(isFlat: false);
			PlaceOn(grid, SortingOrder % 200, PlaceEff: true);
			SeedBank.Instance.SetGridInfo(this, grid, -2, PlacePlayer);
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.Twofloat = grid.Position;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 12;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void Butter(bool synClient = false)
	{
		if ((!synClient && GameManager.Instance.isClient) || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (ProtectPlant != null)
		{
			ProtectPlant.Butter(synClient);
		}
		else if (CarryPlant != null)
		{
			CarryPlant.Butter(synClient);
		}
		else if (canButter)
		{
			isButter = true;
			ResetAnimationSpeed();
			if (butterCoroutine != null)
			{
				StopCoroutine(butterCoroutine);
			}
			butterCoroutine = StartCoroutine(DoFuncWait(UnButter, 4f));
			if (butter == null)
			{
				butter = Object.Instantiate(Icetrap.gameObject).GetComponent<SpriteRenderer>();
				butter.sprite = NormalSprite.Instance.PlantButter;
				butter.transform.SetParent(base.transform);
				butter.transform.position = Icetrap.transform.position + new Vector3(0f, -0.14f);
			}
			if (butter != null)
			{
				butter.sortingOrder = Icetrap.sortingOrder;
				butter.enabled = true;
			}
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 7;
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

	protected IEnumerator DoFuncWait(UnityAction fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun?.Invoke();
	}

	public void Dizzy(int dizzyTime, bool isSyn = false)
	{
		if (!isSyn && GameManager.Instance.isClient)
		{
			return;
		}
		if (ProtectPlant != null)
		{
			ProtectPlant.Dizzy(dizzyTime, isSyn);
		}
		else if (CarryPlant != null)
		{
			CarryPlant.Dizzy(dizzyTime, isSyn);
		}
		else if (canDizzy)
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
			DizzyObject.transform.position = base.transform.position + new Vector3(0f, 0.4f);
			DizzyObject.GetComponent<SortingGroup>().sortingOrder = SortingOrder + 1;
			if (dizzyCoroutine != null)
			{
				StopCoroutine(dizzyCoroutine);
			}
			dizzyCoroutine = StartCoroutine(DeDizzy());
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 8;
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

	public void Dead(bool isFlat = false, float waitTime = 0f, bool synClient = false, bool deadRattle = true)
	{
		if ((!synClient && GameManager.Instance.isClient) || !base.gameObject.activeSelf)
		{
			return;
		}
		ClearGrid(isFlat);
		hp = 0f;
		CreatePanel.Instance.PlantDeadEvent(this);
		Shadow.enabled = false;
		DeadEvent();
		if (deadRattle)
		{
			DeadrattleEvent();
		}
		if (GameManager.Instance.isServer && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Plant;
			synItem.SynCode[0] = 0;
			synItem.SynCode[1] = 0;
			if (isFlat && needFlatDead)
			{
				synItem.SynCode[2] = 1;
			}
			if (!deadRattle)
			{
				synItem.SynCode[2] = 2;
			}
			synItem.Twofloat.x = waitTime;
			SocketServer.Instance.SendSynBag(synItem);
		}
		if (isFlat && needFlatDead && !currGrid.isNoIceWater)
		{
			CreateFlatDead();
		}
		else
		{
			StartCoroutine(WaitDead(waitTime));
		}
	}

	private void ClearGrid(bool isFlat)
	{
		if (currGrid == null)
		{
			return;
		}
		if (currGrid.CurrPlantBase != null && (currGrid.CurrPlantBase.CarryPlant != null || currGrid.CurrPlantBase.ProtectPlant != null))
		{
			base.transform.SetParent(currGrid.CurrPlantBase.transform.parent);
			if (currGrid.CurrPlantBase.CarryPlant == this)
			{
				currGrid.CurrPlantBase.CarryPlant = null;
			}
			else if (currGrid.CurrPlantBase.ProtectPlant == this)
			{
				currGrid.CurrPlantBase.ProtectPlant = null;
			}
		}
		if (CarryPlant != null)
		{
			CarryPlant.transform.SetParent(base.transform.parent);
			CarryPlant.Dead(isFlat);
			CarryPlant = null;
		}
		if (ProtectPlant != null)
		{
			if (CanCarryOtherPlant)
			{
				ProtectPlant.Dead(isFlat);
			}
			else
			{
				ProtectPlant.transform.SetParent(base.transform.parent);
				if (isFlat)
				{
					ProtectPlant.Dead(isFlat: true);
				}
				else
				{
					currGrid.CurrPlantBase = ProtectPlant;
					ProtectPlant = null;
				}
			}
		}
		if (currGrid.CurrPlantBase == this)
		{
			currGrid.CurrPlantBase = null;
		}
		if (isFloatPlant && currGrid.CurrFloatPlant == this)
		{
			currGrid.CurrFloatPlant = null;
		}
		currGrid.CheckLadder();
		OnClearGrid();
	}

	protected virtual void OnClearGrid()
	{
	}

	private void CreateFlatDead()
	{
		if (plantType == PlantType.Squash && PlacePlayer == GameManager.Instance.LocalPlayerSave.playerName)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.FlatSquash);
		}
		ZZZ.gameObject.SetActive(value: false);
		Vector3 vector = currGrid.Position;
		base.transform.position = new Vector3(vector.x, vector.y - 0.2f);
		base.transform.localScale = new Vector3(base.transform.localScale.x, 0.4f, base.transform.localScale.z);
		SetAnimSpeed(0f);
		Shadow.enabled = false;
		HurtAudio();
		StartCoroutine(WaitDead(1f));
	}

	private IEnumerator WaitDead(float waitTime)
	{
		if (waitTime != 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}
		SetAnimSpeed(0f);
		currGrid = null;
		StopAllCoroutines();
		CancelInvoke();
		if (CurrMap != null)
		{
			CurrMap.EntityTempt -= Temperature;
		}
		CurrMap = null;
		PlantManager.Instance.PlantDeadRemove(this);
	}

	protected IEnumerator BrightnessEffect(float targetBright, UnityAction fun)
	{
		float currBright = renderers[0].material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return null;
			currBright += 5f * Time.deltaTime;
			SetAllBrightness(currBright);
		}
		while (1f < targetBright)
		{
			yield return null;
			targetBright -= 5f * Time.deltaTime;
			SetAllBrightness(targetBright);
		}
		SetAllBrightness(1f);
		fun?.Invoke();
		BrightCoroutine = null;
	}

	protected IEnumerator BrightnessEffect2(float wantBright, UnityAction fun)
	{
		float currBright = 1f;
		while (currBright < wantBright)
		{
			yield return null;
			if (Time.deltaTime > 0f)
			{
				currBright += Time.deltaTime;
				SetAllBrightness(currBright);
			}
		}
		SetAllBrightness(1f);
		fun?.Invoke();
	}

	protected int GetBulletSortOrder(int line = 0)
	{
		return (animatorSorting.sortingOrder / 200 + line) * 200 + FixedInfo.BulletSort;
	}

	protected int GetAttackValue()
	{
		return (int)((float)attackValue * customAttack);
	}

	public PlantType GetPlantType()
	{
		return plantType;
	}

	public void StartFlash()
	{
		if (!(CarryPlant != null))
		{
			isFlash = true;
			FlashCoroutine = StartCoroutine(flash());
		}
	}

	public void StopFlash()
	{
		if (FlashCoroutine != null)
		{
			StopCoroutine(FlashCoroutine);
		}
		isFlash = false;
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].material.SetColor("_Color", Color.white);
		}
	}

	private IEnumerator flash()
	{
		float a = 1f;
		while (isFlash)
		{
			if (a >= 0.5f)
			{
				while (a > 0.5f)
				{
					yield return null;
					if (isFlash)
					{
						a -= Time.deltaTime * 1f;
						for (int i = 0; i < renderers.Count; i++)
						{
							renderers[i].material.SetColor("_Color", new Color(a, a, a));
						}
					}
				}
			}
			else
			{
				if (!(a <= 0.5f))
				{
					continue;
				}
				while (a < 0.95f)
				{
					yield return null;
					if (isFlash)
					{
						a += Time.deltaTime * 1f;
						for (int j = 0; j < renderers.Count; j++)
						{
							renderers[j].material.SetColor("_Color", new Color(a, a, a));
						}
					}
				}
			}
		}
	}

	protected virtual void SetEyeTex(int EyeType)
	{
	}

	protected virtual void PlayAnim(string name, int layer)
	{
		animator.Play(name, layer, 0f);
	}

	private IEnumerator CloseEyes()
	{
		while (true)
		{
			yield return new WaitForSeconds(2f);
			if (!isSleeping && Random.Range(0, 6) > 4 && !IsStaticState())
			{
				if (haveSpEye)
				{
					SetEyeTex(100);
				}
				isClosingEye = true;
				yield return new WaitForSeconds(0.1f);
				SetEyeTex(1);
				if (EyeREnderer != null)
				{
					EyeREnderer.material.SetTexture("_EyeTex", (eye1 == null) ? null : eye1.texture);
				}
				yield return new WaitForSeconds(0.1f);
				SetEyeTex(2);
				if (EyeREnderer != null)
				{
					EyeREnderer.material.SetTexture("_EyeTex", (eye2 == null) ? null : eye2.texture);
				}
				yield return new WaitForSeconds(0.1f);
				SetEyeTex(1);
				if (EyeREnderer != null)
				{
					EyeREnderer.material.SetTexture("_EyeTex", (eye1 == null) ? null : eye1.texture);
				}
				yield return new WaitForSeconds(0.1f);
				SetEyeTex(0);
				if (EyeREnderer != null)
				{
					EyeREnderer.material.SetTexture("_EyeTex", null);
				}
				isClosingEye = false;
			}
		}
	}

	public void GoAwake(bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			isSleeping = false;
			ZZZ.gameObject.SetActive(value: false);
			SetEyeTex(0);
			if (EyeREnderer != null)
			{
				EyeREnderer.material.SetTexture("_EyeTex", null);
			}
			GoAwakeSpecial();
			WeatherChangeEvent();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 2;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void GoSleep(bool synClient = false)
	{
		if (!PlantManager.Instance.PlantDontSleep && (synClient || !GameManager.Instance.isClient))
		{
			isSleeping = true;
			ZZZ.gameObject.SetActive(value: true);
			SetEyeTex(2);
			if (EyeREnderer != null)
			{
				EyeREnderer.material.SetTexture("_EyeTex", (eye2 == null) ? null : eye2.texture);
			}
			GoSleepSpecial();
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 0;
				synItem.SynCode[1] = 1;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void GameOverFakeDeath()
	{
		SetAnimSpeed(0f);
		GameOverSpecial();
		CancelInvoke();
		StopAllCoroutines();
	}

	public virtual void OpenBlackAndWhite(bool isOpen)
	{
		if (isOpen)
		{
			for (int i = 0; i < renderers.Count; i++)
			{
				renderers[i].material.SetInt("_OpenGray", 1);
			}
		}
		else
		{
			for (int j = 0; j < renderers.Count; j++)
			{
				renderers[j].material.SetInt("_OpenGray", 0);
			}
		}
	}

	protected virtual float HandleHurt(float hurt, bool isFlat)
	{
		float num = 1f;
		if (isSleeping)
		{
			num++;
		}
		if (isIcetrap)
		{
			num++;
		}
		if (currGrid.CoverNum <= 0)
		{
			num += 0.2f * (float)SkyManager.Instance.HailScale;
		}
		return hurt * num;
	}

	protected virtual void HurtAudio()
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

	protected bool NormalProduceCondition()
	{
		if (isSleeping)
		{
			return false;
		}
		if (isIcetrap)
		{
			return false;
		}
		if (!LVManager.Instance.GameIsStart)
		{
			return false;
		}
		if (LV.Instance.CurrLVType == LVType.IZombie)
		{
			return false;
		}
		if (LVManager.Instance.BootyIsAppeared)
		{
			return false;
		}
		if (LVManager.Instance.LastStandNoCd)
		{
			return false;
		}
		return true;
	}

	protected bool NormalDontAttackCondition()
	{
		if (isSleeping)
		{
			return true;
		}
		if (isIcetrap)
		{
			return true;
		}
		if (currGrid == null)
		{
			return true;
		}
		if (!LVManager.Instance.GameIsStart)
		{
			return true;
		}
		return false;
	}

	public void BurnEvent()
	{
		ClearIce();
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
				synItem.SynCode[1] = 9;
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
				synItem.SynCode[1] = 10;
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
				synItem.SynCode[1] = 11;
				synItem.Twofloat = new Vector2(value, 0f);
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	protected virtual void OnBurnEvent()
	{
	}

	protected virtual void OnInitForPlace()
	{
	}

	protected virtual void OnInitForCreate()
	{
	}

	protected virtual void OnInitForAlmanac()
	{
	}

	protected virtual void OnInitForAll()
	{
	}

	protected virtual void OnIceEvent()
	{
	}

	protected virtual void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
	}

	protected virtual void DeadEvent()
	{
	}

	protected virtual void DeadrattleEvent()
	{
	}

	protected virtual void GoAwakeSpecial()
	{
	}

	protected virtual void GoSleepSpecial()
	{
	}

	protected virtual void GameOverSpecial()
	{
	}

	protected virtual void ResetAnimSpeedEvent(float speed)
	{
	}

	public virtual void PlaceOverEvent()
	{
	}

	public virtual void WeatherChangeEvent()
	{
	}

	public virtual bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		return true;
	}

	protected virtual void OwnerSetColor(Color color)
	{
	}

	protected virtual void OwnerSetBrightness(float value)
	{
	}

	protected virtual void ChangeFacingEvent()
	{
	}
}
