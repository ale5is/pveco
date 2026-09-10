using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hepatica : PlantBase
{
	private Vector2 TargetPos;

	public override float MaxHp => 300f;

	protected override void OnInitForPlace()
	{
		canFrozen = false;
		canIce = false;
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
	}

	private void OnMouseOver()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && base.currGrid != null && Input.GetMouseButtonDown(0))
		{
			CobCannonTarget.Instance.StartAim(this, (Vector2 pos) =>
			{
				Shoot(pos);
			});
		}
	}

	private IEnumerator WaitCharge()
	{
		yield return new WaitForSeconds(35f);
	}

	private void CreateIcecone()
	{
		Object.Instantiate(GameManager.Instance.GameConf.CannonCob).GetComponent<CannonCob>().CreateInit(base.transform.position + new Vector3(-0.1f, 3f), TargetPos, attackValue, isHypno, PlacePlayer);
	}

	public void Shoot(Vector2 pos)
	{
		base.transform.GetComponent<CapsuleCollider2D>().enabled = false;
		TargetPos = pos;
	}
}
