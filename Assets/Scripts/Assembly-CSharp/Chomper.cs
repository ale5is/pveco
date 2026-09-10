using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Chomper : PlantBase
{
	private bool isChewing;

	private bool eatPlant;

	private float attackDis = 2.6f;

	public Sprite ArmSprite;

	public Sprite LeafSptite;

	public SpriteRenderer Hand;

	public SpriteRenderer DownArm;

	public override float MaxHp => 300f;

	protected override int attackValue => 40;

	protected override List<string> DontPlayAnim => new List<string> { "anim_idle", "anim_chew" };

	protected override void OnInitForPlace()
	{
		isChewing = false;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(1.4f, isOver: true);
		}
	}

	protected override bool DoAction()
	{
		if (GameManager.Instance.isClient)
		{
			return false;
		}
		if (isChewing)
		{
			ChewEnd();
			return true;
		}
		return CheckAttack();
	}

	private bool CheckAttack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		if (GameManager.Instance.isClient)
		{
			return false;
		}
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
		bool flag = false;
		if (zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < attackDis)
		{
			flag = true;
		}
		else if (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < attackDis)
		{
			flag = true;
		}
		if (flag)
		{
			SetAnimChange(11);
			ServerSendSyn(1);
			return true;
		}
		return false;
	}

	public override void SpecialAnimEvent1()
	{
		Attack();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.BigChomp, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		if (!isChewing)
		{
			SetAnimChange(12);
		}
	}

	private bool Attack()
	{
		if (GameManager.Instance.isClient)
		{
			return false;
		}
		isChewing = false;
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.currGrid.Point.y, base.transform.position, base.IsFacingLeft, isHypno);
		if (zombieByLineMinDistance != null && Mathf.Abs(zombieByLineMinDistance.transform.position.x - base.transform.position.x) < attackDis)
		{
			if (zombieByLineMinDistance.CanEatByChomper)
			{
				int num = zombieByLineMinDistance.Hp / 80 + 2;
				StartActionCD(num * 3, isOver: false);
				zombieByLineMinDistance.DirectDead(canDropItem: true, 0f);
				eatPlant = false;
				isChewing = true;
			}
			else
			{
				zombieByLineMinDistance.Hurt(GetAttackValue(), Vector2.zero);
			}
			DownArm.sprite = ArmSprite;
			Hand.enabled = true;
		}
		if (zombieByLineMinDistance == null)
		{
			PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, base.IsFacingLeft, !isHypno);
			if (minDisPlant != null && Mathf.Abs(minDisPlant.transform.position.x - base.transform.position.x) < attackDis)
			{
				isChewing = true;
				eatPlant = true;
				int num2 = (int)minDisPlant.Hp / 100 + 1;
				StartActionCD(num2 * 3, isOver: false);
				minDisPlant.Hurt(1800f, Vector2.zero, null);
				DownArm.sprite = LeafSptite;
				Hand.enabled = false;
			}
		}
		ServerSendSyn(2, eatPlant ? 1 : 0);
		return isChewing;
	}

	private void ChewEnd()
	{
		isChewing = false;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(1.4f, isOver: true);
		}
		ServerSendSyn(0);
		SetAnimChange(12);
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			ChewEnd();
		}
		else if (syn.SynCode[1] == 1)
		{
			SetAnimChange(11);
		}
		else if (syn.SynCode[1] == 2)
		{
			isChewing = true;
			if (syn.SynCode[2] == 1)
			{
				DownArm.sprite = LeafSptite;
				Hand.enabled = false;
			}
			else
			{
				DownArm.sprite = ArmSprite;
				Hand.enabled = true;
			}
		}
	}

	protected override void GoAwakeSpecial()
	{
	}

	protected override void GoSleepSpecial()
	{
	}
}
