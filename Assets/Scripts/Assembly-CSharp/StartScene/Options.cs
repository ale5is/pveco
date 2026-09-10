using UnityEngine;

namespace StartScene
{
	public class Options : MonoBehaviour
	{
		public Sprite EnterGreen;

		private Sprite ExitSprite;

		public SpriteRenderer spriteRenderer;

		private void Start()
		{
			ExitSprite = spriteRenderer.sprite;
		}

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				spriteRenderer.sprite = EnterGreen;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
			}
		}

		private void OnMouseExit()
		{
			spriteRenderer.sprite = ExitSprite;
		}

		private void OnMouseDown()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				if (Random.Range(0, 2) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
				}
				UIManager.Instance.ShowSetPanel();
			}
		}
	}
}
