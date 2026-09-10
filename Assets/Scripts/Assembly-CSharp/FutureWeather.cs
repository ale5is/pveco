using System;
using UnityEngine;

[Serializable]
public class FutureWeather
{
	public int appearTime;

	public WeatherType type;

	public int scale;

	public int reportTime;

	public FutureWeather(WeatherType type, int scale, int appearTime)
	{
		this.type = type;
		this.appearTime = appearTime;
		this.scale = scale;
		reportTime = 720;
	}

	public FutureWeather(WeatherType type, int scale, int appearTime, int timeBackwardOffset)
	{
		this.type = type;
		this.appearTime = appearTime + UnityEngine.Random.Range(0, Mathf.Abs(timeBackwardOffset));
		this.scale = scale;
		reportTime = 720;
	}

	public FutureWeather(WeatherType type, int scale, int appearTime, int timeBackwardOffset, int reportTime)
	{
		this.type = type;
		this.appearTime = appearTime + UnityEngine.Random.Range(0, Mathf.Abs(timeBackwardOffset));
		this.scale = scale;
		this.reportTime = reportTime;
	}
}
