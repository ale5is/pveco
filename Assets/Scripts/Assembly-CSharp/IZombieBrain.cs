using UnityEngine;

public class IZombieBrain : ZombieBase
{
	public override int MaxHP => 300;

	protected override GameObject Prefab => GameManager.Instance.GameConf.IZombieBrain;

	protected override float AnToSpeed => 1f;

	protected override float DefSpeed => 1f;

	protected override float attackValue => 0f;

	public override void InitZombieHpState()
	{
		anCanMove = false;
		canIce = false;
		canButter = false;
		canFrozen = false;
		canDizzy = false;
		onlyBoomHurt = true;
	}

	public override void ZombieOnDead(bool dropItem)
	{
		LvItemManager.Instance.IZBrainDead(this);
		if (dropItem)
		{
			LvItemManager.Instance.DropCoin(base.transform.position, AlwaysDrop: true);
		}
	}
}
