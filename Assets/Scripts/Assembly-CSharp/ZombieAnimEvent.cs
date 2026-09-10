using UnityEngine;

public class ZombieAnimEvent : MonoBehaviour
{
	public ZombieBase zombie;

	private void Start()
	{
		zombie = base.transform.parent.parent.GetComponent<ZombieBase>();
	}

	public void AnCanMove()
	{
		if ((bool)zombie)
		{
			zombie.anCanMove = true;
		}
	}

	public void AnDontMove()
	{
		if ((bool)zombie && zombie.CurrGrid != null && !zombie.CurrGrid.IsIce)
		{
			zombie.anCanMove = false;
		}
	}

	public void Attack()
	{
		zombie.AnimAttack();
	}

	public void FlatAttack()
	{
		zombie.AnimAttack(isFlat: true);
	}

	public void Dead()
	{
		zombie.DirectDead(canDropItem: true, 1f, synClient: true);
	}

	public void DirectDead()
	{
		zombie.DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public void DropHead()
	{
		zombie.DropHead();
	}

	public void AnimFailSound()
	{
		zombie.AnimFailSound();
	}

	public void SpecialEvent()
	{
		zombie.SpecialAnimEvent1();
	}

	public void SpecialEvent2()
	{
		zombie.SpecialAnimEvent2();
	}

	public void SpecialEvent3()
	{
		zombie.SpecialAnimEvent3();
	}

	public void SpecialEvent4()
	{
		zombie.SpecialAnimEvent4();
	}

	public void SpecialEvent5()
	{
		zombie.SpecialAnimEvent5();
	}

	public void SpecialEvent6()
	{
		zombie.SpecialAnimEvent6();
	}
}
