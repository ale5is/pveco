using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
	public static ConveyorBelt Instance;

	public Transform Belt;

	public List<Transform> BeltCards = new List<Transform>();

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		Vector3 localPosition = Vector3.MoveTowards(Belt.localPosition, new Vector3(-1f, -0.68f), 0.5f * Time.deltaTime);
		float x = localPosition.x - Belt.localPosition.x;
		Belt.localPosition = localPosition;
		if (Belt.localPosition.x == -1f)
		{
			Belt.localPosition = new Vector3(1f, -0.68f);
		}
		for (int i = 0; i < BeltCards.Count; i++)
		{
			float num = -2.22f + 0.5f * (float)i;
			if (BeltCards[i].localPosition.x > num)
			{
				BeltCards[i].localPosition += new Vector3(x, 0f);
				if (BeltCards[i].localPosition.x < num)
				{
					BeltCards[i].localPosition = new Vector3(num, BeltCards[i].localPosition.y);
				}
			}
		}
	}

	public void AddCard(PlantCard card)
	{
		if (!BeltCards.Contains(card.transform))
		{
			BeltCards.Add(card.transform);
			card.transform.SetParent(base.transform);
			card.transform.localPosition = new Vector3(2.8f, 0f);
		}
	}

	public void RemoveCard(PlantCard card)
	{
		BeltCards.Remove(card.transform);
	}

	private IEnumerator BeltCardSpawner()
	{
		int pp = 10 - MapManager.Instance.mapList.Count * 2;
		if (pp < 4)
		{
			pp = 4;
		}
		while (true)
		{
			List<CardType> cards = new List<CardType>(LV.Instance.GeneralCardPool);
			cards.AddRange(LV.Instance.GeneralCardPool);
			int num = cards.Count / 5;
			for (int i = 0; i < num; i++)
			{
				cards.Add(LV.Instance.GeneralCardPool[Random.Range(0, LV.Instance.GeneralCardPool.Count)]);
			}
			cards.Shuffle();
			for (int j = 0; j < cards.Count; j++)
			{
				int num2 = Random.Range(2, pp) + LV.Instance.BeltTimeAdd;
				yield return new WaitForSeconds(num2);
				if (BeltCards.Count < 10 && !LVManager.Instance.BootyIsAppeared)
				{
					SeedBank.Instance.SpawnBeltCard(cards[j].plantType, cards[j].zombieType);
				}
				else
				{
					j--;
				}
			}
		}
	}

	public void StartMove()
	{
		base.transform.localScale = new Vector3(1.6f, 1.6f);
		StartCoroutine(DoMove(5f));
		if (!GameManager.Instance.isClient)
		{
			StartCoroutine(BeltCardSpawner());
		}
	}

	public void StartMoveBack(bool fade)
	{
		StopAllCoroutines();
		if (fade)
		{
			StartCoroutine(DoMove(6.8f));
			return;
		}
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 6.8f, base.transform.localPosition.z);
		base.transform.localScale = Vector3.zero;
	}

	private IEnumerator DoMove(float targetPosY)
	{
		while (base.transform.localPosition.y != targetPosY)
		{
			yield return null;
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, new Vector3(base.transform.localPosition.x, targetPosY, base.transform.localPosition.z), Time.deltaTime * 5f);
		}
	}
}
