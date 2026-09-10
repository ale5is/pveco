using UnityEngine;

public class CustomScence : MonoBehaviour
{
	public static CustomScence Instance;

	public GameObject Plantern;

	public Transform Selector;

	public CustomMapPanel mapPanel;

	public CustomLevelPanel levelPanel;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Plantern.SetActive(value: false);
	}

	public void OpenInit()
	{
		Plantern.SetActive(value: true);
	}

	public void Close()
	{
		Plantern.SetActive(value: false);
	}

	public void SetSelectorPos(Vector2 pos)
	{
		Selector.position = pos;
	}

	public void OpenCustomMap()
	{
		mapPanel.OpenInit();
	}

	public void OpenCustomLevel()
	{
		levelPanel.OpenInit();
	}
}
