using SocketSave;
using UnityEngine;

public class TwinSunflower : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	private bool isFirst;

	public SpriteRenderer HeadRederer2;

	public override float MaxHp => 300f;

	public override PlantType BasePlant => PlantType.SunFlower;

	public override int BasePlantSunNum => 50;

	protected override void OnInitForPlace()
	{
		isFirst = true;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(0.5f, isOver: false);
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

	private void InstantiateSun()
	{
		float num = 0f;
		if (GobalEffManager.Instance.IsHaveThisEff(GobalEffect.SunTypeIncrease))
		{
			num = 5f;
		}
		SunType sunType = ((isHypno && LV.Instance.CurrLVType != LVType.PvP) ? SunType.Red : SunType.Normal);
		float num2 = 25f + ((float)(base.currGrid.LightNum / 3 * 5) + num);
		if (sunType == SunType.Red)
		{
			num2 = 0f - num2;
		}
		int num3 = 2;
		if (Random.Range(0, 6) > 4)
		{
			num3 = 3;
		}
		for (int i = 0; i < num3; i++)
		{
			SkyManager.Instance.CreatePlantSun(base.transform.position, num2, sunType, PlacePlayer);
		}
		if (!GameManager.Instance.isClient && num3 >= 3 && num2 > 66f)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.Sunflower200, PlacePlayer);
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
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
			HeadRederer2.material.SetTexture("_EyeTex", eye1.texture);
			break;
		case 2:
			HeadRederer2.material.SetTexture("_EyeTex", eye2.texture);
			break;
		}
	}
}
