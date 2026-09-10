using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
	public static SlotMachine Instance;

	public Animator SlotAnimator;

	public Sprite SunCardSpite;

	public Sprite DiamondCardSpite;

	public SpriteRenderer Card1;

	public SpriteRenderer CardSub1;

	public SpriteRenderer Card2;

	public SpriteRenderer CardSub2;

	public SpriteRenderer Card3;

	public SpriteRenderer CardSub3;

	public TextMesh SlotText;

	private bool isRolling;

	private bool isSlot;

	private int RollNum;

	private float speed1;

	private float speed2;

	private float speed3;

	private SlotMachineCard Card1Type = new SlotMachineCard();

	private SlotMachineCard Card2Type = new SlotMachineCard();

	private SlotMachineCard Card3Type = new SlotMachineCard();

	private bool FirstOver1;

	private bool FirstOver2;

	private bool FirstOver3;

	private Coroutine TextCoroutine;

	private int OutAwardNum;

	private List<CardType> cardTypes => LV.Instance.GeneralCardPool;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (!isRolling)
		{
			return;
		}
		float num = 1.2f;
		if (RollNum > 0 && speed1 > 0f)
		{
			if (speed1 > num)
			{
				speed1 -= 2f * Time.deltaTime;
			}
			if (speed1 < num)
			{
				speed1 = num;
			}
		}
		if (RollNum > 1 && speed2 > 0f)
		{
			if (speed2 > num)
			{
				speed2 -= 2f * Time.deltaTime;
			}
			if (speed2 < num)
			{
				speed2 = num;
			}
		}
		if (RollNum > 2 && speed3 > 0f)
		{
			if (speed3 > num)
			{
				speed3 -= 2f * Time.deltaTime;
			}
			if (speed3 < num)
			{
				speed3 = num;
			}
		}
		Card1.transform.localPosition = Vector3.MoveTowards(Card1.transform.localPosition, new Vector3(Card1.transform.localPosition.x, 0.6f), speed1 * Time.deltaTime);
		if (Card1.transform.localPosition.y == 0.6f)
		{
			if (speed1 == num)
			{
				if (FirstOver1)
				{
					speed1 = 0f;
					Card1.color = Color.white;
					CardSub1.color = Color.white;
				}
				else
				{
					FirstOver1 = true;
					ResetCard(isLast: true, Card1, CardSub1, ref Card1Type);
					Card1.transform.localPosition = new Vector3(Card1.transform.localPosition.x, 0.06f);
				}
			}
			else if (speed1 > 0f)
			{
				ResetCard(isLast: false, Card1, CardSub1, ref Card1Type);
				Card1.transform.localPosition = new Vector3(Card1.transform.localPosition.x, 0.06f);
			}
		}
		Card2.transform.localPosition = Vector3.MoveTowards(Card2.transform.localPosition, new Vector3(Card2.transform.localPosition.x, 0.6f), speed2 * Time.deltaTime);
		if (Card2.transform.localPosition.y == 0.6f && speed2 > 0f)
		{
			if (speed2 == num)
			{
				if (FirstOver2)
				{
					speed2 = 0f;
					Card2.color = Color.white;
					CardSub2.color = Color.white;
				}
				else
				{
					FirstOver2 = true;
					ResetCard(isLast: true, Card2, CardSub2, ref Card2Type);
					Card2.transform.localPosition = new Vector3(Card2.transform.localPosition.x, 0.06f);
				}
			}
			else if (speed2 > 0f)
			{
				ResetCard(isLast: false, Card2, CardSub2, ref Card2Type);
				Card2.transform.localPosition = new Vector3(Card2.transform.localPosition.x, 0.06f);
			}
		}
		Card3.transform.localPosition = Vector3.MoveTowards(Card3.transform.localPosition, new Vector3(Card3.transform.localPosition.x, 0.6f), speed3 * Time.deltaTime);
		if (Card3.transform.localPosition.y != 0.6f || !(speed3 > 0f))
		{
			return;
		}
		if (speed3 == num)
		{
			if (FirstOver3)
			{
				speed3 = 0f;
				Card3.color = Color.white;
				CardSub3.color = Color.white;
				isRolling = false;
				isSlot = false;
				PlantType plantType = PlantType.Nope;
				int num2 = 0;
				if (Card1Type.plantType != PlantType.Nope && Card1Type.plantType == Card2Type.plantType)
				{
					num2++;
					plantType = Card1Type.plantType;
				}
				if (Card2Type.plantType != PlantType.Nope && Card2Type.plantType == Card3Type.plantType)
				{
					num2++;
					plantType = Card2Type.plantType;
				}
				if (Card3Type.plantType != PlantType.Nope && Card1Type.plantType == Card3Type.plantType)
				{
					num2++;
					plantType = Card3Type.plantType;
				}
				if (num2 == 0)
				{
					SlotText.text = "再拉一次！";
				}
				else if (num2 == 1)
				{
					SlotText.text = "两个图案相同，一株免费植物！";
					SeedBank.Instance.SpawnDropCard(plantType, ZombieType.Nope, GetSpawnPos());
					ClientSend(num2, plantType, ZombieType.Nope, 0);
					SendMsg("抽到了一株<color=#41FF00>[免费植物]</color>！");
				}
				else if (num2 > 1)
				{
					SlotText.text = "三个图案相同，三株免费植物！";
					for (int i = 0; i < 3; i++)
					{
						SeedBank.Instance.SpawnDropCard(plantType, ZombieType.Nope, GetSpawnPos());
					}
					ClientSend(num2, plantType, ZombieType.Nope, 0);
					SendMsg("抽到了三株<color=#41FF00>[免费植物]</color>！！！");
				}
				if (num2 == 0)
				{
					ZombieType zombieType = ZombieType.Nope;
					if (Card1Type.zombieType != ZombieType.Nope && Card1Type.zombieType == Card2Type.zombieType)
					{
						num2++;
						zombieType = Card1Type.zombieType;
					}
					if (Card2Type.zombieType != ZombieType.Nope && Card2Type.zombieType == Card3Type.zombieType)
					{
						num2++;
						zombieType = Card2Type.zombieType;
					}
					if (Card3Type.zombieType != ZombieType.Nope && Card1Type.zombieType == Card3Type.zombieType)
					{
						num2++;
						zombieType = Card3Type.zombieType;
					}
					if (num2 == 0)
					{
						SlotText.text = "再拉一次！";
					}
					else if (num2 == 1)
					{
						SlotText.text = "两个图案相同，奖励一只免费僵尸！";
						ZombieManager.Instance.UpdateZombieOnRandomLine(zombieType, base.transform.position);
						ClientSend(num2, PlantType.Nope, zombieType, 0);
						SendMsg("抽到了一只<color=#41FF00>[免费僵尸]</color>！");
					}
					else if (num2 > 1)
					{
						SlotText.text = "三个图案相同，奖励三只免费僵尸！";
						for (int j = 0; j < 3; j++)
						{
							ZombieManager.Instance.UpdateZombieOnRandomLine(zombieType, base.transform.position);
						}
						ClientSend(num2, PlantType.Nope, zombieType, 0);
						SendMsg("抽到了三只<color=#41FF00>[免费僵尸]</color>！！！");
					}
				}
				if (num2 == 0)
				{
					int num3 = 0;
					if (Card1Type.otherType != 0 && Card1Type.otherType == Card2Type.otherType)
					{
						num2++;
						num3 = Card1Type.otherType;
					}
					if (Card2Type.otherType != 0 && Card2Type.otherType == Card3Type.otherType)
					{
						num2++;
						num3 = Card2Type.otherType;
					}
					if (Card3Type.otherType != 0 && Card1Type.otherType == Card3Type.otherType)
					{
						num2++;
						num3 = Card3Type.otherType;
					}
					if (num2 == 0)
					{
						SlotText.text = "再拉一次！";
					}
					else if (num2 == 1)
					{
						if (num3 == 1)
						{
							SlotText.text = "阳光小奖！";
							for (int k = 0; k < 4; k++)
							{
								SkyManager.Instance.CreatePlantSun(GetSpawnPos(), 25f, SunType.Normal, null);
							}
							ClientSend(num2, PlantType.Nope, ZombieType.Nope, 1);
							SendMsg("抽到了<color=#41FF00>[阳光小奖]</color>！");
						}
						if (num3 == 2)
						{
							SlotText.text = "钻石小奖！";
							LvItemManager.Instance.SummonDiamond(GetSpawnPos());
							ClientSend(num2, PlantType.Nope, ZombieType.Nope, 2);
							SendMsg("抽到了<color=#41FF00>[钻石小奖]</color>！");
						}
					}
					else if (num2 > 1)
					{
						if (num3 == 1)
						{
							SlotText.text = "阳光大奖！";
							for (int l = 0; l < 6; l++)
							{
								SkyManager.Instance.CreatePlantSun(GetSpawnPos(), 50f, SunType.Normal, null);
							}
							ClientSend(num2, PlantType.Nope, ZombieType.Nope, 1);
							SendMsg("抽到了<color=#41FF00>[阳光大奖]</color>！！！");
						}
						if (num3 == 2)
						{
							SlotText.text = "钻石大奖！";
							for (int m = 0; m < 4; m++)
							{
								LvItemManager.Instance.SummonDiamond(GetSpawnPos());
							}
							ClientSend(num2, PlantType.Nope, ZombieType.Nope, 2);
							SendMsg("抽到了<color=#41FF00>[钻石大奖]</color>！！！");
						}
					}
				}
				if (TextCoroutine != null)
				{
					StopCoroutine(TextCoroutine);
				}
				TextCoroutine = StartCoroutine(TextShark());
				if (num2 > 0)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Chime, base.transform.position, isAll: true);
				}
				else
				{
					ClientSend(0, PlantType.Nope, ZombieType.Nope, 0);
				}
			}
			else
			{
				FirstOver3 = true;
				ResetCard(isLast: true, Card3, CardSub3, ref Card3Type);
				Card3.transform.localPosition = new Vector3(Card3.transform.localPosition.x, 0.06f);
			}
		}
		else if (speed3 > 0f)
		{
			ResetCard(isLast: false, Card3, CardSub3, ref Card3Type);
			Card3.transform.localPosition = new Vector3(Card3.transform.localPosition.x, 0.06f);
		}
	}

	private Vector2 GetSpawnPos()
	{
		return new Vector2(base.transform.position.x + Random.Range(-1.6f, 1.6f), base.transform.position.y + Random.Range(-3.5f, -1.2f));
	}

	private Vector2 GetSpawnPos(Vector2 pos)
	{
		return new Vector2(pos.x + Random.Range(-1.6f, 1.6f), pos.y + Random.Range(-3.5f, -1.2f));
	}

	private void ResetCard(bool isLast, SpriteRenderer card, SpriteRenderer cardSub, ref SlotMachineCard cardType)
	{
		card.sprite = cardSub.sprite;
		if (isLast && cardType == Card2Type && Random.Range(0, 10) > 6)
		{
			Card2Type.otherType = Card1Type.otherType;
			Card2Type.plantType = Card1Type.plantType;
			Card2Type.zombieType = Card1Type.zombieType;
		}
		else if (isLast && cardType == Card3Type && ((!CheckSame(Card1Type, Card2Type) && Random.Range(0, 2) > 0) || Random.Range(0, 5) > 3))
		{
			if (Random.Range(0, 2) > 0)
			{
				Card3Type.otherType = Card1Type.otherType;
				Card3Type.plantType = Card1Type.plantType;
				Card3Type.zombieType = Card1Type.zombieType;
			}
			else
			{
				Card3Type.otherType = Card2Type.otherType;
				Card3Type.plantType = Card2Type.plantType;
				Card3Type.zombieType = Card2Type.zombieType;
			}
		}
		else
		{
			int num = Random.Range(0, cardTypes.Count + 2);
			if (num >= cardTypes.Count)
			{
				cardType.otherType = num - cardTypes.Count + 1;
				if (cardType.otherType == 2 && Random.Range(0, 80) <= 78)
				{
					cardType.otherType = 1;
				}
				cardType.plantType = PlantType.Nope;
				cardType.zombieType = ZombieType.Nope;
			}
			else
			{
				cardType.otherType = 0;
				cardType.plantType = cardTypes[num].plantType;
				cardType.zombieType = cardTypes[num].zombieType;
			}
		}
		if (cardType.otherType > 0)
		{
			if (cardType.otherType == 1)
			{
				cardSub.sprite = SunCardSpite;
			}
			else if (cardType.otherType == 2)
			{
				cardSub.sprite = DiamondCardSpite;
			}
		}
		else if (cardType.plantType != PlantType.Nope)
		{
			cardSub.sprite = SeedBank.Instance.GetPlantNc(cardType.plantType).OwnerSprite;
		}
		else if (cardType.zombieType != ZombieType.Nope)
		{
			cardSub.sprite = SeedBank.Instance.GetZombieNc(cardType.zombieType).OwnerSprite;
		}
	}

	private bool CheckSame(SlotMachineCard card1, SlotMachineCard card2)
	{
		if (card1.otherType != 0 && card1.otherType == card2.otherType)
		{
			return true;
		}
		if (card1.plantType != PlantType.Nope && card1.plantType == card2.plantType)
		{
			return true;
		}
		if (card1.zombieType != ZombieType.Nope && card1.zombieType == card2.zombieType)
		{
			return true;
		}
		return false;
	}

	private void OnMouseDown()
	{
		if (MyTool.IsPointerOverGameObject() || isSlot)
		{
			return;
		}
		if (PlayerManager.Instance.GetSunNum(isSun: true, GameManager.Instance.LocalPlayerSave.playerName) < 50f)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
			return;
		}
		FirstOver1 = false;
		FirstOver2 = false;
		FirstOver3 = false;
		isSlot = true;
		SlotAnimator.Play("down");
		StartCoroutine(Roll());
		SlotText.text = "";
		if (TextCoroutine != null)
		{
			StopCoroutine(TextCoroutine);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.slotmachine, base.transform.position, isAll: true);
		if (GameManager.Instance.isClient)
		{
			ClientSend(-1, PlantType.Nope, ZombieType.Nope, 0);
		}
		else
		{
			PlayerManager.Instance.AddSunNum(-50f, isSun: true, GameManager.Instance.LocalPlayerSave.playerName);
		}
	}

	private void ClientSend(int num, PlantType plant, ZombieType zombie, int itemType)
	{
		if (GameManager.Instance.isClient)
		{
			SlotMchBag slotMchBag = new SlotMchBag();
			slotMchBag.num = num;
			slotMchBag.mapPos = base.transform.position;
			slotMchBag.card.plantType = plant;
			slotMchBag.card.zombieType = zombie;
			slotMchBag.card.otherType = itemType;
			SocketClient.Instance.SendSlotMBag(slotMchBag);
		}
	}

	public void ClientSyn(SlotMchBag bag)
	{
		if (bag.num == -1)
		{
			if (PlayerManager.Instance.GetSunNum(isSun: true, GameManager.Instance.LocalPlayerSave.playerName) >= 50f)
			{
				PlayerManager.Instance.AddSunNum(-50f, isSun: true, GameManager.Instance.LocalPlayerSave.playerName);
				OutAwardNum++;
			}
		}
		else
		{
			if (bag.num <= 0 || OutAwardNum <= 0)
			{
				return;
			}
			OutAwardNum--;
			if (bag.card.plantType != PlantType.Nope)
			{
				for (int i = 0; i < bag.num; i++)
				{
					SeedBank.Instance.SpawnDropCard(bag.card.plantType, ZombieType.Nope, GetSpawnPos(bag.mapPos));
				}
			}
			else if (bag.card.zombieType != ZombieType.Nope)
			{
				for (int j = 0; j < bag.num; j++)
				{
					ZombieManager.Instance.UpdateZombieOnRandomLine(bag.card.zombieType, bag.mapPos);
				}
			}
			else
			{
				if (bag.card.otherType == 0 || bag.card.otherType != 1)
				{
					return;
				}
				if (bag.num > 1)
				{
					for (int k = 0; k < 6; k++)
					{
						SkyManager.Instance.CreatePlantSun(GetSpawnPos(bag.mapPos), 50f, SunType.Normal, null);
					}
				}
				else
				{
					for (int l = 0; l < 4; l++)
					{
						SkyManager.Instance.CreatePlantSun(GetSpawnPos(bag.mapPos), 25f, SunType.Normal, null);
					}
				}
			}
		}
	}

	private void SendMsg(string msg)
	{
		if (GameManager.Instance.isOnline)
		{
			ChatInput.Instance.SendMessageToAll(msg, needName: true);
		}
	}

	private IEnumerator Roll()
	{
		RollNum = 0;
		Color color = new Color(0.4f, 0.4f, 0.4f);
		Card1.color = color;
		CardSub1.color = color;
		Card2.color = color;
		CardSub2.color = color;
		Card3.color = color;
		CardSub3.color = color;
		isRolling = true;
		speed1 = 3f;
		speed2 = speed1;
		speed3 = speed1;
		yield return new WaitForSeconds(0.5f);
		RollNum++;
		yield return new WaitForSeconds(0.5f);
		RollNum++;
		SlotAnimator.Play("up");
		yield return new WaitForSeconds(0.5f);
		RollNum++;
	}

	private IEnumerator TextShark()
	{
		int num = 15;
		float a = SlotText.color.a;
		while (num > 0)
		{
			do
			{
				yield return null;
				if (a > 0.3f)
				{
					a -= Time.deltaTime * 1.5f;
					SetTextAlaph(a);
				}
			}
			while (!(a < 0.3f));
			a = 0.3f;
			SetTextAlaph(a);
			do
			{
				yield return null;
				if (a < 1f)
				{
					a += Time.deltaTime * 2f;
					SetTextAlaph(a);
				}
			}
			while (!(a >= 1f));
			a = 1f;
			SetTextAlaph(a);
			num--;
		}
		SlotText.text = "";
		yield return new WaitForSeconds(5f);
		SlotText.text = "再拉一次！";
		while (true)
		{
			yield return null;
			if (a > 0.3f)
			{
				a -= Time.deltaTime * 1.5f;
				SetTextAlaph(a);
			}
			if (!(a < 0.3f))
			{
				continue;
			}
			a = 0.3f;
			SetTextAlaph(a);
			do
			{
				yield return null;
				if (a < 1f)
				{
					a += Time.deltaTime * 2f;
					SetTextAlaph(a);
				}
			}
			while (!(a >= 1f));
			a = 1f;
			SetTextAlaph(a);
		}
	}

	private void SetTextAlaph(float a)
	{
		SlotText.color = new Color(SlotText.color.r, SlotText.color.g, SlotText.color.b, a);
	}

	public void StartMove()
	{
		isSlot = false;
		isRolling = false;
		OutAwardNum = 0;
		SetTextAlaph(1f);
		SlotText.text = "开始抽奖吧！";
		base.transform.localScale = new Vector3(1.6f, 1.6f);
		StartCoroutine(DoMove(5.1f));
		Sprite ownerSprite = SeedBank.Instance.GetPlantNc(PlantType.SunFlower).OwnerSprite;
		Card1.sprite = ownerSprite;
		Card2.sprite = ownerSprite;
		Card3.sprite = ownerSprite;
	}

	public void StartMoveBack(bool fade)
	{
		isSlot = false;
		isRolling = false;
		SlotAnimator.Play("New State");
		Card1.color = Color.white;
		Card2.color = Color.white;
		Card3.color = Color.white;
		Card1.transform.localPosition = new Vector3(Card1.transform.localPosition.x, 0.06f);
		Card2.transform.localPosition = new Vector3(Card2.transform.localPosition.x, 0.06f);
		Card3.transform.localPosition = new Vector3(Card3.transform.localPosition.x, 0.06f);
		StopAllCoroutines();
		if (fade)
		{
			StartCoroutine(DoMove(7f));
			return;
		}
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 7f, base.transform.localPosition.z);
		base.transform.localScale = Vector3.zero;
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
