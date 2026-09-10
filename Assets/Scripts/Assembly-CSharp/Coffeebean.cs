using UnityEngine;

public class Coffeebean : PlantBase
{
	public override float MaxHp => 300f;

	public override bool ZombieCanEat => false;

	protected override bool HaveShadow => false;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => false;

	public override bool isHaveSpecialCheck => true;

	protected override Vector2 offSet => new Vector2(0f, 0.8f);

	public override bool isFloatPlant => true;

	public override bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		if (grid.CurrFloatPlant == null)
		{
			if (!(grid.CurrPlantBase != null))
			{
				return true;
			}
			if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.isSleeping)
			{
				return true;
			}
			if (grid.CurrPlantBase.isSleeping)
			{
				return true;
			}
			if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.ProtectPlant.isSleeping)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnInitForPlace()
	{
		SetAnimChange(11);
		animatorSorting.sortingOrder = animatorSorting.sortingOrder / 100 * 100 + FixedInfo.FloatPlant;
	}

	public override void SpCheckInitPlace()
	{
		base.currGrid.CurrFloatPlant = this;
	}

	public override void SpecialAnimEvent1()
	{
		PlayAudio();
	}

	public override void SpecialAnimEvent2()
	{
		OverAwake();
	}

	private void PlayAudio()
	{
		if (!(base.currGrid.CurrPlantBase != null))
		{
			return;
		}
		if (base.currGrid.CurrPlantBase.CarryPlant == null)
		{
			if (base.currGrid.CurrPlantBase.isSleeping)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
		}
		else if (base.currGrid.CurrPlantBase.CarryPlant.isSleeping)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
		}
	}

	private void OverAwake()
	{
		if (base.currGrid == null)
		{
			return;
		}
		if (base.currGrid.CurrPlantBase != null)
		{
			if (base.currGrid.CurrPlantBase.CarryPlant != null && base.currGrid.CurrPlantBase.CarryPlant.isSleeping)
			{
				base.currGrid.CurrPlantBase.CarryPlant.GoAwake();
			}
			else if (base.currGrid.CurrPlantBase.isSleeping)
			{
				base.currGrid.CurrPlantBase.GoAwake();
			}
			else if (base.currGrid.CurrPlantBase.ProtectPlant != null && base.currGrid.CurrPlantBase.ProtectPlant.isSleeping)
			{
				base.currGrid.CurrPlantBase.ProtectPlant.GoAwake();
			}
		}
		Dead();
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
		ZZZ.gameObject.SetActive(value: false);
	}
}
