using System.Collections.Generic;
using System.IO;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class CustomMapPanel : MonoBehaviour
{
	public Transform ListContent;

	public Text MapNameText;

	public Text ThemeText;

	public Text HorizontalText;

	public Text VerticalText;

	public Text NotesText;

	public GameObject OptionPrefab;

	private int HorizontalNum;

	private int VerticalNum;

	public CustomMapSave CurrMapSave;

	private List<CustomSaveOption> mapSaveOptions = new List<CustomSaveOption>();

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
		List<CustomMapSave> list = GameManager.Instance.LoadCustomMapFile();
		for (int i = 0; i < list.Count; i++)
		{
			CustomSaveOption component = Object.Instantiate(OptionPrefab).GetComponent<CustomSaveOption>();
			component.CreateInit(list[i]);
			component.transform.SetParent(ListContent);
			mapSaveOptions.Add(component);
		}
		if (mapSaveOptions.Count > 0)
		{
			mapSaveOptions[0].BtnEvent();
		}
		ListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 180 * mapSaveOptions.Count);
	}

	private void ClearOption()
	{
		for (int i = 0; i < mapSaveOptions.Count; i++)
		{
			Object.Destroy(mapSaveOptions[i].gameObject);
		}
		mapSaveOptions.Clear();
		ListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 20f);
	}

	public void CreateNewBtnEvent()
	{
		BtnSound();
		GameManager.Instance.CreateNewCustomMap();
		ClearOption();
		LoadOption();
	}

	public void ChangeThemeBtnEvent()
	{
		BtnSound();
	}

	public void HorizontalUpBtnEvent()
	{
		BtnSound();
		HorizontalNum++;
		HorizontalNum = Mathf.Clamp(HorizontalNum, 9, 11);
		HorizontalText.text = HorizontalNum.ToString();
	}

	public void HorizontalDownBtnEvent()
	{
		BtnSound();
		HorizontalNum--;
		HorizontalNum = Mathf.Clamp(HorizontalNum, 9, 11);
		HorizontalText.text = HorizontalNum.ToString();
	}

	public void VerticalUpBtnEvent()
	{
		BtnSound();
		VerticalNum++;
		VerticalNum = Mathf.Clamp(VerticalNum, 5, 7);
		VerticalText.text = VerticalNum.ToString();
	}

	public void VerticalDownBtnEvent()
	{
		BtnSound();
		VerticalNum--;
		VerticalNum = Mathf.Clamp(VerticalNum, 5, 7);
		VerticalText.text = VerticalNum.ToString();
	}

	public void EditBtnEvent()
	{
		CloseBtn();
		MapManager.Instance.CreateMap(new List<MapType> { MapType.CustomYard });
		MapManager.Instance.mapList[0].CustomMapEditInit(VerticalNum, HorizontalNum, CurrMapSave);
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
		}, "确定删除该地图？", CurrMapSave.MapName, "*删除后将不可恢复*");
	}

	public void DeleteConfirm()
	{
		if (File.Exists(CurrMapSave.SavePath))
		{
			File.Delete(CurrMapSave.SavePath);
		}
		ClearOption();
		LoadOption();
	}

	public void SaveCurrMap()
	{
		if (MapManager.Instance.mapList.Count > 0)
		{
			CurrMapSave.HorizontalNum = MapManager.Instance.mapList[0].MapGridNum.x;
			CurrMapSave.VerticalNum = MapManager.Instance.mapList[0].MapGridNum.y;
			List<Grid> gridList = MapManager.Instance.mapList[0].GridList;
			for (int i = 0; i < gridList.Count; i++)
			{
				CurrMapSave.tileTypes.Add(gridList[i].customTile.GetTileType());
			}
			if (CurrMapSave.mapType == MapType.Nope)
			{
				CurrMapSave.mapType = MapType.CustomYard;
			}
		}
		string value = GameManager.Encrypt(JsonUtility.ToJson(CurrMapSave));
		StreamWriter streamWriter = new StreamWriter(CurrMapSave.SavePath);
		streamWriter.Write(value);
		streamWriter.Close();
	}

	public void LoadSaveOption(CustomMapSave save)
	{
		MapNameText.text = save.MapName;
		NotesText.text = save.MapBrief;
		VerticalNum = Mathf.Clamp(save.VerticalNum, 5, 7);
		HorizontalNum = Mathf.Clamp(save.HorizontalNum, 9, 11);
		VerticalText.text = VerticalNum.ToString();
		HorizontalText.text = HorizontalNum.ToString();
		CurrMapSave = save;
		for (int i = 0; i < mapSaveOptions.Count; i++)
		{
			mapSaveOptions[i].CancelLight();
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
