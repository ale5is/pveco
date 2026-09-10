using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Puddle : MonoBehaviour
{
	public int OnlineId;

	public SpriteRenderer spriteRenderer;

	public Sprite Puddle2;

	public Sprite Puddle3;

	private Coroutine HurtCoroutine;

	private List<Grid> GridList = new List<Grid>();

	public void CreateInit(List<Grid> gridList, Vector2 pos, int OnlineID)
	{
		OnlineId = OnlineID;
		base.transform.position = pos;
		Color puddleColor = MapManager.Instance.GetCurrMap(base.transform.position).puddleColor;
		spriteRenderer.color = new Color(puddleColor.r, puddleColor.g, puddleColor.b, 0f);
		GridList = gridList;
		if (GridList.Count == 2)
		{
			spriteRenderer.sprite = Puddle2;
		}
		else if (GridList.Count == 3)
		{
			spriteRenderer.sprite = Puddle3;
		}
		for (int i = 0; i < gridList.Count; i++)
		{
			gridList[i].isHavePuddle = true;
		}
		StartCoroutine(StartDisplay());
	}

	private IEnumerator StartDisplay()
	{
		float a = 0f;
		while (a < 1f)
		{
			a += Time.deltaTime;
			yield return null;
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
		}
		HurtCoroutine = StartCoroutine(HurtPlant());
	}

	private IEnumerator HurtPlant()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			for (int i = 0; i < GridList.Count; i++)
			{
				if (GridList[i].CurrPlantBase != null && !GridList[i].CurrPlantBase.CanCarryOtherPlant && GridList[i].CurrPlantBase.GetPlantType() != PlantType.CobCannon && GridList[i].CurrPlantBase.GetPlantType() != PlantType.MoonTombStone && !GridList[i].CurrPlantBase.CanPlaceOnWater && !GridList[i].CurrPlantBase.IsZombiePlant)
				{
					GridList[i].CurrPlantBase.Hurt(20f, Vector2.zero, null);
				}
			}
		}
	}

	public void StartDisappear()
	{
		if (MapManager.Instance.puddles.Contains(this))
		{
			MapManager.Instance.puddles.Remove(this);
		}
		if (GameManager.Instance.isServer)
		{
			SynItem synItem = new SynItem();
			synItem.OnlineId = OnlineId;
			synItem.Type = SynItemType.Puddle;
			SocketServer.Instance.SendSynBag(synItem);
		}
		StopCoroutine(HurtCoroutine);
		StartCoroutine(Disappear());
	}

	private IEnumerator Disappear()
	{
		float a = 1f;
		spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
		while (a > 0f)
		{
			a -= Time.deltaTime;
			yield return null;
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
		}
		for (int i = 0; i < GridList.Count; i++)
		{
			GridList[i].isHavePuddle = false;
		}
		Destroy();
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
