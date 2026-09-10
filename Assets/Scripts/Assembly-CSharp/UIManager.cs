using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public Transform UIBackground;

	public LogPanel LogPanel;

	public ConfirmPanel ConfirmPanel;

	public Transform BattleUI;

	private LVStartEF LVStartEF;

	public SetPanel SetPanel;

	public OverPanel OverPanel;

	public Transform LastStandBtn;

	public Transform HostGame;

	public InputField IpInput;

	public InputField PortInput;

	public Transform HostPassword;

	public InputField HostPasswordInput;

	public Transform JoinGame;

	public InputField JoinIpInput;

	public InputField JoinPasswordInput;

	private bool isChatBoxOpen;

	public ChatInput chatInput;

	public Transform ChatBox;

	public Transform OutChatBox;

	public Transform OpenChatButton;

	public Transform QuickChatGroup;

	public bool IsChatBoxOpen
	{
		get
		{
			return isChatBoxOpen;
		}
		set
		{
			isChatBoxOpen = value;
			if (isChatBoxOpen)
			{
				chatInput.InputField.ActivateInputField();
				ChatBox.localScale = new Vector3(1f, 1f, 1f);
				OutChatBox.localScale = new Vector3(0f, 0f, 0f);
			}
			else
			{
				chatInput.ClearInput();
				ChatBox.localScale = new Vector3(0f, 0f, 0f);
				OutChatBox.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	private void Awake()
	{
		Instance = this;
		LVStartEF = BattleUI.Find("LVEF").GetComponent<LVStartEF>();
		OverPanel = BattleUI.Find("OverPanel").GetComponent<OverPanel>();
		SetPanel = base.transform.Find("SetPanel").GetComponent<SetPanel>();
	}

	private void Start()
	{
		if (GameManager.Instance.isAndroid)
		{
			OpenChatButton.gameObject.SetActive(value: true);
		}
		else
		{
			OpenChatButton.gameObject.SetActive(value: false);
		}
		LastStandBtn.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (SetPanel.isOpen || isChatBoxOpen)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (SetPanel.isOpen)
				{
					SetPanel.CloseSetPanel();
				}
				else if (isChatBoxOpen)
				{
					IsChatBoxOpen = false;
				}
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape) && LVManager.Instance.InGame && !SetPanel.isOpen)
		{
			ShowBattleSetPanel();
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			IsChatBoxOpen = true;
		}
		if (Input.GetKeyDown(KeyCode.Slash))
		{
			IsChatBoxOpen = true;
			chatInput.SlashOpen();
		}
	}

	public void SetChooserType(SeedBankType type)
	{
		switch (type)
		{
		case SeedBankType.SunBank:
			SeedChooser.Instance.transform.localScale = Vector3.one;
			ZombieChooser.Instance.transform.localScale = Vector3.zero;
			SeedChooser.Instance.ChangeChooserBtn.localScale = Vector3.zero;
			break;
		case SeedBankType.MoonBank:
			SeedChooser.Instance.transform.localScale = Vector3.zero;
			ZombieChooser.Instance.transform.localScale = Vector3.one;
			ZombieChooser.Instance.ChangeChooserBtn.localScale = Vector3.zero;
			break;
		case SeedBankType.SunAndMoonBank:
			SeedChooser.Instance.transform.localScale = Vector3.one;
			ZombieChooser.Instance.transform.localScale = Vector3.zero;
			SeedChooser.Instance.ChangeChooserBtn.localScale = Vector3.one;
			ZombieChooser.Instance.ChangeChooserBtn.localScale = Vector3.one;
			break;
		}
	}

	public void ChangeChooser()
	{
		if (SeedChooser.Instance.transform.localScale.x == 0f)
		{
			SeedChooser.Instance.transform.localScale = Vector3.one;
			ZombieChooser.Instance.transform.localScale = Vector3.zero;
		}
		else
		{
			SeedChooser.Instance.transform.localScale = Vector3.zero;
			ZombieChooser.Instance.transform.localScale = Vector3.one;
		}
	}

	public void OpenChatBtn()
	{
		if (IsChatBoxOpen)
		{
			chatInput.SendContent();
		}
		else
		{
			IsChatBoxOpen = true;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
	}

	public void ShowLVStartEF()
	{
		LVStartEF.Show();
	}

	public void StopLVStartEF()
	{
		LVStartEF.StopAll();
	}

	public void ShowBigWaveEF()
	{
		LVStartEF.ShowBigWave();
	}

	public void ShowFinalWaveEF()
	{
		LVStartEF.ShowFinalWave();
	}

	public void ShowSetPanel()
	{
		SetPanel.ShowPanel(isShow: true, isBattle: false);
	}

	public void ShowBattleSetPanel()
	{
		SetPanel.ShowPanel(isShow: true, isBattle: true);
	}

	public void ConfirmOpenServer()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		if (IPAddress.TryParse(IpInput.text, out var address) && int.TryParse(PortInput.text, out var result))
		{
			if (result < 1025 || result > 65535)
			{
				LogPanel.DisplayLog("请输入正确的端口", () =>
				{
					HostGame.gameObject.SetActive(value: true);
				});
			}
			else
			{
				SocketServer.Instance.StartServer(address, result);
				CloseHostGame();
			}
		}
		else
		{
			LogPanel.DisplayLog("请输入正确的端口", () =>
			{
				HostGame.gameObject.SetActive(value: true);
			});
		}
	}

	public void ConfirmJoinGame()
	{
		if (GameManager.Instance.isOnline)
		{
			return;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		string[] array = JoinIpInput.text.Split(":");
		int result;
		if (array.Length < 2)
		{
			LogPanel.DisplayLog("请输入正确的地址", () =>
			{
				JoinGame.gameObject.SetActive(value: true);
			});
		}
		else if (int.TryParse(array[1], out result))
		{
			LogPanel.DisplayLog("连接中...", () =>
			{
				JoinGame.gameObject.SetActive(value: true);
			});
			LogPanel.ButtonText.text = "取消";
			LogPanel.CancelConfirm();
			SocketClient.Instance.JoinGame(Dns.GetHostAddresses(array[0])[0], result, JoinPasswordInput.text);
		}
		else
		{
			LogPanel.DisplayLog("请输入正确的地址", () =>
			{
				JoinGame.gameObject.SetActive(value: true);
			});
		}
	}

	public void ReJoinGame()
	{
		if (!GameManager.Instance.isOnline)
		{
			string[] array = JoinIpInput.text.Split(":");
			if (array.Length >= 2)
			{
				int.TryParse(array[1], out var result);
				LogPanel.DisplayLog("连接中...", null);
				LogPanel.CancelConfirm();
				SocketClient.Instance.JoinGame(Dns.GetHostAddresses(array[0])[0], result, JoinPasswordInput.text);
			}
		}
	}

	public void ConnectSuccess()
	{
		LogPanel.gameObject.SetActive(value: false);
		CloseJoinGame();
	}

	public void CloseHostGame()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		IpInput.text = "127.0.0.1";
		PortInput.text = "45678";
		HostGame.gameObject.SetActive(value: false);
	}

	public void CloseJoinGame()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		JoinGame.gameObject.SetActive(value: false);
	}

	public void ConfirmPassword()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		HostPassword.gameObject.SetActive(value: false);
	}

	public void OpenAndFocusUI(bool isBlack = true)
	{
		if (isBlack)
		{
			UIBackground.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
		}
		else
		{
			UIBackground.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
		}
		UIBackground.gameObject.SetActive(value: true);
	}

	public void CloseUI()
	{
		UIBackground.gameObject.SetActive(value: false);
		UIBackground.SetSiblingIndex(0);
	}

	public void OpenBattleUI()
	{
		Shovel.Instance.LvStart();
		Glove.Instance.LvStart();
		CreatePanel.Instance.BattleUIOpen();
		QuickChatGroup.gameObject.SetActive(SetPanel.OpenQuickChat);
		BattleUI.localScale = Vector3.one;
	}

	public void StartLastStand()
	{
		LVManager.Instance.StartLastStand();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
	}
}
