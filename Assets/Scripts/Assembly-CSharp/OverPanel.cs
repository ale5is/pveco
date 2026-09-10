using UnityEngine;

public class OverPanel : MonoBehaviour
{
	public void Over()
	{
		base.gameObject.SetActive(value: true);
		GetComponent<Animator>().Play("OverPanel", 0, 0f);
	}

	public void AnimtionOver()
	{
		LVManager.Instance.OverPanelEvent();
	}
}
