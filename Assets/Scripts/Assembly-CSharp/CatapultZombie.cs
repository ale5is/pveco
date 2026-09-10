using System.Collections.Generic;
using UnityEngine;

public class CatapultZombie : ZombieBase
{
	public CatapultZombieType Type;

	public SpriteRenderer ball1;

	public SpriteRenderer ball2;

	public SpriteRenderer ball3;

	public SpriteRenderer ball4;

	public SpriteRenderer sidingRenderer;

	public SpriteRenderer poleRenderer;

	public SpriteRenderer tape;

	public Sprite BasketBall;

	public Sprite Siding1;

	public Sprite Siding2;

	public Sprite pole1;

	public Sprite pole1noBall;

	public Sprite pole2;

	public Sprite pole2noBall;

	public Sprite Stone;

	public Sprite Siding1s;

	public Sprite Siding2s;

	public Sprite pole1s;

	public Sprite pole1noBalls;

	public Sprite pole2s;

	public Sprite pole2noBalls;

	public List<Sprite> NormalSkin = new List<Sprite>();

	public List<Sprite> RockSkin = new List<Sprite>();

	public List<SpriteRenderer> SkinPart = new List<SpriteRenderer>();

	private Grid BasketGrid;

	private int BasketNum = 20;

	private bool isHalfHp;

	protected override GameObject Prefab => GameManager.Instance.GameConf.CatapultZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => 5f;

	protected override float attackValue => 800f;

	public override int MaxHP => GetTypeHp();

	public override bool CanUseLadder => false;

	public override Vector3 SplashScale => new Vector3(3f, 2f);

	public override Vector3 VaseScale => new Vector3(0.35f, 0.35f);

	public override Vector3 VaseOffset => new Vector3(0f, -0.06f);

	private int GetTypeHp()
	{
		int result = 850;
		switch (Type)
		{
		case CatapultZombieType.Normal:
			result = 850;
			break;
		case CatapultZombieType.Rockpult:
			result = 1250;
			break;
		}
		return result;
	}

	public override void InitZombieHpState()
	{
		isHalfHp = false;
		Sprite sprite = BasketBall;
		List<Sprite> list = NormalSkin;
		switch (Type)
		{
		case CatapultZombieType.Normal:
			BasketNum = 20;
			tape.enabled = true;
			break;
		case CatapultZombieType.Rockpult:
			BasketNum = 10;
			list = RockSkin;
			sprite = Stone;
			tape.enabled = false;
			break;
		}
		ball1.enabled = true;
		ball2.enabled = true;
		ball3.enabled = true;
		ball4.enabled = true;
		ball1.sprite = sprite;
		ball2.sprite = sprite;
		ball3.sprite = sprite;
		ball4.sprite = sprite;
		for (int i = 0; i < SkinPart.Count; i++)
		{
			SkinPart[i].sprite = list[i];
		}
	}

	protected override void UpdateThis()
	{
		if (!IsOVer && (base.State == ZombieState.Walk || base.State == ZombieState.Attack))
		{
			if (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && Vector2.Distance(base.NextGrid.Position, base.transform.position) < 1.6f)
			{
				HurtPlant(base.NextGrid);
			}
			if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && Vector2.Distance(base.CurrGrid.Position, base.transform.position) < 0.9f)
			{
				HurtPlant(base.CurrGrid);
			}
		}
	}

	private void HurtPlant(Grid grid)
	{
		if (grid.CurrPlantBase.GetPlantType() == PlantType.Spike || grid.CurrPlantBase.GetPlantType() == PlantType.SpikeRock)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.balloon_pop, base.transform.position);
			if (grid.CurrPlantBase.GetPlantType() == PlantType.Spike)
			{
				grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this);
			}
			else
			{
				grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this, isFlat: true);
			}
			base.State = ZombieState.Dead;
		}
		else if (isHypno && grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this, isFlat: true);
		}
		else if (!isHypno && !grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this, isFlat: true);
		}
	}

	protected override void EatOutCheck()
	{
		if (BasketNum <= 0)
		{
			base.EatOutCheck();
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 300f * base.HpScale && !isHalfHp)
		{
			isHalfHp = true;
			if (Type == CatapultZombieType.Normal)
			{
				sidingRenderer.sprite = Siding2;
				if (BasketNum > 0)
				{
					poleRenderer.sprite = pole2;
				}
				else
				{
					poleRenderer.sprite = pole2noBall;
				}
			}
			else
			{
				sidingRenderer.sprite = Siding2s;
				if (BasketNum > 0)
				{
					poleRenderer.sprite = pole2s;
				}
				else
				{
					poleRenderer.sprite = pole2noBalls;
				}
			}
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

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombieCatapult).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector2(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Attack:
			if (BasketNum > 0)
			{
				animator.SetInteger("Change", 21);
			}
			break;
		case ZombieState.Dead:
			Shadow.enabled = false;
			base.collider2d.enabled = false;
			if (base.Hp <= 0)
			{
				SpecialAnimEvent4();
			}
			else
			{
				animator.SetInteger("Change", 31);
			}
			break;
		}
	}

	public override void SpecialAnimEvent1()
	{
		if (BasketGrid == null && !(hypnoAttackTarget != null))
		{
			return;
		}
		int num = 1;
		if (!base.IsFacingLeft)
		{
			num = -1;
		}
		float num2 = 75f;
		if (Type == CatapultZombieType.Rockpult)
		{
			num2 = 300f;
		}
		Basketball component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Basketball).GetComponent<Basketball>();
		BasketGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		if (BasketGrid != null)
		{
			component.Init(base.transform.position + new Vector3(1.4f * (float)num, 1.3f), BasketGrid, num2, isHypno, ball1.sprite);
		}
		else
		{
			component.Init(base.transform.position + new Vector3(1.4f * (float)num, 1.3f), hypnoAttackTarget, num2, ball1.sprite);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.basketball, base.transform.position);
		if (Type == CatapultZombieType.Normal)
		{
			if (isHalfHp)
			{
				poleRenderer.sprite = pole2noBall;
			}
			else
			{
				poleRenderer.sprite = pole1noBall;
			}
		}
		else if (isHalfHp)
		{
			poleRenderer.sprite = pole2noBalls;
		}
		else
		{
			poleRenderer.sprite = pole1noBalls;
		}
		BasketNum--;
	}

	public override void SpecialAnimEvent2()
	{
		anCanMove = true;
		if (BasketNum > 0 && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) > 0.9f)
		{
			return;
		}
		BasketGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		if (BasketGrid != null && BasketNum > 0)
		{
			base.State = ZombieState.Attack;
		}
		else if (BasketNum > 0)
		{
			hypnoAttackTarget = ZombieManager.Instance.GetLastZombieByLine(base.CurrLine, base.transform.position, !base.IsFacingLeft, !isHypno);
			if (hypnoAttackTarget != null)
			{
				base.State = ZombieState.Attack;
			}
		}
		if (base.State == ZombieState.Attack && hypnoAttackTarget != null && BasketNum <= 0)
		{
			Vector2 dirction = AttackDir;
			if (!base.IsFacingLeft)
			{
				dirction = new Vector2(0f - AttackDir.x, AttackDir.y);
			}
			hypnoAttackTarget.Hurt(25, dirction);
		}
	}

	public override void SpecialAnimEvent3()
	{
		if (BasketNum == 4)
		{
			ball1.enabled = false;
		}
		if (BasketNum == 3)
		{
			ball2.enabled = false;
		}
		if (BasketNum == 2)
		{
			ball3.enabled = false;
		}
		if (BasketNum == 1)
		{
			ball4.enabled = false;
		}
		if (BasketNum != 0)
		{
			if (Type == CatapultZombieType.Normal)
			{
				if (isHalfHp)
				{
					poleRenderer.sprite = pole2;
				}
				else
				{
					poleRenderer.sprite = pole1;
				}
			}
			else if (isHalfHp)
			{
				poleRenderer.sprite = pole2s;
			}
			else
			{
				poleRenderer.sprite = pole1s;
			}
		}
		BasketGrid = MapManager.Instance.GetLastPlantGrid(base.transform.position, base.CurrLine, base.IsFacingLeft, isHypno);
		if (BasketGrid == null || BasketNum == 0)
		{
			base.State = ZombieState.Walk;
			return;
		}
		hypnoAttackTarget = ZombieManager.Instance.GetLastZombieByLine(base.CurrLine, base.transform.position, !base.IsFacingLeft);
		if (hypnoAttackTarget == null)
		{
			base.State = ZombieState.Walk;
		}
	}

	public override void SpecialAnimEvent4()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.explosion, base.transform.position);
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CloudParticle).transform.position = base.transform.position;
		DirectDead(canDropItem: true, 0.1f);
	}
}
