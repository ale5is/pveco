using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class IceShroomZombie : PlantZombie
{
	private int BoomNum;

	private Grid endGrid;

	protected override GameObject Prefab => GameManager.Instance.GameConf.IceShroomZombie;

	protected override int attackValue2 => 30;

	protected override int OwnerHp => 500;

	protected override bool DoAction()
	{
		CheckBoom();
		return false;
	}

	protected override void PlantZombieInit()
	{
		canFrozen = false;
		canIce = false;
		BoomNum = 0;
		StartActionCD(1f, isOver: true);
		endGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		BoomNum++;
	}

	private void CheckBoom()
	{
		if (LV.Instance.CurrLVType == LVType.VaseBreaker)
		{
			Ice();
		}
		if (PlacePlayer == null)
		{
			if (!base.IsCriticalState && BoomNum > 0 && Random.Range(BoomNum, 18) > 16)
			{
				Ice();
			}
			if (endGrid != null && !base.IsCriticalState && Vector2.Distance(base.transform.position, endGrid.Position) < 2f)
			{
				Ice();
			}
		}
	}

	private void Ice()
	{
		base.CurrMap.FadeTempt -= 6f;
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.IceParticle).transform.position = base.transform.position;
		EffectPanel.Instance.Spark(new Color(0.02f, 1f, 0.96f, 0.3f), 0.05f, base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Frozen, base.transform.position);
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, !isHypno);
		for (int i = 0; i < allZombies.Count; i++)
		{
			allZombies[i].Frozen(Vector2.zero, isAudio: false, 8);
			allZombies[i].Ice();
			if (allZombies[i].collider2d.enabled)
			{
				allZombies[i].Hurt(attackValue2, Vector2.zero, isHard: false);
			}
		}
		List<PlantBase> allPlant = MapManager.Instance.GetAllPlant(base.transform.position, isHypno);
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
			allPlant[j].Hurt(attackValue, Vector2.zero, null);
		}
		ServerSendSyn(1);
		DirectDead(canDropItem: false, 0f);
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
			Ice();
		}
	}
}
