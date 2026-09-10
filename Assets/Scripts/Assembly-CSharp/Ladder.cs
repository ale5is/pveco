using UnityEngine;

public class Ladder : MonoBehaviour
{
	public SpriteMask SpriteMask;

	public SpriteRenderer WhiteWater;

	public void CreateInit(Grid grid, bool isLeft)
	{
		base.transform.SetParent(MapManager.Instance.GetNearestMap(grid.Position).transform);
		if (isLeft)
		{
			base.transform.position = grid.Position + new Vector2(-0.3f, 0f);
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		else
		{
			base.transform.position = grid.Position + new Vector2(0.3f, 0f);
		}
		SpriteMask.gameObject.SetActive(value: false);
		WhiteWater.gameObject.SetActive(value: false);
		if (grid.isNoIceWater)
		{
			WhiteWater.gameObject.SetActive(value: true);
			SpriteMask.gameObject.SetActive(value: true);
			base.transform.position -= new Vector3(0f, 0.4f);
		}
		int num = grid.Point.y * 200 + FixedInfo.Ladder;
		base.transform.GetComponent<SpriteRenderer>().sortingOrder = num;
		WhiteWater.sortingOrder = num + 1;
		SpriteMask.frontSortingOrder = num;
		SpriteMask.backSortingOrder = num - 1;
	}
}
