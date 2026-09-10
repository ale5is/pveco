using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class JacksonZombie : ZombieBase
{
	public SpriteRenderer spotlight;

	public Light2D SpotLight;

	public Light2D CircleLight;

	private int walkGridNum;

	private bool FirstPoint;

	private bool isPointing;

	public bool ClientCanMove;

	private int walkNum;

	private int armRaiseNum;

	private float ownerSpeed;

	private float ownerAnSpeed;

	private Transform anim;

	private List<Grid> Spawngrids = new List<Grid>();

	private List<DancerZombie> dancers = new List<DancerZombie>();

	private List<DancerZombie> EatingDancers = new List<DancerZombie>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.JacksonZombie;

	protected override float AnToSpeed => ownerAnSpeed;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 1350;

	protected override int CriticalHp => 70;

	private float CreateX => base.IsFacingLeft ? (-2) : 2;

	public override void InitZombieHpState()
	{
		walkNum = 0;
		armRaiseNum = 0;
		walkGridNum = 0;
		FirstPoint = false;
		isPointing = false;
		ClientCanMove = true;
		dancers.Clear();
		Spawngrids.Clear();
		EatingDancers.Clear();
		spotlight.gameObject.SetActive(value: false);
		anim = animator.transform;
		if (LVManager.Instance.CurrLVState == LVState.Fighting)
		{
			ownerSpeed = 1.5f;
			ownerAnSpeed = 2f;
			ToTurn(isLeft: false);
			spotlight.sortingOrder = Sorting.sortingOrder + 1;
		}
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (EatingDancers.Count == 0)
			{
				for (int j = 0; j < dancers.Count; j++)
				{
					dancers[j].KingEat(isEat: false);
				}
			}
			SetAnimatorChange(11);
			break;
		case ZombieState.Attack:
			if (FirstPoint)
			{
				for (int i = 0; i < dancers.Count; i++)
				{
					dancers[i].KingEat(isEat: true);
				}
				SetAnimatorChange(21);
			}
			else
			{
				FirstPoint = true;
				StopMoonWalk();
			}
			break;
		}
	}

	private void DismissDancers()
	{
		for (int i = 0; i < dancers.Count; i++)
		{
			dancers[i].LeaveKing();
		}
		EatingDancers.Clear();
		dancers.Clear();
		ResetEating();
	}

	private void ToTurn()
	{
		anim.localScale = new Vector3(0f - anim.localScale.x, anim.localScale.y);
		anim.localPosition = new Vector3(0f - anim.localPosition.x, anim.localPosition.y);
	}

	private void ToTurn(bool isLeft)
	{
		float num = Mathf.Abs(anim.localScale.x);
		float num2 = Mathf.Abs(anim.localPosition.x);
		if (isLeft)
		{
			num2 = 0f - num2;
		}
		else
		{
			num = 0f - num;
		}
		anim.localScale = new Vector3(num, anim.localScale.y);
		anim.localPosition = new Vector3(num2, anim.localPosition.y);
	}

	private void StopMoonWalk()
	{
		ownerSpeed = 4f;
		ownerAnSpeed = 4f;
		base.Speed = DefSpeed;
		ToTurn(isLeft: true);
		spotlight.gameObject.SetActive(value: true);
		StartCoroutine(StartSpotlight());
		CheckDancers();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.danceMusic, base.transform.position);
	}

	private void CheckDancers()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		float x = base.transform.position.x + CreateX;
		List<Grid> columnGrids = MapManager.Instance.GetColumnGrids(base.CurrMap, x);
		Spawngrids = new List<Grid>(columnGrids);
		for (int i = 0; i < columnGrids.Count; i++)
		{
			for (int j = 0; j < dancers.Count; j++)
			{
				if (dancers[j].CurrLine == columnGrids[i].Point.y)
				{
					Spawngrids.Remove(columnGrids[i]);
					break;
				}
			}
		}
		if (Spawngrids.Count > 0)
		{
			isPointing = true;
			anCanMove = false;
			SetAnimatorChange(41);
			base.dontChangeState = true;
			for (int k = 0; k < dancers.Count; k++)
			{
				dancers[k].KingEat(isEat: true);
			}
			ServerSendSyn(0);
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			if (!FirstPoint)
			{
				StopMoonWalk();
			}
			FirstPoint = true;
			isPointing = true;
			ToTurn(isLeft: true);
			anCanMove = false;
			SetAnimatorChange(41);
			base.dontChangeState = true;
		}
		else if (syn.SynCode[1] == 1)
		{
			ToTurn(isLeft: true);
			base.dontChangeState = false;
			base.State = ZombieState.Walk;
		}
		else if (syn.SynCode[1] == 2)
		{
			anCanMove = false;
			base.dontChangeState = true;
			animator.SetInteger("Change", 42);
		}
		else if (syn.SynCode[1] == 3)
		{
			if (syn.SynCode[2] == 1)
			{
				ClientCanMove = false;
			}
			else if (syn.SynCode[2] == 2)
			{
				ClientCanMove = true;
			}
			anCanMove = ClientCanMove;
		}
	}

	private void CreateDancers()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		float x = base.transform.position.x + CreateX;
		for (int i = 0; i < Spawngrids.Count; i++)
		{
			DancerZombie dancerZombie = (DancerZombie)ZombieManager.Instance.OutGround(ZombieType.DancerZombie, new Vector3(x, Spawngrids[i].Position.y), PlacePlayer, needArm: false, isHypno, needHypnoPurple);
			if (dancerZombie == null)
			{
				break;
			}
			dancerZombie.KingInit(this);
			dancers.Add(dancerZombie);
			dancers[i].KingEat(isEat: true);
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		walkGridNum++;
		if (!FirstPoint && walkGridNum >= 3)
		{
			FirstPoint = true;
			StopMoonWalk();
		}
	}

	public void DancerDead(DancerZombie dancer)
	{
		dancers.Remove(dancer);
		EatingDancers.Remove(dancer);
		ResetEating();
	}

	public void DancerEat(bool isEat, DancerZombie dancer)
	{
		if (isEat)
		{
			if (!EatingDancers.Contains(dancer))
			{
				EatingDancers.Add(dancer);
			}
		}
		else
		{
			EatingDancers.Remove(dancer);
		}
		ResetEating();
	}

	private void ResetEating()
	{
		int code = 0;
		if (EatingDancers.Count > 0)
		{
			code = 1;
			anCanMove = false;
			for (int i = 0; i < dancers.Count; i++)
			{
				dancers[i].KingEat(isEat: true);
			}
		}
		else if (base.State != ZombieState.Attack && !isPointing)
		{
			code = 2;
			for (int j = 0; j < dancers.Count; j++)
			{
				dancers[j].KingEat(isEat: false);
			}
		}
		ServerSendSyn(3, code);
	}

	private IEnumerator StartSpotlight()
	{
		while (base.Hp > 0)
		{
			spotlight.color = new Color32(byte.MaxValue, 127, 127, 180);
			SpotLight.color = new Color32(byte.MaxValue, 127, 127, byte.MaxValue);
			CircleLight.color = new Color32(byte.MaxValue, 178, 127, byte.MaxValue);
			if (base.Hp > 0)
			{
				yield return new WaitForSeconds(1f);
				spotlight.color = new Color32(135, byte.MaxValue, 120, 180);
				SpotLight.color = new Color32(135, byte.MaxValue, 120, byte.MaxValue);
				CircleLight.color = new Color32(135, byte.MaxValue, 120, byte.MaxValue);
				if (base.Hp > 0)
				{
					yield return new WaitForSeconds(1f);
					spotlight.color = new Color32(120, 180, byte.MaxValue, 180);
					SpotLight.color = new Color32(120, 180, byte.MaxValue, byte.MaxValue);
					CircleLight.color = new Color32(100, 240, byte.MaxValue, byte.MaxValue);
					if (base.Hp > 0)
					{
						yield return new WaitForSeconds(1f);
						spotlight.color = new Color32(byte.MaxValue, byte.MaxValue, 120, 180);
						SpotLight.color = new Color32(byte.MaxValue, byte.MaxValue, 120, byte.MaxValue);
						CircleLight.color = new Color32(byte.MaxValue, byte.MaxValue, 120, byte.MaxValue);
						yield return new WaitForSeconds(1f);
						continue;
					}
					break;
				}
				break;
			}
			break;
		}
	}

	protected override void HypnoEvent()
	{
		DismissDancers();
	}

	public override void SpecialAnimEvent1()
	{
		ToTurn();
		if (GameManager.Instance.isClient)
		{
			return;
		}
		armRaiseNum++;
		anCanMove = false;
		if (armRaiseNum >= 4)
		{
			armRaiseNum = 0;
			ToTurn(isLeft: true);
			base.dontChangeState = false;
			base.State = ZombieState.Walk;
			for (int i = 0; i < dancers.Count; i++)
			{
				dancers[i].KingDanceSyn(isArmraise: false);
			}
			ServerSendSyn(1);
		}
	}

	public override void SpecialAnimEvent2()
	{
		CreateDancers();
	}

	public override void SpecialAnimEvent3()
	{
		if (EatingDancers.Count == 0)
		{
			for (int i = 0; i < dancers.Count; i++)
			{
				dancers[i].KingEat(isEat: false);
			}
		}
		isPointing = false;
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
	}

	public override void SpecialAnimEvent4()
	{
		ToTurn(isLeft: true);
		if (GameManager.Instance.isClient)
		{
			return;
		}
		walkNum++;
		if (walkNum == 2)
		{
			CheckDancers();
		}
		if (walkNum >= 6)
		{
			walkNum = 0;
			anCanMove = false;
			base.dontChangeState = true;
			SetAnimatorChange(42);
			for (int i = 0; i < dancers.Count; i++)
			{
				dancers[i].KingDanceSyn(isArmraise: true);
			}
			ServerSendSyn(2);
		}
	}

	public override void SpecialAnimEvent5()
	{
		if (GameManager.Instance.isClient)
		{
			if (ClientCanMove && !isPointing)
			{
				anCanMove = true;
			}
		}
		else if (EatingDancers.Count == 0)
		{
			anCanMove = true;
		}
	}

	public override void SpecialAnimEvent6()
	{
		CheckDancers();
	}

	public override void ZombieOnDead(bool dropItem)
	{
		DismissDancers();
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 500f * base.HpScale)
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
}
