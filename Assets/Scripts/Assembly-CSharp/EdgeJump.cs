using UnityEngine;
using UnityEngine.EventSystems;

public class EdgeJump : MonoBehaviour
{
	public Renderer REnderer;

	public string url;

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
			Application.OpenURL(url);
		}
	}
}
