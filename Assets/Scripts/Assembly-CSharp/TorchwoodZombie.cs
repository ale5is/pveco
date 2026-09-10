using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchwoodZombie : PlantZombie
{
	public Collider2D FireCollider;

	public Light2D Light2d;

	public List<SpriteRenderer> fireRederers = new List<SpriteRenderer>();

	protected override GameObject Prefab => GameManager.Instance.GameConf.TorchwoodZombie;

	protected override Vector2 SpeedRange => new Vector2(2f, 2.8f);

	protected override void PlantZombieInit()
	{
		canButter = false;
		canFrozen = false;
		SetFireActive(isActive: true);
		HammerDoorHpState = new List<int> { 640, 270 };
		DoorHpState = new List<int> { 1100, 760, 360, 0 };
		DoorHpStateSprite = new List<Sprite> { door1, door2, door3, null };
		ArmorHpState = new List<int> { 500, 250, 0 };
		ArmorHpStateSprite = new List<Sprite> { armor1, armor2, null };
	}

	protected override void OnIceEvent()
	{
		SetFireActive(isActive: false);
	}

	protected override void OnBurnEvent()
	{
		SetFireActive(isActive: true);
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
		}
	}
}
