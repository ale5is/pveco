using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Cherry : PlantBase
{
	public Texture2D SleepEye1;

	public Texture2D SleepEye2;

	public SpriteRenderer Head1;

	public SpriteRenderer Head2;

	public List<SpriteRenderer> EyeRenderers = new List<SpriteRenderer>();

	public override float MaxHp => 300f;

	protected override int attackValue => 1800;

	protected override void OnInitForAll()
	{
		needFlatDead = false;
		Head1.material.SetTexture("_EyeTex", null);
		Head2.material.SetTexture("_EyeTex", null);
	}

	protected override void OnInitForPlace()
	{
		SetAnimChange(11);
	}

	protected override void DeadrattleEvent()
	{
		Boom();
	}

	private void Boom()
	{
		base.CurrMap.FadeTempt += 5f;
		GameObject obj = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.CherryParticle);
		obj.transform.position = base.transform.position;
		obj.GetComponent<SortingGroup>().sortingOrder = GetBulletSortOrder(1);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CherryBoom, base.transform.position);
		List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(base.transform.position, 2.6f, isHypno, needCapsule: false);
		List<PlantBase> aroundPlant = MapManager.Instance.GetAroundPlant(base.transform.position, 2.6f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int i = 0; i < aroundPlant.Count; i++)
			{
				aroundPlant[i].Hurt(GetAttackValue() / aroundPlant.Count, Vector2.zero, null);
			}
			for (int j = 0; j < zombies.Count; j++)
			{
				zombies[j].BoomHurt(GetAttackValue() / zombies.Count);
			}
		}
		else
		{
			for (int k = 0; k < zombies.Count; k++)
			{
				zombies[k].BoomHurt(GetAttackValue());
			}
			for (int l = 0; l < aroundPlant.Count; l++)
			{
				aroundPlant[l].Hurt(GetAttackValue(), Vector2.zero, null);
			}
		}
		List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(base.currGrid, 1);
		for (int m = 0; m < aroundGrid.Count; m++)
		{
			aroundGrid[m].ClearLadder();
			if (aroundGrid[m].snow != null)
			{
				aroundGrid[m].snow.DirctClear(6, synClient: false);
			}
		}
		List<Obstacle> aroundObstacle = MapManager.Instance.GetAroundObstacle(base.transform.position, 2.6f);
		for (int n = 0; n < aroundObstacle.Count; n++)
		{
			aroundObstacle[n].HurtThis(GetAttackValue());
			if (aroundObstacle[n] is SteelWheel)
			{
				aroundObstacle[n].GetComponent<SteelWheel>().PushForce(5f, base.transform.position);
			}
		}
		CameraControl.Instance.ShakeCamera(base.transform.position);
		if (GameManager.Instance.isClient)
		{
			return;
		}
		int num = 0;
		for (int num2 = 0; num2 < zombies.Count; num2++)
		{
			if (zombies[num2].GetDead() && zombies[num2].BodyScale == 1f)
			{
				num++;
			}
		}
		if (num >= 25)
		{
			AcvmentManager.Instance.GetAchievement(Acvname.Cherry25, PlacePlayer);
		}
	}

	protected override void GoAwakeSpecial()
	{
		SetAnimChange(11);
		Head1.material.SetTexture("_EyeTex", null);
		Head2.material.SetTexture("_EyeTex", null);
		for (int i = 0; i < EyeRenderers.Count; i++)
		{
			EyeRenderers[i].enabled = true;
		}
	}

	protected override void GoSleepSpecial()
	{
		SetAnimChange(12);
		Head1.material.SetTexture("_EyeTex", SleepEye1);
		Head2.material.SetTexture("_EyeTex", SleepEye2);
		for (int i = 0; i < EyeRenderers.Count; i++)
		{
			EyeRenderers[i].enabled = false;
		}
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.reverse_explosion, base.transform.position);
	}

	public override void SpecialAnimEvent2()
	{
		Dead(isFlat: false, 0f, synClient: true);
	}
}
