using System.Collections;
using UnityEngine;

public class BloverZombie : PlantZombie
{
	protected override GameObject Prefab => GameManager.Instance.GameConf.BloverZombie;

	protected override void PlantZombieInit()
	{
		StartActionCD(5f, isOver: true);
	}

	protected override bool DoAction()
	{
		_ = base.State;
		_ = 1;
		return false;
	}

	private IEnumerator Blover()
	{
		yield return new WaitForSeconds(5f);
		base.dontChangeState = false;
		base.State = ZombieState.Attack;
		base.dontChangeState = true;
	}
}
