using System.Collections.Generic;
using UnityEngine;

public class Jalapeno : PlantBase
{
	public Texture2D SleepEye;

	public SpriteRenderer Body;

	public List<SpriteRenderer> EyeRenderers = new List<SpriteRenderer>();

	public override float MaxHp => 300f;

	protected override int attackValue => 1800;

	protected override void OnInitForAll()
	{
		needFlatDead = false;
		Body.material.SetTexture("_EyeTex", null);
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
		if (base.currGrid == null)
		{
			return;
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Jalapenoboom, base.transform.position);
		base.CurrMap.FadeTempt += 5f;
		List<Iceroad> list = new List<Iceroad>();
		for (int i = 0; i < MapManager.Instance.iceroads.Count; i++)
		{
			MapBase mapBase = MapManager.Instance.GetCurrMap(MapManager.Instance.iceroads[i].transform.position);
			if (!(base.CurrMap != mapBase) && MapManager.Instance.iceroads[i].CurrLine == base.currGrid.Point.y)
			{
				list.Add(MapManager.Instance.iceroads[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].Dead();
		}
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 15f, isHypno, needCapsule: false);
		List<PlantBase> linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.currGrid.Point.y, 15f, !isHypno);
		if (LV.Instance.CurrLVType == LVType.PvP)
		{
			for (int k = 0; k < linePlant.Count; k++)
			{
				linePlant[k].BurnEvent();
				linePlant[k].Hurt(GetAttackValue() / linePlant.Count, Vector2.zero, null);
			}
			for (int l = 0; l < zombiesByLine.Count; l++)
			{
				zombiesByLine[l].BurnEvent();
				zombiesByLine[l].BoomHurt(GetAttackValue() / zombiesByLine.Count);
			}
		}
		else
		{
			for (int m = 0; m < zombiesByLine.Count; m++)
			{
				zombiesByLine[m].BurnEvent();
				zombiesByLine[m].BoomHurt(GetAttackValue());
			}
			for (int n = 0; n < linePlant.Count; n++)
			{
				linePlant[n].BurnEvent();
				linePlant[n].Hurt(GetAttackValue(), Vector2.zero, null);
			}
		}
		zombiesByLine = ZombieManager.Instance.GetZombiesByLine(base.currGrid.Point.y, base.transform.position, 15f, !isHypno, needCapsule: false);
		linePlant = MapManager.Instance.GetLinePlant(base.transform.position, base.currGrid.Point.y, 15f, isHypno);
		for (int num = 0; num < zombiesByLine.Count; num++)
		{
			zombiesByLine[num].BurnEvent();
		}
		for (int num2 = 0; num2 < linePlant.Count; num2++)
		{
			linePlant[num2].BurnEvent();
		}
		List<Grid> lineAllGrid = MapManager.Instance.GetLineAllGrid(base.currGrid.Position, base.currGrid.Point.y);
		for (int num3 = 0; num3 < lineAllGrid.Count; num3++)
		{
			lineAllGrid[num3].ClearLadder();
			if (lineAllGrid[num3].snow != null)
			{
				lineAllGrid[num3].snow.DirctClear(6, synClient: false);
			}
		}
		Object.Instantiate(GameManager.Instance.GameConf.JalapenoBoom).GetComponent<JalapenoBoom>().CreateInit(base.currGrid, GetBulletSortOrder());
		CameraControl.Instance.ShakeCamera(base.transform.position);
	}

	protected override void GoAwakeSpecial()
	{
		SetAnimChange(11);
		Body.material.SetTexture("_EyeTex", null);
		for (int i = 0; i < EyeRenderers.Count; i++)
		{
			EyeRenderers[i].enabled = true;
		}
	}

	protected override void GoSleepSpecial()
	{
		SetAnimChange(12);
		Body.material.SetTexture("_EyeTex", SleepEye);
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
