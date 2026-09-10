using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LVFlag : MonoBehaviour
{
	private Image Flag;

	private Image Stick;

	public void CreateInit(Rect rect)
	{
		Flag = base.transform.Find("Flag").GetComponent<Image>();
		Stick = base.transform.Find("Stick").GetComponent<Image>();
		Flag.rectTransform.sizeDelta = new Vector2(rect.width, rect.height) * 0.825f;
		Stick.rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
	}

	public void GoRise(float x)
	{
		StartCoroutine(Rise(x));
	}

	private IEnumerator Rise(float x)
	{
		float y = Flag.transform.position.y + x / 12f;
		while (Flag.transform.position.y < y)
		{
			Flag.transform.Translate(new Vector3(0f, 15f * Time.deltaTime));
			yield return null;
		}
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
