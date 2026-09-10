using System.Collections;
using UnityEngine;

public class Lilypad : PlantBase
{
	public override float MaxHp => 300f;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => true;

	public override bool CanCarryOtherPlant => true;

	protected override Vector2 offSet => new Vector2(0f, -0.25f);

	protected override bool HaveShadow => false;

	public override bool IsLowPlant => true;

	public override bool CanPlaceOnPuddle => true;

	protected override void OnInitForAll()
	{
		canIce = false;
	}

	protected override void OnInitForAlmanac()
	{
		StartCoroutine(FloatUp());
	}

	protected override void OnInitForPlace()
	{
		StartCoroutine(FloatUp());
	}

	private IEnumerator FloatUp()
	{
		Vector3 pos = base.transform.position;
		while (base.transform.position.y < pos.y + 0.06f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				UpFloat();
			}
		}
		StartCoroutine(FloatDown());
	}

	private IEnumerator FloatDown()
	{
		Vector3 pos = base.transform.position;
		while (base.transform.position.y > pos.y - 0.06f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				DownFloat();
			}
		}
		StartCoroutine(FloatUp());
	}

	private void UpFloat()
	{
		base.transform.position += new Vector3(0f, Time.deltaTime * 0.08f, 0f);
	}

	private void DownFloat()
	{
		base.transform.position += new Vector3(0f, (0f - Time.deltaTime) * 0.08f, 0f);
	}
}
