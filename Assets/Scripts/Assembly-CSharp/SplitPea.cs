using UnityEngine;

public class SplitPea : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	public int ShootNum;

	public Sprite eye12;

	public Sprite eye22;

	public SpriteRenderer HeadRederer2;

	public override float MaxHp => 300f;

	protected override int attackValue => 20;

	protected override void OnInitForPlace()
	{
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreateFrontPea();
	}

	public override void SpecialAnimEvent2()
	{
		CreateBackPea();
		if (ShootNum == 0)
		{
			ShootNum++;
			SetAnimChange(0);
		}
		else
		{
			ShootNum = 0;
			SetAnimChange(12);
			base.BaseAnimSpeed = 1f;
		}
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(base.currGrid.Point.y, base.transform.position, isHypno);
		PlantBase plantBase = null;
		if (zombieByLineMinDisNoDir == null)
		{
			plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if (plantBase == null)
			{
				plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, !base.IsFacingLeft, !isHypno);
			}
		}
		if (zombieByLineMinDisNoDir != null || plantBase != null)
		{
			base.BaseAnimSpeed = 3f;
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreateFrontPea()
	{
		if (!NormalDontAttackCondition())
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			Pea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + creatBulletOffsetPos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
		}
	}

	private void CreateBackPea()
	{
		if (!NormalDontAttackCondition())
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			Pea component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (!base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + new Vector3(-0.9f, 0.28f, 0f), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(new Vector3(-0.9f, 0.28f, 0f)), base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
		}
	}

	protected override void SetEyeTex(int EyeType)
	{
		switch (EyeType)
		{
		case 0:
			HeadRederer2.material.SetTexture("_EyeTex", null);
			break;
		case 1:
			HeadRederer2.material.SetTexture("_EyeTex", eye12.texture);
			break;
		case 2:
			HeadRederer2.material.SetTexture("_EyeTex", eye22.texture);
			break;
		}
	}
}
