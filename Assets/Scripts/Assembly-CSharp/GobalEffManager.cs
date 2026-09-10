using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GobalEffManager : MonoBehaviour
{
	public static GobalEffManager Instance;

	public List<Image> EffImages = new List<Image>();

	[SerializeField]
	private List<GobalEffect> CurrEffects = new List<GobalEffect>();

	private bool RefreshGet;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		for (int i = 0; i < EffImages.Count; i++)
		{
			EffImages[i].color = new Color(1f, 1f, 1f, 0f);
		}
	}

	public bool IsHaveThisEff(GobalEffect effect)
	{
		return CurrEffects.Contains(effect);
	}

	private void AddEff(GobalEffect effect)
	{
		if (!IsHaveThisEff(effect))
		{
			RefreshGet = true;
			CurrEffects.Add(effect);
			ResetSpeedRate(effect);
		}
	}

	private void RemoveEff(GobalEffect effect)
	{
		if (IsHaveThisEff(effect))
		{
			RefreshGet = true;
			CurrEffects.Remove(effect);
			ResetSpeedRate(effect);
		}
	}

	private void ResetSpeedRate(GobalEffect effect)
	{
		if (effect == GobalEffect.RainCool)
		{
			PlantManager.Instance.ResetAllSpeedRate();
		}
		if (effect == GobalEffect.ZombieHotRestless)
		{
			ZombieManager.Instance.ResetAllSpeedRate();
		}
	}

	public void RefreshEffect()
	{
		RefreshGet = false;
		int rainScale = SkyManager.Instance.RainScale;
		int snowScale = SkyManager.Instance.SnowScale;
		int time = SkyManager.Instance.Time;
		MapBase currMap = CameraControl.Instance.CurrMap;
		if (currMap == null)
		{
			currMap = MapManager.Instance.GetCurrMap(Vector3.zero);
		}
		if (currMap == null)
		{
			return;
		}
		if (rainScale > 0 && currMap.CurrTempt > 10f && currMap.CurrTempt < 20f)
		{
			AddEff(GobalEffect.RainCool);
		}
		else
		{
			RemoveEff(GobalEffect.RainCool);
		}
		if (rainScale < 4 && snowScale < 4 && time > 660 && time < 840)
		{
			AddEff(GobalEffect.SunTypeIncrease);
		}
		else
		{
			RemoveEff(GobalEffect.SunTypeIncrease);
		}
		if (rainScale < 4 && snowScale < 4 && (time > 1380 || time < 120))
		{
			AddEff(GobalEffect.MoonTypeIncrease);
		}
		else
		{
			RemoveEff(GobalEffect.MoonTypeIncrease);
		}
		if (currMap.CurrTempt > 30f)
		{
			AddEff(GobalEffect.ZombieHotRestless);
		}
		else
		{
			RemoveEff(GobalEffect.ZombieHotRestless);
		}
		if (!RefreshGet)
		{
			return;
		}
		for (int i = 0; i < EffImages.Count; i++)
		{
			EffImages[i].color = new Color(1f, 1f, 1f, 0f);
		}
		for (int j = 0; j < CurrEffects.Count; j++)
		{
			if (j <= EffImages.Count - 1)
			{
				EffImages[j].sprite = GetEffIcon(CurrEffects[j]);
				EffImages[j].color = Color.white;
			}
		}
	}

	private Sprite GetEffIcon(GobalEffect effect)
	{
		Sprite result = null;
		switch (effect)
		{
		case GobalEffect.RainCool:
			result = NormalSprite.Instance.RainCool;
			break;
		case GobalEffect.SunTypeIncrease:
			result = NormalSprite.Instance.SunTypeIncrease;
			break;
		case GobalEffect.MoonTypeIncrease:
			result = NormalSprite.Instance.MoonTypeIncrease;
			break;
		case GobalEffect.ZombieHotRestless:
			result = NormalSprite.Instance.ZombieHotRestless;
			break;
		}
		return result;
	}
}
