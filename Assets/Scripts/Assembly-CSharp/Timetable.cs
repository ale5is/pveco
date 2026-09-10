using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Timetable : MonoBehaviour
{
	public static Timetable Instance;

	private Text TimeText;

	private Text WeatherText;

	private Text TemperatureText;

	private Text WindText;

	private Transform WindPointer;

	private Image TemptMeter;

	public Volume volume;

	public WhiteBalance whiteBalance;

	public Image CurrWeather;

	public Image NextWeather;

	public Text NextWeatherText;

	public Image NextWeather2;

	public Text NextWeather2Text;

	private float maxShakeAngle = 30f;

	private float shakeFrequency = 5f;

	private float baseAngle;

	private float windStrength => (float)SkyManager.Instance.WindScale * 0.4f;

	private void Awake()
	{
		Instance = this;
		volume.profile.TryGet<WhiteBalance>(out whiteBalance);
		TimeText = base.transform.Find("Time").GetComponent<Text>();
		WeatherText = base.transform.Find("Weather").GetComponent<Text>();
		TemperatureText = base.transform.Find("Temperature").GetComponent<Text>();
		WindText = base.transform.Find("WindScale").GetComponent<Text>();
		TemptMeter = base.transform.Find("TemptMeter").GetComponent<Image>();
		WindPointer = base.transform.Find("WindPointer");
	}

	private void Update()
	{
		float num = (SkyManager.Instance.WindTowardRight ? 0f : 180f);
		if (baseAngle != num)
		{
			float num2 = Mathf.Abs(num - baseAngle) * 5f;
			if (baseAngle < num)
			{
				baseAngle += Time.deltaTime * num2;
				if (baseAngle > num)
				{
					baseAngle = num;
				}
			}
			else if (baseAngle > num)
			{
				baseAngle -= Time.deltaTime * num2;
				if (baseAngle < num)
				{
					baseAngle = num;
				}
			}
		}
		float num3 = CalculateShake();
		Vector3 eulerAngles = new Vector3
		{
			z = baseAngle + num3
		};
		WindPointer.eulerAngles = eulerAngles;
	}

	private float CalculateShake()
	{
		if (windStrength <= 0.01f)
		{
			return 0f;
		}
		float num = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) * 2f - 1f) * windStrength * maxShakeAngle;
		if (Mathf.Abs(num) < 0.5f * windStrength)
		{
			float num2 = Mathf.Sin(Time.time * 30f) * 0.5f * windStrength;
			num += num2;
		}
		return num;
	}

	public void UpdateTime(int time)
	{
		int num = time / 60;
		int num2 = time % 60;
		if (num < 10)
		{
			if (num2 < 10)
			{
				TimeText.text = "0" + num + ":0" + num2;
			}
			else
			{
				TimeText.text = "0" + num + ":" + num2;
			}
		}
		else if (num2 < 10)
		{
			TimeText.text = num + ":0" + num2;
		}
		else
		{
			TimeText.text = num + ":" + num2;
		}
	}

	public void UpdateWeather()
	{
		int rainScale = SkyManager.Instance.RainScale;
		int snowScale = SkyManager.Instance.SnowScale;
		int hailScale = SkyManager.Instance.HailScale;
		int windScale = SkyManager.Instance.WindScale;
		if (SkyManager.Instance.IsThunder)
		{
			WeatherText.text = "雷雨";
			CurrWeather.sprite = NormalSprite.Instance.ThunderRain;
		}
		else if (rainScale > 0)
		{
			if (rainScale <= 4)
			{
				WeatherText.text = "小雨";
				CurrWeather.sprite = NormalSprite.Instance.SmallRain;
			}
			else if (rainScale <= 7)
			{
				WeatherText.text = "中雨";
				CurrWeather.sprite = NormalSprite.Instance.MidRain;
			}
			else
			{
				WeatherText.text = "大雨";
				CurrWeather.sprite = NormalSprite.Instance.MidRain;
			}
		}
		else if (snowScale > 0)
		{
			if (snowScale <= 4)
			{
				WeatherText.text = "小雪";
				CurrWeather.sprite = NormalSprite.Instance.SmallSnow;
			}
			else if (snowScale <= 7)
			{
				WeatherText.text = "中雪";
				CurrWeather.sprite = NormalSprite.Instance.MidSnow;
			}
			else
			{
				WeatherText.text = "大雪";
				CurrWeather.sprite = NormalSprite.Instance.BigSnow;
			}
		}
		else if (hailScale > 0)
		{
			if (hailScale <= 4)
			{
				WeatherText.text = "轻雹";
				CurrWeather.sprite = NormalSprite.Instance.SmallHail;
			}
			else if (hailScale <= 7)
			{
				WeatherText.text = "中雹";
				CurrWeather.sprite = NormalSprite.Instance.MidHail;
			}
			else
			{
				WeatherText.text = "重雹";
				CurrWeather.sprite = NormalSprite.Instance.BigHail;
			}
		}
		else
		{
			WeatherText.text = "晴朗";
			CurrWeather.sprite = NormalSprite.Instance.Clear;
		}
		WindText.text = windScale.ToString();
	}

	public void UpdateWeatherReport()
	{
		NextWeatherText.text = "";
		NextWeather2Text.text = "";
		if (LV.Instance.LoadWeathers.Count > 0)
		{
			SetReport(NextWeather, LV.Instance.LoadWeathers[0].type, LV.Instance.LoadWeathers[0].scale, NextWeatherText);
			if (LV.Instance.LoadWeathers.Count > 1)
			{
				SetReport(NextWeather2, LV.Instance.LoadWeathers[1].type, LV.Instance.LoadWeathers[1].scale, NextWeather2Text);
			}
			else
			{
				NextWeather2.sprite = NormalSprite.Instance.NoWeather;
			}
		}
		else
		{
			NextWeather.sprite = NormalSprite.Instance.NoWeather;
			NextWeather2.sprite = NormalSprite.Instance.NoWeather;
		}
		if (!GameManager.Instance.isServer)
		{
			return;
		}
		TimetableSyn timetableSyn = new TimetableSyn();
		if (LV.Instance.LoadWeathers.Count > 0)
		{
			timetableSyn.type = LV.Instance.LoadWeathers[0].type;
			timetableSyn.WeSc = LV.Instance.LoadWeathers[0].scale;
			if (LV.Instance.LoadWeathers.Count > 1)
			{
				timetableSyn.type2 = LV.Instance.LoadWeathers[1].type;
				timetableSyn.WeSc2 = LV.Instance.LoadWeathers[1].scale;
			}
		}
		SocketServer.Instance.SynTimeTable(timetableSyn);
	}

	private void SetReport(Image nextWeather, WeatherType type, int scale, Text text)
	{
		text.text = "";
		switch (type)
		{
		case WeatherType.Nope:
			nextWeather.sprite = NormalSprite.Instance.NoWeather;
			break;
		case WeatherType.Clear:
			nextWeather.sprite = NormalSprite.Instance.Clear;
			break;
		case WeatherType.Rain:
			if (scale <= 4)
			{
				nextWeather.sprite = NormalSprite.Instance.SmallRain;
			}
			else if (scale <= 7)
			{
				nextWeather.sprite = NormalSprite.Instance.MidRain;
			}
			else
			{
				nextWeather.sprite = NormalSprite.Instance.MidRain;
			}
			break;
		case WeatherType.Thunder:
			nextWeather.sprite = NormalSprite.Instance.ThunderRain;
			break;
		case WeatherType.Snow:
			if (scale <= 4)
			{
				nextWeather.sprite = NormalSprite.Instance.SmallSnow;
			}
			else if (scale <= 7)
			{
				nextWeather.sprite = NormalSprite.Instance.MidSnow;
			}
			else
			{
				nextWeather.sprite = NormalSprite.Instance.BigSnow;
			}
			break;
		case WeatherType.Wind:
			text.text = scale.ToString();
			nextWeather.sprite = NormalSprite.Instance.Wind;
			break;
		case WeatherType.Hail:
			if (scale <= 4)
			{
				nextWeather.sprite = NormalSprite.Instance.SmallHail;
			}
			else if (scale <= 7)
			{
				nextWeather.sprite = NormalSprite.Instance.MidHail;
			}
			else
			{
				nextWeather.sprite = NormalSprite.Instance.BigHail;
			}
			break;
		}
	}

	public void UpdateTempt(MapBase tempt)
	{
		if (tempt != null && tempt == CameraControl.Instance.CurrMap)
		{
			if (tempt.CurrTempt > 0f)
			{
				TemptMeter.color = new Color(1f, 0.5f, 0f);
			}
			else
			{
				TemptMeter.color = new Color(0.51f, 0.94f, 1f);
			}
			TemptMeter.fillAmount = Mathf.Abs(tempt.CurrTempt) / 50f;
			TemperatureText.text = ((int)tempt.CurrTempt).ToString() ?? "";
			int num = (int)tempt.CurrTempt;
			if (num > 0 && num < 10)
			{
				num = 0;
			}
			if (num > 10)
			{
				num -= 10;
			}
			whiteBalance.temperature.overrideState = true;
			whiteBalance.temperature.value = num;
		}
	}

	public void LvReset()
	{
		whiteBalance.temperature.overrideState = true;
		whiteBalance.temperature.value = 0f;
	}

	public void ClientSyn(TimetableSyn syn)
	{
		SetReport(NextWeather, syn.type, syn.WeSc, NextWeatherText);
		SetReport(NextWeather2, syn.type2, syn.WeSc2, NextWeather2Text);
	}
}
