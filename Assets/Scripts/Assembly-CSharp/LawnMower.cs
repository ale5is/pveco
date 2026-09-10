using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;

public class LawnMower : MonoBehaviour
{
	public int OnlineID;

	public Animator animator;

	public SpriteRenderer Shadow;

	public bool IsRun;

	private bool IsFacingLeft;

	private bool going;

	private bool inWater;

	private float NormalHigh;

	private Grid currGrid;

	private Grid nextGrid;

	private Vector2 MoveTarget;

	protected EFAudio eFAudio;

	protected virtual bool CanInWater { get; }

	public Grid CurrGrid
	{
		get
		{
			return currGrid;
		}
		set
		{
			if (value != currGrid && value != null)
			{
				currGrid = value;
				nextGrid = MapManager.Instance.GetNextGrid(currGrid, isRight: true);
				ResetMoveTarget();
			}
		}
	}

	public bool InWater
	{
		get
		{
			return inWater;
		}
		protected set
		{
			inWater = value;
			InWaterChangeEvent();
		}
	}

	protected void ResetMoveTarget()
	{
		if (nextGrid == null)
		{
			if (IsFacingLeft)
			{
				MoveTarget = new Vector2(-50f, currGrid.Position.y);
			}
			else
			{
				MoveTarget = new Vector2(50f, currGrid.Position.y);
			}
		}
		else if (currGrid.isSlope)
		{
			if (nextGrid.isSlope)
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, (currGrid.Position.y + nextGrid.Position.y) / 2f);
			}
			else
			{
				MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
			}
		}
		else if (nextGrid.isSlope)
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, currGrid.Position.y);
		}
		else
		{
			MoveTarget = new Vector2((currGrid.Position.x + nextGrid.Position.x) / 2f, nextGrid.Position.y);
		}
		if (IsFacingLeft)
		{
			MoveTarget -= new Vector2(0.1f, 0f);
		}
		else
		{
			MoveTarget += new Vector2(0.1f, 0f);
		}
	}

	private void Update()
	{
		UpdateEvent();
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(CurrGrid.Point.y, base.transform.position, 0.2f, needCapsule: false);
		if (IsRun)
		{
			for (int i = 0; i < zombiesByLine.Count; i++)
			{
				if (!zombiesByLine[i].isHypno)
				{
					zombiesByLine[i].CleanerDead();
					KillZombieEvent();
				}
			}
		}
		else
		{
			for (int j = 0; j < zombiesByLine.Count; j++)
			{
				if (!zombiesByLine[j].isHypno)
				{
					Launch(synClient: false);
					break;
				}
			}
		}
		if (!IsRun)
		{
			return;
		}
		float y = 0f;
		if (InWater)
		{
			y = -0.4f;
		}
		CurrGrid = MapManager.Instance.GetGridByWorldPos(base.transform.position, CurrGrid.Point.y);
		base.transform.position = Vector2.MoveTowards(base.transform.position, MoveTarget + new Vector2(0f, y), Time.deltaTime * 5f);
		if (base.transform.position.x > 8f)
		{
			DestroyMower();
		}
		if (CurrGrid.isWaterGrid && !CurrGrid.IsIce && Mathf.Abs(base.transform.position.x - currGrid.Position.x) < 0.4f && !InWater && !going)
		{
			StartCoroutine(MoveInWater());
		}
		if (Mathf.Abs(CurrGrid.Position.x - base.transform.position.x) > 0.65f && (nextGrid == null || !nextGrid.isWaterGrid || nextGrid.IsIce) && InWater && !going)
		{
			if (IsFacingLeft && CurrGrid.Position.x - base.transform.position.x > 0.65f)
			{
				StartCoroutine(MoveOutWater());
			}
			else if (!IsFacingLeft && base.transform.position.x - CurrGrid.Position.x > 0.65f)
			{
				StartCoroutine(MoveOutWater());
			}
		}
	}

	public void Init(Grid grid, Vector2 pos)
	{
		IsRun = false;
		CurrGrid = grid;
		IsFacingLeft = false;
		base.transform.position = pos;
		animator.speed = 0f;
		NormalHigh = pos.y;
		animator.transform.GetComponent<SortingGroup>().sortingOrder = grid.Point.y * 200 + 193;
	}

	public void Launch(bool synClient)
	{
		if (!GameManager.Instance.isClient || synClient)
		{
			StatsManager.Instance.AddStatsNum(StatsEnum.MowerStartNum);
			IsRun = true;
			animator.speed = 1f;
			LaunchEvent();
			if (GameManager.Instance.isServer)
			{
				SynMap synMap = new SynMap();
				synMap.SynCode[0] = 1;
				synMap.SynCode[1] = OnlineID;
				synMap.mapPos = base.transform.position;
				SocketServer.Instance.SendMapSyn(synMap);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!IsRun && collision.tag == "Zombie")
		{
			ZombieBase component = collision.GetComponent<ZombieBase>();
			if (component.CurrLine == CurrGrid.Point.y && !component.isHypno && !component.GetDead() && !(component is BungiZombie))
			{
				Launch(synClient: false);
			}
		}
	}

	private IEnumerator MoveInWater()
	{
		going = true;
		Shadow.enabled = false;
		while (base.transform.position.y > NormalHigh - 0.4f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, -1f) * Time.deltaTime * 5f);
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(CurrGrid.Position, CurrGrid.Point.y);
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
		if (!CanInWater)
		{
			if (eFAudio != null)
			{
				eFAudio.Close();
			}
			DestroyMower();
		}
		InWater = true;
		going = false;
	}

	private IEnumerator MoveOutWater()
	{
		going = true;
		while (base.transform.position.y < CurrGrid.Position.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(0f, 1f) * Time.deltaTime * 5f);
		}
		Shadow.enabled = true;
		InWater = false;
		going = false;
	}

	public void DestroyMower()
	{
		Object.Destroy(base.gameObject);
	}

	protected virtual void UpdateEvent()
	{
	}

	protected virtual void InWaterChangeEvent()
	{
	}

	protected virtual void KillZombieEvent()
	{
	}

	protected virtual void LaunchEvent()
	{
		eFAudio = AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Lawnmower, base.transform.position);
	}
}
