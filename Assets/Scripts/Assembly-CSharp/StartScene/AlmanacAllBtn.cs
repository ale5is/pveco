using UnityEngine;

namespace StartScene
{
	public class AlmanacAllBtn : MonoBehaviour
	{
		public int BtnType;

		public Renderer REnderer;

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
			if (MyTool.IsPointerOverGameObject())
			{
				return;
			}
			if (BtnType != 4 && BtnType != 5 && BtnType != 6)
			{
				if (Random.Range(0, 2) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
				}
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
			}
			switch (BtnType)
			{
			case 0:
				CameraControl.Instance.SetPosition(new Vector2(0f, -30f));
				break;
			case 1:
				AlmanacScence.Instance.OpenPlantPage();
				break;
			case 2:
				AlmanacScence.Instance.OpenMapPage();
				break;
			case 3:
				AlmanacScence.Instance.OpenCmdPage();
				break;
			case 4:
				AlmanacScence.Instance.OpenZombiePage();
				break;
			case 5:
				AlmanacScence.Instance.OpenWeatherPage();
				break;
			case 6:
				AlmanacScence.Instance.OpenOtherPage();
				break;
			case 7:
				AlmanacScence.Instance.BackMenu();
				break;
			case 8:
				AlmanacScence.Instance.NextPage();
				break;
			}
		}
	}
}
