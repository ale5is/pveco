using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Zamboni : ZombieBase
{
	public Sprite cover1;

	public Sprite cover2;

	public Sprite cover3;

	public Sprite wheel;

	public Sprite flatWheel;

	public SpriteRenderer wheelRenderer;

	public ParticleSystem SmokePs;

	private Iceroad iceroad;

	protected override GameObject Prefab => GameManager.Instance.GameConf.ZamboniZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => 6f;

	protected override float attackValue => 500f;

	public override int MaxHP => 1350;

	public override bool CanUseLadder => false;

	public override Vector3 SplashScale => new Vector3(3f, 2f);

	public override Vector3 VaseScale => new Vector3(0.32f, 0.32f);

	public override Vector3 VaseOffset => new Vector3(-0.1f, -0.14f);

	public override void InitZombieHpState()
	{
		iceroad = null;
		SmokePs.Stop();
		SmokePs.transform.GetComponent<SortingGroup>().sortingOrder = Sorting.sortingOrder + 1;
		canFrozen = false;
		canIce = false;
		canButter = false;
		wheelRenderer.sprite = wheel;
		HpState = new List<int> { 1350, 900, 450 };
		E1HpStateSprite = new List<Sprite> { cover1, cover2, cover3 };
		if (!LVManager.Instance.GameIsStart || IsOVer)
		{
			return;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.zamboni, base.transform.position);
		StartCoroutine(DoFuncWait(() =>
		{
			if (iceroad == null && !base.CurrGrid.isSlope && (!base.CurrGrid.isWaterGrid || base.CurrGrid.IsIce))
			{
				iceroad = Object.Instantiate(GameManager.Instance.GameConf.Iceroad).GetComponent<Iceroad>();
				iceroad.CreateInit(base.transform.position + new Vector3(0f, TargetBaseY + 0.1f), base.CurrLine, base.IsFacingLeft, base.BodyScale);
			}
		}, 0.2f));
	}

	protected override void UpdateThis()
	{
		if (IsOVer)
		{
			return;
		}
		if (base.State == ZombieState.Walk || base.State == ZombieState.Attack)
		{
			if (base.NextGrid != null && base.NextGrid.CurrPlantBase != null && Vector2.Distance(base.NextGrid.Position, base.transform.position) < 0.9f)
			{
				HurtPlant(base.NextGrid);
			}
			if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && Vector2.Distance(base.CurrGrid.Position, base.transform.position) < 0.7f)
			{
				HurtPlant(base.CurrGrid);
			}
		}
		if (base.State == ZombieState.Walk)
		{
			float num = -1.2f * base.BodyScale + 1.2f;
			if (base.IsFacingLeft)
			{
				num *= -1f;
			}
			if ((bool)iceroad)
			{
				iceroad.ChangeLong(base.transform.position.x + num);
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
				grid.CurrPlantBase.Hurt(attackValue, AttackDir, this);
			}
			else
			{
				grid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this, isFlat: true);
			}
			wheelRenderer.sprite = flatWheel;
			base.State = ZombieState.Dead;
		}
		else if (isHypno && grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, AttackDir, this, isFlat: true);
		}
		else if (!isHypno && !grid.CurrPlantBase.isHypno)
		{
			grid.CurrPlantBase.Hurt(attackValue, AttackDir, this, isFlat: true);
		}
	}

	protected override void CheckState()
	{
		base.CheckState();
		if (base.State == ZombieState.Dead)
		{
			base.collider2d.enabled = false;
			Shadow.enabled = false;
			SmokePs.Stop();
			if (wheelRenderer.sprite == flatWheel)
			{
				animator.SetInteger("Change", 30 + Random.Range(1, 3));
			}
			else
			{
				GoDead();
			}
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if (HitSound)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
			}
		}
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		if (base.Speed > 4f)
		{
			base.Speed -= 0.2f;
		}
		if (base.CurrGrid.isSlope && iceroad != null)
		{
			iceroad.startDisappear();
			iceroad = null;
		}
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if ((bool)iceroad)
		{
			iceroad.startDisappear();
		}
	}

	protected override void InWaterChangeEvent()
	{
		if (!IsOVer && base.InWater && (bool)iceroad)
		{
			iceroad.startDisappear();
			iceroad = null;
		}
	}

	private void GoDead()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CarParticle).transform.position = base.transform.position;
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CloudParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.explosion, base.transform.position);
		DirectDead(canDropItem: true, 0.1f);
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombieZomboni).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector2(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		if (nextSprite == cover3)
		{
			SmokePs.Play();
		}
	}

	public override void SpecialAnimEvent1()
	{
		GoDead();
	}

	public override void SpecialAnimEvent2()
	{
		if (base.State == ZombieState.Attack && hypnoAttackTarget != null)
		{
			Vector2 vector = AttackDir;
			if (!base.IsFacingLeft)
			{
				vector = new Vector2(0f - AttackDir.x, AttackDir.y);
			}
			hypnoAttackTarget.Hurt(50, vector);
			hypnoAttackTarget.Frozen(vector, isAudio: false);
		}
	}
}
