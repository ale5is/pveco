using System.Collections.Generic;
using UnityEngine;

public class PaperZombie : ZombieBase
{
	public Sprite paper1;

	public Sprite paper2;

	public Sprite paper3;

	public Sprite whiteEye;

	public Sprite redEye;

	public SpriteRenderer pupils;

	public SpriteRenderer hands;

	public SpriteRenderer hands2;

	public List<Texture2D> DecoratesTex = new List<Texture2D>();

	private float AnTospeed = 4f;

	private float defSpeed = 5f;

	protected override GameObject Prefab => GameManager.Instance.GameConf.PaperZombie;

	protected override float AnToSpeed => AnTospeed;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 150f;

	public override int MaxHP => 500;

	protected override int CriticalHp => 167;

	public override void InitZombieHpState()
	{
		AnTospeed = 4f;
		defSpeed = 5f;
		normalHead = whiteEye;
		HeadRenderer.sprite = whiteEye;
		pupils.enabled = true;
		hands.enabled = true;
		hands2.enabled = false;
		HammerDoorHpState = new List<int> { 0 };
		DoorHpState = new List<int> { 150, 100, 50, 0 };
		DoorHpStateSprite = new List<Sprite> { paper1, paper2, paper3, null };
	}

	protected override void SpriteChangeEvent(Sprite nextSprite)
	{
		Sprite sprite = DoorRenderer.sprite;
		if (base.DoorHp <= 0 && (sprite == paper1 || sprite == paper2 || sprite == paper3) && nextSprite == null)
		{
			DropEquip(DoorRenderer);
			hands.enabled = false;
			hands2.enabled = true;
			LowArmRenderer.enabled = true;
			if ((float)base.Hp < 320f * base.HpScale)
			{
				DropArm();
			}
		}
	}

	private void SetDecorate()
	{
		if (DecoratesTex.Count == 0)
		{
			return;
		}
		if (DecoratesTex.Count <= 2)
		{
			REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[0]);
			if (DecoratesTex.Count == 2)
			{
				REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[1]);
			}
			return;
		}
		int num = Random.Range(0, DecoratesTex.Count);
		int num2 = Random.Range(0, DecoratesTex.Count);
		while (num == num2)
		{
			num2 = Random.Range(0, DecoratesTex.Count);
		}
		REnderer.material.SetTexture("_Decorate1Tex", DecoratesTex[num]);
		REnderer.material.SetTexture("_Decorate2Tex", DecoratesTex[num2]);
	}

	protected override void DoorHpReduceEvent()
	{
		if (base.DoorHp <= 0)
		{
			anCanMove = false;
			SetAnimatorChange(41);
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rip, base.transform.position);
		}
		else if (Random.Range(0, 3) == 0)
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

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 320f * base.HpScale && base.DoorHp <= 0)
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

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (base.DoorHp > 0)
			{
				SetAnimatorChange(11);
			}
			else
			{
				SetAnimatorChange(12);
			}
			break;
		case ZombieState.Attack:
			if (base.DoorHp > 0)
			{
				SetAnimatorChange(21);
			}
			else
			{
				SetAnimatorChange(22);
			}
			break;
		}
	}

	protected override void PlaceCharred()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override void SpecialAnimEvent1()
	{
		base.Speed = 1.8f;
		defSpeed = 1.8f;
		AnTospeed = 3f;
		anCanMove = true;
		base.State = ZombieState.Walk;
		animator.SetInteger("Change", 12);
		normalHead = redEye;
		HeadRenderer.sprite = redEye;
		pupils.enabled = false;
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rarrgh1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.paper_rarrgh2, base.transform.position);
		}
	}
}
