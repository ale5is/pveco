using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Allcoin : MonoBehaviour
{
	private bool isFlying;

	private Animator animator;

	private SphereCollider sphereCollider;

	private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	protected abstract int money { get; }

	protected abstract AudioClip sound { get; }

	protected abstract AudioClip dropSound { get; }

	protected abstract GameObject Prefab { get; }

	public void OnMouseDown()
	{
		if (!isFlying)
		{
			isFlying = true;
			animator.speed = 0f;
			animator.Play("normal", 0, 0f);
			Coinbank.Instance.ShowCoinbank();
			StatsManager.Instance.AddStatsNum(StatsEnum.ClickMoney);
			Vector3 vector = Camera.main.ScreenToWorldPoint(Coinbank.Instance.GetCoinbankTextPos());
			vector = new Vector3(vector.x, vector.y, 0f);
			FlyAnimation(vector);
			AudioManager.Instance.PlayEFAudio(sound, base.transform.position);
		}
	}

	private void FlyAnimation(Vector3 pos)
	{
		StartCoroutine(DoFly(pos));
	}

	private IEnumerator DoFly(Vector3 pos)
	{
		Vector3 direction = (pos - base.transform.position).normalized;
		while (Vector3.Distance(pos, base.transform.position) > 0.5f)
		{
			yield return new WaitForSeconds(0.02f);
			base.transform.Translate(direction);
		}
		PlayerManager.Instance.Money += money;
		Destroy();
	}

	public void InitForItem(Vector2 pos)
	{
		isFlying = false;
		if (sphereCollider == null)
		{
			sphereCollider = GetComponent<SphereCollider>();
			animator = base.transform.Find("Animation").GetComponent<Animator>();
			SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
			renderers.AddRange(componentsInChildren);
		}
		sphereCollider.enabled = true;
		animator.speed = 1f;
		SetAllColor(Color.white);
		base.transform.position = pos;
		StartCoroutine(DoJump());
		Invoke("FadeDestroy", 10f);
	}

	private IEnumerator DoJump()
	{
		bool num = Random.Range(0, 2) == 0;
		Vector3 startPos = base.transform.position;
		float x = Random.Range(0.1f, 0.3f);
		if (num)
		{
			x = 0f - x;
		}
		Grid gridByWorldPos = MapManager.Instance.GetGridByWorldPos(base.transform.position);
		if (gridByWorldPos != null && Mathf.Abs(base.transform.position.x - gridByWorldPos.Position.x) > 1.4f)
		{
			x = ((!(gridByWorldPos.Position.x > base.transform.position.x)) ? (-0.5f) : 0.5f);
		}
		float speed = 0f;
		float scale = base.transform.localScale.y;
		base.transform.localScale = new Vector3(0.1f, 0.1f);
		sphereCollider.enabled = false;
		while (base.transform.localScale.y < scale)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				base.transform.localScale += new Vector3(Time.deltaTime * 5f, Time.deltaTime * 5f);
				speed += 0.04f * Time.deltaTime;
				base.transform.Translate(new Vector3(x * Time.deltaTime, speed, 0f));
			}
		}
		base.transform.localScale = new Vector3(scale, scale);
		sphereCollider.enabled = true;
		while ((double)base.transform.position.y >= (double)startPos.y - 0.2)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				speed -= 0.06f * Time.deltaTime;
				base.transform.Translate(new Vector3(x * Time.deltaTime, speed, 0f));
			}
		}
		AudioManager.Instance.PlayEFAudio(dropSound, base.transform.position);
		PlayerManager.Instance.Coins.Add(this);
	}

	public void DoFlytoMagnet(Vector3 pos)
	{
		if (!isFlying)
		{
			isFlying = true;
			StartCoroutine(Fly(pos));
		}
	}

	private IEnumerator Fly(Vector3 pos)
	{
		Vector3 velocity = default;
		while (Vector3.Distance(pos, base.transform.position) > 0.2f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				base.transform.position = Vector3.SmoothDamp(base.transform.position, pos, ref velocity, 0.4f);
			}
		}
		PlayerManager.Instance.Money += money;
		Destroy();
	}

	private void SetAllColor(Color color)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].color = color;
		}
	}

	public void Destroy()
	{
		CancelInvoke();
		StopAllCoroutines();
		PlayerManager.Instance.Coins.Remove(this);
		PoolManager.Instance.PushObj(Prefab, base.gameObject);
	}

	private void FadeDestroy()
	{
		StartCoroutine(DeadEffect());
	}

	private IEnumerator DeadEffect()
	{
		sphereCollider.enabled = false;
		PlayerManager.Instance.Coins.Remove(this);
		float currBright = renderers[0].color.a;
		while (currBright > 0f)
		{
			yield return null;
			currBright -= 3f * Time.deltaTime;
			SetAllColor(new Color(1f, 1f, 1f, currBright));
		}
		StatsManager.Instance.ClearStatsNum(StatsEnum.ClickMoney);
		StopAllCoroutines();
		PoolManager.Instance.PushObj(Prefab, base.gameObject);
	}
}
