using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;

public class Umbrellaleaf : PlantBase
{
	public Collider2D collider2d;

	public List<Sprite> leafs1 = new List<Sprite>();

	public List<Sprite> leafs2 = new List<Sprite>();

	public List<Sprite> leafs3 = new List<Sprite>();

	public List<SpriteRenderer> leafRenderers = new List<SpriteRenderer>();

	private int SortOrder;

	private bool isDead;

	private int state1;

	private int state2;

	private float GoalAlpha;

	private Coroutine coroutine;

	private bool isBlockGrid;

	public override float MaxHp => 300f;

	protected override void OnInitForAll()
	{
		SetState(1);
		isDead = false;
		GoalAlpha = 1f;
		isBlockGrid = false;
		coroutine = null;
		collider2d.enabled = false;
	}

	protected override void OnInitForPlace()
	{
		state1 = (int)MaxHp / 3 * 2;
		state2 = (int)MaxHp / 3;
		SortOrder = animatorSorting.sortingOrder;
	}

	protected override void HpUpdateEvents(ZombieBase zombie, bool isFlat)
	{
		if (base.Hp <= (float)state1 && base.Hp >= (float)state2)
		{
			SetState(2);
		}
		else if (base.Hp <= (float)state2)
		{
			SetState(3);
		}
		else
		{
			SetState(1);
		}
		if (base.Hp <= 0f && zombie != null)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gulp, base.transform.position);
		}
	}

	public override void SpecialAnimEvent1()
	{
		animatorSorting.sortingOrder = 2000;
	}

	public override void SpecialAnimEvent2()
	{
		SetAnimChange(12);
		animatorSorting.sortingOrder = SortOrder;
		if (isDead)
		{
			Dead();
		}
	}

	public bool Block(float attackValue)
	{
		if (!NormalDontAttackCondition())
		{
			SetAnimChange(11);
			float num = attackValue / 10f;
			if (base.Hp <= num && !isBlockGrid)
			{
				isDead = true;
			}
			else if (num >= 8f)
			{
				Hurt(num, Vector2.down, null);
			}
			return true;
		}
		return false;
	}

	public bool StruckThis()
	{
		if (!NormalDontAttackCondition())
		{
			isDead = true;
			SetAnimChange(11);
		}
		ServerSendSyn(0);
		return !NormalDontAttackCondition();
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StruckThis();
		}
	}

	private void SetState(int state)
	{
		List<Sprite> list = new List<Sprite>();
		list = state switch
		{
			2 => leafs2, 
			3 => leafs3, 
			_ => leafs1, 
		};
		for (int i = 0; i < leafRenderers.Count; i++)
		{
			leafRenderers[i].sprite = list[i];
			SetSprite(leafRenderers[i].name, list[i]);
		}
	}

	public override void WeatherChangeEvent()
	{
		if (!isSleeping)
		{
			if (SkyManager.Instance.HailScale > 0)
			{
				SetAnimChange(13);
				collider2d.enabled = true;
				SpecialAnimEvent1();
				isBlockGrid = true;
				MapManager.Instance.CoverGrid(base.transform.position, base.currGrid.Point, 1, 1, isCover: true, noCorner: false);
			}
			else
			{
				SetAnimChange(14);
				collider2d.enabled = false;
				SpecialAnimEvent2();
				isBlockGrid = false;
				MapManager.Instance.CoverGrid(base.transform.position, base.currGrid.Point, 1, 1, isCover: false, noCorner: false);
			}
		}
	}

	protected override void DeadEvent()
	{
		if (isBlockGrid)
		{
			isBlockGrid = false;
			MapManager.Instance.CoverGrid(base.transform.position, base.currGrid.Point, 1, 1, isCover: false, noCorner: false);
		}
	}

	private void OnMouseEnter()
	{
		GoalAlpha = 0.3f;
		if (coroutine == null)
		{
			coroutine = StartCoroutine(ToAlpha());
		}
	}

	private void OnMouseExit()
	{
		GoalAlpha = 1f;
		if (coroutine == null)
		{
			coroutine = StartCoroutine(ToAlpha());
		}
	}

	private IEnumerator ToAlpha()
	{
		Color color;
		bool flag;
		do
		{
			yield return null;
			color = leafRenderers[0].color;
			flag = color.a < GoalAlpha;
			float num = Time.deltaTime * 3f;
			if (!flag)
			{
				num = 0f - num;
			}
			SetLeafAlpha(color.a + num);
		}
		while ((!flag || !(color.a >= GoalAlpha)) && (flag || !(color.a <= GoalAlpha)));
		coroutine = null;
	}

	private void SetLeafAlpha(float alpha)
	{
		for (int i = 0; i < leafRenderers.Count; i++)
		{
			leafRenderers[i].color = new Color(leafRenderers[i].color.r, leafRenderers[i].color.g, leafRenderers[i].color.b, alpha);
		}
	}
}
