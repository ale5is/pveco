using System.Collections.Generic;
using UnityEngine;

public class PeaParticle : MonoBehaviour
{
	public ParticleSystem Ps;

	public SpriteRenderer splat;

	public List<Sprite> splats = new List<Sprite>();

	private void Start()
	{
		splat.sprite = splats[Random.Range(0, splats.Count)];
		ParticleSystem.MainModule main = Ps.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	private void OnParticleSystemStopped()
	{
		splat.sprite = splats[Random.Range(0, splats.Count)];
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.PeaParticle, base.gameObject);
	}
}
