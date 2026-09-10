using SocketSave;
using UnityEngine;

public class SunShroom : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	private int Sunsum = 20;

	public Sprite sleepEye;

	private bool IsLow;

	private bool isFirst;

	private int GrowNum;

	public override float MaxHp => 300f;

	protected override bool isShroom => true;

	public override bool IsLowPlant => IsLow;

	protected override void OnInitForPlace()
	{
		GrowNum = 0;
		IsLow = true;
		Sunsum = 15;
		isFirst = true;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(createSunTime / 2f, isOver: false);
		}
	}

	private bool CreateSun()
	{
		if (!NormalProduceCondition())
		{
			return false;
		}
		bool flag = !SkyManager.Instance.GetIsDay();
		if (!flag)
		{
			flag = Random.Range(0, 4) > 2;
		}
		if (flag)
		{
			ServerSendSyn(0);
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateSun));
		}
		return flag;
	}

	protected override bool DoAction()
	{
		bool flag = CreateSun();
		if (isFirst & flag)
		{
			isFirst = false;
			StartActionCD(createSunTime, isOver: false);
		}
		if (!isFirst && IsLow)
		{
			GrowNum++;
			if (GrowNum >= 7)
			{
				GoGrow();
			}
		}
		return true;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, null));
		}
		else if (syn.SynCode[1] == 1)
		{
			GoGrow();
		}
	}

	private void GoGrow()
	{
		ServerSendSyn(1);
		Sunsum = 35;
		IsLow = false;
		SetAnimChange(11);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.plantgrow, base.transform.position);
	}

	private void InstantiateSun()
	{
		float num = 0f;
		if (GobalEffManager.Instance.IsHaveThisEff(GobalEffect.MoonTypeIncrease))
		{
			num = 5f;
		}
		float num2 = (float)Sunsum + num;
		SunType sunType = ((isHypno && LV.Instance.CurrLVType != LVType.PvP) ? SunType.Red : SunType.Normal);
		if (sunType == SunType.Red)
		{
			num2 = 0f - num2;
		}
		SkyManager.Instance.CreatePlantSun(base.transform.position, num2, sunType, PlacePlayer);
	}

	protected override void GoAwakeSpecial()
	{
	}

	protected override void GoSleepSpecial()
	{
		EyeREnderer.material.SetTexture("_EyeTex", sleepEye.texture);
	}
}
