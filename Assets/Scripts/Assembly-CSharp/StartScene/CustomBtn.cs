using UnityEngine;

namespace StartScene
{
	public class CustomBtn : MonoBehaviour
	{
		public int BtnType;

		public Renderer REnderer;

		private void OnMouseEnter()
		{
			if (!GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
			{
				REnderer.material.SetFloat("_Brightness", 1.3f);
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
				switch (BtnType)
				{
				case 3:
					CustomScence.Instance.SetSelectorPos(base.transform.position);
					break;
				case 4:
					CustomScence.Instance.SetSelectorPos(base.transform.position);
					break;
				}
			}
		}

		private void OnMouseExit()
		{
			REnderer.material.SetFloat("_Brightness", 1f);
		}

		private void OnMouseDown()
		{
			if (!GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
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
					CustomScence.Instance.OpenInit();
					CameraControl.Instance.SetPosition(new Vector2(-25f, -80f));
					break;
				case 2:
					CustomScence.Instance.Close();
					StartSceneManager.Instance.LoadStartScence(PlayAnim: true);
					CameraControl.Instance.SetPosition(new Vector2(0f, -30f));
					break;
				case 3:
					CustomScence.Instance.OpenCustomMap();
					break;
				case 4:
					CustomScence.Instance.OpenCustomLevel();
					break;
				}
			}
		}
	}
}
