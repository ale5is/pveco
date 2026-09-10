using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public MapBase CurrMap;

	protected List<int> InLines = new List<int>();

	protected Coroutine BrightCoroutine;

	protected Collider2D collider2d;

	private void Awake()
	{
		collider2d = base.transform.GetComponent<Collider2D>();
		collider2d.enabled = false;
	}

	private void Start()
	{
		MapManager.Instance.AddObstacle(this);
	}

	public bool ContainLine(int line)
	{
		return InLines.Contains(line);
	}

	public virtual void HurtThis(float hurt)
	{
	}

	public void DestroyThis()
	{
		MapManager.Instance.RemoveObstacle(this);
		Object.Destroy(base.gameObject);
	}
}
