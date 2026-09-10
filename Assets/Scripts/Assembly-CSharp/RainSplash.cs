using UnityEngine;

public class RainSplash : MonoBehaviour
{
	public void PlayOver()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Rain_splash, base.gameObject);
	}
}
