using System.Collections;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class SunflowerZombie : PlantZombie
{
	private bool isFirst;

	private int createSunTime = 24;

	protected override GameObject Prefab => GameManager.Instance.GameConf.SunflowerZombie;

	protected override void PlantZombieInit()
	{
		if (!GameManager.Instance.isClient)
		{
			isFirst = true;
			StartActionCD(createSunTime / 2, isOver: false);
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
		int shadowNum = SkyManager.Instance.GetShadowNum(base.CurrGrid);
		if (Random.Range(0, shadowNum) > shadowNum - 3)
		{
			ServerSendSyn(1);
			StartCoroutine(BrightnessEffect2(1.5f, InstantiateSun));
			return true;
		}
		return false;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			RandomSpeed = syn.Twofloat.x;
			base.Speed = DefSpeed;
		}
		if (syn.SynCode[1] == 1)
		{
			StartCoroutine(BrightnessEffect2(1.5f, InstantiateSun));
		}
	}

	private void InstantiateSun()
	{
		if (base.CurrGrid != null)
		{
			float num = 0f;
			if (GobalEffManager.Instance.IsHaveThisEff(GobalEffect.SunTypeIncrease))
			{
				num = 5f;
			}
			SkyManager.Instance.CreatePlantSun(base.transform.position, 0f - (25f + ((float)(base.CurrGrid.LightNum / 3 * 5) + num)), SunType.Red, PlacePlayer);
		}
	}

	protected IEnumerator BrightnessEffect2(float wantBright, UnityAction fun)
	{
		float currBright = 1f;
		while (currBright < wantBright)
		{
			yield return null;
			if (Time.deltaTime > 0f)
			{
				currBright += Time.deltaTime;
				SetAllBrightness(currBright);
			}
		}
		SetAllBrightness(1f);
		fun?.Invoke();
	}
}
