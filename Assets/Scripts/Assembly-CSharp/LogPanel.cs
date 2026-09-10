using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LogPanel : MonoBehaviour
{
	public Text logText;

	public Text ButtonText;

	public Button Button;

	private UnityAction action;

	public void DisplayLog(string logContent, UnityAction confirmAction)
	{
		base.gameObject.SetActive(value: true);
		Button.gameObject.SetActive(value: true);
		logText.text = logContent;
		ButtonText.text = "确定";
		action = confirmAction;
	}

	public void Confirm()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GraveButton, base.transform.position, isAll: true);
		if (action != null)
		{
			action();
			action = null;
		}
		Close();
	}

	public void CancelConfirm()
	{
		Button.gameObject.SetActive(value: false);
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
}
