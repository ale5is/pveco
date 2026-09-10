using System.Collections;
using UnityEngine;

namespace StartScene
{
	public class Adventure : MonoBehaviour
	{
		public SpriteRenderer REnderer;

		private bool isSparkle;

		private bool isToDark;

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				REnderer.material.SetFloat("_Brightness", 1.3f);
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
			}
		}

		private void OnMouseExit()
		{
			REnderer.material.SetFloat("_Brightness", 1f);
		}

		private void OnMouseDown()
		{
			if (!MyTool.IsPointerOverGameObject() && !GameManager.Instance.isClient)
			{
				UIManager.Instance.OpenAndFocusUI(isBlack: false);
				base.gameObject.SetActive(value: true);
				isToDark = true;
				isSparkle = true;
				StartCoroutine(PlayZombieHand());
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
				AudioManager.Instance.StopBgAudio();
			}
		}

		private void Update()
		{
			if (!isSparkle)
			{
				return;
			}
			float r = REnderer.color.r;
			if (isToDark)
			{
				if (r > 0.6f)
				{
					r -= Time.deltaTime * 4f;
					REnderer.color = new Color(r, r, r, 1f);
				}
				else
				{
					isToDark = false;
				}
			}
			else if (r < 1f)
			{
				r += Time.deltaTime * 4f;
				REnderer.color = new Color(r, r, r, 1f);
			}
			else
			{
				isToDark = true;
			}
		}

		private IEnumerator PlayZombieHand()
		{
			AdventureHand.Instance.StartPlay();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position, isAll: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameOver, base.transform.position, isAll: true);
			yield return new WaitForSeconds(1.2f);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EvilLaugh, base.transform.position, isAll: true);
			yield return new WaitForSeconds(2.6f);
			LevelSelector.Instance.StartAdvcGame();
			isSparkle = false;
			AdventureHand.Instance.transform.localScale = Vector3.zero;
			UIManager.Instance.CloseUI();
			REnderer.color = new Color(1f, 1f, 1f, 1f);
		}
	}
}
