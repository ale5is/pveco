using UnityEngine;

public class PvPButton : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject() && GameManager.Instance.isOnline)
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
		if (!MyTool.IsPointerOverGameObject() && GameManager.Instance.isOnline)
		{
			PvPSelector.Instance.OpenAndInit();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}
}
