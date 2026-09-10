using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPlantCardNC : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	private Image cardImage;

	public Sprite OwnerSprite;

	private bool isChoosed;

	private bool isUnLock = true;

	public bool isNeedSun;

	public int NeedNum;

	public float CDTime;

	public PlantType CardPlantType;

	public ZombieType CardZombieType;

	public bool isForCreate;

	public bool IsChoosed
	{
		get
		{
			return isChoosed;
		}
		set
		{
			if (isUnLock)
			{
				isChoosed = value;
				if (value)
				{
					cardImage.color = new Color(0.3f, 0.3f, 0.3f);
				}
				else
				{
					cardImage.color = new Color(1f, 1f, 1f);
				}
			}
		}
	}

	public bool IsUnLock
	{
		get
		{
			return isUnLock;
		}
		set
		{
			isUnLock = value;
			if (value)
			{
				cardImage.sprite = OwnerSprite;
				base.transform.GetComponentInChildren<Text>().text = NeedNum.ToString();
				if (LV.Instance.LvSpStates.Contains(LVSpState.FreeDay))
				{
					base.transform.GetComponentInChildren<Text>().text = "0";
				}
			}
			else
			{
				cardImage.sprite = SeedBank.Instance.NoCardSprite;
				base.transform.GetComponentInChildren<Text>().text = "";
			}
		}
	}

	private void Start()
	{
		cardImage = base.transform.GetComponent<Image>();
		IsChoosed = false;
		OwnerSprite = cardImage.sprite;
	}

	public void ClickThis(bool haveSound)
	{
		if (!IsUnLock)
		{
			return;
		}
		if (CardPlantType == PlantType.Heronsbill && LV.Instance.CurrLVType == LVType.PvP)
		{
			if (haveSound)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
			}
		}
		else if (isForCreate)
		{
			SeedChooser.Instance.ChooseOver();
			ZombieChooser.Instance.ChooseOver();
			CreatePanel.Instance.SelectCard(CardPlantType, CardZombieType);
		}
		else if (!IsChoosed && (CardPlantType != PlantType.Nope || CardZombieType != ZombieType.Nope) && !SeedBank.Instance.isFull && !SeedChooser.Instance.isPrepare)
		{
			if (SeedBank.Instance.ChooseCard(this))
			{
				IsChoosed = true;
				if (haveSound)
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
			if (GameManager.Instance.isClient)
			{
				SelectCard selectCard = new SelectCard();
				selectCard.plantType = CardPlantType;
				selectCard.zombieType = CardZombieType;
				selectCard.isBack = false;
				SocketClient.Instance.SelectCard(selectCard);
			}
			if (GameManager.Instance.isServer)
			{
				SelectCard selectCard2 = new SelectCard();
				selectCard2.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
				selectCard2.plantType = CardPlantType;
				selectCard2.zombieType = CardZombieType;
				selectCard2.isBack = false;
				SocketServer.Instance.SelectCard(selectCard2);
			}
		}
		else if (haveSound)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		ClickThis(haveSound: true);
	}
}
