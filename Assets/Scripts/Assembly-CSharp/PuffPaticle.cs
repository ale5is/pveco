using UnityEngine;

public class PuffPaticle : MonoBehaviour
{
	public ParticleSystem Ps;

	private void Start()
	{
		ParticleSystem.MainModule main = Ps.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	private void OnParticleSystemStopped()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.PuffParticle, base.gameObject);
	}
}
