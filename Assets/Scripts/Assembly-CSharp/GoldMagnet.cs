using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMagnet : PlantBase
{
	private bool CDover;

	private bool CheckGet;

	private List<SpriteRenderer> Equips = new List<SpriteRenderer>();

	public override float MaxHp => 300f;

	public override int BasePlantSunNum => 100;

	public override PlantType BasePlant => PlantType.Magnetshroom;

	protected override void OnInitForPlace()
	{
		isClosingEye = false;
		CDover = true;
		StartActionCD(1f, isOver: false);
	}

	protected override bool DoAction()
	{
		if (CDover)
		{
			CheckIronCoin();
		}
		else
		{
			if (Equips.Count > 0)
			{
				SpriteRenderer spriteRenderer = Equips[0];
				Equips.Remove(spriteRenderer);
				Object.Destroy(spriteRenderer.gameObject);
			}
			if (Equips.Count != 0)
			{
				return true;
			}
			CDover = true;
			Equips.Clear();
			SetAnimChange(14);
			StartActionCD(1f, isOver: false);
		}
		return false;
	}

	public override void SpecialAnimEvent1()
	{
		GetCoin();
		GetIron();
	}

	public override void SpecialAnimEvent2()
	{
		CheckGet = false;
		if (Equips.Count > 0)
		{
			CDover = false;
			StartActionCD(7f, isOver: false);
			SetAnimChange(13);
		}
	}

	protected override void OnInitForAll()
	{
	}

	private bool CheckIronCoin()
	{
		if (CheckGet)
		{
			return false;
		}
		if (NormalDontAttackCondition())
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < PlayerManager.Instance.Coins.Count; i++)
		{
			if (Vector3.Distance(PlayerManager.Instance.Coins[i].transform.position, base.transform.position) < 15f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
			for (int j = 0; j < allZombies.Count; j++)
			{
				if (allZombies[j].GetEquipSprite(needClearEquip: false) != null)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			CheckGet = true;
			SetAnimChange(11);
		}
		return flag;
	}

	private void GetIron()
	{
		SetAnimChange(12);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.magnetshroom, base.transform.position);
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		for (int i = 0; i < allZombies.Count; i++)
		{
			if (allZombies[i].GetEquipSprite(needClearEquip: false) != null)
			{
				SpriteRenderer equipSprite = allZombies[i].GetEquipSprite(needClearEquip: false);
				SpriteRenderer component = Object.Instantiate(NormalSprite.Instance.SpriteDisplay).GetComponent<SpriteRenderer>();
				component.sprite = equipSprite.sprite;
				component.transform.localScale = equipSprite.transform.localScale;
				component.transform.rotation = equipSprite.transform.rotation;
				component.transform.position = equipSprite.transform.position;
				Equips.Add(component);
				allZombies[i].GetEquipSprite(needClearEquip: true);
			}
			if (Equips.Count >= 3)
			{
				break;
			}
		}
		if (Equips.Count > 0)
		{
			DoFly();
			if (PlacePlayer == GameManager.Instance.LocalPlayerSave.playerName)
			{
				StatsManager.Instance.AddStatsNum(StatsEnum.AbsorbEquipNum, Equips.Count);
			}
		}
	}

	public void DoFly()
	{
		for (int i = 0; i < Equips.Count; i++)
		{
			StartCoroutine(Fly(base.transform.position + new Vector3(0.3f, 0.3f), Equips[i]));
			Equips[i].sortingOrder = 2200;
		}
	}

	private IEnumerator Fly(Vector3 pos, SpriteRenderer Equip)
	{
		while (Vector3.Distance(pos, Equip.transform.position) > 0.1f)
		{
			yield return null;
			Equip.transform.position = Vector2.MoveTowards(Equip.transform.position, pos, 10f * Time.deltaTime);
		}
		Equip.sortingOrder = animatorSorting.sortingOrder + 1;
	}

	private void GetCoin()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.magnetshroom, base.transform.position);
		MapBase mapBase = MapManager.Instance.GetCurrMap(base.transform.position);
		int num = 0;
		for (int i = 0; i < PlayerManager.Instance.Coins.Count; i++)
		{
			if (MapManager.Instance.GetCurrMap(PlayerManager.Instance.Coins[i].transform.position) == mapBase)
			{
				PlayerManager.Instance.Coins[i].DoFlytoMagnet(base.transform.position);
				num++;
			}
			if (num > 4)
			{
				break;
			}
		}
	}

	protected override void GoAwakeSpecial()
	{
	}

	protected override void GoSleepSpecial()
	{
	}

	protected override void DeadEvent()
	{
		for (int i = 0; i < Equips.Count; i++)
		{
			Object.Destroy(Equips[i].gameObject);
		}
		Equips.Clear();
		CDover = true;
	}
}
