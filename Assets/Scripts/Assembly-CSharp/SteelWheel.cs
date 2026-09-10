using UnityEngine;
using UnityEngine.Rendering;

public class SteelWheel : Obstacle
{
	public Transform Wheel2;

	private float ForceValue;

	private Grid CurrGrid;

	private int CurrLine;

	private void Update()
	{
		if (base.transform.position.x > 8.6f || MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			DestroyThis();
		}
		if (ForceValue == 0f)
		{
			return;
		}
		CurrGrid = MapManager.Instance.GetGridByWorldPos(CurrMap, base.transform.position, CurrLine);
		if (Mathf.Abs(CurrGrid.Position.x - base.transform.position.x) < 0.7f)
		{
			Grid upperGrid = MapManager.Instance.GetUpperGrid(CurrGrid);
			if (CurrGrid != null && CurrGrid.CurrPlantBase != null)
			{
				CurrGrid.CurrPlantBase.Hurt(1800f, Vector2.zero, null, isFlat: true);
			}
			if (upperGrid != null && upperGrid.CurrPlantBase != null)
			{
				upperGrid.CurrPlantBase.Hurt(1800f, Vector2.zero, null, isFlat: true);
			}
		}
		if (!(CurrMap != null) || !(Mathf.Abs(CurrMap.EndLine - base.transform.position.x) > 0.5f))
		{
			return;
		}
		Wheel2.Rotate(new Vector3(0f, 0f, (0f - ForceValue) * Time.deltaTime * 20f));
		base.transform.position += new Vector3(ForceValue * Time.deltaTime, 0f);
		if (ForceValue > 0f)
		{
			ForceValue -= Time.deltaTime * 6f;
			if (ForceValue < 0f)
			{
				ForceValue = 0f;
			}
		}
		else
		{
			ForceValue += Time.deltaTime * 6f;
			if (ForceValue > 0f)
			{
				ForceValue = 0f;
			}
		}
	}

	public void CreateInit(Transform wheel, Transform wheel2, int Line, int sort)
	{
		CurrLine = Line;
		InLines.Add(Line);
		InLines.Add(Line - 1);
		collider2d.enabled = true;
		base.transform.position = wheel.position;
		Wheel2.position = wheel2.position;
		Wheel2.rotation = wheel2.rotation;
		base.transform.GetComponent<SortingGroup>().sortingOrder = sort - 1;
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		CurrGrid = MapManager.Instance.GetGridByWorldPos(CurrMap, base.transform.position, Line);
	}

	public void PushForce(float force, bool isfacingLeft)
	{
		force = Mathf.Abs(force);
		if (isfacingLeft)
		{
			ForceValue -= force;
		}
		else
		{
			ForceValue += force;
		}
	}

	public void PushForce(float force, Vector3 forcePos)
	{
		force = Mathf.Abs(force);
		if (forcePos.x > base.transform.position.x)
		{
			ForceValue -= force;
		}
		else
		{
			ForceValue += force;
		}
	}

	public override void HurtThis(float hurt)
	{
		if (Random.Range(0, 2) == 0)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit1, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ShieldHit2, base.transform.position);
		}
	}
}
