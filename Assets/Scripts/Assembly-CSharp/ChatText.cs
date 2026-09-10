using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
	public RectTransform rectTransform;

	public Text text;

	public void GetContent(string Content)
	{
		text.text = Content;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 50f + 50f * (text.preferredHeight - 44f) / 46f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void GetContent(string Content, Color32 color)
	{
		text.text = Content;
		text.color = color;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 50f + 50f * (text.preferredHeight - 44f) / 46f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}
