using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Tanglekelpgrab : MonoBehaviour
{
	private Tanglekelp Tanglekelp;

	private ZombieBase zombie;

	private int attackValue;

	private bool isGrabZombie;

	public void Init(Tanglekelp tanglekelp, ZombieBase zombie, int attackvalue)
	{
		attackValue = attackvalue;
		GetComponent<SortingGroup>().sortingOrder = zombie.SortOrder + 1;
		base.transform.position = new Vector2(zombie.transform.position.x - 0.82f, tanglekelp.transform.position.y);
		Tanglekelp = tanglekelp;
		this.zombie = zombie;
		isGrabZombie = zombie.Hp <= attackValue;
		StartCoroutine(Grab());
	}

	private IEnumerator Grab()
	{
		if (isGrabZombie)
		{
			zombie.TangkleStopAction();
		}
		yield return new WaitForSeconds(0.3f);
		while (base.transform.position.y > Tanglekelp.transform.position.y - 0.8f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				base.transform.position += new Vector3(0f, -4f * Time.deltaTime, 0f);
				if (isGrabZombie)
				{
					zombie.AnimTranform.transform.position += new Vector3(0f, -4f * Time.deltaTime, 0f);
				}
			}
		}
		Over();
	}

	public void Over()
	{
		if (zombie != null && zombie.isActiveAndEnabled)
		{
			if (isGrabZombie)
			{
				zombie.DirectDead(canDropItem: true, 0f);
			}
			else
			{
				zombie.Hurt(attackValue / 3, Vector2.zero, isHard: false);
			}
		}
		if (Tanglekelp != null && Tanglekelp.isActiveAndEnabled)
		{
			Tanglekelp.CreateSplash();
			Tanglekelp.Dead();
		}
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Tanglekelpgrab, base.gameObject);
	}
}
