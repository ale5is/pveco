using System.Collections.Generic;
using System.IO;
using SaveClass;
using StartScene;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public string SavePath;

	public string lastSavePath;

	public bool isOnline;

	public bool isAndroid;

	public string VersionCode = "0.7";

	public string HostName;

	public UserSave LocalPlayerSave;

	public LvSeries CurrLvSeries;

	public StatsAndAcvSave StatsAcvSave;

	public static char[] keyChars = new char[11]
	{
		's', 'm', 'a', 'r', 't', 'f', 'a', 'l', 'c', 'o',
		'n'
	};

	public GameConf GameConf { get; private set; }

	public AudioConf AudioConf { get; private set; }

	public bool isServer
	{
		get
		{
			if (isOnline)
			{
				return SocketServer.Instance.isServerOpen;
			}
			return false;
		}
	}

	public bool isClient
	{
		get
		{
			if (isOnline)
			{
				return !SocketServer.Instance.isServerOpen;
			}
			return false;
		}
	}

	public MapStoneBase SelectedStone => SelectMap.Instance.SelectedStone;

	public static string Encrypt(string data)
	{
		char[] array = data.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char num = array[i];
			char c = keyChars[i % keyChars.Length];
			char c2 = (char)(num ^ c);
			array[i] = c2;
		}
		return new string(array);
	}

	public static string Decrypt(string data)
	{
		return Encrypt(data);
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			GameConf = Resources.Load<GameConf>("GameConf");
			AudioConf = Resources.Load<AudioConf>("AudioConf");
			SavePath = Application.persistentDataPath + "/saves";
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (isAndroid)
		{
			Screen.SetResolution(1920, 1080, fullscreen: false);
		}
		LoadSetting();
		bool flag = false;
		if (lastSavePath != null && File.Exists(lastSavePath + "/" + FixedInfo.PlayerInfoName))
		{
			StreamReader streamReader = new StreamReader(lastSavePath + "/" + FixedInfo.PlayerInfoName);
			string json = Decrypt(streamReader.ReadToEnd());
			streamReader.Close();
			if (MyTool.TryParseJson<UserSave>(json, out var result))
			{
				LoadSave(result, lastSavePath);
				flag = true;
			}
		}
		if (!flag)
		{
			if (!Directory.Exists(SavePath))
			{
				Directory.CreateDirectory(SavePath);
			}
			DirectoryInfo[] directories = new DirectoryInfo(SavePath).GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				if (File.Exists(directories[i]?.ToString() + "/" + FixedInfo.PlayerInfoName))
				{
					StreamReader streamReader2 = new StreamReader(directories[i]?.ToString() + "/" + FixedInfo.PlayerInfoName);
					string json2 = Decrypt(streamReader2.ReadToEnd());
					streamReader2.Close();
					if (MyTool.TryParseJson<UserSave>(json2, out var result2))
					{
						LoadSave(result2, directories[i].ToString());
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			ChooseSave.Instance.AddUser.Display(canCancel: false);
		}
		StatsManager.Instance.AddStatsNum(StatsEnum.GameOpenNum);
	}

	private void FirstLoadLvSeries()
	{
		int num = LocalPlayerSave.LastAdventureId / 10000;
		if (num > 0 && num <= SelectMap.Instance.Stones.Count)
		{
			SelectMap.Instance.Stones[num - 1].SelectThis();
			LoadLvInfo(num);
		}
		else
		{
			SelectMap.Instance.Stones[0].SelectThis();
			LoadLvInfo(1);
		}
		LevelSelector.Instance.LoadLastLv();
	}

	public void LoadSave(UserSave saveUser, string loadingPath)
	{
		if (lastSavePath != loadingPath)
		{
			lastSavePath = loadingPath;
			SaveSetting();
		}
		LocalPlayerSave = saveUser;
		ChatInput.Instance.ResetAllCmd();
		if (!LocalPlayerSave.UnlockedPlants.Contains(PlantType.PeaShooter))
		{
			LocalPlayerSave.UnlockedPlants.Add(PlantType.PeaShooter);
		}
		if (!LocalPlayerSave.UnlockedPlants.Contains(PlantType.SunFlower))
		{
			LocalPlayerSave.UnlockedPlants.Add(PlantType.SunFlower);
		}
		if (LocalPlayerSave.CardSlotNum < 6)
		{
			LocalPlayerSave.CardSlotNum = 6;
		}
		if (LocalPlayerSave.LastAdventureId <= 10000)
		{
			LocalPlayerSave.LastAdventureId = 10001;
		}
		if (LocalPlayerSave.LastMiniGameId <= 11000)
		{
			LocalPlayerSave.LastMiniGameId = 11001;
		}
		if (LocalPlayerSave.LastPuzzleId <= 12000)
		{
			LocalPlayerSave.LastPuzzleId = 12001;
		}
		if (LocalPlayerSave.MoreOptions.Count <= 0)
		{
			for (int i = 0; i < 10; i++)
			{
				LocalPlayerSave.MoreOptions.Add(item: false);
			}
		}
		PlayerManager.Instance.ReadSave(LocalPlayerSave.MoneyNum);
		StartSceneManager.Instance.changeUser.NameText.text = saveUser.playerName + "!";
		UIManager.Instance.SetPanel.MoreOptionRead(LocalPlayerSave.MoreOptions, LocalPlayerSave.OpenQuickChat, LocalPlayerSave.QuickChat);
		SeedBank.Instance.LastSelectCard = LocalPlayerSave.LastSelectedCard;
		SaveUserInfo();
		StartSceneManager.Instance.LoadStartScence(PlayAnim: false);
		FirstLoadLvSeries();
		bool flag = false;
		if (File.Exists(lastSavePath + "/" + FixedInfo.SAAInfo))
		{
			StreamReader streamReader = new StreamReader(lastSavePath + "/" + FixedInfo.SAAInfo);
			string json = Decrypt(streamReader.ReadToEnd());
			streamReader.Close();
			if (MyTool.TryParseJson<StatsAndAcvSave>(json, out var result))
			{
				flag = true;
				StatsAcvSave = result;
			}
		}
		if (!flag)
		{
			StatsAcvSave = new StatsAndAcvSave();
		}
		StatsManager.Instance.LoadStatsSave();
		AcvmentManager.Instance.LoadAcvSave();
		if (LocalPlayerSave.UnlockedPlants.Count >= 49)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.Plant49);
		}
	}

	public void SaveUserInfo()
	{
		LocalPlayerSave.MoneyNum = PlayerManager.Instance.Money;
		LocalPlayerSave.VersionCode = VersionCode;
		LocalPlayerSave.MoreOptions.Clear();
		LocalPlayerSave.MoreOptions.Add(GobalLight.Instance.LightingNotDark);
		LocalPlayerSave.MoreOptions.Add(SeedBank.Instance.IsCdDown);
		LocalPlayerSave.MoreOptions.Add(PlayerManager.Instance.NormalSunUp);
		LocalPlayerSave.MoreOptions.Add(PlayerManager.Instance.GetSunUp);
		LocalPlayerSave.MoreOptions.Add(SkyManager.Instance.SlowSunAutoCollect);
		LocalPlayerSave.LastSelectedCard = new List<CardType>(SeedBank.Instance.LastSelectCard);
		LocalPlayerSave.OpenQuickChat = UIManager.Instance.SetPanel.OpenQuickChat;
		LocalPlayerSave.QuickChat.Clear();
		LocalPlayerSave.QuickChat.Add(UIManager.Instance.SetPanel.QuickChat1.text);
		LocalPlayerSave.QuickChat.Add(UIManager.Instance.SetPanel.QuickChat2.text);
		LocalPlayerSave.QuickChat.Add(UIManager.Instance.SetPanel.QuickChat3.text);
		string value = Encrypt(JsonUtility.ToJson(LocalPlayerSave));
		StreamWriter streamWriter = new StreamWriter(lastSavePath + "/" + FixedInfo.PlayerInfoName);
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public void SaveSAInfo()
	{
		string value = Encrypt(JsonUtility.ToJson(StatsAcvSave));
		StreamWriter streamWriter = new StreamWriter(lastSavePath + "/" + FixedInfo.SAAInfo);
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public void AddNewPlant(PlantType plantType)
	{
		if (plantType != PlantType.Nope && !new List<PlantType>
		{
			PlantType.MoonTombStone,
			PlantType.RepeaterReverse,
			PlantType.ExplodeNut,
			PlantType.HugeNut
		}.Contains(plantType) && !LocalPlayerSave.UnlockedPlants.Contains(plantType))
		{
			LocalPlayerSave.UnlockedPlants.Add(plantType);
			SaveUserInfo();
			if (LocalPlayerSave.UnlockedPlants.Count >= 49)
			{
				AcvmentManager.Instance.GetAchievement(Acvname.Plant49);
			}
		}
	}

	public LvSave GetLvSave(int LVid)
	{
		int num = LVid / 10000;
		int num2 = CurrLvSeries.LvSaves[0].LvId / 10000;
		if (num != num2)
		{
			LoadLvInfo(num);
		}
		List<LvSave> lvSaves = GetLvSaves(LV.Instance.CurrLvId);
		for (int i = 0; i < lvSaves.Count; i++)
		{
			if (lvSaves[i].LvId == LVid)
			{
				return lvSaves[i];
			}
		}
		return null;
	}

	public void LoadLvInfo(int SeriesId)
	{
		bool flag = false;
		if (lastSavePath != null && File.Exists(lastSavePath + "/Wsave" + SeriesId + ".smf"))
		{
			StreamReader streamReader = new StreamReader(lastSavePath + "/Wsave" + SeriesId + ".smf");
			string json = Decrypt(streamReader.ReadToEnd());
			streamReader.Close();
			if (MyTool.TryParseJson<LvSeries>(json, out var result))
			{
				flag = true;
				CurrLvSeries = result;
				if (CurrLvSeries.LvSaves.Count < SelectMap.Instance.SelectedStone.AdventureLvNum)
				{
					int count = CurrLvSeries.LvSaves.Count;
					int num = SelectMap.Instance.SelectedStone.AdventureLvNum - CurrLvSeries.LvSaves.Count;
					for (int i = 0; i < num; i++)
					{
						LvSave lvSave = new LvSave();
						lvSave.LvId = SeriesId * 10000 + 1 + i + count;
						CurrLvSeries.LvSaves.Add(lvSave);
					}
				}
				if (CurrLvSeries.LvSavesMiniGame.Count < SelectMap.Instance.SelectedStone.MiniGameLvNum)
				{
					int count2 = CurrLvSeries.LvSavesMiniGame.Count;
					int num2 = SelectMap.Instance.SelectedStone.MiniGameLvNum - CurrLvSeries.LvSavesMiniGame.Count;
					for (int j = 0; j < num2; j++)
					{
						LvSave lvSave2 = new LvSave();
						lvSave2.LvId = SeriesId * 11000 + 1 + j + count2;
						CurrLvSeries.LvSavesMiniGame.Add(lvSave2);
					}
				}
				while (CurrLvSeries.LvSavesMiniGame.Count > SelectMap.Instance.SelectedStone.MiniGameLvNum)
				{
					CurrLvSeries.LvSavesMiniGame.RemoveAt(CurrLvSeries.LvSavesMiniGame.Count - 1);
				}
				if (CurrLvSeries.LvSavesPuzzle.Count < SelectMap.Instance.SelectedStone.PuzzleLvNum)
				{
					int count3 = CurrLvSeries.LvSavesPuzzle.Count;
					int num3 = SelectMap.Instance.SelectedStone.PuzzleLvNum - CurrLvSeries.LvSavesPuzzle.Count;
					for (int k = 0; k < num3; k++)
					{
						LvSave lvSave3 = new LvSave();
						lvSave3.LvId = SeriesId * 12000 + 1 + k + count3;
						CurrLvSeries.LvSavesPuzzle.Add(lvSave3);
					}
				}
			}
		}
		if (!flag)
		{
			CurrLvSeries = new LvSeries();
			for (int l = 0; l < SelectMap.Instance.SelectedStone.AdventureLvNum; l++)
			{
				LvSave lvSave4 = new LvSave();
				lvSave4.LvId = SeriesId * 10000 + 1 + l;
				CurrLvSeries.LvSaves.Add(lvSave4);
			}
			for (int m = 0; m < SelectMap.Instance.SelectedStone.MiniGameLvNum; m++)
			{
				LvSave lvSave5 = new LvSave();
				lvSave5.LvId = SeriesId * 11000 + 1 + m;
				CurrLvSeries.LvSavesMiniGame.Add(lvSave5);
			}
			for (int n = 0; n < SelectMap.Instance.SelectedStone.PuzzleLvNum; n++)
			{
				LvSave lvSave6 = new LvSave();
				lvSave6.LvId = SeriesId * 12000 + 1 + n;
				CurrLvSeries.LvSavesPuzzle.Add(lvSave6);
			}
		}
	}

	public void SaveLvInfo(bool saveCurrLv)
	{
		Debug.Log(LV.Instance.CurrLvId);
		List<LvSave> lvSaves = GetLvSaves(LV.Instance.CurrLvId);
		if (saveCurrLv)
		{
			for (int i = 0; i < lvSaves.Count; i++)
			{
				if (lvSaves[i].LvId != LV.Instance.CurrLvId)
				{
					continue;
				}
				if (LV.Instance.IsEasy)
				{
					if ((lvSaves[i].PassTime <= 10 || LVManager.Instance.PassTime < lvSaves[i].PassTime) && LVManager.Instance.PassTime > 10)
					{
						lvSaves[i].PassTime = LVManager.Instance.PassTime;
					}
					lvSaves[i].PassNum++;
				}
				else
				{
					if ((lvSaves[i].HardPTime <= 10 || LVManager.Instance.PassTime < lvSaves[i].HardPTime) && LVManager.Instance.PassTime > 10)
					{
						lvSaves[i].HardPTime = LVManager.Instance.PassTime;
					}
					lvSaves[i].HardPNum++;
				}
				break;
			}
		}
		int num = lvSaves[0].LvId / 10000;
		string value = Encrypt(JsonUtility.ToJson(CurrLvSeries));
		StreamWriter streamWriter = new StreamWriter(lastSavePath + "/Wsave" + num + ".smf");
		streamWriter.Write(value);
		streamWriter.Close();
	}

	private List<LvSave> GetLvSaves(int lvid)
	{
		return (lvid % 10000 / 1000) switch
		{
			1 => CurrLvSeries.LvSavesMiniGame, 
			2 => CurrLvSeries.LvSavesPuzzle, 
			_ => CurrLvSeries.LvSaves, 
		};
	}

	private void LoadSetting()
	{
		if (File.Exists(SavePath + "/" + FixedInfo.SettingInfoName))
		{
			StreamReader streamReader = new StreamReader(SavePath + "/" + FixedInfo.SettingInfoName);
			string json = streamReader.ReadToEnd();
			streamReader.Close();
			if (MyTool.TryParseJson<SettingSave>(json, out var result))
			{
				UIManager.Instance.SetPanel.SaveInit(result);
				lastSavePath = result.lastLoadSavePath;
				Screen.fullScreen = result.isFullScreen;
			}
		}
	}

	public void SaveSetting()
	{
		SettingSave obj = new SettingSave
		{
			bgmVolume = AudioManager.Instance.BgmVolume,
			soundVolume = AudioManager.Instance.SoundVolume,
			lastLoadSavePath = lastSavePath,
			isFullScreen = Screen.fullScreen,
			isF1080P = UIManager.Instance.SetPanel.is1080P,
			isRainFog = SkyManager.Instance.isRainFog,
			isCardSelector = SeedBank.Instance.CardSelector,
			FrameType = UIManager.Instance.SetPanel.FrameType,
			isVsync = UIManager.Instance.SetPanel.isVsync,
			isDisFrame = UIManager.Instance.SetPanel.isDisFrame
		};
		if (!Directory.Exists(SavePath))
		{
			Directory.CreateDirectory(SavePath);
		}
		string value = JsonUtility.ToJson(obj);
		StreamWriter streamWriter = new StreamWriter(SavePath + "/" + FixedInfo.SettingInfoName);
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public List<CustomMapSave> LoadCustomMapFile()
	{
		List<CustomMapSave> list = new List<CustomMapSave>();
		string path = Application.persistentDataPath + "/custom/maps";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string[] files = Directory.GetFiles(path, "*.cm");
		while (true)
		{
			for (int i = 0; i < files.Length; i++)
			{
				if (File.Exists(files[i]))
				{
					StreamReader streamReader = new StreamReader(files[i]);
					string json = Decrypt(streamReader.ReadToEnd());
					streamReader.Close();
					if (MyTool.TryParseJson<CustomMapSave>(json, out var result))
					{
						CustomMapSave customMapSave = result;
						customMapSave.SavePath = files[i];
						list.Add(customMapSave);
					}
				}
			}
			if (files.Length != 0 && list.Count != 0)
			{
				break;
			}
			CreateNewCustomMap();
			files = Directory.GetFiles(path, "*.cm");
		}
		return list;
	}

	public void CreateNewCustomMap()
	{
		string text = Application.persistentDataPath + "/custom/maps";
		CustomMapSave customMapSave = new CustomMapSave();
		customMapSave.MapBrief = "一个新的自定义地图";
		customMapSave.mapType = MapType.CustomYard;
		customMapSave.VerticalNum = 5;
		customMapSave.HorizontalNum = 9;
		for (int i = 0; i < 45; i++)
		{
			customMapSave.tileTypes.Add(TileType.Grass);
		}
		int num = 0;
		string text2 = "";
		while (File.Exists(text + "/newmap" + text2 + ".cm"))
		{
			num++;
			text2 = ((num > 0) ? num.ToString() : "");
		}
		customMapSave.MapName = "newmap" + text2;
		string value = Encrypt(JsonUtility.ToJson(customMapSave));
		StreamWriter streamWriter = new StreamWriter(text + "/newmap" + text2 + ".cm");
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public List<CustomLevelSave> LoadCustomLevelFile()
	{
		List<CustomLevelSave> list = new List<CustomLevelSave>();
		string path = Application.persistentDataPath + "/custom/levels";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string[] files = Directory.GetFiles(path, "*.clv");
		while (true)
		{
			for (int i = 0; i < files.Length; i++)
			{
				if (File.Exists(files[i]))
				{
					StreamReader streamReader = new StreamReader(files[i]);
					string json = Decrypt(streamReader.ReadToEnd());
					streamReader.Close();
					if (MyTool.TryParseJson<CustomLevelSave>(json, out var result))
					{
						CustomLevelSave customLevelSave = result;
						customLevelSave.SavePath = files[i];
						list.Add(customLevelSave);
					}
				}
			}
			if (files.Length != 0 && list.Count != 0)
			{
				break;
			}
			CreateNewCustomLv();
			files = Directory.GetFiles(path, "*.clv");
		}
		return list;
	}

	public void CreateNewCustomLv()
	{
		string text = Application.persistentDataPath + "/custom/levels";
		CustomLevelSave customLevelSave = new CustomLevelSave();
		customLevelSave.LvBrief = "一个新的自定义关卡";
		customLevelSave.lVType = LVType.Normal;
		customLevelSave.series = MapSeries.Yard;
		customLevelSave.mapTypes = new List<MapType> { MapType.FrontYard };
		int num = 0;
		string text2 = "";
		while (File.Exists(text + "/newlv" + text2 + ".clv"))
		{
			num++;
			text2 = ((num > 0) ? num.ToString() : "");
		}
		customLevelSave.LvName = "newlv" + text2;
		string value = Encrypt(JsonUtility.ToJson(customLevelSave));
		StreamWriter streamWriter = new StreamWriter(text + "/newlv" + text2 + ".clv");
		streamWriter.Write(value);
		streamWriter.Close();
	}
}
