using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ZombieHeadParticle : MonoBehaviour
{
	public ParticleSystem Ps;

	private ParticleSystem.MainModule PsMain;

	public void InitCreate(Vector2 pos, int sortorder, Sprite head, bool inWater, float scale)
	{
		PsMain = Ps.main;
		Ps.textureSheetAnimation.SetSprite(0, head);
		base.transform.position = pos;
		PsMain.startSize = scale;
		base.transform.GetComponent<SortingGroup>().sortingOrder = sortorder + 1;
		StartCoroutine(StartPause(inWater));
	}

	private IEnumerator StartPause(bool inWater)
	{
		yield return new WaitForFixedUpdate();
		Ps.Simulate(0f);
		Ps.Play();
		yield return new WaitForSeconds(PsMain.duration - Time.fixedDeltaTime);
		Ps.Pause();
		if (!inWater)
		{
			float x = Random.Range(-1f, 1f);
			float t = Random.Range(0.08f, 0.2f);
			float y = base.transform.position.y;
			while (base.transform.position.y < y + t)
			{
				yield return new WaitForFixedUpdate();
				base.transform.position += new Vector3(x * Time.deltaTime, 2f * Time.deltaTime);
			}
			while (base.transform.position.y > y)
			{
				yield return new WaitForFixedUpdate();
				base.transform.position += new Vector3(x * Time.deltaTime, -1f * Time.deltaTime);
			}
			yield return new WaitForSeconds(0.5f);
		}
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.ZombieHeadParticle, base.gameObject);
	}
}
