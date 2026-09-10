using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class Squash : PlantBase
{
	private Vector3 direction;

	private GameObject Target;

	private bool isAttack;

	private int LineNum;

	public Texture2D eyeBrow;

	public SpriteRenderer EyeRenderer;

	public SpriteRenderer EyebrowRenderer;

	public override float MaxHp => 800f;

	protected override int attackValue => 1800;

	protected override bool haveSpEye => true;

	protected override void OnInitForAll()
	{
		EyeRenderer.enabled = true;
		EyebrowRenderer.enabled = true;
		EyeREnderer.material.SetTexture("_EyeTex", null);
	}

	protected override void OnInitForPlace()
	{
		isAttack = false;
		LineNum = base.currGrid.Point.y;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(1f, isOver: false);
		}
	}

	protected override bool DoAction()
	{
		Attack();
		return false;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0 && base.currGrid != null)
		{
			Attack();
		}
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
		ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(base.currGrid.Point.y, base.transform.position, isHypno);
		PlantBase minDisPlant = MapManager.Instance.GetMinDisPlant(base.transform.position, base.currGrid.Point.y, !isHypno);
		if (zombieByLineMinDisNoDir != null && minDisPlant != null)
		{
			float num = Mathf.Abs(base.transform.position.x - minDisPlant.transform.position.x);
			float num2 = Mathf.Abs(base.transform.position.x - zombieByLineMinDisNoDir.transform.position.x);
			if (num > num2)
			{
				Target = zombieByLineMinDisNoDir.gameObject;
			}
			else
			{
				Target = minDisPlant.gameObject;
			}
		}
		else
		{
			if (zombieByLineMinDisNoDir != null)
			{
				Target = zombieByLineMinDisNoDir.gameObject;
			}
			if (minDisPlant != null)
			{
				Target = minDisPlant.gameObject;
			}
		}
		if (Target != null && Target.transform.position.x - base.transform.position.x < 2.4f && Target.transform.position.x - base.transform.position.x >= 0f)
		{
			if (base.IsFacingLeft)
			{
				SetAnimChange(12);
			}
			else
			{
				SetAnimChange(11);
			}
			Emmm();
			return true;
		}
		if (Target != null && Target.transform.position.x - base.transform.position.x > -2.4f && Target.transform.position.x - base.transform.position.x < 0f)
		{
			if (base.IsFacingLeft)
			{
				SetAnimChange(11);
			}
			else
			{
				SetAnimChange(12);
			}
			Emmm();
			return true;
		}
		return false;
	}

	private void Emmm()
	{
		direction = (Target.transform.position - base.transform.position + new Vector3(0f, 2.1f, 0f)).normalized;
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Squashemm1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Squashemm2, base.transform.position);
		}
		isAttack = true;
		ServerSendSyn(0);
		UnFrozen(10);
		Dead(isFlat: false, 5f);
	}

	private IEnumerator DoFly()
	{
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
		animatorSorting.sortingOrder = GetBulletSortOrder();
		yield return new WaitForSeconds(0.2f);
		SetAnimChange(13);
		StartCoroutine(DoDown());
	}

	private IEnumerator DoDown()
	{
		Grid grid = MapManager.Instance.GetGridByWorldPos(base.transform.position, LineNum);
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
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 1f, isHypno, needCapsule: true);
		for (int i = 0; i < zombies.Count; i++)
		{
			zombies[i].Hurt(GetAttackValue(), Vector2.down);
		}
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.currGrid.Point.y, 1f, !isHypno);
		for (int j = 0; j < linePlant.Count; j++)
		{
			linePlant[j].Hurt(GetAttackValue(), Vector2.down, null, isFlat: true);
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
			PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(base.transform.position + new Vector3(0f, -0.1f), base.currGrid.Point.y);
			Dead();
		}
		else
		{
			GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SquashParticle);
			obj.transform.position = base.transform.position + new Vector3(0f, -0.5f);
			obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder();
			yield return new WaitForSeconds(0.5f);
			Dead();
		}
		if (GameManager.Instance.isClient)
		{
			yield break;
		}
		int num = 0;
		for (int k = 0; k < zombies.Count; k++)
		{
			if (zombies[k].GetDead())
			{
				num++;
			}
		}
		if (num >= 10)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.Squash10, PlacePlayer);
		}
	}

	protected override void GoAwakeSpecial()
	{
		EyeRenderer.enabled = true;
		EyebrowRenderer.enabled = true;
		EyeREnderer.material.SetTexture("_EyeTex", null);
	}

	protected override void GoSleepSpecial()
	{
		EyeRenderer.enabled = false;
		EyebrowRenderer.enabled = false;
		EyeREnderer.material.SetTexture("_EyeTex", eyeBrow);
	}

	public override void PlaceOverEvent()
	{
		if (!GameManager.Instance.isClient)
		{
			Attack();
		}
	}

	protected override void SetEyeTex(int EyeType)
	{
		if (EyeType == 100)
		{
			PlayAnim("anim_blink", 1);
		}
	}

	public override void SpecialAnimEvent1()
	{
		StartCoroutine(DoFly());
	}
}
