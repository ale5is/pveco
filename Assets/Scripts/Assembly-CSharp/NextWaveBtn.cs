using System.Collections;
using UnityEngine;

public class NextWaveBtn : MonoBehaviour
{
	public static NextWaveBtn Instance;

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

	public void CanNextWave()
	{
		if (!GameManager.Instance.isClient)
		{
			FirstClick = true;
			base.transform.localScale = Vector3.one;
		}
	}

	public void CloseBtn()
	{
		base.transform.localScale = Vector3.zero;
	}

	public void BtnEvent()
	{
		if (!GameManager.Instance.isClient)
		{
			if (FirstClick && GameManager.Instance.isAndroid)
			{
				FirstClick = false;
				StartCoroutine(WaitDoubleClick());
			}
			else
			{
				CloseBtn();
				LVManager.Instance.NextWaveBtnEvent();
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
	}

	private IEnumerator WaitDoubleClick()
	{
		yield return new WaitForSeconds(1f);
		FirstClick = true;
	}
}
