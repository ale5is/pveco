using UnityEngine;
using UnityEngine.EventSystems;

namespace StartScene
{
	public class YardStone : MonoBehaviour
	{
		public Sprite MapSprite;

		public Sprite LightSprite;

		private Sprite sprite;

		private void Start()
		{
			sprite = GetComponent<SpriteRenderer>().sprite;
		}

		private void OnMouseEnter()
		{
			if (!EventSystem.current.IsPointerOverGameObject() && !GameManager.Instance.isOnline)
			{
				GetComponent<SpriteRenderer>().sprite = LightSprite;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
			}
		}

		private void OnMouseExit()
		{
			GetComponent<SpriteRenderer>().sprite = sprite;
		}

		private void OnMouseDown()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				_ = GameManager.Instance.isOnline;
			}
		}
	}
}
