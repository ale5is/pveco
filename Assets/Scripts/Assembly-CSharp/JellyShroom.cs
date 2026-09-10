using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class JellyShroom : PlantBase
{
	private bool isNormalState;

	private bool isBonunceState;

	private List<BulletBase> bulletBases = new List<BulletBase>();

	public override float MaxHp => 1000f;

	protected override bool isShroom => true;

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && !SpectatorList.Instance.LocalIsSpectator && (LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(PlacePlayer)) && base.currGrid != null && Input.GetMouseButtonDown(0) && !isSleeping && !isBonunceState && LVManager.Instance.GameIsStart)
		{
			if (GameManager.Instance.isClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 1;
				SocketClient.Instance.SendSynBag(synItem);
			}
			else
			{
				ChangeState();
			}
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			ChangeState();
		}
	}

	private void ChangeState()
	{
		isNormalState = !isNormalState;
		if (isNormalState)
		{
			SetAnimChange(12);
			GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 1.6f);
		}
		else
		{
			SetAnimChange(11);
			GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.6f);
		}
		ServerSendSyn(0);
	}

	public bool GetNormalState()
	{
		return isNormalState;
	}

	public void AddBullet(BulletBase bullet)
	{
		bulletBases.Add(bullet);
		if (!isBonunceState && isNormalState)
		{
			isBonunceState = true;
			SetAnimChange(13);
		}
	}

	protected override void OnInitForPlace()
	{
		isBonunceState = false;
		isNormalState = true;
	}

	public override void SpecialAnimEvent1()
	{
		if (bulletBases.Count <= 0)
		{
			return;
		}
		PlantBase plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		ZombieBase zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieBase != null && plantBase != null)
		{
			if (Mathf.Abs(plantBase.transform.position.x - base.transform.position.x) >= Mathf.Abs(zombieBase.transform.position.x - base.transform.position.x))
			{
				plantBase = null;
			}
			else
			{
				zombieBase = null;
			}
		}
		List<BulletBase> list = new List<BulletBase>();
		for (int i = 0; i < bulletBases.Count; i++)
		{
			if (Mathf.Abs(bulletBases[i].transform.position.x - base.transform.position.x) < 0.9f)
			{
				if (bulletBases[i].IsFacingLeft == base.IsFacingLeft)
				{
					bulletBases[i].PultInit(plantBase, zombieBase, bulletBases[i].transform.position);
				}
				else
				{
					list.Add(bulletBases[i]);
				}
			}
		}
		bulletBases.Clear();
		if (list.Count <= 0)
		{
			return;
		}
		plantBase = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, !base.IsFacingLeft, !isHypno);
		zombieBase = ZombieManager.Instance.GetZombieByLineMinDisCanNoCollGet(base.currGrid.Point.y, base.transform.position, !base.IsFacingLeft, isHypno);
		if (zombieBase != null && plantBase != null)
		{
			if (Mathf.Abs(plantBase.transform.position.x - base.transform.position.x) >= Mathf.Abs(zombieBase.transform.position.x - base.transform.position.x))
			{
				plantBase = null;
			}
			else
			{
				zombieBase = null;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].PultInit(plantBase, zombieBase, list[j].transform.position);
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (bulletBases.Count == 0)
		{
			SetAnimChange(14);
			isBonunceState = false;
		}
	}
}
