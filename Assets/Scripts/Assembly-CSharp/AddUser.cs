using System.Collections;
using System.IO;
using SaveClass;
using UnityEngine;
using UnityEngine.UI;

public class AddUser : MonoBehaviour
{
	public InputField inputField;

	public Text Errortext;

	private Coroutine ErrortextCoroutine;

	private bool CanCancel;

	private void Start()
	{
		Errortext.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void Display(bool canCancel)
	{
		CanCancel = canCancel;
		base.gameObject.SetActive(value: true);
	}

	public void Confirm()
	{
		if (inputField.text == "")
		{
			return;
		}
		if (ChooseSave.Instance.CheckNameRepeat(inputField.text))
		{
			if (ErrortextCoroutine != null)
			{
				StopCoroutine(ErrortextCoroutine);
			}
			ErrortextCoroutine = StartCoroutine(RepeatLog());
			return;
		}
		string text = GameManager.Instance.SavePath + "/" + inputField.text;
		while (Directory.Exists(text))
		{
			text += "smf";
		}
		Directory.CreateDirectory(text);
		UserSave userSave = new UserSave();
		userSave.playerName = inputField.text;
		string data = JsonUtility.ToJson(userSave);
		StreamWriter streamWriter = new StreamWriter(text + "/" + FixedInfo.PlayerInfoName);
		streamWriter.Write(GameManager.Encrypt(data));
		streamWriter.Close();
		inputField.text = "";
		ChooseSave.Instance.LoadSave(userSave, text);
		CanCancel = true;
		Cancel();
	}

	public void Cancel()
	{
		if (CanCancel)
		{
			StopAllCoroutines();
			Errortext.transform.localScale = new Vector3(0f, 0f, 0f);
			inputField.text = "";
			base.gameObject.SetActive(value: false);
			ChooseSave.Instance.transform.gameObject.SetActive(value: true);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		}
	}

	private IEnumerator RepeatLog()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		Errortext.transform.localScale = new Vector3(1f, 1f, 0f);
		yield return new WaitForSeconds(3f);
		Errortext.transform.localScale = new Vector3(0f, 0f, 0f);
	}
}
