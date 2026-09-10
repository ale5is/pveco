using System.Collections.Generic;
using UnityEngine;

public class FallHail : MonoBehaviour
{
	public Sprite HailType1;

	public Sprite HailType2;

	public Sprite HailType3;

	private Grid CurrGrid;

	private float Scale;

	private bool CheckUmbrella;

	private bool isHitUmbrella;

	private void Update()
	{
		if (CurrGrid == null)
		{
			return;
		}
		base.transform.position = Vector2.MoveTowards(base.transform.position, CurrGrid.Position, 15f * Time.deltaTime);
		if (!CheckUmbrella && base.transform.position.y - CurrGrid.Position.y < 0.8f)
		{
			List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(CurrGrid, 1);
			for (int i = 0; i < aroundGrid.Count; i++)
			{
				if (!(aroundGrid[i].CurrPlantBase != null))
				{
					continue;
				}
				if (aroundGrid[i].CurrPlantBase is Umbrellaleaf)
				{
					if (aroundGrid[i].CurrPlantBase.GetComponent<Umbrellaleaf>().Block(150f * Scale))
					{
						isHitUmbrella = true;
					}
				}
				else if (aroundGrid[i].CurrPlantBase.CarryPlant is Umbrellaleaf && aroundGrid[i].CurrPlantBase.CarryPlant.GetComponent<Umbrellaleaf>().Block(150f * Scale))
				{
					isHitUmbrella = true;
				}
			}
		}
		if (base.transform.position.y <= CurrGrid.Position.y)
		{
			if (CurrGrid.isNoIceWater)
			{
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(new Vector2(base.transform.position.x, CurrGrid.Position.y), CurrGrid.Point.y);
				if (Random.Range(1, 3) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
				}
			}
			else
			{
				GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BigHailParticle);
				obj.transform.SetParent(base.transform.parent);
				obj.transform.position = CurrGrid.Position;
			}
			if (CurrGrid.CurrPlantBase != null)
			{
				CurrGrid.CurrPlantBase.Ice();
				CurrGrid.CurrPlantBase.Hurt(150f * Scale, Vector2.down, null);
			}
			ZombieBase zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(CurrGrid.Point.y, base.transform.position, getHyp: false);
			if (zombieByLineMinDisNoDir == null)
			{
				zombieByLineMinDisNoDir = ZombieManager.Instance.GetZombieByLineMinDisNoDir(CurrGrid.Point.y, base.transform.position, getHyp: true);
			}
			if (zombieByLineMinDisNoDir != null && Mathf.Abs(base.transform.position.x - zombieByLineMinDisNoDir.transform.position.x) < 0.4f)
			{
				zombieByLineMinDisNoDir.Ice();
				zombieByLineMinDisNoDir.Hurt((int)(300f * Scale), Vector2.down);
			}
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.FallHail, base.gameObject);
		}
		if (isHitUmbrella)
		{
			GameObject obj2 = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BigHailParticle);
			obj2.transform.SetParent(base.transform.parent);
			obj2.transform.position = CurrGrid.Position;
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.FallHail, base.gameObject);
		}
	}

	public void CreateInit(Grid grid, float scale, int type)
	{
		Scale = scale;
		CurrGrid = grid;
		CheckUmbrella = false;
		isHitUmbrella = false;
		base.transform.localScale = new Vector3(0.2f, 0.2f) * scale;
		MapBase nearestMap = MapManager.Instance.GetNearestMap(grid.Position);
		base.transform.SetParent(nearestMap.transform);
		base.transform.position = new Vector3(grid.Position.x, nearestMap.transform.position.y + nearestMap.MapHalfLengthWidth.y);
		switch (type)
		{
		case 1:
			GetComponent<SpriteRenderer>().sprite = HailType1;
			break;
		case 2:
			GetComponent<SpriteRenderer>().sprite = HailType2;
			break;
		default:
			GetComponent<SpriteRenderer>().sprite = HailType3;
			break;
		}
	}
}
