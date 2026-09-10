using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinriderZombie : ZombieBase
{
	private bool isJump;

	public SpriteMask DolpinMask;

	public List<SpriteRenderer> Dolphins = new List<SpriteRenderer>();

	public List<SpriteRenderer> legSprites = new List<SpriteRenderer>();

	private float AnTospeed;

	private float defSpeed;

	private float WaterDepth;

	protected override GameObject Prefab => GameManager.Instance.GameConf.DolphinriderZombie;

	protected override float AnToSpeed => AnTospeed;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 500;

	protected override int CriticalHp => 167;

	protected override float inWaterDepth => WaterDepth;

	public override void InitZombieHpState()
	{
		NormalSpeed();
		WaterDepth = 0.36f;
		isJump = false;
		DolpinMask.enabled = false;
		DolpinMask.frontSortingOrder = Sorting.sortingOrder;
		DolpinMask.backSortingOrder = Sorting.sortingOrder - 1;
		for (int i = 0; i < legSprites.Count; i++)
		{
			legSprites[i].material.SetInt("_OpenDisplay", 1);
		}
		for (int j = 0; j < Dolphins.Count; j++)
		{
			Dolphins[j].enabled = true;
		}
		WaterMask.transform.localPosition = new Vector3(-0.44f, -1f);
		if (LVManager.Instance.GameIsStart && !IsOVer)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.dolphin_appear, base.transform.position);
		}
	}

	private bool CheckHigh()
	{
		if ((base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.CarryPlant != null && base.CurrGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut) || (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.GetPlantType() == PlantType.Tallnut) || (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && base.NextGrid.CurrPlantBase.CarryPlant != null && base.NextGrid.CurrPlantBase.CarryPlant.GetPlantType() == PlantType.Tallnut))
		{
			SpecialAnimEvent2();
			SpecialAnimEvent5();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bonk, base.transform.position);
			return true;
		}
		return false;
	}

	protected override void UpdateThis()
	{
		if (!base.InWater && !isJump && base.State == ZombieState.Walk && (lastGrid == null || !lastGrid.isNoIceWater))
		{
			float num = Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x);
			if (base.CurrGrid != null && base.CurrGrid.isNoIceWater && num < 0.9f && num > 0.8f)
			{
				base.State = ZombieState.Attack;
				base.dontChangeState = true;
				SetAnimatorChange(41);
				DolpinMask.enabled = true;
				base.collider2d.enabled = false;
				WaterMask.transform.localPosition = new Vector3(-0.44f, -1.47f);
			}
		}
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (base.InWater)
			{
				SetAnimatorChange(12);
			}
			else
			{
				SetAnimatorChange(11);
			}
			break;
		case ZombieState.Attack:
			if (!isJump && base.InWater)
			{
				WaterDepth = 0.8f;
				isJump = true;
				base.dontChangeState = true;
				SetAnimatorChange(42);
				base.collider2d.enabled = false;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.dolphin_jump, base.transform.position);
			}
			else
			{
				SetAnimatorChange(21);
			}
			break;
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 180f * base.HpScale)
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

	private void NormalSpeed()
	{
		defSpeed = 2f;
		AnTospeed = 3f;
		base.Speed = 3f;
	}

	private void xxx()
	{
		AnTospeed = 0.8f;
		base.Speed = 0.8f;
		defSpeed = 0.8f;
		base.State = ZombieState.Walk;
		for (int i = 0; i < legSprites.Count; i++)
		{
			legSprites[i].material.SetInt("_OpenDisplay", 1);
		}
	}

	public override void SpecialAnimEvent1()
	{
		base.dontChangeState = false;
		xxx();
		DirctInWater();
		base.collider2d.enabled = true;
		if (base.IsFacingLeft)
		{
			base.transform.position -= new Vector3(1.04f, 0f);
		}
		else
		{
			base.transform.position += new Vector3(1.04f, 0f);
		}
	}

	public override void SpecialAnimEvent2()
	{
		NormalSpeed();
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
	}

	public override void SpecialAnimEvent3()
	{
		DolpinMask.enabled = false;
		for (int i = 0; i < Dolphins.Count; i++)
		{
			Dolphins[i].enabled = false;
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(base.transform.position + new Vector3(-1.1f, -0.04f), base.CurrGrid.Point.y);
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
	}

	public override void SpecialAnimEvent4()
	{
		if (CheckHigh())
		{
			StartCoroutine(MoveTo(new Vector2(base.transform.position.x, base.transform.position.y)));
			return;
		}
		Grid grid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
		if (Vector2.Distance(base.transform.position, grid.Position) > 4.2f)
		{
			grid = MapManager.Instance.GetGridByWorldPos(base.transform.position + new Vector3(-4.2f, 0f), base.CurrLine);
		}
		Vector2 position = grid.Position;
		position.x -= 0.3f;
		StartCoroutine(MoveTo(position));
	}

	public override void SpecialAnimEvent5()
	{
		base.collider2d.enabled = true;
		for (int i = 0; i < legSprites.Count; i++)
		{
			legSprites[i].material.SetInt("_OpenDisplay", 0);
		}
		WaterMask.transform.localPosition = new Vector3(-0.44f, -1f);
	}

	public override void SpecialAnimEvent6()
	{
		Shadow.enabled = false;
	}

	private IEnumerator MoveTo(Vector2 pos)
	{
		while (Vector2.Distance(base.transform.position, pos) > 0.01f)
		{
			yield return null;
			base.transform.position = Vector2.MoveTowards(base.transform.position, pos, 8f * Time.deltaTime);
		}
		while (BaseTransform.localPosition.y > 0f - inWaterDepth)
		{
			yield return null;
			BaseTransform.Translate(new Vector2(0f, -5f) * Time.deltaTime);
		}
		BaseTransform.localPosition = new Vector3(BaseTransform.localPosition.x, 0f - inWaterDepth);
	}

	protected override void InWaterChangeEvent()
	{
		if (base.State == ZombieState.Dead)
		{
			return;
		}
		if (base.InWater)
		{
			for (int i = 0; i < legSprites.Count; i++)
			{
				legSprites[i].material.SetInt("_OpenDisplay", 0);
			}
		}
		else
		{
			for (int j = 0; j < legSprites.Count; j++)
			{
				legSprites[j].material.SetInt("_OpenDisplay", 1);
			}
		}
		if (base.InWater)
		{
			if (isJump)
			{
				SetAnimatorChange(12);
				return;
			}
			xxx();
			SetAnimatorChange(43);
			WaterMask.transform.localPosition = new Vector3(-0.44f, -1.47f);
			for (int k = 0; k < Dolphins.Count; k++)
			{
				Dolphins[k].enabled = false;
			}
		}
		else if (isJump)
		{
			SetAnimatorChange(11);
		}
		else
		{
			NormalSpeed();
			SetAnimatorChange(13);
			for (int l = 0; l < Dolphins.Count; l++)
			{
				Dolphins[l].enabled = true;
			}
		}
	}
}
