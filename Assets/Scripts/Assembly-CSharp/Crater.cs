using System.Collections;
using UnityEngine;

public class Crater : MonoBehaviour
{
	public Sprite Crater1;

	public Sprite Crater2;

	public Sprite WaterCrater1;

	public Sprite WaterCrater2;

	public Sprite HardCrater1;

	public Sprite HardCrater2;

	private SpriteRenderer renderer1;

	private Grid currGrid;

	public void CreateInit(bool isWater, Grid grid)
	{
		currGrid = grid;
		renderer1 = base.transform.GetComponent<SpriteRenderer>();
		if (isWater)
		{
			renderer1.sprite = WaterCrater1;
		}
		else if (grid.isHardGrid)
		{
			renderer1.sprite = HardCrater1;
		}
		else
		{
			renderer1.sprite = Crater1;
		}
		StartCoroutine(Fade());
	}

	private IEnumerator Fade()
	{
		yield return new WaitForSeconds(90f);
		if (renderer1.sprite == Crater1)
		{
			renderer1.sprite = Crater2;
		}
		else if (renderer1.sprite == WaterCrater1)
		{
			renderer1.sprite = WaterCrater2;
		}
		else
		{
			renderer1.sprite = HardCrater2;
		}
		yield return new WaitForSeconds(90f);
		currGrid.HaveCrater = false;
	}

	public void DestoryThis()
	{
		StopAllCoroutines();
		Object.Destroy(base.gameObject);
	}
}
