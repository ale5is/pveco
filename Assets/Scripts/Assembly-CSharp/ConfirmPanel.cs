using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
	private UnityAction Action;

	public Text TitleText;

	public Text ContentText;

	public Text WarnText;

	public Transform focusOn;

	public void InitEvent(UnityAction action, string Title, string Content, string Warn, Transform focus = null)
	{
		focusOn = focus;
		Action = action;
		base.transform.localScale = Vector3.one;
		TitleText.text = Title;
		ContentText.text = Content;
		WarnText.text = Warn;
		base.gameObject.SetActive(value: true);
	}

	public void Confirm()
	{
		if (Action != null)
		{
			Action();
		}
		base.transform.localScale = Vector3.zero;
	}

	public void Cancel()
	{
		base.transform.localScale = Vector3.zero;
		if (focusOn != null)
		{
			focusOn.gameObject.SetActive(value: true);
		}
	}
}
