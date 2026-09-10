using UnityEngine;

namespace StartScene
{
	public class AcvmentBtn : MonoBehaviour
	{
		public SpriteRenderer REnderer;

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject())
			{
				REnderer.enabled = true;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
			}
		}

		private void OnMouseExit()
		{
			REnderer.enabled = false;
		}

		private void OnMouseDown()
		{
			if (!MyTool.IsPointerOverGameObject() && !GameManager.Instance.isClient)
			{
				CameraControl.Instance.MoveToAcvment();
				AcvmentManager.Instance.InitAcvment();
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
			}
		}
	}
}
