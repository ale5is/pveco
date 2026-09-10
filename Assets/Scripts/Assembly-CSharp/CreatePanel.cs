using System.Collections.Generic;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : MonoBehaviour
{
	public static CreatePanel Instance;

	public Transform OpenBtn;

	public Transform BtnPage;

	public Transform NormalPage;

	public Transform TimePage;

	public Transform MapPage;

	public Transform EntityPage;

	public Transform CustomBtnPage;

	public Transform InfoPage;

	public Transform TilePage;

	public Transform DecorationPage;

	private bool isOpen;

	public List<Image> NormalPageBoxList = new List<Image>();

	private WeatherType SelectedWeather;

	public Image StopTimeBox;

	public Slider WeatherSlider;

	public Slider WindSlider;

	public Text WeatherScaleText;

	public Text WindScaleText;

	public List<Image> WeatherBoxList = new List<Image>();

	public Text SelectedGridText;

	public Text SelectedPlantText;

	public Text SelectedZombieText;

	private PlantType SelectedPlantType = PlantType.PeaShooter;

	private PlantType SelectedZombiePlantType;

	private ZombieType SelectedZombieType = ZombieType.NormalZombie;

	private GridSelector LastSelector;

	private List<GridSelector> gridSelectors = new List<GridSelector>();

	public Text EntityNameText;

	public Text EntityHpText;

	public InputField EntityHpInput;

	public Text EntitySpeedText;

	public Slider SpeedSlider;

	public Text EntityAttackText;

	public Slider AttackSlider;

	private bool isPlantPage;

	private bool isZombiePage;

	private PlantBase SelectedPlant;

	private ZombieBase SelectedZombie;

	public InputField MapNameInput;

	public InputField MapBriefInput;

	public Text SelectedGridText2;

	public Text HouseTypeText;

	public Text FenceTypeText;

	public Text FenceBackTypeText;

	public bool IsCustomMode { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		isOpen = false;
		IsCustomMode = false;
		base.transform.localScale = Vector3.zero;
		SelectedPlantText.text = PlantManager.Instance.GetPlantName(SelectedPlantType);
		SelectedZombieText.text = ZombieManager.Instance.GetZombieName(SelectedZombieType);
	}

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		Grid gridPointByMouse = MapManager.Instance.GetGridPointByMouse();
		if (gridPointByMouse != null)
		{
			Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (isPlantPage && Vector2.Distance(vector, gridPointByMouse.Position) < 1.5f)
			{
				SelectPlant(gridPointByMouse);
			}
			if (isZombiePage)
			{
				SelectZombie(gridPointByMouse, vector);
			}
		}
	}

	public void EnableCreateUpdate()
	{
		if (IsCustomMode)
		{
			return;
		}
		if (PlayerManager.Instance.EnableCreate)
		{
			OpenBtn.transform.localScale = Vector3.one;
			return;
		}
		if (isOpen)
		{
			Openthis();
		}
		OpenBtn.transform.localScale = Vector3.zero;
	}

	public void Openthis()
	{
		isOpen = !isOpen;
		if (IsCustomMode)
		{
			CustomBtnPage.localScale = Vector3.one;
			BtnPage.localScale = Vector3.zero;
		}
		else
		{
			BtnPage.localScale = Vector3.one;
			CustomBtnPage.localScale = Vector3.zero;
		}
		if (isOpen)
		{
			base.transform.localScale = Vector3.one;
			RefreshWeatherBox(SelectedWeather);
			RefreshDayCycle();
			RefreshNormalPage();
		}
		else
		{
			if (IsCustomMode)
			{
				OpenCustomInfoPage();
			}
			else
			{
				OpenNormalPage();
			}
			base.transform.localScale = Vector3.zero;
		}
	}

	public void OpenCustomMode()
	{
		CustomMapSave currMapSave = CustomScence.Instance.mapPanel.CurrMapSave;
		IsCustomMode = true;
		MapNameInput.text = currMapSave.MapName;
		MapBriefInput.text = currMapSave.MapBrief;
		HouseTypeText.text = (currMapSave.HouseType + 1).ToString();
		FenceTypeText.text = (currMapSave.FenceType + 1).ToString();
		FenceBackTypeText.text = (currMapSave.FenceBackType + 1).ToString();
		OpenBtn.transform.localScale = Vector3.one;
		OpenCustomInfoPage();
	}

	public void CloseAll()
	{
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapManager.Instance.mapList[i].HideGridSelector();
		}
		isPlantPage = false;
		isZombiePage = false;
		NormalPage.localScale = Vector3.zero;
		TimePage.localScale = Vector3.zero;
		MapPage.localScale = Vector3.zero;
		EntityPage.localScale = Vector3.zero;
		InfoPage.localScale = Vector3.zero;
		TilePage.localScale = Vector3.zero;
		DecorationPage.localScale = Vector3.zero;
		SelectedPlant = null;
		SelectedZombie = null;
	}

	public void BattleUIOpen()
	{
		if (PlayerManager.Instance.EnableCreate && !GameManager.Instance.isClient)
		{
			OpenBtn.transform.localScale = Vector3.one;
		}
	}

	public void LvRest()
	{
		IsCustomMode = false;
		if (isOpen)
		{
			Openthis();
		}
		OpenBtn.transform.localScale = Vector3.zero;
		gridSelectors.Clear();
	}

	public void OpenNormalPage()
	{
		CloseAll();
		NormalPage.localScale = Vector3.one;
	}

	public void OpenTimePage()
	{
		CloseAll();
		TimePage.localScale = Vector3.one;
	}

	public void OpenMapPage()
	{
		CloseAll();
		MapPage.localScale = Vector3.one;
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapManager.Instance.mapList[i].DisplayGridSelector();
		}
	}

	public void OpenPlantPage()
	{
		CloseAll();
		isPlantPage = true;
		EntityNameText.text = "未选中植物";
		EntityHpText.text = "???";
		EntityPage.localScale = Vector3.one;
	}

	public void OpenZombiePage()
	{
		CloseAll();
		isZombiePage = true;
		EntityNameText.text = "未选中僵尸";
		EntityHpText.text = "???";
		EntityPage.localScale = Vector3.one;
	}

	public void OpenCustomInfoPage()
	{
		CloseAll();
		InfoPage.localScale = Vector3.one;
	}

	public void OpenCustomTilePage()
	{
		CloseAll();
		TilePage.localScale = Vector3.one;
		for (int i = 0; i < MapManager.Instance.mapList.Count; i++)
		{
			MapManager.Instance.mapList[i].DisplayGridSelector();
		}
	}

	public void OpenCustomDecorationPage()
	{
		CloseAll();
		DecorationPage.localScale = Vector3.one;
	}

	public void RefreshNormalPage()
	{
		if (PlayerManager.Instance.SunInfinite)
		{
			NormalPageBoxList[0].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[0].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (SeedBank.Instance.isNoCD)
		{
			NormalPageBoxList[1].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[1].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (PlantManager.Instance.PlantDontSleep)
		{
			NormalPageBoxList[2].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[2].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (PlantManager.Instance.PlantInvincible)
		{
			NormalPageBoxList[3].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[3].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (ZombieManager.Instance.ZombieInvincible)
		{
			NormalPageBoxList[4].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[4].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (LVManager.Instance.StopSpawn)
		{
			NormalPageBoxList[5].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[5].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (ZombieManager.Instance.ZombieDontMove)
		{
			NormalPageBoxList[6].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[6].sprite = NormalSprite.Instance.CheckBox2;
		}
		if (LvItemManager.Instance.VaseAlwaysLight)
		{
			NormalPageBoxList[7].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[7].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void SunInfiniteBtnEvent()
	{
		BtnSound();
		PlayerManager.Instance.SunInfinite = !PlayerManager.Instance.SunInfinite;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (PlayerManager.Instance.SunInfinite)
		{
			NormalPageBoxList[0].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[0].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void CDInfiniteBtnEvent()
	{
		BtnSound();
		SeedBank.Instance.isNoCD = !SeedBank.Instance.isNoCD;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (SeedBank.Instance.isNoCD)
		{
			NormalPageBoxList[1].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[1].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void PlantDontSleepBtnEvent()
	{
		BtnSound();
		PlantManager.Instance.PlantDontSleep = !PlantManager.Instance.PlantDontSleep;
		if (PlantManager.Instance.PlantDontSleep)
		{
			NormalPageBoxList[2].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[2].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void PlantInvincibleBtnEvent()
	{
		BtnSound();
		PlantManager.Instance.PlantInvincible = !PlantManager.Instance.PlantInvincible;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (PlantManager.Instance.PlantInvincible)
		{
			NormalPageBoxList[3].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[3].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void ZombieInvincibleBtnEvent()
	{
		BtnSound();
		ZombieManager.Instance.ZombieInvincible = !ZombieManager.Instance.ZombieInvincible;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (ZombieManager.Instance.ZombieInvincible)
		{
			NormalPageBoxList[4].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[4].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void StopSpawnBtnEvent()
	{
		BtnSound();
		LVManager.Instance.StopSpawn = !LVManager.Instance.StopSpawn;
		if (LVManager.Instance.StopSpawn)
		{
			NormalPageBoxList[5].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[5].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void ZombieMoveBtnEvent()
	{
		BtnSound();
		ZombieManager.Instance.ZombieDontMove = !ZombieManager.Instance.ZombieDontMove;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (ZombieManager.Instance.ZombieDontMove)
		{
			NormalPageBoxList[6].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[6].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void VaseXRayBtnEvent()
	{
		BtnSound();
		LvItemManager.Instance.VaseAlwaysLight = !LvItemManager.Instance.VaseAlwaysLight;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (LvItemManager.Instance.VaseAlwaysLight)
		{
			NormalPageBoxList[7].sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			NormalPageBoxList[7].sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void TimeSetDay()
	{
		BtnSound();
		SkyManager.Instance.DirectSetTime(360);
	}

	public void TimeSetNoon()
	{
		BtnSound();
		SkyManager.Instance.DirectSetTime(720);
	}

	public void TimeSetNight()
	{
		BtnSound();
		SkyManager.Instance.DirectSetTime(1080);
	}

	public void TimeSetMidnight()
	{
		BtnSound();
		SkyManager.Instance.DirectSetTime(0);
	}

	public void ChangeStopTime()
	{
		BtnSound();
		SkyManager.Instance.DayLightCycle = !SkyManager.Instance.DayLightCycle;
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.SendCommandBag();
		}
		if (SkyManager.Instance.DayLightCycle)
		{
			StopTimeBox.sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			StopTimeBox.sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void RefreshDayCycle()
	{
		if (SkyManager.Instance.DayLightCycle)
		{
			StopTimeBox.sprite = NormalSprite.Instance.CheckBoxYes2;
		}
		else
		{
			StopTimeBox.sprite = NormalSprite.Instance.CheckBox2;
		}
	}

	public void NopeBoxEvent()
	{
		BtnSound();
		RefreshWeatherBox(WeatherType.Clear);
	}

	public void RainBoxEvent()
	{
		BtnSound();
		RefreshWeatherBox(WeatherType.Rain);
	}

	public void SnowBoxEvent()
	{
		BtnSound();
		RefreshWeatherBox(WeatherType.Snow);
	}

	public void HailBoxEvent()
	{
		BtnSound();
		RefreshWeatherBox(WeatherType.Hail);
	}

	public void ThunderBoxEvent()
	{
		BtnSound();
		RefreshWeatherBox(WeatherType.Thunder);
	}

	public void WeatherScaleChange()
	{
		if (WeatherSlider.value == 0f)
		{
			WeatherScaleText.text = "小";
		}
		else if (WeatherSlider.value == 1f)
		{
			WeatherScaleText.text = "中";
		}
		else if (WeatherSlider.value == 2f)
		{
			WeatherScaleText.text = "大";
		}
	}

	public void ConfirmWeather()
	{
		BtnSound();
		int scale = 0;
		if (WeatherSlider.value == 0f)
		{
			scale = 4;
		}
		else if (WeatherSlider.value == 1f)
		{
			scale = 7;
		}
		else if (WeatherSlider.value == 2f)
		{
			scale = 10;
		}
		switch (SelectedWeather)
		{
		case WeatherType.Clear:
			SkyManager.Instance.ClearAllWeather(isDirect: false);
			break;
		case WeatherType.Rain:
			SkyManager.Instance.SetRainScale(scale, isDirect: false);
			break;
		case WeatherType.Thunder:
			SkyManager.Instance.SetRainScale(10, isDirect: false);
			SkyManager.Instance.IsThunder = true;
			break;
		case WeatherType.Snow:
			SkyManager.Instance.SetSnowScale(scale, isDirect: false);
			break;
		case WeatherType.Hail:
			SkyManager.Instance.SetHailScale(scale, isDirect: false);
			break;
		case WeatherType.Wind:
			break;
		}
	}

	public void WindScaleChange()
	{
		WindScaleText.text = WindSlider.value.ToString();
		SkyManager.Instance.SetWindScale((int)WindSlider.value);
	}

	private void RefreshWeatherBox(WeatherType type)
	{
		SelectedWeather = type;
		for (int i = 0; i < WeatherBoxList.Count; i++)
		{
			WeatherBoxList[i].sprite = NormalSprite.Instance.CheckBox2;
		}
		switch (SelectedWeather)
		{
		case WeatherType.Clear:
			WeatherBoxList[0].sprite = NormalSprite.Instance.CheckBoxYes2;
			break;
		case WeatherType.Rain:
			WeatherBoxList[1].sprite = NormalSprite.Instance.CheckBoxYes2;
			break;
		case WeatherType.Thunder:
			WeatherBoxList[4].sprite = NormalSprite.Instance.CheckBoxYes2;
			break;
		case WeatherType.Snow:
			WeatherBoxList[2].sprite = NormalSprite.Instance.CheckBoxYes2;
			break;
		case WeatherType.Hail:
			WeatherBoxList[3].sprite = NormalSprite.Instance.CheckBoxYes2;
			break;
		case WeatherType.Wind:
			break;
		}
	}

	public void AddGridSelection(GridSelector selector)
	{
		if (!gridSelectors.Contains(selector))
		{
			gridSelectors.Add(selector);
			SelectedGridText.text = "已选中" + gridSelectors.Count + "个格子";
			SelectedGridText2.text = "已选中" + gridSelectors.Count + "个格子";
		}
	}

	public void RemoveGridSelection(GridSelector selector)
	{
		if (gridSelectors.Remove(selector))
		{
			SelectedGridText.text = "已选中" + gridSelectors.Count + "个格子";
			SelectedGridText2.text = "已选中" + gridSelectors.Count + "个格子";
		}
	}

	public void SelectorMouseDown(GridSelector selector)
	{
		LastSelector = selector;
	}

	public void SelectorMouseUp()
	{
		if (LastSelector != null)
		{
			Grid gridPointByMouse = MapManager.Instance.GetGridPointByMouse();
			Vector2Int point = LastSelector.grid.Point;
			Vector2Int point2 = gridPointByMouse.Point;
			MapBase currMap = MapManager.Instance.GetCurrMap(LastSelector.grid.Position);
			MapBase currMap2 = MapManager.Instance.GetCurrMap(gridPointByMouse.Position);
			if (currMap == currMap2)
			{
				int x = point2.x;
				int x2 = point.x;
				if (point.x > point2.x)
				{
					x = point.x;
					x2 = point2.x;
				}
				int y = point2.y;
				int y2 = point.y;
				if (point.y > point2.y)
				{
					y = point.y;
					y2 = point2.y;
				}
				List<GridSelector> gridSelector = currMap.GetGridSelector();
				for (int i = 0; i < gridSelector.Count; i++)
				{
					if (gridSelector[i].grid.Point.x <= x && gridSelector[i].grid.Point.x >= x2 && gridSelector[i].grid.Point.y <= y && gridSelector[i].grid.Point.y >= y2)
					{
						if (gridSelectors.Contains(LastSelector))
						{
							gridSelector[i].SelectThis();
						}
						else
						{
							gridSelector[i].CancelSelect();
						}
					}
				}
			}
		}
		LastSelector = null;
	}

	public void SelectCard(PlantType plantType, ZombieType zombieType)
	{
		if (plantType != PlantType.Nope)
		{
			if (PlantManager.Instance.IsZombiePlant(plantType))
			{
				SelectedZombiePlantType = plantType;
				SelectedZombieType = ZombieType.Nope;
				SelectedZombieText.text = PlantManager.Instance.GetPlantName(plantType);
			}
			else
			{
				SelectedPlantType = plantType;
				SelectedPlantText.text = PlantManager.Instance.GetPlantName(plantType);
			}
		}
		if (zombieType != ZombieType.Nope)
		{
			SelectedZombieType = zombieType;
			SelectedZombieText.text = ZombieManager.Instance.GetZombieName(zombieType);
		}
	}

	public void LeftLadderBtnEvent()
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.SetLadder(isLeft: true, !gridSelectors[i].grid.HaveLeftLadder);
		}
	}

	public void RightLadderBtnEvent()
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.SetLadder(isLeft: false, !gridSelectors[i].grid.HaveRightLadder);
		}
	}

	public void TombBtnEvent()
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.HaveGraveStone = !gridSelectors[i].grid.HaveGraveStone;
		}
	}

	public void CraterBtnEvent()
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.HaveCrater = !gridSelectors[i].grid.HaveCrater;
		}
	}

	public void ClearPlantBtnEvent()
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			Grid grid = gridSelectors[i].grid;
			if (grid.CurrPlantBase != null)
			{
				if (grid.CurrPlantBase.ProtectPlant != null)
				{
					grid.CurrPlantBase.ProtectPlant.Dead();
				}
				grid.CurrPlantBase.Dead();
			}
		}
	}

	public void ClearGridBtnEvent()
	{
		BtnSound();
		List<GridSelector> list = new List<GridSelector>(gridSelectors);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].CancelSelect();
		}
	}

	public void ChoosePlantBtnEvent()
	{
		SeedChooser.Instance.OpenForCreate();
	}

	public void PlacePlantBtnEvent()
	{
		PlacePlant(SelectedPlantType);
	}

	private void PlacePlant(PlantType type)
	{
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			Grid grid = gridSelectors[i].grid;
			if (grid == null)
			{
				continue;
			}
			PlantBase newPlant = PlantManager.Instance.GetNewPlant(type);
			if (newPlant != null)
			{
				if (SeedBank.Instance.CheckPlant(newPlant, grid, -2, null))
				{
					SeedBank.Instance.PlantConfirm(newPlant, grid, -2, 3, null);
				}
				else
				{
					Object.Destroy(newPlant.gameObject);
				}
			}
		}
	}

	public void ChooseZombieBtnEvent()
	{
		ZombieChooser.Instance.OpenForCreate();
	}

	public void PlaceZombieBtnEvent()
	{
		if (SelectedZombieType != ZombieType.Nope)
		{
			for (int i = 0; i < gridSelectors.Count; i++)
			{
				Grid grid = gridSelectors[i].grid;
				if (grid != null)
				{
					ZombieManager.Instance.UpdateZombie(SelectedZombieType, grid.Position);
				}
			}
		}
		else if (SelectedZombiePlantType != PlantType.Nope)
		{
			PlacePlant(SelectedZombiePlantType);
		}
	}

	public void SelectPlant(Grid grid)
	{
		PlantBase plantBase = null;
		if (grid.CurrPlantBase != null)
		{
			if (SelectedPlant == null || SelectedPlant.currGrid != grid)
			{
				plantBase = grid.CurrPlantBase;
			}
			else if (SelectedPlant == grid.CurrPlantBase)
			{
				if (grid.CurrPlantBase.CarryPlant != null)
				{
					plantBase = grid.CurrPlantBase.CarryPlant;
				}
				else if (grid.CurrPlantBase.ProtectPlant != null)
				{
					plantBase = grid.CurrPlantBase.ProtectPlant;
				}
			}
			else if (SelectedPlant == grid.CurrPlantBase.CarryPlant)
			{
				plantBase = ((!(grid.CurrPlantBase.ProtectPlant != null)) ? grid.CurrPlantBase : grid.CurrPlantBase.ProtectPlant);
			}
			else if (SelectedPlant == grid.CurrPlantBase.ProtectPlant)
			{
				plantBase = grid.CurrPlantBase;
			}
		}
		if (plantBase != null)
		{
			SelectedPlant = plantBase;
			EntityNameText.text = PlantManager.Instance.GetPlantName(SelectedPlant.GetPlantType());
			EntityHpText.text = SelectedPlant.Hp.ToString();
		}
	}

	public void SelectZombie(Grid grid, Vector2 ClickedPos)
	{
		ZombieBase zombieBase = null;
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(grid.Point.y, ClickedPos, getHyp: false);
		ZombieBase zombieByLineMinDisNoDir2 = ZombieManager.Instance.GetZombieByLineMinDisNoDir(grid.Point.y, ClickedPos, getHyp: true);
		if (zombieByLineMinDisNoDir != null)
		{
			zombieBase = ((!(zombieByLineMinDisNoDir2 != null)) ? zombieByLineMinDisNoDir : ((!(Mathf.Abs(zombieByLineMinDisNoDir.transform.position.x - ClickedPos.x) < Mathf.Abs(zombieByLineMinDisNoDir2.transform.position.x - ClickedPos.x))) ? zombieByLineMinDisNoDir2 : zombieByLineMinDisNoDir));
		}
		else if (zombieByLineMinDisNoDir2 != null)
		{
			zombieBase = zombieByLineMinDisNoDir2;
		}
		if (zombieBase != null && Vector2.Distance(zombieBase.transform.position, ClickedPos) < 1.5f)
		{
			SelectedZombie = zombieBase;
			EntityNameText.text = ZombieManager.Instance.GetZombieName(SelectedZombie.zombieType);
			EntityHpText.text = SelectedZombie.Hp.ToString();
		}
	}

	public void ChangeHp(PlantBase plant, ZombieBase zombie)
	{
		int num = -10000;
		if (plant != null && plant == SelectedPlant)
		{
			num = (int)SelectedPlant.Hp;
		}
		else if (zombie != null && zombie == SelectedZombie)
		{
			num = SelectedZombie.Hp;
		}
		if (num > -10000)
		{
			if (num < 0)
			{
				num = 0;
			}
			EntityHpText.text = num.ToString();
		}
	}

	public void PlantDeadEvent(PlantBase plant)
	{
		if (plant == SelectedPlant)
		{
			EntityNameText.text = "未选中植物";
			EntityHpText.text = "???";
			SelectedPlant = null;
		}
	}

	public void ZombieDeadEvent(ZombieBase zombie)
	{
		if (zombie == SelectedZombie)
		{
			EntityNameText.text = "未选中僵尸";
			EntityHpText.text = "???";
			SelectedZombie = null;
		}
	}

	public void ChangeHpBtnEvent()
	{
		if (int.TryParse(EntityHpInput.text, out var result))
		{
			if (SelectedPlant != null)
			{
				SelectedPlant.CustomHp(result);
			}
			if (SelectedZombie != null)
			{
				SelectedZombie.CustomHp(result);
			}
		}
	}

	public void HypnoBtnEvent()
	{
		if (SelectedPlant != null)
		{
			SelectedPlant.Hypno();
		}
		if (SelectedZombie != null)
		{
			SelectedZombie.Hypno();
		}
	}

	public void DizzyBtnEvent()
	{
		if (SelectedPlant != null)
		{
			SelectedPlant.Dizzy(4);
		}
		if (SelectedZombie != null)
		{
			SelectedZombie.Dizzy(4);
		}
	}

	public void IceBtnEvent()
	{
		if (SelectedPlant != null)
		{
			SelectedPlant.Ice();
		}
		if (SelectedZombie != null)
		{
			SelectedZombie.Ice();
		}
	}

	public void ButterBtnEvent()
	{
		if (SelectedPlant != null)
		{
			SelectedPlant.Butter();
		}
		if (SelectedZombie != null)
		{
			SelectedZombie.Butter();
		}
	}

	public void SpeedSliderEvent()
	{
		int num = (int)SpeedSlider.value;
		float speed = 1f;
		switch (num)
		{
		case 0:
			speed = 0.2f;
			break;
		case 1:
			speed = 0.5f;
			break;
		case 3:
			speed = 1.5f;
			break;
		case 4:
			speed = 2f;
			break;
		case 5:
			speed = 5f;
			break;
		case 6:
			speed = 10f;
			break;
		}
		EntitySpeedText.text = speed + "x";
		if (SelectedPlant != null)
		{
			SelectedPlant.CustomSpeed(speed);
		}
		if (SelectedZombie != null)
		{
			SelectedZombie.CustomSpeed(speed);
		}
	}

	public void AttackSliderEvent()
	{
		int num = (int)AttackSlider.value;
		float value = 1f;
		switch (num)
		{
		case 0:
			value = 0f;
			break;
		case 1:
			value = 0.2f;
			break;
		case 2:
			value = 0.5f;
			break;
		case 4:
			value = 2f;
			break;
		case 5:
			value = 5f;
			break;
		case 6:
			value = 10f;
			break;
		case 7:
			value = 20f;
			break;
		case 8:
			value = 50f;
			break;
		}
		EntityAttackText.text = value + "x";
		if (SelectedPlant != null)
		{
			SelectedPlant.CustomAttack(value);
		}
		if (SelectedZombie != null)
		{
			SelectedZombie.CustomAttack(value);
		}
	}

	public void CustomSaveClose()
	{
		BtnSound();
		CustomMapSave currMapSave = CustomScence.Instance.mapPanel.CurrMapSave;
		currMapSave.MapName = MapNameInput.text;
		currMapSave.MapBrief = MapBriefInput.text;
		currMapSave.tileTypes.Clear();
		CustomScence.Instance.mapPanel.SaveCurrMap();
		LVManager.Instance.QuitBattleGame(StartScenceAnim: false);
		CustomScence.Instance.OpenInit();
		CameraControl.Instance.SetPosition(new Vector2(-25f, -80f));
	}

	public void SetNopeGrid()
	{
		BtnSound();
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.customTile.SetType(TileType.Nope, gridSelectors[i].grid);
		}
	}

	public void SetGrassGrid()
	{
		BtnSound();
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.customTile.SetType(TileType.Grass, gridSelectors[i].grid);
		}
	}

	public void SetWaterGrid()
	{
		BtnSound();
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.customTile.SetType(TileType.Water, gridSelectors[i].grid);
		}
	}

	public void SetStoneGrid()
	{
		BtnSound();
		for (int i = 0; i < gridSelectors.Count; i++)
		{
			gridSelectors[i].grid.customTile.SetType(TileType.Stone, gridSelectors[i].grid);
		}
	}

	public void ChangeHouseTypeBtn()
	{
		BtnSound();
		CustomScence.Instance.mapPanel.CurrMapSave.HouseType = MapManager.Instance.mapList[0].ChangeDecoration(0);
		HouseTypeText.text = (CustomScence.Instance.mapPanel.CurrMapSave.HouseType + 1).ToString();
	}

	public void ChangeFenceTypeBtn()
	{
		BtnSound();
		CustomScence.Instance.mapPanel.CurrMapSave.FenceType = MapManager.Instance.mapList[0].ChangeDecoration(1);
		FenceTypeText.text = (CustomScence.Instance.mapPanel.CurrMapSave.FenceType + 1).ToString();
	}

	public void ChangeFenceBackTypeBtn()
	{
		BtnSound();
		CustomScence.Instance.mapPanel.CurrMapSave.FenceBackType = MapManager.Instance.mapList[0].ChangeDecoration(2);
		FenceBackTypeText.text = (CustomScence.Instance.mapPanel.CurrMapSave.FenceBackType + 1).ToString();
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
