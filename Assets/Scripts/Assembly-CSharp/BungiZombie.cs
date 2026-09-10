using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class BungiZombie : ZombieBase
{
	public BungiZombieType Type;

	public float CreateY;

	public Grid SpawnGrid;

	public Sprite NormalTarget;

	public Sprite BlackTarget;

	public SpriteRenderer DioHair;

	public SpriteRenderer NormalCord;

	public SpriteRenderer IronCord;

	public List<Sprite> NormalSkin = new List<Sprite>();

	public List<Sprite> HeavySkin = new List<Sprite>();

	public List<SpriteRenderer> SkinPart = new List<SpriteRenderer>();

	public List<SpriteRenderer> HeavyPart = new List<SpriteRenderer>();

	private PlantBase GetPlantBase;

	private SpriteRenderer Target;

	private bool isHitUmbrella;

	private bool dropStuffState;

	private ZombieBase Goalzombie;

	private UnityAction overAction;

	private Cage ACage;

	protected override GameObject Prefab => GameManager.Instance.GameConf.BungiZombie;

	protected override float AnToSpeed => 1f;

	protected override float DefSpeed => 1f;

	protected override float attackValue => 0f;

	public override int MaxHP => 500;

	public override bool CanBlowBack => false;

	public override void InitZombieHpState()
	{
		dropStuffState = false;
		if (IsOVer)
		{
			return;
		}
		BaseTransform.Find("BungeeCord").localScale = Vector3.one;
		NormalCord.enabled = false;
		IronCord.enabled = false;
		List<Sprite> list = NormalSkin;
		DioHair.enabled = false;
		switch (Type)
		{
		case BungiZombieType.Normal:
		{
			NormalCord.enabled = true;
			for (int j = 0; j < HeavyPart.Count; j++)
			{
				HeavyPart[j].enabled = false;
			}
			animator.transform.localScale = Vector3.one;
			break;
		}
		case BungiZombieType.Heavy:
		{
			list = HeavySkin;
			IronCord.enabled = true;
			for (int i = 0; i < HeavyPart.Count; i++)
			{
				HeavyPart[i].enabled = true;
			}
			animator.transform.localScale = new Vector3(1.2f, 1.2f);
			break;
		}
		}
		for (int k = 0; k < SkinPart.Count; k++)
		{
			SkinPart[k].sprite = list[k];
		}
		if (LVManager.Instance.GameIsStart)
		{
			base.State = ZombieState.Idel;
			isHitUmbrella = false;
			anCanMove = false;
			base.dontChangeState = true;
			Shadow.enabled = false;
			base.collider2d.enabled = false;
			canIce = false;
			canButter = false;
			if (!GameManager.Instance.isClient)
			{
				CreateY = MapManager.Instance.GetMapYHighest(SpawnPos) + 2.1f;
				if (SpawnGrid == null)
				{
					SpawnGrid = MapManager.Instance.GetGridByWorldPos(SpawnPos);
				}
				base.transform.position = new Vector2(SpawnGrid.Position.x, CreateY);
				base.CurrGrid = SpawnGrid;
				base.CurrGrid.isZombieSigned = true;
				if (SpawnGrid.CurrPlantBase != null)
				{
					Sorting.sortingOrder = SpawnGrid.CurrPlantBase.SortingOrder - 1;
				}
				NormalCord.sortingOrder = Sorting.sortingOrder - 1;
				IronCord.sortingOrder = Sorting.sortingOrder - 1;
				StartCoroutine(MoveDown());
				StartCoroutine(SetTarget());
				animator.Play("drop");
			}
			if (Type == BungiZombieType.Heavy)
			{
				ACage = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Cage).GetComponent<Cage>();
				ACage.transform.position = base.transform.position;
				ACage.transform.SetParent(animator.transform);
				animator.Play("hold");
				ACage.SetSort(5);
			}
		}
		else
		{
			base.State = ZombieState.Idel;
			base.transform.position = new Vector3(base.transform.position.x, base.CurrGrid.Position.y + 0.2f, base.transform.position.z);
			NormalCord.sortingOrder = Sorting.sortingOrder - 1;
			IronCord.sortingOrder = Sorting.sortingOrder - 1;
		}
	}

	protected override void CreateInitZombie()
	{
		NormalCord.transform.localScale = Vector3.zero;
		IronCord.transform.localScale = Vector3.zero;
	}

	public override void ServerInitInfo()
	{
		ServerSendSyn(2, SpawnGrid.Position);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] != 2)
		{
			return;
		}
		if (syn.SynCode[1] == 1)
		{
			if (SpawnGrid.CurrPlantBase != null)
			{
				if (SpawnGrid.CurrPlantBase.CarryPlant != null)
				{
					GetPlantBase = SpawnGrid.CurrPlantBase.CarryPlant;
				}
				else if (SpawnGrid.CurrPlantBase.CanCarryOtherPlant && SpawnGrid.CurrPlantBase.ProtectPlant != null)
				{
					GetPlantBase = SpawnGrid.CurrPlantBase.ProtectPlant;
				}
				else
				{
					GetPlantBase = SpawnGrid.CurrPlantBase;
				}
			}
			if (GetPlantBase != null)
			{
				GetPlantBase.Dead(isFlat: false, 10f, synClient: true, deadRattle: false);
				GetPlantBase.transform.SetParent(base.transform);
			}
		}
		else if (syn.SynCode[1] == 2)
		{
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
			{
				syn.Twofloat = MyTool.ReverseX(syn.Twofloat);
			}
			SpawnGrid = MapManager.Instance.GetGridByWorldPos(syn.Twofloat);
			CreateY = MapManager.Instance.GetMapYHighest(SpawnGrid.Position) + 1f;
			base.transform.position = new Vector2(SpawnGrid.Position.x, CreateY);
			base.CurrGrid = SpawnGrid;
			base.CurrGrid.isZombieSigned = true;
			if (SpawnGrid.CurrPlantBase != null)
			{
				Sorting.sortingOrder = SpawnGrid.CurrPlantBase.SortingOrder - 1;
			}
			NormalCord.sortingOrder = Sorting.sortingOrder - 1;
			IronCord.sortingOrder = Sorting.sortingOrder - 1;
			StartCoroutine(MoveDown());
			StartCoroutine(SetTarget());
			animator.Play("drop");
			Debug.Log(LVManager.Instance.CurrLVState);
			_ = LVManager.Instance.CurrLVState;
			_ = 1;
		}
	}

	protected override void CheckState()
	{
		if (ACage == null && !dropStuffState)
		{
			animator.Play("idle1");
		}
		if (base.State == ZombieState.Dead)
		{
			DirectDead(canDropItem: true, 0f);
		}
	}

	private IEnumerator MoveDown()
	{
		if (ACage != null)
		{
			animator.Play("hold");
		}
		yield return new WaitForSeconds(1f);
		float high = 0.4f;
		if (dropStuffState && Type == BungiZombieType.Heavy)
		{
			high = 1.5f;
		}
		switch (Random.Range(0, 3))
		{
		case 0:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bungee_scream1, SpawnGrid.Position);
			break;
		case 1:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bungee_scream2, SpawnGrid.Position);
			break;
		default:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bungee_scream3, SpawnGrid.Position);
			break;
		}
		yield return new WaitForSeconds(1f);
		while (base.transform.position.y > SpawnGrid.Position.y + 0.5f + high)
		{
			yield return null;
			base.transform.Translate(new Vector2(0f, -9f) * Time.deltaTime);
		}
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(MapManager.Instance.GetGridByWorldPos(SpawnGrid.Position), 1);
		for (int i = 0; i < aroundGrid.Count; i++)
		{
			if (!(aroundGrid[i].CurrPlantBase != null))
			{
				continue;
			}
			float num = 0f;
			if (Type == BungiZombieType.Heavy)
			{
				num = 500f;
			}
			if (aroundGrid[i].CurrPlantBase is Umbrellaleaf)
			{
				if (aroundGrid[i].CurrPlantBase.GetComponent<Umbrellaleaf>().Block(num))
				{
					isHitUmbrella = true;
				}
			}
			else if (aroundGrid[i].CurrPlantBase.CarryPlant is Umbrellaleaf && aroundGrid[i].CurrPlantBase.CarryPlant.GetComponent<Umbrellaleaf>().Block(num))
			{
				isHitUmbrella = true;
			}
		}
		if (!isHitUmbrella)
		{
			while (base.transform.position.y > SpawnGrid.Position.y + high)
			{
				yield return null;
				base.transform.Translate(new Vector2(0f, -9f) * Time.deltaTime);
			}
			base.collider2d.enabled = true;
			animator.SetInteger("Change", 41);
			base.dontChangeState = false;
			canIce = true;
			canButter = true;
			if (dropStuffState)
			{
				StartCoroutine(MoveUp());
				if (overAction != null)
				{
					overAction();
				}
				Goalzombie.transform.SetParent(null);
				yield return new WaitForSeconds(0.1f);
				Goalzombie.transform.SetParent(base.transform.parent);
				Goalzombie = null;
			}
			else if (ACage != null)
			{
				ACage.PlaceInit(SpawnGrid);
				ACage = null;
				StartCoroutine(MoveUp());
			}
			else
			{
				yield return new WaitForSeconds(3f);
				animator.SetInteger("Change", 42);
			}
		}
		else
		{
			StartCoroutine(MoveUp());
		}
	}

	private IEnumerator MoveUp()
	{
		canIce = false;
		canButter = false;
		base.collider2d.enabled = false;
		base.dontChangeState = true;
		while (base.transform.position.y < CreateY)
		{
			yield return null;
			base.transform.Translate(new Vector2(0f, 16f) * Time.deltaTime);
		}
		base.CurrGrid.isZombieSigned = false;
		if (GetPlantBase != null)
		{
			GetPlantBase.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
		}
		if (Goalzombie != null)
		{
			Goalzombie.DirectDead(canDropItem: false, 0f);
		}
		DirectDead(canDropItem: false, 0f);
	}

	private IEnumerator SetTarget()
	{
		Target = Object.Instantiate(GameManager.Instance.GameConf.BungeeTarget).GetComponent<SpriteRenderer>();
		Target.sortingOrder = Sorting.sortingOrder + 100;
		if (Type == BungiZombieType.Heavy)
		{
			Target.sprite = BlackTarget;
		}
		else
		{
			Target.sprite = NormalTarget;
		}
		Target.transform.position = base.transform.position;
		Target.transform.SetParent(base.CurrMap.transform);
		while (Target.transform.position.y > SpawnGrid.Position.y - 0.1f)
		{
			yield return null;
			Target.transform.Translate(new Vector2(0f, -14f) * Time.deltaTime);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.grassstep, Target.transform.position);
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (Target != null)
		{
			Object.Destroy(Target.gameObject);
		}
		SpawnGrid = null;
	}

	public override void SpecialAnimEvent1()
	{
		if (!GameManager.Instance.isClient)
		{
			if (SpawnGrid.CurrPlantBase != null)
			{
				if (SpawnGrid.CurrPlantBase.CarryPlant != null)
				{
					GetPlantBase = SpawnGrid.CurrPlantBase.CarryPlant;
				}
				else if (SpawnGrid.CurrPlantBase.CanCarryOtherPlant && SpawnGrid.CurrPlantBase.ProtectPlant != null)
				{
					GetPlantBase = SpawnGrid.CurrPlantBase.ProtectPlant;
				}
				else
				{
					GetPlantBase = SpawnGrid.CurrPlantBase;
				}
			}
			if (GetPlantBase != null)
			{
				ServerSendSyn(1);
				GetPlantBase.Dead(isFlat: false, 10f, synClient: true, deadRattle: false);
				GetPlantBase.transform.SetParent(base.transform);
			}
		}
		StartCoroutine(MoveUp());
		if (Target != null)
		{
			Target.transform.SetParent(base.transform);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.floop, SpawnGrid.Position);
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
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

	public void DropInit(ZombieBase zombie, UnityAction action)
	{
		if (Type == BungiZombieType.Heavy)
		{
			zombie.transform.position += new Vector3(0f, -1.1f);
		}
		if (zombie is RoadrollerZombie)
		{
			DioHair.enabled = true;
		}
		if (ACage != null)
		{
			ACage.DestroyThis();
			ACage = null;
		}
		Goalzombie = zombie;
		overAction = action;
		dropStuffState = true;
		animator.Play("hold");
		zombie.transform.SetParent(animator.transform);
		if (Type == BungiZombieType.Heavy)
		{
			zombie.SetSortingOrder(20);
		}
		else
		{
			zombie.SetSortingOrder(5);
		}
	}
}
