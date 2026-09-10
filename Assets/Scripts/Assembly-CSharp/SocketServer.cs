using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class SocketServer : MonoBehaviour
{
	public static SocketServer Instance;

	private Socket socketWatch;

	public bool isServerOpen;

	private List<UnityAction> actions = new List<UnityAction>();

	private PlayerInfo HostPlayer;

	private List<PlayerInfo> players = new List<PlayerInfo>();

	private List<Socket> sockets = new List<Socket>();

	private int ReConnectCode;

	private List<PlayerInfo> ReConnectPlayer = new List<PlayerInfo>();

	private List<PlayerInfo> HandQuitPlayer = new List<PlayerInfo>();

	private int itemId;

	public int noHostPlayerNum => players.Count;

	public int ItemId
	{
		get
		{
			itemId++;
			return itemId;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		while (actions.Count > 0)
		{
			UnityAction unityAction = actions[0];
			actions.RemoveAt(0);
			try
			{
				unityAction();
			}
			catch (Exception ex)
			{
				Debug.Log("服务器主动断开" + ex);
			}
		}
	}

	public void StartServer(IPAddress ip, int port)
	{
		if (!GameManager.Instance.isOnline)
		{
			socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint localEP = new IPEndPoint(ip, port);
			socketWatch.Bind(localEP);
			socketWatch.Listen(4);
			new Thread(Recevice).Start(socketWatch);
			isServerOpen = true;
			HostPlayer = new PlayerInfo();
			HostPlayer.Name = GameManager.Instance.LocalPlayerSave.playerName;
			GameManager.Instance.HostName = GameManager.Instance.LocalPlayerSave.playerName;
			GameManager.Instance.isOnline = true;
			PlayerList.Instance.UpdatePlayerList(HostPlayer, null);
			BattlePlayerList.Instance.UpdatePlayerList(HostPlayer, null);
			StartCoroutine(SendHeartbeat());
			StartCoroutine(CheckConnect());
		}
	}

	private void Recevice(object obj)
	{
		Debug.Log("启动成功。");
		Socket socket = obj as Socket;
		while (isServerOpen)
		{
			try
			{
				_ = string.Empty;
				Socket socket2 = socket.Accept();
				string text = socket2.RemoteEndPoint.ToString();
				ReceseMsgGoing(socket2);
				Debug.Log(text + ":连接到服务器。");
			}
			catch (Exception)
			{
				if (!isServerOpen)
				{
					Debug.Log("Error:1");
				}
			}
		}
	}

	private void ReceseMsgGoing(Socket TxSocket)
	{
		new Thread(() =>
		{
			PlayerInfo playerInfo = new PlayerInfo();
			playerInfo.VersionCode = "1";
			byte[] array = new byte[1];
			int num = 0;
			bool flag = false;
			while (true)
			{
				try
				{
					byte[] array2 = new byte[1048576];
					int num2 = TxSocket.Receive(array2);
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
						byte b = list[i][0];
						byte b2 = list[i][1];
						string getmsg = Encoding.UTF8.GetString(list[i], 2, list[i].Length - 2);
						switch (b)
						{
						case 0:
							switch (b2)
							{
							case 1:
							{
								playerInfo = JsonUtility.FromJson<PlayerInfo>(getmsg);
								bool flag2 = true;
								if (playerInfo.VersionCode == GameManager.Instance.VersionCode)
								{
									if (CanReConnect(playerInfo))
									{
										if (ReConnectCode != playerInfo.ReCntCode)
										{
											flag2 = false;
										}
									}
									else if (UIManager.Instance.HostPasswordInput.text != "" && playerInfo.Password != UIManager.Instance.HostPasswordInput.text)
									{
										flag2 = false;
										SendFailConnectMsg("密码错误，加入失败。", TxSocket);
									}
									else if (players.Count >= 3)
									{
										flag2 = false;
										SendFailConnectMsg("人数过多，加入失败。", TxSocket);
									}
									else if (HostPlayer.Name == playerInfo.Name)
									{
										flag2 = false;
										SendFailConnectMsg("与在线玩家重名，加入失败。", TxSocket);
									}
									else if (LVManager.Instance.InGame)
									{
										flag2 = false;
										SendFailConnectMsg("游戏已开始，加入失败。", TxSocket);
									}
									else if (playerInfo.CmdEnable != GameManager.Instance.LocalPlayerSave.CmdEnable)
									{
										flag2 = false;
										if (playerInfo.CmdEnable)
										{
											SendFailConnectMsg("你已启用指令，而服务器未启用，无法加入。", TxSocket);
										}
										else
										{
											SendFailConnectMsg("你未启用指令，而服务器已启用，无法加入。", TxSocket);
										}
									}
									else
									{
										for (int num6 = 0; num6 < players.Count; num6++)
										{
											if (players[num6].Name == playerInfo.Name)
											{
												flag2 = false;
												SendFailConnectMsg("与在线玩家重名，加入失败。", TxSocket);
												break;
											}
										}
									}
									if (flag2)
									{
										sockets.Add(TxSocket);
										actions.Add(() =>
										{
											playerInfo.Heartbeat = true;
											if (CanReConnect(playerInfo))
											{
												players.Add(playerInfo);
												for (int j = 0; j < ReConnectPlayer.Count; j++)
												{
													if (ReConnectPlayer[j].Name == playerInfo.Name)
													{
														ReConnectPlayer.Remove(ReConnectPlayer[j]);
														break;
													}
												}
												ReConnectListChange();
											}
											else
											{
												AddNewPlayer(playerInfo);
												SendCommandBag(TxSocket);
												PvPModeSyn syn = new PvPModeSyn
												{
													Mode = PvPSelector.Instance.CurrMode
												};
												SynPvPMode(syn, TxSocket);
												PvPSelector.Instance.ServerSynTeam();
												SpectatorList.Instance.ServerSynSpectList();
											}
										});
										break;
									}
									TxSocket.Close();
								}
								else
								{
									SendFailConnectMsg("与服务器游戏版本不同。服务器游戏版本：v" + GameManager.Instance.VersionCode, TxSocket);
									TxSocket.Close();
								}
								goto end_IL_0817;
							}
							case 2:
								HandQuitPlayer.Add(playerInfo);
								sockets.Remove(TxSocket);
								TxSocket.Close();
								break;
							case byte.MaxValue:
								playerInfo.Heartbeat = true;
								break;
							}
							break;
						case 1:
							switch (b2)
							{
							case 0:
								actions.Add(() =>
								{
									PlantSpawn plantSpawn = JsonUtility.FromJson<PlantSpawn>(getmsg);
									UpdateCardCD updateCardCD = new UpdateCardCD
									{
										CardId = plantSpawn.CardId,
										name = playerInfo.Name
									};
									if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(playerInfo.Name))
									{
										plantSpawn.GridPos = new Vector2(0f - plantSpawn.GridPos.x, plantSpawn.GridPos.y);
									}
									PlantBase newPlant = PlantManager.Instance.GetNewPlant(plantSpawn.plantType);
									Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(plantSpawn.GridPos);
									if (SeedBank.Instance.CheckPlant(newPlant, gridByWorldPos, -1, playerInfo.Name))
									{
										int needSun = -1;
										updateCardCD.OK = false;
										if (plantSpawn.SPcode == 2)
										{
											newPlant.InitForCreate(inGrid: false, null, isBlcWhi: false);
											needSun = SeedBank.Instance.GetPlantNc(newPlant.GetPlantType()).NeedNum;
										}
										SeedBank.Instance.PlantConfirm(newPlant, gridByWorldPos, needSun, plantSpawn.SPcode, playerInfo.Name);
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD));
										BattlePlayerList.Instance.UpdateCardCD(updateCardCD.name, updateCardCD.CardId, updateCardCD.OK);
									}
									else
									{
										updateCardCD.OK = true;
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD), TxSocket);
										UnityEngine.Object.Destroy(newPlant.gameObject);
									}
									SeedBank.Instance.LikeColumnPlace(gridByWorldPos, plantSpawn.plantType, ZombieType.Nope, -1, playerInfo.Name, isRat: false);
								});
								break;
							case 1:
								actions.Add(() =>
								{
									ToolApply toolApply = JsonUtility.FromJson<ToolApply>(getmsg);
									toolApply.User = playerInfo.Name;
									Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(toolApply.GridPos);
									if (toolApply.type == ToolType.Shovel)
									{
										if (Shovel.Instance.ClearPlant(gridByWorldPos, toolApply.GridPos, playerInfo.Name))
										{
											BattlePlayerList.Instance.PlayShovelAnimation(gridByWorldPos.Position, toolApply.Sound, playerInfo.Name);
										}
										SendMsg(2, 6, JsonUtility.ToJson(toolApply));
									}
									else if (toolApply.type == ToolType.Glove)
									{
										Glove.Instance.SynClient(toolApply.OnlineId, toolApply.GridPos);
									}
								});
								break;
							case 2:
								actions.Add(() =>
								{
									ClickedSun sun = JsonUtility.FromJson<ClickedSun>(getmsg);
									SkyManager.Instance.OnlineCollectSun(sun);
								});
								break;
							case 3:
								actions.Add(() =>
								{
									PlayerMap playerMap = JsonUtility.FromJson<PlayerMap>(getmsg);
									playerMap.PlayerName = playerInfo.Name;
									ChangeMap(playerMap);
									BattlePlayerList.Instance.UpdateMapSprite(playerMap.PlayerName, playerMap.Pos);
								});
								break;
							case 4:
								actions.Add(() =>
								{
									SelectCard selectCard = JsonUtility.FromJson<SelectCard>(getmsg);
									selectCard.PlayerName = playerInfo.Name;
									SelectCard(selectCard);
									if (selectCard.isBack)
									{
										BattlePlayerList.Instance.CancelCard(selectCard.PlayerName, selectCard.cardId);
									}
									else
									{
										BattlePlayerList.Instance.SelectCard(selectCard.PlayerName, selectCard.plantType, selectCard.zombieType, selectCard.noAnim);
									}
								});
								break;
							case 5:
								actions.Add(() =>
								{
									SelectPrepare selectPrepare = JsonUtility.FromJson<SelectPrepare>(getmsg);
									selectPrepare.PlayerName = playerInfo.Name;
									SelectPrepare(selectPrepare);
									BattlePlayerList.Instance.UpdateState(selectPrepare.PlayerName, selectPrepare.isPrepare);
								});
								break;
							case 6:
								actions.Add(() =>
								{
									SynItem syn = JsonUtility.FromJson<SynItem>(getmsg);
									SynItem(syn);
								});
								break;
							case 7:
								actions.Add(() =>
								{
									PlantPreview plantPreview = JsonUtility.FromJson<PlantPreview>(getmsg);
									BattlePlayerList.Instance.PreviewPlant(plantPreview);
									PlacePreview(plantPreview, TxSocket);
								});
								break;
							case 8:
								actions.Add(() =>
								{
									UpdateCardCD updateCardCD = JsonUtility.FromJson<UpdateCardCD>(getmsg);
									updateCardCD.name = playerInfo.Name;
									BattlePlayerList.Instance.UpdateCardCD(updateCardCD.name, updateCardCD.CardId, updateCardCD.OK);
									SendMsg(2, 4, JsonUtility.ToJson(updateCardCD), TxSocket, OutThis: true);
								});
								break;
							case 9:
								actions.Add(() =>
								{
									ShovelPreview shovelPreview = JsonUtility.FromJson<ShovelPreview>(getmsg);
									BattlePlayerList.Instance.PreviewShovel(shovelPreview.PlayerName, shovelPreview.GridPos, shovelPreview.isShow);
									ShovelPreview(shovelPreview, TxSocket);
								});
								break;
							case 10:
								actions.Add(() =>
								{
									ZombieSpawnApply zombieSpawnApply = JsonUtility.FromJson<ZombieSpawnApply>(getmsg);
									UpdateCardCD updateCardCD = new UpdateCardCD
									{
										CardId = zombieSpawnApply.CardId,
										name = playerInfo.Name
									};
									if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(playerInfo.Name))
									{
										zombieSpawnApply.GridPos = new Vector2(0f - zombieSpawnApply.GridPos.x, zombieSpawnApply.GridPos.y);
									}
									ZombieBase newZombie = ZombieManager.Instance.GetNewZombie(zombieSpawnApply.Type);
									Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(zombieSpawnApply.GridPos);
									if (SeedBank.Instance.CheckZombie(zombieSpawnApply.Type, gridByWorldPos, -1, playerInfo.Name))
									{
										updateCardCD.OK = false;
										SeedBank.Instance.ZombieConfirm(zombieSpawnApply.Type, newZombie, gridByWorldPos, -1, playerInfo.Name, zombieSpawnApply.isRat);
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD));
										BattlePlayerList.Instance.UpdateCardCD(updateCardCD.name, updateCardCD.CardId, updateCardCD.OK);
									}
									else
									{
										updateCardCD.OK = true;
										SendMsg(2, 4, JsonUtility.ToJson(updateCardCD), TxSocket);
										UnityEngine.Object.Destroy(newZombie.gameObject);
									}
									SeedBank.Instance.LikeColumnPlace(gridByWorldPos, PlantType.Nope, zombieSpawnApply.Type, -1, playerInfo.Name, zombieSpawnApply.isRat);
								});
								break;
							case 11:
								actions.Add(() =>
								{
									ZombiePreview zombiePreview = JsonUtility.FromJson<ZombiePreview>(getmsg);
									BattlePlayerList.Instance.PreviewZombie(zombiePreview);
									ZombiePreview(zombiePreview, TxSocket);
								});
								break;
							case 12:
								actions.Add(() =>
								{
									if (JsonUtility.FromJson<JoinTeamApply>(getmsg).isRed)
									{
										PvPSelector.Instance.JoinRed(playerInfo.Name);
									}
									else
									{
										PvPSelector.Instance.JoinBlue(playerInfo.Name);
									}
								});
								break;
							case 13:
								actions.Add(() =>
								{
									JoinSpecApply joinSpecApply = JsonUtility.FromJson<JoinSpecApply>(getmsg);
									SpectatorList.Instance.ClientJoinSpect(playerInfo.Name, joinSpecApply.isJoin);
								});
								break;
							case 14:
								actions.Add(() =>
								{
									SlotMchBag bag = JsonUtility.FromJson<SlotMchBag>(getmsg);
									SlotMachine.Instance.ClientSyn(bag);
								});
								break;
							}
							break;
						case 2:
							switch (b2)
							{
							case 0:
								actions.Add(() =>
								{
									ChatInput.Instance.AddMessage(getmsg);
									SendMsg(4, 0, getmsg, TxSocket, OutThis: true);
								});
								break;
							case 1:
								actions.Add(() =>
								{
									PrivateChatMsg privateChatMsg = JsonUtility.FromJson<PrivateChatMsg>(getmsg);
									if (privateChatMsg.PlayerName == GameManager.Instance.LocalPlayerSave.playerName)
									{
										string content = "玩家" + playerInfo.Name + "悄悄对你说:" + privateChatMsg.content;
										ChatInput.Instance.AddMessage(content, new Color32(123, 123, 123, byte.MaxValue));
									}
									else
									{
										SendPrivateChatMsg(privateChatMsg.PlayerName, privateChatMsg.content, playerInfo.Name);
									}
								});
								break;
							}
							break;
						}
						continue;
						end_IL_0817:
						break;
					}
				}
				catch (Exception message)
				{
					Debug.Log(message);
					sockets.Remove(TxSocket);
					TxSocket.Close();
					if (playerInfo.VersionCode != "1")
					{
						actions.Add(() =>
						{
							RemovePlayer(playerInfo);
						});
					}
					break;
				}
			}
		}).Start();
	}

	private void SendMsg(byte type1, byte type2, string content = "", Socket socket = null, bool OutThis = false)
	{
		List<byte> list = new List<byte>();
		list.Add(type1);
		list.Add(type2);
		list.AddRange(Encoding.UTF8.GetBytes(content));
		list.InsertRange(0, BitConverter.GetBytes(list.Count));
		byte[] buffer = list.ToArray();
		if (socket == null)
		{
			for (int i = 0; i < sockets.Count; i++)
			{
				try
				{
					sockets[i].Send(buffer);
				}
				catch (Exception)
				{
					PlayerInfo playerInfo = null;
					if (sockets.Contains(sockets[i]))
					{
						playerInfo = players[sockets.IndexOf(sockets[i])];
					}
					sockets[i].Close();
					sockets.Remove(sockets[i]);
					if (playerInfo != null && playerInfo.VersionCode != "1")
					{
						actions.Add(() =>
						{
							RemovePlayer(playerInfo);
						});
					}
					Debug.Log("677" + playerInfo.Name);
				}
			}
			return;
		}
		try
		{
			if (OutThis)
			{
				for (int num = 0; num < sockets.Count; num++)
				{
					if (sockets[num] != socket)
					{
						sockets[num].Send(buffer);
					}
				}
			}
			else
			{
				socket.Send(buffer);
			}
		}
		catch (Exception)
		{
			PlayerInfo playerInfo2 = players[sockets.IndexOf(socket)];
			socket.Close();
			sockets.Remove(socket);
			if (playerInfo2.VersionCode != "1")
			{
				actions.Add(() =>
				{
					RemovePlayer(playerInfo2);
				});
			}
		}
	}

	private void SendMsg(byte type1, byte type2, string content, string playerName)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].Name == playerName && sockets.Count > i)
			{
				SendMsg(type1, type2, content, sockets[i]);
				break;
			}
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
				SendMsg(0, byte.MaxValue);
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
			if (!(Time.realtimeSinceStartup - startT > 3f))
			{
				continue;
			}
			for (int i = 0; i < players.Count; i++)
			{
				if (!players[i].Heartbeat)
				{
					if (sockets.Count > i)
					{
						Debug.LogError(players[i].Name + "无心跳断开");
						sockets[i].Close();
						break;
					}
				}
				else
				{
					players[i].Heartbeat = false;
				}
			}
			startT = Time.realtimeSinceStartup;
		}
	}

	public void CloseServer()
	{
		HostPlayer = null;
		for (int i = 0; i < sockets.Count; i++)
		{
			sockets[i].Close();
		}
		sockets.Clear();
		socketWatch.Close();
		socketWatch = null;
		isServerOpen = false;
		GameManager.Instance.isOnline = false;
		BattlePlayerList.Instance.UpdatePlayerList(null, players);
		PlayerList.Instance.UpdatePlayerList(null, players);
		StopAllCoroutines();
	}

	public void SynItem(SynItem syn)
	{
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Plant)
		{
			PlantBase plantBase = PlantManager.Instance.OnlineGetPlant(syn.OnlineId);
			if (plantBase != null)
			{
				plantBase.OnlineSynPlant(syn);
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Zombie)
		{
			ZombieBase zombieBase = ZombieManager.Instance.OnlineGetZombie(syn.OnlineId);
			if (zombieBase != null)
			{
				zombieBase.OnlineSynZombie(syn);
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Puddle)
		{
			List<Puddle> puddles = MapManager.Instance.puddles;
			for (int i = 0; i < puddles.Count; i++)
			{
				if (puddles[i].OnlineId == syn.OnlineId)
				{
					puddles[i].StartDisappear();
					break;
				}
			}
		}
		if (syn.Type == SynItemType.AllItem || syn.Type == SynItemType.Portal)
		{
			List<PortalController> portalCs = MapManager.Instance.portalCs;
			for (int j = 0; j < portalCs.Count; j++)
			{
				if (portalCs[j].OnlineId == syn.OnlineId)
				{
					portalCs[j].ClientReset(syn);
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

	public List<string> GetAllPlayerNameList()
	{
		List<string> list = new List<string>();
		list.Add(HostPlayer.Name);
		for (int i = 0; i < players.Count; i++)
		{
			list.Add(players[i].Name);
		}
		return list;
	}

	public void SendFailConnectMsg(string msg, Socket socket)
	{
		ConnectInfo connectInfo = new ConnectInfo();
		connectInfo.msg = msg;
		SendMsg(0, 1, JsonUtility.ToJson(connectInfo), socket);
	}

	public void SendHostCD(int CardID, bool isOK)
	{
		UpdateCardCD updateCardCD = new UpdateCardCD();
		updateCardCD.name = GameManager.Instance.LocalPlayerSave.playerName;
		updateCardCD.CardId = CardID;
		updateCardCD.OK = isOK;
		SendMsg(2, 4, JsonUtility.ToJson(updateCardCD));
	}

	public void SendChatMsg(string content)
	{
		SendMsg(4, 0, content);
	}

	public void SendPrivateChatMsg(string name, string content, string sender)
	{
		PrivateChatMsg privateChatMsg = new PrivateChatMsg();
		privateChatMsg.PlayerName = sender;
		privateChatMsg.content = content;
		SendMsg(4, 2, JsonUtility.ToJson(privateChatMsg), name);
	}

	public void KickPlayer(string name)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].Name == name && sockets.Count > i)
			{
				HandQuitPlayer.Add(players[i]);
				SendFailConnectMsg("你被踢出了游戏。", sockets[i]);
				sockets[i].Close();
				break;
			}
		}
	}

	public void LoadLv(LoadLVBag loadLV)
	{
		ReConnectCode = UnityEngine.Random.Range(100000, 999999);
		loadLV.ReCntCode = ReConnectCode;
		Debug.Log(JsonUtility.ToJson(loadLV));
		SendMsg(1, 0, JsonUtility.ToJson(loadLV));
	}

	public void SendAddCard(AddCardBag cardBag, string playerName)
	{
		SendMsg(1, 12, JsonUtility.ToJson(cardBag), playerName);
	}

	public void StartRunLv()
	{
		SendMsg(1, 1);
	}

	public void BigWaveComing(WaveComing bigWave)
	{
		SendMsg(1, 2, JsonUtility.ToJson(bigWave));
	}

	public void UpdateSunNum(SunNumBag SunNum)
	{
		SendMsg(1, 3, JsonUtility.ToJson(SunNum));
	}

	public void SendSynBag(SynItem syn)
	{
		SendMsg(1, 4, JsonUtility.ToJson(syn));
	}

	public void ChangeMap(PlayerMap map)
	{
		SendMsg(1, 5, JsonUtility.ToJson(map));
	}

	public void SelectCard(SelectCard card)
	{
		SendMsg(1, 6, JsonUtility.ToJson(card));
	}

	public void SelectPrepare(SelectPrepare prepare)
	{
		SendMsg(1, 7, JsonUtility.ToJson(prepare));
	}

	public void GameOver(GameOver over)
	{
		SendMsg(1, 8, JsonUtility.ToJson(over));
	}

	public void SynTeamList(PvPTeamList over)
	{
		SendMsg(1, 9, JsonUtility.ToJson(over));
	}

	public void SynPvPMode(PvPModeSyn syn, Socket socket = null)
	{
		SendMsg(1, 10, JsonUtility.ToJson(syn), socket);
	}

	public void SynSpectList(SpectList over)
	{
		SendMsg(1, 11, JsonUtility.ToJson(over));
	}

	public void SynFlagMeter(FlagMeterSyn syn)
	{
		SendMsg(1, 13, JsonUtility.ToJson(syn));
	}

	public void SynTimeTable(TimetableSyn syn)
	{
		SendMsg(1, 14, JsonUtility.ToJson(syn));
	}

	public void SpawnSun(SunSpawn spawn)
	{
		SendMsg(2, 1, JsonUtility.ToJson(spawn));
	}

	public void ClickedSun(ClickedSun sun)
	{
		SendMsg(2, 2, JsonUtility.ToJson(sun));
	}

	public void SpawnPlant(PlantSpawn spawn)
	{
		SendMsg(2, 0, JsonUtility.ToJson(spawn));
	}

	public void PlacePreview(PlantPreview spawn, Socket socket)
	{
		SendMsg(2, 3, JsonUtility.ToJson(spawn), socket, OutThis: true);
	}

	public void ZombiePreview(ZombiePreview spawn, Socket socket)
	{
		SendMsg(3, 5, JsonUtility.ToJson(spawn), socket, OutThis: true);
	}

	public void ShovelPreview(ShovelPreview spawn, Socket socket)
	{
		SendMsg(2, 5, JsonUtility.ToJson(spawn), socket, OutThis: true);
	}

	public void SpawnZombie(ZombieSpawn spawn)
	{
		SendMsg(3, 0, JsonUtility.ToJson(spawn));
	}

	public void SpawnGraveStone(GraveStoneSpawn spawn)
	{
		SendMsg(3, 1, JsonUtility.ToJson(spawn));
	}

	public void SpawnPuddle(PuddleSpawn spawn)
	{
		SendMsg(3, 2, JsonUtility.ToJson(spawn));
	}

	public void SpawnPortal(PortalSpawn spawn)
	{
		SendMsg(3, 6, JsonUtility.ToJson(spawn));
	}

	public void SpawnVase(VaseSpawn spawn)
	{
		SendMsg(3, 9, JsonUtility.ToJson(spawn));
	}

	public void SpawnDropCard(CardSpawn spawn)
	{
		SendMsg(3, 10, JsonUtility.ToJson(spawn));
	}

	public void SpawnMelt(MeltSpawn spawn)
	{
		SendMsg(3, 11, JsonUtility.ToJson(spawn));
	}

	public void SpawnFallHail(FallHailSpawn spawn)
	{
		SendMsg(3, 12, JsonUtility.ToJson(spawn));
	}

	public void SpawnLightning(LightingSpawn spawn)
	{
		SendMsg(3, 3, JsonUtility.ToJson(spawn));
	}

	public void SendShovelAnim(ToolApply apply)
	{
		SendMsg(2, 6, JsonUtility.ToJson(apply));
	}

	public void SendGridState(SynGrid apply)
	{
		SendMsg(3, 7, JsonUtility.ToJson(apply));
	}

	public void SendCommandBag(Socket socket = null)
	{
		CommandBag commandBag = new CommandBag();
		commandBag.Pinv = PlantManager.Instance.PlantInvincible;
		commandBag.Zinv = ZombieManager.Instance.ZombieInvincible;
		commandBag.DLiCy = SkyManager.Instance.DayLightCycle;
		commandBag.SnInf = PlayerManager.Instance.SunInfinite;
		commandBag.CdCle = SeedBank.Instance.isNoCD;
		commandBag.ZomStop = ZombieManager.Instance.ZombieDontMove;
		commandBag.VaseXray = LvItemManager.Instance.VaseAlwaysLight;
		SendMsg(4, 3, JsonUtility.ToJson(commandBag), socket);
	}

	public void SendWeatherCmd(WeatherChange cmd)
	{
		SendMsg(4, 4, JsonUtility.ToJson(cmd));
	}

	public void SendTimeCmd(TimeCmd cmd)
	{
		SendMsg(4, 5, JsonUtility.ToJson(cmd));
	}

	public void SendMapSyn(SynMap cmd)
	{
		SendMsg(3, 4, JsonUtility.ToJson(cmd));
	}

	public void SendSynBooty(SynBooty Syn)
	{
		SendMsg(3, 8, JsonUtility.ToJson(Syn));
	}

	public void SendAcvmentGet(Acvname acvname, string playerName)
	{
		GetAcvment getAcvment = new GetAcvment();
		getAcvment.acv = acvname;
		SendMsg(4, 6, JsonUtility.ToJson(getAcvment), playerName);
	}

	private void SendWaitReConnect(bool isWait)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < ReConnectPlayer.Count; i++)
		{
			list.Add(ReConnectPlayer[i].Name);
		}
		ReConnectInfo reConnectInfo = new ReConnectInfo();
		reConnectInfo.isWait = isWait;
		reConnectInfo.names = list;
		SendMsg(0, 3, JsonUtility.ToJson(reConnectInfo));
	}

	private void ReConnectListChange()
	{
		if (ReConnectPlayer.Count > 0)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < ReConnectPlayer.Count; i++)
			{
				list.Add(ReConnectPlayer[i].Name);
			}
			ReConnect.Instance.LoadPlayerList(list);
			SendWaitReConnect(isWait: true);
		}
		else
		{
			SendWaitReConnect(isWait: false);
			ReConnect.Instance.OverClose();
		}
	}

	private bool CanReConnect(PlayerInfo info)
	{
		for (int i = 0; i < ReConnectPlayer.Count; i++)
		{
			if (ReConnectPlayer[i].Name == info.Name)
			{
				return true;
			}
		}
		return false;
	}

	public void GiveUpReConnect()
	{
		for (int i = 0; i < ReConnectPlayer.Count; i++)
		{
			RemoveDone(ReConnectPlayer[i]);
		}
		ReConnectPlayer.Clear();
		ReConnectListChange();
	}

	private void AddNewPlayer(PlayerInfo player)
	{
		players.Add(player);
		BattlePlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		PlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		string content = player.Name + "加入了游戏";
		ChatInput.Instance.AddMessage(content, new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
		SendMsg(4, 1, content);
		OnlinePlayerInfo onlinePlayerInfo = new OnlinePlayerInfo();
		onlinePlayerInfo.HostPlayer = HostPlayer;
		onlinePlayerInfo.players = players;
		SendMsg(0, 2, JsonUtility.ToJson(onlinePlayerInfo));
	}

	private void RemovePlayer(PlayerInfo player)
	{
		if (!players.Contains(player))
		{
			return;
		}
		players.Remove(player);
		if (LVManager.Instance.InGame)
		{
			if (HandQuitPlayer.Remove(player))
			{
				RemoveDone(player);
				return;
			}
			ReConnectPlayer.Add(player);
			ReConnect.Instance.OpenInit(needCnt: false);
			ReConnectListChange();
		}
		else
		{
			RemoveDone(player);
		}
	}

	private void RemoveDone(PlayerInfo player)
	{
		string content = player.Name + "退出了游戏";
		ChatInput.Instance.AddMessage(content, new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue));
		SendMsg(4, 1, content);
		BattlePlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		PlayerList.Instance.UpdatePlayerList(HostPlayer, players);
		PvPSelector.Instance.ClearQuitPlayer(player.Name);
		SpectatorList.Instance.ClearPlayer(player.Name);
		OnlinePlayerInfo onlinePlayerInfo = new OnlinePlayerInfo();
		onlinePlayerInfo.HostPlayer = HostPlayer;
		onlinePlayerInfo.players = players;
		SendMsg(0, 2, JsonUtility.ToJson(onlinePlayerInfo));
	}
}
