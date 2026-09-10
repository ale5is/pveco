using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedCChangePage : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	private Image LightImage;

	public int BtnType;

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
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		if (BtnType == 1)
		{
			SeedChooser.Instance.CurrPage++;
		}
		else if (BtnType == 2)
		{
			SeedChooser.Instance.CurrPage--;
		}
		else if (BtnType != 3 && BtnType == 4)
		{
			SeedBank.Instance.LoadLastCard();
		}
	}
}
