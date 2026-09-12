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

    public static char[] keyChars =
    {
        's', 'm', 'a', 'r', 't', 'f', 'a', 'l', 'c', 'o', 'n'
    };

    public GameConf GameConf { get; private set; }
    public AudioConf AudioConf { get; private set; }

    public bool isServer =>
        isOnline &&
        SocketServer.Instance != null &&
        SocketServer.Instance.isServerOpen;

    public bool isClient =>
        isOnline &&
        SocketServer.Instance != null &&
        !SocketServer.Instance.isServerOpen;

    public MapStoneBase SelectedStone =>
        SelectMap.Instance.SelectedStone;

    public static string Encrypt(string data)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        char[] chars = data.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(chars[i] ^ keyChars[i % keyChars.Length]);
        }

        return new string(chars);
    }

    public static string Decrypt(string data)
    {
        return Encrypt(data);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GameConf = Resources.Load<GameConf>("GameConf");
        AudioConf = Resources.Load<AudioConf>("AudioConf");

        SavePath = Application.persistentDataPath + "/saves";

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (isAndroid)
            Screen.SetResolution(1920, 1080, false);

        LoadSetting();

        bool loaded = false;

        if (!string.IsNullOrEmpty(lastSavePath))
        {
            string file = lastSavePath + "/" + FixedInfo.PlayerInfoName;

            if (File.Exists(file) &&
                MyTool.TryParseJson<UserSave>(
                    Decrypt(File.ReadAllText(file)),
                    out var save))
            {
                LoadSave(save, lastSavePath);
                loaded = true;
            }
        }

        if (!loaded)
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            foreach (DirectoryInfo dir in new DirectoryInfo(SavePath).GetDirectories())
            {
                string file = dir.FullName + "/" + FixedInfo.PlayerInfoName;

                if (!File.Exists(file))
                    continue;

                if (MyTool.TryParseJson<UserSave>(
                    Decrypt(File.ReadAllText(file)),
                    out var save))
                {
                    LoadSave(save, dir.FullName);
                    loaded = true;
                    break;
                }
            }
        }

        if (!loaded)
        {
            if (ChooseSave.Instance != null &&
                ChooseSave.Instance.AddUser != null)
            {
                ChooseSave.Instance.AddUser.Display(false);
            }

            return;
        }

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.AddStatsNum(
                StatsEnum.GameOpenNum);
        }
    }

    private void FirstLoadLvSeries()
    {
        int series = LocalPlayerSave.LastAdventureId / 10000;

        if (series > 0 &&
            series <= SelectMap.Instance.Stones.Count)
        {
            SelectMap.Instance.Stones[series - 1].SelectThis();
            LoadLvInfo(series);
        }
        else
        {
            SelectMap.Instance.Stones[0].SelectThis();
            LoadLvInfo(1);
        }

        LevelSelector.Instance.LoadLastLv();
    }

    public void LoadSave(UserSave save, string path)
    {
        if (save == null)
            return;

        if (lastSavePath != path)
        {
            lastSavePath = path;
            SaveSetting();
        }

        LocalPlayerSave = save;

        if (ChatInput.Instance != null)
            ChatInput.Instance.ResetAllCmd();

        if (!LocalPlayerSave.UnlockedPlants.Contains(PlantType.PeaShooter))
            LocalPlayerSave.UnlockedPlants.Add(PlantType.PeaShooter);

        if (!LocalPlayerSave.UnlockedPlants.Contains(PlantType.SunFlower))
            LocalPlayerSave.UnlockedPlants.Add(PlantType.SunFlower);

        if (LocalPlayerSave.CardSlotNum < 6)
            LocalPlayerSave.CardSlotNum = 6;

        if (LocalPlayerSave.LastAdventureId <= 10000)
            LocalPlayerSave.LastAdventureId = 10001;

        if (LocalPlayerSave.LastMiniGameId <= 11000)
            LocalPlayerSave.LastMiniGameId = 11001;

        if (LocalPlayerSave.LastPuzzleId <= 12000)
            LocalPlayerSave.LastPuzzleId = 12001;

        if (LocalPlayerSave.MoreOptions.Count == 0)
        {
            for (int i = 0; i < 10; i++)
                LocalPlayerSave.MoreOptions.Add(false);
        }

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ReadSave(
                LocalPlayerSave.MoneyNum);
        }

        if (StartSceneManager.Instance != null &&
            StartSceneManager.Instance.changeUser != null &&
            StartSceneManager.Instance.changeUser.NameText != null)
        {
            StartSceneManager.Instance.changeUser.NameText.text =
                LocalPlayerSave.playerName + "!";
        }

        if (UIManager.Instance != null &&
            UIManager.Instance.SetPanel != null)
        {
            UIManager.Instance.SetPanel.MoreOptionRead(
                LocalPlayerSave.MoreOptions,
                LocalPlayerSave.OpenQuickChat,
                LocalPlayerSave.QuickChat);
        }

        if (SeedBank.Instance != null)
        {
            SeedBank.Instance.LastSelectCard =
                LocalPlayerSave.LastSelectedCard;
        }

        SaveUserInfo();

        if (StartSceneManager.Instance != null)
        {
            StartSceneManager.Instance.LoadStartScence(false);
        }

        FirstLoadLvSeries();

        string file = lastSavePath + "/" + FixedInfo.SAAInfo;

        if (File.Exists(file) &&
            MyTool.TryParseJson<StatsAndAcvSave>(
                Decrypt(File.ReadAllText(file)),
                out var stats))
        {
            StatsAcvSave = stats;
        }
        else
        {
            StatsAcvSave = new StatsAndAcvSave();
        }

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.LoadStatsSave();
        }

        if (AcvmentManager.Instance != null)
        {
            AcvmentManager.Instance.LoadAcvSave();
        }

        if (LocalPlayerSave.UnlockedPlants.Count >= 49 &&
            AcvmentManager.Instance != null)
        {
            AcvmentManager.Instance.GetAchievement(
                Acvname.Plant49);
        }
    }

    public void SaveUserInfo()
    {
        if (LocalPlayerSave == null)
            return;

        LocalPlayerSave.MoneyNum =
            PlayerManager.Instance.Money;

        LocalPlayerSave.VersionCode =
            VersionCode;

        LocalPlayerSave.MoreOptions.Clear();

        LocalPlayerSave.MoreOptions.Add(
            GobalLight.Instance.LightingNotDark);

        LocalPlayerSave.MoreOptions.Add(
            SeedBank.Instance.IsCdDown);

        LocalPlayerSave.MoreOptions.Add(
            PlayerManager.Instance.NormalSunUp);

        LocalPlayerSave.MoreOptions.Add(
            PlayerManager.Instance.GetSunUp);

        LocalPlayerSave.MoreOptions.Add(
            SkyManager.Instance.SlowSunAutoCollect);

        LocalPlayerSave.LastSelectedCard =
            new List<CardType>(
                SeedBank.Instance.LastSelectCard);

        LocalPlayerSave.OpenQuickChat =
            UIManager.Instance.SetPanel.OpenQuickChat;

        LocalPlayerSave.QuickChat.Clear();

        LocalPlayerSave.QuickChat.Add(
            UIManager.Instance.SetPanel.QuickChat1.text);

        LocalPlayerSave.QuickChat.Add(
            UIManager.Instance.SetPanel.QuickChat2.text);

        LocalPlayerSave.QuickChat.Add(
            UIManager.Instance.SetPanel.QuickChat3.text);

        if (!Directory.Exists(lastSavePath))
            Directory.CreateDirectory(lastSavePath);

        File.WriteAllText(
            lastSavePath + "/" + FixedInfo.PlayerInfoName,
            Encrypt(JsonUtility.ToJson(LocalPlayerSave)));
    }

    public void SaveSAInfo()
    {
        if (StatsAcvSave == null ||
            string.IsNullOrEmpty(lastSavePath))
            return;

        File.WriteAllText(
            lastSavePath + "/" + FixedInfo.SAAInfo,
            Encrypt(JsonUtility.ToJson(StatsAcvSave)));
    }

    public void AddNewPlant(PlantType plantType)
    {
        if (LocalPlayerSave == null)
            return;

        if (plantType == PlantType.Nope ||
            plantType == PlantType.MoonTombStone ||
            plantType == PlantType.RepeaterReverse ||
            plantType == PlantType.ExplodeNut ||
            plantType == PlantType.HugeNut ||
            LocalPlayerSave.UnlockedPlants.Contains(plantType))
        {
            return;
        }

        LocalPlayerSave.UnlockedPlants.Add(plantType);

        SaveUserInfo();

        if (LocalPlayerSave.UnlockedPlants.Count >= 49 &&
            AcvmentManager.Instance != null)
        {
            AcvmentManager.Instance.GetAchievement(
                Acvname.Plant49);
        }
    }

    public LvSave GetLvSave(int LVid)
    {
        if (CurrLvSeries == null ||
            CurrLvSeries.LvSaves == null ||
            CurrLvSeries.LvSaves.Count == 0)
        {
            return null;
        }

        int series = LVid / 10000;
        int currentSeries =
            CurrLvSeries.LvSaves[0].LvId / 10000;

        if (series != currentSeries)
            LoadLvInfo(series);

        foreach (LvSave save in GetLvSaves(LV.Instance.CurrLvId))
        {
            if (save.LvId == LVid)
                return save;
        }

        return null;
    }

    public void LoadLvInfo(int SeriesId)
    {
        if (SelectMap.Instance == null ||
            SelectMap.Instance.SelectedStone == null)
        {
            return;
        }

        string file =
            lastSavePath + "/Wsave" + SeriesId + ".smf";

        bool loaded = false;

        if (File.Exists(file) &&
            MyTool.TryParseJson<LvSeries>(
                Decrypt(File.ReadAllText(file)),
                out var series))
        {
            CurrLvSeries = series;
            loaded = true;
        }

        if (!loaded)
            CurrLvSeries = new LvSeries();

        int adventure =
            SelectMap.Instance.SelectedStone.AdventureLvNum;

        int miniGame =
            SelectMap.Instance.SelectedStone.MiniGameLvNum;

        int puzzle =
            SelectMap.Instance.SelectedStone.PuzzleLvNum;

        while (CurrLvSeries.LvSaves.Count < adventure)
        {
            CurrLvSeries.LvSaves.Add(
                new LvSave
                {
                    LvId =
                        SeriesId * 10000 +
                        CurrLvSeries.LvSaves.Count + 1
                });
        }

        while (CurrLvSeries.LvSavesMiniGame.Count < miniGame)
        {
            CurrLvSeries.LvSavesMiniGame.Add(
                new LvSave
                {
                    LvId =
                        SeriesId * 11000 +
                        CurrLvSeries.LvSavesMiniGame.Count + 1
                });
        }

        while (CurrLvSeries.LvSavesPuzzle.Count < puzzle)
        {
            CurrLvSeries.LvSavesPuzzle.Add(
                new LvSave
                {
                    LvId =
                        SeriesId * 12000 +
                        CurrLvSeries.LvSavesPuzzle.Count + 1
                });
        }

        while (CurrLvSeries.LvSavesMiniGame.Count > miniGame)
        {
            CurrLvSeries.LvSavesMiniGame.RemoveAt(
                CurrLvSeries.LvSavesMiniGame.Count - 1);
        }

        while (CurrLvSeries.LvSavesPuzzle.Count > puzzle)
        {
            CurrLvSeries.LvSavesPuzzle.RemoveAt(
                CurrLvSeries.LvSavesPuzzle.Count - 1);
        }
    }

    public void SaveLvInfo(bool saveCurrLv)
    {
        if (CurrLvSeries == null ||
            LV.Instance == null)
        {
            return;
        }

        List<LvSave> saves =
            GetLvSaves(LV.Instance.CurrLvId);

        if (saves == null || saves.Count == 0)
            return;

        if (saveCurrLv)
        {
            foreach (LvSave save in saves)
            {
                if (save.LvId != LV.Instance.CurrLvId)
                    continue;

                if (LV.Instance.IsEasy)
                {
                    if ((save.PassTime <= 10 ||
                         LVManager.Instance.PassTime < save.PassTime) &&
                        LVManager.Instance.PassTime > 10)
                    {
                        save.PassTime =
                            LVManager.Instance.PassTime;
                    }

                    save.PassNum++;
                }
                else
                {
                    if ((save.HardPTime <= 10 ||
                         LVManager.Instance.PassTime < save.HardPTime) &&
                        LVManager.Instance.PassTime > 10)
                    {
                        save.HardPTime =
                            LVManager.Instance.PassTime;
                    }

                    save.HardPNum++;
                }

                break;
            }
        }

        int series =
            saves[0].LvId / 10000;

        File.WriteAllText(
            lastSavePath + "/Wsave" + series + ".smf",
            Encrypt(JsonUtility.ToJson(CurrLvSeries)));
    }

    private List<LvSave> GetLvSaves(int lvid)
    {
        switch ((lvid % 10000) / 1000)
        {
            case 1:
                return CurrLvSeries.LvSavesMiniGame;

            case 2:
                return CurrLvSeries.LvSavesPuzzle;

            default:
                return CurrLvSeries.LvSaves;
        }
    }

    private void LoadSetting()
    {
        string file =
            SavePath + "/" + FixedInfo.SettingInfoName;

        if (!File.Exists(file))
            return;

        if (MyTool.TryParseJson<SettingSave>(
            File.ReadAllText(file),
            out var setting))
        {
            if (UIManager.Instance != null &&
                UIManager.Instance.SetPanel != null)
            {
                UIManager.Instance.SetPanel.SaveInit(setting);
            }

            lastSavePath = setting.lastLoadSavePath;
            Screen.fullScreen = setting.isFullScreen;
        }
    }

    public void SaveSetting()
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);

        SettingSave setting = new SettingSave
        {
            bgmVolume =
                AudioManager.Instance.BgmVolume,

            soundVolume =
                AudioManager.Instance.SoundVolume,

            lastLoadSavePath =
                lastSavePath,

            isFullScreen =
                Screen.fullScreen,

            isF1080P =
                UIManager.Instance.SetPanel.is1080P,

            isRainFog =
                SkyManager.Instance.isRainFog,

            isCardSelector =
                SeedBank.Instance.CardSelector,

            FrameType =
                UIManager.Instance.SetPanel.FrameType,

            isVsync =
                UIManager.Instance.SetPanel.isVsync,

            isDisFrame =
                UIManager.Instance.SetPanel.isDisFrame
        };

        File.WriteAllText(
            SavePath + "/" + FixedInfo.SettingInfoName,
            JsonUtility.ToJson(setting));
    }

    public List<CustomMapSave> LoadCustomMapFile()
    {
        List<CustomMapSave> list =
            new List<CustomMapSave>();

        string path =
            Application.persistentDataPath +
            "/custom/maps";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string[] files =
            Directory.GetFiles(path, "*.cm");

        foreach (string file in files)
        {
            if (!File.Exists(file))
                continue;

            if (MyTool.TryParseJson<CustomMapSave>(
                Decrypt(File.ReadAllText(file)),
                out var map))
            {
                map.SavePath = file;
                list.Add(map);
            }
        }

        if (list.Count == 0)
        {
            CreateNewCustomMap();
            return LoadCustomMapFile();
        }

        return list;
    }

    public void CreateNewCustomMap()
    {
        string path =
            Application.persistentDataPath +
            "/custom/maps";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        CustomMapSave map = new CustomMapSave
        {
            MapBrief = "一个新的自定义地图",
            mapType = MapType.CustomYard,
            VerticalNum = 5,
            HorizontalNum = 9,
            MapName = "newmap"
        };

        for (int i = 0; i < 45; i++)
            map.tileTypes.Add(TileType.Grass);

        int n = 0;
        string name = map.MapName;

        while (File.Exists(path + "/" + name + ".cm"))
        {
            n++;
            name = "newmap" + n;
        }

        map.MapName = name;

        File.WriteAllText(
            path + "/" + name + ".cm",
            Encrypt(JsonUtility.ToJson(map)));
    }

    public List<CustomLevelSave> LoadCustomLevelFile()
    {
        List<CustomLevelSave> list =
            new List<CustomLevelSave>();

        string path =
            Application.persistentDataPath +
            "/custom/levels";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string[] files =
            Directory.GetFiles(path, "*.clv");

        foreach (string file in files)
        {
            if (!File.Exists(file))
                continue;

            if (MyTool.TryParseJson<CustomLevelSave>(
                Decrypt(File.ReadAllText(file)),
                out var level))
            {
                level.SavePath = file;
                list.Add(level);
            }
        }

        if (list.Count == 0)
        {
            CreateNewCustomLv();
            return LoadCustomLevelFile();
        }

        return list;
    }

    public void CreateNewCustomLv()
    {
        string path =
            Application.persistentDataPath +
            "/custom/levels";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        CustomLevelSave level = new CustomLevelSave
        {
            LvBrief = "一个新的自定义关卡",
            lVType = LVType.Normal,
            series = MapSeries.Yard,
            mapTypes = new List<MapType>
            {
                MapType.FrontYard
            },
            LvName = "newlv"
        };

        int n = 0;
        string name = level.LvName;

        while (File.Exists(path + "/" + name + ".clv"))
        {
            n++;
            name = "newlv" + n;
        }

        level.LvName = name;

        File.WriteAllText(
            path + "/" + name + ".clv",
            Encrypt(JsonUtility.ToJson(level)));
    }
}