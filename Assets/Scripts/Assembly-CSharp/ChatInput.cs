using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ChatInput : MonoBehaviour
{
	public static ChatInput Instance;

	public InputField InputField;

	private List<string> oldChat = new List<string>();

	private int oldChatIndex;

	private bool OpenCmdFirst;

	private bool get;

	private int OldChatIndex
	{
		get
		{
			return oldChatIndex;
		}
		set
		{
			if (value < oldChat.Count && value >= 0)
			{
				oldChatIndex = value;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!UIManager.Instance.IsChatBoxOpen)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
		{
			SendContent();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			OldChatIndex--;
			if (oldChat.Count > 0)
			{
				InputField.text = oldChat[OldChatIndex];
			}
			InputField.MoveTextEnd(shift: false);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			OldChatIndex++;
			if (oldChat.Count > 0)
			{
				InputField.text = oldChat[OldChatIndex];
			}
			InputField.MoveTextEnd(shift: false);
		}
	}

	public void ResetAllCmd()
	{
		PlayerManager.Instance.SunInfinite = false;
		PlayerManager.Instance.EnableCreate = false;
		SeedBank.Instance.isNoCD = false;
		PlantManager.Instance.PlantInvincible = false;
		ZombieManager.Instance.ZombieInvincible = false;
		PlantManager.Instance.PlantDontSleep = false;
		LVManager.Instance.LvDontFail = false;
		SkyManager.Instance.DayLightCycle = true;
		SkyManager.Instance.SunAutoCollect = false;
		if (PlayerManager.Instance.IsDebug)
		{
			PlayerManager.Instance.SunInfinite = true;
			SeedBank.Instance.isNoCD = true;
			LVManager.Instance.LvDontFail = true;
			PlantManager.Instance.PlantDontSleep = true;
		}
	}

	public void SendContent()
	{
		if (InputField.text != "")
		{
			if (InputField.text.IndexOf("/") == 0)
			{
				Command(InputField.text);
			}
			else
			{
				SendMessageToAll(InputField.text, needName: true);
			}
			if (oldChat.Count > 20)
			{
				oldChat.Remove(oldChat[0]);
			}
			if (oldChat.Count == 0)
			{
				oldChat.Add(InputField.text);
			}
			if (oldChat.Count > 0 && oldChat[oldChat.Count - 1] != InputField.text)
			{
				oldChat.Add(InputField.text);
			}
		}
		UIManager.Instance.IsChatBoxOpen = false;
	}

	public void SlashOpen()
	{
		StartCoroutine(Slash());
	}

	private IEnumerator Slash()
	{
		yield return new WaitForFixedUpdate();
		InputField.text = "/";
		InputField.MoveTextEnd(shift: false);
	}

	public void ClearInput()
	{
		InputField.text = "";
		oldChatIndex = oldChat.Count;
	}

	public void SendMessageToAll(string content, bool needName)
	{
		string text = content;
		if (needName)
		{
			text = "<" + GameManager.Instance.LocalPlayerSave.playerName + ">" + content;
		}
		AddMessage(text);
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendChatMsg(text);
		}
		if (GameManager.Instance.isClient)
		{
			SocketClient.Instance.SendChatMsg(text);
		}
	}

	public void AddMessage(string content)
	{
		ChatTextGroup.Instance.InputContent(content);
		OutChatTextGroup.Instance.InputContent(content);
	}

	public void AddMessage(string content, Color32 color)
	{
		ChatTextGroup.Instance.InputContent(content, color);
		OutChatTextGroup.Instance.InputContent(content, color);
	}

	private void Command(string InputText)
	{
		InputText = InputText.Remove(0, 1);
		string[] array = InputText.Split(" ");
		try
		{
			switch (array[0].ToLower())
			{
			case "test":
				TestCommand(array);
				break;
			case "sun":
				SunCommand(array);
				break;
			case "summon":
				CommandError("Summon暂不可用");
				break;
			case "time":
				TimeCommand(array);
				break;
			case "cd":
				CDCommand(array);
				break;
			case "weather":
				WeatherCommand(array);
				break;
			case "fill":
				FillCommand(array);
				break;
			case "msg":
				MsgCommand(array);
				break;
			case "kick":
				KickCommand(array);
				break;
			case "kill":
				KillCommand(array);
				break;
			case "gamerule":
				GameruleCommand(array);
				break;
			case "win":
				WinCommand(array);
				break;
			case "card":
				CardCommand(array);
				break;
			case "open":
				OpenCommand(array);
				break;
			case "money":
				MoneyCommand(array);
				break;
			case "unlock":
				UnLockCmd(array);
				break;
			case "create":
				CreateCmd(array);
				break;
			default:
				CommandError();
				break;
			}
		}
		catch
		{
			CommandError();
		}
	}

	private void TestCommand(string[] str)
	{
		if (!PlayerManager.Instance.IsDebug)
		{
			CommandError();
		}
		else if (str[1].ToLower() == "a")
		{
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				MapManager.Instance.mapList[i].DisplayGridSelector();
			}
			get = false;
		}
		else if (str[1].ToLower() == "b")
		{
			get = true;
			new Thread(() =>
			{
				while (get)
				{
					_ = 198;
					_ = 33;
				}
				Debug.Log("Over");
			}).Start();
		}
		else
		{
			CommandError();
		}
	}

	private void CreateCmd(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
			return;
		}
		if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
			return;
		}
		if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
			return;
		}
		PlayerManager.Instance.EnableCreate = !PlayerManager.Instance.EnableCreate;
		if (PlayerManager.Instance.EnableCreate)
		{
			AddMessage("创造面板已开启", new Color32(123, 123, 123, byte.MaxValue));
		}
		else
		{
			AddMessage("创造面板已关闭", new Color32(123, 123, 123, byte.MaxValue));
		}
	}

	private void UnLockCmd(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (LVManager.Instance.InGame)
		{
			CommandError("游戏已开始无法使用该指令");
		}
		else if (str[1].ToLower() == "plant")
		{
			foreach (PlantType value in Enum.GetValues(typeof(PlantType)))
			{
				GameManager.Instance.AddNewPlant(value);
			}
			GameManager.Instance.SaveUserInfo();
			AddMessage("植物已全部解锁", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "level")
		{
			for (int i = 0; i < GameManager.Instance.CurrLvSeries.LvSaves.Count; i++)
			{
				if (GameManager.Instance.CurrLvSeries.LvSaves[i].PassNum <= 0)
				{
					GameManager.Instance.CurrLvSeries.LvSaves[i].PassNum = 1;
				}
			}
			for (int j = 0; j < GameManager.Instance.CurrLvSeries.LvSavesMiniGame.Count; j++)
			{
				if (GameManager.Instance.CurrLvSeries.LvSavesMiniGame[j].PassNum <= 0)
				{
					GameManager.Instance.CurrLvSeries.LvSavesMiniGame[j].PassNum = 1;
				}
			}
			for (int k = 0; k < GameManager.Instance.CurrLvSeries.LvSavesPuzzle.Count; k++)
			{
				if (GameManager.Instance.CurrLvSeries.LvSavesPuzzle[k].PassNum <= 0)
				{
					GameManager.Instance.CurrLvSeries.LvSavesPuzzle[k].PassNum = 1;
				}
			}
			GameManager.Instance.SaveLvInfo(saveCurrLv: false);
			AddMessage("当前系列关卡已全部解锁", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "other")
		{
			GameManager.Instance.LocalPlayerSave.StoreLvl = 2;
			GameManager.Instance.LocalPlayerSave.SwampOpen = true;
			GameManager.Instance.LocalPlayerSave.AlmanacUnLock = true;
			GameManager.Instance.LocalPlayerSave.ShovelUnLock = true;
			GameManager.Instance.SaveUserInfo();
			StartSceneManager.Instance.LoadStartScence(PlayAnim: false);
			AddMessage("商店图鉴沼泽已全部解锁", new Color32(123, 123, 123, byte.MaxValue));
		}
		else
		{
			CommandError();
		}
	}

	private void SunCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (str[1].ToLower() == "set")
		{
			if (int.TryParse(str[2], out var result) && result >= 0)
			{
				PlayerManager.Instance.SetSunNum(result, isSun: true, null);
				PlayerManager.Instance.SetSunNum(result, isSun: false, null);
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "add")
		{
			if (int.TryParse(str[2], out var result2))
			{
				PlayerManager.Instance.AddSunNum(result2, isSun: true, null);
				PlayerManager.Instance.AddSunNum(result2, isSun: false, null);
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "infinite")
		{
			PlayerManager.Instance.SunInfinite = !PlayerManager.Instance.SunInfinite;
			CreatePanel.Instance.RefreshNormalPage();
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (PlayerManager.Instance.SunInfinite)
			{
				AddMessage("无限阳光已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("无限阳光已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "auto")
		{
			SkyManager.Instance.SunAutoCollect = !SkyManager.Instance.SunAutoCollect;
			if (SkyManager.Instance.SunAutoCollect)
			{
				AddMessage("阳光收集已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("阳光收集已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else
		{
			CommandError();
		}
	}

	private void SummonCommand(string[] str)
	{
		int result;
		int result2;
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (int.TryParse(str[1], out result) && int.TryParse(str[2], out result2))
		{
			if (str.Length > 3 && int.TryParse(str[3], out var result3))
			{
				Grid grid = null;
				List<Grid> gridList = CameraControl.Instance.CurrMap.GridList;
				for (int i = 0; i < gridList.Count; i++)
				{
					if (gridList[i].Point.x == result2 - 1 && gridList[i].Point.y == result3 - 1)
					{
						grid = gridList[i];
					}
				}
				if (grid != null)
				{
					ZombieManager.Instance.SummonZombie(result, grid);
				}
			}
			else
			{
				ZombieManager.Instance.SummonZombie(result, result2 - 1, CameraControl.Instance.transform.position);
			}
		}
		else
		{
			CommandError();
		}
	}

	private void TimeCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (str[1].ToLower() == "set")
		{
			if (int.TryParse(str[2], out var result))
			{
				SkyManager.Instance.DirectSetTime(result);
			}
			else if (str[2].ToLower() == "day")
			{
				SkyManager.Instance.DirectSetTime(480);
			}
			else if (str[2].ToLower() == "night")
			{
				SkyManager.Instance.DirectSetTime(1200);
			}
			else
			{
				CommandError();
			}
		}
		else
		{
			CommandError();
		}
	}

	private void CDCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (str[1].ToLower() == "clear")
		{
			SeedBank.Instance.ClearAllCD();
			AddMessage("已清除自己的CD", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "infinite")
		{
			SeedBank.Instance.isNoCD = !SeedBank.Instance.isNoCD;
			CreatePanel.Instance.RefreshNormalPage();
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (SeedBank.Instance.isNoCD)
			{
				AddMessage("无CD已开启", new Color32(123, 123, 123, byte.MaxValue));
				SeedBank.Instance.ClearAllCD();
			}
			else
			{
				AddMessage("无CD已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else
		{
			CommandError();
		}
	}

	private void WeatherCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (SkyManager.Instance.SnowScale > 0)
		{
			CommandError("目前无法使用该指令");
		}
		else if (str[1].ToLower() == "clear")
		{
			SkyManager.Instance.ClearAllWeather(isDirect: false);
			AddMessage("天气更换为晴天", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "rain")
		{
			SkyManager.Instance.SetRainScale(10, isDirect: false);
			AddMessage("天气更换为雨天", new Color32(123, 123, 123, byte.MaxValue));
		}
		else if (str[1].ToLower() == "thunder")
		{
			SkyManager.Instance.SetRainScale(10, isDirect: false);
			SkyManager.Instance.IsThunder = true;
			AddMessage("天气更换为雷雨天", new Color32(123, 123, 123, byte.MaxValue));
		}
		else
		{
			CommandError();
		}
	}

	private void FillCommand(string[] str)
	{
		int result;
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (int.TryParse(str[1], out result))
		{
			PlantType cardType = SeedChooser.Instance.GetCardType(result);
			if (cardType != PlantType.Nope && CameraControl.Instance.CurrMap != null)
			{
				if (int.TryParse(str[2], out var result2) && int.TryParse(str[3], out var result3) && int.TryParse(str[4], out var result4) && int.TryParse(str[5], out var result5))
				{
					int num = 0;
					List<Grid> gridList = CameraControl.Instance.CurrMap.GridList;
					int num2 = ((result2 < result4) ? result2 : result4);
					int num3 = ((result2 > result4) ? result2 : result4);
					int num4 = ((result3 < result5) ? result3 : result5);
					int num5 = ((result3 > result5) ? result3 : result5);
					for (int i = 0; i < gridList.Count; i++)
					{
						if (gridList[i].Point.x >= num2 - 1 && gridList[i].Point.x < num3 && gridList[i].Point.y >= num4 - 1 && gridList[i].Point.y < num5)
						{
							PlantBase newPlant = PlantManager.Instance.GetNewPlant(cardType);
							if (SeedBank.Instance.CheckPlant(newPlant, gridList[i], -2, null))
							{
								num++;
								SeedBank.Instance.PlantConfirm(newPlant, gridList[i], -2, 0, null);
							}
							else
							{
								UnityEngine.Object.Destroy(newPlant.gameObject);
							}
						}
					}
					AddMessage("已种植" + num + "个植物", new Color32(123, 123, 123, byte.MaxValue));
				}
				else
				{
					CommandError();
				}
			}
			else
			{
				CommandError();
			}
		}
		else
		{
			CommandError();
		}
	}

	private void MsgCommand(string[] str)
	{
		if (str.Length > 2)
		{
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendPrivateChatMsg(str[1], str[2], GameManager.Instance.LocalPlayerSave.playerName);
			}
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.SendPrivateChatMsg(str[1], str[2]);
			}
			string content = "你悄悄对玩家" + str[1] + "说:" + str[2];
			AddMessage(content, new Color32(123, 123, 123, byte.MaxValue));
		}
		else
		{
			CommandError();
		}
	}

	private void KickCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.KickPlayer(str[1]);
		}
	}

	private void WinCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
			return;
		}
		if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
			return;
		}
		if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
			return;
		}
		LVManager.Instance.SpawnBooty(new Vector3(CameraControl.Instance.transform.position.x, CameraControl.Instance.transform.position.y, 0f));
		if (LVManager.Instance.OnlyBooty != null)
		{
			LVManager.Instance.OnlyBooty.CollectBooty();
		}
	}

	private void CardCommand(string[] str)
	{
		int result;
		int result2;
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (GameManager.Instance.isOnline)
		{
			CommandError("多人暂不可用");
		}
		else if (int.TryParse(str[1], out result) && int.TryParse(str[2], out result2))
		{
			SeedBank.Instance.ChangeCard(result2, result);
		}
		else
		{
			CommandError();
		}
	}

	private void KillCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (str[1].ToLower() == "plant")
		{
			if (str[2].ToLower() == "all")
			{
				AddMessage("已杀死" + PlantManager.Instance.plants.Count + "个植物", new Color32(123, 123, 123, byte.MaxValue));
				PlantManager.Instance.KillAllPlant();
			}
			else if (str[2].ToLower() == "there")
			{
				AddMessage("已杀死" + PlantManager.Instance.ClearMapPlant(CameraControl.Instance.CurrMap) + "个植物", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "zombie")
		{
			if (str[2].ToLower() == "all")
			{
				AddMessage("已杀死" + ZombieManager.Instance.BigHurtAllZombie() + "个僵尸", new Color32(123, 123, 123, byte.MaxValue));
			}
			else if (str[2].ToLower() == "there")
			{
				AddMessage("已杀死" + ZombieManager.Instance.BigHurtMapZombie(CameraControl.Instance.CurrMap) + "个僵尸", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				CommandError();
			}
		}
		else
		{
			CommandError();
		}
	}

	private void GameruleCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (!LVManager.Instance.InGame || CreatePanel.Instance.IsCustomMode)
		{
			CommandError("游戏未开始无法使用该指令");
		}
		else if (str[1].ToLower() == "plantinvincible")
		{
			PlantManager.Instance.PlantInvincible = !PlantManager.Instance.PlantInvincible;
			CreatePanel.Instance.RefreshNormalPage();
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (PlantManager.Instance.PlantInvincible)
			{
				AddMessage("植物无敌已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("植物无敌已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "zombieinvincible")
		{
			ZombieManager.Instance.ZombieInvincible = !ZombieManager.Instance.ZombieInvincible;
			CreatePanel.Instance.RefreshNormalPage();
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (ZombieManager.Instance.ZombieInvincible)
			{
				AddMessage("僵尸无敌已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("僵尸无敌已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "plantdontsleep")
		{
			PlantManager.Instance.PlantDontSleep = !PlantManager.Instance.PlantDontSleep;
			CreatePanel.Instance.RefreshNormalPage();
			if (PlantManager.Instance.PlantDontSleep)
			{
				AddMessage("植物不睡觉已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("植物不睡觉已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "dontfail")
		{
			LVManager.Instance.LvDontFail = !LVManager.Instance.LvDontFail;
			if (LVManager.Instance.LvDontFail)
			{
				AddMessage("关卡不失败已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("关卡不失败已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else if (str[1].ToLower() == "daylightcycle")
		{
			SkyManager.Instance.DayLightCycle = !SkyManager.Instance.DayLightCycle;
			CreatePanel.Instance.RefreshDayCycle();
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.SendCommandBag();
			}
			if (SkyManager.Instance.DayLightCycle)
			{
				AddMessage("日夜循环已开启", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				AddMessage("日夜循环已关闭", new Color32(123, 123, 123, byte.MaxValue));
			}
		}
		else
		{
			CommandError();
		}
	}

	private void OpenCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (GameManager.Instance.isOnline)
		{
			CommandError("多人游戏中无法使用");
		}
		else if (GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令已启用，无法关闭");
		}
		else if (!OpenCmdFirst)
		{
			OpenCmdFirst = true;
			AddMessage("再次输入/open确认启用指令。该存档开启后将无法关闭，指令启用与指令未启用的存档将无法联机！", new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
		}
		else
		{
			GameManager.Instance.LocalPlayerSave.CmdEnable = true;
			GameManager.Instance.SaveUserInfo();
			AddMessage("指令已启用", new Color32(123, 123, 123, byte.MaxValue));
		}
	}

	private void MoneyCommand(string[] str)
	{
		if (GameManager.Instance.isClient)
		{
			CommandError("无指令使用权限");
		}
		else if (!GameManager.Instance.LocalPlayerSave.CmdEnable)
		{
			CommandError("指令未启用，无法使用，请输入/open来启用指令");
		}
		else if (str[1].ToLower() == "set")
		{
			if (int.TryParse(str[2], out var result) && result >= 0)
			{
				PlayerManager.Instance.Money = result;
				AddMessage("金钱已更改", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				CommandError();
			}
		}
		else if (str[1].ToLower() == "add")
		{
			if (int.TryParse(str[2], out var result2))
			{
				result2 = Mathf.Abs(result2);
				PlayerManager.Instance.Money += result2;
				AddMessage("金钱已增加", new Color32(123, 123, 123, byte.MaxValue));
			}
			else
			{
				CommandError();
			}
		}
	}

	private void CommandError(string content = "")
	{
		if (content == "")
		{
			AddMessage("请输入正确的命令", new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
		}
		else
		{
			AddMessage(content, new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
		}
	}
}
