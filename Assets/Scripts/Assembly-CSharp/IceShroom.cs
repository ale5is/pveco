using System.Collections.Generic;
using UnityEngine;

public class IceShroom : PlantBase
{
	public override float MaxHp => 300f;

	protected override int attackValue => 100;

	protected override bool isShroom => true;

	protected override void OnInitForAll()
	{
		canFrozen = false;
		canIce = false;
	}

	public override void SpecialAnimEvent1()
	{
		if (base.currGrid != null && !isSleeping)
		{
			Dead();
		}
	}

	protected override void DeadrattleEvent()
	{
		if (!isSleeping)
		{
			Ice();
		}
	}

	private void Ice()
	{
		if (isSleeping)
		{
			return;
		}
		base.CurrMap.FadeTempt -= 6f;
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.IceParticle).transform.position = base.transform.position;
		EffectPanel.Instance.Spark(new Color(0.02f, 1f, 0.96f, 0.3f), 0.05f, base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		for (int i = 0; i < allZombies.Count; i++)
		{
			allZombies[i].Frozen(Vector2.zero, isAudio: false, 8);
			allZombies[i].Ice();
			if (allZombies[i].collider2d.enabled)
			{
				allZombies[i].Hurt(GetAttackValue(), Vector2.zero, isHard: false);
			}
		}
		List<PlantBase> allPlant = MapManager.Instance.GetAllPlant(base.transform.position, !isHypno);
		for (int j = 0; j < allPlant.Count; j++)
		{
			if (allPlant[j].ProtectPlant != null)
			{
				allPlant[j].ProtectPlant.Ice();
			}
			if (allPlant[j].CarryPlant != null)
			{
				allPlant[j].CarryPlant.Ice();
			}
			allPlant[j].Frozen(Vector2.zero, isAudio: false, 8);
			allPlant[j].Ice();
			allPlant[j].Hurt(GetAttackValue(), Vector2.zero, null);
		}
		if (GameManager.Instance.isClient)
		{
			return;
		}
		int num = 0;
		for (int k = 0; k < allZombies.Count; k++)
		{
			if (allZombies[k].GetDead() && allZombies[k].BodyScale == 1f)
			{
				num++;
			}
		}
		if (num >= 20)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.IceShroom20, PlacePlayer);
		}
	}
}
