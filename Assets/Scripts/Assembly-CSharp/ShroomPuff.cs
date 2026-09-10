using UnityEngine;
using UnityEngine.Rendering;

public class ShroomPuff : BulletBase
{
	private float createX;

	private bool needDis;

	public Transform particle;

	public void Init(int attackValue, Vector2 pos, int currLine, Vector2 dirct, int sortOrder, bool NeedDis, bool isHyp, bool IsLow)
	{
		SetSortingOrder(sortOrder);
		isLow = IsLow;
		isHypno = isHyp;
		needDis = NeedDis;
		createX = pos.x;
		CurrLine = currLine;
		Dirction = dirct;
		base.transform.position = pos;
		MoveSpeed = 6f;
		base.attackValue = attackValue;
		base.transform.SetParent(MapManager.Instance.GetCurrMap(pos).transform);
		particle.localScale = new Vector3(Mathf.Abs(particle.localScale.x), particle.localScale.y, particle.localScale.z);
		if (dirct.x < 0f)
		{
			particle.localScale = new Vector3(0f - particle.localScale.x, particle.localScale.y, particle.localScale.z);
		}
		BaseInit();
	}

	protected override void UpdateThis()
	{
		if (needDis && Mathf.Abs(base.transform.position.x - createX) > 4.9f)
		{
			DestoryBullet();
			return;
		}
		if (MapManager.Instance.GetCurrMap(base.transform.position) == null)
		{
			DestoryBullet();
		}
		if (base.CurrGrid != null && base.CurrGrid.CurrPlantBase != null && ((!base.CurrGrid.CurrPlantBase.isHypno && isHypno) || (base.CurrGrid.CurrPlantBase.isHypno && !isHypno)) && (!base.CurrGrid.CurrPlantBase.IsLowPlant || (base.CurrGrid.CurrPlantBase.IsLowPlant && (isLow || base.CurrGrid.CurrPlantBase.CarryPlant != null || base.CurrGrid.CurrPlantBase.ProtectPlant != null))) && Mathf.Abs(base.transform.position.x - base.CurrGrid.Position.x) < 0.2f)
		{
			base.CurrGrid.CurrPlantBase.Hurt(attackValue, Dirction, null);
			HitEff();
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
		}
	}

	protected override void TriggerEnter(Collider2D collision)
	{
		if (collision.tag == "Zombie")
		{
			ZombieBase componentInParent = collision.GetComponentInParent<ZombieBase>();
			if (componentInParent.CurrLine == CurrLine && ((componentInParent.isHypno && isHypno) || (!componentInParent.isHypno && !isHypno)))
			{
				componentInParent.Hurt(attackValue, Dirction);
				HitEff();
			}
		}
		if (collision.tag == "Wall" && !collision.transform.GetComponent<MapWall>().IsPass(Dirction))
		{
			if (Random.Range(0, 3) == 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat1, base.transform.position);
			}
			else if (Random.Range(1, 3) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat2, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.splat3, base.transform.position);
			}
			HitEff();
		}
	}

	private void HitEff()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.PuffParticle).transform.position = base.transform.position + new Vector3(Dirction.normalized.x * Random.Range(0.1f, 0.2f), Dirction.normalized.y * Random.Range(0.1f, 0.2f));
		DestoryBullet();
	}

	protected override void DestoryBullet()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ShroomPuff, base.gameObject);
	}

	protected override void SetSortingOrder(int sorting)
	{
		particle.GetComponent<SortingGroup>().sortingOrder = sorting;
		GetComponent<SpriteRenderer>().sortingOrder = sorting;
	}

	protected override void HitEvent()
	{
		HitEff();
	}
}
