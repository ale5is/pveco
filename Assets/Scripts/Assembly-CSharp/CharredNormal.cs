using UnityEngine;

public class CharredNormal : CharredZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.CharredZombie;
}
