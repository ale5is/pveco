using UnityEngine;

public class StarfruitZombie : PlantZombie
{
	public Transform creatBulletPos;

	protected override GameObject Prefab => GameManager.Instance.GameConf.StarfruitZombie;

	protected override int attackValue2 => 20;

	protected override void PlantZombieInit()
	{
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		if (!NormalDontAttackCondition())
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	public override void SpecialAnimEvent1()
	{
		CreateStar(isCheck: false);
		SetAnimChange(12);
	}

	private void CreateStar(bool isCheck)
	{
		Star component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
		component.Init(attackValue2, creatBulletPos.position, -1, Vector2.up, null, isCheck, !isHypno);
		component.transform.SetParent(null);
		Star component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
		component2.Init(attackValue2, creatBulletPos.position, -1, Vector2.down, null, isCheck, !isHypno);
		component2.transform.SetParent(null);
		Star component3 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
		if (base.IsFacingLeft)
		{
			component3.Init(attackValue2, creatBulletPos.position, base.CurrGrid.Point.y, Vector2.right, null, isCheck, !isHypno);
		}
		else
		{
			component3.Init(attackValue2, creatBulletPos.position, base.CurrGrid.Point.y, Vector2.left, null, isCheck, !isHypno);
		}
		component3.transform.SetParent(null);
		Star component4 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
		Vector2 dirc = new Vector2(1.5f, 1f);
		if (base.IsFacingLeft)
		{
			dirc = new Vector2(-1.5f, 1f);
		}
		component4.Init(attackValue2, creatBulletPos.position, -1, dirc, null, isCheck, !isHypno);
		component4.transform.SetParent(null);
		Star component5 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Star).GetComponent<Star>();
		dirc = new Vector2(1.5f, -1f);
		if (base.IsFacingLeft)
		{
			dirc = new Vector2(-1.5f, -1f);
		}
		component5.Init(attackValue2, creatBulletPos.position, -1, dirc, null, isCheck, !isHypno);
		component5.transform.SetParent(null);
	}
}
