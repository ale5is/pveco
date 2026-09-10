using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class PlantCard : MonoBehaviour
{
	public int OnlineId;

	public int CardId;

	private Sprite sprite;

	private TextMesh WantSunText;

	public Transform mask;

	public SpriteRenderer grayImg;

	public SpriteRenderer ImgRenderer;

	public int NeedSun;

	public bool isNeedSun;

	public float CDTime;

	public PlantType CardPlantType;

	public ZombieType CardZombieType;

	public float currTimeForCd;

	private bool canPlace;

	[SerializeField]
	private bool wantPlace;

	private PlantBase plant;

	private List<PlantBase> plantInGrids = new List<PlantBase>();

	private ZombieBase zombie;

	private List<ZombieBase> zombieInGrids = new List<ZombieBase>();

	private List<Grid> InGridGrids = new List<Grid>();

	[SerializeField]
	private CardState cardState;

	public bool isChoosed;

	private UIPlantCardNC myPlantCard;

	private Coroutine CdCoroutine;

	private bool secondClick;

	private bool IsDropType;

	public bool isImitater;

	private float downTargetPosY;

	private bool isFromSky;

	public BoxCollider2D boxCollider;

	private bool DropNeedDes;

	private bool selectorSelected;

	public bool CanUnChoose;

	private bool dontSelect;

	private bool isRat;

	private bool isStarted => LVManager.Instance.GameIsStart;

	public bool SelectorSelected
	{
		get
		{
			if (selectorSelected)
			{
				return !IsDropType;
			}
			return false;
		}
		private set
		{
			selectorSelected = value;
		}
	}

	public bool IsRat
	{
		get
		{
			if (IsDropType)
			{
				return isRat;
			}
			if (LV.Instance.CurrLVType == LVType.IZombie)
			{
				return false;
			}
			return true;
		}
		private set
		{
			isRat = value;
		}
	}

	public CardState CardState
	{
		get
		{
			return cardState;
		}
		set
		{
			if (!SeedBank.Instance.CardSelector)
			{
				SelectorSelected = false;
			}
			if (cardState == value)
			{
				if (cardState == CardState.CanPlace && SelectorSelected && !dontSelect)
				{
					WantPlaceThis();
				}
				return;
			}
			cardState = value;
			if (cardState == CardState.CanPlace && SelectorSelected && !dontSelect)
			{
				WantPlaceThis();
			}
			switch (value)
			{
			case CardState.CanPlace:
				grayImg.color = Color.white;
				ImgRenderer.color = Color.white;
				mask.localScale = new Vector3(mask.localScale.x, 0f);
				break;
			case CardState.NotCD:
				grayImg.color = new Color(0.7f, 0.7f, 0.7f);
				ImgRenderer.color = new Color(0.5f, 0.5f, 0.5f);
				CDEnter();
				if (WantPlace)
				{
					CancelPlace();
				}
				break;
			case CardState.NotSun:
				grayImg.color = new Color(0.7f, 0.7f, 0.7f);
				ImgRenderer.color = new Color(0.5f, 0.5f, 0.5f);
				if (WantPlace)
				{
					CancelPlace();
				}
				mask.localScale = new Vector3(mask.localScale.x, 0f);
				break;
			case CardState.NotAll:
				grayImg.color = new Color(0.6f, 0.6f, 0.6f);
				ImgRenderer.color = new Color(0.4f, 0.4f, 0.4f);
				CDEnter();
				if (WantPlace)
				{
					CancelPlace();
				}
				break;
			}
		}
	}

	public bool CanPlace
	{
		get
		{
			return canPlace;
		}
		set
		{
			if (canPlace != value)
			{
				if (CDTime <= 0f || SeedBank.Instance.isNoCD || IsDropType || LV.Instance.CurrLVType == LVType.IZombie || LVManager.Instance.LastStandNoCd)
				{
					canPlace = true;
				}
				else
				{
					canPlace = value;
				}
				CheckState();
			}
		}
	}

	public bool WantPlace
	{
		get
		{
			return wantPlace;
		}
		private set
		{
			wantPlace = value;
			SeedBank.Instance.AllDropBox(!wantPlace);
			if (wantPlace)
			{
				InstantiatePlantZombie();
				ImgRenderer.color = new Color(0.75f, 0.75f, 0.75f);
				Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (plant != null)
				{
					plant.transform.position = new Vector3(vector.x, vector.y, 0f);
				}
				if (zombie != null)
				{
					zombie.transform.position = new Vector3(vector.x, vector.y, 0f);
				}
				if (!DropNeedDes && SeedBank.Instance.CardSelector)
				{
					GetComponent<SortingGroup>().sortingOrder = 2002;
				}
			}
			else
			{
				ImgRenderer.color = new Color(1f, 1f, 1f);
				if (SeedBank.Instance.CardSelector)
				{
					GetComponent<SortingGroup>().sortingOrder = 2001;
				}
			}
		}
	}

	private void InstantiatePlantZombie()
	{
		if (CardPlantType != PlantType.Nope && plant == null)
		{
			plant = PlantManager.Instance.GetNewPlant(CardPlantType);
			plant.InitForCreate(inGrid: false, null, isImitater);
		}
		else if (CardZombieType != ZombieType.Nope && zombie == null)
		{
			zombie = ZombieManager.Instance.GetNewZombie(CardZombieType);
			zombie.CreateInit(inGrid: false, null, IsRat);
		}
	}

	private void CDEnter()
	{
		if (!CanPlace && CdCoroutine == null)
		{
			mask.localScale = new Vector3(mask.localScale.x, 2.2f);
			currTimeForCd = CDTime;
			CdCoroutine = StartCoroutine(CalCD());
		}
	}

	private IEnumerator CalCD()
	{
		float i = 1f;
		if (SeedBank.Instance.IsCdDown)
		{
			i = 1.3f;
		}
		float calCD = 2.2f / CDTime * 0.05f * i;
		while (currTimeForCd >= 0f)
		{
			yield return new WaitForSeconds(0.05f);
			if (LVManager.Instance.IsRestTime)
			{
				mask.localScale = new Vector3(mask.localScale.x, mask.localScale.y - calCD * 2f);
				currTimeForCd -= 0.05f * i * 2f;
			}
			else
			{
				mask.localScale = new Vector3(mask.localScale.x, mask.localScale.y - calCD);
				currTimeForCd -= 0.05f * i;
			}
		}
		CdCoroutine = null;
		CanPlace = true;
	}

	private void CheckState()
	{
		if (isStarted)
		{
			float sunNum = PlayerManager.Instance.GetSunNum(isNeedSun, GameManager.Instance.LocalPlayerSave.playerName);
			if (canPlace && sunNum >= (float)NeedSun)
			{
				CardState = CardState.CanPlace;
			}
			else if (!canPlace && sunNum >= (float)NeedSun)
			{
				CardState = CardState.NotCD;
			}
			else if (canPlace && sunNum < (float)NeedSun)
			{
				CardState = CardState.NotSun;
			}
			else if (!canPlace && sunNum < (float)NeedSun)
			{
				CardState = CardState.NotAll;
			}
		}
	}

	private void InitForAll()
	{
		isRat = false;
		CanUnChoose = true;
		canPlace = true;
		isChoosed = false;
		IsDropType = false;
		isFromSky = false;
		DropNeedDes = false;
		SelectorSelected = false;
		boxCollider = base.transform.GetComponent<BoxCollider2D>();
		WantSunText = base.transform.Find("SunNumText").GetComponent<TextMesh>();
		sprite = ImgRenderer.sprite;
		grayImg.enabled = true;
		mask.gameObject.SetActive(value: true);
		ImgRenderer.maskInteraction = SpriteMaskInteraction.None;
	}

	public void CardSlotInit()
	{
		InitForAll();
		PlayerManager.Instance.AddSunNumUpdateActionListener(CheckState);
		LVManager.Instance.AddLVStartActionListenr(OnLVStartAction);
	}

	public void InitForSky(float downTargetPosY, Vector3 SpawnPos)
	{
		InitForAll();
		IsDropType = true;
		isFromSky = true;
		this.downTargetPosY = downTargetPosY;
		base.transform.position = SpawnPos;
		StartCoroutine(SkyDown(downTargetPosY));
		StartCoroutine(Disappear());
	}

	public void InitForBelt()
	{
		InitForAll();
		IsDropType = true;
		grayImg.enabled = false;
		mask.gameObject.SetActive(value: false);
		ImgRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
	}

	public void InitForDrop(Vector2 pos, bool isLeft, float x)
	{
		InitForAll();
		IsDropType = true;
		base.transform.position = pos;
		StartCoroutine(DoJump(isLeft, x));
		StartCoroutine(Disappear());
	}

	private IEnumerator SkyDown(float downTargetPosY)
	{
		while (base.transform.position.y > downTargetPosY)
		{
			yield return null;
			base.transform.Translate(new Vector2(0f, -0.5f) * Time.deltaTime);
		}
		if (CardZombieType != ZombieType.Nope)
		{
			ZombieManager.Instance.OutGround(CardZombieType, base.transform.position, null, needArm: false, isHyp: false, purple: false);
			SeedBank.Instance.RemoveDropCard(this);
		}
	}

	private IEnumerator Disappear()
	{
		yield return new WaitForSeconds(15f);
		if (!WantPlace)
		{
			ImgRenderer.color = new Color(0.5f, 0.5f, 0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		if (!WantPlace)
		{
			ImgRenderer.color = Color.white;
		}
		yield return new WaitForSeconds(0.5f);
		if (!WantPlace)
		{
			ImgRenderer.color = new Color(0.5f, 0.5f, 0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		if (!WantPlace)
		{
			ImgRenderer.color = Color.white;
		}
		yield return new WaitForSeconds(0.5f);
		if (!WantPlace)
		{
			ImgRenderer.color = new Color(0.5f, 0.5f, 0.5f);
		}
		yield return new WaitForSeconds(0.5f);
		if (!WantPlace)
		{
			ImgRenderer.color = Color.white;
		}
		yield return new WaitForSeconds(0.5f);
		DropNeedDes = true;
		if (!WantPlace)
		{
			SeedBank.Instance.RemoveDropCard(this);
		}
	}

	private IEnumerator DoJump(bool isLeft, float x)
	{
		Vector3 startPos = base.transform.position;
		if (isLeft)
		{
			x = 0f - x;
		}
		float speed = 0f;
		float scale = base.transform.localScale.y;
		base.transform.localScale = new Vector3(0.1f, 0.1f);
		boxCollider.enabled = false;
		while (base.transform.localScale.y < scale)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				base.transform.localScale += new Vector3(Time.deltaTime * 8f, Time.deltaTime * 8f);
				speed += 0.1f;
				base.transform.Translate(new Vector3(x * Time.deltaTime, speed * Time.deltaTime, 0f));
			}
		}
		boxCollider.enabled = true;
		base.transform.localScale = new Vector3(scale, scale);
		while ((double)base.transform.position.y >= (double)startPos.y - 0.2)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				speed -= 0.08f;
				base.transform.Translate(new Vector3(x * Time.deltaTime, speed * Time.deltaTime, 0f));
			}
		}
	}

	private void OnLVStartAction()
	{
		if (CDTime <= 7.5f || !LV.Instance.StartCardCd)
		{
			CanPlace = true;
		}
		else
		{
			CanPlace = false;
		}
		CheckState();
		if (SelectorSelected)
		{
			SelectorSelected = false;
			SeedBank.Instance.CancelSelect();
			CancelPlace();
		}
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (isChoosed && isStarted)
		{
			if (Input.GetMouseButtonDown(1))
			{
				GoCancelPlant();
			}
			if (WantPlace)
			{
				Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (plant != null)
				{
					plant.transform.position = new Vector3(vector.x, vector.y, 0f);
				}
				if (zombie != null)
				{
					zombie.transform.position = new Vector3(vector.x, vector.y, 0f);
				}
				if (!secondClick && Input.GetMouseButtonDown(0))
				{
					secondClick = true;
					return;
				}
				Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(vector);
				if (gridByWorldPos == null)
				{
					return;
				}
				float num = Vector2.Distance(vector, gridByWorldPos.Position);
				if (num < 1f)
				{
					if (plant != null && SeedBank.Instance.CheckPlant(plant, gridByWorldPos, NeedSun, GameManager.Instance.LocalPlayerSave.playerName))
					{
						planting(gridByWorldPos);
					}
					else if (zombie != null && SeedBank.Instance.CheckZombie(CardZombieType, gridByWorldPos, NeedSun, GameManager.Instance.LocalPlayerSave.playerName))
					{
						Zombieing(gridByWorldPos);
					}
					else
					{
						ClearInGrid();
					}
				}
				else if (!MyTool.IsPointerOverGameObject())
				{
					if (Input.GetMouseButtonDown(0) && num > 1.5f && secondClick)
					{
						GoCancelPlant();
					}
					ClearInGrid();
				}
			}
		}
		if (isFromSky && base.transform.position.y > downTargetPosY)
		{
			base.transform.Translate(Vector3.down * Time.deltaTime);
		}
	}

	private void GoCancelPlant()
	{
		if (WantPlace || SelectorSelected)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
		}
		if (SelectorSelected)
		{
			SelectorSelected = false;
			SeedBank.Instance.CancelSelect();
		}
		CancelPlace();
		if (DropNeedDes)
		{
			SeedBank.Instance.RemoveDropCard(this);
		}
	}

	public void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 1)
		{
			Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(syn.Twofloat);
			if (CardPlantType != PlantType.Nope)
			{
				InstantiatePlantZombie();
				if (SeedBank.Instance.CheckPlant(plant, gridByWorldPos, NeedSun, syn.AName))
				{
					SeedBank.Instance.PlantConfirm(plant, gridByWorldPos, NeedSun, isImitater ? 2 : 0, syn.AName);
					SeedBank.Instance.LikeColumnPlace(gridByWorldPos, CardPlantType, CardZombieType, NeedSun, syn.AName, IsRat);
				}
				else if (plant != null)
				{
					Object.Destroy(plant.gameObject);
				}
			}
			else if (CardZombieType != ZombieType.Nope)
			{
				InstantiatePlantZombie();
				if (SeedBank.Instance.CheckZombie(CardZombieType, gridByWorldPos, NeedSun, syn.AName))
				{
					SeedBank.Instance.ZombieConfirm(CardZombieType, zombie, gridByWorldPos, NeedSun, syn.AName, IsRat);
					SeedBank.Instance.LikeColumnPlace(gridByWorldPos, CardPlantType, CardZombieType, NeedSun, syn.AName, IsRat);
				}
				else if (zombie != null)
				{
					Object.Destroy(zombie.gameObject);
				}
			}
			plant = null;
			zombie = null;
			SeedBank.Instance.RemoveDropCard(this);
		}
		else if (syn.SynCode[0] == 2)
		{
			SeedBank.Instance.RemoveDropCard(this);
		}
	}

	private void UpdateOnlinePreview(Vector2 pos, PlantType type)
	{
		if (GameManager.Instance.isOnline)
		{
			PlantPreview plantPreview = new PlantPreview();
			plantPreview.plantType = type;
			plantPreview.GridPos = pos;
			plantPreview.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
			plantPreview.isImtor = isImitater;
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.ApplyPlacePreview(plantPreview);
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.PlacePreview(plantPreview, null);
			}
		}
	}

	private void UpdateOnlinePreview(Vector2 pos, ZombieType type)
	{
		if (GameManager.Instance.isOnline)
		{
			ZombiePreview zombiePreview = new ZombiePreview();
			zombiePreview.zombieType = type;
			zombiePreview.GridPos = pos;
			zombiePreview.isRat = IsRat;
			zombiePreview.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.ApplyZombiePreview(zombiePreview);
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.ZombiePreview(zombiePreview, null);
			}
		}
	}

	private void ClearInGrid()
	{
		if (plantInGrids.Count > 0)
		{
			for (int i = 0; i < plantInGrids.Count; i++)
			{
				if (plantInGrids[i].gameObject.activeSelf)
				{
					plantInGrids[i].Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
				}
				else
				{
					Object.Destroy(plantInGrids[i].gameObject);
				}
			}
			plantInGrids.Clear();
			UpdateOnlinePreview(default, PlantType.Nope);
		}
		if (zombieInGrids.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < zombieInGrids.Count; j++)
		{
			if (zombieInGrids[j].gameObject.activeSelf)
			{
				zombieInGrids[j].DirectDead(canDropItem: false, 0f, synClient: true);
			}
			else
			{
				Object.Destroy(zombieInGrids[j].gameObject);
			}
		}
		zombieInGrids.Clear();
		UpdateOnlinePreview(default, ZombieType.Nope);
	}

	private void planting(Grid grid)
	{
		if (plantInGrids.Count <= 0)
		{
			if (LV.Instance.LvSpStates.Contains(LVSpState.PlantLikeColumn))
			{
				InGridGrids = MapManager.Instance.GetColumnGrids(grid.Position);
				plantInGrids.Clear();
				for (int i = 0; i < InGridGrids.Count; i++)
				{
					plantInGrids.Add(PlantManager.Instance.GetNewPlant(CardPlantType));
					plantInGrids[i].InitForCreate(inGrid: true, InGridGrids[i], isImitater);
					plantInGrids[i].gameObject.SetActive(SeedBank.Instance.CheckPlant(plantInGrids[i], InGridGrids[i], NeedSun, GameManager.Instance.LocalPlayerSave.playerName));
				}
			}
			else
			{
				InGridGrids.Clear();
				InGridGrids.Add(grid);
				plantInGrids.Clear();
				plantInGrids.Add(PlantManager.Instance.GetNewPlant(CardPlantType));
				plantInGrids[0].InitForCreate(inGrid: true, grid, isImitater);
			}
			UpdateOnlinePreview(grid.Position, CardPlantType);
		}
		else if (LV.Instance.LvSpStates.Contains(LVSpState.PlantLikeColumn))
		{
			if (grid.Point.x != InGridGrids[0].Point.x || Mathf.Abs(grid.Position.y - InGridGrids[0].Position.y) > 15f)
			{
				InGridGrids = MapManager.Instance.GetColumnGrids(grid.Position);
				for (int j = 0; j < InGridGrids.Count; j++)
				{
					if (j < plantInGrids.Count)
					{
						plantInGrids[j].UpdateForCreate(InGridGrids[j]);
						plantInGrids[j].gameObject.SetActive(SeedBank.Instance.CheckPlant(plantInGrids[j], InGridGrids[j], NeedSun, GameManager.Instance.LocalPlayerSave.playerName));
					}
					else
					{
						plantInGrids.Add(PlantManager.Instance.GetNewPlant(CardPlantType));
						plantInGrids[j].InitForCreate(inGrid: true, InGridGrids[j], isImitater);
						plantInGrids[j].gameObject.SetActive(SeedBank.Instance.CheckPlant(plantInGrids[j], InGridGrids[j], NeedSun, GameManager.Instance.LocalPlayerSave.playerName));
					}
				}
				UpdateOnlinePreview(grid.Position, CardPlantType);
			}
		}
		else if (grid != InGridGrids[0])
		{
			InGridGrids[0] = grid;
			plantInGrids[0].UpdateForCreate(grid);
			UpdateOnlinePreview(grid.Position, CardPlantType);
		}
		if (!Input.GetMouseButtonDown(0) && (!GameManager.Instance.isAndroid || SeedBank.Instance.CardSelector || !Input.GetMouseButtonUp(0) || IsDropType))
		{
			return;
		}
		dontSelect = true;
		WantPlace = false;
		CanPlace = false;
		if (GameManager.Instance.isClient)
		{
			if (IsDropType)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Card;
				synItem.AName = GameManager.Instance.LocalPlayerSave.playerName;
				synItem.SynCode[0] = 1;
				synItem.Twofloat = grid.Position;
				SocketClient.Instance.SendSynBag(synItem);
			}
			else
			{
				PlantSpawn plantSpawn = new PlantSpawn();
				plantSpawn.plantType = plant.GetPlantType();
				plantSpawn.GridPos = grid.Position;
				plantSpawn.CardId = CardId;
				plantSpawn.SPcode = (isImitater ? 2 : 0);
				SocketClient.Instance.ApplyPlacePlant(plantSpawn);
			}
			plant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
		}
		else
		{
			if (GameManager.Instance.isServer && !IsDropType)
			{
				SocketServer.Instance.SendHostCD(CardId, isOK: false);
			}
			for (int k = 0; k < InGridGrids.Count; k++)
			{
				if (grid == InGridGrids[k])
				{
					SeedBank.Instance.PlantConfirm(plant, grid, NeedSun, isImitater ? 2 : 0, GameManager.Instance.LocalPlayerSave.playerName);
				}
				else if (SeedBank.Instance.CheckPlant(plantInGrids[k], InGridGrids[k], NeedSun, GameManager.Instance.LocalPlayerSave.playerName))
				{
					SeedBank.Instance.PlantConfirm(PlantManager.Instance.GetNewPlant(CardPlantType), InGridGrids[k], NeedSun, isImitater ? 2 : 0, GameManager.Instance.LocalPlayerSave.playerName);
				}
			}
		}
		PlantOver();
	}

	private void Zombieing(Grid grid)
	{
		if (zombieInGrids.Count <= 0)
		{
			if (LV.Instance.LvSpStates.Contains(LVSpState.PlantLikeColumn))
			{
				InGridGrids = MapManager.Instance.GetColumnGrids(grid.Position);
				zombieInGrids.Clear();
				for (int i = 0; i < InGridGrids.Count; i++)
				{
					zombieInGrids.Add(ZombieManager.Instance.GetNewZombie(CardZombieType));
					zombieInGrids[i].CreateInit(inGrid: true, InGridGrids[i], IsRat);
					zombieInGrids[i].gameObject.SetActive(SeedBank.Instance.CheckZombie(CardZombieType, InGridGrids[i], NeedSun, GameManager.Instance.LocalPlayerSave.playerName));
				}
			}
			else
			{
				InGridGrids.Clear();
				InGridGrids.Add(grid);
				zombieInGrids.Clear();
				zombieInGrids.Add(ZombieManager.Instance.GetNewZombie(CardZombieType));
				zombieInGrids[0].CreateInit(inGrid: true, grid, IsRat);
			}
			UpdateOnlinePreview(grid.Position, CardZombieType);
		}
		else if (LV.Instance.LvSpStates.Contains(LVSpState.PlantLikeColumn))
		{
			if (grid.Point.x != InGridGrids[0].Point.x || Mathf.Abs(grid.Position.y - InGridGrids[0].Position.y) > 15f)
			{
				InGridGrids = MapManager.Instance.GetColumnGrids(grid.Position);
				for (int j = 0; j < InGridGrids.Count; j++)
				{
					if (j < zombieInGrids.Count)
					{
						zombieInGrids[j].UpdateForCreate(InGridGrids[j]);
						zombieInGrids[j].gameObject.SetActive(SeedBank.Instance.CheckZombie(CardZombieType, InGridGrids[j], NeedSun, GameManager.Instance.LocalPlayerSave.playerName));
					}
					else
					{
						zombieInGrids.Add(ZombieManager.Instance.GetNewZombie(CardZombieType));
						zombieInGrids[j].CreateInit(inGrid: true, InGridGrids[j], IsRat);
						zombieInGrids[j].gameObject.SetActive(SeedBank.Instance.CheckZombie(CardZombieType, InGridGrids[j], NeedSun, GameManager.Instance.LocalPlayerSave.playerName));
					}
				}
				UpdateOnlinePreview(grid.Position, CardZombieType);
			}
		}
		else if (grid != InGridGrids[0])
		{
			InGridGrids[0] = grid;
			zombieInGrids[0].UpdateForCreate(grid);
			UpdateOnlinePreview(grid.Position, CardZombieType);
		}
		if (!Input.GetMouseButtonDown(0) && (!GameManager.Instance.isAndroid || SeedBank.Instance.CardSelector || !Input.GetMouseButtonUp(0) || IsDropType))
		{
			return;
		}
		dontSelect = true;
		WantPlace = false;
		CanPlace = false;
		if (GameManager.Instance.isClient)
		{
			ZombieSpawnApply zombieSpawnApply = new ZombieSpawnApply();
			zombieSpawnApply.Type = CardZombieType;
			zombieSpawnApply.GridPos = grid.Position;
			zombieSpawnApply.CardId = CardId;
			zombieSpawnApply.isRat = IsRat;
			SocketClient.Instance.ApplyPlaceZombie(zombieSpawnApply);
			zombie.DirectDead(canDropItem: false, 0f, synClient: true);
		}
		else
		{
			if (GameManager.Instance.isServer && !IsDropType)
			{
				SocketServer.Instance.SendHostCD(CardId, isOK: false);
			}
			for (int k = 0; k < InGridGrids.Count; k++)
			{
				if (grid == InGridGrids[k])
				{
					SeedBank.Instance.ZombieConfirm(CardZombieType, zombie, grid, NeedSun, GameManager.Instance.LocalPlayerSave.playerName, IsRat);
				}
				else if (SeedBank.Instance.CheckZombie(CardZombieType, InGridGrids[k], NeedSun, GameManager.Instance.LocalPlayerSave.playerName))
				{
					SeedBank.Instance.ZombieConfirm(CardZombieType, ZombieManager.Instance.GetNewZombie(CardZombieType), InGridGrids[k], NeedSun, GameManager.Instance.LocalPlayerSave.playerName, IsRat);
				}
			}
		}
		PlantOver();
	}

	private void PlantOver()
	{
		plant = null;
		zombie = null;
		if (!WantPlace)
		{
			ClearInGrid();
		}
		dontSelect = false;
		if (CardState == CardState.CanPlace && SelectorSelected)
		{
			WantPlaceThis();
		}
		if (IsDropType)
		{
			SeedBank.Instance.RemoveDropCard(this);
		}
	}

	public void CancelPlace()
	{
		SelectorSelected = false;
		if (WantPlace)
		{
			if (plant != null && plant.BasePlant != PlantType.Nope)
			{
				MapManager.Instance.PlantnoFlash(plant.BasePlant);
			}
			WantPlace = false;
			CheckState();
			if (plant != null)
			{
				plant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
				plant = null;
			}
			if (zombie != null)
			{
				zombie.DirectDead(canDropItem: false, 0f, synClient: true);
				zombie = null;
			}
			ClearInGrid();
		}
	}

	public void CDTo(float cd)
	{
		if (CdCoroutine != null)
		{
			StopCoroutine(CdCoroutine);
		}
		CdCoroutine = null;
		CanPlace = false;
		currTimeForCd = cd;
	}

	private void OnMouseOver()
	{
		if (MyTool.IsPointerOverGameObject() || !SeedBank.Instance.isCanClick || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		if (isStarted && isChoosed)
		{
			if (cardState != CardState.CanPlace)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
				return;
			}
			if (WantPlace)
			{
				GoCancelPlant();
			}
			else
			{
				if (!IsDropType)
				{
					SeedBank.Instance.ClickSelect(this);
				}
				WantPlaceThis();
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GetPlant, base.transform.position, isAll: true);
			}
			secondClick = false;
		}
		else if (!isStarted && isChoosed && (CardPlantType != PlantType.Nope || CardZombieType != ZombieType.Nope))
		{
			if (CanUnChoose)
			{
				ClearChoose();
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
			}
		}
	}

	public void WantPlaceThis()
	{
		secondClick = true;
		SelectorSelected = true;
		if (!isStarted)
		{
			return;
		}
		if (!WantPlace)
		{
			SeedBank.Instance.AllCancelPlace(NoSelect: false);
		}
		SelectorSelected = true;
		if (isChoosed)
		{
			if (cardState == CardState.CanPlace)
			{
				if (!WantPlace)
				{
					WantPlace = true;
					Shovel.Instance.CancelShovel();
					Glove.Instance.CancelShovel();
				}
				if (plant != null && plant.BasePlant != PlantType.Nope)
				{
					MapManager.Instance.PlantFlash(plant.BasePlant);
				}
			}
		}
		else
		{
			SeedBank.Instance.CancelSelect();
		}
	}

	public void AddChoose(UIPlantCardNC nC)
	{
		if (nC == null)
		{
			return;
		}
		isImitater = false;
		grayImg.material.SetInt("_OpenGray", 0);
		ImgRenderer.material.SetInt("_OpenGray", 0);
		myPlantCard = nC;
		isChoosed = true;
		if (nC.CardPlantType == PlantType.Imitater)
		{
			isImitater = true;
			grayImg.material.SetInt("_OpenGray", 1);
			ImgRenderer.material.SetInt("_OpenGray", 1);
			PlantCard preCard = SeedBank.Instance.GetPreCard(this);
			if (preCard != null)
			{
				UIPlantCardNC plantNc = SeedBank.Instance.GetPlantNc(preCard.CardPlantType);
				if (plantNc == null || PlantManager.Instance.IsZombiePlant(plantNc.CardPlantType))
				{
					myPlantCard = SeedChooser.Instance.GetCardInfo(1);
				}
				else
				{
					myPlantCard = plantNc;
				}
			}
			else
			{
				myPlantCard = SeedChooser.Instance.GetCardInfo(1);
			}
		}
		CDTime = myPlantCard.CDTime;
		NeedSun = myPlantCard.NeedNum;
		isNeedSun = myPlantCard.isNeedSun;
		CardPlantType = myPlantCard.CardPlantType;
		CardZombieType = myPlantCard.CardZombieType;
		grayImg.sprite = myPlantCard.OwnerSprite;
		ImgRenderer.sprite = myPlantCard.OwnerSprite;
		WantSunText.text = myPlantCard.NeedNum.ToString();
		if (isImitater)
		{
			myPlantCard = SeedBank.Instance.GetPlantNc(PlantType.Imitater);
		}
		if (IsDropType)
		{
			NeedSun = 0;
			WantSunText.text = "";
		}
		else if (LV.Instance.LvSpStates.Contains(LVSpState.FreeDay))
		{
			NeedSun = 0;
			WantSunText.text = "0";
		}
	}

	public void ClearChoose()
	{
		SeedBank.Instance.ClearChoose(myPlantCard, this);
		ImgRenderer.sprite = sprite;
		grayImg.sprite = sprite;
		WantSunText.text = "";
		isChoosed = false;
		if (GameManager.Instance.isClient)
		{
			SelectCard selectCard = new SelectCard();
			selectCard.plantType = CardPlantType;
			selectCard.zombieType = CardZombieType;
			selectCard.isBack = true;
			selectCard.cardId = CardId;
			SocketClient.Instance.SelectCard(selectCard);
		}
		if (GameManager.Instance.isServer)
		{
			SelectCard selectCard2 = new SelectCard();
			selectCard2.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
			selectCard2.plantType = CardPlantType;
			selectCard2.zombieType = CardZombieType;
			selectCard2.isBack = true;
			selectCard2.cardId = CardId;
			SocketServer.Instance.SelectCard(selectCard2);
		}
	}

	public void ClearChoose(bool noAnimation)
	{
		if (myPlantCard != null)
		{
			myPlantCard.IsChoosed = false;
		}
	}

	public void ClearChooseOk()
	{
		CDTime = 0f;
		NeedSun = 0;
		myPlantCard.IsChoosed = false;
		CardPlantType = PlantType.Nope;
	}

	public void DestroyCardSlot()
	{
		if (GameManager.Instance.isServer && IsDropType)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Card;
			synItem.SynCode[0] = 2;
			SocketServer.Instance.SendSynBag(synItem);
		}
		ClearInGrid();
		CancelPlace();
		if (CdCoroutine != null)
		{
			StopCoroutine(CdCoroutine);
		}
		Object.Destroy(base.gameObject);
	}
}
