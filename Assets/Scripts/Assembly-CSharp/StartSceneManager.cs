using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
	public static StartSceneManager Instance;

	public Animator BackLeft;

	public Animator BackCenter;

	public Animator BackRight;

	public ChangeUser changeUser;

	public Transform AlmanacTransform;

	public Transform StoreTransform;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		LoadStartScence(PlayAnim: true);
	}

	public void LoadStartScence(bool PlayAnim)
	{
		if (GameManager.Instance.LocalPlayerSave.StoreLvl > 0)
		{
			StoreTransform.localScale = new Vector3(0.6f, 0.6f);
		}
		else
		{
			StoreTransform.localScale = Vector3.zero;
		}
		if (GameManager.Instance.LocalPlayerSave.AlmanacUnLock)
		{
			AlmanacTransform.localScale = new Vector3(0.6f, 0.6f);
		}
		else
		{
			AlmanacTransform.localScale = Vector3.zero;
		}
		if (PlayAnim)
		{
			BackLeft.Play("", 0, 0f);
			BackCenter.Play("", 0, 0f);
			BackRight.Play("", 0, 0f);
			changeUser.PlayAnimation();
		}
	}

	public void GoEndLess()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		Invoke("DoGoEndLess", 0.5f);
	}
}
