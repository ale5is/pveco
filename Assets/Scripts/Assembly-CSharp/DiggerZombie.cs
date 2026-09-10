using System.Collections;
using System.Collections.Generic;
using FTRuntime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiggerZombie : ZombieBase
{
	private float ownerAnToSpeed;

	private float ownerSpeed;

	public Sprite hat1;

	public Sprite hat2;

	public Sprite hat3;

	public SpriteRenderer pickaxe;

	public Renderer dirtRenderer;

	public Animator QuestionMark;

	private bool isDigging;

	public Light2D HatLight;

	public SpriteMask mask;

	private Grid OutGrid;

	protected override GameObject Prefab => GameManager.Instance.GameConf.DiggerZombie;

	protected override float AnToSpeed => ownerAnToSpeed;

	protected override float DefSpeed => ownerSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 470;

	protected override int CriticalHp => 70;

	public override void InitZombieHpState()
	{
		canIce = false;
		ownerAnToSpeed = 1.2f;
		ownerSpeed = 1.2f;
		mask.enabled = false;
		mask.frontSortingOrder = Sorting.sortingOrder;
		mask.backSortingOrder = Sorting.sortingOrder - 1;
		dirtRenderer.enabled = false;
		if (base.CurrGrid != null)
		{
			dirtRenderer.GetComponent<SwfClip>().sortingOrder = base.CurrLine * 200 + 195;
			OutGrid = MapManager.Instance.GetFarestGrid(base.transform.position, getLeft: true, base.CurrLine);
		}
		if (base.CurrGrid != null && base.CurrGrid.isWaterGrid)
		{
			SpecialAnimEvent3();
			animator.Play("walk1");
		}
		else
		{
			base.dontChangeState = true;
			base.collider2d.enabled = false;
			animator.Play("dig");
			isDigging = true;
			if (LVManager.Instance.GameIsStart && !IsOVer)
			{
				Shadow.enabled = false;
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.digger, base.transform.position);
			}
		}
		animator.transform.localPosition = new Vector3(1.39f, 1.55f);
		pickaxe.enabled = true;
		HatLight.enabled = true;
		HpState = new List<int> { 470, 400, 340, 270 };
		E1HpStateSprite = new List<Sprite> { hat1, hat2, hat3, null };
		QuestionMark.gameObject.SetActive(value: false);
	}

	protected override void UpdateThis()
	{
		if (isDigging && !IsOVer)
		{
			if (!isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				anCanMove = false;
			}
			else if (isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				anCanMove = false;
			}
			else if (!anCanMove)
			{
				anCanMove = true;
			}
			if ((base.IsFacingLeft && base.transform.position.x < OutGrid.Position.x - 0.1f) || (!base.IsFacingLeft && base.transform.position.x > OutGrid.Position.x + 0.1f))
			{
				OutGround(goBack: true);
			}
			if (base.NextGrid != null && base.NextGrid.isWaterGrid && Mathf.Abs(base.NextGrid.Position.x - base.transform.position.x) < 1f)
			{
				StartCoroutine(NopickaxeOutDirt());
			}
		}
	}

	private void OutGround(bool goBack)
	{
		isDigging = false;
		anCanMove = false;
		animator.Play("drill");
		animator.transform.localPosition += new Vector3(0f, -2.4f);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
		StartCoroutine(MoveOut(goBack));
	}

	private IEnumerator MoveOut(bool goBack)
	{
		mask.enabled = true;
		dirtRenderer.enabled = true;
		dirtRenderer.GetComponent<SwfClipController>().GotoAndPlay(0);
		while (animator.transform.localPosition.y < 1.55f)
		{
			yield return null;
			animator.transform.Translate(new Vector2(0f, 3f) * Time.deltaTime);
		}
		animator.transform.localPosition = new Vector3(animator.transform.localPosition.x, 1.55f);
		mask.enabled = false;
		dirtRenderer.enabled = false;
		animator.Play("landing");
		if (goBack)
		{
			GoBack();
		}
		base.collider2d.enabled = true;
		Shadow.enabled = true;
	}

	private IEnumerator NopickaxeOutDirt()
	{
		isDigging = false;
		anCanMove = false;
		animator.speed = 0f;
		QuestionMark.gameObject.SetActive(value: true);
		QuestionMark.Play("", 0, 0f);
		do
		{
			yield return null;
		}
		while (!(QuestionMark.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f));
		QuestionMark.gameObject.SetActive(value: false);
		ResetAnimationSpeed();
		OutGround(goBack: false);
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

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = EquipRenderer.sprite;
		if ((sprite == hat1 || sprite == hat2 || sprite == hat3) && nextSprite == null)
		{
			DropEquip(EquipRenderer);
		}
		if (nextSprite == hat3 || nextSprite == null)
		{
			HatLight.enabled = false;
		}
	}

	protected override void PlaceCharred()
	{
		if (!isDigging)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombieDigger).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale, (!pickaxe.enabled) ? 1 : 0);
		}
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 0 && pickaxe.enabled)
		{
			result = pickaxe;
			if (needClearEquip)
			{
				pickaxe.enabled = false;
				if (isDigging)
				{
					StartCoroutine(NopickaxeOutDirt());
				}
			}
		}
		return result;
	}

	public override void SpecialAnimEvent1()
	{
		if (pickaxe.enabled)
		{
			Attack();
		}
	}

	public override void SpecialAnimEvent2()
	{
		canIce = true;
		if (pickaxe.enabled)
		{
			SetAnimatorChange(42);
		}
		else
		{
			SpecialAnimEvent3();
		}
	}

	public override void SpecialAnimEvent3()
	{
		anCanMove = true;
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
		SetAnimatorChange(41);
		ownerAnToSpeed = 3.5f;
		ownerSpeed = 4f;
		base.Speed = DefSpeed;
	}

	public override void SpecialAnimEvent4()
	{
		if (!anCanMove)
		{
			if (!isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && !base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this);
			}
			else if (isHypno && base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && base.CurrGrid.CurrPlantBase.isHypno && base.CurrGrid.CurrPlantBase.GetPlantType() == PlantType.PotatoMine)
			{
				base.CurrGrid.CurrPlantBase.Hurt(attackValue, Vector2.zero, this);
			}
		}
	}

	protected override void ChangeFacingEvent()
	{
		OutGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
	}
}
