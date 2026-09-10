using UnityEngine;
using UnityEngine.Rendering;

public class PotatoParticle : MonoBehaviour
{
	public ParticleSystem Ps;

	public SpriteRenderer mashed;

	public SpriteRenderer sproing;

	private void Start()
	{
		GetComponent<Animator>().Play("PotatoShake", 0, 0f);
		ParticleSystem.MainModule main = Ps.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	public void CreateInit(int sortOrder)
	{
		GetComponent<SortingGroup>().sortingOrder = sortOrder + 1;
		mashed.sortingOrder = sortOrder;
		sproing.sortingOrder = sortOrder + 1;
	}

	private void OnParticleSystemStopped()
	{
		Object.Destroy(base.gameObject);
	}
}
