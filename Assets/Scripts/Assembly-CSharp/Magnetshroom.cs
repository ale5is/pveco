using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetshroom : PlantBase
{
	private bool CDover;

	private bool CheckGet;

	private SpriteRenderer Equip;

	public Sprite NormalEyeSpite;

	public SpriteRenderer Eye1Renderer;

	public SpriteRenderer Eye2Renderer;

	public override float MaxHp => 300f;

	protected override bool isShroom => true;

	protected override bool haveSpEye => true;

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
			CheckIron();
		}
		else
		{
			CdOver();
		}
		return false;
	}

	public override void SpecialAnimEvent1()
	{
		GetIron();
	}

	public override void SpecialAnimEvent2()
	{
		CheckGet = false;
	}

	private bool CheckIron()
	{
		if (CheckGet)
		{
			return false;
		}
		if (NormalDontAttackCondition())
		{
			return false;
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 4.5f, isHypno, needCapsule: false);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i].GetEquipSprite(needClearEquip: false) != null)
			{
				CheckGet = true;
				SetAnimChange(11);
				return true;
			}
		}
		return false;
	}

	private void GetIron()
	{
		SetAnimChange(16);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 4.5f, isHypno, needCapsule: false);
		for (int i = 0; i < zombies.Count; i++)
		{
			if (zombies[i].GetEquipSprite(needClearEquip: false) != null)
			{
				SpriteRenderer equipSprite = zombies[i].GetEquipSprite(needClearEquip: false);
				Equip = Object.Instantiate(NormalSprite.Instance.SpriteDisplay).GetComponent<SpriteRenderer>();
				Equip.sprite = equipSprite.sprite;
				Equip.transform.localScale = equipSprite.transform.localScale;
				Equip.transform.rotation = equipSprite.transform.rotation;
				Equip.transform.position = equipSprite.transform.position;
				CDover = false;
				DoFly();
				zombies[i].GetEquipSprite(needClearEquip: true);
				if (PlacePlayer == GameManager.Instance.LocalPlayerSave.playerName)
				{
					StatsManager.Instance.AddStatsNum(StatsEnum.AbsorbEquipNum);
				}
				SetAnimChange(15);
				StartActionCD(15f, isOver: false);
				break;
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.magnetshroom, base.transform.position);
	}

	private void CdOver()
	{
		CDover = true;
		Object.Destroy(Equip.gameObject);
		Equip = null;
		SetAnimChange(12);
		StartActionCD(1f, isOver: false);
	}

	public void DoFly()
	{
		StartCoroutine(Fly(base.transform.position + new Vector3(0.3f, 0.3f)));
		Equip.sortingOrder = 2200;
	}

	private IEnumerator Fly(Vector3 pos)
	{
		while (Vector3.Distance(pos, Equip.transform.position) > 0.1f)
		{
			yield return null;
			Equip.transform.position = Vector2.MoveTowards(Equip.transform.position, pos, 10f * Time.deltaTime);
		}
		Equip.sortingOrder = animatorSorting.sortingOrder + 1;
	}

	protected override void DeadEvent()
	{
		if (Equip != null)
		{
			Object.Destroy(Equip.gameObject);
		}
		Equip = null;
	}

	protected override void GoAwakeSpecial()
	{
		SetAnimChange(14);
	}

	protected override void GoSleepSpecial()
	{
		SetAnimChange(13);
	}

	protected override void SetEyeTex(int EyeType)
	{
		switch (EyeType)
		{
		case 0:
			Eye1Renderer.sprite = NormalEyeSpite;
			Eye2Renderer.sprite = NormalEyeSpite;
			break;
		case 1:
			Eye1Renderer.sprite = eye1;
			Eye2Renderer.sprite = eye1;
			break;
		case 2:
			Eye1Renderer.sprite = eye2;
			Eye2Renderer.sprite = eye2;
			break;
		}
	}
}
