using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Splash : MonoBehaviour
{
	private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	public void CreateInit(Vector2 pos, int line, float scale = 1f)
	{
		base.transform.localScale = new Vector2(1.6f, 1.6f) * scale;
		base.transform.position = pos + new Vector2(0f, -0.8f);
		base.transform.SetParent(PlantManager.Instance.transform);
		GetComponent<SortingGroup>().sortingOrder = line * 200 + FixedInfo.Splash;
		Color color = MapManager.Instance.GetCurrMap(base.transform.position).SplashColor;
		if (sprites.Count == 0)
		{
			SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
			sprites.AddRange(componentsInChildren);
		}
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].color = color;
		}
	}

	public void EndToClear()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Splash, base.gameObject);
	}
}
