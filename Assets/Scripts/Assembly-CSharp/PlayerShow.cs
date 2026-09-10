using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerShow : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image State;

	public Image MapSprite;

	public Text nameText;

	public OnlineSeedBank SeedBank;

	public PlantBase plantInGrid;

	public ZombieBase zombieInGrid;

	public Animator GridSeletor;

	public TextMesh GridSeletorName;

	private bool isPrepare;

	public bool IsPrepare
	{
		get
		{
			return isPrepare;
		}
		set
		{
			isPrepare = value;
			if (SpectatorList.Instance.IsSpectator(nameText.text))
			{
				State.color = new Color(0.5f, 0.5f, 0.5f);
				isPrepare = true;
			}
			else if (isPrepare)
			{
				State.color = new Color(0f, 1f, 0f);
			}
			else
			{
				State.color = new Color(1f, 0.432f, 0f);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!SpectatorList.Instance.IsSpectator(nameText.text) && !(nameText.text == GameManager.Instance.LocalPlayerSave.playerName))
		{
			SeedBank.transform.localScale = new Vector2(1f, 1f);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SeedBank.transform.localScale = new Vector2(0f, 0f);
	}
}
