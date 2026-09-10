using UnityEngine;

public class HypnoShroomZombie : PlantZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.HyponShroomZombie;

	protected override Vector2 SpeedRange => new Vector2(2f, 2.8f);

	protected override void AttackEvent(PlantBase plant, ZombieBase zombie)
	{
		if (plant != null)
		{
			plant.Hypno();
		}
		else if (zombie != null)
		{
			zombie.Hypno();
		}
		if ((bool)plant || (bool)zombie)
		{
			base.Hp = 0;
		}
	}

	protected override void PlantZombieInit()
	{
	}
}
