using UnityEngine;

public class CharredImp : CharredZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.CharredZombieImp;
}
