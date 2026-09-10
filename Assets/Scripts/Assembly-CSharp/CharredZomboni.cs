using UnityEngine;

public class CharredZomboni : CharredZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.CharredZombieZomboni;
}
