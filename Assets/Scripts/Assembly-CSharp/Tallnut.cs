using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tallnut : PlantBase
{
	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	public SpriteRenderer NutBody;

	private int state1;

	private int state2;

	private float lastHp;

	public override float MaxHp => 8000f;

	protected override Vector2 offSet => new Vector2(0f, 0.1f);

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (base.Hp < lastHp)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.NutParticle).transform.position = base.transform.position;
		}
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			NutBody.sprite = State2;
			SetSprite(NutBody.name, NutBody.sprite);
		}
		else if (base.Hp <= (float)state2)
		{
			NutBody.sprite = State3;
			SetSprite(NutBody.name, NutBody.sprite);
		}
		else
		{
			NutBody.sprite = State1;
			SetSprite(NutBody.name, NutBody.sprite);
		}
		lastHp = base.Hp;
		if (base.Hp <= 0f && zombie != null)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gulp, base.transform.position);
		}
	}

	protected override void OnInitForPlace()
	{
		lastHp = base.Hp;
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
	}

	protected override void OnInitForAll()
	{
		NutBody.sprite = State1;
		SetSprite(NutBody.name, NutBody.sprite);
	}

	protected override void HurtAudio()
	{
		MyTool.RandomOne(new List<UnityAction>
		{
			() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlantClear1, base.transform.position);
			},
			() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlantClear2, base.transform.position);
			}
		});
	}

	protected override float HandleHurt(float hurt, bool isFlat)
	{
		float num = 1f;
		if (isSleeping)
		{
			num++;
		}
		if (isIcetrap)
		{
			num++;
		}
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			num++;
		}
		return hurt * num;
	}
}
