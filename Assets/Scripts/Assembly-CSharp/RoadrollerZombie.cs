using System.Collections.Generic;
using UnityEngine;

public class RoadrollerZombie : ZombieBase
{
	public Sprite Top1;

	public Sprite Top2;

	public Sprite Top3;

	public SpriteRenderer Top;

	public Sprite Back1;

	public Sprite Back2;

	public Sprite Back3;

	public SpriteRenderer Back;

	public Transform Wheel;

	public Transform Wheel2;

	private int state1;

	private int state2;

	public override int MaxHP => 2100;

	protected override GameObject Prefab => GameManager.Instance.GameConf.RoadrollerZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => 5f;

	protected override float attackValue => 600f;

	public override bool CanEatByChomper => false;

	public override bool CanBlowBack => false;

	public override void InitZombieHpState()
	{
		Top.sprite = Top1;
		Back.sprite = Back1;
		state1 = MaxHP / 3 * 2;
		state2 = MaxHP / 3;
		InMoreLines = new List<int> { base.CurrLine - 1 };
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			SetSortingOrder(base.CurrLine * 200 + 62);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.roadroller, base.transform.position);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		canButter = false;
		canDizzy = false;
		if (base.Hp <= state1 && base.Hp >= state2)
		{
			Top.sprite = Top2;
			Back.sprite = Back2;
		}
		else if (base.Hp <= state2)
		{
			Top.sprite = Top3;
			Back.sprite = Back3;
		}
		else
		{
			Top.sprite = Top1;
			Back.sprite = Back1;
		}
		if (HitSound)
		{
			if (Random.Range(0, 2) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
			}
		}
	}

	protected override void UpdateThis()
	{
		if (!IsOVer && (base.State == ZombieState.Walk || base.State == ZombieState.Attack))
		{
			Grid upperGrid = MapManager.Instance.GetUpperGrid(base.NextGrid);
			if (base.NextGrid != null && Mathf.Abs(base.NextGrid.Position.x - base.transform.position.x) < 1.9f)
			{
				HurtPlant(base.NextGrid);
			}
			if (upperGrid != null && Mathf.Abs(upperGrid.Position.x - base.transform.position.x) < 1.9f)
			{
				HurtPlant(upperGrid);
			}
			upperGrid = MapManager.Instance.GetUpperGrid(base.CurrGrid);
			if (base.CurrGrid != null)
			{
				HurtPlant(base.CurrGrid);
			}
			if (upperGrid != null)
			{
				HurtPlant(upperGrid);
			}
			upperGrid = MapManager.Instance.GetUpperGrid(lastGrid);
			if (lastGrid != null)
			{
				HurtPlant(lastGrid);
			}
			if (upperGrid != null)
			{
				HurtPlant(upperGrid);
			}
		}
	}

	private void HurtPlant(Grid grid)
	{
		if (grid.CurrPlantBase != null)
		{
			if (isHypno && grid.CurrPlantBase.isHypno)
			{
				grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this, isFlat: true);
			}
			else if (!isHypno && !grid.CurrPlantBase.isHypno)
			{
				grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this, isFlat: true);
			}
		}
		if (grid.Cage != null)
		{
			grid.Cage.HurtThis(2000f);
		}
	}

	protected override int HandleHurt(int attackValue, Vector2 dirction)
	{
		if ((base.IsFacingLeft && dirction.x > 0f) || (!base.IsFacingLeft && dirction.x < 0f))
		{
			attackValue /= 10;
		}
		return attackValue;
	}

	protected override void CheckState()
	{
		base.CheckState();
		if (base.State == ZombieState.Dead)
		{
			GoDead();
		}
	}

	private void GoDead()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CarParticle).transform.position = base.transform.position;
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CloudParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.explosion, base.transform.position);
		DirectDead(canDropItem: true, 0.1f);
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Steelwheel).GetComponent<SteelWheel>().CreateInit(Wheel, Wheel2, base.CurrLine, Sorting.sortingOrder);
	}

	public override void SpecialAnimEvent1()
	{
		GoDead();
	}
}
