using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SpectatorList : MonoBehaviour
{
	public static SpectatorList Instance;

	public TextMesh JoinBtn;

	public List<TextMesh> NameList = new List<TextMesh>();

	public List<string> SpectsList = new List<string>();

	public bool LocalIsSpectator => SpectsList.Contains(GameManager.Instance.LocalPlayerSave.playerName);

	public int SpectatorNum => SpectsList.Count;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		UpdateNameUI();
	}

	private void OnMouseEnter()
	{
		if (GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
		{
			JoinBtn.color = new Color32(0, 128, byte.MaxValue, byte.MaxValue);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		JoinBtn.color = new Color32(0, 188, byte.MaxValue, byte.MaxValue);
	}

	private void OnMouseDown()
	{
		if (GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
			if (GameManager.Instance.isClient)
			{
				JoinSpecApply joinSpecApply = new JoinSpecApply();
				joinSpecApply.isJoin = !SpectsList.Contains(GameManager.Instance.LocalPlayerSave.playerName);
				SocketClient.Instance.ApplyJoinSpect(joinSpecApply);
			}
			else if (SpectsList.Contains(GameManager.Instance.LocalPlayerSave.playerName))
			{
				SpectsList.Remove(GameManager.Instance.LocalPlayerSave.playerName);
				UpdateNameUI();
			}
			else if (SpectsList.Count < SocketServer.Instance.noHostPlayerNum)
			{
				PvPSelector.Instance.ClearQuitPlayer(GameManager.Instance.LocalPlayerSave.playerName);
				SpectsList.Add(GameManager.Instance.LocalPlayerSave.playerName);
				UpdateNameUI();
			}
		}
	}

	public List<string> GetNoSpectatorPlayerList()
	{
		if (GameManager.Instance.isServer)
		{
			List<string> allPlayerNameList = SocketServer.Instance.GetAllPlayerNameList();
			for (int i = 0; i < SpectsList.Count; i++)
			{
				allPlayerNameList.Remove(SpectsList[i]);
			}
			return allPlayerNameList;
		}
		return null;
	}

	public bool IsSpectator(string Name)
	{
		return SpectsList.Contains(Name);
	}

    private void UpdateNameUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("SpectatorList: GameManager.Instance es NULL");
            return;
        }

        if (GameManager.Instance.LocalPlayerSave == null)
        {
            Debug.LogError("SpectatorList: LocalPlayerSave es NULL");
            return;
        }

        for (int i = 0; i < NameList.Count; i++)
        {
            if (NameList[i] == null)
            {
                Debug.LogError("SpectatorList: NameList[" + i + "] es NULL");
                continue;
            }

            if (SpectsList.Count > i)
            {
                NameList[i].text = SpectsList[i];
            }
            else
            {
                NameList[i].text = "";
            }
        }

        if (JoinBtn == null)
        {
            Debug.LogError("SpectatorList: JoinBtn es NULL");
            return;
        }

        if (SpectsList.Contains(GameManager.Instance.LocalPlayerSave.playerName))
        {
            JoinBtn.text = "退出";
        }
        else
        {
            JoinBtn.text = "加入";
        }

        ServerSynSpectList();
    }

    public void ServerSynSpectList()
	{
		if (GameManager.Instance.isServer)
		{
			SpectList spectList = new SpectList();
			spectList.names = SpectsList;
			SocketServer.Instance.SynSpectList(spectList);
		}
	}

	public void ClientJoinSpect(string player, bool isJoin)
	{
		if (isJoin)
		{
			PvPSelector.Instance.ClearQuitPlayer(player);
			if (!SpectsList.Contains(player) && SpectsList.Count < SocketServer.Instance.noHostPlayerNum)
			{
				SpectsList.Add(player);
				UpdateNameUI();
			}
		}
		else if (SpectsList.Remove(player))
		{
			UpdateNameUI();
		}
	}

	public void ClearPlayer(string player)
	{
		if (SpectsList.Remove(player))
		{
			SpectList spectList = new SpectList();
			spectList.names = SpectsList;
			SocketServer.Instance.SynSpectList(spectList);
			UpdateNameUI();
		}
		if (GameManager.Instance.isServer && SocketServer.Instance.noHostPlayerNum + 1 <= SpectsList.Count && SpectsList.Remove(GameManager.Instance.LocalPlayerSave.playerName))
		{
			UpdateNameUI();
		}
	}

	public void ClientSynList(List<string> list)
	{
		SpectsList = list;
		UpdateNameUI();
	}
}
