using System.Collections.Generic;
using UnityEngine;

public class ChatTextGroup : MonoBehaviour
{
	public static ChatTextGroup Instance;

	public RectTransform rectTransform;

	private List<ChatText> chatTexts = new List<ChatText>();

	public GameObject ChatTextPrefab;

	private int scrollNum;

	private int scroll;

	private int ScrollNum
	{
		get
		{
			return scrollNum;
		}
		set
		{
			if (value <= scroll && value >= 0)
			{
				scrollNum = value;
				rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, -225 - 50 * ScrollNum);
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (scroll > 0)
		{
			if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				ScrollNum++;
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				ScrollNum--;
			}
		}
	}

	public void InputContent(string Content)
	{
		ChatText component = Object.Instantiate(ChatTextPrefab).GetComponent<ChatText>();
		component.transform.SetParent(base.transform);
		component.GetContent(Content);
		chatTexts.Add(component);
		ChatTextupdate();
	}

	public void InputContent(string Content, Color32 color)
	{
		ChatText component = Object.Instantiate(ChatTextPrefab).GetComponent<ChatText>();
		component.transform.SetParent(base.transform);
		component.GetContent(Content, color);
		chatTexts.Add(component);
		ChatTextupdate();
	}

	private void ChatTextupdate()
	{
		if (chatTexts.Count > 20)
		{
			Object.Destroy(chatTexts[0].gameObject);
			chatTexts.Remove(chatTexts[0]);
		}
		if (rectTransform.sizeDelta.y > 400f)
		{
			scroll = ((int)rectTransform.sizeDelta.y - 400) / 50;
		}
	}
}
