using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketSave;
using UnityEngine;

public class SocketClient : MonoBehaviour
{
	public static SocketClient Instance;

	private Socket clientSocket;

	private Thread ReseviceThread;

	private bool needLog;

	private bool needLog2;

	private Coroutine WaitConnect;

	private bool OnlineCheck;

	private bool IsHandOver;

	public TextMesh text;

	private int ReConnectCode;

	private bool isReceiving;

	private readonly ConcurrentQueue<byte[]> messageQueue = new ConcurrentQueue<byte[]>();

	private void Awake()
	{
		Instance = this;
		Application.runInBackground = true;
	}

	private void Update()
	{
		if (GameManager.Instance.isClient && !isReceiving)
		{
			CloseSocket();
		}
		byte[] result;
		while (messageQueue.TryDequeue(out result))
		{
			try
			{
				ProcessMessage(result);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Message processing error: {ex}");
				throw ex;
			}
		}
	}

	private void CloseSocket()
	{
		clientSocket.Close();
		ConnectOver();
	}

	public void JoinGame(IPAddress ip, int port, string passWord)
	{
		if (WaitConnect != null || GameManager.Instance.isOnline)
		{
			return;
		}
		WaitConnect = StartCoroutine(WaitLog());
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint endport = new IPEndPoint(ip, port);
		PlayerInfo playerInfo = new PlayerInfo();
		playerInfo.Name = GameManager.Instance.LocalPlayerSave.playerName;
		playerInfo.VersionCode = GameManager.Instance.VersionCode;
		playerInfo.CmdEnable = GameManager.Instance.LocalPlayerSave.CmdEnable;
		playerInfo.ReCntCode = ReConnectCode;
		playerInfo.Password = passWord;
		needLog = true;
		needLog2 = false;
		IsHandOver = false;
		ReseviceThread = new Thread(() =>
		{
			try
			{
				clientSocket.Connect(endport);
				SendMsg(JsonUtility.ToJson(playerInfo), 0, 1);
				ReseviceMsg(clientSocket);
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		});
		ReseviceThread.Start();
	}

	private void ReseviceMsg(Socket clientSocket)
	{
		isReceiving = true;
		byte[] array = new byte[1];
		int num = 0;
		bool flag = false;
		while (true)
		{
			try
			{
				byte[] array2 = new byte[1048576];
				int num2 = clientSocket.Receive(array2);
				List<byte[]> list = new List<byte[]>();
				int num3 = 0;
				int num4 = num2 - num;
				if (flag)
				{
					if (num4 < 0)
					{
						byte[] array3 = array2.Skip(0).Take(num2).ToArray();
						byte[] array4 = new byte[array.Length + array3.Length];
						array.CopyTo(array4, 0);
						array3.CopyTo(array4, array.Length);
						num = -num4;
						array = array4;
					}
					else
					{
						byte[] array5 = array2.Skip(0).Take(num).ToArray();
						byte[] array6 = new byte[array.Length + array5.Length];
						array.CopyTo(array6, 0);
						array5.CopyTo(array6, array.Length);
						list.Add(array6);
						num3 += num;
					}
				}
				if (num4 >= 0)
				{
					num = 0;
					flag = false;
				}
				while (num4 > 0)
				{
					int num5 = BitConverter.ToInt32(new byte[4]
					{
						array2[num3],
						array2[num3 + 1],
						array2[num3 + 2],
						array2[num3 + 3]
					});
					byte[] item = array2.Skip(num3 + 4).Take(num5).ToArray();
					num4 -= 4 + num5;
					if (num4 >= 0)
					{
						list.Add(item);
					}
					else
					{
						flag = true;
						int count = num5 + num4;
						num = -num4;
						array = array2.Skip(num3 + 4).Take(count).ToArray();
					}
					num3 += 4 + num5;
				}
				for (int i = 0; i < list.Count; i++)
				{
					messageQueue.Enqueue(list[i]);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("连接断开" + ex);
				break;
			}
		}
		isReceiving = false;
	}

	private void ProcessMessage(byte[] bytes)
	{
		byte b = bytes[0];
		byte b2 = bytes[1];
		string text = Encoding.UTF8.GetString(bytes, 2, bytes.Length - 2);
		switch (b)
		{
		case 0:
			if (b2 == 0)
			{
				Debug.Log("0-0" + text);
			}
			switch (b2)
			{
			case 1:
			{
				IsHandOver = true;
				needLog = false;
				ConnectInfo connectInfo = JsonUtility.FromJson<ConnectInfo>(text);
				if (LVManager.Instance.InGame)
				{
					UIManager.Instance.LogPanel.DisplayLog("请重试。", null);
				}
				else
				{
					UIManager.Instance.LogPanel.DisplayLog(connectInfo.msg, () =>
					{
						if (!GameManager.Instance.isOnline)
						{
							UIManager.Instance.JoinGame.gameObject.SetActive(value: true);
						}
					});
				}
				CloseSocket();
				break;
			}
			case 2:
			{
				OnlinePlayerInfo onlinePlayerInfo = JsonUtility.FromJson<OnlinePlayerInfo>(text);
				ConnectSuccess();
				PlayerList.Instance.UpdatePlayerList(onlinePlayerInfo.HostPlayer, onlinePlayerInfo.players);
				BattlePlayerList.Instance.UpdatePlayerList(onlinePlayerInfo.HostPlayer, onlinePlayerInfo.players);
				break;
			}
			case 3:
			{
				ConnectSuccess();
				ReConnectInfo reConnectInfo = JsonUtility.FromJson<ReConnectInfo>(text);
				if (reConnectInfo.isWait)
				{
					ReConnect.Instance.OpenInit(needCnt: false);
				}
				else
				{
					ReConnect.Instance.OverClose();
				}
				ReConnect.Instance.LoadPlayerList(reConnectInfo.names);
				break;
			}
			case byte.MaxValue:
				OnlineCheck = true;
				break;
			}
			break;
		case 1:
			switch (b2)
			{
			case 0:
			{
				Debug.Log(text);
				LoadLVBag loadLVBag = JsonUtility.FromJson<LoadLVBag>(text);
				ReConnectCode = loadLVBag.ReCntCode;
				if (loadLVBag.LoadType == 0)
				{
					LVManager.Instance.StartGame(loadLVBag, -1);
				}
				else if (loadLVBag.LoadType == 1)
				{
					LVManager.Instance.ReStartGame();
				}
				else if (loadLVBag.LoadType == 2)
				{
					LVManager.Instance.QuitBattleGame();
				}
				break;
			}
			case 1:
				SeedChooser.Instance.StartRunLv(synClient: true);
				ZombieChooser.Instance.StartRunLv(synClient: true);
				break;
			case 2:
				LVManager.Instance.ClientShowBigWave(JsonUtility.FromJson<WaveComing>(text));
				break;
			case 3:
				PlayerManager.Instance.ClientUpdateSunNum(JsonUtility.FromJson<SunNumBag>(text));
				break;
			case 4:
				SynItem(JsonUtility.FromJson<SynItem>(text));
				break;
			case 5:
			{
				PlayerMap playerMap = JsonUtility.FromJson<PlayerMap>(text);
				BattlePlayerList.Instance.UpdateMapSprite(playerMap.PlayerName, playerMap.Pos);
				break;
			}
			case 6:
			{
				SelectCard selectCard = JsonUtility.FromJson<SelectCard>(text);
				if (selectCard.PlayerName != GameManager.Instance.LocalPlayerSave.playerName)
				{
					if (selectCard.isBack)
					{
						BattlePlayerList.Instance.CancelCard(selectCard.PlayerName, selectCard.cardId);
					}
					else
					{
						BattlePlayerList.Instance.SelectCard(selectCard.PlayerName, selectCard.plantType, selectCard.zombieType, selectCard.noAnim);
					}
				}
				break;
			}
			case 7:
			{
				SelectPrepare selectPrepare = JsonUtility.FromJson<SelectPrepare>(text);
				BattlePlayerList.Instance.UpdateState(selectPrepare.PlayerName, selectPrepare.isPrepare);
				break;
			}
			case 8:
			{
				GameOver gameOver = JsonUtility.FromJson<GameOver>(text);
				if (LV.Instance.CurrLVType == LVType.PvP)
				{
					LVManager.Instance.PvPGameOver(gameOver.pos, gameOver.isRedFail);
				}
				else
				{
					LVManager.Instance.ZombieGameOver(gameOver.pos);
				}
				break;
			}
			case 9:
			{
				PvPTeamList list2 = JsonUtility.FromJson<PvPTeamList>(text);
				PvPSelector.Instance.ClientSynTeam(list2);
				break;
			}
			case 10:
			{
				PvPModeSyn syn3 = JsonUtility.FromJson<PvPModeSyn>(text);
				PvPSelector.Instance.ClientSynMode(syn3);
				break;
			}
			case 11:
			{
				SpectList spectList = JsonUtility.FromJson<SpectList>(text);
				SpectatorList.Instance.ClientSynList(spectList.names);
				break;
			}
			case 12:
			{
				AddCardBag addCardBag = JsonUtility.FromJson<AddCardBag>(text);
				SeedBank.Instance.AddCards(addCardBag.CardTypes, canAddSlot: true);
				break;
			}
			case 13:
			{
				FlagMeterSyn syn2 = JsonUtility.FromJson<FlagMeterSyn>(text);
				FlagMeter.Instance.ClientSyn(syn2);
				break;
			}
			case 14:
			{
				TimetableSyn syn = JsonUtility.FromJson<TimetableSyn>(text);
				Timetable.Instance.ClientSyn(syn);
				break;
			}
			}
			break;
		case 2:
			switch (b2)
			{
			case 0:
			{
				PlantSpawn plantSpawn = JsonUtility.FromJson<PlantSpawn>(text);
				PlantBase newPlant = PlantManager.Instance.GetNewPlant(plantSpawn.plantType);
				if (plantSpawn.SPcode == 2)
				{
					newPlant.InitForCreate(inGrid: false, null, isBlcWhi: false);
				}
				if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(plantSpawn.PlacePlayer))
				{
					plantSpawn.GridPos = new Vector2(0f - plantSpawn.GridPos.x, plantSpawn.GridPos.y);
				}
				newPlant.OnlineId = plantSpawn.OnlineId;
				Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(plantSpawn.GridPos);
				SeedBank.Instance.PlantConfirm(newPlant, gridByWorldPos, -1, plantSpawn.SPcode, plantSpawn.PlacePlayer);
				break;
			}
			case 1:
				SkyManager.Instance.ClientSpawnSun(JsonUtility.FromJson<SunSpawn>(text));
				break;
			case 2:
				SkyManager.Instance.OnlineCollectSun(JsonUtility.FromJson<ClickedSun>(text));
				break;
			case 3:
			{
				PlantPreview apply2 = JsonUtility.FromJson<PlantPreview>(text);
				BattlePlayerList.Instance.PreviewPlant(apply2);
				break;
			}
			case 4:
			{
				UpdateCardCD updateCardCD = JsonUtility.FromJson<UpdateCardCD>(text);
				if (updateCardCD.name == GameManager.Instance.LocalPlayerSave.playerName)
				{
					if (updateCardCD.OK)
					{
						SeedBank.Instance.PlantFailClearCD(updateCardCD.CardId);
					}
				}
				else
				{
					BattlePlayerList.Instance.UpdateCardCD(updateCardCD.name, updateCardCD.CardId, updateCardCD.OK);
				}
				break;
			}
			case 5:
			{
				ShovelPreview shovelPreview = JsonUtility.FromJson<ShovelPreview>(text);
				BattlePlayerList.Instance.PreviewShovel(shovelPreview.PlayerName, shovelPreview.GridPos, shovelPreview.isShow);
				break;
			}
			case 6:
			{
				ToolApply toolApply = JsonUtility.FromJson<ToolApply>(text);
				BattlePlayerList.Instance.PlayShovelAnimation(toolApply.GridPos, toolApply.Sound, toolApply.User);
				break;
			}
			}
			break;
		case 3:
			switch (b2)
			{
			case 0:
			{
				ZombieSpawn spawnInfo = JsonUtility.FromJson<ZombieSpawn>(text);
				ZombieManager.Instance.UpdateZombie(spawnInfo);
				break;
			}
			case 1:
			{
				GraveStoneSpawn graveStoneSpawn = JsonUtility.FromJson<GraveStoneSpawn>(text);
				MapManager.Instance.GetGridByWorldPos(graveStoneSpawn.MapPos).ClientSynGrave(graveStoneSpawn.Type, graveStoneSpawn.isHave);
				break;
			}
			case 2:
			{
				PuddleSpawn puddleSpawn = JsonUtility.FromJson<PuddleSpawn>(text);
				List<Grid> list = new List<Grid>();
				for (int i = 0; i < puddleSpawn.MapPos.Count; i++)
				{
					list.Add(MapManager.Instance.GetGridByWorldPos(puddleSpawn.MapPos[i]));
				}
				Puddle component = UnityEngine.Object.Instantiate(GameManager.Instance.GameConf.Puddle).GetComponent<Puddle>();
				component.CreateInit(list, puddleSpawn.InitPos, puddleSpawn.OnlineId);
				MapManager.Instance.puddles.Add(component);
				break;
			}
			case 3:
			{
				LightingSpawn lightingSpawn = JsonUtility.FromJson<LightingSpawn>(text);
				SkyManager.Instance.ClientLightningThis(MapManager.Instance.GetGridByWorldPos(lightingSpawn.Pos));
				break;
			}
			case 4:
			{
				SynMap synMap = JsonUtility.FromJson<SynMap>(text);
				MapManager.Instance.GetCurrMap(synMap.mapPos).SynMap(synMap);
				break;
			}
			case 5:
			{
				ZombiePreview apply = JsonUtility.FromJson<ZombiePreview>(text);
				BattlePlayerList.Instance.PreviewZombie(apply);
				break;
			}
			case 6:
			{
				PortalSpawn spawn3 = JsonUtility.FromJson<PortalSpawn>(text);
				MapManager.Instance.ClientCreatePortal(spawn3);
				break;
			}
			case 7:
			{
				SynGrid synGrid = JsonUtility.FromJson<SynGrid>(text);
				if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
				{
					synGrid.GridPos = MyTool.ReverseX(synGrid.GridPos);
				}
				MapManager.Instance.GetGridByWorldPos(synGrid.GridPos).ClientSynState(synGrid);
				break;
			}
			case 8:
			{
				SynBooty synBooty = JsonUtility.FromJson<SynBooty>(text);
				if (LVManager.Instance.InGame)
				{
					if (synBooty.isSpawn)
					{
						LVManager.Instance.SpawnBooty(synBooty.pos, synBooty);
					}
					else if (LVManager.Instance.OnlyBooty != null)
					{
						LVManager.Instance.OnlyBooty.CollectBooty();
					}
				}
				break;
			}
			case 9:
			{
				VaseSpawn vaseSpawn = JsonUtility.FromJson<VaseSpawn>(text);
				LvItemManager.Instance.ClientCreateVase(vaseSpawn);
				break;
			}
			case 10:
			{
				CardSpawn cardSpawn = JsonUtility.FromJson<CardSpawn>(text);
				SeedBank.Instance.ClientSpawnCard(cardSpawn);
				break;
			}
			case 11:
			{
				MeltSpawn spawn2 = JsonUtility.FromJson<MeltSpawn>(text);
				LvItemManager.Instance.SpawnMelt(spawn2);
				break;
			}
			case 12:
			{
				FallHailSpawn spawn = JsonUtility.FromJson<FallHailSpawn>(text);
				LvItemManager.Instance.SpawnFallHail(spawn);
				break;
			}
			}
			break;
		case 4:
			switch (b2)
			{
			case 0:
				ChatInput.Instance.AddMessage(text);
				break;
			case 1:
				ChatInput.Instance.AddMessage(text, new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
				break;
			case 2:
			{
				PrivateChatMsg privateChatMsg = JsonUtility.FromJson<PrivateChatMsg>(text);
				string content = "玩家" + privateChatMsg.PlayerName + "悄悄对你说:" + privateChatMsg.content;
				ChatInput.Instance.AddMessage(content, new Color32(123, 123, 123, byte.MaxValue));
				break;
			}
			case 3:
			{
				CommandBag commandBag = JsonUtility.FromJson<CommandBag>(text);
				PlantManager.Instance.PlantInvincible = commandBag.Pinv;
				ZombieManager.Instance.ZombieInvincible = commandBag.Zinv;
				SkyManager.Instance.DayLightCycle = commandBag.DLiCy;
				PlayerManager.Instance.SunInfinite = commandBag.SnInf;
				SeedBank.Instance.isNoCD = commandBag.CdCle;
				ZombieManager.Instance.ZombieDontMove = commandBag.ZomStop;
				LvItemManager.Instance.VaseAlwaysLight = commandBag.VaseXray;
				break;
			}
			case 4:
			{
				WeatherChange bag = JsonUtility.FromJson<WeatherChange>(text);
				SkyManager.Instance.ClientSynWeather(bag);
				break;
			}
			case 5:
			{
				TimeCmd timeCmd = JsonUtility.FromJson<TimeCmd>(text);
				if (timeCmd.time >= 10000)
				{
					timeCmd.time -= 10000;
					SkyManager.Instance.DirectSetTime(timeCmd.time, synClient: true);
				}
				else
				{
					SkyManager.Instance.Time = timeCmd.time;
				}
				break;
			}
			case 6:
			{
				GetAcvment getAcvment = JsonUtility.FromJson<GetAcvment>(text);
				AcvmentManager.Instance.GetAchievement(getAcvment.acv);
				break;
			}
			case 99:
				Debug.Log(text);
				break;
			}
			break;
		}
	}

	private void SendMsg(string content, byte type1, byte type2)
	{
		try
		{
			List<byte> list = new List<byte>();
			list.Add(type1);
			list.Add(type2);
			if (content != "")
			{
				list.AddRange(Encoding.UTF8.GetBytes(content));
			}
			list.InsertRange(0, BitConverter.GetBytes(list.Count));
			byte[] buffer = list.ToArray();
			clientSocket.Send(buffer);
		}
		catch (Exception message)
		{
			Debug.LogError(type1 + "/" + type2 + "发送断开");
			CloseSocket();
			Debug.Log(message);
		}
	}

	private IEnumerator SendHeartbeat()
	{
		float startT = Time.realtimeSinceStartup;
		while (GameManager.Instance.isOnline)
		{
			yield return null;
			if (Time.realtimeSinceStartup - startT > 0.5f)
			{
				SendMsg("", 0, byte.MaxValue);
				startT = Time.realtimeSinceStartup;
			}
		}
	}

	private IEnumerator CheckConnect()
	{
		float startT = Time.realtimeSinceStartup;
		while (GameManager.Instance.isOnline)
		{
			yield return null;
			if (Time.realtimeSinceStartup - startT > 3f)
			{
				if (!OnlineCheck)
				{
					Debug.LogError("无心跳断线");
					CloseSocket();
					break;
				}
				OnlineCheck = false;
				startT = Time.realtimeSinceStartup;
			}
		}
	}

	private IEnumerator WaitLog()
	{
		float startT = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (!(Time.realtimeSinceStartup - startT > 3f));
		Debug.Log("e999");
		CloseSocket();
		UIManager.Instance.LogPanel.Confirm();
		WaitConnect = null;
	}

	private void ConnectSuccess()
	{
		if (!GameManager.Instance.isOnline)
		{
			if (WaitConnect != null)
			{
				StopCoroutine(WaitConnect);
			}
			WaitConnect = null;
			needLog2 = true;
			Application.runInBackground = true;
			GameManager.Instance.isOnline = true;
			OnlineCheck = true;
			StartCoroutine(CheckConnect());
			StartCoroutine(SendHeartbeat());
			UIManager.Instance.ConnectSuccess();
		}
	}

	public void CloseClient()
	{
		SendMsg("", 0, 2);
		IsHandOver = true;
		needLog = false;
		needLog2 = false;
		CloseSocket();
	}

	public void ReConnectGiveUp()
	{
		CloseClient();
		ConnectOverDone();
	}

	private void ConnectOver()
	{
		Debug.LogError("6666");
		if (!GameManager.Instance.isOnline)
		{
			return;
		}
		GameManager.Instance.isOnline = false;
		if (WaitConnect != null)
		{
			StopCoroutine(WaitConnect);
		}
		if (LVManager.Instance.InGame)
		{
			if (IsHandOver)
			{
				LVManager.Instance.QuitBattleGame();
				ConnectOverDone();
			}
			else
			{
				StopAllCoroutines();
				ReConnect.Instance.OpenInit(needCnt: true);
			}
		}
		else
		{
			ConnectOverDone();
		}
	}

	private void ConnectOverDone()
	{
		ReConnectCode = 0;
		SpectatorList.Instance.ClientSynList(new List<string>());
		PvPSelector.Instance.ResetPvPInfo();
		PlayerList.Instance.UpdatePlayerList(null, new List<PlayerInfo>());
		BattlePlayerList.Instance.UpdatePlayerList(null, new List<PlayerInfo>());
		StopAllCoroutines();
		if (needLog && needLog2)
		{
			UIManager.Instance.LogPanel.DisplayLog("与服务器断开连接。", null);
		}
		else if (needLog)
		{
			UIManager.Instance.LogPanel.DisplayLog("连接超时。", () =>
			{
				UIManager.Instance.JoinGame.gameObject.SetActive(value: true);
			});
		}
	}

	public void SendChatMsg(string msg)
	{
		SendMsg(msg, 2, 0);
	}

	public void SendPrivateChatMsg(string name, string content)
	{
		PrivateChatMsg privateChatMsg = new PrivateChatMsg();
		privateChatMsg.PlayerName = name;
		privateChatMsg.content = content;
		SendMsg(JsonUtility.ToJson(privateChatMsg), 2, 1);
	}

	public void ChangeMap(PlayerMap map)
	{
		SendMsg(JsonUtility.ToJson(map), 1, 3);
	}

	public void SelectCard(SelectCard card)
	{
		SendMsg(JsonUtility.ToJson(card), 1, 4);
	}

	public void SelectPrepare(SelectPrepare prepare)
	{
		SendMsg(JsonUtility.ToJson(prepare), 1, 5);
	}

	public void ApplyTool(ToolApply apply)
	{
		SendMsg(JsonUtility.ToJson(apply), 1, 1);
	}

	public void UpdateCD(int cardID, bool Ok)
	{
		UpdateCardCD updateCardCD = new UpdateCardCD();
		updateCardCD.CardId = cardID;
		updateCardCD.OK = Ok;
		SendMsg(JsonUtility.ToJson(updateCardCD), 1, 8);
	}

	public void ClickedSun(ClickedSun sun)
	{
		SendMsg(JsonUtility.ToJson(sun), 1, 2);
	}

	public void SynItem(SynItem syn)
	{
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Plant)
		{
			List<PlantBase> plants = PlantManager.Instance.plants;
			for (int i = 0; i < plants.Count; i++)
			{
				if (plants[i].OnlineId == syn.OnlineId)
				{
					plants[i].OnlineSynPlant(syn);
					break;
				}
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Zombie)
		{
			List<ZombieBase> list = new List<ZombieBase>(ZombieManager.Instance.GetAllZombies());
			List<ZombieBase> allHypZombies = ZombieManager.Instance.GetAllHypZombies();
			list.AddRange(allHypZombies);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].OnlineId == syn.OnlineId)
				{
					list[j].OnlineSynZombie(syn);
					break;
				}
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Puddle)
		{
			List<Puddle> puddles = MapManager.Instance.puddles;
			for (int k = 0; k < puddles.Count; k++)
			{
				if (puddles[k].OnlineId == syn.OnlineId)
				{
					puddles[k].StartDisappear();
					break;
				}
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Portal)
		{
			List<PortalController> portalCs = MapManager.Instance.portalCs;
			for (int l = 0; l < portalCs.Count; l++)
			{
				if (portalCs[l].OnlineId == syn.OnlineId)
				{
					portalCs[l].ClientReset(syn);
					break;
				}
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Vase)
		{
			LvItemManager.Instance.SynVase(syn);
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Card)
		{
			SeedBank.Instance.SynDropCard(syn);
		}
	}

	public void ApplyPlacePlant(PlantSpawn spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 0);
	}

	public void ApplyPlaceZombie(ZombieSpawnApply spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 10);
	}

	public void ApplyPlacePreview(PlantPreview spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 7);
	}

	public void ApplyShovelPreview(ShovelPreview spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 9);
	}

	public void ApplyZombiePreview(ZombiePreview spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 11);
	}

	public void ApplyJoinTeam(JoinTeamApply spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 12);
	}

	public void ApplyJoinSpect(JoinSpecApply spawn)
	{
		SendMsg(JsonUtility.ToJson(spawn), 1, 13);
	}

	public void SendSynBag(SynItem syn)
	{
		SendMsg(JsonUtility.ToJson(syn), 1, 6);
	}

	public void SendSlotMBag(SlotMchBag bag)
	{
		SendMsg(JsonUtility.ToJson(bag), 1, 14);
	}
}
