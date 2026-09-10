using System.Collections.Generic;
using UnityEngine;

public class NormalSprite : MonoBehaviour
{
	public static NormalSprite Instance;

	public GameObject SpriteDisplay;

	public Sprite CheckBox;

	public Sprite CheckBox2;

	public Sprite CheckBoxYes;

	public Sprite CheckBoxYes2;

	public Sprite CheckBoxNo;

	[Header("地图封面")]
	public Sprite YardDay;

	public Sprite YardNight;

	public Sprite SwampDay;

	public Sprite SwampNight;

	public Sprite CustomYard;

	public List<Sprite> YardMiniGame = new List<Sprite>();

	[Header("战利品")]
	public Sprite Almanac;

	public Sprite Store;

	public Sprite Shovel;

	public Sprite MoneyBag;

	public Sprite Trophy;

	public Sprite ZombieNote;

	public Sprite Taco;

	[Header("天气图标")]
	public Sprite NoWeather;

	public Sprite Clear;

	public Sprite SmallRain;

	public Sprite MidRain;

	public Sprite BigRain;

	public Sprite ThunderRain;

	public Sprite SmallSnow;

	public Sprite MidSnow;

	public Sprite BigSnow;

	public Sprite SmallHail;

	public Sprite MidHail;

	public Sprite BigHail;

	public Sprite Wind;

	[Header("全局效果图标")]
	public Sprite RainCool;

	public Sprite SunTypeIncrease;

	public Sprite MoonTypeIncrease;

	public Sprite ZombieHotRestless;

	public Sprite PlantButter;

	public List<Sprite> SeaweedSprites = new List<Sprite>();

	private void Awake()
	{
		Instance = this;
	}
}
