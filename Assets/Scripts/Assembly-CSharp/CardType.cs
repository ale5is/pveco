using System;

[Serializable]
public class CardType
{
	public PlantType plantType;

	public ZombieType zombieType;

	public CardType(PlantType plantType)
	{
		this.plantType = plantType;
	}

	public CardType(ZombieType zombieType)
	{
		this.zombieType = zombieType;
	}

	public CardType(PlantType plantType, ZombieType zombieType)
	{
		this.plantType = plantType;
		this.zombieType = zombieType;
	}
}
