using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class UmbrellaleafZombie : PlantZombie
{
	private int CurrSortOrder;

	private bool isStruct;

	public List<SpriteRenderer> LeafRenderers = new List<SpriteRenderer>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.UmbrallaZombie;

	protected override void PlantZombieInit()
	{
		isStruct = false;
		CurrSortOrder = Sorting.sortingOrder;
		ArmorHpState = new List<int> { 500, 250, 0 };
		ArmorHpStateSprite = new List<Sprite> { armor1, armor2, null };
		for (int i = 0; i < LeafRenderers.Count; i++)
		{
			LeafRenderers[i].bounds = new Bounds(base.transform.position, Vector3.one * 10f);
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (!base.InWater)
		{
			Sorting.sortingOrder = GetBulletSortOrder();
			PlantHeadAnimator.GetComponent<SortingGroup>().sortingOrder = 25;
		}
	}

	public override void SpecialAnimEvent2()
	{
		SetAnimChange(12);
		Sorting.sortingOrder = CurrSortOrder;
		PlantHeadAnimator.GetComponent<SortingGroup>().sortingOrder = 3;
		if (isStruct)
		{
			base.Hp = 0;
		}
	}

	public bool Block()
	{
		if (!NormalDontAttackCondition())
		{
			SetAnimChange(11);
		}
		return !NormalDontAttackCondition();
	}

	public bool StruckThis()
	{
		if (!NormalDontAttackCondition())
		{
			isStruct = true;
			SetAnimChange(11);
		}
		ServerSendSyn(1);
		return !NormalDontAttackCondition();
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			RandomSpeed = syn.Twofloat.x;
			base.Speed = DefSpeed;
		}
		if (syn.SynCode[1] == 1)
		{
			StruckThis();
		}
	}
}
