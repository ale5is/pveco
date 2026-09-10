using UnityEngine;

public class Imitater : PlantBase
{
	private bool IsProtectPlant;

	private PlantType basePlant;

	private int basePlantSunNum;

	private bool canCarryOtherPlant;

	private bool haveSpecialCheck;

	private PlantType PlacePlant;

	public override float MaxHp => 300f;

	public override bool isProtectPlant => IsProtectPlant;

	public override PlantType BasePlant => basePlant;

	public override int BasePlantSunNum => basePlantSunNum;

	public override bool CanCarryOtherPlant => canCarryOtherPlant;

	public override bool isHaveSpecialCheck => haveSpecialCheck;

	protected override void OnInitForPlace()
	{
		SetAnimChange(11);
	}

	public void CopyPlantInfo(PlantBase plant)
	{
		IsProtectPlant = plant.isProtectPlant;
		basePlant = plant.BasePlant;
		haveSpecialCheck = plant.isHaveSpecialCheck;
		basePlantSunNum = plant.BasePlantSunNum;
		canCarryOtherPlant = plant.CanCarryOtherPlant;
		if (canCarryOtherPlant)
		{
			haveSpecialCheck = true;
		}
		PlacePlant = plant.GetPlantType();
	}

	public override void SpecialAnimEvent1()
	{
		Object.Instantiate(GameManager.Instance.GameConf.ImitaterParticle).transform.position = base.transform.position;
	}

	public override void SpecialAnimEvent2()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		Dead(isFlat: false, 1f);
		PlantBase newPlant = PlantManager.Instance.GetNewPlant(PlacePlant);
		if (SeedBank.Instance.CheckPlant(newPlant, base.currGrid, -2, PlacePlayer))
		{
			SeedBank.Instance.PlantConfirm(newPlant, base.currGrid, -2, 1, PlacePlayer);
			if (isHypno && LV.Instance.CurrLVType != LVType.PvP)
			{
				if (needHypnoPurple)
				{
					newPlant.Hypno();
				}
				else
				{
					newPlant.RatThis();
				}
			}
		}
		else
		{
			Object.Destroy(newPlant.gameObject);
		}
		Dead();
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
		ZZZ.gameObject.SetActive(value: false);
	}
}
