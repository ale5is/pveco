using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SeedBank : MonoBehaviour
{
	public int CardNum = 4;

	private int DecidedCardNum;

	private int choosedNum;

	public bool isFull;

	public bool isCanClick;

	public bool isNoCD;

	public GameObject SunPos;

	public GameObject MoonPos;

	public GameObject MoonBank;

	public GameObject Selector;

	public GameObject SelectorOff;

	public Sprite SunBankSpite;

	public Sprite MoonBankSpite;

	public Sprite SunAndMoonBankSpite;

	private SpriteRenderer seedBank;

	public TextMesh sunNumText;

	public TextMesh moonNumText;

	private List<PlantCard> slotList = new List<PlantCard>();

	private List<PlantCard> DropCards = new List<PlantCard>();

	public Sprite NoCardSprite;

	public static SeedBank Instance;

	private int currOrderNum = 14;

	public bool IsCdDown;

	public bool CardSelector = true;

	[SerializeField]
	private int currSelectedId;

	public List<UIPlantCardAnimation> uICardAnims = new List<UIPlantCardAnimation>();

	public List<CardType> LastSelectCard = new List<CardType>();

	public bool NeedSummonSun
	{
		get
		{
			if (LV.Instance.CurrBankType == BankType.Normal || LV.Instance.CurrBankType == BankType.SlotMachine)
			{
				if (LV.Instance.CurrSeedBankType != SeedBankType.SunBank)
				{
					return LV.Instance.CurrSeedBankType == SeedBankType.SunAndMoonBank;
				}
				return true;
			}
			return false;
		}
	}

	public bool NeedSummonMoon
	{
		get
		{
			if (LV.Instance.CurrBankType == BankType.Normal || LV.Instance.CurrBankType == BankType.SlotMachine)
			{
				if (LV.Instance.CurrSeedBankType != SeedBankType.MoonBank)
				{
					return LV.Instance.CurrSeedBankType == SeedBankType.SunAndMoonBank;
				}
				return true;
			}
			return false;
		}
	}

	public int noBasePlantExtra => 50;

	public int ChoosedNum
	{
		get
		{
			return choosedNum;
		}
		set
		{
			choosedNum = value;
			if (choosedNum >= CardNum)
			{
				isFull = true;
			}
			else
			{
				isFull = false;
			}
		}
	}

	public int CurrOrderNum
	{
		get
		{
			return currOrderNum;
		}
		set
		{
			currOrderNum = value;
			if (value > FixedInfo.PlantBack)
			{
				currOrderNum = FixedInfo.PlantFront;
			}
		}
	}

	private int CurrSelectedId
	{
		get
		{
			return currSelectedId;
		}
		set
		{
			if (!CardSelector || Time.timeScale == 0f || slotList.Count == 0)
			{
				return;
			}
			if (HaveSelect())
			{
				currSelectedId = value;
				if (currSelectedId > slotList.Count - 1)
				{
					currSelectedId = 0;
				}
				if (currSelectedId < 0)
				{
					currSelectedId = slotList.Count - 1;
				}
				if (currSelectedId < 0)
				{
					currSelectedId = 0;
				}
			}
			Selector.transform.localScale = Vector3.one;
			SelectorOff.transform.localScale = Vector3.zero;
			slotList[currSelectedId].WantPlaceThis();
			Selector.transform.localPosition = slotList[currSelectedId].transform.localPosition;
			SelectorOff.transform.localPosition = slotList[currSelectedId].transform.localPosition;
		}
	}

	private bool HaveSelect()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].SelectorSelected)
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		seedBank = base.transform.GetComponent<SpriteRenderer>();
		sunNumText = base.transform.Find("SunNumText").GetComponent<TextMesh>();
		isFull = false;
		isCanClick = true;
	}

	private void Update()
	{
		if (Time.timeScale != 0f && CardSelector)
		{
			if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				CurrSelectedId--;
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				CurrSelectedId++;
			}
		}
	}

	public void UpdateSeedBankType(SeedBankType type)
	{
		UIManager.Instance.SetChooserType(type);
		switch (type)
		{
		case SeedBankType.SunBank:
			seedBank.sprite = SunBankSpite;
			MoonBank.transform.localScale = Vector3.zero;
			MoonBank.transform.localPosition = new Vector3(0.65f, MoonBank.transform.localPosition.y);
			break;
		case SeedBankType.MoonBank:
			seedBank.sprite = MoonBankSpite;
			MoonBank.transform.localScale = Vector3.one;
			MoonBank.transform.localPosition = new Vector3(0.65f, MoonBank.transform.localPosition.y);
			break;
		case SeedBankType.SunAndMoonBank:
			seedBank.sprite = SunAndMoonBankSpite;
			MoonBank.transform.localScale = Vector3.one;
			MoonBank.transform.localPosition = new Vector3(-0.5f, MoonBank.transform.localPosition.y);
			break;
		}
	}

	public bool AllNoSunPlace()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].isChoosed && (slotList[i].CardState == CardState.CanPlace || slotList[i].CardState == CardState.NotCD))
			{
				return false;
			}
		}
		return true;
	}

	public void AllDropBox(bool enable)
	{
		for (int i = 0; i < DropCards.Count; i++)
		{
			DropCards[i].boxCollider.enabled = enable;
		}
	}

	public void UpdateSunNum(string sunNum)
	{
		sunNumText.text = sunNum;
	}

	public void UpdateMoonNum(string moonNum)
	{
		moonNumText.text = moonNum;
	}

	public UIPlantCardNC GetPlantNc(PlantType type)
	{
		UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(type);
		if (cardInfo == null)
		{
			cardInfo = ZombieChooser.Instance.GetCardInfo(type);
		}
		return cardInfo;
	}

	public UIPlantCardNC GetZombieNc(ZombieType type)
	{
		return ZombieChooser.Instance.GetCardInfo(type);
	}

	public void SynDropCard(SynItem syn)
	{
		for (int i = 0; i < DropCards.Count; i++)
		{
			if (DropCards[i].OnlineId == syn.OnlineId)
			{
				DropCards[i].OnlineSyn(syn);
			}
		}
	}

	private IEnumerator RainCardSpawner()
	{
		float pp = 12 - MapManager.Instance.mapList.Count * 2;
		if (pp < 6f)
		{
			pp = 6f;
		}
		do
		{
			List<CardType> cards = new List<CardType>(LV.Instance.GeneralCardPool);
			int num = cards.Count / 5;
			for (int i = 0; i < num; i++)
			{
				cards.Add(LV.Instance.GeneralCardPool[Random.Range(0, LV.Instance.GeneralCardPool.Count)]);
			}
			cards.Shuffle();
			for (int j = 0; j < cards.Count; j++)
			{
				float seconds = Random.Range(2f, pp - (float)SkyManager.Instance.RainScale * 0.3f);
				yield return new WaitForSeconds(seconds);
				if (!LVManager.Instance.BootyIsAppeared)
				{
					CardType cardType = LV.Instance.GeneralCardPool[Random.Range(0, LV.Instance.GeneralCardPool.Count)];
					SpawnSkyCard(cardType.plantType, cardType.zombieType);
				}
			}
		}
		while (!LVManager.Instance.BootyIsAppeared);
	}

	public void ClientSpawnCard(CardSpawn cardSpawn)
	{
		if (cardSpawn.spawnType == SpawnCardType.Drop)
		{
			SpawnDropCard(cardSpawn.cardType.plantType, cardSpawn.cardType.zombieType, cardSpawn.SpaPos, cardSpawn.TwoFloat.x == 0f, cardSpawn.TwoFloat.y).OnlineId = cardSpawn.OnlineId;
		}
		else if (cardSpawn.spawnType == SpawnCardType.FromSky)
		{
			SpawnSkyCard(cardSpawn.cardType.plantType, cardSpawn.cardType.zombieType, cardSpawn.SpaPos).OnlineId = cardSpawn.OnlineId;
		}
		else if (cardSpawn.spawnType == SpawnCardType.ConveyerBelt)
		{
			SpawnBeltCard(cardSpawn.cardType.plantType, cardSpawn.cardType.zombieType, synClient: true).OnlineId = cardSpawn.OnlineId;
		}
	}

	public void RemoveDropCard(PlantCard card)
	{
		ConveyorBelt.Instance.RemoveCard(card);
		DropCards.Remove(card);
		card.DestroyCardSlot();
	}

	public void SpawnDropCard(PlantType type, ZombieType zombieType, Vector2 pos)
	{
		if (!GameManager.Instance.isClient)
		{
			bool flag = Random.Range(0, 2) == 0;
			float num = Random.Range(0.2f, 0.8f);
			PlantCard plantCard = SpawnDropCard(type, zombieType, pos, flag, num);
			if (GameManager.Instance.isServer)
			{
				plantCard.OnlineId = SocketServer.Instance.ItemId;
				CardSpawn cardSpawn = new CardSpawn();
				cardSpawn.OnlineId = plantCard.OnlineId;
				cardSpawn.spawnType = SpawnCardType.Drop;
				cardSpawn.cardType = new CardType(type, zombieType);
				cardSpawn.SpaPos = pos;
				cardSpawn.TwoFloat = new Vector2((!flag) ? 1 : 0, num);
				SocketServer.Instance.SpawnDropCard(cardSpawn);
			}
		}
	}

	private PlantCard SpawnDropCard(PlantType type, ZombieType zombieType, Vector2 pos, bool isLeft, float x)
	{
		PlantCard component = Object.Instantiate(GameManager.Instance.GameConf.CardSlot).GetComponent<PlantCard>();
		component.InitForDrop(pos, isLeft, x);
		if (type != PlantType.Nope)
		{
			component.AddChoose(GetPlantNc(type));
		}
		else
		{
			component.AddChoose(GetZombieNc(zombieType));
		}
		component.transform.position = new Vector3(pos.x, pos.y, -1f);
		DropCards.Add(component);
		return component;
	}

	public void SpawnSkyCard(PlantType type, ZombieType zombieType)
	{
		if (!GameManager.Instance.isClient)
		{
			Vector3 vector = MapManager.Instance.GetRandomGrid().Position + new Vector2(Random.Range(-0.7f, 0.7f), Random.Range(-0.7f, 0f));
			PlantCard plantCard = SpawnSkyCard(type, zombieType, vector);
			if (GameManager.Instance.isServer)
			{
				plantCard.OnlineId = SocketServer.Instance.ItemId;
				CardSpawn cardSpawn = new CardSpawn();
				cardSpawn.OnlineId = plantCard.OnlineId;
				cardSpawn.spawnType = SpawnCardType.FromSky;
				cardSpawn.cardType = new CardType(type, zombieType);
				cardSpawn.SpaPos = vector;
				SocketServer.Instance.SpawnDropCard(cardSpawn);
			}
		}
	}

	private PlantCard SpawnSkyCard(PlantType type, ZombieType zombieType, Vector3 gridPos)
	{
		PlantCard component = Object.Instantiate(GameManager.Instance.GameConf.CardSlot).GetComponent<PlantCard>();
		component.InitForSky(gridPos.y, new Vector3(gridPos.x, MapManager.Instance.GetMapYHighest(gridPos)));
		if (type != PlantType.Nope)
		{
			component.AddChoose(GetPlantNc(type));
		}
		else
		{
			component.AddChoose(GetZombieNc(zombieType));
		}
		DropCards.Add(component);
		return component;
	}

	public PlantCard SpawnBeltCard(PlantType type, ZombieType zombieType, bool synClient = false)
	{
		if (GameManager.Instance.isClient && !synClient)
		{
			return null;
		}
		PlantCard component = Object.Instantiate(GameManager.Instance.GameConf.CardSlot).GetComponent<PlantCard>();
		component.InitForBelt();
		if (type != PlantType.Nope)
		{
			component.AddChoose(GetPlantNc(type));
		}
		else
		{
			component.AddChoose(GetZombieNc(zombieType));
		}
		DropCards.Add(component);
		ConveyorBelt.Instance.AddCard(component);
		if (GameManager.Instance.isServer)
		{
			component.OnlineId = SocketServer.Instance.ItemId;
			CardSpawn cardSpawn = new CardSpawn();
			cardSpawn.OnlineId = component.OnlineId;
			cardSpawn.spawnType = SpawnCardType.ConveyerBelt;
			cardSpawn.cardType = new CardType(type, zombieType);
			SocketServer.Instance.SpawnDropCard(cardSpawn);
		}
		return component;
	}

	public void SpawnCardSlot()
	{
		ClearCardSlot();
		isFull = false;
		if (CardNum == 0)
		{
			isFull = true;
			seedBank.size = new Vector2(4.9f, seedBank.size.y);
		}
		else
		{
			seedBank.size = new Vector2(4.9f + (float)(CardNum - 4) * 0.85f, seedBank.size.y);
		}
		if (seedBank.size.x < 5.8f)
		{
			seedBank.size = new Vector2(5.8f, seedBank.size.y);
		}
		for (int i = 0; i < CardNum; i++)
		{
			PlantCard component = Object.Instantiate(GameManager.Instance.GameConf.CardSlot).GetComponent<PlantCard>();
			component.CardSlotInit();
			component.transform.SetParent(Instance.transform);
			component.transform.localPosition = new Vector3(1.75f + 0.85f * (float)i, 0f);
			component.transform.localScale = new Vector3(0.8f, 0.8f);
			component.CardId = i;
			slotList.Add(component);
		}
		if (CardSelector && slotList.Count > 0)
		{
			CurrSelectedId = 0;
			Selector.transform.localPosition = slotList[0].transform.localPosition;
		}
		else
		{
			Selector.transform.localScale = Vector3.zero;
			SelectorOff.transform.localScale = Vector3.zero;
		}
	}

	public void SaveSelectedCard()
	{
		LastSelectCard.Clear();
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardPlantType != PlantType.Nope || slotList[i].CardZombieType != ZombieType.Nope)
			{
				if (slotList[i].isImitater)
				{
					LastSelectCard.Add(new CardType(PlantType.Imitater, slotList[i].CardZombieType));
				}
				else
				{
					LastSelectCard.Add(new CardType(slotList[i].CardPlantType, slotList[i].CardZombieType));
				}
			}
		}
		GameManager.Instance.SaveUserInfo();
	}

	public void LoadLastCard()
	{
		for (int i = 0; i < LastSelectCard.Count; i++)
		{
			if (LastSelectCard[i].plantType != PlantType.Nope)
			{
				if (LV.Instance.CurrSeedBankType == SeedBankType.SunBank)
				{
					UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(LastSelectCard[i].plantType);
					if (cardInfo != null)
					{
						cardInfo.ClickThis(haveSound: false);
					}
				}
				else if (LV.Instance.CurrSeedBankType == SeedBankType.SunAndMoonBank)
				{
					GetPlantNc(LastSelectCard[i].plantType).ClickThis(haveSound: false);
				}
			}
			else
			{
				if (LastSelectCard[i].zombieType == ZombieType.Nope)
				{
					continue;
				}
				if (LV.Instance.CurrSeedBankType == SeedBankType.MoonBank)
				{
					UIPlantCardNC cardInfo2 = ZombieChooser.Instance.GetCardInfo(LastSelectCard[i].zombieType);
					if (cardInfo2 != null)
					{
						cardInfo2.ClickThis(haveSound: false);
					}
				}
				else if (LV.Instance.CurrSeedBankType == SeedBankType.SunAndMoonBank)
				{
					GetZombieNc(LastSelectCard[i].zombieType).ClickThis(haveSound: false);
				}
			}
		}
	}

	public void ClickSelect(PlantCard card)
	{
		int num = slotList.IndexOf(card);
		slotList[currSelectedId].WantPlaceThis();
		CurrSelectedId = num;
	}

	public void CancelSelect()
	{
		if (CardSelector)
		{
			Selector.transform.localScale = Vector3.zero;
			SelectorOff.transform.localScale = Vector3.one;
		}
	}

	public void PlantFailClearCD(int cardID)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardId == cardID)
			{
				slotList[i].currTimeForCd = 0f;
				break;
			}
		}
	}

	public void ChangeCard(int plantId, int cardId)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (i == cardId - 1)
			{
				UIPlantCardNC cardInfo = SeedChooser.Instance.GetCardInfo(plantId);
				slotList[i].AddChoose(cardInfo);
				slotList[i].currTimeForCd = 0f;
			}
		}
	}

	public void AddCards(List<CardType> cardTypes, bool canAddSlot)
	{
		for (int i = 0; i < cardTypes.Count; i++)
		{
			AddCard(cardTypes[i].plantType, cardTypes[i].zombieType, 0f, CanUnChoose: false, canAddSlot);
			UIPlantCardNC uIPlantCardNC = GetPlantNc(cardTypes[i].plantType);
			if (uIPlantCardNC == null)
			{
				uIPlantCardNC = GetZombieNc(cardTypes[i].zombieType);
			}
			uIPlantCardNC.IsChoosed = true;
			SendSelect(cardTypes[i].plantType, cardTypes[i].zombieType);
		}
	}

	private void SendSelect(PlantType CardPlantType, ZombieType CardZombieType)
	{
		if (GameManager.Instance.isClient)
		{
			SelectCard selectCard = new SelectCard();
			selectCard.plantType = CardPlantType;
			selectCard.zombieType = CardZombieType;
			selectCard.isBack = false;
			selectCard.noAnim = true;
			SocketClient.Instance.SelectCard(selectCard);
		}
		if (GameManager.Instance.isServer)
		{
			SelectCard selectCard2 = new SelectCard();
			selectCard2.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
			selectCard2.plantType = CardPlantType;
			selectCard2.zombieType = CardZombieType;
			selectCard2.isBack = false;
			selectCard2.noAnim = true;
			SocketServer.Instance.SelectCard(selectCard2);
		}
	}

	public void AddCard(PlantType type, ZombieType type1, float CurrCd, bool CanUnChoose, bool canAddSlot)
	{
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			return;
		}
		UIPlantCardNC uIPlantCardNC = null;
		if (type != PlantType.Nope)
		{
			uIPlantCardNC = GetPlantNc(type);
		}
		else if (type1 != ZombieType.Nope)
		{
			uIPlantCardNC = GetZombieNc(type1);
		}
		if (uIPlantCardNC == null)
		{
			return;
		}
		ChoosedNum++;
		uIPlantCardNC.IsChoosed = true;
		if (isNoCD)
		{
			CurrCd = 0f;
		}
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardPlantType == PlantType.Nope && slotList[i].CardZombieType == ZombieType.Nope)
			{
				slotList[i].AddChoose(uIPlantCardNC);
				slotList[i].CDTo(CurrCd);
				slotList[i].CanUnChoose = CanUnChoose;
				return;
			}
		}
		if (canAddSlot && slotList.Count < 15)
		{
			seedBank.size = new Vector2(4.9f + (float)(slotList.Count + 1 - 4) * 0.85f, seedBank.size.y);
			PlantCard component = Object.Instantiate(GameManager.Instance.GameConf.CardSlot).GetComponent<PlantCard>();
			component.CardSlotInit();
			component.transform.SetParent(Instance.transform);
			component.transform.localPosition = new Vector3(1.75f + 0.85f * (float)slotList.Count, 0f);
			component.transform.localScale = new Vector3(0.8f, 0.8f);
			component.CardId = slotList.Count;
			slotList.Add(component);
			component.AddChoose(uIPlantCardNC);
			component.CDTo(CurrCd);
			component.CanUnChoose = CanUnChoose;
		}
	}

	public void ClearCardSlot()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i] != null)
			{
				if (slotList[i].isChoosed)
				{
					slotList[i].ClearChoose(noAnimation: true);
					ChoosedNum--;
				}
				slotList[i].DestroyCardSlot();
			}
		}
		for (int j = 0; j < DropCards.Count; j++)
		{
			if (DropCards[j] != null)
			{
				DropCards[j].DestroyCardSlot();
			}
		}
		for (int k = 0; k < uICardAnims.Count; k++)
		{
			if (uICardAnims[k] != null)
			{
				uICardAnims[k].DestroyThis();
			}
		}
		ConveyorBelt.Instance.BeltCards.Clear();
		ChoosedNum = 0;
		uICardAnims.Clear();
		slotList.Clear();
		DropCards.Clear();
		isFull = false;
		currSelectedId = 0;
		PlayerManager.Instance.ClearCardUpdateEvent();
	}

	public void AllCancelPlace(bool NoSelect)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			slotList[i].CancelPlace();
		}
		for (int j = 0; j < DropCards.Count; j++)
		{
			DropCards[j].CancelPlace();
		}
		if (NoSelect)
		{
			CancelSelect();
		}
	}

	public bool ChooseCard(UIPlantCardNC nC)
	{
		if (slotList.Count < 1)
		{
			return false;
		}
		for (int i = 0; i < slotList.Count; i++)
		{
			if (!slotList[i].isChoosed)
			{
				DecidedCardNum = i;
				break;
			}
		}
		slotList[DecidedCardNum].isChoosed = true;
		ChoosedNum++;
		Vector2 vector = Camera.main.WorldToScreenPoint(slotList[DecidedCardNum].transform.position);
		UIPlantCardAnimation component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CardSlotAnimation).GetComponent<UIPlantCardAnimation>();
		component.transform.SetParent(UIManager.Instance.transform);
		component.CreateInit(nC.transform.position, nC, slotList[DecidedCardNum]);
		component.PlayChooseAnimation(vector, isBack: false);
		return true;
	}

	public PlantCard GetPreCard(PlantCard pC)
	{
		int num = slotList.IndexOf(pC);
		if (num <= 0)
		{
			return null;
		}
		return slotList[num - 1];
	}

	public void ClearChoose(UIPlantCardNC nC, PlantCard pC)
	{
		ChoosedNum--;
		UIPlantCardAnimation component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CardSlotAnimation).GetComponent<UIPlantCardAnimation>();
		component.transform.SetParent(UIManager.Instance.transform);
		Vector2 initPos = Camera.main.WorldToScreenPoint(pC.transform.position);
		component.CreateInit(initPos, nC, pC);
		component.PlayChooseAnimation(nC.transform.position, isBack: true);
	}

	public bool ClearCD(PlantType type)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardPlantType == type && slotList[i].currTimeForCd > 0f)
			{
				slotList[i].currTimeForCd = 0f;
				if (GameManager.Instance.isServer)
				{
					SocketServer.Instance.SendHostCD(slotList[i].CardId, isOK: true);
				}
				if (GameManager.Instance.isClient)
				{
					SocketClient.Instance.UpdateCD(slotList[i].CardId, Ok: true);
				}
				return true;
			}
		}
		return false;
	}

	public void StartAllCD()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			slotList[i].CanPlace = false;
		}
	}

	public void ClearAllCD()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			slotList[i].currTimeForCd = 0f;
		}
	}

	public void LikeColumnPlace(Grid grid, PlantType plantType, ZombieType zombieType, int SPCode, string Name, bool isRat)
	{
		if (!LV.Instance.LvSpStates.Contains(LVSpState.PlantLikeColumn))
		{
			return;
		}
		List<Grid> columnGrids = MapManager.Instance.GetColumnGrids(grid.Position);
		for (int i = 0; i < columnGrids.Count; i++)
		{
			if (grid == columnGrids[i])
			{
				continue;
			}
			if (plantType != PlantType.Nope)
			{
				PlantBase newPlant = PlantManager.Instance.GetNewPlant(plantType);
				if (CheckPlant(newPlant, columnGrids[i], -1, Name))
				{
					PlantConfirm(newPlant, columnGrids[i], -1, SPCode, Name);
				}
				else
				{
					Object.Destroy(newPlant.gameObject);
				}
			}
			else if (zombieType != ZombieType.Nope)
			{
				ZombieBase newZombie = ZombieManager.Instance.GetNewZombie(zombieType);
				newZombie.CreateInit(inGrid: false, null, isRat);
				if (CheckZombie(zombieType, grid, -1, Name))
				{
					ZombieConfirm(zombieType, newZombie, grid, -1, Name, isRat);
				}
				else
				{
					Object.Destroy(newZombie.gameObject);
				}
			}
		}
	}

	public bool CheckPlant(PlantBase plant, Grid grid, int NeedSun, string PlacePlayer)
	{
		if (grid == null)
		{
			return false;
		}
		if (NeedSun == -1)
		{
			UIPlantCardNC plantNc = GetPlantNc(plant.GetPlantType());
			if (plantNc == null)
			{
				plantNc = ZombieChooser.Instance.GetCardInfo(plant.GetPlantType());
				NeedSun = plantNc.NeedNum;
			}
			else
			{
				NeedSun = plantNc.NeedNum;
			}
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.FreeDay))
		{
			NeedSun = 0;
		}
		if (plant.IsZombiePlant)
		{
			if (PlayerManager.Instance.GetSunNum(isSun: false, PlacePlayer) < (float)NeedSun)
			{
				return false;
			}
		}
		else if (PlayerManager.Instance.GetSunNum(isSun: true, PlacePlayer) < (float)NeedSun)
		{
			return false;
		}
		if (plant != null)
		{
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				if (grid.CurrGridType != TeamType.Normal)
				{
					bool flag = PvPSelector.Instance.RedTeamNames.Contains(PlacePlayer);
					if (flag && grid.CurrGridType == TeamType.BlueTeam)
					{
						return false;
					}
					if (!flag && grid.CurrGridType == TeamType.RedTeam)
					{
						return false;
					}
				}
				if (grid.CurrPlantBase != null && !PvPSelector.Instance.IsSameTeam(PlacePlayer, grid.CurrPlantBase.PlacePlayer))
				{
					return false;
				}
			}
			if (grid.IceRoadNum > 0)
			{
				return false;
			}
			if (grid.Vases.Count > 0)
			{
				return false;
			}
			if (grid.HaveCrater)
			{
				return false;
			}
			if (grid.isOccupied || grid.isEmpty)
			{
				return false;
			}
			if (plant.GetPlantType() == PlantType.Gravebuster)
			{
				if (grid.HaveGraveStone && grid.CurrPlantBase == null)
				{
					return true;
				}
				return false;
			}
			if (grid.HaveGraveStone)
			{
				return false;
			}
			if (!plant.isHaveSpecialCheck)
			{
				if (plant.BasePlant == PlantType.Nope)
				{
					return NormalCheck(plant, grid, PlacePlayer);
				}
				if (grid.CurrPlantBase != null)
				{
					if (grid.CurrPlantBase.GetPlantType() == plant.BasePlant && grid.CurrPlantBase.CarryPlant == null)
					{
						return true;
					}
					if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() == plant.BasePlant)
					{
						return true;
					}
				}
				if (plant.BasePlantSunNum < 0 || NeedSun == -2)
				{
					return NormalCheck(plant, grid, PlacePlayer);
				}
				int num = NeedSun + plant.BasePlantSunNum + Instance.noBasePlantExtra;
				if (PlayerManager.Instance.GetSunNum(isSun: true, PlacePlayer) >= (float)num)
				{
					return NormalCheck(plant, grid, PlacePlayer);
				}
			}
			else if (plant.SpecialPlantCheck(grid, NeedSun, PlacePlayer))
			{
				return true;
			}
		}
		return false;
	}

	private bool NormalCheck(PlantBase plant, Grid grid, string PlacePlayer)
	{
		if (grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase.isHypno)
			{
				return false;
			}
			if (CanUseFirstAidNut(plant.GetPlantType()) && (GameManager.Instance.LocalPlayerSave.SpItems.Contains(SpItem.FirstAidNut) || PlacePlayer != GameManager.Instance.LocalPlayerSave.playerName))
			{
				if (grid.CurrPlantBase.GetPlantType() == plant.GetPlantType())
				{
					if (grid.CurrPlantBase.Hp <= grid.CurrPlantBase.MaxHp / 3f * 2f)
					{
						return true;
					}
				}
				else if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() == plant.GetPlantType())
				{
					if (grid.CurrPlantBase.CarryPlant.Hp <= grid.CurrPlantBase.CarryPlant.MaxHp / 3f * 2f)
					{
						return true;
					}
				}
				else if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.ProtectPlant.GetPlantType() == plant.GetPlantType() && grid.CurrPlantBase.ProtectPlant.Hp <= grid.CurrPlantBase.ProtectPlant.MaxHp / 3f * 2f)
				{
					return true;
				}
			}
			if (!plant.CanCarryed && !plant.isProtectPlant)
			{
				return false;
			}
			if (grid.CurrPlantBase.CanPlaceOnWater && grid.CurrPlantBase.CanCarryOtherPlant && !plant.CanPlaceOnWaterCarry)
			{
				return false;
			}
			if (grid.CurrPlantBase.CanCarryOtherPlant && !plant.CanCarryOtherPlant && grid.CurrPlantBase.CarryPlant == null && !plant.isProtectPlant)
			{
				return true;
			}
			if (plant.isProtectPlant && grid.CurrPlantBase.CanProtect && grid.CurrPlantBase.ProtectPlant == null && (grid.CurrPlantBase.CarryPlant == null || (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.CanProtect)))
			{
				return true;
			}
			if (grid.CurrPlantBase.isProtectPlant && plant.CanProtect && plant.CanPlaceOnGrass)
			{
				return true;
			}
			if (!grid.CurrPlantBase.CanCarryOtherPlant && plant.CanCarryOtherPlant && grid.CurrPlantBase.CanCarryed && grid.CurrPlantBase.GetPlantType() != PlantType.CobCannon)
			{
				if (plant.CanPlaceOnWater && (grid.isHavePuddle || grid.isWaterGrid) && !grid.CurrPlantBase.CanPlaceOnWater && grid.CurrPlantBase.GetPlantType() != PlantType.PotatoMine)
				{
					return true;
				}
				if (plant.CanPlaceOnGrass && !grid.isWaterGrid)
				{
					return true;
				}
			}
		}
		else
		{
			if (!grid.isWaterGrid && !grid.isHavePuddle && !grid.isHardGrid && plant.CanPlaceOnGrass)
			{
				return true;
			}
			if (grid.isWaterGrid && !grid.IsIce && plant.CanPlaceOnWater)
			{
				return true;
			}
			if ((grid.isHardGrid || (grid.isWaterGrid && grid.IsIce)) && plant.CanPlaceOnHardGround)
			{
				return true;
			}
			if (grid.isHavePuddle && plant.CanPlaceOnPuddle)
			{
				return true;
			}
		}
		return false;
	}

	public void PlantConfirm(PlantBase plant, Grid grid, int NeedSun, int SPcode, string PlacePlayer)
	{
		if (grid == null)
		{
			return;
		}
		plant.PlacePlayer = PlacePlayer;
		if (GameManager.Instance.isServer)
		{
			PlantSpawn plantSpawn = new PlantSpawn();
			plantSpawn.OnlineId = SocketServer.Instance.ItemId;
			plantSpawn.plantType = plant.GetPlantType();
			plantSpawn.PlacePlayer = PlacePlayer;
			plantSpawn.GridPos = grid.Position;
			plant.OnlineId = plantSpawn.OnlineId;
			plantSpawn.SPcode = SPcode;
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
			{
				plantSpawn.GridPos = new Vector2(0f - grid.Position.x, grid.Position.y);
			}
			SocketServer.Instance.SpawnPlant(plantSpawn);
		}
		if (SPcode == 2)
		{
			Imitater component = PlantManager.Instance.GetNewPlant(PlantType.Imitater).GetComponent<Imitater>();
			component.CopyPlantInfo(plant);
			component.PlacePlayer = PlacePlayer;
			plant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			plant = component;
		}
		PlantManager.Instance.plants.Add(plant);
		if (NeedSun == -1)
		{
			NeedSun = GetPlantNc(plant.GetPlantType()).NeedNum;
		}
		if (LV.Instance.LvSpStates.Contains(LVSpState.FreeDay))
		{
			NeedSun = 0;
		}
		plant.InitForPlace(grid, CurrOrderNum, SPcode != 3);
		SetGridInfo(plant, grid, NeedSun, PlacePlayer);
		CurrOrderNum++;
		if (NeedSun == -2)
		{
			NeedSun = 0;
		}
		if (plant.IsZombiePlant)
		{
			PlayerManager.Instance.AddSunNum(-NeedSun, isSun: false, PlacePlayer);
		}
		else
		{
			PlayerManager.Instance.AddSunNum(-NeedSun, isSun: true, PlacePlayer);
		}
		if (SPcode == 1)
		{
			plant.OpenBlackAndWhite(isOpen: true);
		}
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
		{
			plant.RatThis(synClient: true);
		}
		plant.PlaceOverEvent();
	}

	public void SetGridInfo(PlantBase plant, Grid grid, int NeedSun, string PlacePlayer)
	{
		if (plant.isHaveSpecialCheck)
		{
			plant.SpCheckInitPlace();
		}
		else if (plant.BasePlant != PlantType.Nope)
		{
			bool flag = true;
			MapManager.Instance.PlantnoFlash(plant.BasePlant);
			if (grid.CurrPlantBase != null && grid.CurrPlantBase.CanCarryOtherPlant && grid.CurrPlantBase.GetPlantType() != plant.BasePlant)
			{
				if (grid.CurrPlantBase.CarryPlant != null)
				{
					grid.CurrPlantBase.CarryPlant.Dead();
					flag = false;
				}
				plant.transform.SetParent(grid.CurrPlantBase.transform);
				grid.CurrPlantBase.CarryPlant = plant;
			}
			else if (grid.CurrPlantBase != null)
			{
				if (grid.CurrPlantBase.GetPlantType() != plant.BasePlant && grid.CurrPlantBase.isProtectPlant)
				{
					plant.ProtectPlant = grid.CurrPlantBase;
					plant.ProtectPlant.transform.SetParent(plant.transform);
				}
				else if (grid.CurrPlantBase.ProtectPlant != null)
				{
					plant.ProtectPlant = grid.CurrPlantBase.ProtectPlant;
					grid.CurrPlantBase.ProtectPlant = null;
					plant.ProtectPlant.transform.SetParent(plant.transform);
					grid.CurrPlantBase.Dead();
					flag = false;
				}
				else
				{
					grid.CurrPlantBase.Dead();
					flag = false;
				}
				grid.CurrPlantBase = plant;
			}
			else
			{
				grid.CurrPlantBase = plant;
			}
			if (flag && plant.BasePlantSunNum >= 0 && NeedSun != -2)
			{
				PlayerManager.Instance.AddSunNum(-(plant.BasePlantSunNum + noBasePlantExtra), isSun: true, PlacePlayer);
			}
		}
		else if (grid.CurrPlantBase != null)
		{
			if (CanUseFirstAidNut(plant.GetPlantType()) && grid.CurrPlantBase.GetPlantType() == plant.GetPlantType())
			{
				plant.ProtectPlant = grid.CurrPlantBase.ProtectPlant;
				grid.CurrPlantBase.Dead();
				grid.CurrPlantBase = plant;
				if (plant.ProtectPlant != null)
				{
					plant.ProtectPlant.transform.SetParent(plant.transform);
				}
				if (PlacePlayer == GameManager.Instance.LocalPlayerSave.playerName)
				{
					StatsManager.Instance.AddStatsNum(StatsEnum.NutFirstAidNum);
				}
			}
			else if (CanUseFirstAidNut(plant.GetPlantType()) && grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() == plant.GetPlantType())
			{
				grid.CurrPlantBase.CarryPlant.Dead();
				plant.transform.SetParent(grid.CurrPlantBase.transform);
				grid.CurrPlantBase.CarryPlant = plant;
				if (PlacePlayer == GameManager.Instance.LocalPlayerSave.playerName)
				{
					StatsManager.Instance.AddStatsNum(StatsEnum.NutFirstAidNum);
				}
			}
			else if (plant.isProtectPlant && CanUseFirstAidNut(plant.GetPlantType()) && grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.ProtectPlant.GetPlantType() == plant.GetPlantType())
			{
				grid.CurrPlantBase.ProtectPlant.Dead();
				plant.transform.SetParent(grid.CurrPlantBase.transform);
				grid.CurrPlantBase.ProtectPlant = plant;
				if (PlacePlayer == GameManager.Instance.LocalPlayerSave.playerName)
				{
					StatsManager.Instance.AddStatsNum(StatsEnum.NutFirstAidNum);
				}
			}
			else if (grid.CurrPlantBase.CanCarryOtherPlant && !plant.isProtectPlant)
			{
				plant.transform.SetParent(grid.CurrPlantBase.transform);
				grid.CurrPlantBase.CarryPlant = plant;
			}
			else if (plant.isProtectPlant)
			{
				grid.CurrPlantBase.ProtectPlant = plant;
				plant.transform.SetParent(grid.CurrPlantBase.transform);
			}
			else if (grid.CurrPlantBase.isProtectPlant && !plant.CanCarryOtherPlant)
			{
				grid.CurrPlantBase.transform.SetParent(plant.transform);
				plant.ProtectPlant = grid.CurrPlantBase;
				grid.CurrPlantBase = plant;
			}
			else if (!grid.CurrPlantBase.CanCarryOtherPlant && plant.CanCarryOtherPlant)
			{
				grid.CurrPlantBase.transform.SetParent(plant.transform);
				if (grid.CurrPlantBase.isProtectPlant)
				{
					plant.ProtectPlant = grid.CurrPlantBase;
				}
				else
				{
					plant.CarryPlant = grid.CurrPlantBase;
				}
				grid.CurrPlantBase.transform.position += new Vector3(plant.CarryOffset.x, plant.CarryOffset.y);
				if (grid.CurrPlantBase.ProtectPlant != null)
				{
					plant.ProtectPlant = grid.CurrPlantBase.ProtectPlant;
					plant.ProtectPlant.transform.SetParent(plant.transform);
					grid.CurrPlantBase.ProtectPlant = null;
				}
				grid.CurrPlantBase = plant;
			}
		}
		else
		{
			grid.CurrPlantBase = plant;
		}
	}

	public bool CheckZombie(ZombieType type2, Grid grid, int NeedMoon, string PlacePlayer)
	{
		if (NeedMoon == -1)
		{
			NeedMoon = ZombieChooser.Instance.GetCardInfo(type2).NeedNum;
		}
		if (PlayerManager.Instance.GetSunNum(isSun: false, PlacePlayer) < (float)NeedMoon)
		{
			return false;
		}
		if (!grid.CanPlaceZombie && type2 != ZombieType.BungiZombie)
		{
			return false;
		}
		if (LV.Instance.CurrLVType == LVType.PvP && grid.CurrGridType != TeamType.Normal && PlacePlayer != null)
		{
			bool flag = PvPSelector.Instance.RedTeamNames.Contains(PlacePlayer);
			if (type2 == ZombieType.BungiZombie)
			{
				if (!flag && grid.CurrGridType == TeamType.BlueTeam)
				{
					return false;
				}
				if (flag && grid.CurrGridType == TeamType.RedTeam)
				{
					return false;
				}
				if (grid.CurrGridType == TeamType.AllTeam)
				{
					return false;
				}
			}
			else
			{
				if (flag && grid.CurrGridType == TeamType.BlueTeam)
				{
					return false;
				}
				if (!flag && grid.CurrGridType == TeamType.RedTeam)
				{
					return false;
				}
				if (grid.CurrGridType == TeamType.AllTeam)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void ZombieConfirm(ZombieType type, ZombieBase zombie, Grid grid, int NeedMoon, string PlacePlayer, bool ratZombie)
	{
		if (NeedMoon == -1)
		{
			NeedMoon = GetZombieNc(type).NeedNum;
		}
		zombie.PlacePlayer = PlacePlayer;
		ZombieManager.Instance.UpdateZombie(type, zombie, grid.Position, grid.Point.y);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (PvPSelector.Instance.IsSameTeam(PlacePlayer))
			{
				zombie.RatThis(synClient: true);
			}
		}
		else if (ratZombie)
		{
			zombie.RatThis();
		}
		PlayerManager.Instance.AddSunNum(-NeedMoon, isSun: false, PlacePlayer);
	}

	public void ReplacePlant(PlantBase plant, PlantType type)
	{
		PlantBase newPlant = PlantManager.Instance.GetNewPlant(type);
		newPlant.PlacePlayer = plant.PlacePlayer;
		Grid currGrid = plant.currGrid;
		plant.Dead();
		PlantConfirm(newPlant, currGrid, -2, 3, newPlant.PlacePlayer);
	}

	private bool CanUseFirstAidNut(PlantType type)
	{
		return new List<PlantType>
		{
			PlantType.WallNut,
			PlantType.Tallnut,
			PlantType.Pumpkin,
			PlantType.Garlic
		}.Contains(type);
	}

	public void StartMove(SeedBankType type)
	{
		if (LV.Instance.LvSpStates.Contains(LVSpState.RainPlant))
		{
			if (!GameManager.Instance.isClient)
			{
				StartCoroutine(RainCardSpawner());
			}
			return;
		}
		if (LV.Instance.CurrBankType == BankType.ConveryorBelt)
		{
			ConveyorBelt.Instance.StartMove();
		}
		else
		{
			UpdateSeedBankType(type);
			base.transform.localScale = new Vector3(1.1f, 1.1f);
			StartCoroutine(DoMove(5.1f));
		}
		if (LV.Instance.CurrBankType == BankType.SlotMachine)
		{
			SlotMachine.Instance.StartMove();
		}
	}

	public void StartMoveBack(bool fade)
	{
		if (LV.Instance.CurrBankType == BankType.ConveryorBelt)
		{
			ConveyorBelt.Instance.StartMoveBack(fade);
		}
		else
		{
			StopAllCoroutines();
			if (fade)
			{
				StartCoroutine(DoMove(6.8f));
			}
			else
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, 6.8f, base.transform.localPosition.z);
				base.transform.localScale = Vector3.zero;
			}
		}
		if (LV.Instance.CurrBankType == BankType.SlotMachine)
		{
			SlotMachine.Instance.StartMoveBack(fade);
		}
	}

	private IEnumerator DoMove(float targetPosY)
	{
		while (base.transform.localPosition.y != targetPosY)
		{
			yield return null;
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, new Vector3(base.transform.localPosition.x, targetPosY, base.transform.localPosition.z), Time.deltaTime * 5f);
		}
	}
}
