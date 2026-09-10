using SocketSave;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Glove : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public static Glove Instance;

	public Transform Indicator;

	public Sprite AllGlove1;

	public Sprite AllGlove2;

	public Sprite PlantGlove1;

	public Sprite PlantGlove2;

	public Sprite ZombieGlove1;

	public Sprite ZombieGlove2;

	private Transform gloveImg;

	private bool isGlove;

	private Grid CurrGrid;

	private PlantBase currPlant;

	private ZombieBase currZombie;

	private bool isPlantGrove
	{
		get
		{
			if (!PlayerManager.Instance.EnableCreate)
			{
				return LV.Instance.EnablePlantGlove;
			}
			return true;
		}
	}

	private bool isZombieGrove
	{
		get
		{
			if (!PlayerManager.Instance.EnableCreate)
			{
				return LV.Instance.EnableZombieGlove;
			}
			return true;
		}
	}

	public bool IsGlove
	{
		get
		{
			return isGlove;
		}
		private set
		{
			if (LV.Instance.CurrLVType == LVType.IZombie || !GameManager.Instance.LocalPlayerSave.ShovelUnLock || !LVManager.Instance.GameIsStart || SpectatorList.Instance.LocalIsSpectator)
			{
				return;
			}
			SeedBank.Instance.AllCancelPlace(NoSelect: true);
			CobCannonTarget.Instance.StopAim();
			isGlove = value;
			if (IsGlove)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
				return;
			}
			currPlant = null;
			currZombie = null;
			if (isPlantGrove && isZombieGrove)
			{
				gloveImg.GetComponent<Image>().sprite = AllGlove1;
			}
			else if (isPlantGrove)
			{
				gloveImg.GetComponent<Image>().sprite = PlantGlove1;
			}
			else if (isZombieGrove)
			{
				gloveImg.GetComponent<Image>().sprite = ZombieGlove1;
			}
			UpdateOnlinePreview(default, isShow: false);
			gloveImg.localPosition = Vector3.zero;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void CancelShovel()
	{
		isGlove = false;
		UpdateOnlinePreview(default, isShow: false);
		gloveImg.localRotation = Quaternion.Euler(0f, 0f, 0f);
		gloveImg.localPosition = Vector3.zero;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!IsGlove)
		{
			IsGlove = true;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		gloveImg.transform.localScale = new Vector2(1.2f, 1.2f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		gloveImg.transform.localScale = new Vector2(1f, 1f);
	}

	private void Start()
	{
		gloveImg = base.transform.Find("glove");
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (!UIManager.Instance.IsChatBoxOpen && Input.GetKeyDown(KeyCode.Alpha4))
		{
			IsGlove = !IsGlove;
		}
		if (!IsGlove)
		{
			return;
		}
		gloveImg.position = Input.mousePosition;
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
			if (currPlant != null)
			{
				if (SeedBank.Instance.CheckPlant(currPlant, gridPointByMouse, -2, currPlant.PlacePlayer))
				{
					if (GameManager.Instance.isClient)
					{
						ToolApply toolApply = new ToolApply();
						toolApply.type = ToolType.Glove;
						toolApply.GridPos = gridPointByMouse.Position;
						toolApply.OnlineId = currPlant.OnlineId;
						SocketClient.Instance.ApplyTool(toolApply);
					}
					else
					{
						currPlant.MoveToGrid(gridPointByMouse);
					}
				}
				IsGlove = false;
			}
			else if (currZombie != null)
			{
				if (GameManager.Instance.isClient)
				{
					ToolApply toolApply2 = new ToolApply();
					toolApply2.type = ToolType.Glove;
					toolApply2.GridPos = vector;
					toolApply2.OnlineId = currZombie.OnlineId;
					SocketClient.Instance.ApplyTool(toolApply2);
				}
				else
				{
					currZombie.TeleportTo(new Vector2(vector.x, gridPointByMouse.Position.y));
				}
				IsGlove = false;
			}
			else if (Vector2.Distance(vector, gridPointByMouse.Position) < 1.6f)
			{
				if (GetTarget(gridPointByMouse, vector, GameManager.Instance.LocalPlayerSave.playerName))
				{
					if (isPlantGrove && isZombieGrove)
					{
						gloveImg.GetComponent<Image>().sprite = AllGlove2;
					}
					else if (isPlantGrove)
					{
						gloveImg.GetComponent<Image>().sprite = PlantGlove2;
					}
					else if (isZombieGrove)
					{
						gloveImg.GetComponent<Image>().sprite = ZombieGlove2;
					}
				}
			}
			else
			{
				IsGlove = false;
			}
		}
		if (Input.GetMouseButtonDown(1) && IsGlove)
		{
			IsGlove = false;
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

	public bool GetTarget(Grid grid, Vector2 ClickedPos, string applicant)
	{
		if (grid == null)
		{
			return false;
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
		PlantBase plantBase = null;
		if (isPlantGrove && grid.CurrPlantBase != null && Vector2.Distance(ClickedPos, grid.CurrPlantBase.transform.position) < 1.5f)
		{
			plantBase = ((!(grid.CurrPlantBase.CarryPlant == null)) ? grid.CurrPlantBase.CarryPlant : ((grid.CurrPlantBase.ProtectPlant != null && grid.CurrPlantBase.CanPlaceOnWater) ? grid.CurrPlantBase.ProtectPlant : ((!(grid.CurrPlantBase.ProtectPlant != null) || !grid.CurrPlantBase.CanCarryOtherPlant) ? grid.CurrPlantBase : grid.CurrPlantBase.ProtectPlant)));
		}
		ZombieBase zombieBase = null;
		if (isZombieGrove)
		{
			ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(grid.Point.y, ClickedPos, getHyp: false);
			ZombieBase zombieByLineMinDisNoDir2 = ZombieManager.Instance.GetZombieByLineMinDisNoDir(grid.Point.y, ClickedPos, getHyp: true);
			if (zombieByLineMinDisNoDir != null)
			{
				zombieBase = ((!(zombieByLineMinDisNoDir2 != null)) ? zombieByLineMinDisNoDir : ((!(Mathf.Abs(zombieByLineMinDisNoDir.transform.position.x - ClickedPos.x) < Mathf.Abs(zombieByLineMinDisNoDir2.transform.position.x - ClickedPos.x))) ? zombieByLineMinDisNoDir2 : zombieByLineMinDisNoDir));
			}
			else if (zombieByLineMinDisNoDir2 != null)
			{
				zombieBase = zombieByLineMinDisNoDir2;
			}
		}
		if (zombieBase != null && Vector2.Distance(zombieBase.transform.position, ClickedPos) > 1.6f)
		{
			zombieBase = null;
		}
		if (zombieBase != null)
		{
			if (plantBase != null)
			{
				if (Mathf.Abs(zombieBase.transform.position.x - ClickedPos.x) < Mathf.Abs(plantBase.transform.position.x - ClickedPos.x))
				{
					currZombie = zombieBase;
				}
				else
				{
					currPlant = plantBase;
				}
			}
			else
			{
				currZombie = zombieBase;
			}
		}
		else if (plantBase != null)
		{
			currPlant = plantBase;
		}
		if (currPlant != null || currZombie != null)
		{
			return true;
		}
		return false;
	}

	public void SynClient(int id, Vector2 pos)
	{
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(pos);
		PlantBase plantBase = PlantManager.Instance.OnlineGetPlant(id);
		if (plantBase != null)
		{
			if (isPlantGrove && gridByWorldPos != null)
			{
				plantBase.MoveToGrid(gridByWorldPos);
			}
		}
		else if (isZombieGrove)
		{
			ZombieBase zombieBase = ZombieManager.Instance.OnlineGetZombie(id);
			if (zombieBase != null)
			{
				zombieBase.TeleportTo(new Vector2(pos.x, gridByWorldPos.Position.y));
			}
		}
	}

	public void LvStart()
	{
		if (isPlantGrove || isZombieGrove)
		{
			base.transform.localScale = Vector3.one;
			if (isPlantGrove && isZombieGrove)
			{
				gloveImg.GetComponent<Image>().sprite = AllGlove1;
			}
			else if (isPlantGrove)
			{
				gloveImg.GetComponent<Image>().sprite = PlantGlove1;
			}
			else if (isZombieGrove)
			{
				gloveImg.GetComponent<Image>().sprite = ZombieGlove1;
			}
		}
		else
		{
			base.transform.localScale = Vector3.zero;
		}
	}
}
