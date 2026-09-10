using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
	private PortalController Controller;

	public Grid CurrGrid;

	private bool CanTp;

	private Animator animator;

	private SortingGroup Sorting;

	private Collider2D collider2;

	public void InitThis(Grid grid, PortalController controller)
	{
		Controller = controller;
		animator = base.transform.GetComponent<Animator>();
		Sorting = base.transform.GetComponent<SortingGroup>();
		collider2 = base.transform.GetComponent<Collider2D>();
		CanTp = false;
		collider2.enabled = false;
		CurrGrid = grid;
		Sorting.sortingOrder = CurrGrid.Point.y * 200 + 198;
		base.transform.position = grid.Position + new Vector2(0.6f, 0.1f);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Portal, base.transform.position);
	}

	public void ResetThis(Grid grid)
	{
		CanTp = false;
		collider2.enabled = false;
		CurrGrid = grid;
		animator.Play("Anim3");
		Sorting.sortingOrder = CurrGrid.Point.y * 200 + 192;
	}

	public void OpenOver()
	{
		CanTp = true;
		collider2.enabled = true;
		animator.Play("Anim2");
	}

	public void CloseOver()
	{
		CanTp = false;
		collider2.enabled = false;
		base.transform.position = CurrGrid.Position + new Vector2(0.7f, 0.1f);
		animator.Play("Anim1");
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Portal, base.transform.position);
	}

	public Portal GetNextPortal()
	{
		return Controller.GetNextPortal(this);
	}

	private void FixedUpdate()
	{
		if (!CanTp)
		{
			return;
		}
		List<ZombieBase> zombiesByLine = ZombieManager.Instance.GetZombiesByLine(CurrGrid.Point.y, base.transform.position, 0.5f, needCapsule: false);
		for (int i = 0; i < zombiesByLine.Count; i++)
		{
			Portal nextPortal = Controller.GetNextPortal(this);
			if (!zombiesByLine[i].dontChangeState)
			{
				if (zombiesByLine[i].IsFacingLeft && zombiesByLine[i].transform.position.x > base.transform.position.x)
				{
					zombiesByLine[i].TeleportTo(new Vector2(nextPortal.transform.position.x - 0.2f, nextPortal.CurrGrid.Position.y));
				}
				else if (!zombiesByLine[i].IsFacingLeft && zombiesByLine[i].transform.position.x <= base.transform.position.x)
				{
					zombiesByLine[i].TeleportTo(new Vector2(nextPortal.transform.position.x + 0.2f, nextPortal.CurrGrid.Position.y));
				}
			}
		}
	}
}
