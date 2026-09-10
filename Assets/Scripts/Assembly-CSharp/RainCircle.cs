using UnityEngine;

public class RainCircle : MonoBehaviour
{
	public void PlayOver()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Rain_circle, base.gameObject);
	}
}
