using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.UI;

public class GoOtherMap : MonoBehaviour
{
	public static GoOtherMap Instance;

	public Image GotoMap1;

	public Image GotoMap2;

	private List<int> MapIds = new List<int>();

	private List<MapBase> mapList => MapManager.Instance.mapList;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (!UIManager.Instance.IsChatBoxOpen)
		{
			if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
			{
				GoOtherYard();
			}
			if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
			{
				GoOtherYard2();
			}
		}
	}

	public void LoadInit()
	{
		MapIds.Clear();
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapIds.Add(i);
		}
		GotoMap1.gameObject.SetActive(MapManager.Instance.mapList.Count > 1);
		if (MapManager.Instance.mapList.Count > 1)
		{
			GotoMap1.sprite = MapManager.Instance.mapList[1].GotoSprite;
		}
		GotoMap2.gameObject.SetActive(MapManager.Instance.mapList.Count > 2);
		if (MapManager.Instance.mapList.Count > 2)
		{
			GotoMap2.sprite = MapManager.Instance.mapList[2].GotoSprite;
		}
	}

	private void ResetAll()
	{
		MapIds.Sort();
		if (MapManager.Instance.mapList.Count > 1)
		{
			GotoMap1.sprite = mapList[1].GotoSprite;
		}
		if (MapManager.Instance.mapList.Count > 2)
		{
			GotoMap2.sprite = mapList[2].GotoSprite;
		}
	}

	public void GoOtherYard()
	{
		if (MapManager.Instance.mapList.Count < 2)
		{
			return;
		}
		if (MapIds[0] == 2)
		{
			ResetAll();
		}
		if (CameraControl.Instance.GoOtherYard(MapIds[1]))
		{
			StatsManager.Instance.AddStatsNum(StatsEnum.ChangeMapNum);
			AudioManager.Instance.ChangeMapReset();
			if (UIManager.Instance.SetPanel.isOpenMapFade)
			{
				FadeEffPanel.Instance.FadeIn();
			}
			int value = MapIds[0];
			MapIds[0] = MapIds[1];
			MapIds[1] = value;
			GotoMap1.sprite = mapList[MapIds[1]].GotoSprite;
			if (GameManager.Instance.isOnline)
			{
				BattlePlayerList.Instance.UpdateMapSprite(GameManager.Instance.LocalPlayerSave.playerName, mapList[MapIds[0]].transform.position);
			}
			if (GameManager.Instance.isClient)
			{
				PlayerMap playerMap = new PlayerMap();
				playerMap.Pos = mapList[MapIds[0]].transform.position;
				SocketClient.Instance.ChangeMap(playerMap);
			}
			if (GameManager.Instance.isServer)
			{
				PlayerMap playerMap2 = new PlayerMap();
				playerMap2.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
				playerMap2.Pos = mapList[MapIds[0]].transform.position;
				SocketServer.Instance.ChangeMap(playerMap2);
			}
		}
	}

	public void GoOtherYard2()
	{
		if (MapManager.Instance.mapList.Count < 3)
		{
			return;
		}
		if (MapIds[0] == 1)
		{
			ResetAll();
		}
		if (CameraControl.Instance.GoOtherYard(MapIds[1]))
		{
			StatsManager.Instance.AddStatsNum(StatsEnum.ChangeMapNum);
			AudioManager.Instance.ChangeMapReset();
			if (UIManager.Instance.SetPanel.isOpenMapFade)
			{
				FadeEffPanel.Instance.FadeIn();
			}
			int value = MapIds[0];
			MapIds[0] = MapIds[2];
			MapIds[2] = value;
			CameraControl.Instance.GoOtherYard(MapIds[0]);
			GotoMap2.sprite = mapList[MapIds[2]].GotoSprite;
			if (GameManager.Instance.isOnline)
			{
				BattlePlayerList.Instance.UpdateMapSprite(GameManager.Instance.LocalPlayerSave.playerName, mapList[MapIds[0]].transform.position);
			}
			if (GameManager.Instance.isClient)
			{
				PlayerMap playerMap = new PlayerMap();
				playerMap.Pos = mapList[MapIds[0]].transform.position;
				SocketClient.Instance.ChangeMap(playerMap);
			}
			if (GameManager.Instance.isServer)
			{
				PlayerMap playerMap2 = new PlayerMap();
				playerMap2.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
				playerMap2.Pos = mapList[MapIds[0]].transform.position;
				SocketServer.Instance.ChangeMap(playerMap2);
			}
		}
	}
}
