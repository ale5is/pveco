using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class ZombieOutGround : MonoBehaviour
{
	public Animator HandUp;

	public SpriteMask Mask;

	public ParticleSystem DirtParticle;

	private Grid GoalGrid;

	private ZombieBase Goalzombie;

	private UnityAction overAction;

	private bool isStart;

	private void Update()
	{
		if (!isStart)
		{
			return;
		}
		if (Goalzombie.transform.position.y < GoalGrid.Position.y)
		{
			Goalzombie.transform.Translate(new Vector2(0f, 6f) * Time.deltaTime);
			return;
		}
		if (overAction != null)
		{
			overAction();
		}
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ZombieOutGround, base.gameObject);
	}

	public void CreateInit(Grid grid, ZombieBase zombie, UnityAction action, bool needArm)
	{
		isStart = false;
		GoalGrid = grid;
		Goalzombie = zombie;
		overAction = action;
		Mask.backSortingOrder = grid.Point.y * 200 + 191;
		Mask.frontSortingOrder = grid.Point.y * 200 + 192;
		HandUp.GetComponent<SortingGroup>().sortingOrder = grid.Point.y * 200 + 191;
		DirtParticle.GetComponent<SortingGroup>().sortingOrder = grid.Point.y * 200 + 192;
		zombie.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 2.5f);
		if (GoalGrid.isNoIceWater)
		{
			StartCoroutine(ZombieOut(0.4f));
			HandUp.gameObject.SetActive(value: false);
			DirtParticle.gameObject.SetActive(value: false);
			return;
		}
		DirtParticle.gameObject.SetActive(value: true);
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.DirtRise, base.transform.position);
		if (needArm)
		{
			HandUp.gameObject.SetActive(value: true);
			StartCoroutine(ZombieOut(1f));
		}
		else
		{
			HandUp.gameObject.SetActive(value: false);
			StartCoroutine(ZombieOut(0.4f));
		}
	}

	private IEnumerator ZombieOut(float time)
	{
		yield return new WaitForSeconds(time);
		isStart = true;
	}
}
