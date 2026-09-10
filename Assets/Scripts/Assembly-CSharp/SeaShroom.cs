using UnityEngine;

public class SeaShroom : PlantBase
{
	private Vector3 creatBulletOffsetPos = new Vector2(0.3f, -0.14f);

	public override float MaxHp => 300f;

	protected override int attackValue => 20;

	protected override Vector2 offSet => new Vector2(0f, -0.35f);

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => true;

	protected override bool HaveShadow => false;

	public override bool CanCarryed => false;

	public override bool CanProtect => false;

	protected override bool isShroom => true;

	public override bool IsLowPlant => true;

	protected override void OnInitForPlace()
	{
		StartActionCD(1.4f, isOver: true);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		if ((zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < 4.9f) || (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < 4.9f))
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreatePuff();
		SetAnimChange(12);
	}

	private void CreatePuff()
	{
		if (base.currGrid != null)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Puff, base.transform.position);
			ShroomPuff component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomPuff).GetComponent<ShroomPuff>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), NeedDis: true, isHypno, IsLow: true);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + creatBulletOffsetPos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), NeedDis: true, isHypno, IsLow: true);
			}
		}
	}
}
