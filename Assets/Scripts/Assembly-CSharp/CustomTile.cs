using System.Collections.Generic;
using UnityEngine;

public class CustomTile : MonoBehaviour
{
	public SpriteRenderer UpBorder;

	public SpriteRenderer DownBorder;

	public SpriteRenderer LeftBorder;

	public SpriteRenderer RightBorder;

	public List<Sprite> GridSprites = new List<Sprite>();

	public List<Sprite> StoneGridSprites = new List<Sprite>();

	public SpriteMask WaterMask;

	public Grid CurrGrid;

	private TileType Type;

	private List<CustomTile> AroundTiles;

	public void SetType(TileType type, Grid grid)
	{
		Type = type;
		CurrGrid = grid;
		grid.customTile = this;
		if (AroundTiles == null)
		{
			AroundTiles = MapManager.Instance.GetCurrMap(CurrGrid.Position).GetAroundTile(CurrGrid.Point);
		}
		RefreshState();
		for (int i = 0; i < AroundTiles.Count; i++)
		{
			AroundTiles[i].RefreshState();
		}
		grid.isEmpty = false;
		WaterMask.enabled = false;
		grid.isWaterGrid = false;
		grid.isHardGrid = false;
		UpBorder.sortingOrder = 5;
		DownBorder.sortingOrder = 5;
		LeftBorder.sortingOrder = 6;
		RightBorder.sortingOrder = 6;
		switch (type)
		{
		case TileType.Nope:
			grid.isEmpty = true;
			UpBorder.sprite = null;
			DownBorder.sprite = null;
			LeftBorder.sprite = null;
			RightBorder.sprite = null;
			base.transform.GetComponent<SpriteRenderer>().sprite = null;
			break;
		case TileType.Grass:
			UpBorder.sortingOrder = 3;
			DownBorder.sortingOrder = 3;
			LeftBorder.sortingOrder = 4;
			RightBorder.sortingOrder = 4;
			if ((grid.Point.x + grid.Point.y) % 2 == 0)
			{
				UpBorder.sprite = GridSprites[1];
				DownBorder.sprite = GridSprites[2];
				LeftBorder.sprite = GridSprites[3];
				RightBorder.sprite = GridSprites[4];
				base.transform.GetComponent<SpriteRenderer>().sprite = GridSprites[0];
			}
			else
			{
				UpBorder.sprite = GridSprites[6];
				DownBorder.sprite = GridSprites[7];
				LeftBorder.sprite = GridSprites[8];
				RightBorder.sprite = GridSprites[9];
				base.transform.GetComponent<SpriteRenderer>().sprite = GridSprites[5];
			}
			break;
		case TileType.Water:
			WaterMask.enabled = true;
			grid.isWaterGrid = true;
			UpBorder.sprite = GridSprites[11];
			DownBorder.sprite = GridSprites[12];
			LeftBorder.sprite = GridSprites[13];
			RightBorder.sprite = GridSprites[14];
			base.transform.GetComponent<SpriteRenderer>().sprite = GridSprites[10];
			break;
		case TileType.Stone:
			grid.isHardGrid = true;
			UpBorder.sprite = null;
			DownBorder.sprite = null;
			LeftBorder.sprite = null;
			RightBorder.sprite = null;
			if ((grid.Point.x + grid.Point.y) % 2 == 0)
			{
				base.transform.GetComponent<SpriteRenderer>().sprite = StoneGridSprites[0];
			}
			else
			{
				base.transform.GetComponent<SpriteRenderer>().sprite = StoneGridSprites[1];
			}
			break;
		}
	}

	public void RefreshState()
	{
		if (AroundTiles.Count < 8)
		{
			AroundTiles = MapManager.Instance.GetCurrMap(CurrGrid.Position).GetAroundTile(CurrGrid.Point);
		}
		for (int i = 0; i < AroundTiles.Count; i++)
		{
			SpriteRenderer spriteRenderer = null;
			if (AroundTiles[i].CurrGrid.Point == CurrGrid.Point + new Vector2Int(0, -1))
			{
				spriteRenderer = UpBorder;
			}
			else if (AroundTiles[i].CurrGrid.Point == CurrGrid.Point + new Vector2Int(0, 1))
			{
				spriteRenderer = DownBorder;
			}
			else if (AroundTiles[i].CurrGrid.Point == CurrGrid.Point + new Vector2Int(-1, 0))
			{
				spriteRenderer = LeftBorder;
			}
			else if (AroundTiles[i].CurrGrid.Point == CurrGrid.Point + new Vector2Int(1, 0))
			{
				spriteRenderer = RightBorder;
			}
			if (spriteRenderer != null)
			{
				if (AroundTiles[i].Type == TileType.Water && Type == TileType.Grass)
				{
					spriteRenderer.enabled = false;
				}
				else if (AroundTiles[i].Type == TileType.Stone && Type == TileType.Grass)
				{
					spriteRenderer.enabled = false;
				}
				else if (AroundTiles[i].Type == Type)
				{
					spriteRenderer.enabled = false;
				}
				else
				{
					spriteRenderer.enabled = true;
				}
			}
		}
	}

	public void SetColor(Color color)
	{
		UpBorder.color = color;
		DownBorder.color = color;
		LeftBorder.color = color;
		RightBorder.color = color;
		base.transform.GetComponent<SpriteRenderer>().color = color;
	}

	public TileType GetTileType()
	{
		return Type;
	}
}
