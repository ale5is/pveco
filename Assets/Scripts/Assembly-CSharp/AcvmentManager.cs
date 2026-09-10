using System;
using System.Collections.Generic;
using UnityEngine;

public class AcvmentManager : MonoBehaviour
{
	public static AcvmentManager Instance;

	public GameObject AchivPrefab;

	public Sprite EmptyIcon;

	public List<Sprite> AcvIcons = new List<Sprite>();

	private List<TextMesh> LoadedAcvOverNum = new List<TextMesh>();

	private List<SpriteRenderer> LoadedAcvRederer = new List<SpriteRenderer>();

	private List<Acvname> achievementList = new List<Acvname>
	{
		Acvname.Potato5,
		Acvname.Cherry25,
		Acvname.Popcorn4,
		Acvname.Squash10,
		Acvname.ClickMoney100,
		Acvname.SunFull,
		Acvname.RollNut5,
		Acvname.Plant49,
		Acvname.UnitAsOne,
		Acvname.Sunflower200,
		Acvname.Money100w,
		Acvname.LuckCorn5,
		Acvname.UltimateKill,
		Acvname.CheatSquash50,
		Acvname.ChaosZombie,
		Acvname.FlatSquash,
		Acvname.IceShroom20
	};

	private List<Acvname> CurrLvOverAcv = new List<Acvname>();

	private List<Acvname> GetOnlyOne = new List<Acvname>
	{
		Acvname.Plant49,
		Acvname.Money100w
	};

	private List<int> OverAcvmentNum => GameManager.Instance.StatsAcvSave.OverAcvNum;

	private void Awake()
	{
		Instance = this;
	}

	public void LoadAcvSave()
	{
		int length = Enum.GetValues(typeof(Acvname)).Length;
		while (OverAcvmentNum.Count < length)
		{
			OverAcvmentNum.Add(0);
		}
	}

	public void LVResetThis()
	{
		CurrLvOverAcv.Clear();
	}

	public void GetAchievement(Acvname achiv, string playerName)
	{
		if (playerName == GameManager.Instance.LocalPlayerSave.playerName)
		{
			GetAchievement(achiv);
		}
		else if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendAcvmentGet(achiv, playerName);
		}
	}

	public void GetAchievement(Acvname achiv)
	{
		if (!CurrLvOverAcv.Contains(achiv))
		{
			CurrLvOverAcv.Add(achiv);
			if (OverAcvmentNum[(int)achiv] == 0)
			{
				StatsManager.Instance.AddStatsNum(StatsEnum.OverAcvNum);
				ChatInput.Instance.SendMessageToAll("完成了成就<color=#41FF00>[" + GetAchivName(achiv) + "]</color>！", needName: true);
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Achivment, Vector2.zero, isAll: true);
			}
			OverAcvmentNum[(int)achiv]++;
			if (OverAcvmentNum[(int)achiv] == 1)
			{
				GameManager.Instance.SaveSAInfo();
			}
			if (GetOnlyOne.Contains(achiv))
			{
				OverAcvmentNum[(int)achiv] = 1;
			}
		}
	}

	public void InitAcvment()
	{
		if (LoadedAcvRederer.Count == 0)
		{
			for (int i = 0; i < achievementList.Count; i++)
			{
				GameObject obj = UnityEngine.Object.Instantiate(AchivPrefab);
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = new Vector3(0f, -2.5f - 1.5f * (float)i);
				SpriteRenderer component = obj.transform.Find("AcvIcons").GetComponent<SpriteRenderer>();
				TextMesh component2 = obj.transform.Find("AcvTitle").GetComponent<TextMesh>();
				TextMesh component3 = obj.transform.Find("AcvContent").GetComponent<TextMesh>();
				TextMesh component4 = obj.transform.Find("AcvOverNum").GetComponent<TextMesh>();
				LoadedAcvRederer.Add(component);
				LoadedAcvOverNum.Add(component4);
				component4.text = "";
				component.color = new Color(1f, 1f, 1f, 0.4f);
				component2.text = GetAchivName(achievementList[i]);
				component3.text = GetAchivContent(achievementList[i]);
				if (AcvIcons.Count > i)
				{
					component.sprite = AcvIcons[i];
				}
				else
				{
					component.sprite = EmptyIcon;
				}
			}
		}
		for (int j = 0; j < achievementList.Count; j++)
		{
			int num = OverAcvmentNum[(int)achievementList[j]];
			if (num > 0)
			{
				LoadedAcvRederer[j].color = Color.white;
			}
			else
			{
				LoadedAcvRederer[j].color = new Color(1f, 1f, 1f, 0.4f);
			}
			if (num > 1)
			{
				LoadedAcvOverNum[j].text = num.ToString();
			}
			else
			{
				LoadedAcvOverNum[j].text = "";
			}
		}
	}

	private string GetAchivName(Acvname acvname)
	{
		string result = "???";
		switch (acvname)
		{
		case Acvname.Potato5:
			result = "土豆泥";
			break;
		case Acvname.Cherry25:
			result = "火爆兄弟";
			break;
		case Acvname.Popcorn4:
			result = "爆米花";
			break;
		case Acvname.Squash10:
			result = "泰山压顶";
			break;
		case Acvname.ClickMoney100:
			result = "铁公鸡";
			break;
		case Acvname.SunFull:
			result = "阳光满屋";
			break;
		case Acvname.RollNut5:
			result = "五颗星";
			break;
		case Acvname.Plant49:
			result = "植物猎人";
			break;
		case Acvname.UnitAsOne:
			result = "团结一致";
			break;
		case Acvname.Sunflower200:
			result = "光芒万丈";
			break;
		case Acvname.Money100w:
			result = "百万富翁";
			break;
		case Acvname.LuckCorn5:
			result = "幸运玉米";
			break;
		case Acvname.UltimateKill:
			result = "极限击杀";
			break;
		case Acvname.CheatSquash50:
			result = "诈骗的艺术";
			break;
		case Acvname.ChaosZombie:
			result = "混沌僵尸";
			break;
		case Acvname.FlatSquash:
			result = "倒反天罡";
			break;
		case Acvname.IceShroom20:
			result = "冷血杀手";
			break;
		}
		return result;
	}

	private string GetAchivContent(Acvname acvname)
	{
		string result = "???";
		switch (acvname)
		{
		case Acvname.Potato5:
			result = "成功用土豆地雷一次炸飞5只僵尸。";
			break;
		case Acvname.Cherry25:
			result = "成功用一个樱桃炸弹同时炸死25只正常体型的僵尸。";
			break;
		case Acvname.Popcorn4:
			result = "成功用一发玉米炮弹同时炸死4个巨人僵尸。";
			break;
		case Acvname.Squash10:
			result = "成功用窝瓜一次压死10只僵尸。";
			break;
		case Acvname.ClickMoney100:
			result = "连续捡取100次金钱，过程中不能因有金钱消失而中断。";
			break;
		case Acvname.SunFull:
			result = "在单一关卡内胜利并剩余10000阳光。";
			break;
		case Acvname.RollNut5:
			result = "用一个坚果撞倒5个僵尸。";
			break;
		case Acvname.Plant49:
			result = "获得49株植物。";
			break;
		case Acvname.UnitAsOne:
			result = "四名玩家一同通关一个三地图关卡。";
			break;
		case Acvname.Sunflower200:
			result = "让一株向日葵一次产生200或以上的阳光。";
			break;
		case Acvname.Money100w:
			result = "赚到100万。";
			break;
		case Acvname.LuckCorn5:
			result = "玉米投手连续投掷5发黄油。";
			break;
		case Acvname.UltimateKill:
			result = "在僵尸将要进家时击杀他。(暂不可完成)";
			break;
		case Acvname.CheatSquash50:
			result = "让窝瓜被诈骗50次。(暂不可完成)";
			break;
		case Acvname.ChaosZombie:
			result = "让一只僵尸同时获得魅惑、黄油、眩晕、冻结的效果。";
			break;
		case Acvname.FlatSquash:
			result = "让1个窝瓜被压扁。";
			break;
		case Acvname.IceShroom20:
			result = "使用寒冰菇一次冻死20只正常体型的僵尸。";
			break;
		}
		return result;
	}
}
