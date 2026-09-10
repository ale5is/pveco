using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCob : MonoBehaviour
{
	public string OwnerPlayer;

	private float UpLine;

	private Vector2 targetPos;

	private SpriteRenderer BlastMark;

	private int attackValue;

	private bool isHypno;

	private Grid TargetGrid;

	public void CreateInit(Vector2 pos, Vector2 target, int attackvalue, bool isHyp, string player)
	{
		OwnerPlayer = player;
		isHypno = isHyp;
		attackValue = attackvalue;
		base.transform.GetComponent<SpriteRenderer>().enabled = true;
		BlastMark = base.transform.Find("BlastMark").GetComponent<SpriteRenderer>();
		BlastMark.enabled = false;
		TargetGrid = MapManager.Instance.GetGridByWorldPos(target);
		BlastMark.sortingOrder = FixedInfo.GetBaseSort(TargetGrid.Point.y) + 1;
		MapBase nearestMap = MapManager.Instance.GetNearestMap(pos);
		UpLine = nearestMap.transform.position.y + nearestMap.MapHalfLengthWidth.y;
		base.transform.position = pos;
		targetPos = target;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		base.transform.SetParent(nearestMap.transform);
		StartCoroutine(MoveUp());
	}

	private IEnumerator MoveUp()
	{
		while (base.transform.position.y < UpLine + 5f)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(1f, 0f) * Time.deltaTime / 0.1f);
		}
		StartCoroutine(MoveDown());
	}

	private IEnumerator MoveDown()
	{
		base.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
		MapBase currMap = MapManager.Instance.GetCurrMap(targetPos);
		float num = currMap.transform.position.y + currMap.MapHalfLengthWidth.y;
		base.transform.position = new Vector3(targetPos.x, num + 5f);
		float seconds = 2f;
		if (currMap == MapManager.Instance.GetCurrMap(targetPos))
		{
			seconds = 0.5f;
		}
		yield return new WaitForSeconds(seconds);
		while (base.transform.position.y > targetPos.y)
		{
			yield return new WaitForFixedUpdate();
			base.transform.Translate(new Vector2(1f, 0f) * Time.deltaTime * 10f);
		}
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, isHypno, needCapsule: false);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(attackValue / aroundPlant.Count, Vector2.zero, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(attackValue / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(attackValue);
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(attackValue, Vector2.zero, null);
			}
		}
		if (Vector2.Distance(TargetGrid.Position, base.transform.position) < 3f)
		{
			List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(TargetGrid, 1);
			for (int m = 0; m < aroundGrid.Count; m++)
			{
				aroundGrid[m].ClearLadder();
				if (aroundGrid[m].snow != null)
				{
					aroundGrid[m].snow.DirctClear(6, synClient: false);
				}
			}
		}
		List<Obstacle> aroundObstacle = MapManager.Instance.GetAroundObstacle(base.transform.position, 2.6f);
		for (int n = 0; n < aroundObstacle.Count; n++)
		{
			aroundObstacle[n].HurtThis(attackValue);
			if (aroundObstacle[n] is SteelWheel)
			{
				aroundObstacle[n].GetComponent<SteelWheel>().PushForce(5f, base.transform.position);
			}
		}
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PopcornParticle).transform.position = base.transform.position;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Doomm, base.transform.position);
		base.transform.GetComponent<SpriteRenderer>().enabled = false;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		BlastMark.enabled = true;
		CameraControl.Instance.ShakeCamera(base.transform.position);
		if (!GameManager.Instance.isClient)
		{
			int num2 = 0;
			for (int num3 = 0; num3 < zombies.Count; num3++)
			{
				if (zombies[num3].GetDead() && ZombieManager.Instance.IsGargantuar(zombies[num3].zombieType))
				{
					num2++;
				}
			}
			if (num2 >= 4)
			{
				AcvmentManager.Instance.GetAchievement(Acvname.Popcorn4, OwnerPlayer);
			}
		}
		yield return new WaitForSeconds(1f);
		Object.Destroy(base.gameObject);
	}
}
