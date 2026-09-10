using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Pumpkin : PlantBase
{
	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	public Sprite State4;

	private int state1;

	private int state2;

	private int state3;

	private Animator backAnimator;

	public SpriteRenderer PumpkinBody;

	public SpriteRenderer BackRenderer;

	public override float MaxHp => 4000f;

	public override bool isProtectPlant => true;

	protected override Vector2 offSet => new Vector2(0f, -0.3f);

	public override bool CanProtect => false;

	protected override void OnInitForAll()
	{
		PumkinInit();
		PumpkinBody.sprite = State1;
		SetSprite(PumpkinBody.name, PumpkinBody.sprite);
	}

	protected override void OnInitForCreate()
	{
		backAnimator.speed = 0f;
		BackRenderer.color = PumpkinBody.color;
		backAnimator.GetComponent<SortingGroup>().sortingOrder = animatorSorting.sortingOrder - 1;
	}

	protected override void OnInitForPlace()
	{
		int num = animatorSorting.sortingOrder / 100 * 100;
		backAnimator.GetComponent<SortingGroup>().sortingOrder = num + FixedInfo.ProtectBack;
		animatorSorting.sortingOrder = num + FixedInfo.ProtectPlant;
		Icetrap.sortingOrder = animatorSorting.sortingOrder + 1;
	}

	protected override void ResetAnimSpeedEvent(float speed)
	{
		PumkinInit();
		backAnimator.speed = speed;
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			PumpkinBody.sprite = State2;
			SetSprite(PumpkinBody.name, PumpkinBody.sprite);
		}
		else if (base.Hp <= (float)state2 && base.Hp >= (float)state3)
		{
			PumpkinBody.sprite = State3;
			SetSprite(PumpkinBody.name, PumpkinBody.sprite);
		}
		else if (base.Hp <= (float)state3)
		{
			PumpkinBody.sprite = State4;
			SetSprite(PumpkinBody.name, PumpkinBody.sprite);
		}
		else
		{
			PumpkinBody.sprite = State1;
			SetSprite(PumpkinBody.name, PumpkinBody.sprite);
		}
	}

	private void PumkinInit()
	{
		if (backAnimator == null)
		{
			state1 = (int)MaxHp / 4 * 3;
			state2 = (int)MaxHp / 4 * 2;
			state3 = (int)MaxHp / 4;
			backAnimator = base.transform.Find("Back").GetComponent<Animator>();
		}
	}

	public override void OpenBlackAndWhite(bool isOpen)
	{
		PumkinInit();
		if (isOpen)
		{
			BackRenderer.material.SetInt("_OpenGray", 1);
		}
		else
		{
			BackRenderer.material.SetInt("_OpenGray", 0);
		}
		base.OpenBlackAndWhite(isOpen);
	}

	protected override void OwnerSetColor(Color color)
	{
		BackRenderer.color = color;
	}

	protected override void OwnerSetBrightness(float value)
	{
		BackRenderer.material.SetFloat("_Brightness", value);
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
