using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BigTitle : MonoBehaviour
{
	public static BigTitle Instance;

	public Text TitleText;

	public Text BlackText;

	private bool isSpark;

	private bool isUp;

	private float Intensity;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (Intensity < 0.8f)
		{
			Intensity = 0.8f;
		}
		Color color = new Color(0.81f, 0.87f, 0.62f);
		if (!isSpark)
		{
			return;
		}
		if (isUp)
		{
			Intensity += Time.deltaTime * 0.8f;
			if (Intensity > 1.1f)
			{
				isUp = false;
			}
		}
		else
		{
			Intensity -= Time.deltaTime * 0.8f;
			if (Intensity < 0.8f)
			{
				isUp = true;
			}
		}
		color *= Intensity;
		TitleText.color = new Color(color.r, color.g, color.b, 1f);
	}

	public void LvReset()
	{
		StopAllCoroutines();
		base.gameObject.SetActive(value: false);
	}

	public void DisPlayInfo(string msg, int time, UnityAction action)
	{
		isSpark = true;
		Intensity = 1f;
		TitleText.text = msg;
		BlackText.text = msg;
		base.gameObject.SetActive(value: true);
		StartCoroutine(DoFuncWait(action, time));
	}

	protected IEnumerator DoFuncWait(UnityAction fun, float time)
	{
		yield return new WaitForSeconds(time);
		fun?.Invoke();
		base.gameObject.SetActive(value: false);
	}
}
