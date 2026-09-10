using System.Collections.Generic;
using UnityEngine;

public class GloomShroomZombie : PlantZombie
{
	public Transform creatBulletPos;

	protected override GameObject Prefab => GameManager.Instance.GameConf.GloomShroomZombie;

	protected override int attackValue2 => 20;

	protected override void PlantZombieInit()
	{
		StartActionCD(1.9f, isOver: true);
		ArmorHpState = new List<int> { 500, 250, 0 };
		ArmorHpStateSprite = new List<Sprite> { armor1, armor2, null };
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Fume, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		HurtZombie();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, !isHypno, needCapsule: true);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, isHypno);
		if (zombies.Count > 0 || aroundPlant.Count > 0)
		{
			SetAnimChange(11);
		}
		return false;
	}

	private void HurtZombie()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CircleFumeParticle).transform.position = base.transform.position;
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, !isHypno, needCapsule: true);
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].Hurt(attackValue2, Vector2.zero, isHard: false);
		}
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, isHypno);
		for (int j = 0; j < aroundPlant.Count; j++)
		{
			aroundPlant[j].Hurt(attackValue2, Vector2.zero, null);
		}
	}
}
