using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
	private int onlineSunId;

	public static SkyManager Instance;

	private List<Sun> sunList = new List<Sun>();

	public int clickedSunNum;

	private Coroutine TimeCoroutine;

	private Coroutine RainChangeCoroutine;

	private Coroutine SnowChangeCoroutine;

	private Coroutine HailChangeCoroutine;

	public Lightning lightning;

	public GameObject NormalLightning;

	public SpriteRenderer ScrollFog;

	public ParticleSystem RainParticle;

	public ParticleSystem SnowParticle;

	public ParticleSystem HailParticle;

	public bool DayLightCycle;

	private ParticleSystem.EmissionModule RainEmissionModule;

	private ParticleSystem.EmissionModule SnowEmissionModule;

	private ParticleSystem.EmissionModule HailEmissionModule;

	private bool isThunder;

	private bool canLightning;

	private int LightningTime;

	public int WindTime;

	public int CurrWindTime;

	private bool windTowardRight;

	private int rainScale;

	private int snowScale;

	private int windScale;

	private int hailScale;

	[SerializeField]
	private int time;

	public bool SunAutoCollect;

	public bool SlowSunAutoCollect;

	public bool isRainFog;

	public int Time
	{
		get
		{
			return time;
		}
		set
		{
			int num = value;
			if (!DayLightCycle && !GameManager.Instance.isClient)
			{
				num = time;
			}
			StatsManager.Instance.AddStatsNum(StatsEnum.GameTime);
			if (LVManager.Instance.GameIsStart)
			{
				if (GetIsDay(time) && !GetIsDay(num))
				{
					AudioManager.Instance.FadeBgAndPlayNew(LV.Instance.NightBgm);
				}
				else if (!GetIsDay(time) && GetIsDay(num))
				{
					AudioManager.Instance.FadeBgAndPlayNew(LV.Instance.DayBgm);
				}
			}
			time = num;
			if (Time > 1439)
			{
				time = 0;
			}
			Timetable.Instance.UpdateTime(Time);
			GobalEffManager.Instance.RefreshEffect();
			if (!GameManager.Instance.isClient)
			{
				if (LV.Instance.LoadWeathers.Count > 0 && LV.Instance.LoadWeathers[0].appearTime != 0 && LV.Instance.LoadWeathers[0].appearTime == LVManager.Instance.PassTime)
				{
					PlayWeather(LV.Instance.LoadWeathers[0]);
					MyTool.MoveFirstToLast(LV.Instance.LoadWeathers);
					Timetable.Instance.UpdateWeatherReport();
				}
				if (DayLightCycle && LV.Instance.TimeAction.ContainsKey(Time))
				{
					LV.Instance.TimeAction[Time]();
				}
			}
			clickedSunNum = 0;
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].BaseTimeChange(time);
			}
			GobalLight.Instance.TimeChange();
			UpdateWind();
			if (IsThunder && !GameManager.Instance.isClient)
			{
				if (canLightning && (Random.Range(0, 15) > 13 || LightningTime > 5))
				{
					LightningTime = 0;
					lightning.gameObject.SetActive(value: true);
					Grid grid = MapManager.Instance.GetRandomGrid();
					PlantBase plantBase = null;
					List<Clematis> list = new List<Clematis>();
					List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(grid, 2);
					for (int j = 0; j < aroundGrid.Count; j++)
					{
						if (aroundGrid[j].CurrPlantBase != null)
						{
							if (aroundGrid[j].CurrPlantBase is Clematis)
							{
								list.Add((Clematis)aroundGrid[j].CurrPlantBase);
							}
							else if (aroundGrid[j].CurrPlantBase.CarryPlant is Clematis)
							{
								list.Add((Clematis)aroundGrid[j].CurrPlantBase.CarryPlant);
							}
						}
					}
					if (list.Count > 0)
					{
						plantBase = list[Random.Range(0, list.Count)];
						grid = plantBase.currGrid;
					}
					else
					{
						List<Umbrellaleaf> list2 = new List<Umbrellaleaf>();
						List<Grid> aroundGrid2 = MapManager.Instance.GetAroundGrid(grid, 1);
						for (int k = 0; k < aroundGrid2.Count; k++)
						{
							if (aroundGrid2[k].CurrPlantBase != null)
							{
								if (aroundGrid2[k].CurrPlantBase is Umbrellaleaf)
								{
									list2.Add((Umbrellaleaf)aroundGrid2[k].CurrPlantBase);
								}
								else if (aroundGrid2[k].CurrPlantBase.CarryPlant is Umbrellaleaf)
								{
									list2.Add((Umbrellaleaf)aroundGrid2[k].CurrPlantBase.CarryPlant);
								}
							}
						}
						if (list2.Count > 0)
						{
							plantBase = list2[Random.Range(0, list.Count)];
						}
					}
					lightning.LightGrid(grid, plantBase);
					StartCoroutine(WaitLightning());
					if (GameManager.Instance.isServer)
					{
						LightingSpawn lightingSpawn = new LightingSpawn();
						lightingSpawn.Pos = grid.Position;
						SocketServer.Instance.SpawnLightning(lightingSpawn);
					}
				}
				else
				{
					LightningTime++;
				}
			}
			if (HailScale > 3 && Random.Range(0, 15) < HailScale)
			{
				int count = MapManager.Instance.mapList.Count;
				for (int l = 0; l < count; l++)
				{
					LvItemManager.Instance.SpawnFallHail(MapManager.Instance.GetRandomGrid(), Random.Range(5, 6 + HailScale) / 10);
				}
			}
		}
	}

	public int OnlineSunId
	{
		get
		{
			onlineSunId++;
			return onlineSunId;
		}
		private set
		{
			onlineSunId = value;
		}
	}

	public int RainScale
	{
		get
		{
			return rainScale;
		}
		private set
		{
			rainScale = value;
			Timetable.Instance.UpdateWeather();
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].BaseTimeChange(Time);
			}
		}
	}

	public bool IsThunder
	{
		get
		{
			return isThunder;
		}
		set
		{
			if (LVManager.Instance.InGame && isThunder != value)
			{
				isThunder = value;
				canLightning = true;
				GobalLight.Instance.ThunderChange();
				Timetable.Instance.UpdateWeather();
				if (GameManager.Instance.isServer)
				{
					WeatherChange weatherChange = new WeatherChange();
					weatherChange.type = WeatherType.Rain;
					weatherChange.WeSc = RainScale;
					weatherChange.isTud = IsThunder;
					SocketServer.Instance.SendWeatherCmd(weatherChange);
				}
			}
		}
	}

	public int SnowScale
	{
		get
		{
			return snowScale;
		}
		private set
		{
			snowScale = value;
			Timetable.Instance.UpdateWeather();
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].BaseTimeChange(Time);
			}
		}
	}

	public int WindScale
	{
		get
		{
			int num = windScale + RainScale / 4;
			if (num > 5)
			{
				num = 5;
			}
			return num;
		}
		private set
		{
			if (windScale != value)
			{
				if (value > 5)
				{
					windScale = 5;
				}
				else
				{
					windScale = value;
				}
				WindScaleReSet();
				if (GameManager.Instance.isServer)
				{
					WeatherChange weatherChange = new WeatherChange();
					weatherChange.type = WeatherType.Wind;
					weatherChange.WeSc = windScale;
					weatherChange.isTud = WindTowardRight;
					SocketServer.Instance.SendWeatherCmd(weatherChange);
				}
			}
		}
	}

	public bool WindTowardRight
	{
		get
		{
			return windTowardRight;
		}
		private set
		{
			windTowardRight = value;
		}
	}

	public int HailScale
	{
		get
		{
			return hailScale;
		}
		private set
		{
			hailScale = value;
			Timetable.Instance.UpdateWeather();
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].BaseTimeChange(Time);
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		DayLightCycle = true;
		canLightning = true;
		SunAutoCollect = false;
		RainEmissionModule = RainParticle.emission;
		RainEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		SnowEmissionModule = SnowParticle.emission;
		SnowEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		HailEmissionModule = HailParticle.emission;
		HailEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		AudioManager.Instance.weatherAudio.enabled = false;
	}

	public void StartTime()
	{
		TimeCoroutine = StartCoroutine(TimeAdd());
	}

	public void StopTime()
	{
		if (TimeCoroutine != null)
		{
			StopCoroutine(TimeCoroutine);
		}
	}

	private IEnumerator TimeAdd()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (LV.Instance.CurrLVType != LVType.IZombie && LV.Instance.CurrLVType != LVType.VaseBreaker)
			{
				if (!GameManager.Instance.isClient)
				{
					Time++;
				}
				if (GameManager.Instance.isServer)
				{
					TimeCmd timeCmd = new TimeCmd();
					timeCmd.time = Time;
					SocketServer.Instance.SendTimeCmd(timeCmd);
				}
			}
			LVManager.Instance.PassTime++;
			LVManager.Instance.LvTotalTime--;
		}
	}

	public void SetWindScale(int scale)
	{
		WindScale = scale;
	}

	private void UpdateWind()
	{
		if (WindScale <= 0)
		{
			return;
		}
		if (WindTime == 0)
		{
			WindTime = Random.Range(20, 40);
			CurrWindTime += WindTime;
		}
		if (CurrWindTime > 0)
		{
			CurrWindTime--;
			if (CurrWindTime == 0)
			{
				CurrWindTime = -WindTime;
				SetWindToward(isRight: false);
			}
		}
		else if (CurrWindTime < 0)
		{
			CurrWindTime++;
			if (CurrWindTime == 0)
			{
				WindTime = 0;
				SetWindToward(isRight: true);
			}
		}
	}

	private void SetWindToward(bool isRight, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			windTowardRight = isRight;
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].WindDirectionReset();
			}
			if (GameManager.Instance.isServer)
			{
				WeatherChange weatherChange = new WeatherChange();
				weatherChange.type = WeatherType.Wind;
				weatherChange.WeSc = windScale;
				weatherChange.isTud = WindTowardRight;
				SocketServer.Instance.SendWeatherCmd(weatherChange);
			}
		}
	}

	private void WindScaleReSet()
	{
		Timetable.Instance.UpdateWeather();
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapManager.Instance.mapList[i].WindScaleReset();
		}
		AudioManager.Instance.SetWindVolume(WindScale > 0, needFade: true);
	}

	public void AffectWind(bool facingLeft, int intensity)
	{
		if (GameManager.Instance.isClient || WindScale == 0)
		{
			return;
		}
		intensity = (int)((float)intensity * (2f - (float)WindScale * 0.2f));
		if (facingLeft)
		{
			int num = CurrWindTime - intensity;
			if (num < 0 && CurrWindTime > 0)
			{
				CurrWindTime = num - WindTime;
				SetWindToward(isRight: false);
			}
			else
			{
				CurrWindTime = num;
			}
		}
		else
		{
			int num = CurrWindTime + intensity;
			if (num >= 0 && CurrWindTime < 0)
			{
				CurrWindTime = num;
				SetWindToward(isRight: true);
				WindTime = 0;
			}
			else
			{
				CurrWindTime = num;
			}
		}
		if (CurrWindTime > 50)
		{
			CurrWindTime = 50;
		}
		else if (CurrWindTime < -50)
		{
			CurrWindTime = -50;
		}
	}

	private IEnumerator WaitLightning()
	{
		int last = -10;
		canLightning = false;
		int waitTime = 45 - MapManager.Instance.mapList.Count * 10;
		for (int i = 0; i < waitTime; i++)
		{
			yield return new WaitForSeconds(1f);
			if (Random.Range(0, 7) > 5 && i - last > 3)
			{
				last = i;
				if (Random.Range(0, 2) == 0)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder3, base.transform.position, isAll: true);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder4, base.transform.position, isAll: true);
				}
				StartCoroutine(NormalLightningLight());
			}
		}
		canLightning = true;
	}

	private IEnumerator NormalLightningLight()
	{
		NormalLightning.SetActive(value: true);
		yield return new WaitForSeconds(0.8f);
		NormalLightning.SetActive(value: false);
		yield return new WaitForSeconds(0.6f);
		NormalLightning.SetActive(value: true);
		yield return new WaitForSeconds(0.6f);
		NormalLightning.SetActive(value: false);
	}

	public int GetWeatherTempt()
	{
		if (RainScale > 0)
		{
			return -RainScale;
		}
		if (SnowScale > 0)
		{
			return -SnowScale - 10;
		}
		if (HailScale > 0)
		{
			return -HailScale / 2;
		}
		return 0;
	}

	public void SetRainScale(int scale, bool isDirect, bool synClient = false)
	{
		if (scale == RainScale || (!synClient && GameManager.Instance.isClient) || !LVManager.Instance.InGame)
		{
			return;
		}
		if (SnowScale > 0)
		{
			SetSnowScale(0, isDirect: false);
		}
		if (HailScale > 0)
		{
			SetHailScale(0, isDirect: false);
		}
		RainScale = scale;
		if ((RainScale == 0) & isDirect)
		{
			RainParticle.gameObject.SetActive(value: false);
		}
		else
		{
			RainParticle.gameObject.SetActive(value: true);
		}
		AudioManager.Instance.SetRainVolume(scale > 0, !isDirect);
		if (isDirect)
		{
			RainEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(scale * 30);
			if (!isRainFog)
			{
				if (scale > 6)
				{
					ScrollFog.enabled = true;
					ScrollFog.material.SetFloat("_FogIntensity", (float)(scale - 6) * 0.05f - 0.1f);
				}
				else
				{
					ScrollFog.enabled = false;
				}
			}
		}
		else
		{
			if (RainChangeCoroutine != null)
			{
				StopCoroutine(RainChangeCoroutine);
			}
			RainChangeCoroutine = StartCoroutine(ChangeRain(scale));
		}
		WindScaleReSet();
		GobalLight.Instance.RainScaleChange(!isDirect);
		if (GameManager.Instance.isServer)
		{
			WeatherChange weatherChange = new WeatherChange();
			weatherChange.type = WeatherType.Rain;
			weatherChange.WeSc = RainScale;
			weatherChange.isTud = IsThunder;
			weatherChange.isNoFade = isDirect;
			SocketServer.Instance.SendWeatherCmd(weatherChange);
		}
	}

	private IEnumerator ChangeRain(int scale)
	{
		int i = 1;
		if (scale > 6 && !isRainFog)
		{
			if (!ScrollFog.enabled)
			{
				ScrollFog.material.SetFloat("_FogIntensity", -0.2f);
			}
			ScrollFog.enabled = true;
		}
		else
		{
			ScrollFog.enabled = false;
		}
		if ((float)(scale * 30) < RainEmissionModule.rateOverTime.constant)
		{
			i = -1;
		}
		while ((float)(scale * 30) != RainEmissionModule.rateOverTime.constant)
		{
			yield return new WaitForSeconds(0.05f);
			RainEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(RainEmissionModule.rateOverTime.constant + (float)i);
		}
		if (!isRainFog)
		{
			float fog = ScrollFog.material.GetFloat("_FogIntensity");
			float goal = (float)(scale - 6) * 0.05f - 0.1f;
			if (i > 0)
			{
				while (fog < goal)
				{
					yield return new WaitForSeconds(0.1f);
					fog += 0.01f;
					ScrollFog.material.SetFloat("_FogIntensity", fog);
				}
			}
			else
			{
				while (fog > goal)
				{
					yield return new WaitForSeconds(0.1f);
					fog -= 0.01f;
					ScrollFog.material.SetFloat("_FogIntensity", fog);
				}
			}
		}
		RainChangeCoroutine = null;
	}

	public void SetSnowScale(int scale, bool isDirect, bool synClient = false)
	{
		if (!synClient && GameManager.Instance.isClient)
		{
			return;
		}
		SnowScale = scale;
		if (RainScale > 0)
		{
			SetRainScale(0, isDirect: false);
		}
		if (HailScale > 0)
		{
			SetHailScale(0, isDirect: false);
		}
		if ((SnowScale == 0) & isDirect)
		{
			SnowParticle.gameObject.SetActive(value: false);
		}
		else
		{
			SnowParticle.gameObject.SetActive(value: true);
		}
		if (isDirect)
		{
			SnowEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve((scale - 1) * 30);
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].SnowInit();
			}
		}
		else
		{
			if (SnowChangeCoroutine != null)
			{
				StopCoroutine(SnowChangeCoroutine);
			}
			SnowChangeCoroutine = StartCoroutine(ChangeSnow((snowScale - 1) * 30));
		}
		if (GameManager.Instance.isServer)
		{
			WeatherChange weatherChange = new WeatherChange();
			weatherChange.type = WeatherType.Snow;
			weatherChange.WeSc = SnowScale;
			weatherChange.isTud = IsThunder;
			weatherChange.isNoFade = isDirect;
			SocketServer.Instance.SendWeatherCmd(weatherChange);
		}
	}

	private IEnumerator ChangeSnow(int scale)
	{
		int i = 1;
		if ((float)scale < SnowEmissionModule.rateOverTime.constant)
		{
			i = -1;
		}
		while ((float)scale != SnowEmissionModule.rateOverTime.constant)
		{
			yield return new WaitForSeconds(0.05f);
			SnowEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(SnowEmissionModule.rateOverTime.constant + (float)i);
		}
	}

	public void SetHailScale(int scale, bool isDirect, bool synClient = false)
	{
		if (!synClient && GameManager.Instance.isClient)
		{
			return;
		}
		if (SnowScale > 0)
		{
			SetSnowScale(0, isDirect: false);
		}
		if (RainScale > 0)
		{
			SetRainScale(0, isDirect: false);
		}
		HailScale = scale;
		float num = HailScale * 30 + 100;
		if (scale == 0)
		{
			num = 0f;
		}
		if ((HailScale == 0) & isDirect)
		{
			HailParticle.gameObject.SetActive(value: false);
		}
		else
		{
			HailParticle.gameObject.SetActive(value: true);
		}
		AudioManager.Instance.SetHailVolume(scale > 0, !isDirect);
		ParticleSystem.MainModule main = HailParticle.main;
		main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.1f + 0.03f * (float)HailScale);
		RainEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(scale * 20);
		PlantManager.Instance.AllCheckWeather();
		if (isDirect)
		{
			HailEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(num);
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].SnowInit();
			}
		}
		else
		{
			if (HailChangeCoroutine != null)
			{
				StopCoroutine(HailChangeCoroutine);
			}
			HailChangeCoroutine = StartCoroutine(ChangeHail(num));
		}
		GobalLight.Instance.HailScaleChange(!isDirect);
		if (GameManager.Instance.isServer)
		{
			WeatherChange weatherChange = new WeatherChange();
			weatherChange.type = WeatherType.Hail;
			weatherChange.WeSc = HailScale;
			weatherChange.isNoFade = isDirect;
			SocketServer.Instance.SendWeatherCmd(weatherChange);
		}
	}

	private IEnumerator ChangeHail(float scale)
	{
		int i = 5;
		if (scale < HailEmissionModule.rateOverTime.constant)
		{
			i *= -1;
		}
		while (scale != HailEmissionModule.rateOverTime.constant)
		{
			yield return new WaitForSeconds(0.05f);
			HailEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(HailEmissionModule.rateOverTime.constant + (float)i);
		}
	}

	public void DirectSetTime(int time, bool synClient = false)
	{
		if (!GameManager.Instance.isClient || synClient)
		{
			bool dayLightCycle = DayLightCycle;
			DayLightCycle = true;
			int num = time;
			if (time < 0)
			{
				num = -time;
			}
			if (time >= 1440)
			{
				num = time % 1440;
			}
			Time = num;
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].TimeInitMap(Time);
				MapManager.Instance.mapList[i].BaseTimeChange(Time);
			}
			GobalLight.Instance.InitIntensity();
			DayLightCycle = dayLightCycle;
			if (GameManager.Instance.isServer)
			{
				TimeCmd timeCmd = new TimeCmd();
				timeCmd.time = 10000 + num;
				SocketServer.Instance.SendTimeCmd(timeCmd);
			}
		}
	}

	public void ClientLightningThis(Grid grid)
	{
		lightning.gameObject.SetActive(value: true);
		lightning.LightGrid(grid, null);
	}

	public void ClientSynWeather(WeatherChange bag)
	{
		if (bag.type == WeatherType.Rain)
		{
			SetRainScale(bag.WeSc, bag.isNoFade, synClient: true);
		}
		else if (bag.type == WeatherType.Snow)
		{
			SetSnowScale(bag.WeSc, bag.isNoFade, synClient: true);
		}
		else if (bag.type == WeatherType.Hail)
		{
			SetHailScale(bag.WeSc, bag.isNoFade, synClient: true);
		}
		else if (bag.type == WeatherType.Wind)
		{
			WindScale = bag.WeSc;
			SetWindToward(bag.isTud, synClient: true);
		}
		IsThunder = bag.isTud;
	}

	public void ClearAllWeather(bool isDirect, bool synClient = false)
	{
		IsThunder = false;
		SetRainScale(0, isDirect, synClient);
		SetSnowScale(0, isDirect, synClient);
		SetHailScale(0, isDirect, synClient);
		WindScale = 0;
	}

	public void PlayWeather(FutureWeather weather)
	{
		if (!GameManager.Instance.isClient)
		{
			bool isDirect = weather.appearTime <= 0;
			switch (weather.type)
			{
			case WeatherType.Clear:
				ClearAllWeather(isDirect);
				break;
			case WeatherType.Rain:
				SetRainScale(weather.scale, isDirect);
				break;
			case WeatherType.Thunder:
				IsThunder = true;
				SetRainScale(10, isDirect);
				break;
			case WeatherType.Snow:
				SetSnowScale(weather.scale, isDirect);
				break;
			case WeatherType.Wind:
				WindScale = weather.scale;
				break;
			case WeatherType.Hail:
				SetHailScale(weather.scale, isDirect);
				break;
			}
		}
	}

	public int GetShadowNum(Grid grid)
	{
		if (grid == null)
		{
			return 0;
		}
		int num = 1;
		if (!GetIsDay())
		{
			num += 2;
		}
		if (RainScale > 6)
		{
			num++;
		}
		if (grid.isShadow)
		{
			num += 2;
		}
		return num;
	}

	public void ResetAll()
	{
		while (sunList.Count > 0)
		{
			sunList[0].DestroySun();
		}
		NormalLightning.SetActive(value: false);
		canLightning = true;
		sunList.Clear();
		sunList = new List<Sun>();
		ClearAllWeather(isDirect: true, synClient: true);
		StopAllCoroutines();
		GobalLight.Instance.ResetAll();
	}

	public void RemoveSun(Sun sun)
	{
		sunList.Remove(sun);
	}

	public Sun GetARandomSun(MapBase map)
	{
		List<Sun> list = new List<Sun>();
		for (int i = 0; i < sunList.Count; i++)
		{
			if (sunList[i].CanGet && MapManager.Instance.GetCurrMap(sunList[i].transform.position) == map)
			{
				list.Add(sunList[i]);
			}
		}
		Sun result = null;
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	public void CollectAllSun()
	{
		if (!GameManager.Instance.isClient)
		{
			for (int i = 0; i < sunList.Count; i++)
			{
				sunList[i].CollectSun();
			}
		}
	}

	public void OnlineCollectSun(ClickedSun sun)
	{
		for (int i = 0; i < sunList.Count; i++)
		{
			if (sunList[i].OnlineSunId == sun.OnlineSunId)
			{
				sunList[i].OnlineSyn(sun);
				break;
			}
		}
	}

	public void ClientSpawnSun(SunSpawn spawn)
	{
		Sun component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Sun).GetComponent<Sun>();
		component.transform.SetParent(base.transform);
		if (spawn.isSkySun)
		{
			component.InitForSky(spawn.Afloat, spawn.Pos, spawn.type);
		}
		else
		{
			if (!SpectatorList.Instance.LocalIsSpectator && LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
			{
				spawn.Pos = new Vector2(0f - spawn.Pos.x, spawn.Pos.y);
			}
			component.InitForPlant(spawn.Pos, spawn.Afloat, spawn.type, spawn.Player);
		}
		component.OnlineSunId = spawn.OnlineId;
		sunList.Add(component);
	}

	public void CreateSkySun(Vector2 SpawnPos, float DownY, SunType type)
	{
		if (!GameManager.Instance.isClient && LV.Instance.CurrLVType == LVType.Normal)
		{
			Sun component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Sun).GetComponent<Sun>();
			component.transform.SetParent(base.transform);
			component.InitForSky(DownY, SpawnPos, type);
			sunList.Add(component);
			if (GameManager.Instance.isServer)
			{
				SunSpawn sunSpawn = new SunSpawn();
				sunSpawn.OnlineId = OnlineSunId;
				sunSpawn.Player = null;
				component.OnlineSunId = sunSpawn.OnlineId;
				sunSpawn.Pos = SpawnPos;
				sunSpawn.type = type;
				sunSpawn.isSkySun = true;
				sunSpawn.Afloat = DownY;
				SocketServer.Instance.SpawnSun(sunSpawn);
			}
		}
	}

	public void CreatePlantSun(Vector2 pos, float SunNum, SunType type, string Player)
	{
		if (!GameManager.Instance.isClient)
		{
			Sun component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Sun).GetComponent<Sun>();
			component.transform.SetParent(base.transform);
			component.InitForPlant(pos, SunNum, type, Player);
			sunList.Add(component);
			if (GameManager.Instance.isServer)
			{
				SunSpawn sunSpawn = new SunSpawn();
				sunSpawn.OnlineId = OnlineSunId;
				sunSpawn.Player = Player;
				component.OnlineSunId = sunSpawn.OnlineId;
				sunSpawn.Pos = pos;
				sunSpawn.type = type;
				sunSpawn.isSkySun = false;
				sunSpawn.Afloat = SunNum;
				SocketServer.Instance.SpawnSun(sunSpawn);
			}
		}
	}

	public bool GetIsDay()
	{
		return GetIsDay(Time);
	}

	public bool GetIsDay(int time)
	{
		if (time < 1110 && time > 360)
		{
			return true;
		}
		return false;
	}
}
