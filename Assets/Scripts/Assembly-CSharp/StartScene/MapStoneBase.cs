using UnityEngine;

namespace StartScene
{
	public class MapStoneBase : MonoBehaviour
	{
		public Sprite MapSprite;

		public Sprite LightSprite;

		public Sprite normalSprite;

		public int AdventureLvNum;

		public int MiniGameLvNum;

		public int PuzzleLvNum;

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject() && !(SelectMap.Instance.SelectedStone == this))
			{
				GetComponent<SpriteRenderer>().sprite = LightSprite;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
			}
		}

		private void OnMouseExit()
		{
			if (SelectMap.Instance.SelectedStone != this)
			{
				GetComponent<SpriteRenderer>().sprite = normalSprite;
			}
		}

		private void OnMouseDown()
		{
			if (!MyTool.IsPointerOverGameObject() && !(SelectMap.Instance.SelectedStone == this))
			{
				SelectThis();
			}
		}

		public void SelectThis()
		{
			GetComponent<SpriteRenderer>().sprite = LightSprite;
			SelectMap.Instance.SelectStone(this);
		}

		public void ClearSelect()
		{
			if (!(SelectMap.Instance.SelectedStone == this))
			{
				GetComponent<SpriteRenderer>().sprite = normalSprite;
			}
		}
	}
}
