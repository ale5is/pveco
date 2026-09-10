using UnityEngine;

public class Tanglekelp : PlantBase
{
	private ZombieBase zombie;

	private bool isAttack;

	public SpriteMask WaterMask;

	public override float MaxHp => 300f;

	public override bool ZombieCanEat => isSleeping;

	public override bool CanPlaceOnWater => true;

	protected override int attackValue => 1800;

	protected override Vector2 offSet => new Vector2(0f, -0.2f);

	public override bool CanPlaceOnGrass => false;

	public override bool CanCarryed => false;

	protected override bool HaveShadow => false;

	public override bool CanProtect => false;

	public override bool IsLowPlant => true;

	protected override void OnInitForAll()
	{
		WaterMask.enabled = false;
		PlayAnim("anim_idle_aquarium", 0);
	}

	protected override void OnInitForPlace()
	{
		isAttack = false;
		PlayAnim("anim_idle", 0);
		StartActionCD(1f, isOver: true);
	}

	protected override bool DoAction()
	{
		Attack();
		return false;
	}

	private void Attack()
	{
		if (!NormalDontAttackCondition() && !isAttack && base.currGrid != null)
		{
			zombie = ZombieManager.Instance.GetZombieByLineMinDisNoDir(base.currGrid.Point.y, base.transform.position, isHypno, needCapsule: false);
			if (!(zombie == null) && zombie.InWater && Mathf.Abs(zombie.transform.position.x - base.transform.position.x) < 0.8f)
			{
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Tanglekelpgrab).GetComponent<Tanglekelpgrab>().Init(this, zombie, GetAttackValue());
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(new Vector2(zombie.transform.position.x, base.transform.position.y + 0.15f), base.currGrid.Point.y);
				isAttack = true;
				WaterMask.enabled = true;
				WaterMask.frontSortingOrder = zombie.SortOrder + 1;
				WaterMask.backSortingOrder = zombie.SortOrder;
			}
		}
	}

	public void CreateSplash()
	{
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.Splash).GetComponent<Splash>().CreateInit(base.transform.position + new Vector3(0f, 0.15f), base.currGrid.Point.y);
		if (Random.Range(1, 3) == 1)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DropWater, base.transform.position);
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ZombieEnteringWater, base.transform.position);
		}
	}
}
