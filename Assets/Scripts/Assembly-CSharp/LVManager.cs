using System;
using System.Collections;
using System.Collections.Generic;
using SocketSave;
using StartScene;
using UnityEngine;
using UnityEngine.Events;

public class LVManager : MonoBehaviour
{
	public static LVManager Instance;

	public bool LvSpawnisOver;

	public bool BootyIsAppeared;

	public Booty OnlyBooty;

	public bool CanHandNextWave;

	public bool LastStandNoCd;

	public bool isBigWave;

	public bool LvDontFail;

	public bool StopSpawn;

	public int PassTime;

	private LVState currLVState;

	private int AllWeight;

	private int currLVWave;

	private int BIGwaveNum;

	[SerializeField]
	private int CurrSubLv;

	private UnityAction LvStartAction;

	private Coroutine RestSpawnCoroutine;

	private Coroutine AutoNextWaveCoroutine;

	[SerializeField]
	private int lvTotalTime;

	private bool isWaitSpawn;

	private bool waitingGoNextWave;

	public bool InGame => MapManager.Instance.mapList.Count > 0;

	public bool GameIsStart => currLVState == LVState.Fighting;

	public LVState CurrLVState
	{
		get
		{
			return currLVState;
		}
		private set
		{
			currLVState = value;
			switch (currLVState)
			{
			case LVState.Start:
				LvSpawnisOver = false;
				BootyIsAppeared = false;
				if (OnlyBooty != null)
				{
					OnlyBooty.DestroyThis();
				}
				OnlyBooty = null;
				break;
			case LVState.Fighting:
			case LVState.End:
				break;
			}
		}
	}

	public int CurrLVWave
	{
		get
		{
			return currLVWave;
		}
		set
		{
			currLVWave = value;
			if (currLVWave < LV.Instance.Weights[0].Count)
			{
				AutoStartNextWave();
				return;
			}
			LvSpawnisOver = true;
			WaveComing waveComing = new WaveComing();
			waveComing.WaveType = 3;
			SocketServer.Instance.BigWaveComing(waveComing);
		}
	}

	public int LvTotalTime
	{
		get
		{
			return lvTotalTime;
		}
		set
		{
			if (GameManager.Instance.isClient)
			{
				return;
			}
			int num = lvTotalTime;
			if (!StopSpawn)
			{
				lvTotalTime = value;
			}
			if (lvTotalTime < 0)
			{
				lvTotalTime = 0;
			}
			if (lvTotalTime == 0)
			{
				FlagMeter.Instance.SetRestTimeText(null);
			}
			if (num > 10 && lvTotalTime < 10)
			{
				SkipRestBtn.Instance.CloseBtn();
			}
			if ((LV.Instance.SubLvs.Count > CurrSubLv || IsRestTime) && num > 0 && value == 0 && !waitingGoNextWave)
			{
				if (!isWaitSpawn)
				{
					GoNextSubLv();
				}
				AllTime();
				IsRestTime = false;
				isWaitSpawn = false;
				AutoStartNextWave();
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
			}
			if (IsRestTime)
			{
				FlagMeter.Instance.SetRestTimeText("下次进攻还有" + LvTotalTime + "秒");
			}
			else
			{
				FlagMeter.Instance.SetRestTimeText(null);
			}
		}
	}

	public bool IsRestTime { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	public string LvTypeName(LVType type)
	{
		string result = "";
		switch (type)
		{
		case LVType.Normal:
			result = "普通模式";
			break;
		case LVType.PvP:
			result = "玩家对战";
			break;
		case LVType.IZombie:
			result = "我是僵尸";
			break;
		case LVType.VaseBreaker:
			result = "砸罐子";
			break;
		}
		return result;
	}

	public void StartGame(LoadLVBag loadBag, int LVId)
	{
		if (!InGame && (!GameManager.Instance.isClient || loadBag != null))
		{
			AudioManager.Instance.StopBgAudio();
			CameraControl.Instance.InAcvment = false;
			PvPSelector.Instance.CloseSelector();
			LevelSelector.Instance.CloseSelector();
			UIManager.Instance.LogPanel.Close();
			UIManager.Instance.SetPanel.CloseSetPanel();
			AlmanacScence.Instance.BackMenu();
			GobalLight.Instance.InitIntensity();
			StartLv(loadBag, LVId, needloadLv: true);
			PlayerManager.Instance.ResetSunNum();
			GoOtherMap.Instance.LoadInit();
			PlantManager.Instance.LoadLvStartPlant();
			Timetable.Instance.UpdateTempt(CameraControl.Instance.CurrMap);
			if (LV.Instance.CurrLVType == LVType.IZombie || LV.Instance.CurrLVType == LVType.VaseBreaker)
			{
				Timetable.Instance.transform.localScale = Vector3.zero;
			}
			else
			{
				Timetable.Instance.transform.localScale = Vector3.one;
			}
		}
	}

	private void StartLv(LoadLVBag loadBag, int LVId, bool needloadLv)
	{
		int num = UnityEngine.Random.Range(1000000, 9999999);
		CurrLVState = LVState.Start;
		if (GameManager.Instance.isClient && loadBag != null)
		{
			num = loadBag.LvSeed;
			if (needloadLv)
			{
				LV.Instance.ClientLoadLv(loadBag);
			}
			if (SpectatorList.Instance.LocalIsSpectator)
			{
				SeedBank.Instance.CardNum = 0;
			}
			else
			{
				for (int i = 0; i < loadBag.NameList.Count; i++)
				{
					if (loadBag.NameList[i] == GameManager.Instance.LocalPlayerSave.playerName)
					{
						SeedBank.Instance.CardNum = loadBag.CardNumList[i];
					}
				}
			}
			BattlePlayerList.Instance.LoadAllSeedBank(loadBag.NameList, loadBag.CardNumList);
			FlagMeter.Instance.SetLvlName(LV.Instance.LvName, loadBag.BoolTypes[0]);
		}
		List<int> list = new List<int>();
		List<string> noSpectatorPlayerList = SpectatorList.Instance.GetNoSpectatorPlayerList();
		if (GameManager.Instance.isServer)
		{
			if (needloadLv)
			{
				LV.Instance.LoadLV(LVId, LevelSelector.Instance.IsEasy, onlyInfo: false, isRun: false);
			}
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				int cardNum = PvPSelector.Instance.CardNum;
				int num2 = cardNum / PvPSelector.Instance.RedTeamNames.Count;
				int num3 = cardNum % PvPSelector.Instance.RedTeamNames.Count;
				int num4 = cardNum / PvPSelector.Instance.BlueTeamNames.Count;
				int num5 = cardNum % PvPSelector.Instance.BlueTeamNames.Count;
				for (int j = 0; j < noSpectatorPlayerList.Count; j++)
				{
					bool flag = false;
					bool flag2 = false;
					if (PvPSelector.Instance.RedTeamNames.Contains(noSpectatorPlayerList[j]))
					{
						if (!flag)
						{
							flag = true;
							list.Add(num2 + num3);
						}
						else
						{
							list.Add(num2);
						}
					}
					else if (PvPSelector.Instance.BlueTeamNames.Contains(noSpectatorPlayerList[j]))
					{
						if (!flag2)
						{
							flag2 = true;
							list.Add(num4 + num5);
						}
						else
						{
							list.Add(num4);
						}
					}
				}
				SeedBank.Instance.CardNum = 0;
				for (int k = 0; k < noSpectatorPlayerList.Count; k++)
				{
					if (noSpectatorPlayerList[k] == GameManager.Instance.LocalPlayerSave.playerName)
					{
						SeedBank.Instance.CardNum = list[k];
					}
				}
			}
			else
			{
				_ = noSpectatorPlayerList.Count;
				int num6;
				if (LV.Instance.CardNum >= 0)
				{
					num6 = LV.Instance.CardNum;
				}
				else
				{
					int num7 = 0;
					for (int l = 0; l < GameManager.Instance.LocalPlayerSave.SpItems.Count; l++)
					{
						if (GameManager.Instance.LocalPlayerSave.SpItems[l] == SpItem.StoreCardSlot)
						{
							num7++;
						}
					}
					num6 = GameManager.Instance.LocalPlayerSave.CardSlotNum + num7;
				}
				if (!LV.Instance.BanMultyCardAdd)
				{
					num6 += noSpectatorPlayerList.Count - 1;
				}
				List<int> list2 = new List<int>();
				for (int m = 0; m < num6; m++)
				{
					list2.Add(0);
				}
				List<List<int>> list3 = MyTool.SplitByNumberOfLists(list2, noSpectatorPlayerList.Count);
				for (int n = 0; n < noSpectatorPlayerList.Count; n++)
				{
					list.Add(list3[n].Count);
				}
				SeedBank.Instance.CardNum = list3[0].Count;
			}
			if (SpectatorList.Instance.LocalIsSpectator)
			{
				SeedBank.Instance.CardNum = 0;
			}
			LoadLVBag loadLVBag = new LoadLVBag();
			loadLVBag.LvId = LVId;
			loadLVBag.LvSeed = num;
			loadLVBag.LvName = LV.Instance.LvName;
			loadLVBag.dayBgm = LV.Instance.DayBgm;
			loadLVBag.nightBgm = LV.Instance.NightBgm;
			loadLVBag.BankType = LV.Instance.CurrBankType;
			loadLVBag.SeedBankType = LV.Instance.CurrSeedBankType;
			loadLVBag.LoadMapTypes = LV.Instance.LoadMapTypes;
			loadLVBag.LvSpStates = LV.Instance.LvSpStates;
			loadLVBag.CardNumList = list;
			loadLVBag.NameList = noSpectatorPlayerList;
			loadLVBag.BoolTypes = new List<bool>
			{
				LevelSelector.Instance.IsEasy,
				LV.Instance.EnableShovel,
				LV.Instance.EnablePlantGlove,
				LV.Instance.EnableZombieGlove
			};
			List<ZombieType> list4 = new List<ZombieType>();
			List<int> list5 = new List<int>();
			for (int num8 = 0; num8 < LV.Instance.ZombieTypes.Count; num8++)
			{
				list5.Add(LV.Instance.ZombieTypes[num8].Count);
				list4.AddRange(LV.Instance.ZombieTypes[num8]);
			}
			loadLVBag.ZTypesSplit = list5;
			loadLVBag.ZombieTypes = list4;
			SocketServer.Instance.LoadLv(loadLVBag);
		}
		if (!GameManager.Instance.isClient)
		{
			if (needloadLv)
			{
				LV.Instance.LoadLV(LVId, LevelSelector.Instance.IsEasy, onlyInfo: false, isRun: true);
			}
			if (!GameManager.Instance.isServer)
			{
				if (LV.Instance.CardNum >= 0)
				{
					SeedBank.Instance.CardNum = LV.Instance.CardNum;
				}
				else
				{
					int num9 = 0;
					for (int num10 = 0; num10 < GameManager.Instance.LocalPlayerSave.SpItems.Count; num10++)
					{
						if (GameManager.Instance.LocalPlayerSave.SpItems[num10] == SpItem.StoreCardSlot)
						{
							num9++;
						}
					}
					SeedBank.Instance.CardNum = GameManager.Instance.LocalPlayerSave.CardSlotNum + num9;
				}
			}
			if (needloadLv && LV.Instance.CurrLVType == LVType.Normal)
			{
				MapManager.Instance.CreateAllMower();
			}
		}
		if (GameManager.Instance.isServer)
		{
			BattlePlayerList.Instance.LoadAllSeedBank(noSpectatorPlayerList, list);
		}
		UIManager.Instance.OpenBattleUI();
		if (!GameManager.Instance.isClient)
		{
			SumWeight();
			FlagMeter.Instance.CreateFlag(AllWeight);
			FlagMeter.Instance.SetLvlName(LV.Instance.LvName, LevelSelector.Instance.IsEasy);
		}
		SeedBank.Instance.SpawnCardSlot();
		if (CurrSubLv == 0 || (CurrSubLv > 0 && LvTotalTime > 0))
		{
			Vector2 position = new Vector2(-3.5f, 0f);
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				CameraControl.Instance.SetPosition(Vector2.zero);
				StartCoroutine(WaitTimeDo(LVStartCameraAction, 1f));
				PlayChooseCardBgAudio();
			}
			else if (LV.Instance.CurrLVType == LVType.IZombie)
			{
				LoadFixedCard(LV.Instance.FixedCard);
				LVStartEFOver();
				LV.Instance.SpSeedBankMove();
				MapManager.Instance.CreateAllBrain();
				MapManager.Instance.LoadIZombieMap();
				CameraControl.Instance.SetPosition(position);
			}
			else if (LV.Instance.CurrLVType == LVType.VaseBreaker)
			{
				LoadFixedCard(LV.Instance.FixedCard);
				LVStartEFOver();
				if (LV.Instance.FixedCard.Count > 0)
				{
					LV.Instance.SpSeedBankMove();
				}
				MapManager.Instance.LoadVaseBreaker();
				CameraControl.Instance.SetPosition(position);
			}
			else if (LV.Instance.CurrBankType == BankType.ConveryorBelt || LV.Instance.CurrBankType == BankType.SlotMachine || LV.Instance.LvSpStates.Contains(LVSpState.RainPlant))
			{
				LoadFixedCard(LV.Instance.FixedCard);
				CameraControl.Instance.SetPosition(position);
				LV.Instance.SpSeedBankMove();
				LVStartCameraBackAction();
				PlayChooseCardBgAudio();
			}
			else
			{
				UnityEngine.Random.InitState(num);
				PlayChooseCardBgAudio();
				ZombieManager.Instance.ShowZombie();
				CameraControl.Instance.MoveForLVStart(LVStartCameraAction);
			}
		}
		if (CurrSubLv > 0 && LvTotalTime == 0)
		{
			LoadFixedCard(LV.Instance.FixedCard);
			LoadFixedCard(SeedBank.Instance.LastSelectCard, canAddSlot: false);
			LVStartEFOver();
			LV.Instance.SpSeedBankMove();
		}
	}

	private void PlayChooseCardBgAudio()
	{
		if (GameManager.Instance.isClient)
		{
			StartCoroutine(WaitTimeDo(PLayChooseCard, 0.1f));
		}
		else
		{
			PLayChooseCard();
		}
	}

	private void PLayChooseCard()
	{
		if (SkyManager.Instance.GetIsDay())
		{
			AudioManager.Instance.PlayBgAudio(BgmType.ChooseYourSeeds);
		}
		else
		{
			AudioManager.Instance.PlayBgAudio(BgmType.NightChooseSeeds);
		}
	}

	public void QuitBattleGame(bool StartScenceAnim = true)
	{
		if (GameManager.Instance.isServer)
		{
			LoadLVBag loadLVBag = new LoadLVBag();
			loadLVBag.LoadType = 2;
			SocketServer.Instance.LoadLv(loadLVBag);
		}
		ResetScence();
		LevelSelector.Instance.LoadLastLv();
		PvPSelector.Instance.ResetPvPInfo();
		CreatePanel.Instance.LvRest();
		AudioManager.Instance.PlayBgAudio(BgmType.Nor);
		CameraControl.Instance.SetPosition(new Vector2(0f, -30f));
		StartSceneManager.Instance.LoadStartScence(StartScenceAnim);
	}

	public void ReStartGame()
	{
		StatsManager.Instance.AddStatsNum(StatsEnum.ReStartNum);
		if (GameManager.Instance.isServer)
		{
			LoadLVBag loadLVBag = new LoadLVBag();
			loadLVBag.LoadType = 1;
			SocketServer.Instance.LoadLv(loadLVBag);
		}
		ResetScence();
		LevelSelector.Instance.StartCurrGame();
	}

	private void ResetScence()
	{
		ResetLv();
		PassTime = 0;
		CurrSubLv = 0;
		GobalLight.Instance.gobalLight.intensity = 1f;
		AudioManager.Instance.ChangeMapReset();
		SeedBank.Instance.isCanClick = true;
		UIManager.Instance.StopLVStartEF();
		UIManager.Instance.OverPanel.gameObject.SetActive(value: false);
		PlantManager.Instance.LvReset();
		ZombieManager.Instance.LvReset();
		SkyManager.Instance.ResetAll();
		MapManager.Instance.ResetScence();
		PoolManager.Instance.ClearPool();
		UIManager.Instance.LogPanel.Close();
		UIManager.Instance.BattleUI.localScale = Vector3.zero;
		Timetable.Instance.LvReset();
		EffectPanel.Instance.LvReset();
		UIManager.Instance.LastStandBtn.gameObject.SetActive(value: false);
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(base.transform.GetChild(num).gameObject);
		}
		StopAllCoroutines();
		GameManager.Instance.SaveSAInfo();
		LvItemManager.Instance.LvReset();
		BigTitle.Instance.LvReset();
	}

	private void ResetLv()
	{
		currLVWave = 0;
		BIGwaveNum = 0;
		AllWeight = 0;
		IsRestTime = false;
		isBigWave = false;
		LastStandNoCd = false;
		LvStartAction = null;
		CurrLVState = LVState.Start;
		PlayerManager.Instance.ClearMoney();
		CobCannonTarget.Instance.StopAim();
		Shovel.Instance.CancelShovel();
		SeedBank.Instance.ClearCardSlot();
		SeedBank.Instance.StartMoveBack(fade: false);
		SeedChooser.Instance.ResetThis();
		ZombieChooser.Instance.ResetThis();
		NextWaveBtn.Instance.CloseBtn();
		SkipRestBtn.Instance.CloseBtn();
		AcvmentManager.Instance.LVResetThis();
		UIManager.Instance.SetPanel.LvReset();
		SkyManager.Instance.StopTime();
		PlayerManager.Instance.LvReset();
	}

	public void ZombieGameOver(Vector2 overPos)
	{
		if (!LvDontFail)
		{
			if (GameManager.Instance.isServer)
			{
				GameOver gameOver = new GameOver();
				gameOver.pos = overPos;
				SocketServer.Instance.GameOver(gameOver);
			}
			StatsManager.Instance.AddStatsNum(StatsEnum.FailNum);
			StopAllCoroutines();
			CameraControl.Instance.GoOtherYard(MapManager.Instance.mapList.IndexOf(MapManager.Instance.GetCurrMap(overPos)));
			Shovel.Instance.CancelShovel();
			SkyManager.Instance.ResetAll();
			MapManager.Instance.GameOverPause();
			PlantManager.Instance.GameOverPause();
			ZombieManager.Instance.GameOverPause();
			AudioManager.Instance.StopBgAudio();
			SeedBank.Instance.AllCancelPlace(NoSelect: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlant1, base.transform.position, isAll: true);
			UIManager.Instance.OverPanel.Over();
		}
	}

	public void GameOver2()
	{
		if (!LvDontFail)
		{
			StopAllCoroutines();
			Shovel.Instance.CancelShovel();
			SkyManager.Instance.ResetAll();
			MapManager.Instance.GameOverPause();
			PlantManager.Instance.GameOverPause();
			ZombieManager.Instance.GameOverPause();
			AudioManager.Instance.StopBgAudio();
			SeedBank.Instance.AllCancelPlace(NoSelect: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			OverPanelEvent();
		}
	}

	public void PvPGameOver(Vector2 overPos, bool isRedFail)
	{
		if (LvDontFail)
		{
			return;
		}
		if (GameManager.Instance.isServer)
		{
			GameOver gameOver = new GameOver();
			gameOver.pos = overPos;
			gameOver.isRedFail = isRedFail;
			SocketServer.Instance.GameOver(gameOver);
		}
		StopAllCoroutines();
		CameraControl.Instance.GoOtherYard(MapManager.Instance.mapList.IndexOf(MapManager.Instance.GetCurrMap(overPos)));
		Shovel.Instance.CancelShovel();
		SkyManager.Instance.ResetAll();
		MapManager.Instance.GameOverPause();
		PlantManager.Instance.GameOverPause();
		ZombieManager.Instance.GameOverPause();
		AudioManager.Instance.StopBgAudio();
		SeedBank.Instance.AllCancelPlace(NoSelect: true);
		if (PvPSelector.Instance.LocalIsRedTeam)
		{
			if (isRedFail)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameWin, base.transform.position, isAll: true);
			}
		}
		else if (PvPSelector.Instance.LocalIsRedTeam)
		{
			if (isRedFail)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameWin, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			}
		}
		string logContent = "红方获得胜利！";
		if (isRedFail)
		{
			logContent = "蓝方获得胜利！";
		}
		UIManager.Instance.LogPanel.DisplayLog(logContent, () =>
		{
			if (!GameManager.Instance.isClient)
			{
				QuitBattleGame();
			}
		});
		UIManager.Instance.LogPanel.ButtonText.text = "返回主界面";
		if (GameManager.Instance.isClient)
		{
			UIManager.Instance.LogPanel.Button.gameObject.SetActive(value: false);
		}
	}

	public void OverPanelEvent()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		UIManager.Instance.LogPanel.DisplayLog("游戏结束。", () =>
		{
			if (!GameManager.Instance.isClient)
			{
				ReStartGame();
			}
		});
		UIManager.Instance.LogPanel.ButtonText.text = "重新开始";
	}

	public void GameWin(Booty booty)
	{
		CurrLVState = LVState.End;
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			QuitBattleGame();
			return;
		}
		OnlyBooty = null;
		ResetScence();
		AwardScence.Instance.JumpTo(booty, () =>
		{
			if (!GameManager.Instance.isClient)
			{
				if (LV.Instance.CurrLvId % 10000 < GameManager.Instance.SelectedStone.AdventureLvNum)
				{
					LevelSelector.Instance.LoadLastLv();
					StartGame(null, LV.Instance.CurrLvId);
				}
				else
				{
					QuitBattleGame();
				}
			}
		});
	}

	public void StartRunLv()
	{
		SeedBank.Instance.isCanClick = false;
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			StartCoroutine(WaitTimeDo(LVStartCameraBackAction, 1f));
		}
		else
		{
			CameraControl.Instance.MoveBackForLVStart(LVStartCameraBackAction);
		}
	}

	public void LVStartCameraAction()
	{
		SeedBank.Instance.StartMove(LV.Instance.CurrSeedBankType);
		SeedChooser.Instance.ResetThis();
		SeedChooser.Instance.StartMove();
		ZombieChooser.Instance.ResetThis();
		ZombieChooser.Instance.StartMove();
		LoadFixedCard(LV.Instance.FixedCard);
	}

	private void LoadFixedCard(List<CardType> cards, bool canAddSlot = true)
	{
		if (!GameManager.Instance.isOnline)
		{
			SeedBank.Instance.AddCards(cards, canAddSlot: true);
		}
		else if (GameManager.Instance.isServer)
		{
			List<string> noSpectatorPlayerList = SpectatorList.Instance.GetNoSpectatorPlayerList();
			List<List<CardType>> list = MyTool.SplitByNumberOfLists(cards, noSpectatorPlayerList.Count);
			for (int i = 1; i < list.Count; i++)
			{
				AddCardBag addCardBag = new AddCardBag();
				addCardBag.CardTypes = list[i];
				SocketServer.Instance.SendAddCard(addCardBag, noSpectatorPlayerList[i]);
			}
			SeedBank.Instance.AddCards(list[0], canAddSlot);
		}
	}

	public void LVStartCameraBackAction()
	{
		if (LV.Instance.CurrLVType == LVType.Normal)
		{
			ZombieManager.Instance.ClearAllZombie();
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.LastStand))
		{
			LVStartEFOver();
		}
		else
		{
			UIManager.Instance.ShowLVStartEF();
		}
	}

	public void LVStartEFOver()
	{
		startRunLV();
		SeedBank.Instance.isCanClick = true;
		CurrLVState = LVState.Fighting;
		if (LV.Instance.CurrLVType == LVType.IZombie || LV.Instance.CurrLVType == LVType.VaseBreaker)
		{
			AudioManager.Instance.PlayBgAudio(BgmType.Cerebrawl);
		}
		else if (SkyManager.Instance.GetIsDay())
		{
			AudioManager.Instance.PlayBgAudio(LV.Instance.DayBgm);
		}
		else
		{
			AudioManager.Instance.PlayBgAudio(LV.Instance.NightBgm);
		}
		if (LV.Instance.StartAction != null)
		{
			LV.Instance.StartAction();
		}
		LvStartAction?.Invoke();
	}

	public void AddLVStartActionListenr(UnityAction action)
	{
		LvStartAction = (UnityAction)Delegate.Combine(LvStartAction, action);
	}

	private void OnAllZombieDeadAction()
	{
		CurrLVWave++;
		ZombieManager.Instance.RemoveAllZombieDeadAction(OnAllZombieDeadAction);
		if (AutoNextWaveCoroutine != null)
		{
			StopCoroutine(AutoNextWaveCoroutine);
		}
		AutoNextWaveCoroutine = null;
	}

	public void StartLastStand()
	{
		RunLv();
		LastStandNoCd = false;
		SeedBank.Instance.StartAllCD();
		UIManager.Instance.LastStandBtn.gameObject.SetActive(value: false);
		if (GameManager.Instance.isServer)
		{
			WaveComing waveComing = new WaveComing();
			waveComing.WaveType = 6;
			SocketServer.Instance.BigWaveComing(waveComing);
		}
	}

	private void startRunLV()
	{
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			SkyManager.Instance.StartTime();
		}
		else
		{
			if (LV.Instance.CurrLVType == LVType.IZombie || LV.Instance.CurrLVType == LVType.VaseBreaker)
			{
				return;
			}
			if (LV.Instance.LvSpStates.Contains(LVSpState.LastStand))
			{
				LastStandNoCd = true;
				SeedBank.Instance.ClearAllCD();
				ChatInput.Instance.AddMessage("开始建造你的阵型！！");
				if (!GameManager.Instance.isClient)
				{
					UIManager.Instance.LastStandBtn.gameObject.SetActive(value: true);
				}
			}
			else
			{
				RunLv();
			}
		}
	}

	private void RunLv()
	{
		SkyManager.Instance.StartTime();
		if (GameManager.Instance.isClient)
		{
			return;
		}
		isWaitSpawn = true;
		waitingGoNextWave = false;
		if (CurrSubLv == 0)
		{
			float time = UnityEngine.Random.Range(LV.Instance.SetupTime.x, LV.Instance.SetupTime.y);
			StartCoroutine(WaitTimeDo(() =>
			{
				AllTime();
				isWaitSpawn = false;
				AutoStartNextWave();
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
			}, time));
		}
		else
		{
			IsRestTime = true;
			SkipRestBtn.Instance.CanSkipWave();
			if (RestSpawnCoroutine != null)
			{
				StopCoroutine(RestSpawnCoroutine);
			}
			RestSpawnCoroutine = StartCoroutine(RestZombie());
		}
	}

	private void AutoStartNextWave()
	{
		StartCoroutine(NextWave());
	}

	public void OnZombieDeadEvent()
	{
		CheckNextWaveBtn();
	}

	private void CheckNextWaveBtn()
	{
		int num = MapManager.Instance.mapList.Count * 2;
		if (LV.Instance.Weights.Count > 0 && CanHandNextWave && ZombieManager.Instance.GetZombieNum() <= num && CurrLVWave < LV.Instance.Weights[0].Count - 1 && !LastStandNoCd)
		{
			NextWaveBtn.Instance.CanNextWave();
		}
	}

	public void NextWaveBtnEvent()
	{
		if (CanHandNextWave)
		{
			OnAllZombieDeadAction();
		}
	}

	private IEnumerator AutoNextWave(float time)
	{
		CanHandNextWave = false;
		int handTime = 15 + MapManager.Instance.mapList.Count * 5;
		if (time > (float)handTime)
		{
			yield return new WaitForSeconds(handTime);
			CanHandNextWave = true;
			CheckNextWaveBtn();
			yield return new WaitForSeconds(time - (float)handTime);
		}
		else
		{
			yield return new WaitForSeconds(time);
		}
		CanHandNextWave = false;
		NextWaveBtn.Instance.CloseBtn();
		OnAllZombieDeadAction();
	}

	private IEnumerator NextWave()
	{
		isBigWave = false;
		CanHandNextWave = false;
		NextWaveBtn.Instance.CloseBtn();
		List<int> WaveWeights = new List<int>();
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			WaveWeights.Add(LV.Instance.Weights[i][CurrLVWave]);
		}
		if (GameManager.Instance.isServer)
		{
			WaveComing waveComing = new WaveComing();
			waveComing.WaveType = 0;
			if (CurrLVWave == LV.Instance.BigWaveNum[BIGwaveNum])
			{
				if (BIGwaveNum == LV.Instance.BigWaveNum.Count - 1)
				{
					waveComing.WaveType = 2;
				}
				else
				{
					waveComing.WaveType = 1;
				}
				waveComing.WaitTime = 4f;
			}
			SocketServer.Instance.BigWaveComing(waveComing);
		}
		if (CurrLVWave == LV.Instance.BigWaveNum[BIGwaveNum])
		{
			if (BIGwaveNum == LV.Instance.BigWaveNum.Count - 1)
			{
				UIManager.Instance.ShowFinalWaveEF();
			}
			else
			{
				UIManager.Instance.ShowBigWaveEF();
			}
			isBigWave = true;
			yield return new WaitForSeconds(4f);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
			FlagMeter.Instance.FlagRise(BIGwaveNum);
			BIGwaveNum++;
			for (int j = 0; j < MapManager.Instance.mapList.Count; j++)
			{
				for (int k = 0; k < LV.Instance.BigWaveFixedZombie.Count; k++)
				{
					ZombieManager.Instance.UpdateZombieOnRandomLine(LV.Instance.BigWaveFixedZombie[k], MapManager.Instance.mapList[j].transform.position);
				}
			}
			if (LV.Instance.BigWaveAction != null)
			{
				LV.Instance.BigWaveAction();
			}
			MapManager.Instance.AllGraveOutZombie();
			MapManager.Instance.AllMapWaterOutZombie();
		}
		float spawnTime = 5f;
		while (true)
		{
			if (StopSpawn)
			{
				Debug.Log("等待中");
				yield return new WaitForSeconds(1f);
				continue;
			}
			int num = 0;
			for (int l = 0; l < WaveWeights.Count; l++)
			{
				num += WaveWeights[l];
			}
			if (num == 0)
			{
				break;
			}
			int num2 = 0;
			for (int m = 0; m < WaveWeights.Count; m++)
			{
				if (WaveWeights[m] == 0)
				{
					continue;
				}
				int index;
				while (true)
				{
					num2++;
					index = UnityEngine.Random.Range(0, LV.Instance.ZombieTypes[m].Count);
					if ((ZombieManager.Instance.GetZombieWeight(LV.Instance.ZombieTypes[m][index]) <= WaveWeights[m] || !LV.Instance.WeightLimit) && (CurrLVWave >= LV.Instance.ProphaseLimitWave || !LV.Instance.ProphaseLimitZombie.Contains(LV.Instance.ZombieTypes[m][index])))
					{
						break;
					}
					if (num2 > 10)
					{
						index = 0;
						break;
					}
				}
				if (MapManager.Instance.mapList.Count - 1 < m)
				{
					break;
				}
				Vector3 position = MapManager.Instance.mapList[m].transform.position;
				bool flag = false;
				ZombieType zombieType = LV.Instance.ZombieTypes[m][index];
				if (MapManager.Instance.mapList[m].SpSpawnZombie(out var pos, out var SPCode))
				{
					flag = true;
					switch (SPCode)
					{
					case 0:
						ZombieManager.Instance.MapSPZombie(zombieType, pos, MapManager.Instance.mapList[m]);
						break;
					case 1:
						ZombieManager.Instance.OutGround(zombieType, pos, null, needArm: false, isHyp: false, purple: false);
						break;
					}
				}
				int spawnCode = 0;
				if (CurrLVWave < LV.Instance.ProphaseLimitWave && num2 <= 10)
				{
					spawnCode = 1;
				}
				if (!flag && LV.Instance.LvSpStates.Contains(LVSpState.BungiMode) && !ZombieManager.Instance.CantBungiSky.Contains(zombieType) && UnityEngine.Random.Range(0f, 1f) <= LV.Instance.BungiSpRate && ZombieManager.Instance.UpdateBungiZombieOnRandomLine(zombieType, position))
				{
					flag = true;
				}
				if (!flag && ZombieManager.Instance.UpdateZombieOnRandomLine(zombieType, position, spawnCode))
				{
					flag = true;
				}
				if (flag || num2 > 10)
				{
					int num3 = ZombieManager.Instance.GetZombieWeight(zombieType);
					if (num3 > WaveWeights[m])
					{
						num3 = WaveWeights[m];
					}
					WaveWeights[m] -= num3;
					FlagMeter.Instance.UpdateHead(num3);
				}
				else
				{
					m--;
				}
			}
			if (MapManager.Instance.mapList.Count == 0)
			{
				break;
			}
			float num4 = UnityEngine.Random.Range(0.2f, 1.5f);
			if (spawnTime >= num4)
			{
				spawnTime -= num4;
				yield return new WaitForSeconds(num4);
			}
		}
		ZombieManager.Instance.AddAllZombieDeadAction(OnAllZombieDeadAction);
		AutoNextWaveCoroutine = StartCoroutine(AutoNextWave((float)GetAutoTime(WaveWeights) + spawnTime));
	}

	private IEnumerator RestZombie()
	{
		do
		{
			float sunNum = PlayerManager.Instance.GetSunNum(isSun: true, null);
			float seconds = 10f - sunNum / 1000f + 10f;
			yield return new WaitForSeconds(seconds);
			if (!IsRestTime)
			{
				break;
			}
			int item = (int)(sunNum / 500f) + 1;
			List<int> WaveWeights = new List<int>();
			for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
			{
				WaveWeights.Add(item);
			}
			while (true)
			{
				int num = 0;
				for (int j = 0; j < WaveWeights.Count; j++)
				{
					num += WaveWeights[j];
				}
				if (num == 0)
				{
					break;
				}
				int num2 = 0;
				for (int k = 0; k < WaveWeights.Count; k++)
				{
					if (WaveWeights[k] == 0)
					{
						continue;
					}
					int index;
					while (true)
					{
						num2++;
						index = UnityEngine.Random.Range(0, LV.Instance.ZombieTypes[k].Count);
						if (ZombieManager.Instance.GetZombieWeight(LV.Instance.ZombieTypes[k][index]) <= WaveWeights[k] || !LV.Instance.WeightLimit)
						{
							break;
						}
						if (num2 > 10)
						{
							index = 0;
							break;
						}
					}
					if (MapManager.Instance.mapList.Count - 1 < k)
					{
						break;
					}
					Vector3 position = MapManager.Instance.mapList[k].transform.position;
					bool flag = false;
					ZombieType type = LV.Instance.ZombieTypes[k][index];
					if (MapManager.Instance.mapList[k].SpSpawnZombie(out var pos, out var SPCode))
					{
						flag = true;
						switch (SPCode)
						{
						case 0:
							ZombieManager.Instance.MapSPZombie(type, pos, MapManager.Instance.mapList[k]);
							break;
						case 1:
							ZombieManager.Instance.OutGround(type, pos, null, needArm: false, isHyp: false, purple: false);
							break;
						}
					}
					if (!flag && ZombieManager.Instance.UpdateZombieOnRandomLine(type, position))
					{
						flag = true;
					}
					if (flag || num2 > 10)
					{
						int num3 = ZombieManager.Instance.GetZombieWeight(type);
						if (num3 > WaveWeights[k])
						{
							num3 = WaveWeights[k];
						}
						WaveWeights[k] -= num3;
					}
					else
					{
						k--;
					}
				}
				float seconds2 = UnityEngine.Random.Range(2, 8);
				yield return new WaitForSeconds(seconds2);
			}
		}
		while (IsRestTime);
	}

	private int GetAutoTime(List<int> WaveWeights)
	{
		int num = 0;
		for (int i = 0; i < WaveWeights.Count; i++)
		{
			num += WaveWeights[i];
		}
		num += num / 2;
		num += LV.Instance.NextWaveLossTime;
		int num2 = 50 + LV.Instance.NextWaveLossTime;
		if (num < num2)
		{
			num = num2;
		}
		return Mathf.Clamp(num, 10, 85);
	}

	private void AllTime()
	{
		int num = 0;
		if (CurrSubLv == 0)
		{
			num += (int)LV.Instance.SetupTime.y;
		}
		num += LV.Instance.BigWaveNum.Count * 4;
		num += LV.Instance.Weights[0].Count * 5;
		for (int i = 0; i < LV.Instance.Weights[0].Count; i++)
		{
			List<int> list = new List<int>();
			for (int j = 0; j < MapManager.Instance.mapList.Count; j++)
			{
				list.Add(LV.Instance.Weights[j][i]);
			}
			num += GetAutoTime(list);
		}
		LvTotalTime = num;
	}

	private IEnumerator WaitTimeDo(UnityAction action, float time)
	{
		yield return new WaitForSeconds(time);
		action?.Invoke();
	}

	public void ZombieNumChange(int num)
	{
		if (IsRestTime)
		{
			if (num > 0)
			{
				SkipRestBtn.Instance.CloseBtn();
			}
			else if (LvTotalTime > 10)
			{
				SkipRestBtn.Instance.CanSkipWave();
			}
		}
	}

	public void SumWeight()
	{
		if (LV.Instance.Weights.Count == 0)
		{
			AllWeight = 0;
			return;
		}
		int num = 0;
		for (int i = 0; i < LV.Instance.Weights[0].Count; i++)
		{
			if (i == LV.Instance.BigWaveNum[num])
			{
				num++;
				continue;
			}
			for (int j = 0; j < LV.Instance.Weights.Count; j++)
			{
				AllWeight += LV.Instance.Weights[j][i];
			}
		}
	}

	public void SpawnBooty(Vector3 pos, SynBooty syn1 = null)
	{
		if (BootyIsAppeared)
		{
			return;
		}
		BootyIsAppeared = true;
		if (PlayerManager.Instance.GetSunNum(isSun: true, GameManager.Instance.LocalPlayerSave.playerName) >= 10000f && !PlayerManager.Instance.SunInfinite && LV.Instance.CurrLVType == LVType.Normal)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.SunFull);
		}
		if (MapManager.Instance.mapList.Count >= 3 && PlayerList.Instance.PlayerNum >= 4)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.UnitAsOne);
		}
		StatsManager.Instance.AddStatsNum(StatsEnum.WinNum);
		Booty component = UnityEngine.Object.Instantiate(GameManager.Instance.GameConf.Moneybag).GetComponent<Booty>();
		component.transform.position = pos;
		if (GameManager.Instance.isClient && syn1 != null)
		{
			AwardScence.Instance.LoadText(syn1.info1, syn1.info2, syn1.info3);
			component.InitThis(syn1);
		}
		else
		{
			component.InitThis();
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(component.transform.position);
		if (gridByWorldPos != null)
		{
			if (Vector2.Distance(component.transform.position, gridByWorldPos.Position) > 0.8f)
			{
				component.transform.position = gridByWorldPos.Position;
			}
			OnlyBooty = component;
			if (GameManager.Instance.isServer)
			{
				SynBooty synBooty = new SynBooty();
				synBooty.isSpawn = true;
				synBooty.pos = pos;
				synBooty.BootyPlant = LV.Instance.BootyPlant;
				synBooty.sprite = LV.Instance.BootySprite;
				synBooty.info1 = AwardScence.Instance.Title.text;
				synBooty.info2 = AwardScence.Instance.ItemName.text;
				synBooty.info3 = AwardScence.Instance.Content.text;
				SocketServer.Instance.SendSynBooty(synBooty);
			}
		}
	}

	public void SettleLv(Vector3 pos)
	{
		if (LV.Instance.SubLvs.Count <= CurrSubLv)
		{
			SpawnBooty(pos);
			return;
		}
		if (GameManager.Instance.isServer)
		{
			WaveComing waveComing = new WaveComing();
			waveComing.WaveType = 4;
			SocketServer.Instance.BigWaveComing(waveComing);
		}
		waitingGoNextWave = true;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.HugeWave, base.transform.position, isAll: true);
		AudioManager.Instance.FadeBgAndPlayNew(BgmType.Nope, faster: true);
		BigTitle.Instance.DisPlayInfo("更多的僵尸要来了！", 4, () =>
		{
			GoNextSubLv();
		});
	}

	private void GoNextSubLv()
	{
		ResetLv();
		LV.Instance.ReadSubLv(LV.Instance.SubLvs[CurrSubLv]);
		CurrSubLv++;
		StartLv(null, 0, needloadLv: false);
	}

	public void SkipRest()
	{
		if (LvTotalTime > 5)
		{
			if (GameManager.Instance.isServer)
			{
				WaveComing waveComing = new WaveComing();
				waveComing.WaveType = 5;
				SocketServer.Instance.BigWaveComing(waveComing);
			}
			FadeEffPanel.Instance.FadeOutIn(() =>
			{
				SkyManager.Instance.DirectSetTime(SkyManager.Instance.Time + LvTotalTime - 5);
				LvTotalTime = 5;
			});
		}
	}

	public void ClientShowBigWave(WaveComing bigWave)
	{
		if (bigWave.WaveType == 0)
		{
			isBigWave = false;
		}
		else if (bigWave.WaveType == 1)
		{
			isBigWave = true;
			UIManager.Instance.ShowBigWaveEF();
			StartCoroutine(WaitTimeDo(() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
				FlagMeter.Instance.FlagRise(BIGwaveNum);
				BIGwaveNum++;
			}, bigWave.WaitTime));
		}
		else if (bigWave.WaveType == 2)
		{
			isBigWave = true;
			UIManager.Instance.ShowFinalWaveEF();
			StartCoroutine(WaitTimeDo(() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Awooga, base.transform.position, isAll: true);
				FlagMeter.Instance.FlagRise(BIGwaveNum);
				BIGwaveNum++;
			}, bigWave.WaitTime));
		}
		else if (bigWave.WaveType == 3)
		{
			LvSpawnisOver = true;
		}
		else
		{
			if (bigWave.WaveType == 4)
			{
				return;
			}
			if (bigWave.WaveType == 5)
			{
				FadeEffPanel.Instance.FadeOutIn(() =>
				{
					LvTotalTime = 5;
				});
			}
			else if (bigWave.WaveType == 6)
			{
				LastStandNoCd = false;
				SeedBank.Instance.StartAllCD();
			}
		}
	}
}
