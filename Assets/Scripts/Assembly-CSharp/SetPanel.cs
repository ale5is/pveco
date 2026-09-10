using System.Collections;
using System.Collections.Generic;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : MonoBehaviour
{
	public Slider MusicSlider;

	public Slider SoundSlider;

	public Text ContinueText;

	public Text ContinueText2;

	public Button BackMenu;

	public Button Restart;

	public Button ClientQuit;

	public Button MoreOption;

	public Transform AllMoreOptionPage;

	public Transform MOMainPage;

	public Transform MOHardPage;

	public Transform MOGraphicsPage;

	public Transform MOQuickChatPage;

	public Transform MOStatisticsPage;

	public Transform GrapNoAndroid;

	public Transform FrameDisObj;

	public Button RainFogBt;

	public Button VsyncBt;

	public Text FrameText;

	public Text ResolutionText;

	public Button SunUpBt;

	public Button NormalSunBt;

	public Button CdDownBt;

	public Button LightningBt;

	public Button SlowSunBt;

	public InputField QuickChat1;

	public InputField QuickChat2;

	public InputField QuickChat3;

	public Button CardSlectorBt;

	public Button FrameDisplayBt;

	public Button OpenQChatBt;

	public Image FullScreen;

	public Image X2Speed;

	public bool isOpen;

	private bool isFullScreen;

	public int FrameType;

	public bool is1080P;

	public bool is2xSpeed;

	public bool isVsync;

	public bool isOpenMapFade;

	public bool isDisFrame;

	public bool OpenQuickChat;

	private bool QuickChatLimit;

	private void Start()
	{
		if (GameManager.Instance.isAndroid)
		{
			GrapNoAndroid.gameObject.SetActive(value: false);
		}
	}

	public void ShowPanel(bool isShow, bool isBattle)
	{
		isOpen = isShow;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		if (isShow)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			GameManager.Instance.SaveSetting();
			base.transform.localScale = new Vector3(0f, 0f, 0f);
			AllMoreOptionPage.localScale = new Vector3(0f, 0f, 0f);
		}
		if (isShow & isBattle)
		{
			if (!GameManager.Instance.isOnline)
			{
				Time.timeScale = 0f;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Pause, base.transform.position, isAll: true);
				AudioManager.Instance.StopBgAudio();
			}
		}
		else if (!GameManager.Instance.isOnline)
		{
			Time.timeScale = 1f;
			if (is2xSpeed)
			{
				Time.timeScale = 2f;
			}
			AudioManager.Instance.PlayBgAudio(BgmType.Nope);
		}
		if (isBattle)
		{
			X2Speed.gameObject.SetActive(!GameManager.Instance.isOnline);
			if (GameManager.Instance.isClient)
			{
				ClientQuit.transform.localScale = Vector3.one;
				BackMenu.transform.localScale = Vector3.zero;
				Restart.transform.localScale = Vector3.zero;
			}
			else
			{
				ClientQuit.transform.localScale = Vector3.zero;
				BackMenu.transform.localScale = Vector3.one;
				if (LV.Instance.CurrLVType != LVType.PvP)
				{
					Restart.transform.localScale = Vector3.one;
				}
				else
				{
					Restart.transform.localScale = Vector3.zero;
				}
			}
			MoreOption.transform.localScale = Vector3.zero;
			ContinueText.text = "继续游戏";
			ContinueText2.text = "继续游戏";
		}
		else
		{
			X2Speed.gameObject.SetActive(value: false);
			MoreOption.transform.localScale = Vector3.one;
			BackMenu.transform.localScale = Vector3.zero;
			Restart.transform.localScale = Vector3.zero;
			ClientQuit.transform.localScale = Vector3.zero;
			ContinueText.text = "确定";
			ContinueText2.text = "确定";
		}
		base.gameObject.SetActive(value: true);
	}

	public void LvReset()
	{
		if (is2xSpeed)
		{
			Time.timeScale = 1f;
			is2xSpeed = false;
			X2Speed.sprite = NormalSprite.Instance.CheckBox;
		}
	}

	public void VolumeChange()
	{
		AudioManager.Instance.SetVolume(SoundSlider.value, MusicSlider.value);
	}

	public void SaveInit(SettingSave save)
	{
		MusicSlider.value = save.bgmVolume;
		SoundSlider.value = save.soundVolume;
		isFullScreen = save.isFullScreen;
		is1080P = save.isF1080P;
		isVsync = save.isVsync;
		FrameType = save.FrameType;
		isDisFrame = save.isDisFrame;
		SkyManager.Instance.isRainFog = save.isRainFog;
		SeedBank.Instance.CardSelector = save.isCardSelector;
		if (isFullScreen)
		{
			FullScreen.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		Application.targetFrameRate = SetFrameType(FrameType);
		if (isVsync)
		{
			VsyncBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		if (!GameManager.Instance.isAndroid)
		{
			if (is1080P)
			{
				Screen.SetResolution(1920, 1080, isFullScreen);
				ResolutionText.text = "1920*1080";
			}
			else
			{
				Screen.SetResolution(1280, 720, isFullScreen);
				ResolutionText.text = "1280*720";
			}
		}
		if (SkyManager.Instance.isRainFog)
		{
			RainFogBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		if (isDisFrame)
		{
			FrameDisplayBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		FrameDisObj.gameObject.SetActive(isDisFrame);
		if (SeedBank.Instance.CardSelector)
		{
			CardSlectorBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
	}

	private int SetFrameType(int FrameType)
	{
		int num = 60;
		switch (FrameType)
		{
		case 0:
			num = 60;
			break;
		case 1:
			num = 120;
			break;
		case 2:
			num = 160;
			break;
		case 3:
			num = 320;
			break;
		case 4:
			num = -1;
			break;
		case 5:
			num = 30;
			break;
		}
		FrameText.text = num.ToString();
		if (num == -1)
		{
			FrameText.text = "无限制";
		}
		return num;
	}

	public void RestartScene()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		UIManager.Instance.ConfirmPanel.InitEvent(() =>
		{
			if (!GameManager.Instance.isOnline)
			{
				Time.timeScale = 1f;
				AudioManager.Instance.StopBgAudio();
			}
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			LVManager.Instance.ReStartGame();
			CloseSetPanel();
		}, "重新开始本关卡？", "你确定要重新开始本关吗？", "", base.transform);
	}

	public void BackMainScene()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		string warn = "";
		if (LVManager.Instance.GameIsStart)
		{
			warn = "*当前版本无法保存当前进度*";
		}
		UIManager.Instance.ConfirmPanel.InitEvent(() =>
		{
			if (!GameManager.Instance.isOnline)
			{
				Time.timeScale = 1f;
				AudioManager.Instance.StopBgAudio();
			}
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			LVManager.Instance.QuitBattleGame();
			CloseSetPanel();
		}, "返回到主菜单？", "你确定要返回到主菜单吗？", warn, base.transform);
	}

	public void DoClientQuit()
	{
		if (GameManager.Instance.isClient)
		{
			SocketClient.Instance.CloseClient();
			CloseSetPanel();
		}
	}

	private void InitMoreOption()
	{
		MOMainPage.transform.localScale = Vector3.one;
		MOQuickChatPage.transform.localScale = Vector3.zero;
		MOHardPage.transform.localScale = Vector3.zero;
		MOGraphicsPage.transform.localScale = Vector3.zero;
		MOStatisticsPage.transform.localScale = Vector3.zero;
		StatsManager.Instance.CloseViwer();
	}

	public void QuickChat1Btn()
	{
		SendQuickChat(1);
	}

	public void QuickChat2Btn()
	{
		SendQuickChat(2);
	}

	public void QuickChat3Btn()
	{
		SendQuickChat(3);
	}

	public void SendQuickChat(int num)
	{
		if (!QuickChatLimit)
		{
			string text = "";
			if (num == 1)
			{
				text = QuickChat1.text;
			}
			if (num == 2)
			{
				text = QuickChat2.text;
			}
			if (num == 3)
			{
				text = QuickChat3.text;
			}
			if (text != "")
			{
				StartCoroutine(waitLimit());
				ChatInput.Instance.SendMessageToAll(text, needName: true);
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	private IEnumerator waitLimit()
	{
		QuickChatLimit = true;
		yield return new WaitForSeconds(1f);
		QuickChatLimit = false;
	}

	public void OpenMoreOption()
	{
		AllMoreOptionPage.transform.localScale = Vector3.one;
		InitMoreOption();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void MoreOptionBack()
	{
		if (MOMainPage.transform.localScale.x > 0f)
		{
			GameManager.Instance.SaveUserInfo();
			AllMoreOptionPage.transform.localScale = Vector3.zero;
		}
		else
		{
			InitMoreOption();
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void OpenQuickChatPage()
	{
		MOMainPage.transform.localScale = Vector3.zero;
		MOQuickChatPage.transform.localScale = Vector3.one;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void OpenStatisticsPage()
	{
		MOMainPage.transform.localScale = Vector3.zero;
		MOStatisticsPage.transform.localScale = Vector3.one;
		StatsManager.Instance.LoadStats();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void OpenHardPage()
	{
		MOMainPage.transform.localScale = Vector3.zero;
		MOHardPage.transform.localScale = Vector3.one;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void OpenGraphicsPage()
	{
		MOMainPage.transform.localScale = Vector3.zero;
		MOGraphicsPage.transform.localScale = Vector3.one;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void MoreOptionRead(List<bool> bools, bool openQuickChat, List<string> chats)
	{
		for (int i = 0; i < bools.Count; i++)
		{
			switch (i)
			{
			case 0:
				GobalLight.Instance.LightingNotDark = bools[0];
				break;
			case 1:
				SeedBank.Instance.IsCdDown = bools[1];
				break;
			case 2:
				PlayerManager.Instance.NormalSunUp = bools[2];
				break;
			case 3:
				PlayerManager.Instance.GetSunUp = bools[3];
				break;
			case 4:
				SkyManager.Instance.SlowSunAutoCollect = bools[4];
				break;
			}
		}
		if (SeedBank.Instance.IsCdDown)
		{
			CdDownBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			CdDownBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		if (GobalLight.Instance.LightingNotDark)
		{
			LightningBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			LightningBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		if (PlayerManager.Instance.NormalSunUp)
		{
			NormalSunBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			NormalSunBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		if (PlayerManager.Instance.GetSunUp)
		{
			SunUpBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			SunUpBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		if (SkyManager.Instance.SlowSunAutoCollect)
		{
			SlowSunBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			SlowSunBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		OpenQuickChat = openQuickChat;
		if (OpenQuickChat)
		{
			OpenQChatBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		if (chats.Count > 0)
		{
			QuickChat1.text = chats[0];
		}
		if (chats.Count > 1)
		{
			QuickChat2.text = chats[1];
		}
		if (chats.Count > 2)
		{
			QuickChat3.text = chats[2];
		}
	}

	public void CDDownBtn()
	{
		SeedBank.Instance.IsCdDown = !SeedBank.Instance.IsCdDown;
		if (SeedBank.Instance.IsCdDown)
		{
			CdDownBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			CdDownBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void LightningBtn()
	{
		GobalLight.Instance.LightingNotDark = !GobalLight.Instance.LightingNotDark;
		if (GobalLight.Instance.LightingNotDark)
		{
			LightningBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			LightningBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void NormalSunBtn()
	{
		PlayerManager.Instance.NormalSunUp = !PlayerManager.Instance.NormalSunUp;
		if (PlayerManager.Instance.NormalSunUp)
		{
			NormalSunBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			NormalSunBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void SunUpBtn()
	{
		PlayerManager.Instance.GetSunUp = !PlayerManager.Instance.GetSunUp;
		if (PlayerManager.Instance.GetSunUp)
		{
			SunUpBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			SunUpBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void SlowSunBtn()
	{
		SkyManager.Instance.SlowSunAutoCollect = !SkyManager.Instance.SlowSunAutoCollect;
		if (SkyManager.Instance.SlowSunAutoCollect)
		{
			SlowSunBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			SlowSunBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void RainFogBtn()
	{
		SkyManager.Instance.isRainFog = !SkyManager.Instance.isRainFog;
		if (SkyManager.Instance.isRainFog)
		{
			RainFogBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			RainFogBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void VsyncBtn()
	{
		isVsync = !isVsync;
		if (isVsync)
		{
			QualitySettings.vSyncCount = 1;
			VsyncBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			VsyncBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		_ = GameManager.Instance.isAndroid;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void FrameUpBtn()
	{
		FrameType++;
		if (FrameType > 5)
		{
			FrameType = 0;
		}
		Application.targetFrameRate = SetFrameType(FrameType);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void FrameDownBtn()
	{
		FrameType--;
		if (FrameType < 0)
		{
			FrameType = 5;
		}
		Application.targetFrameRate = SetFrameType(FrameType);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void ResolutionUpBtn()
	{
		is1080P = !is1080P;
		if (!GameManager.Instance.isAndroid)
		{
			if (is1080P)
			{
				Screen.SetResolution(1920, 1080, isFullScreen);
				ResolutionText.text = "1920*1080";
			}
			else
			{
				Screen.SetResolution(1280, 720, isFullScreen);
				ResolutionText.text = "1280*720";
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void ResolutionDownBtn()
	{
		is1080P = !is1080P;
		if (!GameManager.Instance.isAndroid)
		{
			if (is1080P)
			{
				Screen.SetResolution(1920, 1080, isFullScreen);
				ResolutionText.text = "1920*1080";
			}
			else
			{
				Screen.SetResolution(1280, 720, isFullScreen);
				ResolutionText.text = "1280*720";
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void FrameDisplayBtn()
	{
		isDisFrame = !isDisFrame;
		FrameDisObj.gameObject.SetActive(isDisFrame);
		if (isDisFrame)
		{
			FrameDisplayBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			FrameDisplayBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void CardSlectorBtn()
	{
		SeedBank.Instance.CardSelector = !SeedBank.Instance.CardSelector;
		if (SeedBank.Instance.CardSelector)
		{
			CardSlectorBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			CardSlectorBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void OpenQuickChatBtn()
	{
		OpenQuickChat = !OpenQuickChat;
		if (OpenQuickChat)
		{
			OpenQChatBt.image.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			OpenQChatBt.image.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void CloseSetPanel()
	{
		ShowPanel(isShow: false, isBattle: false);
	}

	public void FullScreenButton()
	{
		isFullScreen = !isFullScreen;
		if (isFullScreen)
		{
			FullScreen.sprite = NormalSprite.Instance.CheckBoxYes;
			Screen.SetResolution(1920, 1080, isFullScreen);
		}
		else
		{
			FullScreen.sprite = NormalSprite.Instance.CheckBox;
			if (is1080P)
			{
				Screen.SetResolution(1920, 1080, isFullScreen);
			}
			else
			{
				Screen.SetResolution(1280, 720, isFullScreen);
			}
		}
		Screen.fullScreen = isFullScreen;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}

	public void XSpeedButton()
	{
		is2xSpeed = !is2xSpeed;
		if (is2xSpeed)
		{
			X2Speed.sprite = NormalSprite.Instance.CheckBoxYes;
		}
		else
		{
			X2Speed.sprite = NormalSprite.Instance.CheckBox;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
	}
}
