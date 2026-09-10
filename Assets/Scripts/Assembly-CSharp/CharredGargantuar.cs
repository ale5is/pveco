using System.Collections.Generic;
using UnityEngine;

public class CharredGargantuar : CharredZombie
{
	public List<SpriteRenderer> ImpSprites = new List<SpriteRenderer>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.CharredZombieGargantuar;

	protected override void OwnerInit(int SpCode)
	{
		for (int i = 0; i < ImpSprites.Count; i++)
		{
			ImpSprites[i].enabled = SpCode == 1;
		}
	}
}
