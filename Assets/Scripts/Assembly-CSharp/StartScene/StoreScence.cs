using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartScene
{
	public class StoreScence : MonoBehaviour
	{
		public static StoreScence Instance;

		public Sprite SoldOut;

		public Sprite ComingSoon;

		public Animator CarAnimator;

		public Transform DaveBubble;

		public TextMesh DaveText;

		public TextMesh MoneyText;

		public BoxCollider2D NextBtn;

		public BoxCollider2D PrevBtn;

		public CrazyDave crazyDave;

		public List<Transform> ItemPages = new List<Transform>();

		private int currPage;

		private Coroutine CloseBubbleCoroutine;

		public int CurrPage
		{
			get
			{
				return currPage;
			}
			set
			{
				int index = currPage;
				if (value > ItemPages.Count - 1)
				{
					currPage = 0;
				}
				else if (value < 0)
				{
					currPage = ItemPages.Count - 1;
				}
				else
				{
					currPage = value;
				}
				ItemPages[index].localScale = new Vector3(0f, 0f);
				ItemPages[currPage].localScale = new Vector3(1f, 1f);
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			crazyDave.gameObject.SetActive(value: false);
		}

		public void OpenAndInitStore()
		{
			for (int i = 0; i < ItemPages.Count; i++)
			{
				ItemPages[i].localScale = new Vector3(0f, 0f);
			}
			if (GameManager.Instance.LocalPlayerSave.StoreLvl > 1)
			{
				NextBtn.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
				PrevBtn.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
			}
			else
			{
				NextBtn.transform.localScale = Vector3.zero;
				PrevBtn.transform.localScale = Vector3.zero;
			}
			CurrPage = 0;
			StopAllCoroutines();
			CarAnimator.Play("CarStart", 0, 0f);
			crazyDave.gameObject.SetActive(value: true);
			crazyDave.StoreOpneInit();
			for (int j = 0; j < ItemPages.Count; j++)
			{
				StoreGoods[] componentsInChildren = ItemPages[j].GetComponentsInChildren<StoreGoods>();
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].InitGoods();
				}
			}
			StartCoroutine(DaveSmallTalk());
			CameraControl.Instance.SetPosition(new Vector2(base.transform.position.x, base.transform.position.y));
			DaveBubble.transform.localScale = Vector3.zero;
			CloseBubbleCoroutine = null;
		}

		public void Close()
		{
			crazyDave.gameObject.SetActive(value: false);
		}

		public void PrevPage()
		{
			CurrPage--;
			CarAnimator.Play("CarReOpen", 0, 0f);
			StartCoroutine(WaitCarClose());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Hatchback_close, base.transform.position, isAll: true);
		}

		public void NextPage()
		{
			CurrPage++;
			CarAnimator.Play("CarReOpen", 0, 0f);
			StartCoroutine(WaitCarClose());
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Hatchback_close, base.transform.position, isAll: true);
		}

		private IEnumerator WaitCarClose()
		{
			PrevBtn.enabled = false;
			NextBtn.enabled = false;
			yield return new WaitForSeconds(0.6f);
			PrevBtn.enabled = true;
			NextBtn.enabled = true;
		}

		public void UpdateMoneyBank(int money)
		{
			MoneyText.text = money.ToString();
		}

		public void DaveLoadInfo(string content)
		{
			DaveText.text = content;
			crazyDave.StoreLongTalk();
			DaveBubble.transform.localScale = new Vector3(2f, 2f, 2f);
			if (CloseBubbleCoroutine != null)
			{
				StopCoroutine(CloseBubbleCoroutine);
			}
		}

		public void GoodsMouseExit()
		{
			if (CloseBubbleCoroutine != null)
			{
				StopCoroutine(CloseBubbleCoroutine);
			}
			CloseBubbleCoroutine = StartCoroutine(WaitCloseBubble(2.5f));
		}

		private IEnumerator WaitCloseBubble(float time)
		{
			DaveBubble.transform.localScale = new Vector3(2f, 2f, 2f);
			yield return new WaitForSeconds(time);
			DaveBubble.transform.localScale = Vector3.zero;
			CloseBubbleCoroutine = null;
		}

		private IEnumerator DaveSmallTalk()
		{
			while (CameraControl.Instance.transform.position.x == base.transform.position.x && CameraControl.Instance.transform.position.y == base.transform.position.y)
			{
				yield return new WaitForSeconds(12f);
				if (CloseBubbleCoroutine == null)
				{
					if (Random.Range(0, 3) == 0)
					{
						DaveText.text = "一起来吧！我疯掉了！！";
					}
					else if (Random.Range(1, 3) == 1)
					{
						DaveText.text = "我的价格很不可思议吧！！！";
					}
					else
					{
						DaveText.text = "我吃掉在地上的东西。";
					}
					crazyDave.StoreShortTalk();
					CloseBubbleCoroutine = StartCoroutine(WaitCloseBubble(5f));
				}
			}
		}
	}
}
