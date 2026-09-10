using UnityEngine;
using UnityEngine.EventSystems;

public class MultiplayerBtn : MonoBehaviour
{
	public Renderer REnderer;

	public int type;

	private void OnMouseEnter()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
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
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (type == 0)
		{
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.CloseServer();
			}
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.CloseClient();
			}
		}
		else if (type == 1)
		{
			UIManager.Instance.HostPassword.gameObject.SetActive(value: true);
		}
	}
}
