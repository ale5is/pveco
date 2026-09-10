using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Grid
{
	public TeamType CurrGridType;

	public Vector2Int Point;

	public Vector2 Position;

	public int HotNum;

	private int lightNum;

	public int CoverNum;

	private bool haveGraveStone;

	private GraveStone GraveStone;

	private bool haveCrater;

	private GameObject Crater;

	private bool haveRightLadder;

	private GameObject RightLadder;

	private bool haveLeftLadder;

	private GameObject LeftLadder;

	private int iceRoadNum;

	public bool isEmpty;

	public bool isWaterGrid;

	private bool isIce;

	public bool isHardGrid;

	public bool isSlope;

	public bool isShadow;

	public bool isOccupied;

	public bool needChangeLine;

	public bool isHavePuddle;

	public bool isZombieSigned;

	public Snow snow;

	public bool CanPlaceZombie = true;

	public List<Vase> Vases = new List<Vase>();

	public Cage Cage;

	public CustomTile customTile;

	private PlantBase currPlantBase;

	private PlantBase currFloatPlant;

	public bool isNoIceWater
	{
		get
		{
			if (isWaterGrid)
			{
				return !IsIce;
			}
			return false;
		}
	}

	public int SnowLvl
	{
		get
		{
			if (!(snow == null))
			{
				return snow.SnowLvl;
			}
			return 0;
		}
	}

	public PlantBase CurrPlantBase
	{
		get
		{
			return currPlantBase;
		}
		set
		{
			currPlantBase = value;
		}
	}

	public PlantBase CurrFloatPlant
	{
		get
		{
			return currFloatPlant;
		}
		set
		{
			currFloatPlant = value;
		}
	}

	public int IceRoadNum
	{
		get
		{
			return iceRoadNum;
		}
		set
		{
			iceRoadNum = value;
			if (iceRoadNum < 0)
			{
				iceRoadNum = 0;
			}
			if (iceRoadNum > 0 && CurrPlantBase != null)
			{
				CurrPlantBase.Hurt(99999f, Vector2.zero, null, isFlat: true);
			}
		}
	}

	public bool HaveGraveStone
	{
		get
		{
			return haveGraveStone;
		}
		set
		{
			if (!haveGraveStone & value)
			{
				GraveStone = MapManager.Instance.GraveStoneUp(this);
				if (CurrPlantBase != null)
				{
					if (CurrPlantBase.ProtectPlant != null)
					{
						CurrPlantBase.ProtectPlant.Dead();
					}
					CurrPlantBase.Dead();
				}
			}
			if (GameManager.Instance.isServer)
			{
				GraveStoneSpawn graveStoneSpawn = new GraveStoneSpawn();
				graveStoneSpawn.MapPos = Position;
				graveStoneSpawn.isHave = value;
				graveStoneSpawn.Type = GraveStone.TypeId;
				SocketServer.Instance.SpawnGraveStone(graveStoneSpawn);
			}
			if (haveGraveStone && !value)
			{
				Object.Destroy(GraveStone.gameObject);
			}
			haveGraveStone = value;
		}
	}

	public bool HaveCrater
	{
		get
		{
			return haveCrater;
		}
		set
		{
			if (!haveCrater & value)
			{
				Crater = MapManager.Instance.CreateCrater(this);
				if (CurrPlantBase != null)
				{
					CurrPlantBase.Dead();
				}
			}
			if (haveCrater && !value)
			{
				Object.Destroy(Crater);
			}
			haveCrater = value;
		}
	}

	public bool HaveRightLadder
	{
		get
		{
			return haveRightLadder;
		}
		set
		{
			if (!haveRightLadder & value)
			{
				Ladder component = Object.Instantiate(GameManager.Instance.GameConf.Ladder).GetComponent<Ladder>();
				component.CreateInit(this, isLeft: false);
				RightLadder = component.gameObject;
			}
			if (haveRightLadder && !value)
			{
				Object.Destroy(RightLadder);
			}
			if (haveRightLadder != value && GameManager.Instance.isServer)
			{
				SynGrid synGrid = new SynGrid();
				synGrid.GridPos = Position;
				synGrid.SynCode[0] = 2;
				synGrid.SynCode[1] = 2;
				synGrid.isHave = value;
				SocketServer.Instance.SendGridState(synGrid);
			}
			haveRightLadder = value;
		}
	}

	public bool HaveLeftLadder
	{
		get
		{
			return haveLeftLadder;
		}
		private set
		{
			if (!haveLeftLadder & value)
			{
				Ladder component = Object.Instantiate(GameManager.Instance.GameConf.Ladder).GetComponent<Ladder>();
				component.CreateInit(this, isLeft: true);
				LeftLadder = component.gameObject;
			}
			if (haveLeftLadder && !value)
			{
				Object.Destroy(LeftLadder);
			}
			if (haveLeftLadder != value && GameManager.Instance.isServer)
			{
				SynGrid synGrid = new SynGrid();
				synGrid.GridPos = Position;
				synGrid.SynCode[0] = 2;
				synGrid.SynCode[1] = 1;
				synGrid.isHave = value;
				SocketServer.Instance.SendGridState(synGrid);
			}
			haveLeftLadder = value;
		}
	}

	public int LightNum
	{
		get
		{
			return lightNum;
		}
		set
		{
			lightNum = value;
			for (int i = 0; i < Vases.Count; i++)
			{
				Vases[i].CheckLight();
			}
		}
	}

	public bool IsIce
	{
		get
		{
			return isIce;
		}
		set
		{
			isIce = value;
			if (!isIce && CurrPlantBase != null && !CurrPlantBase.CanPlaceOnWater)
			{
				if (CurrPlantBase.CarryPlant != null)
				{
					CurrPlantBase.CarryPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
				}
				if (CurrPlantBase.ProtectPlant != null)
				{
					CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
				}
				CurrPlantBase.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(Position, Point.y);
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, Position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, Position);
				}
			}
		}
	}

	public Grid(Vector2Int point, Vector2 position)
	{
		Point = point;
		Position = position;
	}

	public void CheckLadder()
	{
		if (!GameManager.Instance.isClient && (HaveLeftLadder || HaveRightLadder))
		{
			bool flag = true;
			if (CurrPlantBase != null && (CurrPlantBase.MaxHp >= 1000f || (CurrPlantBase.ProtectPlant != null && CurrPlantBase.ProtectPlant.MaxHp >= 1000f) || (CurrPlantBase.CarryPlant != null && CurrPlantBase.CarryPlant.MaxHp >= 1000f)))
			{
				flag = false;
			}
			if (flag)
			{
				ClearLadder();
			}
		}
	}

	public void ClearLadder()
	{
		if (!GameManager.Instance.isClient)
		{
			HaveLeftLadder = false;
			HaveRightLadder = false;
		}
	}

	public void SetLadder(bool isLeft, bool isHave, bool synClient = false)
	{
		if (synClient || !GameManager.Instance.isClient)
		{
			if (isLeft)
			{
				HaveLeftLadder = isHave;
			}
			else
			{
				HaveRightLadder = isHave;
			}
		}
	}

	public void ClientSynGrave(int type, bool isHave)
	{
		if (!HaveGraveStone & isHave)
		{
			HaveGraveStone = true;
			if (GraveStone != null)
			{
				GraveStone.ClientSynType(type);
			}
		}
		else if (!isHave)
		{
			HaveGraveStone = false;
		}
	}

	public void ClientSynState(SynGrid syn)
	{
		if (syn.SynCode[0] == 1)
		{
			if (snow != null)
			{
				snow.DirctClear(syn.SynCode[1], synClient: true);
			}
		}
		else
		{
			if (syn.SynCode[0] != 2)
			{
				return;
			}
			if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
			{
				if (syn.SynCode[1] == 1)
				{
					SetLadder(isLeft: false, syn.isHave, synClient: true);
				}
				else if (syn.SynCode[1] == 2)
				{
					SetLadder(isLeft: true, syn.isHave, synClient: true);
				}
			}
			else if (syn.SynCode[1] == 1)
			{
				SetLadder(isLeft: true, syn.isHave, synClient: true);
			}
			else if (syn.SynCode[1] == 2)
			{
				SetLadder(isLeft: false, syn.isHave, synClient: true);
			}
		}
	}

	public void DesToryGrid()
	{
		if (HaveCrater)
		{
			Crater.GetComponent<Crater>().DestoryThis();
		}
		if (HaveGraveStone)
		{
			GraveStone.GetComponent<GraveStone>().DestoryThis();
		}
		if (HaveRightLadder)
		{
			Object.Destroy(RightLadder);
		}
		if (HaveLeftLadder)
		{
			Object.Destroy(LeftLadder);
		}
		if (CurrFloatPlant != null)
		{
			CurrFloatPlant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
		}
		if (CurrPlantBase != null)
		{
			if (CurrPlantBase.ProtectPlant != null)
			{
				CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			}
			CurrPlantBase.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
		}
	}
}
