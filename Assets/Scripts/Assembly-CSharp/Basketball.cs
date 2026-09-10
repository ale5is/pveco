using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{
	private Vector2 StartPos;

	private Vector2 midPos;

	private Vector2 EndPos;

	private Grid TargetGrid;

	private ZombieBase TargetZombie;

	private Vector2 UmbrellaTarget;

	private float percent;

	private float percentSpeed;

	private float attackValue;

	private bool isHitUmbrella;

	private bool checkUmbrellaOver;

	private bool isHypno;

	public void Init(Vector2 startPos, Grid targetGrid, float attackValue, bool isHypno, Sprite sprite)
	{
		TargetZombie = null;
		checkUmbrellaOver = false;
		percent = 0f;
		this.isHypno = isHypno;
		isHitUmbrella = false;
		this.attackValue = attackValue;
		StartPos = startPos;
		TargetGrid = targetGrid;
		EndPos = TargetGrid.Position;
		midPos = MyTool.GetMiddlePosition(StartPos, TargetGrid.Position);
		percentSpeed = 6f / (TargetGrid.Position - StartPos).magnitude;
		if (percentSpeed > 1f)
		{
			percentSpeed = 1f;
		}
		base.transform.position = startPos;
		GetComponent<SpriteRenderer>().sprite = sprite;
	}

	public void Init(Vector2 startPos, ZombieBase target, float attackValue, Sprite sprite)
	{
		checkUmbrellaOver = false;
		percent = 0f;
		isHitUmbrella = false;
		this.attackValue = attackValue;
		StartPos = startPos;
		TargetZombie = target;
		EndPos = target.transform.position;
		TargetGrid = MapManager.Instance.GetGridByWorldPos(EndPos);
		midPos = MyTool.GetMiddlePosition(StartPos, TargetZombie.transform.position);
		percentSpeed = 6f / (EndPos - StartPos).magnitude;
		if (percentSpeed > 1f)
		{
			percentSpeed = 1f;
		}
		base.transform.position = startPos;
		GetComponent<SpriteRenderer>().sprite = sprite;
	}

	private void Update()
	{
		if (isHitUmbrella)
		{
			if (percent >= 0.8f)
			{
				Destroy();
			}
			percent += percentSpeed * Time.deltaTime;
			base.transform.position = MyTool.Bezier(percent, StartPos, midPos, UmbrellaTarget);
			return;
		}
		if (percent > 0.95f && !checkUmbrellaOver && TargetGrid != null)
		{
			List<Grid> aroundGrid = MapManager.Instance.GetAroundGrid(TargetGrid, 1);
			for (int i = 0; i < aroundGrid.Count; i++)
			{
				if (aroundGrid[i].CurrPlantBase != null && aroundGrid[i].CurrPlantBase.isHypno == isHypno)
				{
					if (aroundGrid[i].CurrPlantBase is Umbrellaleaf)
					{
						if (aroundGrid[i].CurrPlantBase.GetComponent<Umbrellaleaf>().Block(attackValue))
						{
							isHitUmbrella = true;
						}
					}
					else if (aroundGrid[i].CurrPlantBase.CarryPlant is Umbrellaleaf && aroundGrid[i].CurrPlantBase.CarryPlant.GetComponent<Umbrellaleaf>().Block(attackValue))
					{
						isHitUmbrella = true;
					}
				}
				if (isHitUmbrella)
				{
					break;
				}
			}
			if (!isHitUmbrella)
			{
				List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(TargetGrid.Position, 2.6f, isHypno, needCapsule: true);
				for (int j = 0; j < zombies.Count; j++)
				{
					if (zombies[j] is UmbrellaleafZombie && zombies[j].isHypno == !isHypno && zombies[j].GetComponent<UmbrellaleafZombie>().Block())
					{
						isHitUmbrella = true;
					}
				}
			}
			if (isHitUmbrella)
			{
				percent = 0f;
				int num = 1;
				if (StartPos.x > TargetGrid.Position.x)
				{
					num = -1;
				}
				StartPos = base.transform.position;
				UmbrellaTarget = StartPos + new Vector2(2 * num, 0f);
				percentSpeed = 3f / (UmbrellaTarget - StartPos).magnitude;
				midPos = MyTool.GetMiddlePosition(StartPos, UmbrellaTarget);
			}
			checkUmbrellaOver = true;
		}
		if (percent >= 1f)
		{
			if (TargetZombie != null)
			{
				TargetZombie.Hurt((int)attackValue, Vector2.up);
			}
			else if (TargetGrid != null && TargetGrid.CurrPlantBase != null)
			{
				TargetGrid.CurrPlantBase.Hurt(attackValue, Vector2.down, null);
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
			Destroy();
		}
		percent += percentSpeed * Time.deltaTime;
		base.transform.position = MyTool.Bezier(percent, StartPos, midPos, EndPos);
		base.transform.Rotate(new Vector3(0f, 0f, -100f * Time.deltaTime));
	}

	private void Destroy()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Basketball, base.gameObject);
	}
}
