using UnityEngine;

public class ThreePeater : PlantBase
{
	private bool canAttack;

	public SpriteRenderer HeadRederer2;

	public SpriteRenderer HeadRederer3;

	public override float MaxHp => 300f;

	protected override int attackValue => 20;

	protected override void OnInitForAll()
	{
		canAttack = false;
	}

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
		CreatePea();
		SetAnimChange(12);
		base.BaseAnimSpeed = 1f;
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		if (ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y + 1, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			canAttack = true;
		}
		if (!canAttack && ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			canAttack = true;
		}
		if (!canAttack && ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y - 1, base.transform.position, base.IsFacingLeft, isHypno) != null)
		{
			canAttack = true;
		}
		if (!canAttack)
		{
			if (MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno) != null)
			{
				canAttack = true;
			}
			if (!canAttack && MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y + 1, base.IsFacingLeft, !isHypno) != null)
			{
				canAttack = true;
			}
			if (!canAttack && MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y - 1, base.IsFacingLeft, !isHypno) != null)
			{
				canAttack = true;
			}
		}
		if (canAttack)
		{
			SetAnimChange(11);
			base.BaseAnimSpeed = 2f;
		}
		return canAttack;
	}

	private void CreatePea()
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
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(new Vector3(0.25f, 0.45f, 0f)), base.currGrid.Point.y - 1, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + new Vector3(0.25f, 0.45f, 0f), base.currGrid.Point.y - 1, Vector2.right, GetBulletSortOrder(-1), isHypno);
			}
			component.StartVerticalMove(1.4f);
			component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(new Vector3(0.35f, 0.2f, 0f)), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + new Vector3(0.35f, 0.2f, 0f), base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
			component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Pea).GetComponent<Pea>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(new Vector3(0.8f, -0.12f, 0f)), base.currGrid.Point.y + 1, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + new Vector3(0.8f, -0.12f, 0f), base.currGrid.Point.y + 1, Vector2.right, GetBulletSortOrder(1), isHypno);
			}
			component.StartVerticalMove(-1.1f);
			canAttack = false;
		}
	}

	protected override void SetEyeTex(int EyeType)
	{
		switch (EyeType)
		{
		case 0:
			HeadRederer2.material.SetTexture("_EyeTex", null);
			HeadRederer3.material.SetTexture("_EyeTex", null);
			break;
		case 1:
			HeadRederer2.material.SetTexture("_EyeTex", eye1.texture);
			HeadRederer3.material.SetTexture("_EyeTex", eye1.texture);
			break;
		case 2:
			HeadRederer2.material.SetTexture("_EyeTex", eye2.texture);
			HeadRederer3.material.SetTexture("_EyeTex", eye2.texture);
			break;
		}
	}
}
