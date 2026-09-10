using System.Collections.Generic;
using UnityEngine;

public class MapCloud : MonoBehaviour
{
	public List<Sprite> CloudSprites = new List<Sprite>();

	private SpriteRenderer REnderer;

	private float BaseSpeed;

	private MapBase CurrMap;

	private bool isShadow;

	private void Update()
	{
		int num = -1;
		if (SkyManager.Instance.WindTowardRight)
		{
			num = 1;
		}
		if (CurrMap.IsFacingLeft)
		{
			num *= -1;
		}
		base.transform.position += new Vector3(num, 0f) * BaseSpeed * Time.deltaTime * (1f + (float)SkyManager.Instance.WindScale * 0.1f);
		if (base.transform.position.x > 17.5f || base.transform.position.x < -17.5f)
		{
			CurrMap.MapCloudClear(this);
			Object.Destroy(base.gameObject);
		}
	}

	public void CreateInit(bool isShadow, MapBase map)
	{
		int spriteId = Random.Range(0, CloudSprites.Count);
		ClientInit(isShadow, map, Random.Range(0.3f, 1f), spriteId);
	}

	public void ClientInit(bool isShadow, MapBase map, float speed, int spriteId)
	{
		this.isShadow = isShadow;
		REnderer = base.transform.GetComponent<SpriteRenderer>();
		CurrMap = map;
		BaseSpeed = speed;
		REnderer.sprite = CloudSprites[spriteId];
		int rainScale = SkyManager.Instance.RainScale;
		REnderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		if (isShadow)
		{
			REnderer.sortingOrder = 1900;
			float num = Random.Range(0.5f, 1.5f) + (float)rainScale * 0.05f;
			base.transform.localScale = new Vector3(num, num);
			REnderer.color = new Color(0f, 0f, 0f, 0.12f - (float)rainScale * 0.01f);
		}
		else
		{
			REnderer.color = Color.white;
		}
	}

	public float GetBaseSpeed()
	{
		return BaseSpeed;
	}

	public int GetSpriteId()
	{
		return CloudSprites.IndexOf(REnderer.sprite);
	}

	public void SetTimeColor(Color color)
	{
		if (!isShadow)
		{
			REnderer.color = new Color(color.r, color.g, color.b, 1f);
		}
	}
}
