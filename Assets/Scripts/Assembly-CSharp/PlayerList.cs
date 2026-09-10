using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
	public static PlayerList Instance;

	public GameObject QuitButton;

	public GameObject PasswordButton;

	public TextMesh HostName;

	public TextMesh Name1;

	public TextMesh Name2;

	public TextMesh Name3;

	public TextMesh Tip;

	public int PlayerNum;

	private void Awake()
	{
		Instance = this;
	}

	public void UpdatePlayerList(PlayerInfo Host, List<PlayerInfo> players)
	{
		PlayerNum = 1;
		if (players != null)
		{
			PlayerNum += players.Count;
		}
		if (Host != null && Host.Name == GameManager.Instance.LocalPlayerSave.playerName)
		{
			Tip.text = "多人游戏已开启";
		}
		else
		{
			Tip.text = "已加入多人游戏";
		}
		if (GameManager.Instance.isOnline)
		{
			QuitButton.SetActive(value: true);
		}
		else
		{
			QuitButton.SetActive(value: false);
			PasswordButton.SetActive(value: false);
			Tip.text = "多人游戏未开启";
		}
		if (GameManager.Instance.isServer)
		{
			PasswordButton.SetActive(value: true);
		}
		if (Host != null)
		{
			GameManager.Instance.HostName = Host.Name;
		}
		if (Host != null)
		{
			HostName.text = Host.Name;
		}
		else
		{
			HostName.text = "";
		}
		if (players != null)
		{
			if (players.Count > 0)
			{
				Name1.text = players[0].Name;
			}
			else
			{
				Name1.text = "";
			}
			if (players.Count > 1)
			{
				Name2.text = players[1].Name;
			}
			else
			{
				Name2.text = "";
			}
			if (players.Count > 2)
			{
				Name3.text = players[2].Name;
			}
			else
			{
				Name3.text = "";
			}
		}
	}
}
