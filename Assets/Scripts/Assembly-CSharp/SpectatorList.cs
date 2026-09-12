using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SpectatorList : MonoBehaviour
{
    public static SpectatorList Instance;

    public TextMesh JoinBtn;

    public List<TextMesh> NameList = new List<TextMesh>();

    public List<string> SpectsList = new List<string>();

    public bool LocalIsSpectator
    {
        get
        {
            if (GameManager.Instance == null ||
                GameManager.Instance.LocalPlayerSave == null)
            {
                return false;
            }

            return SpectsList.Contains(GameManager.Instance.LocalPlayerSave.playerName);
        }
    }

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
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return;
        }

        if (GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
        {
            if (JoinBtn != null)
            {
                JoinBtn.color = new Color32(0, 128, byte.MaxValue, byte.MaxValue);
            }

            if (AudioManager.Instance != null &&
                GameManager.Instance.AudioConf != null)
            {
                AudioManager.Instance.PlayEFAudio(
                    GameManager.Instance.AudioConf.Bleep,
                    transform.position,
                    true);
            }
        }
    }

    private void OnMouseExit()
    {
        if (JoinBtn != null)
        {
            JoinBtn.color = new Color32(0, 188, byte.MaxValue, byte.MaxValue);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return;
        }

        if (!GameManager.Instance.isOnline ||
            MyTool.IsPointerOverGameObject())
        {
            return;
        }

        if (AudioManager.Instance != null &&
            GameManager.Instance.AudioConf != null)
        {
            if (Random.Range(0, 2) == 1)
            {
                AudioManager.Instance.PlayEFAudio(
                    GameManager.Instance.AudioConf.Tap,
                    transform.position,
                    true);
            }
            else
            {
                AudioManager.Instance.PlayEFAudio(
                    GameManager.Instance.AudioConf.Tap2,
                    transform.position,
                    true);
            }
        }

        string playerName = GameManager.Instance.LocalPlayerSave.playerName;

        if (GameManager.Instance.isClient)
        {
            if (SocketClient.Instance == null)
            {
                return;
            }

            JoinSpecApply joinSpecApply = new JoinSpecApply();
            joinSpecApply.isJoin = !SpectsList.Contains(playerName);

            SocketClient.Instance.ApplyJoinSpect(joinSpecApply);
        }
        else if (SpectsList.Contains(playerName))
        {
            SpectsList.Remove(playerName);
            UpdateNameUI();
        }
        else if (SocketServer.Instance != null &&
                 SpectsList.Count < SocketServer.Instance.noHostPlayerNum)
        {
            if (PvPSelector.Instance != null)
            {
                PvPSelector.Instance.ClearQuitPlayer(playerName);
            }

            SpectsList.Add(playerName);
            UpdateNameUI();
        }
    }

    public List<string> GetNoSpectatorPlayerList()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return null;
        }

        if (GameManager.Instance.isServer &&
            SocketServer.Instance != null)
        {
            List<string> allPlayerNameList =
                SocketServer.Instance.GetAllPlayerNameList();

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
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return;
        }

        for (int i = 0; i < NameList.Count; i++)
        {
            if (NameList[i] == null)
            {
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

        if (JoinBtn != null)
        {
            if (SpectsList.Contains(
                GameManager.Instance.LocalPlayerSave.playerName))
            {
                JoinBtn.text = "退出";
            }
            else
            {
                JoinBtn.text = "加入";
            }
        }

        ServerSynSpectList();
    }

    public void ServerSynSpectList()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return;
        }

        if (GameManager.Instance.isServer &&
            SocketServer.Instance != null)
        {
            SpectList spectList = new SpectList();
            spectList.names = SpectsList;
            SocketServer.Instance.SynSpectList(spectList);
        }
    }

    public void ClientJoinSpect(string player, bool isJoin)
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return;
        }

        if (isJoin)
        {
            if (PvPSelector.Instance != null)
            {
                PvPSelector.Instance.ClearQuitPlayer(player);
            }

            if (!SpectsList.Contains(player) &&
                SocketServer.Instance != null &&
                SpectsList.Count < SocketServer.Instance.noHostPlayerNum)
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
        if (GameManager.Instance == null ||
            GameManager.Instance.LocalPlayerSave == null)
        {
            return;
        }

        if (SpectsList.Remove(player))
        {
            if (GameManager.Instance.isServer &&
                SocketServer.Instance != null)
            {
                SpectList spectList = new SpectList();
                spectList.names = SpectsList;
                SocketServer.Instance.SynSpectList(spectList);
            }

            UpdateNameUI();
        }

        if (GameManager.Instance.isServer &&
            SocketServer.Instance != null &&
            SocketServer.Instance.noHostPlayerNum + 1 <= SpectsList.Count &&
            SpectsList.Remove(GameManager.Instance.LocalPlayerSave.playerName))
        {
            UpdateNameUI();
        }
    }

    public void ClientSynList(List<string> list)
    {
        SpectsList = list ?? new List<string>();
        UpdateNameUI();
    }
}