using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torchwood : PlantBase
{
	public Collider2D FireCollider;

	public Sprite Fire;

	public Sprite NoFire;

	public Light2D Light2d;

	public List<SpriteRenderer> fireRederers;

	public override float MaxHp => 300f;

	public override float Temperature => 1f;

	public int lineNum => base.currGrid.Point.y;

	protected override void OnInitForAll()
	{
		canFrozen = false;
		Light2d.enabled = true;
		FireCollider.enabled = false;
		for (int i = 0; i < fireRederers.Count; i++)
		{
			fireRederers[i].enabled = true;
		}
	}

	protected override void OnInitForCreate()
	{
		FireCollider.enabled = false;
	}

	protected override void OnInitForPlace()
	{
		SetFireActive(isActive: true);
	}

	protected override void GoAwakeSpecial()
	{
		SetFireActive(isActive: true);
	}

	protected override void GoSleepSpecial()
	{
		SetFireActive(isActive: false);
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
			for (int i = 0; i < fireRederers.Count; i++)
			{
				fireRederers[i].enabled = isActive;
			}
			MapManager.Instance.WarmGrid(base.transform.position, base.currGrid.Point, 1, 1, isActive);
			MapManager.Instance.WarmGrid(base.transform.position, base.currGrid.Point, 2, 2, isActive);
			MapManager.Instance.GetCurrMap(base.transform.position).fog.LightFog(base.currGrid.Point, 1, 1, isActive, noCorner: true, 1);
			MapManager.Instance.LightGrid(base.transform.position, base.currGrid.Point, 1, 1, isActive, noCorner: true);
		}
	}

	protected override void OnBurnEvent()
	{
		GoAwake();
	}
}
