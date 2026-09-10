using System.Collections.Generic;
using UnityEngine;

public class OnlineSeedBank : MonoBehaviour
{
	public int CardNum = 4;

	private int DecidedCardNum;

	private int choosedNum;

	public bool isFull;

	public bool isCanClick;

	public Transform group;

	public PlayerShow OwnerShow;

	private RectTransform seedBank;

	private List<UIPlantCard> slotList = new List<UIPlantCard>();

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

	private void Start()
	{
		seedBank = base.transform.GetComponent<RectTransform>();
		isFull = false;
		isCanClick = true;
	}

	public UIPlantCard GetPreCard(UIPlantCard pC)
	{
		int num = slotList.IndexOf(pC);
		if (num <= 0)
		{
			return null;
		}
		return slotList[num - 1];
	}

	public void UpdateCD(int cardID, bool isClear)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardId == cardID)
			{
				if (!isClear)
				{
					slotList[i].CDEnter();
				}
				else
				{
					slotList[i].currTimeForCd = 0f;
				}
				break;
			}
		}
	}

	public void UpdateSunNum(int sunNum)
	{
	}

	public void SpawnCardSlot(int soltNum)
	{
		ClearCardSlot();
		CardNum = soltNum;
		seedBank.sizeDelta = new Vector2(18 + CardNum * 48, seedBank.sizeDelta.y);
		for (int i = 0; i < CardNum; i++)
		{
			UIPlantCard component = Object.Instantiate(GameManager.Instance.GameConf.UICardSlot).GetComponent<UIPlantCard>();
			component.transform.SetParent(group);
			component.CardId = i;
			component.ownerSeedBank = this;
			slotList.Add(component);
			component.transform.localScale = new Vector3(1f, 1f);
		}
	}

	public void ClearCardSlot()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].isChoosed)
			{
				if (GameManager.Instance.isServer && LVManager.Instance.GameIsStart)
				{
					SeedBank.Instance.AddCard(slotList[i].CardPlantType, slotList[i].CardZombieType, slotList[i].currTimeForCd, CanUnChoose: true, canAddSlot: true);
				}
				slotList[i].ClearChoose(noAnimation: false);
				ChoosedNum--;
			}
			slotList[i].DestroyCardSlot();
		}
		ChoosedNum = 0;
		slotList.Clear();
	}

	public void ChooseCard(UIPlantCardNC nC, bool needAnim)
	{
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
		if (needAnim)
		{
			UIPlantCardAnimation component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CardSlotAnimation).GetComponent<UIPlantCardAnimation>();
			component.transform.SetParent(UIManager.Instance.transform);
			component.CreateInit(nC.transform.position, nC, null);
			component.PlayChooseAnimation(OwnerShow.transform.position);
		}
		slotList[DecidedCardNum].AddChoose(nC);
	}

	public void ClearChoose(int cardId, bool needAnim)
	{
		ChoosedNum--;
		UIPlantCard uIPlantCard = null;
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardId == cardId)
			{
				uIPlantCard = slotList[i];
				break;
			}
		}
		if (!(uIPlantCard == null))
		{
			uIPlantCard.ClearChoose();
			if (needAnim)
			{
				uIPlantCard.myPlantCard.IsChoosed = false;
				UIPlantCardAnimation component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CardSlotAnimation).GetComponent<UIPlantCardAnimation>();
				component.transform.SetParent(UIManager.Instance.transform);
				component.CreateInit(uIPlantCard.transform.position, uIPlantCard.myPlantCard, null);
				component.PlayChooseAnimation(uIPlantCard.myPlantCard.transform.position);
			}
		}
	}

	public bool ClearCD(PlantType type)
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			if (slotList[i].CardPlantType == type && slotList[i].currTimeForCd > 0f)
			{
				slotList[i].currTimeForCd = 0f;
				return true;
			}
		}
		return false;
	}
}
