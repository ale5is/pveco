using System.Collections.Generic;
using UnityEngine;

public class Glowstarfruit : PlantBase
{
	private bool NeedShoot;

	private bool NeedShootTurned;

	private bool IsTurned;

	private bool CanBigStar;

	private bool HaveBigStar;

	public bool TurnedChange;

	private BigGreenStar BigStar;

	public ParticleSystem Firefly;

	public override float MaxHp => 300f;

	protected override int attackValue => 40;

	public override PlantType BasePlant => PlantType.Starfruit;

	public override int BasePlantSunNum => 125;

	protected override List<string> DontPlayAnim => new List<string> { "anim_idle", "anim_idle2" };

	protected override void OnInitForCreate()
	{
		Firefly.gameObject.SetActive(value: false);
	}

	protected override void OnInitForPlace()
	{
		BigStar = null;
		HaveBigStar = true;
		CanBigStar = false;
		NeedShoot = false;
		NeedShootTurned = false;
		IsTurned = false;
		Firefly.gameObject.SetActive(value: true);
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		CheckAttack();
		if (CanBigStar)
		{
			CanBigStar = false;
			SetAnimChange(13);
			return true;
		}
		if (NeedShoot || NeedShootTurned)
		{
			if (NeedShoot && !IsTurned)
			{
				NeedShoot = false;
				NeedShootTurned = false;
				SetAnimChange(11);
				return true;
			}
			if (NeedShootTurned && IsTurned)
			{
				NeedShoot = false;
				NeedShootTurned = false;
				SetAnimChange(11);
				return true;
			}
			if (NeedShoot && IsTurned)
			{
				IsTurned = false;
				SetAnimChange(15);
			}
			else if (NeedShootTurned && !IsTurned)
			{
				IsTurned = true;
				SetAnimChange(14);
			}
		}
		else if (!NormalDontAttackCondition() && ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno, needCapsule: true).Count > 0)
		{
			CreateStar(isCheck: true, TurnedChange);
		}
		return false;
	}

	public override void SpecialAnimEvent1()
	{
		CreateStar(isCheck: false, IsTurned);
		SetAnimChange(12);
	}

	public override void SpecialAnimEvent2()
	{
		BigStar = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BigGreenStar).GetComponent<BigGreenStar>();
		if (base.IsFacingLeft)
		{
			BigStar.Init(100, base.transform.position, base.currGrid.Point.y, Vector2.left, isHypno);
		}
		else
		{
			BigStar.Init(100, base.transform.position, base.currGrid.Point.y, Vector2.right, isHypno);
		}
	}

	public override void SpecialAnimEvent3()
	{
		if (BigStar != null)
		{
			BigStar.GoMove();
		}
		BigStar = null;
		IsTurned = false;
		HaveBigStar = false;
		HaveBigStar = false;
		SetAnimChange(12);
		StartCoroutine(DoFuncWait(() =>
		{
			HaveBigStar = true;
		}, 10f));
	}

	private void CheckAttack()
	{
		if (!HaveBigStar)
		{
			CanBigStar = false;
		}
		else if (base.currGrid != null && !isSleeping)
		{
			ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
			PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if ((zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < 4.9f) || (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < 4.9f))
			{
				CanBigStar = true;
			}
			else
			{
				CanBigStar = false;
			}
		}
	}

	public void CheckZombieResult(bool isTurned)
	{
		if (isTurned)
		{
			NeedShootTurned = true;
		}
		else
		{
			NeedShoot = true;
		}
	}

	private void CreateStar(bool isCheck, bool isTurned)
	{
		if (NormalDontAttackCondition())
		{
			return;
		}
		GreenStar component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		component.Init(GetAttackValue(), base.transform.position, -1, Vector2.up, this, isCheck, isHypno);
		component.transform.SetParent(null);
		GreenStar component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		component2.Init(GetAttackValue(), base.transform.position, -1, Vector2.down, this, isCheck, isHypno);
		component2.transform.SetParent(null);
		if (isCheck)
		{
			if (isTurned)
			{
				CheckTurned(isCheck);
			}
			else
			{
				CheckNormal(isCheck);
			}
		}
		else if (IsTurned)
		{
			CheckTurned(isCheck);
		}
		else
		{
			CheckNormal(isCheck);
		}
		TurnedChange = !TurnedChange;
	}

	private void CheckTurned(bool isCheck)
	{
		GreenStar component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		if (base.IsFacingLeft)
		{
			component.Init(GetAttackValue(), base.transform.position, base.currGrid.Point.y, Vector2.left, this, isCheck, isHypno);
		}
		else
		{
			component.Init(GetAttackValue(), base.transform.position, base.currGrid.Point.y, Vector2.right, this, isCheck, isHypno);
		}
		component.transform.SetParent(null);
		GreenStar component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		Vector2 dirc = new Vector2(-1.5f, 1f);
		if (base.IsFacingLeft)
		{
			dirc = new Vector2(1.5f, 1f);
		}
		component2.Init(GetAttackValue(), base.transform.position, -1, dirc, this, isCheck, isHypno);
		component2.transform.SetParent(null);
		GreenStar component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		dirc = new Vector2(-1.5f, -1f);
		if (base.IsFacingLeft)
		{
			dirc = new Vector2(1.5f, -1f);
		}
		component3.Init(GetAttackValue(), base.transform.position, -1, dirc, this, isCheck, isHypno);
		component3.transform.SetParent(null);
	}

	private void CheckNormal(bool isCheck)
	{
		GreenStar component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		if (base.IsFacingLeft)
		{
			component.Init(GetAttackValue(), base.transform.position, base.currGrid.Point.y, Vector2.right, this, isCheck, isHypno);
		}
		else
		{
			component.Init(GetAttackValue(), base.transform.position, base.currGrid.Point.y, Vector2.left, this, isCheck, isHypno);
		}
		component.transform.SetParent(null);
		GreenStar component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		Vector2 dirc = new Vector2(1.5f, 1f);
		if (base.IsFacingLeft)
		{
			dirc = new Vector2(-1.5f, 1f);
		}
		component2.Init(GetAttackValue(), base.transform.position, -1, dirc, this, isCheck, isHypno);
		component2.transform.SetParent(null);
		GreenStar component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.GreenStar).GetComponent<GreenStar>();
		dirc = new Vector2(1.5f, -1f);
		if (base.IsFacingLeft)
		{
			dirc = new Vector2(-1.5f, -1f);
		}
		component3.Init(GetAttackValue(), base.transform.position, -1, dirc, this, isCheck, isHypno);
		component3.transform.SetParent(null);
	}

	protected override void DeadEvent()
	{
		if (BigStar != null)
		{
			BigStar.GoMove();
		}
	}
}
