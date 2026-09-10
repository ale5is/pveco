using SocketSave;
using UnityEngine;

public class RottenImitater : PlantBase
{
	private bool ToZombie;

	private PlantType Ptype;

	public override float MaxHp => 300f;

	public override bool CanPlaceOnWater => true;

	public override bool CanPlaceOnHardGround => true;

	protected override void OnInitForPlace()
	{
		SetAnimChange(11);
	}

	public override void PlaceOverEvent()
	{
		Ptype = PlantType.Nope;
		if (base.currGrid.CurrPlantBase == this)
		{
			if (base.currGrid.isHardGrid || (base.currGrid.isWaterGrid && base.currGrid.IsIce))
			{
				Ptype = PlantType.Pot;
			}
			else if (base.currGrid.isNoIceWater || base.currGrid.isHavePuddle)
			{
				Ptype = PlantType.Lilypad;
			}
		}
		ToZombie = Random.Range(0, 5) > 3;
		ServerSendSyn(ToZombie ? 1 : 2);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 1)
		{
			ToZombie = true;
		}
		if (syn.SynCode[1] == 2)
		{
			ToZombie = false;
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (ToZombie)
		{
			Dead(isFlat: false, 4f);
			Shadow.enabled = true;
			SetAnimChange(12);
		}
	}

	public override void SpecialAnimEvent2()
	{
		Object.Instantiate(GameManager.Instance.GameConf.ImitaterParticle).transform.position = base.transform.position;
		if (GameManager.Instance.isClient)
		{
			return;
		}
		int num = 0;
		Dead(isFlat: false, 1f);
		PlantBase plantBase = null;
		if (Ptype == PlantType.Nope)
		{
			while (true)
			{
				if (num > 5)
				{
					Ptype = PlantType.SunFlower;
					break;
				}
				Ptype = PlantManager.Instance.GetRandomType();
				if (Ptype != PlantType.Imitater && Ptype != PlantType.RottenImitater)
				{
					plantBase = PlantManager.Instance.GetNewPlant(Ptype);
					if (SeedBank.Instance.CheckPlant(plantBase, base.currGrid, -2, PlacePlayer) && !plantBase.IsZombiePlant)
					{
						break;
					}
					num++;
					Object.Destroy(plantBase.gameObject);
				}
			}
		}
		else
		{
			plantBase = PlantManager.Instance.GetNewPlant(Ptype);
		}
		if (SeedBank.Instance.CheckPlant(plantBase, base.currGrid, -2, PlacePlayer))
		{
			SeedBank.Instance.PlantConfirm(plantBase, base.currGrid, -2, 0, PlacePlayer);
			if (isHypno && LV.Instance.CurrLVType != LVType.PvP)
			{
				if (needHypnoPurple)
				{
					plantBase.Hypno();
				}
				else
				{
					plantBase.RatThis();
				}
			}
		}
		else
		{
			Object.Destroy(plantBase.gameObject);
		}
		Dead();
	}

	public override void SpecialAnimEvent3()
	{
		Object.Instantiate(GameManager.Instance.GameConf.ImitaterParticle).transform.position = base.transform.position;
		if (GameManager.Instance.isClient)
		{
			return;
		}
		int num = 0;
		ZombieType zombieType = ZombieType.Nope;
		ZombieBase zombieBase = null;
		while (!(zombieBase != null))
		{
			num++;
			zombieType = ZombieManager.Instance.RottenGetZombieType();
			if (num > 5)
			{
				zombieType = ZombieType.NormalZombie;
			}
			zombieBase = ZombieManager.Instance.GetNewZombie(zombieType);
		}
		if (SeedBank.Instance.CheckZombie(zombieType, base.currGrid, -2, PlacePlayer))
		{
			SeedBank.Instance.ZombieConfirm(zombieType, zombieBase, base.currGrid, -2, PlacePlayer, ratZombie: false);
		}
		else
		{
			Object.Destroy(zombieBase.gameObject);
		}
		Dead();
	}

	protected override void GoSleepSpecial()
	{
		GoAwake();
		ZZZ.gameObject.SetActive(value: false);
	}
}
