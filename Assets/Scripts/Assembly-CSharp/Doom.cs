using UnityEngine;
using UnityEngine.Rendering;

public class Doom : MonoBehaviour
{
	public void CreateInit(Vector2 gridPos, int sort)
	{
		base.transform.position = gridPos + new Vector2(0f, 1.68f);
		base.transform.GetComponent<SortingGroup>().sortingOrder = sort;
	}

	public void PlayOver()
	{
		Object.Destroy(base.gameObject);
	}
}
