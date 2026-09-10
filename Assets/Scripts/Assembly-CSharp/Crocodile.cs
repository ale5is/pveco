using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;

public class Crocodile : ZombieBase
{
	public Sprite NormalBody;

	public Sprite WaterBody;

	public SpriteRenderer BodyRenderer;

	private float InWaterX;

	private float RandomSpeed = 4f;

	private bool needMove;

	private Vector3 groundPos;

	public override int MaxHP => 1000;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Crocodile;

	protected override float AnToSpeed => 5f;

	protected override float DefSpeed => RandomSpeed;

	protected override float attackValue => 2000f;

	protected override float InWaterDistance => -0.3f;

	protected override float OutWaterDistance => -0.2f;

	public override Vector3 VaseScale => new Vector3(0.32f, 0.32f);

	public override Vector3 VaseOffset => new Vector3(-0.1f, 0.13f);

	public override void InitZombieHpState()
	{
		needMove = false;
		needInWater = true;
		base.collider2d.enabled = true;
	}

	protected override void UpdateThis()
	{
		if (base.InWater && !needInWater && !base.dontChangeState)
		{
			if (LV.Instance.CurrLVType == LVType.Normal && !isHypno)
			{
				if (base.NextGrid == null || !base.NextGrid.isWaterGrid || base.NextGrid.IsIce)
				{
					if (base.IsFacingLeft && base.CurrGrid.Position.x - base.transform.position.x > -0.4f)
					{
						if (base.CurrGrid.Point.x >= 7)
						{
							needInWater = true;
						}
						else
						{
							GoBack();
							base.transform.position += new Vector3(-1.4f, 0f);
						}
					}
					else if (!base.IsFacingLeft && base.transform.position.x - base.CurrGrid.Position.x > -0.4f)
					{
						GoBack();
						base.transform.position += new Vector3(1.4f, 0f);
					}
				}
			}
			else
			{
				needInWater = true;
			}
		}
		if (needMove)
		{
			anCanMove = false;
			Vector3 normalized = (groundPos - base.transform.position).normalized;
			base.transform.Translate(normalized * Time.deltaTime * 6f);
			if (base.transform.position.y < groundPos.y)
			{
				needMove = false;
				base.collider2d.enabled = true;
				base.dontChangeState = false;
				base.State = ZombieState.Walk;
				canIce = true;
				Shadow.enabled = true;
			}
		}
	}

	protected override void NeedSynInit()
	{
		RandomSpeed = Random.Range(2.6f, 4.5f);
	}

	public override void ServerInitInfo()
	{
		ServerSendSyn(2, new Vector2(RandomSpeed, 0f));
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[0] == 2)
		{
			if (syn.SynCode[1] == 0)
			{
				ChewEnd();
			}
			else if (syn.SynCode[1] == 1)
			{
				base.State = ZombieState.Attack;
				base.dontChangeState = true;
				animator.SetInteger("Change", 0);
			}
			else if (syn.SynCode[1] == 2)
			{
				RandomSpeed = syn.Twofloat.x;
				base.Speed = DefSpeed;
			}
		}
	}

	public override void SpecialAnimEvent1()
	{
		int num = 0;
		Vector2 dirction = Vector2.left;
		if (!base.IsFacingLeft)
		{
			dirction = Vector2.right;
		}
		if (hypnoAttackTarget != null && hypnoAttackTarget.Hp <= 0)
		{
			hypnoAttackTarget = null;
		}
		if (isHypno)
		{
			if (hypnoAttackTarget != null)
			{
				num = hypnoAttackTarget.Hp / 80 + 2;
				if ((float)hypnoAttackTarget.Hp <= attackValue)
				{
					hypnoAttackTarget.DirectDead(canDropItem: true, 0f);
				}
				else
				{
					hypnoAttackTarget.Hurt((int)attackValue, dirction);
				}
			}
			else if (AttackGrid != null && AttackGrid.CurrPlantBase != null && AttackGrid.CurrPlantBase.isHypno)
			{
				num = ((!(AttackGrid.CurrPlantBase.Hp < attackValue)) ? ((int)attackValue / 80 + 5) : ((int)AttackGrid.CurrPlantBase.Hp / 80 + 5));
				AttackGrid.CurrPlantBase.Hurt(attackValue, dirction, this);
			}
		}
		else if (hypnoAttackTarget != null)
		{
			num = hypnoAttackTarget.Hp / 80 + 2;
			if ((float)hypnoAttackTarget.Hp <= attackValue)
			{
				hypnoAttackTarget.DirectDead(canDropItem: true, 0f);
			}
			else
			{
				hypnoAttackTarget.Hurt((int)attackValue, dirction);
			}
		}
		else if (AttackGrid != null && AttackGrid.CurrPlantBase != null && !AttackGrid.CurrPlantBase.isHypno)
		{
			num = ((!(AttackGrid.CurrPlantBase.Hp < attackValue)) ? ((int)attackValue / 80 + 5) : ((int)AttackGrid.CurrPlantBase.Hp / 80 + 5));
			AttackGrid.CurrPlantBase.Hurt(attackValue, dirction, this);
		}
		if (num > 0)
		{
			base.State = ZombieState.Attack;
			base.dontChangeState = true;
			animator.SetInteger("Change", 0);
			if (!GameManager.Instance.isClient)
			{
				Invoke("ChewEnd", num);
			}
			ServerSendSyn(1);
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.crocodileBite, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		MyTool.RandomOne(new List<UnityAction>
		{
			() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.crocodileYell1, base.transform.position);
			},
			() =>
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.crocodileYell2, base.transform.position);
			}
		});
	}

	public override void SpecialAnimEvent3()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.crocodileYell2, base.transform.position);
	}

	private void ChewEnd()
	{
		base.dontChangeState = false;
		base.State = ZombieState.Walk;
		ServerSendSyn(0);
	}

	protected override void InWaterChangeEvent()
	{
		if (base.State != ZombieState.Dead)
		{
			needInWater = !base.InWater;
			if (base.InWater)
			{
				BodyRenderer.sprite = WaterBody;
				animator.SetInteger("Change", 12);
				base.collider2d.enabled = false;
			}
			else
			{
				BodyRenderer.sprite = NormalBody;
				animator.SetInteger("Change", 11);
				base.collider2d.enabled = true;
			}
		}
	}

	public void ThrowInit(Vector2 goal)
	{
		canIce = false;
		base.dontChangeState = true;
		Shadow.enabled = false;
		base.collider2d.enabled = false;
		groundPos = goal;
		needMove = true;
	}

	protected override void CheckState()
	{
		base.CheckState();
		switch (base.State)
		{
		case ZombieState.Walk:
			if (base.InWater)
			{
				BodyRenderer.sprite = WaterBody;
				animator.SetInteger("Change", 12);
			}
			else
			{
				BodyRenderer.sprite = NormalBody;
				animator.SetInteger("Change", 11);
			}
			break;
		case ZombieState.Attack:
			animator.SetInteger("Change", 21);
			break;
		}
	}

	public override void SpecialAnimEvent4()
	{
		InWaterX = base.transform.position.x;
		if (AttackGrid != null)
		{
			base.dontChangeState = true;
			base.transform.position = new Vector3(AttackGrid.Position.x, base.transform.position.y);
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(new Vector3(base.transform.position.x, base.CurrGrid.Position.y), base.CurrGrid.Point.y);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
	}

	public override void SpecialAnimEvent5()
	{
		base.transform.position = new Vector3(InWaterX, base.transform.position.y);
	}
}
