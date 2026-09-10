using System.Collections;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class Vase : MonoBehaviour
{
	public int OnlineId;

	private Grid CurrGrid;

	public Sprite HalfVase;

	public Sprite NormalVase;

	public Sprite NormalVaseInner;

	public Sprite PlantVase;

	public Sprite PlantVaseInner;

	public Sprite ZombieVase;

	public Sprite ZombieVaseInner;

	public SpriteRenderer REnderer;

	public SpriteRenderer REnderer2;

	public SpriteMask VaseMask;

	public SpriteMask WaterMask;

	public SpriteMask WaterMask2;

	public GameObject WhiteWater;

	public SpriteRenderer CardREnderer;

	private ZombieBase zombie;

	private PlantType plantType;

	private ZombieType zombieType;

	private bool isClicked;

	private Coroutine coroutine;

	private bool breaking;

	private bool isLighted;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			REnderer.material.SetFloat("_Brightness", 1.3f);
			REnderer2.material.SetFloat("_Brightness", 1.3f);
		}
	}

	private void OnMouseExit()
	{
		REnderer.material.SetFloat("_Brightness", 1f);
		REnderer2.material.SetFloat("_Brightness", 1f);
	}

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && !isClicked && Input.GetMouseButtonDown(0))
		{
			isClicked = true;
			Shovel.Instance.CancelShovel();
			StatsManager.Instance.AddStatsNum(StatsEnum.VaseBreakNum);
			if (GameManager.Instance.isClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Vase;
				synItem.SynCode[0] = 1;
				SocketClient.Instance.SendSynBag(synItem);
			}
			else
			{
				BreakVase();
			}
		}
	}

	private void BreakVase()
	{
		isClicked = true;
		if (GameManager.Instance.isServer && !breaking)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Vase;
			synItem.SynCode[0] = 1;
			SocketServer.Instance.SendSynBag(synItem);
		}
		if (!breaking)
		{
			StartCoroutine(Break());
		}
	}

	public void SetVaseType(int type)
	{
		switch (type)
		{
		case 1:
			REnderer.sprite = PlantVase;
			REnderer2.sprite = PlantVaseInner;
			break;
		case 2:
			REnderer.sprite = ZombieVase;
			REnderer2.sprite = ZombieVaseInner;
			break;
		default:
			REnderer.sprite = NormalVase;
			REnderer2.sprite = NormalVaseInner;
			break;
		}
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Vase;
			synItem.SynCode[0] = 2;
			synItem.SynCode[1] = type;
			SocketServer.Instance.SendSynBag(synItem);
		}
	}

	public void CreateInit(Grid grid, VaseType type)
	{
		isClicked = false;
		breaking = false;
		CurrGrid = grid;
		CurrGrid.Vases.Add(this);
		base.transform.position = grid.Position + new Vector2(0f, 0.175f);
		base.transform.GetComponent<SortingGroup>().sortingOrder = grid.Point.y * 200 + FixedInfo.Vase;
		VaseMask.sprite = NormalVase;
		WaterMask.enabled = CurrGrid.isNoIceWater;
		WaterMask2.enabled = CurrGrid.isNoIceWater;
		WhiteWater.SetActive(CurrGrid.isNoIceWater);
		if (CurrGrid.isNoIceWater)
		{
			VaseMask.sprite = HalfVase;
			base.transform.position += new Vector3(0f, -0.4f);
		}
		if (type.plantType != PlantType.Nope)
		{
			plantType = type.plantType;
			CardREnderer.enabled = true;
			CardREnderer.sprite = SeedBank.Instance.GetPlantNc(type.plantType).OwnerSprite;
		}
		else if (type.zombieType != ZombieType.Nope)
		{
			zombie = ZombieManager.Instance.GetNewZombie(type.zombieType);
			zombie.transform.SetParent(base.transform);
			zombie.CreateInit(inGrid: false, null, isRat: false);
			zombie.SetInsideVisible();
			zombie.SetSortingOrder(CardREnderer.sortingOrder);
			zombie.transform.position = base.transform.position + zombie.VaseOffset;
			zombie.transform.localScale = zombie.VaseScale;
			zombieType = type.zombieType;
			CardREnderer.enabled = false;
		}
	}

	private IEnumerator Break()
	{
		breaking = true;
		BattlePlayerList.Instance.PlayHammerAnimation(base.transform.position, 1, GameManager.Instance.LocalPlayerSave.playerName);
		yield return new WaitForSeconds(0.3f);
		BreakEvent(synClient: true);
	}

	public void BreakEvent(bool synClient = false)
	{
		if (GameManager.Instance.isClient && !synClient)
		{
			return;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.vase_breaking, base.transform.position);
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.VaseParticle).transform.position = base.transform.position + new Vector3(0f, 0.3f);
		if (!GameManager.Instance.isClient)
		{
			if (plantType != PlantType.Nope)
			{
				SeedBank.Instance.SpawnDropCard(plantType, ZombieType.Nope, base.transform.position);
			}
			else if (zombieType != ZombieType.Nope)
			{
				ZombieBase component = ZombieManager.Instance.GetNewZombie(zombieType).GetComponent<ZombieBase>();
				ZombieManager.Instance.UpdateZombie(zombieType, component, CurrGrid.Position, CurrGrid.Point.y);
				if (CurrGrid.isNoIceWater)
				{
					component.DirctInWater();
				}
			}
			if (GameManager.Instance.isServer && !breaking)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Vase;
				synItem.SynCode[0] = 3;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
		CurrGrid.Vases.Remove(this);
		LvItemManager.Instance.DestoryVase(this);
	}

	public void CheckLight()
	{
		bool flag = false;
		float xx = 1f;
		if (!isLighted && (CurrGrid.LightNum >= 3 || LvItemManager.Instance.VaseAlwaysLight))
		{
			xx = 0.3f;
			flag = true;
		}
		if (isLighted && CurrGrid.LightNum < 3 && !LvItemManager.Instance.VaseAlwaysLight)
		{
			flag = true;
		}
		if (flag)
		{
			isLighted = !isLighted;
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}
			coroutine = StartCoroutine(Tofloat(xx));
		}
	}

	private IEnumerator Tofloat(float xx)
	{
		float oo = 3f;
		if (REnderer.color.a > xx)
		{
			oo = -3f;
		}
		if (oo > 0f)
		{
			while (REnderer.color.a <= xx)
			{
				yield return null;
				REnderer.color = new Color(REnderer.color.r, REnderer.color.g, REnderer.color.b, REnderer.color.a + oo * Time.deltaTime);
			}
		}
		else
		{
			while (REnderer.color.a > xx)
			{
				yield return null;
				REnderer.color = new Color(REnderer.color.r, REnderer.color.g, REnderer.color.b, REnderer.color.a + oo * Time.deltaTime);
			}
		}
	}

	public void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 1)
		{
			BreakVase();
		}
		else if (syn.SynCode[0] == 2)
		{
			SetVaseType(syn.SynCode[1]);
		}
		else if (syn.SynCode[0] == 3)
		{
			BreakEvent(synClient: true);
		}
	}
}
