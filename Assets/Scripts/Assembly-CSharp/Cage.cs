using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Cage : Obstacle
{
	public Sprite Cage1;

	public Sprite Cage2;

	public Sprite Cage3;

	public SortingGroup Sorting;

	public SpriteRenderer FrontCage;

	public SpriteRenderer BackCage;

	private float Hp;

	private float MaxHp = 1800f;

	private int state1;

	private int state2;

	private Grid CurrGrid;

	public void PlaceInit(Grid grid)
	{
		Hp = MaxHp;
		CurrGrid = grid;
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
		InLines.Add(grid.Point.y);
		collider2d.enabled = true;
		Sorting.sortingOrder = FixedInfo.GetBaseSort(grid.Point.y) + FixedInfo.CageFront;
		BackCage.sortingOrder = FixedInfo.GetBaseSort(grid.Point.y) + FixedInfo.CageBack;
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		base.transform.SetParent(CurrMap.transform);
		if (grid.Cage != null)
		{
			grid.Cage.DestroyThis();
		}
		grid.Cage = this;
		base.transform.position = grid.Position + new Vector2(0f, 0.5f);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.cageplace, base.transform.position);
	}

	public void SetSort(int sort)
	{
		Sorting.sortingOrder = sort;
		BackCage.sortingOrder = sort - 1;
		FrontCage.sprite = Cage1;
	}

	public override void HurtThis(float hurt)
	{
		if (collider2d.enabled)
		{
			Hp -= hurt;
			if (BrightCoroutine != null)
			{
				StopCoroutine(BrightCoroutine);
			}
			BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f));
			if (Hp <= (float)state1 && Hp >= (float)state2)
			{
				FrontCage.sprite = Cage2;
			}
			else if (Hp <= (float)state2)
			{
				FrontCage.sprite = Cage3;
			}
			else
			{
				FrontCage.sprite = Cage1;
			}
			if (Hp <= 0f)
			{
				collider2d.enabled = false;
				base.transform.GetComponent<Animator>().Play("CageSplit");
			}
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

	public void SplitAnim()
	{
		if (CurrGrid.Cage == this)
		{
			CurrGrid.Cage = null;
		}
		DestroyThis();
	}

	protected IEnumerator BrightnessEffect(float targetBright)
	{
		float currBright = FrontCage.material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return null;
			currBright += 5f * Time.deltaTime;
			FrontCage.material.SetFloat("_Brightness", currBright);
		}
		while (1f < targetBright)
		{
			yield return null;
			targetBright -= 5f * Time.deltaTime;
			FrontCage.material.SetFloat("_Brightness", targetBright);
		}
		FrontCage.material.SetFloat("_Brightness", 1f);
		BrightCoroutine = null;
	}
}
