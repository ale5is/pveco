using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPult : MonoBehaviour
{
	private PlantBase targetplant;

	private ZombieBase targetzombie;

	private Vector2 startPos;

	private Vector2 midPos;

	private Vector2 lastTargetPos;

	private Vector2 zombieDeadPos;

	protected float percent;

	private float percentSpeed;

	private bool isHit;

	private bool targetDead;

	protected bool isHypno;

	private float rotationNum;

	protected int attackValue;

	private float GroundY;

	private Transform Shadow;

	private int CurrLine;

	private Grid currGrid;

	private Grid nextGrid;

	private bool IsFacingLeft;

	private Vector2 slopeShadowTarget;

	private Vector2 UmbrellaTarget;

	private bool isHitUmbrella;

	private bool checkUmbrellaOver;

	public Transform Bullet;

	private float windRate;

	protected abstract float Speed { get; }

	protected virtual bool NeedPeaAudio { get; }

	protected abstract GameObject Prefab { get; }

	protected int FinalDamage
	{
		get
		{
			float num = (float)attackValue * (1f + windRate * 2f);
			if (num < 0f)
			{
				num = 0f;
			}
			return (int)num;
		}
	}

	public Grid CurrGrid
	{
		get
		{
			return currGrid;
		}
		set
		{
			if (value == currGrid || value == null)
			{
				return;
			}
			currGrid = value;
			nextGrid = MapManager.Instance.GetNextGrid(currGrid, !IsFacingLeft);
			if (!currGrid.isSlope)
			{
				return;
			}
			if (nextGrid == null)
			{
				if (IsFacingLeft)
				{
					slopeShadowTarget = new Vector2(-50f, currGrid.Position.y);
				}
				else
				{
					slopeShadowTarget = new Vector2(50f, currGrid.Position.y);
				}
			}
			else if (nextGrid.isSlope)
			{
				slopeShadowTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, (currGrid.Position.y + nextGrid.Position.y) / 2f);
			}
			else
			{
				slopeShadowTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
			}
		}
	}

	public void Init(PlantBase plant, ZombieBase zombie, Vector2 startPos, int attackValue, int line, bool isHyp)
	{
		if (plant != null)
		{
			GroundY = plant.currGrid.Position.y - 0.3f;
			targetplant = plant;
			targetzombie = null;
			lastTargetPos = targetplant.transform.position;
		}
		else
		{
			GroundY = zombie.CurrGrid.Position.y - 0.3f;
			targetplant = null;
			targetzombie = zombie;
			lastTargetPos = targetzombie.transform.position;
		}
		isHitUmbrella = false;
		checkUmbrellaOver = false;
		IsFacingLeft = lastTargetPos.x < startPos.x;
		CurrLine = line;
		isHypno = isHyp;
		targetDead = false;
		percent = 0f;
		isHit = false;
		this.attackValue = AttackValueHandle(attackValue);
		this.startPos = startPos;
		Bullet.GetComponent<SpriteRenderer>().sortingOrder = line * 200 + FixedInfo.BulletSort;
		percentSpeed = Speed / (lastTargetPos - this.startPos).magnitude;
		MapBase nearestMap = MapManager.Instance.GetNearestMap(startPos);
		int windScale = SkyManager.Instance.WindScale;
		float num = 1f;
		if (windScale > 0)
		{
			windRate = (float)windScale * 0.05f;
			if (windRate > 0.25f)
			{
				windRate = 0.25f;
			}
			if (IsFacingLeft == SkyManager.Instance.WindTowardRight)
			{
				windRate = 0f - windRate;
			}
			if (nearestMap.IsFacingLeft)
			{
				windRate = 0f - windRate;
			}
			num += windRate;
		}
		percentSpeed *= num;
		if (percentSpeed > Speed / 6f)
		{
			percentSpeed = Speed / 6f;
		}
		base.transform.position = startPos;
		rotationNum = Random.Range(60, 150);
		base.transform.SetParent(nearestMap.transform);
		Shadow = base.transform.Find("Shadow");
	}

	protected virtual int AttackValueHandle(int Value)
	{
		return Value;
	}

	private void Update()
	{
		if (isHit)
		{
			return;
		}
		CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
		if (CurrGrid.isSlope)
		{
			float num = base.transform.position.x - CurrGrid.Position.x;
			Shadow.transform.position = new Vector3(base.transform.position.x, CurrGrid.Position.y - 0.5f + num / Mathf.Abs(slopeShadowTarget.x - CurrGrid.Position.x) * Mathf.Abs(slopeShadowTarget.y - CurrGrid.Position.y));
		}
		else
		{
			Shadow.transform.position = new Vector3(base.transform.position.x, CurrGrid.Position.y - 0.5f);
		}
		float num2 = 1f - Mathf.Abs(base.transform.position.y - startPos.y) / 8f;
		Shadow.transform.localScale = new Vector3(num2, num2);
		if (isHitUmbrella)
		{
			if (percent >= 0.8f)
			{
				Destroy();
			}
			percent += percentSpeed * Time.deltaTime;
			base.transform.position = MyTool.Bezier(percent, startPos, midPos, UmbrellaTarget);
			return;
		}
		if (percent > 0.9f && !checkUmbrellaOver)
		{
			List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(MapManager.Instance.GetGridByWorldPos(lastTargetPos, CurrLine), 1);
			for (int i = 0; i < aroundGrid.Count; i++)
			{
				if (aroundGrid[i].CurrPlantBase != null && aroundGrid[i].CurrPlantBase.isHypno == !isHypno)
				{
					if (aroundGrid[i].CurrPlantBase is Umbrellaleaf)
					{
						if (aroundGrid[i].CurrPlantBase.GetComponent<Umbrellaleaf>().Block(FinalDamage))
						{
							isHitUmbrella = true;
						}
					}
					else if (aroundGrid[i].CurrPlantBase.CarryPlant is Umbrellaleaf && aroundGrid[i].CurrPlantBase.CarryPlant.GetComponent<Umbrellaleaf>().Block(FinalDamage))
					{
						isHitUmbrella = true;
					}
				}
				if (isHitUmbrella)
				{
					break;
				}
			}
			if (!isHitUmbrella)
			{
				List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(lastTargetPos, 2.6f, isHypno, needCapsule: true);
				for (int j = 0; j < zombies.Count; j++)
				{
					if (zombies[j] is UmbrellaleafZombie && zombies[j].GetComponent<UmbrellaleafZombie>().Block())
					{
						isHitUmbrella = true;
						zombies[j].Hurt(10, Vector2.up);
						break;
					}
				}
			}
			if (isHitUmbrella)
			{
				percent = 0f;
				int num3 = 1;
				if (startPos.x > lastTargetPos.x)
				{
					num3 = -1;
				}
				startPos = base.transform.position;
				UmbrellaTarget = startPos + new Vector2(2 * num3, 0f);
				percentSpeed = 3f / (UmbrellaTarget - startPos).magnitude;
				midPos = MyTool.GetMiddlePosition(startPos, UmbrellaTarget);
			}
			checkUmbrellaOver = true;
		}
		if (targetplant == null)
		{
			if (!targetDead && targetzombie.Hp > 0 && (targetzombie.collider2d.enabled || targetzombie.CanNoCollGet))
			{
				lastTargetPos = targetzombie.transform.position;
				zombieDeadPos = targetzombie.transform.position;
			}
			else if (!targetDead)
			{
				targetDead = true;
				lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
				if ((targetzombie.transform.position.x < startPos.x && !isHypno) || (targetzombie.transform.position.x > startPos.x && isHypno))
				{
					targetDead = true;
					lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
				}
			}
		}
		else if (!targetDead && targetplant.Hp > 0f)
		{
			lastTargetPos = targetplant.transform.position;
			zombieDeadPos = targetplant.transform.position;
		}
		else if (!targetDead)
		{
			targetDead = true;
			lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
			if ((targetplant.transform.position.x < startPos.x && !isHypno) || (targetplant.transform.position.x > startPos.x && isHypno))
			{
				targetDead = true;
				lastTargetPos = zombieDeadPos + new Vector2(0f, -1f);
			}
		}
		if (percent >= 1f)
		{
			if (!targetDead)
			{
				if (targetplant != null)
				{
					if ((!isHypno && targetplant.isHypno) || (isHypno && !targetplant.isHypno))
					{
						targetplant.Hurt(FinalDamage, Vector2.down, null);
						if (NeedPeaAudio)
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
				}
				else if (targetzombie != null && targetzombie.gameObject.activeSelf && ((isHypno && targetzombie.isHypno) || (!isHypno && !targetzombie.isHypno)))
				{
					targetzombie.Hurt(FinalDamage, Vector2.down);
				}
			}
			HitEvent(targetplant, targetzombie, Bullet.GetComponent<SpriteRenderer>().sortingOrder);
			Destroy();
		}
		if (percent > 0.2f && base.transform.position.y < GroundY)
		{
			HitEvent(null, null, Bullet.GetComponent<SpriteRenderer>().sortingOrder);
			Destroy();
		}
		percent += percentSpeed * Time.deltaTime;
		if (percent > 1f)
		{
			percent = 1f;
		}
		Bullet.Rotate(new Vector3(0f, 0f, (0f - rotationNum) * Time.deltaTime));
		if (percent < 0.6f)
		{
			midPos = Vector2.Lerp(startPos, lastTargetPos, 0.5f);
			midPos.y += Mathf.Abs(startPos.x - lastTargetPos.x);
			if (midPos.y < startPos.y + 6.5f)
			{
				midPos.y = startPos.y + 6.5f;
			}
		}
		base.transform.position = MyTool.Bezier(percent, startPos, midPos, lastTargetPos);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Obstacle")
		{
			Obstacle component = collision.transform.GetComponent<Obstacle>();
			if (component.ContainLine(CurrLine))
			{
				component.HurtThis(attackValue);
				HitEvent(null, null, Bullet.GetComponent<SpriteRenderer>().sortingOrder);
				Destroy();
			}
		}
		if (!(targetzombie == null) && collision.transform == targetzombie.transform && collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			isHit = true;
			HitEvent(null, componentInParent, Bullet.GetComponent<SpriteRenderer>().sortingOrder);
			if (componentInParent != null && componentInParent.gameObject.activeSelf && ((isHypno && componentInParent.isHypno) || (!isHypno && !componentInParent.isHypno)))
			{
				componentInParent.Hurt(FinalDamage, Vector2.down);
			}
			Destroy();
		}
	}

	protected abstract void HitEvent(PlantBase plant, ZombieBase zombie, int sortOrder);

	private void Destroy()
	{
		PoolManager.Instance.PushObj(Prefab, base.gameObject);
	}
}
