using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
	private static PoolManager instance;

	private GameObject poolObj;

	public Dictionary<GameObject, List<GameObject>> poolDataDic = new Dictionary<GameObject, List<GameObject>>();

	public static PoolManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new PoolManager();
			}
			return instance;
		}
	}

	public GameObject GetObj(GameObject prefab)
	{
		GameObject gameObject = null;
		if (poolDataDic.ContainsKey(prefab) && poolDataDic[prefab].Count > 0)
		{
			gameObject = poolDataDic[prefab][0];
			poolDataDic[prefab].RemoveAt(0);
		}
		else
		{
			gameObject = Object.Instantiate(prefab);
			gameObject.name = prefab.name;
		}
		if (gameObject == null)
		{
			Debug.Log(prefab.name);
		}
		gameObject.SetActive(value: true);
		gameObject.transform.SetParent(null);
		return gameObject;
	}

	public void PushObj(GameObject prefab, GameObject obj)
	{
		if (poolObj == null)
		{
			poolObj = new GameObject("PoolObj");
		}
		if (poolDataDic.ContainsKey(prefab))
		{
			if (!poolDataDic[prefab].Contains(obj))
			{
				poolDataDic[prefab].Add(obj);
			}
		}
		else
		{
			poolDataDic.Add(prefab, new List<GameObject> { obj });
		}
		if (!poolObj.transform.Find(prefab.name))
		{
			new GameObject(prefab.name).transform.SetParent(poolObj.transform);
		}
		obj.SetActive(value: false);
		obj.transform.SetParent(poolObj.transform.Find(prefab.name));
	}

	public void ClearPool()
	{
		foreach (KeyValuePair<GameObject, List<GameObject>> item in poolDataDic)
		{
			foreach (GameObject item2 in item.Value)
			{
				Object.Destroy(item2);
			}
			item.Value.Clear();
		}
		poolDataDic.Clear();
		if (poolObj != null)
		{
			Object.Destroy(poolObj);
			poolObj = null;
		}
	}
}
