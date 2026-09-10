using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Plantern : PlantBase
{
	public Texture2D lightOut;

	private bool isLight;

	public Light2D light2d;

	private int X = 3;

	private int Y = 1;

	private EFAudio efAudio;

	public override float MaxHp => 300f;

	protected override void DeadEvent()
	{
		if (isLight)
		{
			SetLight(toLight: false);
		}
	}

	protected override void OnInitForPlace()
	{
		if (!isLight && !isSleeping)
		{
			light2d.enabled = true;
			efAudio = AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.plantern, base.transform.position);
			SetLight(toLight: true);
		}
	}

	protected override void OnInitForCreate()
	{
		light2d.enabled = true;
	}

	protected override void GoAwakeSpecial()
	{
		if (!isLight)
		{
			light2d.enabled = true;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.plantern, base.transform.position);
			SetLight(toLight: true);
		}
	}

	protected override void GoSleepSpecial()
	{
		EyeREnderer.material.SetTexture("_EyeTex", lightOut);
		light2d.enabled = false;
		if (efAudio != null)
		{
			efAudio.Close();
		}
		efAudio = null;
		SetLight(toLight: false);
	}

	private void SetLight(bool toLight)
	{
		if (isLight != toLight)
		{
			base.CurrMap.fog.LightFog(base.currGrid.Point, X, Y, toLight, noCorner: true, 1);
			base.CurrMap.fog.LightFog(base.currGrid.Point, 1, 1, toLight, noCorner: false, 2);
			MapManager.Instance.LightGrid(base.transform.position, base.currGrid.Point, X, Y, toLight, noCorner: true);
			MapManager.Instance.LightGrid(base.transform.position, base.currGrid.Point, 1, 1, toLight, noCorner: false, 2);
			isLight = toLight;
		}
	}
}
