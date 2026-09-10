using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Melon : BulletPult
{
	protected override float Speed => 6f;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Melon;

	protected override void HitEvent(PlantBase plant, ZombieBase zombie, int sortOrder)
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.melonimpact, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.melonimpact2, base.transform.position);
		}
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.MelonParticle);
		obj.transform.position = base.transform.position;
		obj.transform.GetComponent<SortingGroup>().sortingOrder = sortOrder;
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2f, isHypno, needCapsule: true);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i] != zombie)
			{
				zombies[i].Hurt(attackValue / 2, Vector2.down);
			}
		}
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, !isHypno);
		for (int j = 0; j < aroundPlant.Count; j++)
		{
			aroundPlant[j].Hurt(attackValue / 2, Vector2.down, null);
		}
	}
}
