using System.Collections.Generic;
using UnityEngine;

public class Garlic : PlantBase
{
	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	public SpriteRenderer GarlicBody;

	private int state1;

	private int state2;

	public List<SpriteRenderer> StemRederer = new List<SpriteRenderer>();

	public override float MaxHp => 400f;

	protected override void OnInitForAll()
	{
		GarlicBody.sprite = State1;
		SetSprite(GarlicBody.name, GarlicBody.sprite);
		for (int i = 0; i < StemRederer.Count; i++)
		{
			StemRederer[i].enabled = true;
			SetSpriteEnable(StemRederer[i].name, StemRederer[i].enabled);
		}
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (zombie != null && !isFlat)
		{
			zombie.Yuck();
		}
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			if (GarlicBody.sprite != State2)
			{
				GarlicBody.sprite = State2;
				SetSprite(GarlicBody.name, GarlicBody.sprite);
				for (int i = 0; i < StemRederer.Count; i++)
				{
					StemRederer[i].enabled = false;
					SetSpriteEnable(StemRederer[i].name, StemRederer[i].enabled);
				}
			}
		}
		else if (base.Hp <= (float)state2)
		{
			if (GarlicBody.sprite != State3)
			{
				GarlicBody.sprite = State3;
				SetSprite(GarlicBody.name, GarlicBody.sprite);
				for (int j = 0; j < StemRederer.Count; j++)
				{
					StemRederer[j].enabled = false;
					SetSpriteEnable(StemRederer[j].name, StemRederer[j].enabled);
				}
			}
		}
		else if (GarlicBody.sprite != State1)
		{
			GarlicBody.sprite = State1;
			SetSprite(GarlicBody.name, GarlicBody.sprite);
			for (int k = 0; k < StemRederer.Count; k++)
			{
				StemRederer[k].enabled = true;
				SetSpriteEnable(StemRederer[k].name, StemRederer[k].enabled);
			}
		}
	}

	protected override void OnInitForPlace()
	{
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
	}
}
