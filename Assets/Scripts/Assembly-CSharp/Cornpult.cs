using SocketSave;
using UnityEngine;

public class Cornpult : PlantBase
{
	public SpriteRenderer ButterRenderer;

	private int ButterNum;

	public override float MaxHp => 300f;

	protected override int attackValue => 40;

	protected override void OnInitForAll()
	{
		ButterNum = 0;
		ButterRenderer.enabled = false;
		SetSpriteEnable(ButterRenderer.transform.name, enable: false);
	}

	protected override void OnInitForPlace()
	{
		StartActionCD(2.9f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreateBullet();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		PlantBase plantBase = null;
		ZombieBase zombieByLineMinDisCanNoCollGet = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieByLineMinDisCanNoCollGet == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		}
		if (zombieByLineMinDisCanNoCollGet != null || plantBase != null)
		{
			SetAnimChange(11);
			if (!GameManager.Instance.isClient)
			{
				if (Random.Range(0, 4) > 2)
				{
					ButterNum++;
					ButterRenderer.enabled = true;
					SetSpriteEnable(ButterRenderer.transform.name, enable: true);
				}
				else
				{
					ButterNum = 0;
				}
				if (ButterNum >= 5)
				{
					AcvmentManager.Instance.GetAchievement(Acvname.LuckCorn5, PlacePlayer);
				}
			}
			if (ButterRenderer.enabled)
			{
				ServerSendSyn(0);
			}
			return true;
		}
		return false;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			ButterRenderer.enabled = true;
			SetSpriteEnable(ButterRenderer.transform.name, enable: true);
		}
	}

	private void CreateBullet()
	{
		if (NormalDontAttackCondition())
		{
			return;
		}
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
		}
		PlantBase plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		ZombieBase zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieBase != null && plantBase != null)
		{
			if (Mathf.Abs(plantBase.transform.position.x - base.transform.position.x) >= Mathf.Abs(zombieBase.transform.position.x - base.transform.position.x))
			{
				plantBase = null;
			}
			else
			{
				zombieBase = null;
			}
		}
		if (!(zombieBase != null) && !(plantBase != null))
		{
			return;
		}
		if (ButterRenderer.enabled)
		{
			Butter component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Butter).GetComponent<Butter>();
			component.transform.SetParent(null);
			component.Bullet.rotation = Quaternion.Euler(0f, 0f, -63f);
			Vector3 vector = ((!base.IsFacingLeft) ? new Vector3(-0.145f, 1.2f) : MyTool.ReverseX(new Vector3(-0.145f, 1.2f)));
			component.Init(plantBase, zombieBase, base.transform.position + vector, GetAttackValue(), base.currGrid.Point.y, isHypno);
			ButterRenderer.enabled = false;
			SetSpriteEnable(ButterRenderer.transform.name, enable: false);
			return;
		}
		Kernal component2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Kernal).GetComponent<Kernal>();
		component2.transform.SetParent(null);
		Vector3 vector2;
		if (base.IsFacingLeft)
		{
			vector2 = MyTool.ReverseX(new Vector3(-0.22f, 1.1f));
			component2.Bullet.rotation = Quaternion.Euler(0f, 0f, 194f);
		}
		else
		{
			vector2 = new Vector3(-0.22f, 1.1f);
			component2.Bullet.rotation = Quaternion.Euler(0f, 0f, 14f);
		}
		component2.Init(plantBase, zombieBase, base.transform.position + vector2, GetAttackValue() / 2, base.currGrid.Point.y, isHypno);
	}
}
