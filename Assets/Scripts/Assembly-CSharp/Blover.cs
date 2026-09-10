using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blover : PlantBase
{
	private int loopNum;

	private bool blowZombie;

	public Texture2D SleepEye;

	public SpriteRenderer HeadRenderer;

	public override float MaxHp => 300f;

	protected override void OnInitForPlace()
	{
		loopNum = 0;
		SetAnimChange(11);
	}

	protected override void GoAwakeSpecial()
	{
		SetAnimChange(11);
		HeadRenderer.material.SetTexture("_EyeTex", null);
	}

	protected override void GoSleepSpecial()
	{
		SetAnimChange(13);
		HeadRenderer.material.SetTexture("_EyeTex", SleepEye);
	}

	private IEnumerator BlowZimbieBack(List<ZombieBase> zombies)
	{
		float move = 0.5f;
		if (base.IsFacingLeft)
		{
			move = -0.5f;
		}
		List<Obstacle> obs = MapManager.Instance.GetAllObstacle(base.CurrMap);
		while (blowZombie)
		{
			yield return null;
			if (!(Time.timeScale > 0f))
			{
				continue;
			}
			for (int i = 0; i < zombies.Count; i++)
			{
				if (zombies[i].Hp > 0 && zombies[i].CanBlowBack)
				{
					zombies[i].transform.Translate(new Vector2(1f, 0f) * Time.deltaTime * move);
				}
			}
			for (int j = 0; j < obs.Count; j++)
			{
				if (obs[j] != null && obs[j] is SteelWheel)
				{
					obs[j].GetComponent<SteelWheel>().PushForce(7f * Time.deltaTime, base.IsFacingLeft);
				}
			}
		}
	}

	protected override void DeadEvent()
	{
		blowZombie = false;
	}

	public override void SpecialAnimEvent1()
	{
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.blover, base.transform.position);
		blowZombie = true;
		SkyManager.Instance.AffectWind(base.IsFacingLeft, 5);
		if (!base.IsFacingLeft)
		{
			MapManager.Instance.GetCurrMap(base.transform.position).fog.Blow();
		}
		List<ZombieBase> allZombies = ZombieManager.Instance.GetAllZombies(base.transform.position, isHypno);
		StartCoroutine(BlowZimbieBack(allZombies));
		for (int i = 0; i < allZombies.Count; i++)
		{
			if (allZombies[i] is BalloonZombie)
			{
				allZombies[i].GetComponent<BalloonZombie>().Blow();
			}
		}
	}

	public override void SpecialAnimEvent2()
	{
		if (loopNum < 2)
		{
			loopNum++;
			return;
		}
		blowZombie = false;
		Dead();
	}
}
