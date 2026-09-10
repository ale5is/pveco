using UnityEngine;

public class Starfruit : PlantBase
{
	private bool NeedShoot;

	protected override int attackValue => 20;

	public override float MaxHp => 300f;

	protected override void OnInitForPlace()
	{
		NeedShoot = false;
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		if (NeedShoot)
		{
			NeedShoot = false;
			SetAnimChange(11);
		}
		else if (!NormalDontAttackCondition() && ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno, needCapsule: true).Count > 0)
		{
			CreateStar(isCheck: true);
		}
		return NeedShoot;
	}

	public override void SpecialAnimEvent1()
	{
		CreateStar(isCheck: false);
		SetAnimChange(12);
	}

	public void CheckZombieResult()
	{
		NeedShoot = true;
	}

	private void CreateStar(bool isCheck)
	{
		if (!NormalDontAttackCondition())
		{
			Star component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			component.Init(GetAttackValue(), base.transform.position, -1, Vector2.up, this, isCheck, isHypno);
			component.transform.SetParent(null);
			Star component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			component2.Init(GetAttackValue(), base.transform.position, -1, Vector2.down, this, isCheck, isHypno);
			component2.transform.SetParent(null);
			Star component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			if (base.IsFacingLeft)
			{
				component3.Init(GetAttackValue(), base.transform.position, base.currGrid.Point.y, Vector2.right, this, isCheck, isHypno);
			}
			else
			{
				component3.Init(GetAttackValue(), base.transform.position, base.currGrid.Point.y, Vector2.left, this, isCheck, isHypno);
			}
			component3.transform.SetParent(null);
			Star component4 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			Vector2 dirc = new Vector2(1.5f, 1f);
			if (base.IsFacingLeft)
			{
				dirc = new Vector2(-1.5f, 1f);
			}
			component4.Init(GetAttackValue(), base.transform.position, -1, dirc, this, isCheck, isHypno);
			component4.transform.SetParent(null);
			Star component5 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
			dirc = new Vector2(1.5f, -1f);
			if (base.IsFacingLeft)
			{
				dirc = new Vector2(-1.5f, -1f);
			}
			component5.Init(GetAttackValue(), base.transform.position, -1, dirc, this, isCheck, isHypno);
			component5.transform.SetParent(null);
		}
	}
}
