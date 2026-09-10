using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melt : MonoBehaviour
{
	public Sprite Melt1;

	public Sprite Melt2;

	private float duration;

	private Grid currGrid;

	private bool isHypno;

	private SpriteRenderer REnderer;

	public void CreateInit(int type, int line, Vector3 pos)
	{
		float currTempt = MapManager.Instance.GetNearestMap(pos).CurrTempt;
		float num = 1f + (currTempt - 30f) / 40f;
		if (num < 1f)
		{
			num = 1f;
		}
		if (num > 1.5f)
		{
			num = 1.5f;
		}
		duration = 5f * num;
		currGrid = MapManager.Instance.GetGridByWorldPos(pos, line);
		REnderer = base.transform.GetComponent<SpriteRenderer>();
		if (type == 1)
		{
			REnderer.sprite = Melt1;
		}
		else
		{
			REnderer.sprite = Melt2;
		}
		base.transform.position = new Vector3(pos.x, currGrid.Position.y - 0.4f, pos.z);
		StartCoroutine(StartRun());
		REnderer.sortingOrder = currGrid.Point.y * 200 + 1;
	}

	private IEnumerator StartRun()
	{
		float a = 0f;
		while (a < 1f)
		{
			a += Time.deltaTime * 2f;
			yield return null;
			REnderer.color = new Color(REnderer.color.r, REnderer.color.g, REnderer.color.b, a);
		}
		while (true)
		{
			yield return new WaitForSeconds(0.6f);
			duration -= 0.6f;
			if (duration <= 0f)
			{
				break;
			}
			List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(currGrid.Point.y, base.transform.position, 0.6f, isHypno, needCapsule: true);
			for (int i = 0; i < zombiesByLine.Count; i++)
			{
				zombiesByLine[i].Hurt(60, Vector2.up, isHard: false, HitSound: false);
			}
			if (currGrid.CurrPlantBase != null && currGrid.CurrPlantBase.isHypno != isHypno && Mathf.Abs(base.transform.position.x - currGrid.Position.x) < 0.6f)
			{
				currGrid.CurrPlantBase.Hurt(60f, Vector2.up, null);
			}
		}
		a = 1f;
		while (a > 0f)
		{
			a -= Time.deltaTime * 2f;
			yield return null;
			REnderer.color = new Color(REnderer.color.r, REnderer.color.g, REnderer.color.b, a);
		}
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Melt, base.gameObject);
	}
}
