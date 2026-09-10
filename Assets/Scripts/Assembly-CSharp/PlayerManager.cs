using System;
using System.Collections.Generic;
using SocketSave;
using StartScene;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	private UnityAction SunNumUpdateAction;

	public static PlayerManager Instance;

	public bool IsDebug;

	public List<Allcoin> Coins = new List<Allcoin>();

	private float sunNum;

	private float moonNum;

	private float sunNumTeam;

	private float moonNumTeam;

	private bool sunInfinite;

	private int money;

	public bool GetSunUp;

	public bool NormalSunUp;

	private bool enableCreate;

	private float SunNum
	{
		get
		{
			return sunNum;
		}
		set
		{
			if (!GameManager.Instance.isClient && !SunInfinite)
			{
				sunNum = value;
				if (sunNum != 1000000f && sunNum > 10000f)
				{
					sunNum = 10000f;
				}
				if (sunNum < 0f)
				{
					sunNum = 0f;
				}
				if ((LV.Instance.CurrLVType == LVType.PvP && PvPSelector.Instance.LocalIsRedTeam) || LV.Instance.CurrLVType != LVType.PvP)
				{
					SeedBank.Instance.UpdateSunNum(sunNum.ToString());
				}
				if (SunNumUpdateAction != null)
				{
					SunNumUpdateAction();
				}
				if (GameManager.Instance.isServer)
				{
					SunNumBag sunNumBag = new SunNumBag();
					sunNumBag.sunNum = (int)sunNum;
					sunNumBag.isSun = true;
					sunNumBag.isBlTeam = false;
					SocketServer.Instance.UpdateSunNum(sunNumBag);
				}
			}
		}
	}

	private float MoonNum
	{
		get
		{
			return moonNum;
		}
		set
		{
			if (!GameManager.Instance.isClient && !SunInfinite)
			{
				moonNum = value;
				if (moonNum != 1000000f && moonNum > 10000f)
				{
					moonNum = 10000f;
				}
				if (moonNum < 0f)
				{
					moonNum = 0f;
				}
				if ((LV.Instance.CurrLVType == LVType.PvP && PvPSelector.Instance.LocalIsRedTeam) || LV.Instance.CurrLVType != LVType.PvP)
				{
					SeedBank.Instance.UpdateMoonNum(moonNum.ToString());
				}
				if (SunNumUpdateAction != null)
				{
					SunNumUpdateAction();
				}
				if (GameManager.Instance.isServer)
				{
					SunNumBag sunNumBag = new SunNumBag();
					sunNumBag.sunNum = (int)moonNum;
					sunNumBag.isSun = false;
					sunNumBag.isBlTeam = false;
					SocketServer.Instance.UpdateSunNum(sunNumBag);
				}
			}
		}
	}

	private float SunNumTeam
	{
		get
		{
			return sunNumTeam;
		}
		set
		{
			if (!GameManager.Instance.isClient && !SunInfinite)
			{
				sunNumTeam = value;
				if (sunNumTeam != 1000000f && sunNumTeam > 10000f)
				{
					sunNumTeam = 10000f;
				}
				if (sunNumTeam < 0f)
				{
					sunNumTeam = 0f;
				}
				if (LV.Instance.CurrLVType == LVType.PvP && PvPSelector.Instance.LocalIsBlueTeam)
				{
					SeedBank.Instance.UpdateSunNum(sunNumTeam.ToString());
				}
				if (SunNumUpdateAction != null)
				{
					SunNumUpdateAction();
				}
				if (GameManager.Instance.isServer)
				{
					SunNumBag sunNumBag = new SunNumBag();
					sunNumBag.sunNum = (int)sunNumTeam;
					sunNumBag.isSun = true;
					sunNumBag.isBlTeam = true;
					SocketServer.Instance.UpdateSunNum(sunNumBag);
				}
			}
		}
	}

	private float MoonNumTeam
	{
		get
		{
			return moonNumTeam;
		}
		set
		{
			if (!GameManager.Instance.isClient && !SunInfinite)
			{
				moonNumTeam = value;
				if (moonNumTeam != 1000000f && moonNumTeam > 10000f)
				{
					moonNumTeam = 10000f;
				}
				if (moonNumTeam < 0f)
				{
					moonNumTeam = 0f;
				}
				if (LV.Instance.CurrLVType == LVType.PvP && PvPSelector.Instance.LocalIsBlueTeam)
				{
					SeedBank.Instance.UpdateMoonNum(moonNumTeam.ToString());
				}
				if (SunNumUpdateAction != null)
				{
					SunNumUpdateAction();
				}
				if (GameManager.Instance.isServer)
				{
					SunNumBag sunNumBag = new SunNumBag();
					sunNumBag.sunNum = (int)moonNumTeam;
					sunNumBag.isSun = false;
					sunNumBag.isBlTeam = true;
					SocketServer.Instance.UpdateSunNum(sunNumBag);
				}
			}
		}
	}

	public int Money
	{
		get
		{
			return money;
		}
		set
		{
			if (value > money)
			{
				StatsManager.Instance.AddStatsNum(StatsEnum.EarnMoney, value - money);
			}
			SetMoney(value);
		}
	}

	public bool SunInfinite
	{
		get
		{
			return sunInfinite;
		}
		set
		{
			if (value == sunInfinite)
			{
				return;
			}
			if (value)
			{
				if (GameManager.Instance.isClient)
				{
					sunNum = 1000000f;
					moonNum = 1000000f;
					sunNumTeam = 1000000f;
					moonNumTeam = 1000000f;
				}
				else
				{
					SunNum = 1000000f;
					MoonNum = 1000000f;
					SunNumTeam = 1000000f;
					MoonNumTeam = 1000000f;
				}
				SeedBank.Instance.UpdateSunNum("无限");
				SeedBank.Instance.UpdateMoonNum("无限");
				sunInfinite = value;
			}
			else
			{
				sunInfinite = value;
				SunNum = 10000f;
				MoonNum = 10000f;
				SunNumTeam = 10000f;
				MoonNumTeam = 10000f;
			}
		}
	}

	public bool EnableCreate
	{
		get
		{
			return enableCreate;
		}
		set
		{
			enableCreate = value;
			Glove.Instance.LvStart();
			CreatePanel.Instance.EnableCreateUpdate();
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void SetMoney(int money2)
	{
		money = money2;
		if (money > 9999999)
		{
			money = 9999999;
		}
		if (money < 0)
		{
			money = 0;
		}
		Coinbank.Instance.UpdateCoinbankNum(money);
		StoreScence.Instance.UpdateMoneyBank(money);
	}

	public void ReadSave(int money2)
	{
		SetMoney(money2);
	}

	public float GetSunNum(bool isSun, string Player)
	{
		float num = ((Player == null) ? ((!isSun) ? MoonNum : SunNum) : ((LV.Instance.CurrLVType == LVType.PvP) ? (PvPSelector.Instance.BlueTeamNames.Contains(Player) ? ((!isSun) ? MoonNumTeam : SunNumTeam) : ((!isSun) ? MoonNum : SunNum)) : ((!isSun) ? MoonNum : SunNum)));
		if (num > 10000f)
		{
			num = 10000f;
		}
		return num;
	}

	public void AddSunNum(float Value, bool isSun, string Player)
	{
		if (GetSunUp && Value > 0f)
		{
			Value *= 2f;
		}
		if (Player == null)
		{
			if (isSun)
			{
				SunNum += Value;
			}
			else
			{
				MoonNum += Value;
			}
		}
		else if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (PvPSelector.Instance.RedTeamNames.Contains(Player))
			{
				if (isSun)
				{
					SunNum += Value;
				}
				else
				{
					MoonNum += Value;
				}
			}
			else if (PvPSelector.Instance.BlueTeamNames.Contains(Player))
			{
				if (isSun)
				{
					SunNumTeam += Value;
				}
				else
				{
					MoonNumTeam += Value;
				}
			}
		}
		else if (isSun)
		{
			SunNum += Value;
		}
		else
		{
			MoonNum += Value;
		}
	}

	public void SetSunNum(float Value, bool isSun, string Player)
	{
		if (Player == null)
		{
			if (isSun)
			{
				SunNum = Value;
			}
			else
			{
				MoonNum = Value;
			}
		}
		else if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (PvPSelector.Instance.RedTeamNames.Contains(Player))
			{
				if (isSun)
				{
					SunNum = Value;
				}
				else
				{
					MoonNum = Value;
				}
			}
			else if (PvPSelector.Instance.BlueTeamNames.Contains(Player))
			{
				if (isSun)
				{
					SunNumTeam = Value;
				}
				else
				{
					MoonNumTeam = Value;
				}
			}
		}
		else if (isSun)
		{
			SunNum = Value;
		}
		else
		{
			MoonNum = Value;
		}
	}

	public void ClientUpdateSunNum(SunNumBag bag)
	{
		if (bag.sunNum == 1000000)
		{
			if (!SunInfinite)
			{
				SunInfinite = true;
			}
			return;
		}
		if (SunInfinite)
		{
			SunInfinite = false;
		}
		if (bag.isSun)
		{
			if (LV.Instance.CurrLVType == LVType.PvP)
			{
				if (PvPSelector.Instance.LocalIsRedTeam && !bag.isBlTeam)
				{
					sunNum = bag.sunNum;
					SeedBank.Instance.UpdateSunNum(sunNum.ToString());
				}
				else if (PvPSelector.Instance.LocalIsBlueTeam && bag.isBlTeam)
				{
					sunNumTeam = bag.sunNum;
					SeedBank.Instance.UpdateSunNum(sunNumTeam.ToString());
				}
			}
			else
			{
				sunNum = bag.sunNum;
				SeedBank.Instance.UpdateSunNum(sunNum.ToString());
			}
		}
		else if (LV.Instance.CurrLVType == LVType.PvP)
		{
			if (PvPSelector.Instance.LocalIsRedTeam && !bag.isBlTeam)
			{
				moonNum = bag.sunNum;
				SeedBank.Instance.UpdateMoonNum(moonNum.ToString());
			}
			else if (PvPSelector.Instance.LocalIsBlueTeam && bag.isBlTeam)
			{
				moonNumTeam = bag.sunNum;
				SeedBank.Instance.UpdateMoonNum(moonNumTeam.ToString());
			}
		}
		else
		{
			moonNum = bag.sunNum;
			SeedBank.Instance.UpdateMoonNum(moonNum.ToString());
		}
		if (SunNumUpdateAction != null)
		{
			SunNumUpdateAction();
		}
	}

	public void ClearCardUpdateEvent()
	{
		SunNumUpdateAction = null;
	}

	public void ResetSunNum()
	{
		int num = LV.Instance.LvNormalSunNum;
		if (NormalSunUp)
		{
			int num2 = num / 25;
			if (num2 < 2)
			{
				num2 = 2;
			}
			num += num2 / 2 * 25;
		}
		SunNum = num;
		MoonNum = num;
		SunNumTeam = num;
		MoonNumTeam = num;
	}

	public void ClearMoney()
	{
		List<Allcoin> list = new List<Allcoin>(Coins);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Destroy();
		}
	}

	public void AddSunNumUpdateActionListener(UnityAction action)
	{
		SunNumUpdateAction = (UnityAction)Delegate.Combine(SunNumUpdateAction, action);
	}

	public void LvReset()
	{
		SunNumUpdateAction = null;
	}
}
