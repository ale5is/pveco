using UnityEngine;

public class HypnoShroom : PlantBase
{
	public SpriteRenderer NormalEye1;

	public SpriteRenderer NormalEye2;

	public SpriteRenderer SleepEye1;

	public SpriteRenderer SleepEye2;

	public override float MaxHp => 300f;

	protected override bool isShroom => true;

	protected override void OnInitForAll()
	{
		SleepEye1.enabled = false;
		SleepEye2.enabled = false;
		NormalEye1.enabled = true;
		NormalEye2.enabled = true;
	}

	protected override void GoSleepSpecial()
	{
		SleepEye1.enabled = true;
		SleepEye2.enabled = true;
		NormalEye1.enabled = false;
		NormalEye2.enabled = false;
		SetAnimChange(11);
	}

	protected override void GoAwakeSpecial()
	{
		SleepEye1.enabled = false;
		SleepEye2.enabled = false;
		NormalEye1.enabled = true;
		NormalEye2.enabled = true;
		SetAnimChange(12);
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (zombie != null && !isSleeping && !isFlat && !GameManager.Instance.isClient)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.mindControlled, base.transform.position);
			zombie.Hypno();
			Dead();
		}
	}
}
