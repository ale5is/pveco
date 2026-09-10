using UnityEngine;

namespace StartScene
{
	public class StoreButton : MonoBehaviour
	{
		public int BtnType;

		public Renderer REnderer;

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				REnderer.material.SetFloat("_Brightness", 1.3f);
			}
		}

		private void OnMouseExit()
		{
			REnderer.material.SetFloat("_Brightness", 1f);
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
				switch (BtnType)
				{
				case 1:
					CameraControl.Instance.SetPosition(new Vector2(0f, -30f));
					StartSceneManager.Instance.LoadStartScence(PlayAnim: true);
					StoreScence.Instance.Close();
					break;
				case 2:
					StoreScence.Instance.PrevPage();
					break;
				case 3:
					StoreScence.Instance.NextPage();
					break;
				}
			}
		}
	}
}
