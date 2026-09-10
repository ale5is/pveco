using System.Collections;
using UnityEngine;

public class Polevaulter : ZombieBase
{
	private bool isJump;

	private float aniSpeed;

	protected override float DefSpeed => 2.5f;

	protected override float attackValue => 50f;

	public override int MaxHP => 500;

	protected override int CriticalHp => 167;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Polevaulter;

	protected override float AnToSpeed => aniSpeed;

	protected override void UpdateThis()
	{
		if (isJump || base.State != ZombieState.Walk || base.NextGrid == null)
		{
			return;
		}
		if (base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.ZombieCanEat && ((!base.NextGrid.HaveRightLadder && base.IsFacingLeft) || (!base.NextGrid.HaveLeftLadder && !base.IsFacingLeft)) && ((isHypno && base.NextGrid.CurrPlantBase.isHypno) || (!isHypno && !base.NextGrid.CurrPlantBase.isHypno)) && base.transform.position.x - base.NextGrid.CurrPlantBase.transform.position.x < 1.4f * base.BodyScale)
		{
			aniSpeed = 5f;
			base.State = ZombieState.Attack;
		}
		for (int i = 0; i < PlantManager.Instance.RollNuts.Count; i++)
		{
			if ((PlantManager.Instance.RollNuts[i].BowlingLine == base.CurrLine && Mathf.Abs(base.transform.position.x - PlantManager.Instance.RollNuts[i].transform.position.x) < 3.5f && base.IsFacingLeft && base.transform.position.x > PlantManager.Instance.RollNuts[i].transform.position.x) || (!base.IsFacingLeft && base.transform.position.x < PlantManager.Instance.RollNuts[i].transform.position.x))
			{
				aniSpeed = 5f;
				base.State = ZombieState.Attack;
				base.collider2d.enabled = false;
			}
		}
	}

	public override void InitZombieHpState()
	{
		aniSpeed = 2.5f;
		canButter = true;
		canIce = true;
		isJump = false;
		animator.Play("run");
		Shadow.transform.localPosition = new Vector3(0f, Shadow.transform.localPosition.y);
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	private void CheckHigh()
	{
		if ((base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.CarryPlant != null && base.CurrGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut) || (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.CarryPlant != null && base.NextGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut))
		{
			canButter = true;
			canIce = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bonk, base.transform.position);
			base.collider2d.enabled = true;
			base.State = ZombieState.Walk;
			animator.Play("walk1", 0, 0f);
			if (!base.InWater)
			{
				Shadow.enabled = true;
			}
			isJump = true;
			base.Speed = 5f;
		}
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			animator.SetInteger("Change", 11);
			break;
		case ZombieState.Attack:
			if (isJump)
			{
				animator.SetInteger("Change", 21);
				break;
			}
			canButter = false;
			canIce = false;
			animator.SetInteger("Change", 41);
			break;
		case ZombieState.Idel:
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 320f * base.HpScale)
		{
			DropArm();
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

	protected override void EatOutCheck()
	{
		if (isJump)
		{
			base.EatOutCheck();
		}
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.grassstep, base.transform.position);
		base.collider2d.enabled = false;
		Shadow.enabled = false;
		CheckHigh();
	}

	public override void AnimFailSound()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Zombiefail2, base.transform.position);
		}
	}

	public override void SpecialAnimEvent2()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Polevault, base.transform.position);
	}

	public override void SpecialAnimEvent3()
	{
		if (!base.InWater)
		{
			Shadow.enabled = true;
		}
		float num = 2.4f * base.BodyScale;
		if (base.IsFacingLeft)
		{
			Shadow.transform.position += new Vector3(0f - num, 0f, 0f);
		}
		else
		{
			Shadow.transform.position += new Vector3(num, 0f, 0f);
		}
	}

	public override void SpecialAnimEvent4()
	{
		animator.Play("walk1");
	}

	public override void SpecialAnimEvent5()
	{
		if (!isJump)
		{
			isJump = true;
			base.State = ZombieState.Walk;
			base.Speed = 5f;
			canButter = true;
			canIce = true;
			base.collider2d.enabled = true;
			float num = 2.4f * base.BodyScale;
			if (base.IsFacingLeft)
			{
				Shadow.transform.position += new Vector3(num, 0f, 0f);
				base.transform.position += new Vector3(0f - num, 0f, 0f);
			}
			else
			{
				Shadow.transform.position += new Vector3(0f - num, 0f, 0f);
				base.transform.position += new Vector3(num, 0f, 0f);
			}
		}
	}

	public override void SpecialAnimEvent6()
	{
		float y = MapManager.Instance.GetGridByWorldPos(base.transform.position + new Vector3(-2.4f, 0f, 0f), base.CurrLine).Position.y;
		StartCoroutine(MoveDown(y));
	}

	private IEnumerator MoveDown(float y)
	{
		if (base.transform.position.y > y)
		{
			while (base.transform.position.y > y)
			{
				yield return null;
				base.transform.Translate(new Vector2(0f, -3f) * Time.deltaTime);
			}
		}
		else
		{
			while (base.transform.position.y < y)
			{
				yield return null;
				base.transform.Translate(new Vector2(0f, 3f) * Time.deltaTime);
			}
		}
	}
}
