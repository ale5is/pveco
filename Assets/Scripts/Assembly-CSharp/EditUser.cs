using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EditUser : MonoBehaviour
{
	public GameObject ConfirmDelete;

	public InputField inputField;

	private void Start()
	{
		ConfirmDelete.SetActive(value: false);
	}

	public void EditConfirm()
	{
		if (inputField.text != ChooseSave.Instance.SelectedSaveOption.userSave.playerName && inputField.text != "" && !ChooseSave.Instance.CheckNameRepeat(inputField.text))
		{
			ChooseSave.Instance.SelectedSaveOption.userSave.playerName = inputField.text;
			string data = JsonUtility.ToJson(ChooseSave.Instance.SelectedSaveOption.userSave);
			StreamWriter streamWriter = new StreamWriter(ChooseSave.Instance.SelectedSaveOption.Path.ToString() + "/" + FixedInfo.PlayerInfoName);
			streamWriter.Write(GameManager.Encrypt(data));
			streamWriter.Close();
			inputField.text = "";
		}
		ChooseSave.Instance.LoadSavegroup();
		EditCancel();
	}

	public void EditCancel()
	{
		ChooseSave.Instance.transform.localScale = Vector3.one;
		ChooseSave.Instance.transform.gameObject.SetActive(value: true);
		ChooseSave.Instance.LoadSavegroup();
		base.gameObject.SetActive(value: false);
	}

	public void OpenDelete()
	{
		UIManager.Instance.ConfirmPanel.InitEvent(() =>
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			DeleteConfirm();
		}, "确定删除该玩家？", ChooseSave.Instance.SelectedSaveOption.userSave.playerName, "*删除后将不可恢复*");
	}

	public void DeleteConfirm()
	{
		ChooseSave.Instance.SelectedSaveOption.DeleteSave();
		ChooseSave.Instance.SelectedSaveOption = null;
		ChooseSave.Instance.LoadSavegroup();
		EditCancel();
		ConfirmDelete.SetActive(value: false);
	}

	public void DeleteCancel()
	{
		base.gameObject.SetActive(value: true);
		ConfirmDelete.SetActive(value: false);
	}
}
