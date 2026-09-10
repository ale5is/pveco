using System.Collections.Generic;
using UnityEngine;

public class SpikeRock : PlantBase
{
	private int noFlatNum = 9;

	public SpriteRenderer Spike1;

	public SpriteRenderer Spike2;

	public SpriteRenderer Spike3;

	public SpriteRenderer Eyebrow1;

	public SpriteRenderer Eyebrow2;

	public Texture2D SleepEye;

	public override float MaxHp => 300f;

	public override bool ZombieCanEat => false;

	protected override Vector2 offSet => new Vector2(0f, -0.3f);

	protected override int attackValue => 25;

	protected override bool HaveShadow => false;

	public override PlantType BasePlant => PlantType.Spike;

	public override bool CanCarryed => false;

	public override bool CanProtect => false;

	public override bool IsLowPlant => true;

	protected override bool haveSpEye => true;

	public override int BasePlantSunNum => 100;

	protected override void OnInitForAll()
	{
		noFlatNum = 9;
		Spike1.enabled = true;
		Spike2.enabled = true;
		Spike3.enabled = true;
		SetSpriteEnable(Spike1.transform.name, enable: true);
		SetSpriteEnable(Spike2.transform.name, enable: true);
		SetSpriteEnable(Spike3.transform.name, enable: true);
	}

	protected override void OnInitForPlace()
	{
		StartActionCD(1.4f, isOver: true);
	}

	protected override bool DoAction()
	{
		return CheckAttack();
	}

	public override void SpecialAnimEvent1()
	{
		Attack();
		SetAnimChange(12);
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		if (ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 0.69f, isHypno, needCapsule: true).Count > 0)
		{
			SetAnimChange(11);
			return true;
		}
		return false;
	}

	private void Attack()
	{
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 0.69f, isHypno, needCapsule: true);
		if (zombiesByLine.Count != 0)
		{
			for (int i = 0; i < zombiesByLine.Count; i++)
			{
				zombiesByLine[i].Hurt(GetAttackValue(), Vector2.up, isHard: false);
			}
		}
	}

	protected override float HandleHurt(float hurt, bool isFlat)
	{
		if (isFlat && noFlatNum > 0)
		{
			noFlatNum--;
			if (noFlatNum <= 6 && noFlatNum > 3)
			{
				Spike1.enabled = false;
				Spike2.enabled = true;
				SetSpriteEnable(Spike1.transform.name, enable: false);
				SetSpriteEnable(Spike2.transform.name, enable: true);
			}
			else if (noFlatNum > 0 && noFlatNum <= 3)
			{
				Spike1.enabled = false;
				Spike2.enabled = false;
				SetSpriteEnable(Spike1.transform.name, enable: false);
				SetSpriteEnable(Spike2.transform.name, enable: false);
			}
			else if (noFlatNum == 0)
			{
				Spike3.enabled = false;
				SetSpriteEnable(Spike3.transform.name, enable: false);
			}
			if (noFlatNum > -1)
			{
				return 0f;
			}
			return hurt;
		}
		return hurt;
	}

	protected override void GoAwakeSpecial()
	{
		Eyebrow1.enabled = true;
		Eyebrow2.enabled = true;
		EyeREnderer.material.SetTexture("_EyeTex", null);
	}

	protected override void GoSleepSpecial()
	{
		Eyebrow1.enabled = false;
		Eyebrow2.enabled = false;
		EyeREnderer.material.SetTexture("_EyeTex", SleepEye);
	}

	protected override void SetEyeTex(int EyeType)
	{
		PlayAnim("anim_blink", 1);
	}
}
