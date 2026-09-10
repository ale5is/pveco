using UnityEngine;

public class CharredCatapult : CharredZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.CharredZombieCatapult;
}
