using System.Collections.Generic;
using UnityEngine;

namespace StartScene
{
	public class SelectMap : MonoBehaviour
	{
		public static SelectMap Instance;

		public TextMesh Text;

		public SpriteRenderer MapSprite;

		public List<MapStoneBase> Stones = new List<MapStoneBase>();

		public MapStoneBase SelectedStone;

		public Transform Lock;

		public int CurrLvSeriesId => Stones.IndexOf(SelectedStone) + 1;

		private void Awake()
		{
			Instance = this;
		}

		public void SelectStone(MapStoneBase selectedStone)
		{
			if (SelectedStone == selectedStone)
			{
				return;
			}
			SelectedStone = selectedStone;
			for (int i = 0; i < Stones.Count; i++)
			{
				Stones[i].ClearSelect();
			}
			Lock.localScale = Vector3.zero;
			Text.transform.localScale = Vector3.one;
			base.transform.GetComponent<SpriteRenderer>().enabled = true;
			base.transform.GetComponent<BoxCollider2D>().enabled = true;
			MapSprite.sprite = SelectedStone.MapSprite;
			MapSprite.color = Color.white;
			if (selectedStone == Stones[0])
			{
				GameManager.Instance.LoadLvInfo(1);
			}
			else if (selectedStone == Stones[1])
			{
				if (GameManager.Instance.LocalPlayerSave.SwampOpen)
				{
					Lock.localScale = Vector3.zero;
					GameManager.Instance.LoadLvInfo(2);
				}
				else
				{
					Text.transform.localScale = Vector3.zero;
					Lock.localScale = new Vector3(1.5f, 1.5f);
					base.transform.GetComponent<SpriteRenderer>().enabled = false;
					base.transform.GetComponent<BoxCollider2D>().enabled = false;
					SpriteRenderer mapSprite = MapSprite;
					Color color = (MapSprite.color = new Color32(100, 100, 100, byte.MaxValue));
					mapSprite.color = color;
				}
			}
			LevelSelector.Instance.LoadLastLv();
		}

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				Text.color = Color.white;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
			}
		}

		private void OnMouseExit()
		{
			Text.color = Color.black;
		}

		private void OnMouseDown()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				LevelSelector.Instance.OpenSelector();
			}
		}
	}
}
