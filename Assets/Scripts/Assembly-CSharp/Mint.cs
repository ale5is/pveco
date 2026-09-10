using UnityEngine;

public class Mint : PlantBase
{
	public override float MaxHp => 300f;

	public override bool ZombieCanEat => false;

	protected override bool HaveShadow => false;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => false;

	public override bool isHaveSpecialCheck => true;

	protected override Vector2 offSet => new Vector2(0f, 0.8f);

	public override bool isFloatPlant => true;

	protected override void OnInitForAll()
	{
		canFrozen = false;
		canIce = false;
	}

	public override bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		if (grid.CurrFloatPlant == null)
		{
			return true;
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
		if (base.currGrid != null)
		{
			base.currGrid.CurrFloatPlant = this;
		}
	}

	public override void SpecialAnimEvent1()
	{
		OverAwake();
	}

	public override void SpecialAnimEvent2()
	{
		Dead();
	}

	private void OverAwake()
	{
		if (base.currGrid.CurrPlantBase != null && (LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(GameManager.Instance.LocalPlayerSave.playerName, PlacePlayer)))
		{
			if (base.currGrid.CurrPlantBase.CarryPlant != null && SeedBank.Instance.ClearCD(base.currGrid.CurrPlantBase.CarryPlant.GetPlantType()))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
			else if (base.currGrid.CurrPlantBase != null && SeedBank.Instance.ClearCD(base.currGrid.CurrPlantBase.GetPlantType()))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
			else if (base.currGrid.CurrPlantBase.ProtectPlant != null && SeedBank.Instance.ClearCD(base.currGrid.CurrPlantBase.ProtectPlant.GetPlantType()))
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.wakeup, base.transform.position);
			}
		}
	}

	protected override void GoSleepSpecial()
	{
		ZZZ.gameObject.SetActive(value: false);
	}
}
