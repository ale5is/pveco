using UnityEngine;

public class EquipDropEF : MonoBehaviour
{
	public SpriteRenderer EquipRE;

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
			if (EquipRE.color.a > 0f)
			{
				EquipRE.color = new Color(EquipRE.color.r, EquipRE.color.g, EquipRE.color.b, EquipRE.color.a - Time.deltaTime * 4f);
			}
			else
			{
				PoolManager.Instance.PushObj(GameManager.Instance.GameConf.EquipDropEF, base.gameObject);
			}
		}
		if (DirctDisappear || base.transform.position.y > GoalGrid.Position.y - 0.2f)
		{
			timer += Time.deltaTime;
			base.transform.Translate(new Vector3(HorizontalSpeed, (0f - timer) * 15f) * Time.deltaTime);
			EquipRE.transform.Rotate(new Vector3(0f, 0f, (0f - RotationSpeed) * Time.deltaTime));
		}
		else if (EquipRE.color.a > 0f)
		{
			EquipRE.color = new Color(EquipRE.color.r, EquipRE.color.g, EquipRE.color.b, EquipRE.color.a - Time.deltaTime * 2f);
		}
		else
		{
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.EquipDropEF, base.gameObject);
		}
	}

	public void CreateInit(int sort, Grid grid, SpriteRenderer equipRE, Vector3 scale, bool FacingLeft)
	{
		timer = 0f;
		GoalGrid = grid;
		DirctDisappear = GoalGrid.isNoIceWater;
		RotationSpeed = Random.Range(100, 200);
		HorizontalSpeed = Random.Range(1.5f, 3f);
		if (!FacingLeft)
		{
			HorizontalSpeed = 0f - HorizontalSpeed;
		}
		base.transform.position = equipRE.transform.position;
		base.transform.localScale = scale;
		EquipRE.transform.rotation = equipRE.transform.rotation;
		EquipRE.transform.localScale = equipRE.transform.localScale;
		EquipRE.sprite = equipRE.sprite;
		EquipRE.color = equipRE.color;
		EquipRE.sortingOrder = sort + 1;
	}
}
