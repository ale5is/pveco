using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class PortalController : MonoBehaviour
{
	public int OnlineId;

	private List<Portal> portals = new List<Portal>();

	public void InitThis(int portalNum, int type)
	{
		if (portalNum < 2)
		{
			portalNum = 2;
		}
		else if (portalNum > 5)
		{
			portalNum = 5;
		}
		for (int i = 0; i < portalNum; i++)
		{
			Portal portal = null;
			portal = type switch
			{
				1 => Object.Instantiate(GameManager.Instance.GameConf.CirclePortal).GetComponent<Portal>(), 
				2 => Object.Instantiate(GameManager.Instance.GameConf.RhombusPortal).GetComponent<Portal>(), 
				_ => Object.Instantiate(GameManager.Instance.GameConf.SquarePortal).GetComponent<Portal>(), 
			};
			Grid randomNoWaterGrid = MapManager.Instance.GetRandomNoWaterGrid(3);
			for (int j = 0; j < portals.Count; j++)
			{
				while (randomNoWaterGrid == portals[j].CurrGrid)
				{
					randomNoWaterGrid = MapManager.Instance.GetRandomNoWaterGrid(3);
				}
			}
			portal.InitThis(randomNoWaterGrid, this);
			portal.transform.SetParent(base.transform);
			portals.Add(portal);
		}
		if (GameManager.Instance.isServer)
		{
			List<Vector2> list = new List<Vector2>();
			for (int k = 0; k < portals.Count; k++)
			{
				list.Add(portals[k].CurrGrid.Position);
			}
			OnlineId = SocketServer.Instance.ItemId;
			PortalSpawn portalSpawn = new PortalSpawn();
			portalSpawn.OnlineId = OnlineId;
			portalSpawn.Pos = list;
			portalSpawn.type = type;
			SocketServer.Instance.SpawnPortal(portalSpawn);
		}
		StartCoroutine(ResetDoor());
	}

	public void ClientInit(PortalSpawn spawn)
	{
		OnlineId = spawn.OnlineId;
		for (int i = 0; i < spawn.Pos.Count; i++)
		{
			Portal portal = null;
			portal = ((spawn.type != 1) ? ((spawn.type != 2) ? Object.Instantiate(GameManager.Instance.GameConf.SquarePortal).GetComponent<Portal>() : Object.Instantiate(GameManager.Instance.GameConf.RhombusPortal).GetComponent<Portal>()) : Object.Instantiate(GameManager.Instance.GameConf.CirclePortal).GetComponent<Portal>());
			Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(spawn.Pos[i]);
			portal.InitThis(gridByWorldPos, this);
			portal.transform.SetParent(base.transform);
			portals.Add(portal);
		}
	}

	private IEnumerator ResetDoor()
	{
		while (true)
		{
			yield return new WaitForSeconds(60f);
			ResetAllPortal();
		}
	}

	private void ResetAllPortal()
	{
		Grid randomNoWaterGrid = MapManager.Instance.GetRandomNoWaterGrid(3);
		for (int i = 0; i < portals.Count; i++)
		{
			if (Random.Range(0, 2) > 0)
			{
				continue;
			}
			for (int j = 0; j < portals.Count; j++)
			{
				while (randomNoWaterGrid == portals[j].CurrGrid)
				{
					randomNoWaterGrid = MapManager.Instance.GetRandomNoWaterGrid(3);
				}
			}
			portals[i].ResetThis(randomNoWaterGrid);
			if (GameManager.Instance.isServer)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Portal;
				synItem.SynCode[0] = i;
				synItem.Twofloat = randomNoWaterGrid.Position;
				SocketServer.Instance.SendSynBag(synItem);
			}
		}
	}

	public void ClientReset(SynItem syn)
	{
		portals[syn.SynCode[0]].ResetThis(MapManager.Instance.GetGridByWorldPos(syn.Twofloat));
	}

	public Portal GetNextPortal(Portal portal)
	{
		if (portals.IndexOf(portal) + 1 >= portals.Count)
		{
			return portals[0];
		}
		return portals[portals.IndexOf(portal) + 1];
	}
}
