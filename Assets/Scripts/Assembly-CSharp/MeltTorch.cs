using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MeltTorch : PlantBase
{
	public Collider2D FireCollider;

	public Light2D Light2d;

	public SortingGroup SmokeSort;

	public SortingGroup MeltsSort;

	private List<int> GetPea = new List<int>();

	public override float MaxHp => 1000f;

	public override float Temperature => 1f;

	public override PlantType BasePlant => PlantType.Torchwood;

	public int lineNum => base.currGrid.Point.y;

	protected override void OnInitForAll()
	{
		canFrozen = false;
		Light2d.enabled = true;
		FireCollider.enabled = false;
		SmokeSort.gameObject.SetActive(value: false);
		MeltsSort.gameObject.SetActive(value: false);
	}

	protected override void OnInitForCreate()
	{
		FireCollider.enabled = false;
	}

	protected override void OnInitForPlace()
	{
		SetFireActive(isActive: true);
		SmokeSort.sortingOrder = base.SortingOrder + 1;
		MeltsSort.sortingOrder = base.SortingOrder + 1;
		SmokeSort.gameObject.SetActive(value: true);
		MeltsSort.gameObject.SetActive(value: true);
	}

	protected override void DeadEvent()
	{
		SetFireActive(isActive: false);
	}

	protected override void OnIceEvent()
	{
		SetFireActive(isActive: false);
	}

	private void SetFireActive(bool isActive)
	{
		if (FireCollider.enabled != isActive)
		{
			canFrozen = !isActive;
			Light2d.enabled = isActive;
			FireCollider.enabled = isActive;
			MapManager.Instance.WarmGrid(base.transform.position, base.currGrid.Point, 2, 2, isActive);
			MapManager.Instance.WarmGrid(base.transform.position, base.currGrid.Point, 2, 2, isActive);
			MapManager.Instance.GetCurrMap(base.transform.position).fog.LightFog(base.currGrid.Point, 1, 1, isActive, noCorner: false, 1);
			MapManager.Instance.LightGrid(base.transform.position, base.currGrid.Point, 1, 1, isActive, noCorner: false);
		}
	}

	protected override void OnBurnEvent()
	{
	}

	public int GetMeltValue()
	{
		if (GameManager.Instance.isClient)
		{
			if (GetPea.Count > 0)
			{
				int result = GetPea[0];
				GetPea.RemoveAt(0);
				return result;
			}
			return 6;
		}
		if (GetPea.Count == 0)
		{
			int num = Random.Range(0, 6);
			GetPeaInfo(num);
			ServerSendSyn(1, num);
		}
		if (GetPea.Count > 0)
		{
			int result2 = GetPea[0];
			GetPea.RemoveAt(0);
			return result2;
		}
		return 6;
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 1)
		{
			GetPeaInfo(syn.SynCode[2]);
		}
	}

	private void GetPeaInfo(int type)
	{
		switch (type)
		{
		case 0:
			GetPea = new List<int>
			{
				1, 9, 8, 9, 5, 1, 7, 4, 7, 10,
				5, 6, 10, 10, 1, 5, 6, 5, 9, 5
			};
			break;
		case 1:
			GetPea = new List<int>
			{
				8, 5, 6, 1, 8, 3, 7, 1, 2, 2,
				9, 3, 8, 5, 5, 7, 4, 10, 1, 2
			};
			break;
		case 2:
			GetPea = new List<int>
			{
				2, 4, 6, 5, 9, 3, 1, 5, 9, 1,
				9, 9, 6, 2, 9, 8, 1, 2, 1, 8
			};
			break;
		case 3:
			GetPea = new List<int>
			{
				5, 8, 2, 2, 1, 5, 5, 2, 5, 4,
				2, 6, 1, 5, 4, 7, 3, 10, 7, 8
			};
			break;
		case 4:
			GetPea = new List<int>
			{
				8, 5, 3, 3, 1, 9, 3, 6, 2, 2,
				3, 9, 6, 9, 2, 6, 1, 8, 7, 3
			};
			break;
		case 5:
			GetPea = new List<int>
			{
				3, 8, 2, 9, 3, 6, 4, 6, 9, 3,
				3, 5, 8, 5, 8, 7, 10, 3, 5, 3
			};
			break;
		}
	}
}
