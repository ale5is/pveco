using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GobalLight : MonoBehaviour
{
	public Light2D gobalLight;

	public static GobalLight Instance;

	public float TimeIntensity;

	public float WeatherIntensity;

	public float ThunderIntensity;

	private Coroutine RainCoroutine;

	private Coroutine ThunderCoroutine;

	public bool LightingNotDark;

	private void Awake()
	{
		Instance = this;
	}

	public void ResetAll()
	{
		StopAllCoroutines();
		gobalLight.intensity = 1f;
	}

	private void ResetGobalLight()
	{
		gobalLight.intensity = TimeIntensity + WeatherIntensity + ThunderIntensity;
	}

	public void InitIntensity()
	{
		StopAllCoroutines();
		int time = SkyManager.Instance.Time;
		if (time > 420 && time < 1020)
		{
			TimeIntensity = 0.2f;
		}
		else if (time > 1140 || time < 300)
		{
			TimeIntensity = 0f;
		}
		WeatherIntensity = 0.5f;
		if (SkyManager.Instance.RainScale > 0)
		{
			WeatherIntensity = 0.5f - (float)SkyManager.Instance.RainScale * 0.05f;
		}
		else if (SkyManager.Instance.HailScale > 0)
		{
			WeatherIntensity = 0.5f - (float)SkyManager.Instance.HailScale * 0.02f;
		}
		if (SkyManager.Instance.IsThunder)
		{
			ThunderIntensity = 0f;
			if (LightingNotDark)
			{
				ThunderIntensity = 0.2f;
			}
		}
		else
		{
			ThunderIntensity = 0.3f;
		}
		TimeChange();
	}

	public void RainScaleChange(bool fade)
	{
		if (LVManager.Instance.InGame)
		{
			if (RainCoroutine != null)
			{
				StopCoroutine(RainCoroutine);
			}
			if (fade)
			{
				RainCoroutine = StartCoroutine(RainFade());
				return;
			}
			WeatherIntensity = 0.5f - (float)SkyManager.Instance.RainScale * 0.05f;
			ResetGobalLight();
		}
	}

	private IEnumerator RainFade()
	{
		float goal = 0.5f - (float)SkyManager.Instance.RainScale * 0.05f;
		float x = (goal - WeatherIntensity) / 300f;
		for (int i = 0; i < 300; i++)
		{
			yield return new WaitForSeconds(0.1f);
			WeatherIntensity += x;
			ResetGobalLight();
		}
		WeatherIntensity = goal;
		ResetGobalLight();
	}

	public void SnowScaleChange()
	{
		if (RainCoroutine != null)
		{
			StopCoroutine(RainCoroutine);
		}
		RainCoroutine = StartCoroutine(RainFade());
	}

	private IEnumerator SnowFade()
	{
		float goal = 0.5f - (float)SkyManager.Instance.SnowScale * 0.02f;
		float x = (goal - WeatherIntensity) / 300f;
		for (int i = 0; i < 300; i++)
		{
			yield return new WaitForSeconds(0.1f);
			WeatherIntensity += x;
			ResetGobalLight();
		}
		WeatherIntensity = goal;
		ResetGobalLight();
	}

	public void HailScaleChange(bool fade)
	{
		if (LVManager.Instance.InGame)
		{
			if (RainCoroutine != null)
			{
				StopCoroutine(RainCoroutine);
			}
			if (fade)
			{
				RainCoroutine = StartCoroutine(HailFade());
				return;
			}
			WeatherIntensity = 0.5f - (float)SkyManager.Instance.HailScale * 0.02f;
			ResetGobalLight();
		}
	}

	private IEnumerator HailFade()
	{
		float goal = 0.5f - (float)SkyManager.Instance.HailScale * 0.02f;
		float x = (goal - WeatherIntensity) / 300f;
		for (int i = 0; i < 300; i++)
		{
			yield return new WaitForSeconds(0.1f);
			WeatherIntensity += x;
			ResetGobalLight();
		}
		WeatherIntensity = goal;
		ResetGobalLight();
	}

	public void ThunderChange()
	{
		if (LVManager.Instance.InGame)
		{
			if (ThunderCoroutine != null)
			{
				StopCoroutine(ThunderCoroutine);
			}
			ThunderCoroutine = StartCoroutine(ThunderFade());
		}
	}

	private IEnumerator ThunderFade()
	{
		float goal = 0.3f;
		if (SkyManager.Instance.IsThunder)
		{
			goal = 0f;
			if (LightingNotDark)
			{
				goal = 0.2f;
			}
		}
		float x = (goal - ThunderIntensity) / 300f;
		for (int i = 0; i < 300; i++)
		{
			yield return new WaitForSeconds(0.1f);
			ThunderIntensity += x;
			ResetGobalLight();
		}
		ThunderIntensity = goal;
		ResetGobalLight();
	}

	public void TimeChange()
	{
		int time = SkyManager.Instance.Time;
		if (time >= 300 && time <= 420)
		{
			float num = (float)(time - 300) / 120f;
			TimeIntensity = 0.2f * num;
		}
		if (time >= 1020 && time <= 1140)
		{
			float num2 = (float)(time - 1020) / 120f;
			TimeIntensity = 0.2f - 0.2f * num2;
		}
		ResetGobalLight();
	}
}
