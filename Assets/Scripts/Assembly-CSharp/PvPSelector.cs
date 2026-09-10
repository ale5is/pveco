using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.UI;

public class PvPSelector : MonoBehaviour
{
	public static PvPSelector Instance;

	public PvPMode CurrMode;

	public bool SkySun;

	public bool SkyMoon;

	public Image SkySunImg;

	public Image SkyMoonImg;

	public Image LeftImg;

	public Image RightImg;

	public Sprite Plant;

	public Sprite Zombie;

	public Sprite PlantAndZombie;

	public Transform StartPage;

	public Transform SettingPage;

	public Transform StartBtnTr;

	public Transform SettingBtnTr;

	public InputField TimeInput;

	public InputField SunNumInput;

	public List<Text> ModeTexts = new List<Text>();

	public List<Text> RedNameTexts = new List<Text>();

	public List<Text> BlueNameTexts = new List<Text>();

	private int RedFailNum;

	private int BlueFailNum;

	public List<string> RedTeamNames = new List<string>();

	public List<string> BlueTeamNames = new List<string>();

	public int CardNum = 10;

	public Text CardNumText;

	public bool LocalIsRedTeam => RedTeamNames.Contains(GameManager.Instance.LocalPlayerSave.playerName);

	public bool LocalIsBlueTeam => BlueTeamNames.Contains(GameManager.Instance.LocalPlayerSave.playerName);

	public string StartTime => TimeInput.text;

	public string StartSunNum => SunNumInput.text;

	private void Awake()
	{
		Instance = this;
	}

	public void AddCard()
	{
		if (CardNum < 15)
		{
			CardNum++;
			CardNumText.text = CardNum.ToString();
		}
	}

	public void DownCard()
	{
		if (CardNum > 6)
		{
			CardNum--;
			CardNumText.text = CardNum.ToString();
		}
	}

	private void Start()
	{
		for (int i = 0; i < RedNameTexts.Count; i++)
		{
			RedNameTexts[i].text = "";
		}
		for (int j = 0; j < BlueNameTexts.Count; j++)
		{
			BlueNameTexts[j].text = "";
		}
		ResetAllModeText();
		CurrMode = PvPMode.PlantVsZomibe;
		LeftImg.sprite = Plant;
		RightImg.sprite = Zombie;
		ModeTexts[1].color = new Color32(100, byte.MaxValue, 0, byte.MaxValue);
	}

	public void ResetPvPInfo()
	{
		RedFailNum = 0;
		BlueFailNum = 0;
		RedTeamNames.Clear();
		BlueTeamNames.Clear();
		UpdateNameText();
	}

	public void ClearQuitPlayer(string player)
	{
		if (!LVManager.Instance.InGame)
		{
			if (RedTeamNames.Remove(player))
			{
				UpdateNameText();
			}
			if (BlueTeamNames.Remove(player))
			{
				UpdateNameText();
			}
		}
	}

	public SeedBankType GetSeedBankType()
	{
		SeedBankType result = SeedBankType.SunBank;
		switch (CurrMode)
		{
		case PvPMode.PlantVsPlant:
			result = SeedBankType.SunBank;
			break;
		case PvPMode.PlantVsZomibe:
			result = ((!LocalIsRedTeam) ? SeedBankType.MoonBank : SeedBankType.SunBank);
			break;
		case PvPMode.ZombieVsZombie:
			result = SeedBankType.MoonBank;
			break;
		case PvPMode.PlZomVsPlZom:
			result = SeedBankType.SunAndMoonBank;
			break;
		}
		return result;
	}

	public bool IsSameTeam(string PlayerName)
	{
		if (GameManager.Instance.isServer && SpectatorList.Instance.LocalIsSpectator && RedTeamNames.Contains(PlayerName))
		{
			return true;
		}
		if (GameManager.Instance.isClient && SpectatorList.Instance.LocalIsSpectator)
		{
			if (PlayerName == GameManager.Instance.HostName && SpectatorList.Instance.IsSpectator(PlayerName))
			{
				return true;
			}
			if (BlueTeamNames.Contains(GameManager.Instance.HostName))
			{
				if (BlueTeamNames.Contains(PlayerName))
				{
					return true;
				}
			}
			else if (RedTeamNames.Contains(PlayerName))
			{
				return true;
			}
		}
		if (PlayerName == GameManager.Instance.HostName && SpectatorList.Instance.IsSpectator(PlayerName))
		{
			if (LocalIsRedTeam)
			{
				return true;
			}
			return false;
		}
		if (LocalIsRedTeam)
		{
			if (RedTeamNames.Contains(PlayerName))
			{
				return true;
			}
		}
		else if (LocalIsBlueTeam && BlueTeamNames.Contains(PlayerName))
		{
			return true;
		}
		return false;
	}

	public bool IsSameTeam(string PlayerName, string Name2)
	{
		if (RedTeamNames.Contains(PlayerName) && RedTeamNames.Contains(Name2))
		{
			return true;
		}
		if (BlueTeamNames.Contains(PlayerName) && BlueTeamNames.Contains(Name2))
		{
			return true;
		}
		return false;
	}

	private void TapAudio()
	{
		if (base.transform.localScale.x != 0f)
		{
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
		}
	}

	public void PvPBtn()
	{
		TapAudio();
		ResetAllModeText();
		SetCurrMode(PvPMode.PlantVsPlant);
		LeftImg.sprite = Plant;
		RightImg.sprite = Plant;
		ModeTexts[0].color = new Color32(100, byte.MaxValue, 0, byte.MaxValue);
	}

	public void PvZBtn()
	{
		TapAudio();
		ResetAllModeText();
		SetCurrMode(PvPMode.PlantVsZomibe);
		LeftImg.sprite = Plant;
		RightImg.sprite = Zombie;
		ModeTexts[1].color = new Color32(100, byte.MaxValue, 0, byte.MaxValue);
	}

	public void ZvZBtn()
	{
		TapAudio();
		ResetAllModeText();
		SetCurrMode(PvPMode.ZombieVsZombie);
		LeftImg.sprite = Zombie;
		RightImg.sprite = Zombie;
		ModeTexts[2].color = new Color32(100, byte.MaxValue, 0, byte.MaxValue);
	}

	public void PZvPZBtn()
	{
		TapAudio();
		ResetAllModeText();
		SetCurrMode(PvPMode.PlZomVsPlZom);
		LeftImg.sprite = PlantAndZombie;
		RightImg.sprite = PlantAndZombie;
		ModeTexts[3].color = new Color32(100, byte.MaxValue, 0, byte.MaxValue);
	}

	public void TargetDead(string PlacePlayer)
	{
		if (RedTeamNames.Contains(PlacePlayer))
		{
			RedFailNum++;
			if (RedFailNum == 3)
			{
				LVManager.Instance.PvPGameOver(default, isRedFail: true);
			}
		}
		else if (BlueTeamNames.Contains(PlacePlayer))
		{
			BlueFailNum++;
			if (BlueFailNum == 3)
			{
				LVManager.Instance.PvPGameOver(default, isRedFail: false);
			}
		}
	}

	private void SetCurrMode(PvPMode mode)
	{
		if (CurrMode != mode)
		{
			CurrMode = mode;
			if (GameManager.Instance.isServer)
			{
				PvPModeSyn pvPModeSyn = new PvPModeSyn();
				pvPModeSyn.Mode = CurrMode;
				SocketServer.Instance.SynPvPMode(pvPModeSyn);
			}
		}
	}

	private void ResetAllModeText()
	{
		for (int i = 0; i < ModeTexts.Count; i++)
		{
			ModeTexts[i].color = new Color32(byte.MaxValue, 190, 0, byte.MaxValue);
		}
	}

	public void ServerSynTeam()
	{
		if (GameManager.Instance.isServer)
		{
			PvPTeamList pvPTeamList = new PvPTeamList();
			pvPTeamList.red = RedTeamNames;
			pvPTeamList.blue = BlueTeamNames;
			SocketServer.Instance.SynTeamList(pvPTeamList);
		}
	}

	public void JoinRedBtn()
	{
		if (GameManager.Instance.isClient)
		{
			JoinTeamApply joinTeamApply = new JoinTeamApply();
			joinTeamApply.isRed = true;
			SocketClient.Instance.ApplyJoinTeam(joinTeamApply);
		}
		if (GameManager.Instance.isServer)
		{
			JoinRed(GameManager.Instance.LocalPlayerSave.playerName);
		}
	}

	public void JoinRed(string playerName)
	{
		SpectatorList.Instance.ClearPlayer(playerName);
		if (!RedTeamNames.Contains(playerName) && RedTeamNames.Count <= 2)
		{
			BlueTeamNames.Remove(playerName);
			RedTeamNames.Add(playerName);
			UpdateNameText();
		}
	}

	public void JoinBlueBtn()
	{
		if (GameManager.Instance.isClient)
		{
			JoinTeamApply joinTeamApply = new JoinTeamApply();
			joinTeamApply.isRed = false;
			SocketClient.Instance.ApplyJoinTeam(joinTeamApply);
		}
		if (GameManager.Instance.isServer)
		{
			JoinBlue(GameManager.Instance.LocalPlayerSave.playerName);
		}
	}

	public void JoinBlue(string playerName)
	{
		SpectatorList.Instance.ClearPlayer(playerName);
		if (!BlueTeamNames.Contains(playerName) && BlueTeamNames.Count <= 2)
		{
			RedTeamNames.Remove(playerName);
			BlueTeamNames.Add(playerName);
			UpdateNameText();
		}
	}

	public void ClientSynTeam(PvPTeamList list)
	{
		RedTeamNames = list.red;
		BlueTeamNames = list.blue;
		UpdateNameText();
	}

	public void ClientSynMode(PvPModeSyn syn)
	{
		switch (syn.Mode)
		{
		case PvPMode.PlantVsPlant:
			PvPBtn();
			break;
		case PvPMode.PlantVsZomibe:
			PvZBtn();
			break;
		case PvPMode.ZombieVsZombie:
			ZvZBtn();
			break;
		case PvPMode.PlZomVsPlZom:
			PZvPZBtn();
			break;
		}
	}

	private void UpdateNameText()
	{
		for (int i = 0; i < RedNameTexts.Count; i++)
		{
			if (RedTeamNames.Count > i)
			{
				RedNameTexts[i].text = RedTeamNames[i];
			}
			else
			{
				RedNameTexts[i].text = "";
			}
		}
		for (int j = 0; j < BlueNameTexts.Count; j++)
		{
			if (BlueTeamNames.Count > j)
			{
				BlueNameTexts[j].text = BlueTeamNames[j];
			}
			else
			{
				BlueNameTexts[j].text = "";
			}
		}
		ServerSynTeam();
	}

	public void StartBtn()
	{
		if (GameManager.Instance.isServer)
		{
			if (BlueTeamNames.Count + RedTeamNames.Count + SpectatorList.Instance.SpectatorNum != SocketServer.Instance.noHostPlayerNum + 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
			}
			else if (BlueTeamNames.Count > 0 && RedTeamNames.Count > 0)
			{
				TapAudio();
				LVManager.Instance.StartGame(null, 1);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
			}
		}
	}

	public void SettingBtn()
	{
		TapAudio();
		StartPage.localScale = Vector3.zero;
		SettingPage.localScale = Vector3.one;
	}

	public void BackBtn()
	{
		TapAudio();
		if (SettingPage.localScale.x > 0f)
		{
			StartPage.localScale = Vector3.one;
			SettingPage.localScale = Vector3.zero;
		}
		else
		{
			CloseSelector();
		}
	}

	public void CloseSelector()
	{
		base.transform.localScale = Vector3.zero;
	}

	public void SunChangeBtn()
	{
		SkySun = !SkySun;
		if (SkySun)
		{
			SkySunImg.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			SkySunImg.sprite = NormalSprite.Instance.CheckBox;
		}
	}

	public void MoonChangeBtn()
	{
		SkyMoon = !SkyMoon;
		if (SkyMoon)
		{
			SkyMoonImg.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			SkyMoonImg.sprite = NormalSprite.Instance.CheckBox;
		}
	}

	public void OpenAndInit()
	{
		base.transform.localScale = Vector3.one;
		base.gameObject.SetActive(value: true);
		if (GameManager.Instance.isClient)
		{
			StartBtnTr.localScale = Vector3.zero;
			SettingBtnTr.localScale = Vector3.zero;
			for (int i = 0; i < ModeTexts.Count; i++)
			{
				ModeTexts[i].transform.parent.GetComponent<Button>().enabled = false;
			}
		}
		else
		{
			StartBtnTr.localScale = Vector3.one;
			SettingBtnTr.localScale = Vector3.one;
			for (int j = 0; j < ModeTexts.Count; j++)
			{
				ModeTexts[j].transform.parent.GetComponent<Button>().enabled = true;
			}
		}
	}
}
