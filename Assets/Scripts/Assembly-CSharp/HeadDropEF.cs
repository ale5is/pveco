using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HeadDropEF : MonoBehaviour
{
	public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	public Transform Rotation;

	private Grid GoalGrid;

	private float RotationSpeed;

	private float HorizontalSpeed;

	private float NormalUpSpeed;

	private bool DirctDisappear;

	private int JumpNum;

	private void Update()
	{
		if (GoalGrid == null)
		{
			return;
		}
		if (DirctDisappear)
		{
			if (renderers[0].color.a > 0f)
			{
				for (int i = 0; i < renderers.Count; i++)
				{
					renderers[i].color = new Color(renderers[i].color.r, renderers[i].color.g, renderers[i].color.b, renderers[i].color.a - Time.deltaTime * 4f);
				}
			}
			else
			{
				PoolManager.Instance.PushObj(GameManager.Instance.GameConf.HeadDropEF, base.gameObject);
			}
		}
		if (DirctDisappear || base.transform.position.y > GoalGrid.Position.y - 0.4f)
		{
			NormalUpSpeed -= Time.deltaTime * 10f;
			base.transform.Translate(new Vector3(HorizontalSpeed, NormalUpSpeed) * Time.deltaTime);
			Rotation.transform.Rotate(new Vector3(0f, 0f, (0f - RotationSpeed) * Time.deltaTime));
		}
		else if (JumpNum <= 0)
		{
			if (renderers[0].color.a > 0f)
			{
				for (int j = 0; j < renderers.Count; j++)
				{
					renderers[j].color = new Color(renderers[j].color.r, renderers[j].color.g, renderers[j].color.b, renderers[j].color.a - Time.deltaTime * 2f);
				}
			}
			else
			{
				PoolManager.Instance.PushObj(GameManager.Instance.GameConf.HeadDropEF, base.gameObject);
			}
		}
		else if (NormalUpSpeed <= 0f)
		{
			JumpNum--;
			NormalUpSpeed = 1.5f * (float)JumpNum;
		}
		else
		{
			NormalUpSpeed -= Time.deltaTime * 10f;
			base.transform.Translate(new Vector3(HorizontalSpeed, NormalUpSpeed) * Time.deltaTime);
		}
	}

	public void CreateInit(int sort, Grid grid, SpriteRenderer headRe, List<SpriteRenderer> sprites, Vector3 scale, bool FacingLeft)
	{
		GoalGrid = grid;
		Rotation.rotation = new Quaternion(0f, 0f, 0f, 0f);
		DirctDisappear = GoalGrid.isNoIceWater;
		RotationSpeed = Random.Range(100, 200);
		HorizontalSpeed = Random.Range(0.5f, 1f);
		if (!FacingLeft)
		{
			HorizontalSpeed = 0f - HorizontalSpeed;
		}
		NormalUpSpeed = Random.Range(2f, 3f);
		JumpNum = Random.Range(2, 4);
		Rotation.GetComponent<SortingGroup>().sortingOrder = sort + 1;
		Rotation.transform.localScale = scale;
		if (headRe != null)
		{
			base.transform.position = headRe.transform.position;
		}
		if (renderers.Count < sprites.Count)
		{
			int num = sprites.Count - renderers.Count;
			for (int i = 0; i < num; i++)
			{
				renderers.Add(new GameObject("Head").AddComponent<SpriteRenderer>());
				renderers[renderers.Count - 1].transform.SetParent(Rotation);
				renderers[renderers.Count - 1].sortingLayerName = "main";
			}
		}
		for (int j = 0; j < renderers.Count; j++)
		{
			if (j < sprites.Count)
			{
				renderers[j].transform.position = sprites[j].transform.position;
				renderers[j].transform.rotation = sprites[j].transform.rotation;
				renderers[j].transform.localScale = sprites[j].transform.localScale;
				renderers[j].sprite = sprites[j].sprite;
				renderers[j].color = sprites[j].color;
				renderers[j].sortingOrder = sprites[j].sortingOrder;
				renderers[j].enabled = sprites[j].enabled;
			}
			else
			{
				renderers[j].enabled = false;
			}
		}
	}
}
