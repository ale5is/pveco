using System;
using System.Collections.Generic;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance;

	public GameObject StatsDisPreFab;

	public Transform Content;

	public GameObject ScrollView;

	private List<Text> LoadedObj = new List<Text>();

	private List<StatsEnum> StatsList = new List<StatsEnum>
	{
		StatsEnum.GameTime,
		StatsEnum.GameOpenNum,
		StatsEnum.EarnMoney,
		StatsEnum.SpendMoney,
		StatsEnum.ClickMoney,
		StatsEnum.WinNum,
		StatsEnum.FailNum,
		StatsEnum.ReStartNum,
		StatsEnum.OverAcvNum,
		StatsEnum.ShovelOffNum,
		StatsEnum.MowerStartNum,
		StatsEnum.VaseBreakNum,
		StatsEnum.ClickSunNum,
		StatsEnum.NutFirstAidNum,
		StatsEnum.AbsorbEquipNum,
		StatsEnum.ChangeMapNum
	};

	private StatsAndAcvSave StatsSave => GameManager.Instance.StatsAcvSave;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		ScrollView.SetActive(value: false);
	}

	public void LoadStatsSave()
	{
		int length = Enum.GetValues(typeof(StatsEnum)).Length;
		while (StatsSave.OtherStats.Count < length)
		{
			StatsSave.OtherStats.Add(0);
		}
	}

	public void AddStatsNum(StatsEnum stats, int addNum = 1)
	{
		int num = StatsSave.OtherStats[(int)stats];
		StatsSave.OtherStats[(int)stats] += addNum;
		if (stats == StatsEnum.EarnMoney && StatsSave.OtherStats[(int)stats] >= 1000000)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.Money100w);
		}
		if (stats == StatsEnum.ClickMoney && StatsSave.OtherStats[(int)stats] >= 100 && num < 100)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.ClickMoney100);
		}
	}

	public void ClearStatsNum(StatsEnum stats)
	{
		StatsSave.OtherStats[(int)stats] = 0;
	}

	public void LoadStats()
	{
		ScrollView.SetActive(value: true);
		ScrollView.transform.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
		if (LoadedObj.Count == 0)
		{
			for (int i = 0; i < StatsList.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(StatsDisPreFab);
				Text component = gameObject.transform.Find("TitleText").GetComponent<Text>();
				Text component2 = gameObject.transform.Find("NumText").GetComponent<Text>();
				component.text = GetStatsName(StatsList[i]);
				if (i % 2 == 1)
				{
					gameObject.transform.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.1f);
				}
				LoadedObj.Add(component2);
				gameObject.transform.SetParent(Content);
				gameObject.transform.localScale = new Vector3(1.4f, 1.4f);
			}
			Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 56 * StatsList.Count);
		}
		for (int j = 0; j < StatsList.Count; j++)
		{
			if (StatsList[j] == StatsEnum.GameTime)
			{
				int num = StatsSave.OtherStats[(int)StatsList[j]];
				int num2 = num / 86400;
				int num3 = num % 86400;
				int num4 = num3 / 3600;
				int num5 = num3 % 3600 / 60;
				LoadedObj[j].text = "";
				if (num2 > 0)
				{
					Text text = LoadedObj[j];
					text.text = text.text + num2 + "d";
				}
				if (num4 > 0)
				{
					Text text2 = LoadedObj[j];
					text2.text = text2.text + num4 + "h";
				}
				Text text3 = LoadedObj[j];
				text3.text = text3.text + num5 + "m";
			}
			else
			{
				LoadedObj[j].text = StatsSave.OtherStats[(int)StatsList[j]].ToString();
			}
		}
		Content.GetComponent<RectTransform>().localPosition = new Vector2(0f, 0f);
	}

	public void CloseViwer()
	{
		ScrollView.SetActive(value: false);
	}

	private string GetStatsName(StatsEnum stats)
	{
		string result = "???";
		switch (stats)
		{
		case StatsEnum.GameTime:
			result = "游戏时间";
			break;
		case StatsEnum.GameOpenNum:
			result = "游戏打开次数";
			break;
		case StatsEnum.EarnMoney:
			result = "赚到的钱";
			break;
		case StatsEnum.SpendMoney:
			result = "花掉的钱";
			break;
		case StatsEnum.ClickMoney:
			result = "连续点击金钱数";
			break;
		case StatsEnum.EatGraveNum:
			result = "啃掉的墓碑数量";
			break;
		case StatsEnum.WinNum:
			result = "关卡胜利数";
			break;
		case StatsEnum.FailNum:
			result = "关卡失败数";
			break;
		case StatsEnum.ReStartNum:
			result = "重新开始数";
			break;
		case StatsEnum.WakeUpNum:
			result = "唤醒植物数";
			break;
		case StatsEnum.OverAcvNum:
			result = "完成成就数";
			break;
		case StatsEnum.CheatSquashNum:
			result = "诈骗窝瓜数";
			break;
		case StatsEnum.ShovelOffNum:
			result = "铲子使用数";
			break;
		case StatsEnum.MowerStartNum:
			result = "小推车启动数";
			break;
		case StatsEnum.VaseBreakNum:
			result = "罐子砸碎数";
			break;
		case StatsEnum.ClickSunNum:
			result = "收集阳光个数";
			break;
		case StatsEnum.NutFirstAidNum:
			result = "坚果修复术使用数";
			break;
		case StatsEnum.AbsorbEquipNum:
			result = "吸引装备数";
			break;
		case StatsEnum.ChangeMapNum:
			result = "切换地图数";
			break;
		}
		return result;
	}
}
