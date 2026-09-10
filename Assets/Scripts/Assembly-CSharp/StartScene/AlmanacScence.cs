using System.Collections.Generic;
using UnityEngine;

namespace StartScene
{
	public class AlmanacScence : MonoBehaviour
	{
		public static AlmanacScence Instance;

		public Transform MenuPage;

		public Transform CardPage;

		public Transform BackMenuBtn;

		public SpriteRenderer TitleBarREderer;

		public TextMesh TitleBarText1;

		public TextMesh TitleBarText2;

		public Sprite Bar2;

		public Sprite Bar3;

		public Sprite Bar4;

		public Transform PlantPage;

		public Transform ZombiePage;

		public Transform MapPage;

		public Transform WeatherPage;

		public Transform CmdPage;

		public Transform OtherPage;

		private int PageNum;

		private List<AlmanacCardSlot> cardSlots = new List<AlmanacCardSlot>();

		private void Awake()
		{
			Instance = this;
		}

		public void InitAlmanac()
		{
			if (cardSlots.Count == 0)
			{
				CardPage.transform.localScale = Vector3.one;
				for (int i = 0; i < 6; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						AlmanacCardSlot component = Object.Instantiate(GameManager.Instance.GameConf.AlmanacCardSlot).GetComponent<AlmanacCardSlot>();
						component.transform.SetParent(CardPage);
						component.transform.localPosition = new Vector3(1.2f * (float)j, -1.6f * (float)i);
						cardSlots.Add(component);
					}
				}
			}
			BackMenu();
		}

		public void BackMenu()
		{
			TitleBarREderer.sprite = Bar2;
			TitleBarText1.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  目 录";
			TitleBarText2.text = "大 图 鉴  -  目 录";
			MenuPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = Vector3.zero;
			PlantPage.transform.localScale = Vector3.zero;
			ZombiePage.transform.localScale = Vector3.zero;
			MapPage.transform.localScale = Vector3.zero;
			WeatherPage.transform.localScale = Vector3.zero;
			CmdPage.transform.localScale = Vector3.zero;
			OtherPage.transform.localScale = Vector3.zero;
			ZombiePage.transform.localScale = Vector3.zero;
			CardPage.transform.localScale = Vector3.zero;
			AlmcPlantLoader.Instance.ClearPlantAndZombie();
		}

		public void OpenPlantPage()
		{
			TitleBarREderer.sprite = Bar3;
			TitleBarText1.color = new Color32(210, 156, 42, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  植 物";
			TitleBarText2.text = "大 图 鉴  -  植 物";
			PageNum = 0;
			UIPlantCardNC[] cardInfos = SeedChooser.Instance.GetCardInfos(PageNum);
			for (int i = 0; i < cardInfos.Length; i++)
			{
				if (cardSlots.Count >= i)
				{
					cardSlots[i].UpdateInfo(cardInfos[i]);
				}
			}
			MenuPage.transform.localScale = Vector3.zero;
			PlantPage.transform.localScale = Vector3.one;
			CardPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = new Vector3(1f, 1.5f, 1f);
			cardSlots[0].LoadThis();
		}

		public void NextPage()
		{
			PageNum++;
			if (PageNum > SeedChooser.Instance.Pages.Count - 1)
			{
				PageNum = 0;
			}
			UIPlantCardNC[] cardInfos = SeedChooser.Instance.GetCardInfos(PageNum);
			for (int i = 0; i < cardInfos.Length; i++)
			{
				if (cardSlots.Count >= i)
				{
					cardSlots[i].UpdateInfo(cardInfos[i]);
				}
			}
			if (cardInfos.Length < cardSlots.Count)
			{
				for (int j = cardInfos.Length; j < cardSlots.Count; j++)
				{
					cardSlots[j].ResetInfo();
				}
			}
		}

		public void OpenZombiePage()
		{
			TitleBarREderer.sprite = Bar4;
			TitleBarText1.color = new Color32(0, 196, 0, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  僵 尸";
			TitleBarText2.text = "大 图 鉴  -  僵 尸";
			PageNum = 0;
			UIPlantCardNC[] cardInfos = ZombieChooser.Instance.GetCardInfos(PageNum);
			for (int i = 0; i < cardInfos.Length; i++)
			{
				if (cardSlots.Count >= i)
				{
					cardSlots[i].UpdateInfo(cardInfos[i]);
				}
			}
			MenuPage.transform.localScale = Vector3.zero;
			ZombiePage.transform.localScale = Vector3.one;
			CardPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = new Vector3(1f, 1.5f, 1f);
			cardSlots[0].LoadThis();
		}

		public void OpenMapPage()
		{
			TitleBarREderer.sprite = Bar3;
			TitleBarText1.color = new Color32(210, 156, 42, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  地 图";
			TitleBarText2.text = "大 图 鉴  -  地 图";
			MenuPage.transform.localScale = Vector3.zero;
			MapPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = new Vector3(1f, 1.5f, 1f);
		}

		public void OpenWeatherPage()
		{
			TitleBarREderer.sprite = Bar3;
			TitleBarText1.color = new Color32(210, 156, 42, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  天 气";
			TitleBarText2.text = "大 图 鉴  -  天 气";
			MenuPage.transform.localScale = Vector3.zero;
			WeatherPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = new Vector3(1f, 1.5f, 1f);
		}

		public void OpenCmdPage()
		{
			TitleBarREderer.sprite = Bar3;
			TitleBarText1.color = new Color32(210, 156, 42, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  指 令";
			TitleBarText2.text = "大 图 鉴  -  指 令";
			MenuPage.transform.localScale = Vector3.zero;
			CmdPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = new Vector3(1f, 1.5f, 1f);
		}

		public void OpenOtherPage()
		{
			TitleBarREderer.sprite = Bar3;
			TitleBarText1.color = new Color32(210, 156, 42, byte.MaxValue);
			TitleBarText1.text = "大 图 鉴  -  其 他";
			TitleBarText2.text = "大 图 鉴  -  其 他";
			MenuPage.transform.localScale = Vector3.zero;
			OtherPage.transform.localScale = Vector3.one;
			BackMenuBtn.transform.localScale = new Vector3(1f, 1.5f, 1f);
		}
	}
}
