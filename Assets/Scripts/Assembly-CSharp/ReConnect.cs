using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReConnect : MonoBehaviour
{
	public static ReConnect Instance;

	public Text BtnText;

	public Text NameText;

	public Text AnimText;

	public GameObject ReCtBtn;

	public bool IsOpening
	{
		get
		{
			if (base.transform.localScale.x > 0f)
			{
				return base.transform.localScale.y > 0f;
			}
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void OpenInit(bool needCnt)
	{
		NameText.text = "";
		Time.timeScale = 0f;
		ReCtBtn.SetActive(value: false);
		AnimText.gameObject.SetActive(value: true);
		if (needCnt)
		{
			AnimText.gameObject.SetActive(value: false);
			ReCtBtn.SetActive(value: true);
		}
		if (!(base.transform.localScale.x > 0f))
		{
			SeedBank.Instance.AllCancelPlace(NoSelect: true);
			base.transform.localScale = Vector3.one;
		}
	}

	public void OverClose()
	{
		Time.timeScale = 1f;
		base.transform.localScale = Vector3.zero;
	}

	public void LoadPlayerList(List<string> names)
	{
		NameText.text = "";
		ReCtBtn.SetActive(value: false);
		AnimText.gameObject.SetActive(value: true);
		for (int i = 0; i < names.Count; i++)
		{
			Text nameText = NameText;
			nameText.text = nameText.text + names[i] + "\n";
		}
	}

	public void GiveUpBtn()
	{
		if (GameManager.Instance.isServer)
		{
			SocketServer.Instance.GiveUpReConnect();
			return;
		}
		OverClose();
		LVManager.Instance.QuitBattleGame();
		SocketClient.Instance.ReConnectGiveUp();
	}

	public void ReConnectBtn()
	{
		UIManager.Instance.ReJoinGame();
	}
}
