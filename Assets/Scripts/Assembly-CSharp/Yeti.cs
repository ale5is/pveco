using SocketSave;
using UnityEngine;

public class Yeti : ZombieBase
{
	private bool isBack;

	private float defSpeed = 4f;

	private Grid endGrid;

	private int WalkNum;

	protected override float DefSpeed => defSpeed;

	protected override float attackValue => 50f;

	public override int MaxHP => 4000;

	public override bool CanEatByChomper => false;

	protected override int CriticalHp => 167;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Yeti;

	protected override float AnToSpeed => 2f;

	public override void InitZombieHpState()
	{
		isBack = false;
		canIce = false;
		canFrozen = false;
		endGrid = MapManager.Instance.GetFarestGrid(base.transform.position, base.IsFacingLeft, base.CurrLine);
	}

	public override void ZombieOnDead(bool dropItem)
	{
		if (dropItem)
		{
			for (int i = 0; i < 3; i++)
			{
				Allcoin component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Diamond).GetComponent<Allcoin>();
				component.transform.SetParent(null);
				component.InitForItem(base.transform.position);
			}
		}
	}

	protected override void CurrGridChangeEvent(Grid lastGrid)
	{
		WalkNum++;
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

	private void GoBackEscape()
	{
		GoBack();
		isBack = true;
		base.AddSpeed = 1f;
		ServerSendSyn(0);
	}

	public override void SpecialAnimEvent1()
	{
		if (AttackGrid != null && AttackGrid.CurrPlantBase != null)
		{
			AttackGrid.CurrPlantBase.Ice();
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (!GameManager.Instance.isClient)
		{
			if (!isBack && Random.Range(WalkNum, 18) > 16 && WalkNum > 1)
			{
				GoBackEscape();
			}
			if (!isBack && Vector2.Distance(base.transform.position, endGrid.Position) < 2f)
			{
				GoBackEscape();
			}
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			GoBackEscape();
		}
	}
}
