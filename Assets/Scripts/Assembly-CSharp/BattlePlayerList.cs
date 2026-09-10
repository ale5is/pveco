using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class BattlePlayerList : MonoBehaviour
{
	public static BattlePlayerList Instance;

	public PlayerShow HostShow;

	public PlayerShow Player2Show;

	public PlayerShow Player3Show;

	public PlayerShow Player4Show;

	public List<Animator> ShovelAnims = new List<Animator>();

	public List<Animator> HammerAnims = new List<Animator>();

	private int ShovelAnimNum;

	private int HammerAnimNum;

	private void Awake()
	{
		Instance = this;
	}

	public void LoadAllSeedBank(List<string> players, List<int> cardNum)
	{
		Player2Show.IsPrepare = false;
		Player3Show.IsPrepare = false;
		Player4Show.IsPrepare = false;
		Player2Show.transform.SetAsLastSibling();
		Player3Show.transform.SetAsLastSibling();
		Player4Show.transform.SetAsLastSibling();
		for (int i = 0; i < players.Count; i++)
		{
			if (HostShow.nameText.text == players[i])
			{
				HostShow.SeedBank.SpawnCardSlot(cardNum[i]);
			}
			else if (Player2Show.nameText.text == players[i])
			{
				Player2Show.SeedBank.SpawnCardSlot(cardNum[i]);
			}
			else if (Player3Show.nameText.text == players[i])
			{
				Player3Show.SeedBank.SpawnCardSlot(cardNum[i]);
			}
			else if (Player4Show.nameText.text == players[i])
			{
				Player4Show.SeedBank.SpawnCardSlot(cardNum[i]);
			}
		}
		if (HostShow.transform.localScale.x > 0f && MapManager.Instance.mapList.Count > 0)
		{
			HostShow.MapSprite.sprite = MapManager.Instance.mapList[0].GotoSprite;
		}
		if (Player2Show.transform.localScale.x > 0f && MapManager.Instance.mapList.Count > 0)
		{
			Player2Show.MapSprite.sprite = MapManager.Instance.mapList[0].GotoSprite;
		}
		if (Player3Show.transform.localScale.x > 0f && MapManager.Instance.mapList.Count > 0)
		{
			Player3Show.MapSprite.sprite = MapManager.Instance.mapList[0].GotoSprite;
		}
		if (Player4Show.transform.localScale.x > 0f && MapManager.Instance.mapList.Count > 0)
		{
			Player4Show.MapSprite.sprite = MapManager.Instance.mapList[0].GotoSprite;
		}
	}

	public void PreviewPlant(PlantPreview apply)
	{
		if (apply.PlayerName == GameManager.Instance.LocalPlayerSave.playerName || (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(apply.PlayerName)))
		{
			return;
		}
		PlayerShow playerShow = null;
		if (HostShow.nameText.text == apply.PlayerName)
		{
			playerShow = HostShow;
		}
		else if (Player2Show.nameText.text == apply.PlayerName)
		{
			playerShow = Player2Show;
		}
		else if (Player3Show.nameText.text == apply.PlayerName)
		{
			playerShow = Player3Show;
		}
		else if (Player4Show.nameText.text == apply.PlayerName)
		{
			playerShow = Player4Show;
		}
		if (apply.plantType == PlantType.Nope)
		{
			if (playerShow.plantInGrid != null)
			{
				playerShow.plantInGrid.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
				playerShow.plantInGrid = null;
			}
			playerShow.GridSeletor.gameObject.SetActive(value: false);
		}
		else if (playerShow.plantInGrid == null)
		{
			Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(apply.GridPos);
			if (gridByWorldPos != null)
			{
				playerShow.GridSeletor.gameObject.SetActive(value: true);
				playerShow.GridSeletor.transform.position = gridByWorldPos.Position + new Vector2(-0.5f, 0.3f);
				playerShow.plantInGrid = PlantManager.Instance.GetNewPlant(apply.plantType);
				playerShow.plantInGrid.transform.SetParent(PlantManager.Instance.transform);
				playerShow.plantInGrid.InitForCreate(inGrid: true, gridByWorldPos, apply.isImtor);
			}
		}
		else
		{
			Grid gridByWorldPos2 = MapManager.Instance.GetGridByWorldPos(apply.GridPos);
			if (gridByWorldPos2 != null)
			{
				playerShow.GridSeletor.transform.position = gridByWorldPos2.Position + new Vector2(-0.5f, 0.3f);
				playerShow.plantInGrid.UpdateForCreate(gridByWorldPos2);
			}
		}
	}

	public void PreviewZombie(ZombiePreview apply)
	{
		if (apply.PlayerName == GameManager.Instance.LocalPlayerSave.playerName || (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(apply.PlayerName)))
		{
			return;
		}
		PlayerShow playerShow = null;
		if (HostShow.nameText.text == apply.PlayerName)
		{
			playerShow = HostShow;
		}
		else if (Player2Show.nameText.text == apply.PlayerName)
		{
			playerShow = Player2Show;
		}
		else if (Player3Show.nameText.text == apply.PlayerName)
		{
			playerShow = Player3Show;
		}
		else if (Player4Show.nameText.text == apply.PlayerName)
		{
			playerShow = Player4Show;
		}
		if (apply.zombieType == ZombieType.Nope)
		{
			if (playerShow.zombieInGrid != null)
			{
				playerShow.zombieInGrid.DirectDead(canDropItem: false, 0f, synClient: true);
				playerShow.zombieInGrid = null;
			}
			playerShow.GridSeletor.gameObject.SetActive(value: false);
		}
		else if (playerShow.zombieInGrid == null)
		{
			Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(apply.GridPos);
			if (gridByWorldPos != null)
			{
				playerShow.GridSeletor.gameObject.SetActive(value: true);
				playerShow.GridSeletor.transform.position = gridByWorldPos.Position + new Vector2(-0.5f, 0.3f);
				playerShow.zombieInGrid = ZombieManager.Instance.GetNewZombie(apply.zombieType);
				playerShow.zombieInGrid.transform.SetParent(PlantManager.Instance.transform);
				playerShow.zombieInGrid.CreateInit(inGrid: true, gridByWorldPos, apply.isRat);
			}
		}
		else
		{
			Grid gridByWorldPos2 = MapManager.Instance.GetGridByWorldPos(apply.GridPos);
			if (gridByWorldPos2 != null)
			{
				playerShow.GridSeletor.transform.position = gridByWorldPos2.Position + new Vector2(-0.5f, 0.3f);
				playerShow.zombieInGrid.UpdateForCreate(gridByWorldPos2);
			}
		}
	}

	public void PreviewShovel(string playerName, Vector2 pos, bool isShow)
	{
		if (playerName == GameManager.Instance.LocalPlayerSave.playerName || (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(playerName)))
		{
			return;
		}
		PlayerShow playerShow = null;
		if (HostShow.nameText.text == playerName)
		{
			playerShow = HostShow;
		}
		else if (Player2Show.nameText.text == playerName)
		{
			playerShow = Player2Show;
		}
		else if (Player3Show.nameText.text == playerName)
		{
			playerShow = Player3Show;
		}
		else if (Player4Show.nameText.text == playerName)
		{
			playerShow = Player4Show;
		}
		if (!isShow)
		{
			playerShow.GridSeletor.gameObject.SetActive(value: false);
			return;
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(pos);
		if (gridByWorldPos != null)
		{
			playerShow.GridSeletor.gameObject.SetActive(value: true);
			playerShow.GridSeletor.transform.position = gridByWorldPos.Position + new Vector2(-0.5f, 0.3f);
		}
	}

	public void PlayShovelAnimation(Vector2 GridPos, int Sound, string playerName)
	{
		if (LV.Instance.CurrLVType == LVType.PvP && !PvPSelector.Instance.IsSameTeam(playerName))
		{
			return;
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(GridPos);
		if (gridByWorldPos == null)
		{
			return;
		}
		ShovelAnimNum++;
		if (ShovelAnimNum > ShovelAnims.Count - 1)
		{
			ShovelAnimNum = 0;
		}
		ShovelAnims[ShovelAnimNum].transform.position = gridByWorldPos.Position + new Vector2(0.5f, 0.5f);
		ShovelAnims[ShovelAnimNum].Play("Shovel", 0, 0f);
		switch (Sound)
		{
		case 1:
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant1, ShovelAnims[ShovelAnimNum].transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Plant2, ShovelAnims[ShovelAnimNum].transform.position);
			}
			break;
		case 2:
			if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.shoveling_snow1, ShovelAnims[ShovelAnimNum].transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.shoveling_snow2, ShovelAnims[ShovelAnimNum].transform.position);
			}
			break;
		}
	}

	public void PlayHammerAnimation(Vector2 pos, int type, string playerName)
	{
		if ((LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(playerName)) && MapManager.Instance.GetGridByWorldPos(pos) != null)
		{
			HammerAnimNum++;
			if (HammerAnimNum > HammerAnims.Count - 1)
			{
				HammerAnimNum = 0;
			}
			HammerAnims[HammerAnimNum].transform.position = pos + new Vector2(0.24f, -0.45f);
			switch (type)
			{
			case 1:
				HammerAnims[HammerAnimNum].Play("anim_open_pot", 0, 0f);
				break;
			case 2:
				HammerAnims[HammerAnimNum].Play("anim_whack_zombie", 0, 0f);
				break;
			default:
				HammerAnims[HammerAnimNum].Play("anim_whack_zombie", 0, 0f);
				break;
			}
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.swing, base.transform.position);
		}
	}

	public void SelectCard(string playerName, PlantType type, ZombieType zType, bool noAnim)
	{
		PlayerShow playerShow = null;
		if (HostShow.nameText.text == playerName)
		{
			playerShow = HostShow;
		}
		else if (Player2Show.nameText.text == playerName)
		{
			playerShow = Player2Show;
		}
		else if (Player3Show.nameText.text == playerName)
		{
			playerShow = Player3Show;
		}
		else if (Player4Show.nameText.text == playerName)
		{
			playerShow = Player4Show;
		}
		UIPlantCardNC uIPlantCardNC = null;
		uIPlantCardNC = ((type == PlantType.Nope) ? SeedBank.Instance.GetZombieNc(zType) : SeedBank.Instance.GetPlantNc(type));
		bool flag = true;
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			flag = PvPSelector.Instance.IsSameTeam(playerShow.nameText.text);
			if (flag)
			{
				uIPlantCardNC.IsChoosed = true;
			}
		}
		else
		{
			uIPlantCardNC.IsChoosed = true;
		}
		if (!uIPlantCardNC.IsUnLock)
		{
			flag = false;
		}
		playerShow.SeedBank.ChooseCard(uIPlantCardNC, flag && !noAnim);
	}

	public void CancelCard(string playerName, int cardId)
	{
		PlayerShow playerShow = null;
		if (HostShow.nameText.text == playerName)
		{
			playerShow = HostShow;
		}
		else if (Player2Show.nameText.text == playerName)
		{
			playerShow = Player2Show;
		}
		else if (Player3Show.nameText.text == playerName)
		{
			playerShow = Player3Show;
		}
		else if (Player4Show.nameText.text == playerName)
		{
			playerShow = Player4Show;
		}
		bool needAnim = true;
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			needAnim = PvPSelector.Instance.IsSameTeam(playerShow.nameText.text);
		}
		playerShow.SeedBank.ClearChoose(cardId, needAnim);
	}

	public bool CheckPrepare()
	{
		if (Player2Show.nameText.text != null && Player2Show.nameText.text != "" && !Player2Show.IsPrepare)
		{
			return false;
		}
		if (Player3Show.nameText.text != null && Player3Show.nameText.text != "" && !Player3Show.IsPrepare)
		{
			return false;
		}
		if (Player4Show.nameText.text != null && Player4Show.nameText.text != "" && !Player4Show.IsPrepare)
		{
			return false;
		}
		return true;
	}

	public void UpdateMapSprite(string playerName, Vector2 pos)
	{
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		if (currMap != null)
		{
			Sprite gotoSprite = currMap.GotoSprite;
			if (HostShow.nameText.text == playerName)
			{
				HostShow.MapSprite.sprite = gotoSprite;
			}
			else if (Player2Show.nameText.text == playerName)
			{
				Player2Show.MapSprite.sprite = gotoSprite;
			}
			else if (Player3Show.nameText.text == playerName)
			{
				Player3Show.MapSprite.sprite = gotoSprite;
			}
			else if (Player4Show.nameText.text == playerName)
			{
				Player4Show.MapSprite.sprite = gotoSprite;
			}
		}
	}

	public void UpdateCardCD(string playerName, int cardID, bool isOk)
	{
		if (HostShow.nameText.text == playerName)
		{
			HostShow.SeedBank.UpdateCD(cardID, isOk);
		}
		else if (Player2Show.nameText.text == playerName)
		{
			Player2Show.SeedBank.UpdateCD(cardID, isOk);
		}
		else if (Player3Show.nameText.text == playerName)
		{
			Player3Show.SeedBank.UpdateCD(cardID, isOk);
		}
		else if (Player4Show.nameText.text == playerName)
		{
			Player4Show.SeedBank.UpdateCD(cardID, isOk);
		}
	}

	public void UpdateState(string playerName, bool isPrepare)
	{
		if (Player2Show.nameText.text == playerName)
		{
			Player2Show.IsPrepare = isPrepare;
		}
		else if (Player3Show.nameText.text == playerName)
		{
			Player3Show.IsPrepare = isPrepare;
		}
		else if (Player4Show.nameText.text == playerName)
		{
			Player4Show.IsPrepare = isPrepare;
		}
	}

	public void UpdatePlayerList(PlayerInfo Host, List<PlayerInfo> players)
	{
		if (Host != null)
		{
			HostShow.nameText.text = Host.Name;
			HostShow.GridSeletorName.text = Host.Name;
			HostShow.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			ClearPreview(HostShow);
			HostShow.nameText.text = "";
			HostShow.SeedBank.ClearCardSlot();
			HostShow.GridSeletor.gameObject.SetActive(value: false);
			HostShow.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		if (players == null)
		{
			return;
		}
		if (!Contain(players, Player2Show.nameText.text))
		{
			ClearPreview(Player2Show);
			Player2Show.nameText.text = "";
			Player2Show.transform.SetAsLastSibling();
			Player2Show.SeedBank.ClearCardSlot();
			Player2Show.GridSeletor.gameObject.SetActive(value: false);
			Player2Show.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		if (!Contain(players, Player3Show.nameText.text))
		{
			ClearPreview(Player3Show);
			Player3Show.nameText.text = "";
			Player3Show.transform.SetAsLastSibling();
			Player3Show.SeedBank.ClearCardSlot();
			Player3Show.GridSeletor.gameObject.SetActive(value: false);
			Player3Show.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		if (!Contain(players, Player4Show.nameText.text))
		{
			ClearPreview(Player4Show);
			Player4Show.nameText.text = "";
			Player4Show.transform.SetAsLastSibling();
			Player4Show.SeedBank.ClearCardSlot();
			Player4Show.GridSeletor.gameObject.SetActive(value: false);
			Player4Show.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		for (int i = 0; i < players.Count; i++)
		{
			if (!Contain2(players[i].Name))
			{
				if (Player2Show.nameText.text == "")
				{
					Player2Show.nameText.text = players[i].Name;
					Player2Show.GridSeletorName.text = players[i].Name;
					Player2Show.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				else if (Player3Show.nameText.text == "")
				{
					Player3Show.nameText.text = players[i].Name;
					Player3Show.GridSeletorName.text = players[i].Name;
					Player3Show.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				else if (Player4Show.nameText.text == "")
				{
					Player4Show.nameText.text = players[i].Name;
					Player4Show.GridSeletorName.text = players[i].Name;
					Player4Show.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
	}

	private bool Contain(List<PlayerInfo> players, string name)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].Name == name)
			{
				return true;
			}
		}
		return false;
	}

	private bool Contain2(string name)
	{
		if (Player2Show.nameText.text == name)
		{
			return true;
		}
		if (Player3Show.nameText.text == name)
		{
			return true;
		}
		if (Player4Show.nameText.text == name)
		{
			return true;
		}
		return false;
	}

	private void ClearPreview(PlayerShow show)
	{
		if (show.plantInGrid != null)
		{
			show.plantInGrid.Dead(isFlat: false, 0f, synClient: true, deadRattle: false);
			show.plantInGrid = null;
		}
		show.GridSeletor.gameObject.SetActive(value: false);
	}
}
