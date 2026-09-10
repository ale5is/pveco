using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class CustomSaveOption : MonoBehaviour
{
	public Text NameText;

	public Text BriefText;

	private MapType mapType;

	private CustomMapSave customMapSave;

	private CustomLevelSave customLvSave;

	public void CreateInit(CustomMapSave save)
	{
		mapType = MapType.Nope;
		customMapSave = save;
		customLvSave = null;
		NameText.text = save.MapName;
		BriefText.text = save.MapBrief;
	}

	public void CreateInit(CustomLevelSave save)
	{
		mapType = MapType.Nope;
		customLvSave = save;
		customMapSave = null;
		NameText.text = save.LvName;
		BriefText.text = LVManager.Instance.LvTypeName(save.lVType) + " " + MapManager.Instance.MapSeriesName(save.series);
	}

	public void CreateInit(MapType type, CustomMapSave save, string brief)
	{
		mapType = type;
		customLvSave = null;
		customMapSave = save;
		if (save == null)
		{
			NameText.text = MapManager.Instance.MapTypeName(type);
			BriefText.text = brief;
		}
		else
		{
			NameText.text = save.MapName;
			BriefText.text = save.MapBrief;
		}
	}

	public void BtnEvent()
	{
		if (customLvSave != null)
		{
			CustomScence.Instance.levelPanel.LoadSaveOption(customLvSave);
		}
		if (mapType == MapType.Nope)
		{
			if (customMapSave != null)
			{
				CustomScence.Instance.mapPanel.LoadSaveOption(customMapSave);
			}
		}
		else if (customMapSave == null)
		{
			CustomScence.Instance.levelPanel.MapSelect(mapType);
		}
		else
		{
			CustomScence.Instance.levelPanel.MapSelect(customMapSave);
		}
		base.transform.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
	}

	public void CancelLight()
	{
		base.transform.GetComponent<Image>().color = Color.white;
	}
}
