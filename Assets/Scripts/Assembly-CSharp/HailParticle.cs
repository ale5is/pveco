using UnityEngine;

public class HailParticle : MonoBehaviour
{
	private void Start()
	{
		ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	private void OnParticleSystemStopped()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.HailParticle, base.gameObject);
	}
}
