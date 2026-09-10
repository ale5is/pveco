using SocketSave;
using UnityEngine;

public class MoonTombStone : PlantBase
{
	private float createSunTime = 24f;

	private float lightTime = 1.5f;

	public SpriteRenderer Body;

	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	private int state1;

	private int state2;

	private bool isFirst;

	public override float MaxHp => 500f;

	public override bool CanCarryed => false;

	public override bool IsZombiePlant => true;

	public override bool CanProtect => false;

	protected override void OnInitForAll()
	{
		Body.sprite = State1;
	}

	protected override void OnInitForPlace()
	{
		isFirst = true;
		needFlatDead = false;
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(createSunTime / 2f, isOver: false);
		}
	}

	protected override bool DoAction()
	{
		bool flag = CreateMoon();
		if (isFirst & flag)
		{
			isFirst = false;
			StartActionCD(createSunTime, isOver: false);
		}
		return true;
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			Body.sprite = State2;
		}
		else if (base.Hp <= (float)state2)
		{
			Body.sprite = State3;
		}
		else
		{
			Body.sprite = State1;
		}
	}

	private bool CreateMoon()
	{
		if (!NormalProduceCondition())
		{
			return false;
		}
		ServerSendSyn(0);
		StartCoroutine(BrightnessEffect2(lightTime, InstantiateMoon));
		return true;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StartCoroutine(BrightnessEffect2(lightTime, InstantiateMoon));
		}
	}

	private void InstantiateMoon()
	{
		SkyManager.Instance.CreatePlantSun(base.transform.position, 25f, SunType.Moon, PlacePlayer);
	}

	protected override void GoSleepSpecial()
	{
		ZZZ.gameObject.SetActive(value: false);
		GoAwake();
	}
}
