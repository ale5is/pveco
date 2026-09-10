using UnityEngine;

public class TallnutZombie : PlantZombie
{
	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	public SpriteRenderer NutBody;

	private int state1;

	private int state2;

	protected override GameObject Prefab => GameManager.Instance.GameConf.TallnutZombie;

	protected override Vector2 SpeedRange => new Vector2(2f, 2.8f);

	protected override int OwnerHp => 2470;

	protected override void PlantZombieInit()
	{
		NutBody.sprite = State1;
		state1 = MaxHP / 3 * 2;
		state2 = MaxHP / 3;
	}

	protected override void HpReduceEventPZombie()
	{
		if (base.Hp <= state1 && base.Hp >= state2)
		{
			NutBody.sprite = State2;
		}
		else if (base.Hp <= state2)
		{
			NutBody.sprite = State3;
		}
		else
		{
			NutBody.sprite = State1;
		}
	}
}
