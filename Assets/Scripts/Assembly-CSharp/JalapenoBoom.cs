using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JalapenoBoom : MonoBehaviour
{
	public GameObject FirePrefab;

	private List<JalapenoFire> fires = new List<JalapenoFire>();

	public void CreateInit(Grid currGrid, int sortOrder)
	{
		List<Grid> lineAllGrid = MapManager.Instance.GetLineAllGrid(currGrid.Position, currGrid.Point.y);
		for (int i = 0; i < lineAllGrid.Count; i++)
		{
			if (i == 0 && !lineAllGrid[i].isOccupied)
			{
				JalapenoFire component = Object.Instantiate(FirePrefab).GetComponent<JalapenoFire>();
				component.CreateInit(lineAllGrid[i].Position + new Vector2(-1.45f, -0.3f), sortOrder);
				fires.Add(component);
			}
			JalapenoFire component2 = Object.Instantiate(FirePrefab).GetComponent<JalapenoFire>();
			component2.CreateInit(lineAllGrid[i].Position + new Vector2(0f, -0.3f), sortOrder);
			fires.Add(component2);
			if (i == lineAllGrid.Count - 1)
			{
				JalapenoFire component3 = Object.Instantiate(FirePrefab).GetComponent<JalapenoFire>();
				component3.CreateInit(lineAllGrid[i].Position + new Vector2(1.45f, -0.3f), sortOrder);
				fires.Add(component3);
			}
		}
		StartCoroutine(DisAppear());
	}

	private IEnumerator DisAppear()
	{
		for (int i = 0; i < fires.Count; i++)
		{
			yield return new WaitForSeconds(0.05f);
			fires[i].DisAppear();
		}
		yield return new WaitForSeconds(1f);
		for (int j = 0; j < fires.Count; j++)
		{
			PoolManager.Instance.PushObj(FirePrefab, fires[j].gameObject);
		}
		Object.Destroy(base.gameObject);
	}
}
