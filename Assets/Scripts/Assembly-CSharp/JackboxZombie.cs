using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class JackboxZombie : ZombieBase
{
	private int BoomNum;

	public SpriteRenderer jackBox;

	public SpriteRenderer jackBoxHandle;

	private EFAudio fAudio;

	private Grid endGrid;

	public Sprite SpNormalArm;

	public Sprite SpLostArm;

	protected override GameObject Prefab => GameManager.Instance.GameConf.JackboxZombie;

	protected override float AnToSpeed => 4f;

	protected override float DefSpeed => 1.8f;

	protected override float attackValue => 80f;

	public override int MaxHP => 500;

	protected override int CriticalHp => 167;

	public override Vector3 VaseOffset => new Vector3(0.06f, -0.3f);

	public override void InitZombieHpState()
	{
		BoomNum = 0;
		MidArmRenderer.sprite = SpNormalArm;
		jackBox.material.SetInt("_OpenDisplay", 1);
		jackBoxHandle.material.SetInt("_OpenDisplay", 1);
		endGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
		if (LVManager.Instance.GameIsStart && !IsOVer && LV.Instance.CurrLVType != LVType.VaseBreaker)
		{
			fAudio = AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.jackinthebox, base.transform.position);
		}
	}

	private void PopClip()
	{
		if (!base.IsCriticalState && !GameManager.Instance.isClient)
		{
			ServerSendSyn(0);
			anCanMove = false;
			SetAnimatorChange(41);
		}
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
		if ((float)base.Hp < 320f * base.HpScale && MidArmRenderer.sprite != SpLostArm)
		{
			DropArm();
			MidArmRenderer.material.SetInt("_OpenDisplay", 1);
			MidArmRenderer.sprite = SpLostArm;
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
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CharredZombie).GetComponent<CharredZombie>().CreateInit(base.transform.position, new Vector3(0f, TargetBaseY), Sorting.sortingOrder, base.IsFacingLeft, BaseTransform.localScale);
		DirectDead(canDropItem: true, 0f, synClient: true);
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (fAudio != null && fAudio.PlayClipName == GameManager.Instance.AudioConf.jackinthebox.name)
		{
			fAudio.Close();
		}
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		BoomNum++;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			anCanMove = false;
			SetAnimatorChange(41);
		}
	}

	public override SpriteRenderer GetEquipSprite(bool needClearEquip)
	{
		SpriteRenderer result = null;
		if (base.Hp > 0 && jackBox.enabled)
		{
			result = jackBox;
			if (needClearEquip)
			{
				jackBox.material.SetInt("_OpenDisplay", 0);
				jackBoxHandle.material.SetInt("_OpenDisplay", 0);
				base.Hp = 270;
			}
		}
		return result;
	}

	public override void SpecialAnimEvent1()
	{
		if (jackBox.enabled && Random.Range(0, 8) == 1)
		{
			PopClip();
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (LV.Instance.CurrLVType == LVType.VaseBreaker)
		{
			PopClip();
		}
		if (PlacePlayer == null)
		{
			if (jackBox.enabled && BoomNum > 0 && Random.Range(BoomNum, 18) > 16)
			{
				PopClip();
			}
			if (endGrid != null && jackBox.enabled && Vector2.Distance(base.transform.position, endGrid.Position) < 2f)
			{
				PopClip();
			}
		}
	}

	public override void SpecialAnimEvent3()
	{
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.jack_surprise1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.jack_surprise2, base.transform.position);
		}
	}

	public override void SpecialAnimEvent4()
	{
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(base.CurrGrid, 1);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.25f, isHypno);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, !isHypno, needCapsule: false);
		if (PlacePlayer == null)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(1800f, Vector2.left, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(1800);
			}
		}
		else
		{
			for (int k = 0; k < aroundPlant.Count; k++)
			{
				if (aroundPlant[k].IsZombiePlant)
				{
					aroundPlant[k].Hurt(1800 / aroundPlant.Count, Vector2.left, null);
				}
				else
				{
					aroundPlant[k].Hurt(1800f, Vector2.left, null);
				}
			}
			for (int l = 0; l < zombies.Count; l++)
			{
				zombies[l].BoomHurt(1800 / zombies.Count);
			}
		}
		for (int m = 0; m < aroundGrid.Count; m++)
		{
			for (int n = 0; n < aroundGrid[m].Vases.Count; n++)
			{
				aroundGrid[m].Vases[n].BreakEvent();
			}
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CarParticle).transform.position = base.transform.position;
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.JackboxCloudParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.explosion, base.transform.position);
		DirectDead(canDropItem: false, 0.1f);
	}

	protected override void ClickEvent(string clickPlayer)
	{
		if (clickPlayer == PlacePlayer)
		{
			PopClip();
		}
	}
}
