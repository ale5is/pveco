using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class CobCannon : PlantBase
{
	private Grid nextGrid;

	private Vector2 TargetPos;

	private bool CanCharge;

	private bool ChargeOver;

	private string ShootPlayer;

	public override float MaxHp => 600f;

	public override bool isHaveSpecialCheck => true;

	public override int BasePlantSunNum => 100;

	public override bool CanProtect => false;

	protected override int attackValue => 1800;

	protected override Vector2 offSet => new Vector2(0.5f, 0f);

	protected override List<string> DontPlayAnim => new List<string> { "anim_idle", "anim_unarmed_idle" };

	public override PlantType BasePlant => PlantType.Cornpult;

	protected override void OnInitForCreate()
	{
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
	}

	protected override void OnInitForPlace()
	{
		ShootPlayer = "";
		ChargeOver = false;
		CanCharge = true;
		if (!GameManager.Instance.isClient)
		{
			StartActionCD(35f, isOver: true);
		}
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		MapManager.Instance.PlantnoFlash(BasePlant);
	}

	public override void SpCheckInitPlace()
	{
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
		{
			nextGrid = MapManager.Instance.GetNextGrid(base.currGrid);
		}
		else
		{
			nextGrid = MapManager.Instance.GetNextGrid(base.currGrid, isRight: true);
		}
		int num = 0;
		int num2 = 0;
		if (base.currGrid.CurrPlantBase != null && base.currGrid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			if (base.currGrid.CurrPlantBase.isSleeping)
			{
				num2++;
			}
			base.currGrid.CurrPlantBase.Dead();
			base.currGrid.CurrPlantBase = this;
		}
		else if (base.currGrid.CurrPlantBase == null)
		{
			base.currGrid.CurrPlantBase = this;
			num += 100;
		}
		else if (base.currGrid.CurrPlantBase.CarryPlant != null && base.currGrid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			if (base.currGrid.CurrPlantBase.CarryPlant.isSleeping)
			{
				num2++;
			}
			base.currGrid.CurrPlantBase.CarryPlant.Dead();
			base.transform.SetParent(base.currGrid.CurrPlantBase.transform);
			base.currGrid.CurrPlantBase.CarryPlant = this;
		}
		else if (base.currGrid.CurrPlantBase.CarryPlant == null)
		{
			base.transform.SetParent(base.currGrid.CurrPlantBase.transform);
			base.currGrid.CurrPlantBase.CarryPlant = this;
			num += 100;
		}
		if (nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			if (nextGrid.CurrPlantBase.isSleeping)
			{
				num2++;
			}
			nextGrid.CurrPlantBase.Dead();
			nextGrid.CurrPlantBase = this;
		}
		else if (nextGrid.CurrPlantBase == null)
		{
			nextGrid.CurrPlantBase = this;
			num += 100;
		}
		else if (nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			if (nextGrid.CurrPlantBase.CarryPlant.isSleeping)
			{
				num2++;
			}
			nextGrid.CurrPlantBase.CarryPlant.Dead();
			base.transform.SetParent(nextGrid.CurrPlantBase.transform);
			nextGrid.CurrPlantBase.CarryPlant = this;
		}
		else if (nextGrid.CurrPlantBase.CarryPlant == null)
		{
			base.transform.SetParent(nextGrid.CurrPlantBase.transform);
			nextGrid.CurrPlantBase.CarryPlant = this;
			num += 100;
		}
		if (num != 0)
		{
			num += SeedBank.Instance.noBasePlantExtra;
		}
		PlayerManager.Instance.AddSunNum(-num, isSun: true, PlacePlayer);
	}

	public override bool SpecialPlantCheck(Grid grid, int NeedSun, string Player)
	{
		Grid grid2 = ((LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(Player)) ? MapManager.Instance.GetNextGrid(grid, isRight: true) : MapManager.Instance.GetNextGrid(grid));
		if (grid2 == null)
		{
			return false;
		}
		if (LV.Instance.CurrLVType == LVType.PvP && grid2.CurrGridType != TeamType.Normal)
		{
			bool flag = PvPSelector.Instance.RedTeamNames.Contains(Player);
			if (flag && grid2.CurrGridType == TeamType.BlueTeam)
			{
				return false;
			}
			if (!flag && grid2.CurrGridType == TeamType.RedTeam)
			{
				return false;
			}
		}
		int num = 0;
		if (grid.isOccupied || grid2.isOccupied)
		{
			return false;
		}
		if (grid.HaveGraveStone || grid2.HaveGraveStone)
		{
			return false;
		}
		if (grid.HaveCrater || grid2.HaveCrater)
		{
			return false;
		}
		if ((grid.isWaterGrid && grid.CurrPlantBase == null) || (grid2.isWaterGrid && grid2.CurrPlantBase == null))
		{
			return false;
		}
		if (grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase.GetPlantType() != BasePlant && !grid.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (grid.CurrPlantBase.ProtectPlant != null)
			{
				return false;
			}
			if (grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (grid2.CurrPlantBase != null)
		{
			if (grid2.CurrPlantBase.GetPlantType() != BasePlant && !grid2.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (grid2.CurrPlantBase.ProtectPlant != null)
			{
				return false;
			}
			if (grid2.CurrPlantBase.CarryPlant != null && grid2.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (grid.CurrPlantBase != null && grid2.CurrPlantBase != null && grid.CurrPlantBase.GetPlantType() != grid2.CurrPlantBase.GetPlantType() && grid.CurrPlantBase.CanCarryOtherPlant && grid2.CurrPlantBase.CanCarryOtherPlant)
		{
			return false;
		}
		if (grid.CurrPlantBase == null && grid2.CurrPlantBase != null)
		{
			if (grid2.CurrPlantBase.GetPlantType() != BasePlant && !grid2.isWaterGrid)
			{
				return false;
			}
			if (grid2.isWaterGrid && !grid2.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (grid2.CurrPlantBase.CanCarryOtherPlant && grid2.CurrPlantBase.CarryPlant != null && grid2.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (grid2.CurrPlantBase == null && grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase.GetPlantType() != BasePlant && !grid.isWaterGrid)
			{
				return false;
			}
			if (grid.isWaterGrid && !grid.CurrPlantBase.CanCarryOtherPlant)
			{
				return false;
			}
			if (grid.CurrPlantBase.CanCarryOtherPlant && grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() != BasePlant)
			{
				return false;
			}
		}
		if (grid.CurrPlantBase != null && grid.CurrPlantBase.GetPlantType() == BasePlant)
		{
			num++;
		}
		else if (grid.CurrPlantBase != null && grid.CurrPlantBase.CarryPlant != null && grid.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			num++;
		}
		if (grid2.CurrPlantBase != null && grid2.CurrPlantBase.GetPlantType() == BasePlant)
		{
			num++;
		}
		else if (grid2.CurrPlantBase != null && grid2.CurrPlantBase.CarryPlant != null && grid2.CurrPlantBase.CarryPlant.GetPlantType() == BasePlant)
		{
			num++;
		}
		int num2 = NeedSun + 200 - num * 100;
		if (num != 2)
		{
			num2 += SeedBank.Instance.noBasePlantExtra;
		}
		if (PlayerManager.Instance.GetSunNum(isSun: true, Player) < (float)num2)
		{
			return false;
		}
		return true;
	}

	protected override bool DoAction()
	{
		if (!ChargeOver)
		{
			if (!CanCharge)
			{
				CanCharge = true;
				return true;
			}
			ChargeOver = true;
			CanCharge = false;
			SetAnimChange(11);
			ServerSendSyn(1);
		}
		return false;
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.coblaunch, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		CreateCannon();
		CanCharge = false;
		ChargeOver = false;
	}

	public override void SpecialAnimEvent3()
	{
		base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.shoop, base.transform.position);
	}

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && !SpectatorList.Instance.LocalIsSpectator && (LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(PlacePlayer)) && base.currGrid != null && Input.GetMouseButtonDown(0) && !isSleeping && LVManager.Instance.GameIsStart)
		{
			CobCannonTarget.Instance.StartAim(this, (Vector2 pos) =>
			{
				ShootCannon(pos);
			});
		}
	}

	private void CreateCannon()
	{
		if (!NormalDontAttackCondition())
		{
			CannonCob component = Object.Instantiate(GameManager.Instance.GameConf.CannonCob).GetComponent<CannonCob>();
			Vector3 vector = new Vector3(-0.6f, 3f);
			if (base.IsFacingLeft)
			{
				vector = MyTool.ReverseX(vector);
			}
			component.CreateInit(base.transform.position + vector, TargetPos, GetAttackValue(), isHypno, ShootPlayer);
		}
	}

	public void ShootCannon(Vector2 pos, bool synClient = false)
	{
		ShootPlayer = GameManager.Instance.LocalPlayerSave.playerName;
		if (GameManager.Instance.isClient && !synClient)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Plant;
			synItem.Twofloat = pos;
			synItem.AName = GameManager.Instance.LocalPlayerSave.playerName;
			synItem.SynCode[0] = 1;
			SocketClient.Instance.SendSynBag(synItem);
			CanCharge = true;
			base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		}
		else
		{
			ServerSendSyn(0, pos, 0, 0, ShootPlayer);
			SetAnimChange(12);
			base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
			TargetPos = pos;
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				if (GameManager.Instance.isClient && !PvPSelector.Instance.IsSameTeam(GameManager.Instance.HostName))
				{
					syn.Twofloat = MyTool.ReverseX(syn.Twofloat);
				}
				if (GameManager.Instance.isServer && !PvPSelector.Instance.IsSameTeam(PlacePlayer))
				{
					syn.Twofloat = MyTool.ReverseX(syn.Twofloat);
				}
			}
			ShootPlayer = syn.AName;
			ShootCannon(syn.Twofloat, synClient: true);
		}
		else if (syn.SynCode[1] == 1)
		{
			SetAnimChange(11);
		}
	}

	protected override void GoAwakeSpecial()
	{
		if (CanCharge)
		{
			base.transform.GetComponent<CapsuleCollider2D>().enabled = true;
		}
	}

	protected override void GoSleepSpecial()
	{
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
	}

	protected override void OnClearGrid()
	{
		if (nextGrid != null)
		{
			if (nextGrid.CurrPlantBase == this)
			{
				nextGrid.CurrPlantBase = null;
			}
			else if (nextGrid.CurrPlantBase != null && nextGrid.CurrPlantBase.CarryPlant != null && nextGrid.CurrPlantBase.CarryPlant == this)
			{
				nextGrid.CurrPlantBase.CarryPlant = null;
			}
		}
	}
}
