using UnityEngine;
using UnityEngine.Events;

public class AwardScence : MonoBehaviour
{
	public static AwardScence Instance;

	public Transform AwardPos;

	public Transform BtnLight;

	public TextMesh Title;

	public TextMesh TitleShadow;

	public TextMesh ItemName;

	public TextMesh Content;

	private Booty Booty;

	private UnityAction ContinueEvent;

	private void Awake()
	{
		Instance = this;
	}

	public void LoadText(string title, string itemName, string cont)
	{
		Title.text = title;
		TitleShadow.text = title;
		ItemName.text = itemName;
		Content.text = cont;
	}

	public void JumpTo(Booty booty, UnityAction action)
	{
		if (Booty != null)
		{
			Booty.DestroyThis();
		}
		BtnLight.localScale = Vector3.zero;
		CameraControl.Instance.SetPosition(base.transform.position);
		ContinueEvent = action;
		if (booty != null)
		{
			Booty = booty;
			booty.StartShow();
			booty.StopAllCoroutines();
			booty.transform.position = AwardPos.position;
			booty.transform.localScale = new Vector3(1.5f, 1.5f);
		}
		AudioManager.Instance.PlayBgAudio(BgmType.ZenGarden);
	}

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			BtnLight.localScale = Vector3.one;
		}
	}

	private void OnMouseExit()
	{
		BtnLight.localScale = Vector3.zero;
	}

	private void OnMouseDown()
	{
		if (MyTool.IsPointerOverGameObject())
		{
			return;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		if (!GameManager.Instance.isClient)
		{
			if (ContinueEvent == null)
			{
				LVManager.Instance.QuitBattleGame();
			}
			else
			{
				ContinueEvent();
			}
		}
	}
}
