using System.Collections.Generic;
using UnityEngine;

public class StarveZombie : ZombieBase
{
	public SpriteRenderer BodyRenderer;

	public SpriteRenderer PuffyEyeRenderer;

	public SpriteRenderer Paunch4Renderer;

	public SpriteRenderer Paunch5Renderer;

	public List<Sprite> FatHeads = new List<Sprite>();

	public List<Sprite> FatBodys = new List<Sprite>();

	private int EatNum;

	private int CurrMaxHp;

	private bool canEatByChomper;

	public override int MaxHP => 720;

	protected override GameObject Prefab => GameManager.Instance.GameConf.StarveZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 4f;

	protected override float attackValue => 80f;

	protected override int CriticalHp => 100;

	protected override float OutWaterDistance => 0.35f;

	protected override float inWaterDepth => 0.7f;

	public override bool CanEatByChomper => canEatByChomper;

	public override void InitZombieHpState()
	{
		EatNum = 0;
		CurrMaxHp = MaxHP;
		canEatByChomper = true;
		HeadRenderer.sprite = FatHeads[0];
		BodyRenderer.sprite = FatBodys[0];
		Paunch4Renderer.enabled = false;
		Paunch5Renderer.enabled = false;
		PuffyEyeRenderer.enabled = false;
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			base.AddSpeed = 1f;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < (float)CurrMaxHp * 0.4f * base.HpScale)
		{
			DropArm();
		}
		if (EatNum >= 32 && (float)base.Hp < (float)CurrMaxHp * 0.5f * base.HpScale)
		{
			PuffyEyeRenderer.enabled = true;
		}
		if (HitSound)
		{
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
		}
	}

	protected override void OnAttackEvent()
	{
		EatNum++;
		int num = 0;
		bool flag = false;
		if (EatNum == 5)
		{
			flag = true;
			num = 1;
		}
		if (EatNum == 18)
		{
			flag = true;
			num = 2;
		}
		if (EatNum == 32)
		{
			flag = true;
			num = 3;
			canEatByChomper = false;
			Paunch4Renderer.enabled = true;
			Paunch5Renderer.enabled = false;
		}
		if (EatNum == 48)
		{
			flag = true;
			num = 4;
			Paunch4Renderer.enabled = false;
			Paunch5Renderer.enabled = true;
		}
		if (flag)
		{
			base.Hp += 850;
			CurrMaxHp += 850;
			base.AddSpeed = 1f - (float)num * 0.3f;
			HeadRenderer.sprite = FatHeads[num];
			BodyRenderer.sprite = FatBodys[num];
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (EatNum < 21)
		{
			AnimFailSound();
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
		}
	}
}
