using System.Collections.Generic;
using SaveClass;
using StartScene;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
	public static LevelSelector Instance;

	public LevelDisPlay DisPlay1;

	public LevelDisPlay DisPlay2;

	public LevelDisPlay DisPlay3;

	public LevelDisPlay DisPlay4;

	public Text Title;

	public Text WeatherContent;

	public Text PassNumContent;

	public Text PassTimeContent;

	public Text Difficulty;

	public GameObject IconPrefab;

	public Transform IconGroup;

	public TextMesh AdventureText;

	public Image AdvcBtn;

	public Image MiniGBtn;

	public Image PuzzBtn;

	public bool IsEasy = true;

	private int CurrIndex;

	private int CurrLvId;

	private int CurrAdvcId;

	private List<LvSave> lVInfoList;

	public LvSave SelectedLvSave { get; private set; }

	private void Awake()
	{
		Instance = this;
		AdvcBtn.material = new Material(AdvcBtn.material);
		MiniGBtn.material = new Material(MiniGBtn.material);
		PuzzBtn.material = new Material(PuzzBtn.material);
	}

	public void StartAdvcGame()
	{
		if (!GameManager.Instance.isClient)
		{
			LVManager.Instance.StartGame(null, CurrAdvcId);
		}
	}

	public void StartCurrGame()
	{
		if (!GameManager.Instance.isClient)
		{
			LVManager.Instance.StartGame(null, CurrLvId);
		}
	}

	public void ChangeHard()
	{
		IsEasy = !IsEasy;
		if (IsEasy)
		{
			Difficulty.text = "简单";
			Difficulty.color = new Color32(101, 244, 36, byte.MaxValue);
		}
		else
		{
			Difficulty.text = "困难";
			Difficulty.color = new Color32(244, 36, 39, byte.MaxValue);
		}
		DisLevelInfo(SelectedLvSave);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
	}

	public void OpenSelector()
	{
		UpdateLevelDis();
		base.transform.localScale = Vector3.one;
		base.gameObject.SetActive(value: true);
	}

	public void CloseSelector()
	{
		base.transform.localScale = Vector3.zero;
	}

	private void ClearBtn()
	{
		AdvcBtn.material.SetFloat("_Brightness", 1f);
		MiniGBtn.material.SetFloat("_Brightness", 1f);
		PuzzBtn.material.SetFloat("_Brightness", 1f);
	}

	public void LoadAdventureBtn()
	{
		ClearBtn();
		AdvcBtn.material.SetFloat("_Brightness", 1.4f);
		lVInfoList = GameManager.Instance.CurrLvSeries.LvSaves;
		LoadLastLv(GameManager.Instance.LocalPlayerSave.LastAdventureId);
		if (base.transform.localScale.x > 0f)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}

	public void LoadMiniGameBtn()
	{
		ClearBtn();
		MiniGBtn.material.SetFloat("_Brightness", 1.4f);
		lVInfoList = GameManager.Instance.CurrLvSeries.LvSavesMiniGame;
		LoadLastLv(GameManager.Instance.LocalPlayerSave.LastMiniGameId);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
	}

	public void LoadPuzzleBtn()
	{
		ClearBtn();
		PuzzBtn.material.SetFloat("_Brightness", 1.4f);
		lVInfoList = GameManager.Instance.CurrLvSeries.LvSavesPuzzle;
		LoadLastLv(GameManager.Instance.LocalPlayerSave.LastPuzzleId);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
	}

	public void LoadLastLv()
	{
		LoadAdventureBtn();
	}

	public void LoadLastLv(int lastId)
	{
		int num = lastId % 4;
		if (SelectMap.Instance.CurrLvSeriesId != lastId / 10000)
		{
			CurrIndex = 1;
			num = 1;
		}
		else
		{
			CurrIndex = lastId % 1000;
		}
		switch (num)
		{
		case 0:
			CurrIndex -= 3;
			if (CurrIndex < 1)
			{
				CurrIndex = 1;
			}
			UpdateLevelDis();
			DisPlay4.OnPointerClick(null);
			break;
		case 1:
			UpdateLevelDis();
			DisPlay1.OnPointerClick(null);
			break;
		case 2:
			CurrIndex--;
			if (CurrIndex < 1)
			{
				CurrIndex = 1;
			}
			UpdateLevelDis();
			DisPlay2.OnPointerClick(null);
			break;
		case 3:
			CurrIndex -= 2;
			if (CurrIndex < 1)
			{
				CurrIndex = 1;
			}
			UpdateLevelDis();
			DisPlay3.OnPointerClick(null);
			break;
		}
	}

	public void SelectThis(LevelDisPlay levelDis)
	{
		SelectedLvSave = levelDis.lVInfo;
		DisLevelInfo(SelectedLvSave);
		DisPlay1.OnPointerExit(null);
		DisPlay2.OnPointerExit(null);
		DisPlay3.OnPointerExit(null);
		DisPlay4.OnPointerExit(null);
	}

	private void DisLevelInfo(LvSave info)
	{
		LV.Instance.LoadLV(info.LvId, IsEasy, onlyInfo: true, isRun: false);
		MapInfoIcon[] componentsInChildren = IconGroup.GetComponentsInChildren<MapInfoIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
		for (int j = 0; j < LV.Instance.LoadMapTypes.Count; j++)
		{
			GameObject mapPrefab = MapManager.Instance.GetMapPrefab(LV.Instance.LoadMapTypes[j]);
			if (mapPrefab != null)
			{
				MapInfoIcon component = Object.Instantiate(IconPrefab).GetComponent<MapInfoIcon>();
				component.CreateInit(mapPrefab.GetComponent<MapBase>().GotoSprite, isYes: true);
				component.transform.SetParent(IconGroup);
				component.transform.localScale = Vector3.one;
			}
		}
		string text = "";
		if (HaveWeather(WeatherType.Rain))
		{
			text += " 有雨";
		}
		if (HaveWeather(WeatherType.Thunder))
		{
			text += " 有雷";
		}
		if (HaveWeather(WeatherType.Snow))
		{
			text += " 有雪";
		}
		if (HaveWeather(WeatherType.Hail))
		{
			text += " 冰雹";
		}
		if (HaveWeather(WeatherType.Wind))
		{
			text += " 有风";
		}
		if (text == "")
		{
			text = " 晴朗";
		}
		WeatherContent.text = text;
		int num;
		int num2;
		if (IsEasy)
		{
			num = info.PassNum;
			num2 = info.PassTime;
		}
		else
		{
			num = info.HardPNum;
			num2 = info.HardPTime;
		}
		Title.text = LV.Instance.LvName;
		if (num <= 0 || num2 < 5)
		{
			PassNumContent.text = "未通过";
			PassTimeContent.text = "--:--";
		}
		else
		{
			PassNumContent.text = num + "次";
			int num3 = num2 / 3600;
			int num4 = num2 % 3600 / 60;
			int num5 = num2 % 60;
			string text2 = "";
			if (num3 > 0)
			{
				text2 = text2 + num3 + "时";
			}
			if (num4 > 0)
			{
				text2 = text2 + num4 + "分";
			}
			text2 = text2 + num5 + "秒";
			PassTimeContent.text = text2;
		}
		CurrLvId = info.LvId;
		if (info.LvId % 10000 < 1000)
		{
			CurrAdvcId = info.LvId;
			AdventureText.text = info.LvId / 10000 + "-" + info.LvId % 10000;
		}
	}

	private bool HaveWeather(WeatherType weatherType)
	{
		for (int i = 0; i < LV.Instance.LoadWeathers.Count; i++)
		{
			if (LV.Instance.LoadWeathers[i].type == weatherType)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateLevelDis()
	{
		if (lVInfoList.Count >= CurrIndex)
		{
			if (CurrIndex >= 2)
			{
				DisPlay1.OpenInit(lVInfoList[CurrIndex - 1], lVInfoList[CurrIndex - 2]);
			}
			else
			{
				DisPlay1.OpenInit(lVInfoList[CurrIndex - 1], null);
			}
		}
		else
		{
			DisPlay1.OpenInit(null, null);
		}
		if (lVInfoList.Count >= CurrIndex + 1)
		{
			DisPlay2.OpenInit(lVInfoList[CurrIndex], lVInfoList[CurrIndex - 1]);
		}
		else
		{
			DisPlay2.OpenInit(null, null);
		}
		if (lVInfoList.Count >= CurrIndex + 2)
		{
			DisPlay3.OpenInit(lVInfoList[CurrIndex + 1], lVInfoList[CurrIndex]);
		}
		else
		{
			DisPlay3.OpenInit(null, null);
		}
		if (lVInfoList.Count >= CurrIndex + 3)
		{
			DisPlay4.OpenInit(lVInfoList[CurrIndex + 2], lVInfoList[CurrIndex + 1]);
		}
		else
		{
			DisPlay4.OpenInit(null, null);
		}
		DisPlay1.OnPointerExit(null);
		DisPlay2.OnPointerExit(null);
		DisPlay3.OnPointerExit(null);
		DisPlay4.OnPointerExit(null);
	}

	public void LevelUp()
	{
		if (CurrIndex >= 3)
		{
			CurrIndex -= 4;
			UpdateLevelDis();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}

	public void LevelDown()
	{
		if (CurrIndex <= lVInfoList.Count - 4)
		{
			CurrIndex += 4;
			UpdateLevelDis();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}
}
