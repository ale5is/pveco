using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
	protected bool isHit;

	protected bool isHypno;

	protected bool isLow;

	protected float MoveSpeed;

	protected Vector2 Dirction;

	protected int attackValue;

	protected int CurrLine;

	private bool FirstTp;

	private bool HaveShadow;

	protected Grid currGrid;

	private Grid nextGrid;

	private Vector2 slopeShadowTarget;

	protected Vector2 ShadowScale;

	protected SpriteRenderer Shadow;

	private float RealSpeed;

	protected MapBase CurrMap;

	protected bool isPultTrack;

	private PlantBase targetplant;

	private ZombieBase targetzombie;

	private Vector2 startPos;

	private Vector2 midPos;

	private Vector2 lastTargetPos;

	private Vector2 zombieDeadPos;

	protected float percent;

	private float percentSpeed;

	private float GroundY;

	private bool targetDead;

	private Vector2 UmbrellaTarget;

	private bool isHitUmbrella;

	private bool checkUmbrellaOver;

	private float windRate;

	public bool IsFacingLeft
	{
		get
		{
			if (!isPultTrack)
			{
				return Dirction.x < 0f;
			}
			return lastTargetPos.x < startPos.x;
		}
	}

	protected virtual bool canBounce { get; }

	protected virtual bool canUpBounce { get; }

	protected virtual bool canBlow { get; } = true;

	protected int FinalDamage
	{
		get
		{
			float num = 20f * RealSpeed / MoveSpeed - 20f;
			float num2 = (float)attackValue + num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			return (int)num2;
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
			CurrGridChange();
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

	private void Update()
	{
		if (CurrLine == -1)
		{
			CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position);
		}
		else
		{
			CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrLine);
		}
		if (isPultTrack)
		{
			PultMove();
		}
		else if (canBlow)
		{
			int windScale = SkyManager.Instance.WindScale;
			if (windScale > 0)
			{
				int num = (SkyManager.Instance.WindTowardRight ? 1 : (-1));
				if (CurrMap.IsFacingLeft)
				{
					num = -num;
				}
				Vector2 dirction = Dirction;
				Dirction += new Vector2(Time.deltaTime * (float)num * 0.2f * (float)windScale, 0f);
				if (dirction.x > 0f != Dirction.x > 0f)
				{
					base.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector2(Dirction.x, 0f));
				}
				float num2 = Time.deltaTime * (0.8f + (float)windScale * 0.3f);
				if (CurrMap.IsFacingLeft)
				{
					num2 = 0f - num2;
				}
				if (SkyManager.Instance.WindTowardRight == Dirction.x > 0f)
				{
					RealSpeed += num2;
				}
				else
				{
					RealSpeed -= num2;
				}
				if (RealSpeed < MoveSpeed * 0.4f)
				{
					RealSpeed = MoveSpeed * 0.4f;
				}
			}
			Vector2 vector = Dirction.normalized * RealSpeed * Time.deltaTime;
			if (!isHit)
			{
				base.transform.position += new Vector3(vector.x, vector.y);
			}
		}
		UpdateThis();
		if (!HaveShadow || CurrGrid == null)
		{
			return;
		}
		if (CurrGrid.isSlope)
		{
			float num3 = base.transform.position.x - CurrGrid.Position.x;
			Shadow.transform.position = new Vector3(base.transform.position.x, CurrGrid.Position.y - 0.5f + num3 / Mathf.Abs(slopeShadowTarget.x - CurrGrid.Position.x) * Mathf.Abs(slopeShadowTarget.y - CurrGrid.Position.y));
			return;
		}
		float num4 = CurrGrid.Position.y - 0.5f;
		if (base.transform.position.y - num4 < 0.2f)
		{
			num4 = base.transform.position.y - 0.2f;
		}
		Shadow.transform.position = new Vector3(base.transform.position.x, num4);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Portal")
		{
			Portal component = collision.GetComponent<Portal>();
			if (CurrLine != component.CurrGrid.Point.y)
			{
				return;
			}
			if (!FirstTp)
			{
				float x = 0.2f;
				if (base.transform.position.x > collision.transform.position.x)
				{
					x = -0.2f;
				}
				Portal nextPortal = component.GetNextPortal();
				base.transform.position = nextPortal.transform.position + new Vector3(x, 0f);
				CurrLine = nextPortal.CurrGrid.Point.y;
				FirstTp = true;
				SetSortingOrder(CurrLine * 200 + 199);
			}
			else if (Dirction.x != 0f)
			{
				if (Dirction.x > 0f && base.transform.position.x < collision.transform.position.x)
				{
					Portal nextPortal2 = component.GetNextPortal();
					base.transform.position = nextPortal2.transform.position + new Vector3(0.2f, 0f);
					CurrLine = nextPortal2.CurrGrid.Point.y;
					SetSortingOrder(CurrLine * 200 + 199);
				}
				else if (Dirction.x < 0f && base.transform.position.x > collision.transform.position.x)
				{
					Portal nextPortal3 = component.GetNextPortal();
					base.transform.position = nextPortal3.transform.position + new Vector3(-0.2f, 0f);
					CurrLine = nextPortal3.CurrGrid.Point.y;
					SetSortingOrder(CurrLine * 200 + 199);
				}
			}
		}
		if (collision.tag == "Jelly")
		{
			JellyShroom component2 = collision.GetComponent<JellyShroom>();
			if (component2.CurrLine == CurrLine || (CurrLine == -1 && Dirction.x != 0f))
			{
				if (component2.GetNormalState())
				{
					if (canUpBounce && isHypno == component2.isHypno)
					{
						component2.AddBullet(this);
						attackValue += 20;
					}
				}
				else if (canBounce)
				{
					Dirction = -Dirction;
					base.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector2(Dirction.x, 0f));
				}
			}
		}
		TriggerEnter(collision);
	}

	protected void BaseInit()
	{
		FirstTp = false;
		HaveShadow = false;
		isPultTrack = false;
		RealSpeed = MoveSpeed;
		CurrMap = MapManager.Instance.GetNearestMap(base.transform.position);
		base.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector2(Dirction.normalized.x, 0f));
		Transform transform = base.transform.Find("Shadow");
		if (transform != null && Shadow == null)
		{
			Shadow = transform.GetComponent<SpriteRenderer>();
			ShadowScale = Shadow.transform.localScale;
		}
		if (CurrLine != -1)
		{
			HaveShadow = Shadow != null;
		}
	}

	protected abstract void SetSortingOrder(int sorting);

	protected abstract void HitEvent();

	protected abstract void DestoryBullet();

	protected virtual void UpdateThis()
	{
	}

	protected virtual void SetPultRotation(Quaternion quaternion)
	{
	}

	protected virtual void TriggerEnter(Collider2D collision)
	{
	}

	protected virtual void CurrGridChange()
	{
	}

	public void PultInit(PlantBase plant, ZombieBase zombie, Vector2 startPos)
	{
		if (isHit)
		{
			return;
		}
		targetDead = false;
		if (plant != null)
		{
			GroundY = plant.currGrid.Position.y - 0.3f;
			targetplant = plant;
			targetzombie = null;
			lastTargetPos = targetplant.transform.position;
		}
		else if (zombie != null)
		{
			GroundY = zombie.CurrGrid.Position.y - 0.3f;
			targetplant = null;
			targetzombie = zombie;
			lastTargetPos = targetzombie.transform.position;
		}
		else
		{
			targetDead = true;
			lastTargetPos = new Vector2(IsFacingLeft ? (base.transform.position.x - 5f) : (base.transform.position.x + 5f), currGrid.Position.y - 1f);
		}
		isHitUmbrella = false;
		checkUmbrellaOver = false;
		percent = 0f;
		isHit = false;
		this.startPos = startPos;
		percentSpeed = 6f / (lastTargetPos - this.startPos).magnitude;
		if (percentSpeed > 1f)
		{
			percentSpeed = 1f;
		}
		base.transform.position = startPos;
		isPultTrack = true;
		Dirction = Vector2.down;
		int windScale = SkyManager.Instance.WindScale;
		float num = 1f;
		if (windScale > 0 && canBlow)
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
			if (CurrMap.IsFacingLeft)
			{
				windRate = 0f - windRate;
			}
			num += windRate;
		}
		percentSpeed *= num;
	}

	private void PultMove()
	{
		if (isHit)
		{
			return;
		}
		float num = 1f - Mathf.Abs(base.transform.position.y - startPos.y) / 8f;
		Shadow.transform.localScale = ShadowScale * num;
		if (isHitUmbrella)
		{
			if (percent >= 0.8f)
			{
				DestoryBullet();
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
						if (aroundGrid[i].CurrPlantBase.GetComponent<Umbrellaleaf>().Block(attackValue))
						{
							isHitUmbrella = true;
						}
					}
					else if (aroundGrid[i].CurrPlantBase.CarryPlant is Umbrellaleaf && aroundGrid[i].CurrPlantBase.CarryPlant.GetComponent<Umbrellaleaf>().Block(attackValue))
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
						zombies[j].Hurt(10, Vector2.down);
						break;
					}
				}
			}
			if (isHitUmbrella)
			{
				percent = 0f;
				int num2 = 1;
				if (startPos.x > lastTargetPos.x)
				{
					num2 = -1;
				}
				startPos = base.transform.position;
				UmbrellaTarget = startPos + new Vector2(2 * num2, 0f);
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
						targetplant.Hurt(attackValue, Vector2.down, null);
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
				else if (targetzombie != null && targetzombie.gameObject.activeSelf && ((isHypno && targetzombie.isHypno) || (!isHypno && !targetzombie.isHypno)))
				{
					targetzombie.Hurt(attackValue, Vector2.down);
				}
			}
			HitEvent();
			isHit = true;
		}
		if (percent > 0.2f && base.transform.position.y < GroundY)
		{
			HitEvent();
			isHit = true;
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
		percent += percentSpeed * Time.deltaTime;
		if (percent > 1f)
		{
			percent = 1f;
		}
		if (percent < 0.6f)
		{
			midPos = Vector2.Lerp(startPos, lastTargetPos, 0.5f);
			midPos.y += Mathf.Abs(startPos.x - lastTargetPos.x);
			if (midPos.y < startPos.y + 6.5f)
			{
				midPos.y = startPos.y + 6.5f;
			}
		}
		Vector3 position = base.transform.position;
		base.transform.position = MyTool.Bezier(percent, startPos, midPos, lastTargetPos);
		if (!isHit)
		{
			Vector3 toDirection = position - base.transform.position;
			toDirection.z = 0f;
			SetPultRotation(Quaternion.FromToRotation(Vector3.left, toDirection));
		}
	}
}
