using UnityEngine;

public class LVStartEF : MonoBehaviour
{
	private Animator animator;

	private bool showFinal;

	private bool isStart;

	private bool startOverEvent;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (isStart && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
		{
			base.gameObject.SetActive(value: false);
			isStart = false;
		}
	}

	public void Show()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ReadySetPlant, base.transform.position, isAll: true);
		base.gameObject.SetActive(value: true);
		animator.Play("LVStartEF", 0, 0f);
		startOverEvent = true;
	}

	public void StopAll()
	{
		startOverEvent = false;
		base.gameObject.SetActive(value: false);
	}

	public void LVStartShowOver()
	{
		if (startOverEvent)
		{
			isStart = true;
			LVManager.Instance.LVStartEFOver();
		}
	}

	public void BigWaveShowOver()
	{
		if (showFinal)
		{
			showFinal = false;
			base.gameObject.SetActive(value: true);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FinalWave, base.transform.position, isAll: true);
			animator.Play("LastWave", 0, 0f);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void CloseLast()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ShowBigWave()
	{
		base.gameObject.SetActive(value: true);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.HugeWave, base.transform.position, isAll: true);
		animator.Play("BigWave", 0, 0f);
	}

	public void ShowFinalWave()
	{
		showFinal = true;
		ShowBigWave();
	}
}
