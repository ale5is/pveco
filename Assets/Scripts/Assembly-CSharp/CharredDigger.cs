using UnityEngine;

public class CharredDigger : CharredZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.CharredZombieDigger;

	protected override void OwnerInit(int SpCode)
	{
		if (SpCode == 1)
		{
			animator.Play("anim_crumble_noaxe", 0, 0f);
		}
	}
}
