using UnityEngine;

public class Icecone : MonoBehaviour
{
	private Vector2 targetPos;

	private SpriteRenderer SmallIce;

	private Rigidbody2D rigibody;

	private int attackValue;

	private Vector2 Dirction;

	private bool isHit;

	public void CreateInit(Vector2 pos, Vector2 target, int attackvalue)
	{
		isHit = false;
		attackValue = attackvalue;
		Dirction = (target - pos).normalized;
		rigibody = GetComponent<Rigidbody2D>();
		rigibody.velocity = Vector2.zero;
		rigibody.velocity = Dirction.normalized * 6f;
		base.transform.GetComponent<SpriteRenderer>().enabled = true;
		SmallIce = base.transform.Find("Ice").GetComponent<SpriteRenderer>();
		SmallIce.enabled = true;
		base.transform.position = pos;
		targetPos = target;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isHit)
		{
			return;
		}
		if (collision.tag == "Zombie")
		{
			ZombieBase component = collision.GetComponent<ZombieBase>();
			if (!component.isHypno)
			{
				isHit = true;
				component.Hurt(attackValue, Dirction);
				PoolManager.Instance.GetObj(GameManager.Instance.GameConf.SnowpeaParticle).transform.position = base.transform.position + new Vector3(rigibody.velocity.normalized.x * Random.Range(0.1f, 0.2f), rigibody.velocity.normalized.y * Random.Range(0.1f, 0.2f));
				Destroy();
			}
		}
		if (collision.tag == "Torchwood")
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.FirePea, base.transform.position);
			Destroy();
		}
	}

	private void Destroy()
	{
		StopAllCoroutines();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Pea, base.gameObject);
	}
}
