using SaveClass;
using UnityEngine;

namespace StartScene
{
	public class StoreGoods : MonoBehaviour
	{
		public GoodsType Type;

		private SpriteRenderer REnderer;

		private SpriteRenderer SoldOutLabel;

		private TextMesh PriceText;

		private string DaveInfo;

		private int Price;

		private UserSave PlayerSave => GameManager.Instance.LocalPlayerSave;

		private void Start()
		{
			REnderer = base.transform.GetComponent<SpriteRenderer>();
			PriceText = base.transform.Find("Store_PriceTag").Find("Price").GetComponent<TextMesh>();
			SoldOutLabel = base.transform.Find("Store_SoldOutLabel").GetComponent<SpriteRenderer>();
		}

		public void InitGoods()
		{
			switch (Type)
			{
			case GoodsType.CardSlot:
			{
				DaveInfo = "增加卡片槽数，让你\n每关可选更多的植物！";
				int num = 0;
				for (int i = 0; i < PlayerSave.SpItems.Count; i++)
				{
					if (PlayerSave.SpItems[i] == SpItem.StoreCardSlot)
					{
						num++;
					}
				}
				Price = 160000;
				SoldOutLabel.sprite = null;
				if (num == 0)
				{
					Price = 2500;
				}
				else if (num == 1)
				{
					Price = 5000;
				}
				else if (num == 2)
				{
					Price = 20000;
				}
				else if (num == 3)
				{
					Price = 60000;
				}
				else if (num == 4)
				{
					Price = 100000;
				}
				else if (num == 5)
				{
					Price = 160000;
				}
				else if (num > 5)
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				break;
			}
			case GoodsType.PoolCleaner:
				Price = 1000;
				DaveInfo = "这辆池塘清洁车,能用来\n提高池塘的防御等级！";
				if (PlayerSave.SpItems.Contains(SpItem.PoolCleaner))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.Rack:
				Price = 200;
				DaveInfo = "这把干草叉能击倒最快碰\n到它的那只僵尸!一次性购买3把！";
				if (PlayerSave.SpItems.Contains(SpItem.Rack))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.RoofCleaner:
				Price = 3000;
				DaveInfo = "这辆屋顶推车，能加强屋顶\n的防御等级！";
				if (PlayerSave.SpItems.Contains(SpItem.RoofCleaner))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.GatlingPea:
				Price = 5000;
				DaveInfo = "让你的双发射手变成机枪\n射手！机枪射手一次能够\n射出6颗豌豆！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.GatlingPea))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.TwinSunflower:
				Price = 5000;
				DaveInfo = "让你的向日葵变成双胞\n向日葵！双胞向日葵提供的\n阳光是向日葵的2倍！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.TwinSunflower))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.GloomShroom:
				Price = 7500;
				DaveInfo = "让你的大喷菇变成忧郁菇！\n忧郁菇能在小范围内\n做迅速的攻击！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.GloomShroom))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.Cattail:
				Price = 15000;
				DaveInfo = "让你的莲叶变成猫尾草！\n猫尾草能够攻击任一路线上的\n僵尸，而且能打下气球僵尸！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.Cattail))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.SpikeRock:
				Price = 7500;
				DaveInfo = "让你的地刺变成钢地刺！\n钢地刺有着2倍的\n攻击力且非常耐用！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.SpikeRock))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.GoldMagnet:
				Price = 7500;
				DaveInfo = "让你的磁力菇变成吸金磁！\n吸金磁可以全方位吸取\n更多金属防具并且\n可以收集金币和钻石！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.GoldMagnet))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.WinterMelon:
				Price = 15000;
				DaveInfo = "让你的西瓜投手变成冰瓜投手！\n冰瓜投手有高攻击力的同时\n可以让被击中的僵尸慢下来！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.Wintermelonpult))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.CobCannon:
				Price = 20000;
				DaveInfo = "让你的玉米投手变成玉米加农炮！\n点击一个玉米加农炮\n发动致命攻击！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.CobCannon))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.Imitater:
				Price = 30000;
				DaveInfo = "变身茄子让你在游戏中\n同时拥有两个相同的植物！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.Imitater))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.RottenImitater:
				Price = 60000;
				DaveInfo = "腐烂模仿者会随机变成\n一个植物或者是僵尸！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.RottenImitater))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.FirstAidNut:
				Price = 8000;
				DaveInfo = "坚果愈合术可以让你受伤的\n坚果焕然一新，\n对其他防御植物一样有效！";
				if (PlayerSave.SpItems.Contains(SpItem.FirstAidNut))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.SnowRepeater:
				Price = 10000;
				DaveInfo = "让你的寒冰射手变成极冻射手！\n极冻射手拥有\n超强的僵尸控制能力！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.SnowRepeater))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.Glowstarfruit:
				Price = 15000;
				DaveInfo = "让你的杨桃变成荧光杨桃！\n荧光杨桃拥有\n强大的控制与输出能力！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.Glowstarfruit))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			case GoodsType.MeltTorch:
				Price = 45000;
				DaveInfo = "让你的火炬树桩变成熔融树桩！\n熔融树桩可将豌豆变为\n超级强大的熔融豌豆！";
				if (PlayerSave.UnlockedPlants.Contains(PlantType.MeltTorch))
				{
					SoldOutLabel.sprite = StoreScence.Instance.SoldOut;
				}
				else
				{
					SoldOutLabel.sprite = null;
				}
				break;
			}
			PriceText.text = "$" + Price;
		}

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				REnderer.material.SetFloat("_Brightness", 1.3f);
				StoreScence.Instance.DaveLoadInfo(DaveInfo);
			}
		}

		private void OnMouseExit()
		{
			REnderer.material.SetFloat("_Brightness", 1f);
			StoreScence.Instance.GoodsMouseExit();
		}

		private void OnMouseDown()
		{
			if (MyTool.IsPointerOverGameObject() || SoldOutLabel.sprite != null)
			{
				return;
			}
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
			if (PlayerManager.Instance.Money < Price)
			{
				UIManager.Instance.ConfirmPanel.InitEvent(() =>
				{
				}, "无法购买物品", "你的金钱不足以购买这件物品。", "");
				return;
			}
			UIManager.Instance.ConfirmPanel.InitEvent(() =>
			{
				switch (Type)
				{
				case GoodsType.CardSlot:
					PlayerSave.SpItems.Add(SpItem.StoreCardSlot);
					break;
				case GoodsType.PoolCleaner:
					PlayerSave.SpItems.Add(SpItem.PoolCleaner);
					break;
				case GoodsType.Rack:
					PlayerSave.SpItems.Add(SpItem.Rack);
					PlayerSave.SpItems.Add(SpItem.Rack);
					PlayerSave.SpItems.Add(SpItem.Rack);
					break;
				case GoodsType.RoofCleaner:
					PlayerSave.SpItems.Add(SpItem.RoofCleaner);
					break;
				case GoodsType.GatlingPea:
					GameManager.Instance.AddNewPlant(PlantType.GatlingPea);
					break;
				case GoodsType.TwinSunflower:
					GameManager.Instance.AddNewPlant(PlantType.TwinSunflower);
					break;
				case GoodsType.GloomShroom:
					GameManager.Instance.AddNewPlant(PlantType.GloomShroom);
					break;
				case GoodsType.Cattail:
					GameManager.Instance.AddNewPlant(PlantType.Cattail);
					break;
				case GoodsType.SpikeRock:
					GameManager.Instance.AddNewPlant(PlantType.SpikeRock);
					break;
				case GoodsType.GoldMagnet:
					GameManager.Instance.AddNewPlant(PlantType.GoldMagnet);
					break;
				case GoodsType.WinterMelon:
					GameManager.Instance.AddNewPlant(PlantType.Wintermelonpult);
					break;
				case GoodsType.CobCannon:
					GameManager.Instance.AddNewPlant(PlantType.CobCannon);
					break;
				case GoodsType.Imitater:
					GameManager.Instance.AddNewPlant(PlantType.Imitater);
					break;
				case GoodsType.FirstAidNut:
					PlayerSave.SpItems.Add(SpItem.FirstAidNut);
					break;
				case GoodsType.SnowRepeater:
					GameManager.Instance.AddNewPlant(PlantType.SnowRepeater);
					break;
				case GoodsType.Glowstarfruit:
					GameManager.Instance.AddNewPlant(PlantType.Glowstarfruit);
					break;
				case GoodsType.RottenImitater:
					GameManager.Instance.AddNewPlant(PlantType.RottenImitater);
					break;
				case GoodsType.MeltTorch:
					GameManager.Instance.AddNewPlant(PlantType.MeltTorch);
					break;
				}
				PlayerManager.Instance.Money -= Price;
				StatsManager.Instance.AddStatsNum(StatsEnum.SpendMoney, Price);
				InitGoods();
				GameManager.Instance.SaveUserInfo();
			}, "确定购买物品", "你确定要花费$" + Price + "购买这个物品吗？", "");
		}
	}
}
