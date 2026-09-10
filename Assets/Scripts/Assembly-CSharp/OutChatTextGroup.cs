using System.Collections;
using UnityEngine;

public class OutChatTextGroup : MonoBehaviour
{
	public static OutChatTextGroup Instance;

	public GameObject ChatTextPrefab;

	private void Awake()
	{
		Instance = this;
	}

	public void InputContent(string Content)
	{
		ChatText component = Object.Instantiate(ChatTextPrefab).GetComponent<ChatText>();
		component.transform.SetParent(base.transform);
		component.GetContent(Content);
		StartCoroutine(ClearChatText(component));
	}

	public void InputContent(string Content, Color32 color)
	{
		ChatText component = Object.Instantiate(ChatTextPrefab).GetComponent<ChatText>();
		component.transform.SetParent(base.transform);
		component.GetContent(Content, color);
		StartCoroutine(ClearChatText(component));
	}

	private IEnumerator ClearChatText(ChatText chat)
	{
		yield return new WaitForSeconds(5f);
		Object.Destroy(chat.gameObject);
	}
}
