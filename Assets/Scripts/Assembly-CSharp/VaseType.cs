using System;

[Serializable]
public class VaseType
{
	public int PlaceX;

	public PlantType plantType;

	public ZombieType zombieType;

	public VaseType(int lineX, PlantType type)
	{
		PlaceX = lineX;
		plantType = type;
		zombieType = ZombieType.Nope;
	}

	public VaseType(int lineX, ZombieType type)
	{
		PlaceX = lineX;
		plantType = PlantType.Nope;
		zombieType = type;
	}
}
