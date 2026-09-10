using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Gravebuster : PlantBase
{
	public ParticleSystem StonePs;

	public override float MaxHp => 300f;

	public override bool CanPlaceOnGrass => false;

	public override bool CanPlaceOnWater => false;

	protected override bool HaveShadow => false;

	protected override Vector2 offSet => new Vector2(0f, 0.7f);

	protected override void OnInitForAll()
	{
		canIce = false;
		StonePs.gameObject.SetActive(value: false);
	}

	protected override void OnInitForPlace()
	{
		SetAnimChange(11);
		StonePs.gameObject.SetActive(value: true);
		animatorSorting.sortingOrder = animatorSorting.sortingOrder / 100 * 100 + 102;
		StonePs.GetComponent<SortingGroup>().sortingOrder = animatorSorting.sortingOrder + 1;
	}

	public override void SpecialAnimEvent1()
	{
		SetAnimChange(0);
		StartCoroutine(MoveDown());
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.gravestone_chomp, base.transform.position);
	}

	private IEnumerator MoveDown()
	{
		while (base.transform.position.y > base.currGrid.Position.y + 0.1f)
		{
			yield return null;
			base.gameObject.transform.Translate(new Vector2(0f, -0.2f) * Time.deltaTime);
		}
		if (!GameManager.Instance.isClient)
		{
			base.currGrid.HaveGraveStone = false;
		}
		LvItemManager.Instance.DropCoin(base.transform.position, AlwaysDrop: true);
		Dead();
	}

	protected override void GoSleepSpecial()
	{
		ZZZ.gameObject.SetActive(value: false);
		StonePs.gameObject.SetActive(value: true);
	}
}
