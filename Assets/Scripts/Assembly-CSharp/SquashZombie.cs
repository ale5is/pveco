using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class SquashZombie : PlantZombie
{
	private Vector3 direction;

	private GameObject Target;

	private bool isAttack;

	protected override GameObject Prefab => GameManager.Instance.GameConf.SquashZombie;

	protected override Vector2 SpeedRange => new Vector2(1.8f, 2.2f);

	protected override int attackValue2 => 1800;

	protected override void PlantZombieInit()
	{
		isAttack = false;
		PlantHeadAnimator.GetComponent<SortingGroup>().sortingOrder = 3;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(1f, isOver: false);
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 1)
		{
			Attack();
		}
		if (syn.SynCode[1] == 0)
		{
			RandomSpeed = syn.Twofloat.x;
			base.Speed = DefSpeed;
		}
	}

	protected override bool DoAction()
	{
		Attack();
		return false;
	}

	private bool Attack()
	{
		if (NormalDontAttackCondition())
		{
			return false;
		}
		if (isAttack)
		{
			return false;
		}
		Target = null;
		ZombieBase zombieByLineMinDistance = ZombieManager.Instance.GetZombieByLineMinDistance(base.CurrGrid.Point.y, base.transform.position, base.IsFacingLeft, !isHypno);
		PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.CurrGrid.Point.y, base.IsFacingLeft, isHypno);
		if (zombieByLineMinDistance != null && minDisPlant != null)
		{
			float num = Mathf.Abs(base.transform.position.x - minDisPlant.transform.position.x);
			float num2 = Mathf.Abs(base.transform.position.x - zombieByLineMinDistance.transform.position.x);
			if (num > num2)
			{
				Target = zombieByLineMinDistance.gameObject;
			}
			else
			{
				Target = minDisPlant.gameObject;
			}
		}
		else
		{
			if (zombieByLineMinDistance != null)
			{
				Target = zombieByLineMinDistance.gameObject;
			}
			if (minDisPlant != null)
			{
				Target = minDisPlant.gameObject;
			}
		}
		if (Target != null && Target.transform.position.x - base.transform.position.x < 1.8f && Target.transform.position.x - base.transform.position.x >= 0f)
		{
			isAttack = true;
		}
		else if (Target != null && Target.transform.position.x - base.transform.position.x > -1.8f && Target.transform.position.x - base.transform.position.x < 0f)
		{
			isAttack = true;
		}
		if (isAttack)
		{
			SetAnimChange(11);
			ServerSendSyn(1);
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Squashemm1, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Squashemm2, base.transform.position);
			}
			return true;
		}
		return false;
	}

	private IEnumerator DoFly()
	{
		if (base.InWater)
		{
			DirctOutWater();
		}
		Shadow.enabled = false;
		base.dontChangeState = true;
		float Y = Target.transform.position.y + 2f;
		while (base.transform.position.y < Y)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				if (Vector2.Distance(Target.transform.position, base.transform.position) < 3f)
				{
					direction = (Target.transform.position - base.transform.position + new Vector3(0f, 2.1f, 0f)).normalized;
				}
				base.transform.Translate(direction * 14f * Time.deltaTime);
			}
		}
		yield return new WaitForSeconds(0.2f);
		SetAnimChange(13);
		StartCoroutine(DoDown());
	}

	private IEnumerator DoDown()
	{
		Grid grid = MapManager.Instance.GetGridByWorldPos(base.transform.position, base.CurrLine);
		float Y = grid.Position.y;
		float speed = -10f;
		while (base.transform.position.y > Y)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				speed -= 20f * Time.deltaTime;
				base.transform.Translate(0f, speed * Time.deltaTime, 0f);
			}
		}
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.CurrGrid.Point.y, base.transform.position, 1f, !isHypno, needCapsule: true);
		for (int i = 0; i < zombiesByLine.Count; i++)
		{
			zombiesByLine[i].Hurt(attackValue2, Vector2.down);
		}
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.CurrGrid.Point.y, 1f, isHypno);
		for (int j = 0; j < linePlant.Count; j++)
		{
			linePlant[j].Hurt(attackValue2, Vector2.down, null, isFlat: true);
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gargantuar_thump, base.transform.position);
		if (grid.isNoIceWater && Mathf.Abs(grid.Position.x - base.transform.position.x) < 1.3f)
		{
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
			}
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(base.transform.position + new Vector3(0f, -0.1f), base.CurrGrid.Point.y);
			DirectDead(canDropItem: false, 0f);
		}
		else
		{
			GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SquashParticle);
			obj.transform.position = base.transform.position + new Vector3(0f, -0.5f);
			obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder();
			yield return new WaitForSeconds(0.5f);
			base.Hp = 0;
		}
	}

	public override void SpecialAnimEvent1()
	{
		StartCoroutine(DoFly());
	}
}
