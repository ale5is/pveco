using UnityEngine;

public class ArmDropEF : MonoBehaviour
{
	public Transform Rotation;

	public SpriteRenderer MidArmRE;

	public SpriteRenderer LowArmRE;

	private float timer;

	private Grid GoalGrid;

	private float RotationSpeed;

	private float HorizontalSpeed;

	private bool DirctDisappear;

	private void Update()
	{
		if (GoalGrid == null)
		{
			return;
		}
		if (DirctDisappear)
		{
			if (MidArmRE.color.a > 0f)
			{
				MidArmRE.color = new Color(MidArmRE.color.r, MidArmRE.color.g, MidArmRE.color.b, MidArmRE.color.a - Time.deltaTime * 4f);
				LowArmRE.color = MidArmRE.color;
			}
			else
			{
				PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ArmDropEF, base.gameObject);
			}
		}
		if (DirctDisappear || base.transform.position.y > GoalGrid.Position.y - 0.4f)
		{
			timer += Time.deltaTime;
			base.transform.Translate(new Vector3(HorizontalSpeed, (0f - timer) * 10f) * Time.deltaTime);
			Rotation.transform.Rotate(new Vector3(0f, 0f, (0f - RotationSpeed) * Time.deltaTime));
		}
		else if (MidArmRE.color.a > 0f)
		{
			MidArmRE.color = new Color(MidArmRE.color.r, MidArmRE.color.g, MidArmRE.color.b, MidArmRE.color.a - Time.deltaTime * 2f);
			LowArmRE.color = MidArmRE.color;
		}
		else
		{
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ArmDropEF, base.gameObject);
		}
	}

	public void CreateInit(int sort, Grid grid, SpriteRenderer midArmRE, SpriteRenderer lowArmRE, Vector3 scale, bool FacingLeft)
	{
		timer = 0f;
		GoalGrid = grid;
		Rotation.rotation = new Quaternion(0f, 0f, 0f, 0f);
		Rotation.transform.localScale = scale;
		DirctDisappear = GoalGrid.isNoIceWater;
		RotationSpeed = Random.Range(100, 200);
		HorizontalSpeed = Random.Range(0.5f, 1f);
		if (!FacingLeft)
		{
			HorizontalSpeed = 0f - HorizontalSpeed;
		}
		if (lowArmRE != null)
		{
			base.transform.position = lowArmRE.transform.position;
		}
		else if (midArmRE != null)
		{
			base.transform.position = midArmRE.transform.position;
		}
		if (midArmRE != null)
		{
			MidArmRE.transform.position = midArmRE.transform.position;
			MidArmRE.transform.rotation = midArmRE.transform.rotation;
			MidArmRE.transform.localScale = midArmRE.transform.localScale;
			MidArmRE.sprite = midArmRE.sprite;
			MidArmRE.color = midArmRE.color;
			MidArmRE.sortingOrder = sort + 3;
		}
		else
		{
			MidArmRE.transform.localScale = Vector3.zero;
		}
		if (lowArmRE != null)
		{
			LowArmRE.transform.position = lowArmRE.transform.position;
			LowArmRE.transform.rotation = lowArmRE.transform.rotation;
			LowArmRE.transform.localScale = lowArmRE.transform.localScale;
			LowArmRE.sprite = lowArmRE.sprite;
			LowArmRE.color = lowArmRE.color;
			LowArmRE.sortingOrder = sort + 2;
		}
		else
		{
			LowArmRE.transform.localScale = Vector3.zero;
		}
	}
}
