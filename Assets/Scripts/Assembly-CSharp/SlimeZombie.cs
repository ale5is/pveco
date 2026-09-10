using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class SlimeZombie : ZombieBase
{
	public Sprite Sun;

	public Sprite SilverCoin;

	public Sprite GoldCoin;

	public Sprite Diamond;

	public SpriteRenderer ItemRenderer;

	private int SplitLvl;

	private float Scale;

	private bool isDeadIce;

	private int DropType;

	public override int MaxHP => 330;

	protected override GameObject Prefab => GameManager.Instance.GameConf.SlimeZombie;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => 5f;

	protected override float attackValue => 50f;

	public override void InitZombieHpState()
	{
		Scale = 1f;
		SplitLvl = 0;
		isDeadIce = false;
		ItemRenderer.enabled = true;
		ItemRenderer.color = new Color(1f, 1f, 1f, 0.3f);
		if (!LVManager.Instance.GameIsStart)
		{
			ItemRenderer.enabled = false;
		}
	}

	protected override void NeedSynInit()
	{
		int num = Random.Range(0, 100);
		if (num < 15)
		{
			DropType = 1;
		}
		else if (num < 30)
		{
			DropType = 2;
		}
		else if (num < 31)
		{
			DropType = 3;
		}
		else if (num < 45)
		{
			DropType = 4;
		}
		else if (num < 70)
		{
			DropType = 5;
		}
		else
		{
			DropType = 0;
		}
		SetItemSpite();
	}

	public override void ServerInitInfo()
	{
		ServerSendSyn(1, DropType);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2 && syn.SynCode[1] == 1)
		{
			DropType = syn.SynCode[2];
			SetItemSpite();
		}
	}

	private void SetItemSpite()
	{
		if (DropType == 1)
		{
			ItemRenderer.sprite = SilverCoin;
			ItemRenderer.transform.localScale = new Vector3(1.5f, 1.5f);
		}
		else if (DropType == 2)
		{
			ItemRenderer.sprite = GoldCoin;
			ItemRenderer.transform.localScale = new Vector3(1.5f, 1.5f);
		}
		else if (DropType == 3)
		{
			ItemRenderer.sprite = Diamond;
			ItemRenderer.transform.localScale = new Vector3(1.4f, 1.4f);
		}
		else if (DropType == 4)
		{
			ItemRenderer.sprite = Sun;
			ItemRenderer.transform.localScale = new Vector3(1f, 1f);
		}
		else if (DropType == 5)
		{
			ItemRenderer.sprite = Sun;
			ItemRenderer.transform.localScale = new Vector3(0.6f, 0.6f);
		}
		else if (DropType == 0)
		{
			ItemRenderer.enabled = false;
		}
	}

	protected override void PlaceCharred()
	{
		DirectDead(canDropItem: true, 0f);
	}

	protected override void BoomDeadEvent()
	{
		SplitChild(2);
	}

	public override void SpecialAnimEvent1()
	{
		SplitChild(1);
		if (DropType == 1)
		{
			LvItemManager.Instance.SummonSilverCoin(base.transform.position);
		}
		else if (DropType == 2)
		{
			LvItemManager.Instance.SummonGoldCoin(base.transform.position);
		}
		else if (DropType == 3)
		{
			LvItemManager.Instance.SummonDiamond(base.transform.position);
		}
		else if (DropType == 4)
		{
			SkyManager.Instance.CreatePlantSun(base.transform.position, 25f, SunType.Normal, null);
		}
		else if (DropType == 5)
		{
			SkyManager.Instance.CreatePlantSun(base.transform.position, 15f, SunType.Normal, null);
		}
	}

	public override void SpecialAnimEvent2()
	{
		float num = 0.8f;
		if (Scale == num)
		{
			num = 1f;
		}
		SetAnimatorChange(0);
		base.State = ZombieState.Walk;
		StartCoroutine(GrowUp(num));
	}

	private void SplitChild(int range)
	{
		if (Scale > 0.8f)
		{
			GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SlimeParticle);
			obj.transform.SetParent(base.transform.parent);
			obj.transform.position = base.transform.position;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.slimeBlast, base.transform.position);
		}
		else
		{
			GameObject obj2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SlimeSmallParticle);
			obj2.transform.SetParent(base.transform.parent);
			obj2.transform.position = base.transform.position;
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.slime1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.slime2, base.transform.position);
			}
		}
		if (!(Scale > 0.8f) || GameManager.Instance.isClient)
		{
			return;
		}
		List<Grid> list = new List<Grid>();
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(base.CurrGrid, range);
		for (int i = 0; i < aroundGrid.Count; i++)
		{
			if (!aroundGrid[i].isOccupied)
			{
				list.Add(aroundGrid[i]);
			}
		}
		list.Shuffle();
		int num = 3;
		num = ((SplitLvl >= 3) ? (num + (4 - SplitLvl)) : (num + SplitLvl));
		if (num <= 0)
		{
			num = 1;
		}
		if (isDeadIce || base.isIcetrap)
		{
			num--;
			if (num > 3)
			{
				num = 3;
			}
		}
		int num2 = ((!base.IsFacingLeft) ? 1 : (-1));
		for (int j = 0; j < num; j++)
		{
			if (list.Count > j)
			{
				Vector2 vector = new Vector2(Random.Range(0.1f, 0.5f) * (float)num2, 0f);
				ZombieManager.Instance.UpdateZombie(ZombieType.SlimeZombie, list[j].Position + vector).GetComponent<SlimeZombie>().SetChild(SplitLvl);
			}
		}
	}

	private void SetChild(int splitLvl)
	{
		base.Hp = 120;
		DropType = 0;
		Scale = 0.6f;
		base.BodyScale = Scale;
		SplitLvl = splitLvl + 1;
		ItemRenderer.enabled = false;
		StartCoroutine(DoFuncWait(() =>
		{
			StateUp1();
		}, 6f));
	}

	private IEnumerator GrowUp(float scale)
	{
		do
		{
			yield return null;
			if (Scale < scale)
			{
				Scale += Time.deltaTime;
				base.BodyScale = Scale;
				continue;
			}
			Scale = scale;
			yield break;
		}
		while (Scale != scale);
		if (Scale == 0.8f)
		{
			base.Hp = 210;
		}
		if (Scale == 1f)
		{
			base.Hp = MaxHP;
		}
	}

	private void StateUp1()
	{
		SetAnimatorChange(41);
		StartCoroutine(DoFuncWait(() =>
		{
			StateUp2();
		}, 10f));
	}

	private void StateUp2()
	{
		SetAnimatorChange(41);
	}

	protected override void HpReduceEvent(bool isHard, bool HitSound)
	{
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
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.slime1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.slime2, base.transform.position);
		}
	}

	protected override int HandleHurt(int attackValue, Vector2 dirction)
	{
		return attackValue - 5;
	}

	protected override void CreateInitZombie()
	{
		ItemRenderer.enabled = false;
	}

	protected override void DeadStateGetStaticBuff()
	{
		isDeadIce = base.isIcetrap;
	}
}
