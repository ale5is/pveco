using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shovel : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public static Shovel Instance;

	private Transform shovelImg;

	private bool isShovel;

	private Grid CurrGrid;

	public bool IsShovel
	{
		get
		{
			return isShovel;
		}
		private set
		{
			if (LV.Instance.CurrLVType != LVType.IZombie && GameManager.Instance.LocalPlayerSave.ShovelUnLock && LVManager.Instance.GameIsStart && !SpectatorList.Instance.LocalIsSpectator)
			{
				SeedBank.Instance.AllCancelPlace(NoSelect: true);
				CobCannonTarget.Instance.StopAim();
				isShovel = value;
				if (IsShovel)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Shovel, base.transform.position, isAll: true);
					shovelImg.localRotation = Quaternion.Euler(0f, 0f, -10f);
				}
				else
				{
					UpdateOnlinePreview(default, isShow: false);
					shovelImg.localRotation = Quaternion.Euler(0f, 0f, 0f);
					shovelImg.localPosition = Vector3.zero;
				}
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void CancelShovel()
	{
		isShovel = false;
		UpdateOnlinePreview(default, isShow: false);
		shovelImg.localRotation = Quaternion.Euler(0f, 0f, 0f);
		shovelImg.localPosition = Vector3.zero;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		IsShovel = !IsShovel;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		shovelImg.transform.localScale = new Vector2(1.2f, 1.2f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		shovelImg.transform.localScale = new Vector2(1f, 1f);
	}

	private void Start()
	{
		shovelImg = base.transform.Find("shovel");
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (!UIManager.Instance.IsChatBoxOpen && Input.GetKeyDown(KeyCode.Alpha1))
		{
			IsShovel = !IsShovel;
		}
		if (!IsShovel)
		{
			return;
		}
		shovelImg.position = Input.mousePosition;
		Grid gridPointByMouse = MapManager.Instance.GetGridPointByMouse();
		if (gridPointByMouse == null)
		{
			return;
		}
		Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Vector2.Distance(vector, gridPointByMouse.Position) < 1f)
		{
			if (CurrGrid == null)
			{
				CurrGrid = gridPointByMouse;
				UpdateOnlinePreview(gridPointByMouse.Position, isShow: true);
			}
			else if (CurrGrid != gridPointByMouse)
			{
				CurrGrid = gridPointByMouse;
				UpdateOnlinePreview(gridPointByMouse.Position, isShow: true);
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (gridPointByMouse.snow != null && gridPointByMouse.snow.SnowLvl > 0 && gridPointByMouse.IceRoadNum <= 0 && Vector2.Distance(vector, gridPointByMouse.Position) < 1f)
			{
				if (GameManager.Instance.isClient)
				{
					ToolApply toolApply = new ToolApply();
					toolApply.type = ToolType.Shovel;
					toolApply.GridPos = gridPointByMouse.Position;
					toolApply.Sound = 2;
					SocketClient.Instance.ApplyTool(toolApply);
				}
				else if (ClearPlant(gridPointByMouse, vector, GameManager.Instance.LocalPlayerSave.playerName))
				{
					BattlePlayerList.Instance.PlayShovelAnimation(gridPointByMouse.Position, 2, GameManager.Instance.LocalPlayerSave.playerName);
				}
				if (GameManager.Instance.isServer)
				{
					ToolApply toolApply2 = new ToolApply();
					toolApply2.type = ToolType.Shovel;
					toolApply2.GridPos = gridPointByMouse.Position;
					toolApply2.Sound = 2;
					SocketServer.Instance.SendShovelAnim(toolApply2);
				}
				StatsManager.Instance.AddStatsNum(StatsEnum.ShovelOffNum);
				IsShovel = false;
			}
			else if (gridPointByMouse.CurrPlantBase != null && !gridPointByMouse.CurrPlantBase.isHypno && Vector2.Distance(vector, gridPointByMouse.Position) < 1f)
			{
				if (GameManager.Instance.isClient)
				{
					ToolApply toolApply3 = new ToolApply();
					toolApply3.type = ToolType.Shovel;
					toolApply3.GridPos = gridPointByMouse.Position;
					toolApply3.Sound = 1;
					SocketClient.Instance.ApplyTool(toolApply3);
				}
				else if (ClearPlant(gridPointByMouse, vector, GameManager.Instance.LocalPlayerSave.playerName))
				{
					BattlePlayerList.Instance.PlayShovelAnimation(gridPointByMouse.Position, 1, GameManager.Instance.LocalPlayerSave.playerName);
				}
				if (GameManager.Instance.isServer)
				{
					ToolApply toolApply4 = new ToolApply();
					toolApply4.type = ToolType.Shovel;
					toolApply4.GridPos = gridPointByMouse.Position;
					toolApply4.Sound = 1;
					SocketServer.Instance.SendShovelAnim(toolApply4);
				}
				StatsManager.Instance.AddStatsNum(StatsEnum.ShovelOffNum);
				IsShovel = false;
			}
			else if (Vector2.Distance(vector, gridPointByMouse.Position) > 1.6f)
			{
				IsShovel = false;
			}
		}
		if (Input.GetMouseButtonDown(1) && IsShovel)
		{
			IsShovel = false;
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
			}
		}
	}

	private void UpdateOnlinePreview(Vector2 pos, bool isShow)
	{
		if (GameManager.Instance.isOnline)
		{
			ShovelPreview shovelPreview = new ShovelPreview();
			shovelPreview.GridPos = pos;
			shovelPreview.isShow = isShow;
			shovelPreview.PlayerName = GameManager.Instance.LocalPlayerSave.playerName;
			if (GameManager.Instance.isClient)
			{
				SocketClient.Instance.ApplyShovelPreview(shovelPreview);
			}
			if (GameManager.Instance.isServer)
			{
				SocketServer.Instance.ShovelPreview(shovelPreview, null);
			}
		}
	}

	public bool ClearPlant(Grid grid, Vector2 ClickedPos, string applicant)
	{
		if (grid == null)
		{
			return false;
		}
		if (grid.snow != null && grid.snow.SnowLvl > 0)
		{
			grid.snow.DirctClear(2, synClient: false);
			return true;
		}
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (!PvPSelector.Instance.IsSameTeam(applicant))
			{
				grid = MapManager.Instance.GetGridByWorldPos(MyTool.ReverseX(grid.Position));
				ClickedPos = MyTool.ReverseX(ClickedPos);
			}
			if (grid.CurrPlantBase != null && !PvPSelector.Instance.IsSameTeam(grid.CurrPlantBase.PlacePlayer, applicant))
			{
				return false;
			}
		}
		if (grid.CurrPlantBase != null && Vector2.Distance(ClickedPos, grid.CurrPlantBase.transform.position) < 1.5f)
		{
			if (grid.CurrPlantBase.CarryPlant == null)
			{
				if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.CanPlaceOnWater)
				{
					grid.CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
					return true;
				}
				if (grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.CanCarryOtherPlant)
				{
					grid.CurrPlantBase.ProtectPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
					return true;
				}
				grid.CurrPlantBase.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
				return true;
			}
			grid.CurrPlantBase.CarryPlant.Dead(isFlat: false, 0f, synClient: false, deadRattle: false);
			return true;
		}
		return false;
	}

	public void LvStart()
	{
		if (GameManager.Instance.LocalPlayerSave.ShovelUnLock && LV.Instance.CurrLVType != LVType.IZombie && LV.Instance.EnableShovel)
		{
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = Vector3.zero;
		}
	}
}
