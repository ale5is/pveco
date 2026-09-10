using System.Collections.Generic;
using System.IO;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class CustomLevelPanel : MonoBehaviour
{
	public Transform ListContent;

	public Transform MapList;

	public Transform MapListContent;

	public Text LvNameText;

	public Text ThemeText;

	public Text MapText1;

	public Text MapText2;

	public Text MapText3;

	public Text LvTypeText;

	public Text BriefText;

	public Image MapPreview;

	public GameObject OptionPrefab;

	public CustomLevelSave CurrLvSave;

	private int ClickedMap;

	private LVType currLvType;

	private MapSeries currMapSeries;

	private List<MapType> LvMapTypes = new List<MapType>();

	private List<CustomMapSave> LvCustomMaps = new List<CustomMapSave>();

	private List<CustomSaveOption> lvSaveOptions = new List<CustomSaveOption>();

	private List<CustomSaveOption> mapSaveOptions = new List<CustomSaveOption>();

	private void Start()
	{
		currMapSeries = MapSeries.Yard;
		ThemeText.text = MapManager.Instance.MapSeriesName(currMapSeries);
		LvTypeText.text = LVManager.Instance.LvTypeName(currLvType);
		LoadMapOptionName();
		LoadPreviewImage();
	}

	public void OpenInit()
	{
		base.transform.localScale = Vector3.one;
		ClearOption();
		LoadOption();
	}

	public void CloseBtn()
	{
		BtnSound();
		base.transform.localScale = Vector3.zero;
	}

	private void LoadOption()
	{
		List<CustomLevelSave> list = GameManager.Instance.LoadCustomLevelFile();
		for (int i = 0; i < list.Count; i++)
		{
			CustomSaveOption component = Object.Instantiate(OptionPrefab).GetComponent<CustomSaveOption>();
			component.CreateInit(list[i]);
			component.transform.SetParent(ListContent);
			lvSaveOptions.Add(component);
		}
		if (lvSaveOptions.Count > 0)
		{
			lvSaveOptions[0].BtnEvent();
		}
		ListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 180 * lvSaveOptions.Count);
	}

	private void ClearOption()
	{
		for (int i = 0; i < lvSaveOptions.Count; i++)
		{
			Object.Destroy(lvSaveOptions[i].gameObject);
		}
		lvSaveOptions.Clear();
		ListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 20f);
	}

	public void CreateNewBtnEvent()
	{
		BtnSound();
		GameManager.Instance.CreateNewCustomLv();
		ClearOption();
		LoadOption();
	}

	public void ChangeThemeBtnEvent()
	{
		BtnSound();
		LvMapTypes.Clear();
		if (currMapSeries == MapSeries.Yard)
		{
			LvMapTypes.Add(MapType.FrontSwamp);
			currMapSeries = MapSeries.Swamp;
		}
		else if (currMapSeries == MapSeries.Swamp)
		{
			currMapSeries = MapSeries.CustomYard;
		}
		else if (currMapSeries == MapSeries.CustomYard)
		{
			LvMapTypes.Add(MapType.FrontYard);
			currMapSeries = MapSeries.Yard;
		}
		LoadPreviewImage();
		ThemeText.text = MapManager.Instance.MapSeriesName(currMapSeries);
		LoadMapOptionName();
	}

	public void ChangeLvTypeBtnEvent()
	{
		BtnSound();
		if (currLvType == LVType.Normal)
		{
			currLvType = LVType.IZombie;
		}
		else if (currLvType == LVType.IZombie)
		{
			currLvType = LVType.VaseBreaker;
		}
		else if (currLvType == LVType.VaseBreaker)
		{
			currLvType = LVType.Normal;
		}
		LvTypeText.text = LVManager.Instance.LvTypeName(currLvType);
	}

	public void EditBtnEvent()
	{
		CloseBtn();
		MapManager.Instance.CreateMap(new List<MapType> { MapType.CustomYard });
		CameraControl.Instance.SetPosition(new Vector2(-3.5f, 0f));
		SkyManager.Instance.DirectSetTime(480);
		CreatePanel.Instance.OpenCustomMode();
	}

	public void DeleteBtnEvent()
	{
		BtnSound();
		UIManager.Instance.ConfirmPanel.InitEvent(() =>
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			DeleteConfirm();
		}, "确定删除该关卡？", CurrLvSave.LvName, "*删除后将不可恢复*");
	}

	private void DeleteConfirm()
	{
		if (File.Exists(CurrLvSave.SavePath))
		{
			File.Delete(CurrLvSave.SavePath);
		}
		ClearOption();
		LoadOption();
	}

	public void Map1Btn()
	{
		BtnSound();
		ClickedMap = 1;
		OpenMapList();
	}

	public void Map2Btn()
	{
		BtnSound();
		ClickedMap = 2;
		OpenMapList();
	}

	public void Map3Btn()
	{
		BtnSound();
		ClickedMap = 3;
		OpenMapList();
	}

	public void OpenMapList()
	{
		MapList.transform.localScale = Vector3.one;
		for (int i = 0; i < mapSaveOptions.Count; i++)
		{
			Object.Destroy(mapSaveOptions[i].gameObject);
		}
		mapSaveOptions.Clear();
		MapListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 20f);
		switch (currMapSeries)
		{
		case MapSeries.Yard:
		{
			for (int k = 0; k < 3; k++)
			{
				CustomSaveOption component2 = Object.Instantiate(OptionPrefab).GetComponent<CustomSaveOption>();
				component2.transform.SetParent(MapListContent);
				mapSaveOptions.Add(component2);
				switch (k)
				{
				case 0:
					component2.CreateInit(MapType.FrontYard, null, "原版前院地图");
					break;
				case 1:
					component2.CreateInit(MapType.BackYard, null, "原版泳池地图");
					break;
				case 2:
					component2.CreateInit(MapType.Roof, null, "原版屋顶地图");
					break;
				}
			}
			break;
		}
		case MapSeries.Swamp:
		{
			for (int l = 0; l < 2; l++)
			{
				CustomSaveOption component3 = Object.Instantiate(OptionPrefab).GetComponent<CustomSaveOption>();
				component3.transform.SetParent(MapListContent);
				mapSaveOptions.Add(component3);
				switch (l)
				{
				case 0:
					component3.CreateInit(MapType.FrontSwamp, null, "沼泽前院地图");
					break;
				case 1:
					component3.CreateInit(MapType.BackSwamp, null, "沼泽后院地图");
					break;
				case 2:
					component3.CreateInit(MapType.Basement, null, "沼泽地下室地图");
					break;
				}
			}
			break;
		}
		case MapSeries.CustomYard:
		{
			List<CustomMapSave> list = GameManager.Instance.LoadCustomMapFile();
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].mapType == MapType.CustomYard)
				{
					CustomSaveOption component = Object.Instantiate(OptionPrefab).GetComponent<CustomSaveOption>();
					component.CreateInit(MapType.CustomYard, list[j], "");
					component.transform.SetParent(MapListContent);
					mapSaveOptions.Add(component);
				}
			}
			break;
		}
		}
		MapListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 180 * mapSaveOptions.Count);
		if (mapSaveOptions.Count == 0)
		{
			MapList.transform.localScale = Vector3.zero;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		}
	}

	public void MapSelect(MapType type)
	{
		if (!LvMapTypes.Contains(type))
		{
			if (ClickedMap == 1)
			{
				if (LvMapTypes.Count > 0)
				{
					LvMapTypes[0] = type;
				}
				else
				{
					LvMapTypes.Add(type);
				}
			}
			else if (ClickedMap == 2)
			{
				if (LvMapTypes.Count > 1)
				{
					LvMapTypes[1] = type;
				}
				else
				{
					LvMapTypes.Add(type);
				}
			}
			else if (ClickedMap == 3)
			{
				if (LvMapTypes.Count > 2)
				{
					LvMapTypes[2] = type;
				}
				else
				{
					LvMapTypes.Add(type);
				}
			}
			List<MapType> list = new List<MapType>();
			if (currMapSeries == MapSeries.Yard)
			{
				if (LvMapTypes.Contains(MapType.FrontYard))
				{
					list.Add(MapType.FrontYard);
				}
				if (LvMapTypes.Contains(MapType.BackYard))
				{
					list.Add(MapType.BackYard);
				}
				if (LvMapTypes.Contains(MapType.Roof))
				{
					list.Add(MapType.Roof);
				}
			}
			else if (currMapSeries == MapSeries.Swamp)
			{
				if (LvMapTypes.Contains(MapType.FrontSwamp))
				{
					list.Add(MapType.FrontSwamp);
				}
				if (LvMapTypes.Contains(MapType.BackSwamp))
				{
					list.Add(MapType.BackSwamp);
				}
				if (LvMapTypes.Contains(MapType.Basement))
				{
					list.Add(MapType.Basement);
				}
			}
			LvMapTypes = list;
		}
		LoadMapOptionName();
		MapList.transform.localScale = Vector3.zero;
	}

	public void MapSelect(CustomMapSave save)
	{
		if (!LvCustomMaps.Contains(save))
		{
			if (ClickedMap == 1)
			{
				if (LvCustomMaps.Count > 0)
				{
					LvCustomMaps[0] = save;
				}
				else
				{
					LvCustomMaps.Add(save);
				}
			}
			else if (ClickedMap == 2)
			{
				if (LvCustomMaps.Count > 1)
				{
					LvCustomMaps[1] = save;
				}
				else
				{
					LvCustomMaps.Add(save);
				}
			}
			else if (ClickedMap == 3)
			{
				if (LvCustomMaps.Count > 2)
				{
					LvCustomMaps[2] = save;
				}
				else
				{
					LvCustomMaps.Add(save);
				}
			}
			if (ClickedMap == 1)
			{
				if (LvMapTypes.Count > 0)
				{
					LvMapTypes[0] = save.mapType;
				}
				else
				{
					LvMapTypes.Add(save.mapType);
				}
			}
			else if (ClickedMap == 2)
			{
				if (LvMapTypes.Count > 1)
				{
					LvMapTypes[1] = save.mapType;
				}
				else
				{
					LvMapTypes.Add(save.mapType);
				}
			}
			else if (ClickedMap == 3)
			{
				if (LvMapTypes.Count > 2)
				{
					LvMapTypes[2] = save.mapType;
				}
				else
				{
					LvMapTypes.Add(save.mapType);
				}
			}
		}
		LoadMapOptionName();
		MapList.transform.localScale = Vector3.zero;
	}

	public void LoadMapOptionName()
	{
		MapText1.text = "无地图";
		MapText2.text = "无地图";
		MapText3.text = "无地图";
		for (int i = 0; i < LvMapTypes.Count && i != 3; i++)
		{
			string text = "无地图";
			if (LvMapTypes[i] == MapType.CustomYard)
			{
				if (LvCustomMaps.Count > i)
				{
					text = LvCustomMaps[i].MapName;
				}
			}
			else
			{
				text = MapManager.Instance.MapTypeName(LvMapTypes[i]);
			}
			switch (i)
			{
			case 0:
				MapText1.text = text;
				break;
			case 1:
				MapText2.text = text;
				break;
			case 2:
				MapText3.text = text;
				break;
			}
		}
	}

	public void LoadPreviewImage()
	{
		Sprite sprite = NormalSprite.Instance.CustomYard;
		switch (currMapSeries)
		{
		case MapSeries.Yard:
			sprite = NormalSprite.Instance.YardDay;
			break;
		case MapSeries.Swamp:
			sprite = NormalSprite.Instance.SwampDay;
			break;
		case MapSeries.CustomYard:
			sprite = NormalSprite.Instance.CustomYard;
			break;
		}
		MapPreview.sprite = sprite;
	}

	public void ClearSelectedMap()
	{
		BtnSound();
		LvMapTypes.Clear();
		LvCustomMaps.Clear();
		LoadMapOptionName();
	}

	public void SaveCurrMap()
	{
		string value = GameManager.Encrypt(JsonUtility.ToJson(CurrLvSave));
		StreamWriter streamWriter = new StreamWriter(CurrLvSave.SavePath);
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public void LoadSaveOption(CustomLevelSave save)
	{
		LvNameText.text = save.LvName;
		BriefText.text = save.LvBrief;
		currLvType = save.lVType;
		currMapSeries = save.series;
		LvMapTypes = save.mapTypes;
		ThemeText.text = MapManager.Instance.MapSeriesName(currMapSeries);
		LvTypeText.text = LVManager.Instance.LvTypeName(currLvType);
		LoadMapOptionName();
		LoadPreviewImage();
		CurrLvSave = save;
		for (int i = 0; i < lvSaveOptions.Count; i++)
		{
			lvSaveOptions[i].CancelLight();
		}
	}

	private void BtnSound()
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
