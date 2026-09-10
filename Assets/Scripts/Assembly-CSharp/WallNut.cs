using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class WallNut : PlantBase
{
	public Sprite State1;

	public Sprite State2;

	public Sprite State3;

	public Sprite ExplodeState1;

	public Sprite ExplodeState2;

	public Sprite ExplodeState3;

	public Sprite Eye1;

	public Sprite Eye2;

	public Sprite ExplodeEye1;

	public Sprite ExplodeEye2;

	public SpriteRenderer NutBody;

	public Collider2D BoxCollider;

	private int state1;

	private int state2;

	private float lastHp;

	private float maxhp;

	private int updown;

	private bool FirstBowling;

	public int BowlingLine;

	private int LastBowlingLine;

	private int BowlingHitNum;

	private int Type;

	private Vector2 NutOffset;

	private int BowlingDeadNum;

	public override float MaxHp => maxhp;

	protected override Vector2 offSet => NutOffset;

	protected override int attackValue => 1800;

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (base.Hp < lastHp)
		{
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.NutParticle).transform.position = base.transform.position;
		}
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			if (Type == 1)
			{
				NutBody.sprite = ExplodeState2;
			}
			else
			{
				NutBody.sprite = State2;
			}
			SetSprite(NutBody.name, NutBody.sprite);
		}
		else if (base.Hp <= (float)state2)
		{
			if (Type == 1)
			{
				NutBody.sprite = ExplodeState3;
			}
			else
			{
				NutBody.sprite = State3;
			}
			SetSprite(NutBody.name, NutBody.sprite);
		}
		else
		{
			if (Type == 1)
			{
				NutBody.sprite = ExplodeState1;
			}
			else
			{
				NutBody.sprite = State1;
			}
			SetSprite(NutBody.name, NutBody.sprite);
		}
		lastHp = base.Hp;
	}

	protected override void OnInitForPlace()
	{
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
		lastHp = base.Hp;
	}

	public void SetType(PlantType type)
	{
		NutOffset = Vector2.zero;
		maxhp = 4000f;
		switch (type)
		{
		case PlantType.ExplodeNut:
			Type = 1;
			break;
		case PlantType.HugeNut:
			Type = 2;
			maxhp = 16000f;
			NutOffset = new Vector2(0f, 0.5f);
			break;
		default:
			Type = 0;
			break;
		}
		if (Type == 1)
		{
			eye1 = ExplodeEye1;
			eye2 = ExplodeEye2;
		}
		else
		{
			eye1 = Eye1;
			eye2 = Eye2;
		}
	}

	public override void PlaceOverEvent()
	{
		FirstBowling = true;
		if (!LV.Instance.LvSpStates.Contains(LVSpState.NutBowling))
		{
			return;
		}
		if (Type == 0)
		{
			FirstBowling = Random.Range(0, 2) == 0;
			BowlingLine = base.currGrid.Point.y;
			if (BowlingLine == 0)
			{
				FirstBowling = true;
			}
			else if (BowlingLine == base.CurrMap.MapGridNum.y - 1)
			{
				FirstBowling = false;
			}
		}
		else
		{
			FirstBowling = false;
		}
		BowlingLine = base.currGrid.Point.y;
		LastBowlingLine = -1;
		BoxCollider.enabled = true;
		PlayAnim("anim_roll", 0);
		Dead(isFlat: false, 10f, synClient: true);
		if (Type != 0)
		{
			FirstBowling = true;
		}
		Shadow.enabled = true;
		StartCoroutine(BowlingMove());
		if (Type != 2)
		{
			PlantManager.Instance.RollNuts.Add(this);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bowling, base.transform.position);
		NutBody.bounds = new Bounds(base.transform.position, Vector3.one * 10f);
	}

	private IEnumerator BowlingMove()
	{
		while (true)
		{
			yield return null;
			if (!(Time.timeScale > 0f))
			{
				continue;
			}
			Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position);
			BowlingLine = gridByWorldPos.Point.y;
			animatorSorting.sortingOrder = BowlingLine * 200 + 101;
			Shadow.sortingOrder = BowlingLine * 200 + FixedInfo.Shadow;
			if ((BowlingLine == 0 && base.transform.position.y > gridByWorldPos.Position.y) || (BowlingLine == base.CurrMap.MapGridNum.y - 1 && base.transform.position.y < gridByWorldPos.Position.y))
			{
				LastBowlingLine = BowlingLine;
				if (BowlingLine == 0)
				{
					updown = -Mathf.Abs(updown);
				}
				else
				{
					updown = Mathf.Abs(updown);
				}
			}
			if (base.transform.position.x > 7.9f)
			{
				PlantManager.Instance.RollNuts.Remove(this);
				Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
			}
			base.transform.Translate(new Vector3(3.5f, updown) * Time.deltaTime);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.tag == "Zombie"))
		{
			return;
		}
		ZombieBase component = collision.GetComponent<ZombieBase>();
		if (Type == 1)
		{
			PlantManager.Instance.RollNuts.Remove(this);
			Dead();
		}
		else if (Type == 2 && component.CurrLine == base.currGrid.Point.y)
		{
			component.Hurt(3600, Vector2.zero);
		}
		else
		{
			if (component.CurrLine != BowlingLine || LastBowlingLine == BowlingLine || ((!component.isHypno || !isHypno) && (component.isHypno || isHypno)))
			{
				return;
			}
			Vector2 dirction = new Vector2(1f, 0f);
			LastBowlingLine = BowlingLine;
			if (!GameManager.Instance.isClient)
			{
				if (updown != 0)
				{
					dirction = new Vector2(0f, updown);
					updown *= -1;
				}
				else
				{
					updown = 4;
					if (FirstBowling)
					{
						updown = -4;
					}
				}
			}
			CameraControl.Instance.ShakeCamera(base.transform.position);
			ServerSendSyn((updown > 0) ? 1 : 2);
			component.HammerHurt(200, dirction);
			if (component.GetDead())
			{
				BowlingDeadNum++;
			}
			if (BowlingDeadNum == 5)
			{
				AcvmentManager.Instance.GetAchievement(Acvname.RollNut5, PlacePlayer);
			}
			if (BowlingHitNum < 4)
			{
				for (int i = 0; i < BowlingHitNum; i++)
				{
					LvItemManager.Instance.DropCoin(base.transform.position, AlwaysDrop: true, notDiamond: true);
				}
			}
			if (BowlingHitNum < 4)
			{
				BowlingHitNum++;
			}
			MyTool.RandomOne(new List<UnityAction>
			{
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bowlingimpact1, base.transform.position);
				},
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bowlingimpact2, base.transform.position);
				}
			});
		}
	}

	protected override void OnInitForAll()
	{
		updown = 0;
		BowlingDeadNum = 0;
		BowlingHitNum = 0;
		if (Type == 1)
		{
			NutBody.sprite = ExplodeState1;
		}
		else if (Type == 2)
		{
			NutBody.sprite = State1;
			base.transform.localScale = new Vector3(2f, 2f);
		}
		else
		{
			NutBody.sprite = State1;
		}
		SetSprite(NutBody.name, NutBody.sprite);
		BoxCollider.enabled = false;
	}

	protected override void HurtAudio()
	{
		MyTool.RandomOne(new List<UnityAction>
		{
			() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlantClear1, base.transform.position);
			},
			() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.EatPlantClear2, base.transform.position);
			}
		});
	}

	protected override float HandleHurt(float hurt, bool isFlat)
	{
		float num = 1f;
		if (isSleeping)
		{
			num++;
		}
		if (isIcetrap)
		{
			num++;
		}
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			num++;
		}
		return hurt * num;
	}

	protected override void DeadrattleEvent()
	{
		if (Type == 1 && FirstBowling)
		{
			Boom();
		}
	}

	private void Boom()
	{
		base.CurrMap.FadeTempt += 5f;
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CherryParticle);
		obj.transform.position = base.transform.position;
		obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder(1);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CherryBoom, base.transform.position);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.3f, isHypno, needCapsule: false);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.3f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(attackValue / aroundPlant.Count, Vector2.zero, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(attackValue / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(attackValue);
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(attackValue, Vector2.zero, null);
			}
		}
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(base.currGrid, 1);
		for (int m = 0; m < aroundGrid.Count; m++)
		{
			aroundGrid[m].ClearLadder();
			if (aroundGrid[m].snow != null)
			{
				aroundGrid[m].snow.DirctClear(6, synClient: false);
			}
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 1)
		{
			updown = 4;
		}
		else if (syn.SynCode[1] == 2)
		{
			updown = -4;
		}
	}
}
