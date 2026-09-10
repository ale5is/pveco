using System.Collections.Generic;
using UnityEngine.Events;

public class SubLv
{
	public float BungiSpRate;

	public List<LVSpState> LvSpStates = new List<LVSpState>();

	public int CardNum;

	public bool StartCardCd;

	public bool BanMultyCardAdd;

	public int BeltTimeAdd;

	public BankType CurrBankType;

	public SeedBankType CurrSeedBankType;

	public List<CardType> GeneralCardPool = new List<CardType>();

	public List<CardType> FixedCard = new List<CardType>();

	public List<int> BigWaveNum = new List<int>();

	public List<List<int>> Weights = new List<List<int>>();

	public List<List<ZombieType>> ZombieTypes = new List<List<ZombieType>>();

	public List<ZombieType> GraveZombie = new List<ZombieType>
	{
		ZombieType.NormalZombie,
		ZombieType.ConeZombie,
		ZombieType.BucketZombie
	};

	public int WaterZombieNum;

	public List<ZombieType> WaterZombie = new List<ZombieType>();

	public List<ZombieType> BigWaveFixedZombie = new List<ZombieType>();

	public UnityAction StartAction;

	public UnityAction BigWaveAction;

	public Dictionary<int, UnityAction> TimeAction = new Dictionary<int, UnityAction>();

	public List<List<List<PlantType>>> StartPlants = new List<List<List<PlantType>>>();

	public int PlantVaseNum;

	public int ZombieVaseNum;

	public List<List<List<VaseType>>> VaseBreakerVase = new List<List<List<VaseType>>>();
}
