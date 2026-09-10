using UnityEngine;

public class GridSelector : MonoBehaviour
{
	public Grid grid;

	private SpriteRenderer REnderer;

	private bool isSelected;

	private void Awake()
	{
		REnderer = base.transform.GetComponent<SpriteRenderer>();
	}

	private void OnMouseDown()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			if (isSelected)
			{
				CancelSelect();
			}
			else
			{
				SelectThis();
			}
			CreatePanel.Instance.SelectorMouseDown(this);
		}
	}

	private void OnMouseUp()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			CreatePanel.Instance.SelectorMouseUp();
		}
	}

	public void SelectThis()
	{
		isSelected = true;
		REnderer.color = Color.green;
		CreatePanel.Instance.AddGridSelection(this);
	}

	public void CancelSelect()
	{
		isSelected = false;
		REnderer.color = Color.white;
		CreatePanel.Instance.RemoveGridSelection(this);
	}
}
