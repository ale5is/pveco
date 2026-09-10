using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
	public ParticleSystem MainLightning;

	public ParticleSystem SubLightning;

	public GameObject MainSpotLight;

	public GameObject AllLight;

	public void LightGrid(Grid grid, PlantBase plant)
	{
		base.transform.position = grid.Position;
		MainSpotLight.transform.position = grid.Position + new Vector2(0f, 11f);
		MainLightning.Play();
		SubLightning.Play();
		AllLight.gameObject.SetActive(value: false);
		switch (Random.Range(0, 3))
		{
		case 0:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder, base.transform.position, isAll: true);
			break;
		case 1:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder1, base.transform.position, isAll: true);
			break;
		case 2:
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Thunder2, base.transform.position, isAll: true);
			break;
		}
		MapBase currMap = MapManager.Instance.GetCurrMap(grid.Position);
		SubLightning.transform.position = new Vector3(grid.Position.x, currMap.transform.position.y + currMap.MapHalfLengthWidth.y);
		StartCoroutine(waitLight(grid, plant));
	}

	private IEnumerator waitLight(Grid grid, PlantBase plant)
	{
		yield return new WaitForSeconds(0.3f);
		AllLight.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(0.3f);
		bool flag = false;
		if (GameManager.Instance.isClient && grid.CurrPlantBase != null)
		{
			if (grid.CurrPlantBase is Clematis || grid.CurrPlantBase is Umbrellaleaf)
			{
				flag = true;
			}
			else if (grid.CurrPlantBase.CarryPlant is Clematis || grid.CurrPlantBase.CarryPlant is Umbrellaleaf)
			{
				flag = true;
			}
		}
		if (!plant && !flag)
		{
			List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(grid, 1);
			for (int i = 0; i < aroundGrid.Count; i++)
			{
				if (aroundGrid[i].CurrPlantBase != null)
				{
					aroundGrid[i].CurrPlantBase.Hurt(300f, Vector2.zero, null);
				}
			}
			List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(grid.Position, 2.6f, getHyp: true, needCapsule: false);
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(600);
			}
			List<ZombieBase> zombies2 = ZombieManager.Instance.GetZombies(grid.Position, 2.6f, getHyp: false, needCapsule: false);
			for (int k = 0; k < zombies2.Count; k++)
			{
				zombies2[k].BoomHurt(600);
			}
			if (zombies.Count > 0 || zombies2.Count > 0)
			{
				if (Random.Range(0, 2) == 0)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bilibili1, base.transform.position);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.bilibili2, base.transform.position);
				}
			}
		}
		else if (plant != null)
		{
			if (plant is Clematis)
			{
				((Clematis)plant).StruckThis();
			}
			else if (plant is Umbrellaleaf)
			{
				((Umbrellaleaf)plant).StruckThis();
			}
		}
		yield return new WaitForSeconds(0.1f);
		base.transform.gameObject.SetActive(value: false);
	}
}
