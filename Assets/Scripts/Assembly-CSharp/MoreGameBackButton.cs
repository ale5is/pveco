using UnityEngine;

public class MoreGameBackButton : MonoBehaviour
{
	public Sprite Normal;

	public Sprite Light;

	public SpriteRenderer REnderer;

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			REnderer.sprite = Light;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		REnderer.sprite = Normal;
	}

	private void OnMouseDown()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			CameraControl.Instance.MoveTo(new Vector2(0f, -30f), null);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		}
	}
}
