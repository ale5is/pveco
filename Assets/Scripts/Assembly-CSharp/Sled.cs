using System.Collections;
using UnityEngine;

public class Sled : MonoBehaviour
{
	public Sprite Sled2;

	public Sprite Sled3;

	public Sprite Sled4;

	public SpriteRenderer SledOut;

	public SpriteRenderer SledInn;

	private int hp = 500;

	private Coroutine BrightCoroutine;

	public int Hp
	{
		get
		{
			return hp;
		}
		set
		{
			if (value <= hp)
			{
				if (BrightCoroutine != null)
				{
					StopCoroutine(BrightCoroutine);
				}
				BrightCoroutine = StartCoroutine(BrightnessEffect(1.5f));
			}
			hp = value;
			if (hp < 100)
			{
				SledInn.transform.position += new Vector3(0f, -0.12f);
				SledOut.sprite = Sled4;
			}
			else if (hp < 200)
			{
				SledOut.sprite = Sled3;
			}
			else if (hp < 300)
			{
				SledOut.sprite = Sled2;
			}
		}
	}

	protected IEnumerator BrightnessEffect(float targetBright)
	{
		float currBright = SledOut.material.GetFloat("_Brightness");
		while (currBright < targetBright)
		{
			yield return null;
			currBright += 5f * Time.deltaTime;
			SledOut.material.SetFloat("_Brightness", currBright);
			SledInn.material.SetFloat("_Brightness", currBright);
		}
		while (1f < targetBright)
		{
			yield return null;
			targetBright -= 5f * Time.deltaTime;
			SledOut.material.SetFloat("_Brightness", targetBright);
			SledInn.material.SetFloat("_Brightness", targetBright);
		}
		SledOut.material.SetFloat("_Brightness", 1f);
		SledInn.material.SetFloat("_Brightness", 1f);
		BrightCoroutine = null;
	}

	public void SledDead(bool isBoom)
	{
		StartCoroutine(DeadEffect(isBoom));
	}

	private IEnumerator DeadEffect(bool isBoom)
	{
		if (isBoom)
		{
			SledOut.color = Color.black;
			SledInn.color = Color.black;
			yield return new WaitForSeconds(1f);
		}
		else
		{
			float currAlpha = SledOut.color.a;
			while (currAlpha > 0f)
			{
				yield return null;
				currAlpha -= 5f * Time.deltaTime;
				SledOut.color = new Color(1f, 1f, 1f, currAlpha);
				SledInn.color = new Color(1f, 1f, 1f, currAlpha);
			}
		}
		Object.Destroy(base.gameObject);
	}

	public void CreateInit(int order)
	{
		SledInn.sortingOrder = order + 2;
		SledOut.sortingOrder = order + 3;
	}

	public void Jump()
	{
		SledInn.sortingOrder -= 3;
		SledOut.sortingOrder += 3;
		StartCoroutine(MoveDown());
	}

	private IEnumerator MoveDown()
	{
		while (base.transform.localPosition.x != -0.35f)
		{
			yield return null;
			base.transform.localPosition = Vector2.MoveTowards(base.transform.localPosition, new Vector2(-0.35f, 0f), Time.deltaTime * 2f);
		}
	}
}
