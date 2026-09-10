using System.Collections;
using UnityEngine;

public class SkipRestBtn : MonoBehaviour
{
	public static SkipRestBtn Instance;

	private bool FirstClick;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		CloseBtn();
		FirstClick = true;
	}

	public void CloseBtn()
	{
		base.transform.localScale = Vector3.zero;
	}

	public void CanSkipWave()
	{
		FirstClick = true;
		base.transform.localScale = Vector3.one;
	}

	public void BtnEvent()
	{
		if (FirstClick && GameManager.Instance.isAndroid)
		{
			FirstClick = false;
			StartCoroutine(WaitDoubleClick());
		}
		else
		{
			CloseBtn();
			LVManager.Instance.SkipRest();
		}
		if (Random.Range(0, 2) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
		}
	}

	private IEnumerator WaitDoubleClick()
	{
		yield return new WaitForSeconds(1f);
		FirstClick = true;
	}
}
