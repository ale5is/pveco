using System.Collections.Generic;
using UnityEngine;

public class TrackBullet : MonoBehaviour
{
	private PlantBase targetplant;

	private ZombieBase targetzombie;

	private Vector3 startPos;

	private Vector3 midPos;

	private Vector3 lastTargetPos;

	private Vector3 lastPos;

	private float percent;

	private float percentSpeed;

	private bool isHit;

	private bool isHypno;

	protected int attackValue;

	public Transform Bullet;

	public void Init(Vector2 pos, int attackValue, bool isHyp)
	{
		percent = 0f;
		isHit = false;
		isHypno = isHyp;
		targetplant = null;
		targetzombie = null;
		this.attackValue = attackValue;
		startPos = pos;
		lastPos = pos - new Vector2(1f, 1f);
		base.transform.position = pos;
		Bullet.rotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
	}

	private Vector2 GetMiddlePosition(Vector2 a, Vector2 b)
	{
		Vector2 vector = Vector2.Lerp(a, b, 0.5f);
		Vector2 normalized = Vector2.Perpendicular(a - b).normalized;
		float num = -0.5f;
		float num2 = 0.3f;
		return vector + (b - a).magnitude * normalized * num * num2;
	}

	private void Update()
	{
		if (Time.timeScale == 0f || isHit)
		{
			return;
		}
		if (targetzombie != null && (!targetzombie.gameObject.activeSelf || targetzombie.Hp <= 0 || (!targetzombie.collider2d.enabled && !(targetzombie is BalloonZombie))))
		{
			targetzombie = null;
		}
		if (targetplant != null && (!targetplant.gameObject.activeSelf || targetplant.Hp <= 0f))
		{
			targetplant = null;
		}
		if (targetzombie == null && targetplant == null)
		{
			GetTargetZombie();
			if (targetzombie == null && targetplant == null)
			{
				Vector3 position = base.transform.position;
				Vector3 normalized = (position - lastPos).normalized;
				base.transform.position += 3f * normalized * Time.deltaTime;
				if (base.transform.position != position)
				{
					lastPos = position;
				}
			}
			else
			{
				percent = 0f;
				startPos = base.transform.position;
				if (targetplant == null)
				{
					percentSpeed = 4f / (targetzombie.transform.position - base.transform.position).magnitude;
					lastTargetPos = targetzombie.transform.position;
				}
				else
				{
					percentSpeed = 4f / (targetplant.transform.position - base.transform.position).magnitude;
					lastTargetPos = targetplant.transform.position;
				}
				midPos = GetMiddlePosition(startPos, lastTargetPos);
			}
		}
		else
		{
			percent += percentSpeed * Time.deltaTime;
			if (targetzombie != null)
			{
				lastTargetPos = targetzombie.transform.position;
			}
			if (targetplant != null)
			{
				lastTargetPos = targetplant.transform.position;
			}
			lastPos = base.transform.position;
			base.transform.position = MyTool.Bezier(percent, startPos, midPos, lastTargetPos);
		}
		if (percent >= 1f)
		{
			if (targetzombie != null)
			{
				targetzombie.Hurt(attackValue, Vector2.zero);
			}
			if (targetplant != null)
			{
				targetplant.Hurt(attackValue, Vector2.zero, null);
			}
			Destroy();
		}
		if (percent > 1f)
		{
			percent = 1f;
		}
		if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			Destroy();
		}
		Vector3 toDirection = lastPos - base.transform.position;
		toDirection.z = 0f;
		Bullet.rotation = Quaternion.FromToRotation(Vector3.left, toDirection);
	}

	private void GetTargetZombie()
	{
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		ZombieBase zombieBase = null;
		if (isHypno)
		{
			float num = float.MinValue;
			for (int i = 0; i < allZombies.Count; i++)
			{
				if (allZombies[i].transform.position.x > num && allZombies[i].Hp > 0 && (allZombies[i].collider2d.enabled || allZombies[i] is BalloonZombie))
				{
					zombieBase = allZombies[i];
					num = allZombies[i].transform.position.x;
				}
			}
		}
		else
		{
			float num2 = float.MaxValue;
			for (int j = 0; j < allZombies.Count; j++)
			{
				if (allZombies[j].transform.position.x < num2 && allZombies[j].Hp > 0 && (allZombies[j].collider2d.enabled || allZombies[j] is BalloonZombie))
				{
					zombieBase = allZombies[j];
					num2 = allZombies[j].transform.position.x;
				}
			}
		}
		targetzombie = zombieBase;
		if (!(targetzombie == null))
		{
			return;
		}
		List<PlantBase> allPlant = MapManager.Instance.GetAllPlant(base.transform.position, !isHypno);
		PlantBase plantBase = null;
		if (isHypno)
		{
			float num3 = float.MinValue;
			for (int k = 0; k < allPlant.Count; k++)
			{
				if (allPlant[k].transform.position.x > num3 && allPlant[k].Hp > 0f)
				{
					plantBase = allPlant[k];
					num3 = allPlant[k].transform.position.x;
				}
			}
		}
		else
		{
			float num4 = float.MaxValue;
			for (int l = 0; l < allPlant.Count; l++)
			{
				if (allPlant[l].transform.position.x < num4 && allPlant[l].Hp > 0f)
				{
					plantBase = allPlant[l];
					num4 = allPlant[l].transform.position.x;
				}
			}
		}
		targetplant = plantBase;
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.CattailBullet, base.gameObject);
	}
}
