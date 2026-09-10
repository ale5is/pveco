using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPlantCardAnimation : MonoBehaviour
{
	private Text WantSunText;

	public Image image;

	public int NeedSun;

	private UIPlantCardNC uIPlantCardNC;

	private PlantCard PlantCard;

	private void Awake()
	{
		WantSunText = base.transform.Find("CardSunNumText").GetComponent<Text>();
		image = base.transform.GetComponent<Image>();
	}

	public void CreateInit(Vector2 initPos, UIPlantCardNC nC, PlantCard pC)
	{
		SeedBank.Instance.uICardAnims.Add(this);
		base.transform.position = initPos;
		base.transform.localScale = SeedChooser.Instance.ForGetSizeNc.transform.localScale;
		image.sprite = nC.OwnerSprite;
		WantSunText.text = nC.NeedNum.ToString();
		NeedSun = nC.NeedNum;
		uIPlantCardNC = nC;
		PlantCard = pC;
		base.transform.SetParent(SeedChooser.Instance.transform.parent);
	}

	public void PlayChooseAnimation(Vector3 target, UnityAction action = null)
	{
		StartCoroutine(ChooseAnimation(target, action));
	}

	public void PlayChooseAnimation(Vector3 target, bool isBack)
	{
		StartCoroutine(ChooseAnimation(target, isBack));
	}

	private IEnumerator ChooseAnimation(Vector3 target, bool isBack)
	{
		Vector2 dir = (target - base.transform.position).normalized;
		if (isBack)
		{
			while (target.y < base.transform.position.y)
			{
				yield return null;
				base.transform.Translate(dir * 2000f * Time.deltaTime);
			}
		}
		else
		{
			while (target.y > base.transform.position.y)
			{
				yield return null;
				base.transform.Translate(dir * 2000f * Time.deltaTime);
			}
		}
		if (isBack)
		{
			PlantCard.ClearChooseOk();
		}
		else
		{
			PlantCard.AddChoose(uIPlantCardNC);
		}
		SeedBank.Instance.uICardAnims.Remove(this);
		DestroyThis();
	}

	public void DestroyThis()
	{
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.CardSlotAnimation, base.gameObject);
	}

	private IEnumerator ChooseAnimation(Vector3 target, UnityAction action)
	{
		Vector2 dir = (target - base.transform.position).normalized;
		Vector2.Distance(target, base.transform.position);
		if (target.y > base.transform.position.y)
		{
			while (target.y > base.transform.position.y)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(dir * 2000f * Time.deltaTime);
			}
		}
		else
		{
			while (target.y < base.transform.position.y)
			{
				yield return new WaitForFixedUpdate();
				base.transform.Translate(dir * 2000f * Time.deltaTime);
			}
		}
		action?.Invoke();
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.CardSlotAnimation, base.gameObject);
	}
}
