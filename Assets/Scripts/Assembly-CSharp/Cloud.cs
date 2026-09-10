using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
	public Animator animator;

	public SpriteRenderer spriteRenderer;

	public SpriteRenderer spriteRenderer2;

	public List<Sprite> cloudSprites = new List<Sprite>();

	private void Start()
	{
		PlayOver();
	}

	public void PlayOver()
	{
		animator.speed = Random.Range(0.4f, 1f);
		spriteRenderer.sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];
		spriteRenderer2.sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];
		while (spriteRenderer.sprite == spriteRenderer2.sprite)
		{
			spriteRenderer2.sprite = cloudSprites[Random.Range(0, cloudSprites.Count)];
		}
	}
}
