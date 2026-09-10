using System.Collections;
using SocketSave;
using UnityEngine;

public class Snow : MonoBehaviour
{
	private int snowLvl;

	public SpriteRenderer BigSnow;

	public SpriteRenderer LowSnow;

	private bool SnowOpened;

	private bool LowSnowOpen;

	private Grid grid;

	private int nextLvlTime;

	private Coroutine BigSnowCoroutine;

	private Coroutine LowSnowCoroutine;

	private bool StopSnow;

	public int SnowLvl
	{
		get
		{
			return snowLvl;
		}
		set
		{
			if (value > 6 || value < 0 || (grid.HotNum > 1 && value > snowLvl))
			{
				return;
			}
			snowLvl = value;
			if (snowLvl == 0)
			{
				SnowOpened = false;
				if (BigSnowCoroutine != null)
				{
					StopCoroutine(BigSnowCoroutine);
				}
				BigSnowCoroutine = StartCoroutine(SnowDisplay(needTime: false));
			}
			if (!SnowOpened && snowLvl > 0)
			{
				SnowOpened = true;
				if (BigSnowCoroutine != null)
				{
					StopCoroutine(BigSnowCoroutine);
				}
				BigSnowCoroutine = StartCoroutine(SnowDisplay(needTime: false));
			}
			if (!LowSnowOpen && snowLvl > 3)
			{
				LowSnowOpen = true;
				if (LowSnowCoroutine != null)
				{
					StopCoroutine(LowSnowCoroutine);
				}
				LowSnowCoroutine = StartCoroutine(LowSnowDisplay());
			}
			if (LowSnowOpen && snowLvl <= 3)
			{
				LowSnowOpen = false;
				if (LowSnowCoroutine != null)
				{
					StopCoroutine(LowSnowCoroutine);
				}
				LowSnowCoroutine = StartCoroutine(LowSnowDisplay());
			}
		}
	}

	public void CreateInit(Grid grid, bool fade)
	{
		this.grid = grid;
		SnowLvl = 2;
		LowSnow.sortingOrder = grid.Point.y * 200 + 195;
		LowSnow.color = new Color(1f, 1f, 1f, 0f);
		SnowOpen(fade);
	}

	public void SnowOpen(bool fade)
	{
		StopSnow = false;
		if (grid.HotNum > 1)
		{
			BigSnow.color = new Color(1f, 1f, 1f, 0f);
			return;
		}
		SnowOpened = true;
		nextLvlTime = 60;
		if (fade)
		{
			BigSnow.color = new Color(1f, 1f, 1f, 0f);
			StartCoroutine(SnowDisplay(needTime: false));
		}
	}

	public void ChangeColor(Color color)
	{
		BigSnow.color = new Color(color.r, color.g, color.b, BigSnow.color.a);
		LowSnow.color = new Color(color.r, color.g, color.b, LowSnow.color.a);
	}

	public void SnowOver()
	{
		SnowLvl = 0;
		StopSnow = true;
	}

	public void TimeChange()
	{
		if (StopSnow)
		{
			nextLvlTime = 999;
			return;
		}
		if (grid.HotNum > 1)
		{
			if (SnowOpened)
			{
				nextLvlTime += grid.HotNum * 4;
				if (nextLvlTime >= 140)
				{
					SnowLvl--;
					if (SnowLvl > 0)
					{
						nextLvlTime = 0;
					}
					else
					{
						nextLvlTime = 100;
					}
				}
			}
		}
		else if (SnowLvl < 6)
		{
			nextLvlTime -= SkyManager.Instance.SnowScale;
			if (nextLvlTime <= 0)
			{
				SnowLvl++;
				nextLvlTime = 140;
			}
		}
		if (SnowLvl > 0 && grid.HotNum <= 1 && grid.CurrPlantBase != null)
		{
			int num = SnowLvl;
			if (num < 5)
			{
				num = 5;
			}
			if (!GameManager.Instance.isClient && Random.Range(0, 40) < num && grid.CurrPlantBase != null)
			{
				grid.CurrPlantBase.Frozen(Vector2.up, isAudio: false);
			}
			if (grid.CurrPlantBase.FrozenLevel > 6 && grid.CurrPlantBase.SnowHurt)
			{
				grid.CurrPlantBase.Hurt(2 + SnowLvl, Vector2.up, null);
			}
		}
	}

	public void DirctClear(int lvl, bool synClient)
	{
		if (GameManager.Instance.isClient && !synClient)
		{
			return;
		}
		snowLvl -= lvl;
		if (snowLvl < 0)
		{
			snowLvl = 0;
		}
		if (snowLvl == 0)
		{
			if (BigSnowCoroutine != null)
			{
				StopCoroutine(BigSnowCoroutine);
			}
			SnowOpened = false;
			BigSnow.color = new Color(BigSnow.color.r, BigSnow.color.g, BigSnow.color.b, 0f);
		}
		if (LowSnowOpen && snowLvl <= 3)
		{
			if (LowSnowCoroutine != null)
			{
				StopCoroutine(LowSnowCoroutine);
			}
			LowSnowOpen = false;
			LowSnow.color = new Color(LowSnow.color.r, LowSnow.color.g, LowSnow.color.b, 0f);
		}
		nextLvlTime = 140;
		if (GameManager.Instance.isServer && !synClient)
		{
			SynGrid synGrid = new SynGrid();
			synGrid.GridPos = grid.Position;
			synGrid.SynCode[0] = 1;
			synGrid.SynCode[1] = lvl;
			SocketServer.Instance.SendGridState(synGrid);
		}
	}

	private IEnumerator SnowDisplay(bool needTime)
	{
		if (SnowOpened)
		{
			if (needTime)
			{
				yield return new WaitForSeconds(30 - SkyManager.Instance.SnowScale * 2);
			}
			float a = BigSnow.color.a;
			while (a < 1f)
			{
				a += Time.deltaTime;
				yield return null;
				BigSnow.color = new Color(BigSnow.color.r, BigSnow.color.g, BigSnow.color.b, a);
			}
		}
		else
		{
			if (needTime)
			{
				yield return new WaitForSeconds(30f);
			}
			float a = BigSnow.color.a;
			while (a > 0f)
			{
				a -= Time.deltaTime;
				yield return null;
				BigSnow.color = new Color(BigSnow.color.r, BigSnow.color.g, BigSnow.color.b, a);
			}
		}
		BigSnowCoroutine = null;
	}

	private IEnumerator LowSnowDisplay()
	{
		if (LowSnowOpen)
		{
			float a = LowSnow.color.a;
			while (a < 1f)
			{
				a += Time.deltaTime;
				yield return null;
				LowSnow.color = new Color(LowSnow.color.r, LowSnow.color.g, LowSnow.color.b, a);
			}
		}
		else
		{
			float a = LowSnow.color.a;
			while (a > 0f)
			{
				a -= Time.deltaTime;
				yield return null;
				LowSnow.color = new Color(LowSnow.color.r, LowSnow.color.g, LowSnow.color.b, a);
			}
		}
		LowSnowCoroutine = null;
	}
}
