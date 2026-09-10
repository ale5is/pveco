using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FumeShroomZombie : PlantZombie
{
	public Transform creatBulletPos;

	protected override GameObject Prefab => GameManager.Instance.GameConf.FumeShroomZombie;

	protected override int attackValue2 => 20;

	protected override void PlantZombieInit()
	{
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		CreateFume();
	}

	public override void SpecialAnimEvent2()
	{
		HurtZombie();
		SetAnimChange(12);
		base.BaseAnimSpeed = 1f;
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.CurrGrid.Point.y, base.transform.position, base.IsFacingLeft, !isHypno);
		PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.CurrGrid.Point.y, base.IsFacingLeft, isHypno);
		if ((zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < 5.6f) || (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < 5.6f))
		{
			base.BaseAnimSpeed = 2f;
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void CreateFume()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Fume, base.transform.position);
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.ShroomFumeParticle);
		obj.transform.localScale = new Vector3(Mathf.Abs(obj.transform.localScale.x), obj.transform.localScale.y);
		obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder();
		if (base.IsFacingLeft)
		{
			obj.transform.position = creatBulletPos.transform.position;
			obj.transform.localScale = new Vector3(0f - obj.transform.localScale.x, obj.transform.localScale.y);
		}
		else
		{
			obj.transform.position = creatBulletPos.transform.position;
		}
	}

	private void HurtZombie()
	{
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.CurrGrid.Point.y, base.transform.position, 5.6f, !isHypno, needCapsule: true);
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.CurrGrid.Point.y, 5.6f, isHypno);
		for (int i = 0; i < zombiesByLine.Count; i++)
		{
			if (base.IsFacingLeft && zombiesByLine[i].transform.position.x < base.transform.position.x)
			{
				zombiesByLine[i].Hurt(attackValue2, Vector2.zero);
			}
			else if (!base.IsFacingLeft && zombiesByLine[i].transform.position.x > base.transform.position.x)
			{
				zombiesByLine[i].Hurt(attackValue2, Vector2.zero);
			}
		}
		for (int j = 0; j < linePlant.Count; j++)
		{
			if (base.IsFacingLeft && linePlant[j].transform.position.x < base.transform.position.x)
			{
				linePlant[j].Hurt(attackValue2, Vector2.zero, null);
			}
			else if (!base.IsFacingLeft && linePlant[j].transform.position.x > base.transform.position.x)
			{
				linePlant[j].Hurt(attackValue2, Vector2.zero, null);
			}
		}
	}
}
