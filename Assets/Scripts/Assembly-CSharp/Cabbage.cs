using UnityEngine;
using UnityEngine.Rendering;

public class Cabbage : BulletPult
{
	protected override float Speed => 6f;

	protected override bool NeedPeaAudio => true;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Cabbage;

	protected override void HitEvent(PlantBase plant, ZombieBase zombie, int sortOrder)
	{
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PeaParticle);
		obj.transform.position = base.transform.position + new Vector3(Random.Range(0.1f, 0.2f), Random.Range(0.1f, 0.2f));
		obj.transform.GetComponent<SortingGroup>().sortingOrder = sortOrder;
	}
}
