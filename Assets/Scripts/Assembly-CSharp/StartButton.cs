using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public Text StartText;

	private Image LightImage;

	private void Awake()
	{
		LightImage = base.transform.Find("Light").GetComponent<Image>();
		LightImage.transform.localScale = Vector3.zero;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		LightImage.transform.localScale = Vector3.one;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		LightImage.transform.localScale = Vector3.zero;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		SeedBank.Instance.SaveSelectedCard();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		if (GameManager.Instance.isClient)
		{
			if (!SpectatorList.Instance.LocalIsSpectator)
			{
				SeedChooser.Instance.Prepare();
				ZombieChooser.Instance.Prepare();
			}
			return;
		}
		if (GameManager.Instance.isServer)
		{
			if (!BattlePlayerList.Instance.CheckPrepare())
			{
				return;
			}
			SocketServer.Instance.StartRunLv();
		}
		SeedChooser.Instance.StartRunLv();
		ZombieChooser.Instance.StartRunLv();
	}
}
