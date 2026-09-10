using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeEffPanel : MonoBehaviour
{
	public static FadeEffPanel Instance;

	public float fadeDuration = 0.25f;

	private Image fadeImage;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		fadeImage = base.transform.GetComponent<Image>();
		if (fadeImage != null)
		{
			Color color = fadeImage.color;
			color.a = 0f;
			fadeImage.color = color;
		}
	}

	public void FadeOut()
	{
		StartCoroutine(FadeRoutine(0f, 1f));
	}

	public void FadeIn()
	{
		StartCoroutine(FadeRoutine(0.8f, 0f));
	}

	public void FadeOutIn(UnityAction action)
	{
		StartCoroutine(FadeOutInRoutine(action));
	}

	private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
	{
		if (!(fadeImage == null))
		{
			float elapsedTime = 0f;
			Color color = fadeImage.color;
			color.a = startAlpha;
			fadeImage.color = color;
			yield return new WaitForSeconds(0.05f);
			while (elapsedTime < fadeDuration)
			{
				elapsedTime += Time.deltaTime;
				float t = Mathf.Clamp01(elapsedTime / fadeDuration);
				color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
				fadeImage.color = color;
				yield return null;
			}
			color.a = targetAlpha;
			fadeImage.color = color;
		}
	}

	private IEnumerator FadeOutInRoutine(UnityAction action)
	{
		yield return FadeRoutine(0f, 1f);
		yield return new WaitForSeconds(0.1f);
		action?.Invoke();
		yield return FadeRoutine(1f, 0f);
	}
}
