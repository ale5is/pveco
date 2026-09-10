using UnityEngine;
using UnityEngine.EventSystems;

public class MailboxTable : MonoBehaviour
{
	public Renderer REnderer;

	private void OnMouseEnter()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
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
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			CameraControl.Instance.SetPosition(new Vector2(0f, -30f));
			StartSceneManager.Instance.LoadStartScence(PlayAnim: true);
		}
	}
}
