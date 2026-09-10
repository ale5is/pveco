using SocketSave;
using UnityEngine;

public class SunFlower : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	private bool isFirst;

	private int IZSunNum = 8;

	private float IZSunState;

	private float lastHp;

	public override float MaxHp => 300f;

	protected override void OnInitForPlace()
	{
		isFirst = true;
		lastHp = base.Hp;
		IZSunState = MaxHp / (float)IZSunNum;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(createSunTime / 2f, isOver: false);
		}
	}

	protected override bool DoAction()
	{
		bool flag = CreateSun();
		if (isFirst & flag)
		{
			isFirst = false;
			StartActionCD(createSunTime, isOver: false);
		}
		return true;
	}

	private bool CreateSun()
	{
		if (!NormalProduceCondition())
		{
			return false;
		}
		int shadowNum = SkyManager.Instance.GetShadowNum(base.currGrid);
		if (Random.Range(0, shadowNum) > shadowNum - 3)
		{
			ServerSendSyn(0);
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
			return true;
		}
		return false;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}

	private void InstantiateSun()
	{
		if (base.currGrid != null)
		{
			float num = 0f;
			if (GobalEffManager.Instance.IsHaveThisEff(GobalEffect.SunTypeIncrease))
			{
				num = 5f;
			}
			float num2 = 25f + ((float)(base.currGrid.LightNum / 3 * 5) + num);
			SunType sunType = ((isHypno && LV.Instance.CurrLVType != LVType.PvP) ? SunType.Red : SunType.Normal);
			if (sunType == SunType.Red)
			{
				num2 = 0f - num2;
			}
			SkyManager.Instance.CreatePlantSun(base.transform.position, num2, sunType, PlacePlayer);
		}
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (LV.Instance.CurrLVType != LVType.IZombie)
		{
			return;
		}
		if (base.Hp < lastHp)
		{
			int num = (int)((MaxHp - base.Hp) / IZSunState);
			int num2 = (int)((MaxHp - lastHp) / IZSunState);
			int num3 = num - num2;
			for (int i = 0; i < num3; i++)
			{
				IZSunNum--;
				SkyManager.Instance.CreatePlantSun(base.transform.position, 25f, SunType.Moon, PlacePlayer);
			}
		}
		lastHp = base.Hp;
	}

	protected override void DeadEvent()
	{
	}
}
