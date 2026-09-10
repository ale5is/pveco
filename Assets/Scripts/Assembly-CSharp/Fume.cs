using UnityEngine;

public class Fume : MonoBehaviour
{
	public void Init(Vector2 pos)
	{
		base.transform.position = pos;
	}

	private void Dead()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ShroomFumeParticle, base.gameObject);
	}
}
