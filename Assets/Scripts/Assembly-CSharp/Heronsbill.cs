using SocketSave;
using UnityEngine;

public class Heronsbill : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	private bool isFirst;

	public override float MaxHp => 300f;

	public override bool ZombieCanEat => false;

	protected override Vector2 offSet => new Vector2(0f, -0.1f);

	public override bool CanProtect => false;

	public override bool IsLowPlant => true;

	protected override void OnInitForPlace()
	{
		isFirst = true;
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

	private void InstantiateSun()
	{
		float num = 0f;
		if (GobalEffManager.Instance.IsHaveThisEff(GobalEffect.SunTypeIncrease))
		{
			num = 2f;
		}
		float num2 = 10f + ((float)(base.currGrid.LightNum / 3 * 2) + num);
		SunType sunType = ((isHypno && LV.Instance.CurrLVType != LVType.PvP) ? SunType.Red : SunType.Normal);
		if (sunType == SunType.Red)
		{
			num2 = 0f - num2;
		}
		SkyManager.Instance.CreatePlantSun(base.transform.position, num2, sunType, PlacePlayer);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
	}
}
