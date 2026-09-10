using SaveClass;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelDisPlay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	public Image MapImage;

	public Text LevelText;

	public Image REnderer;

	public Image LvCard;

	public Transform LockImg;

	public LvSave lVInfo;

	public void OpenInit(LvSave info, LvSave lastInfo)
	{
		lVInfo = info;
		LockImg.localScale = Vector3.one;
		LvCard.color = new Color32(100, 100, 100, byte.MaxValue);
		MapImage.color = new Color32(100, 100, 100, byte.MaxValue);
		if (info == null)
		{
			LevelText.text = "暂无关卡";
			LvCard.transform.localScale = Vector3.zero;
			MapImage.sprite = NormalSprite.Instance.YardDay;
			return;
		}
		LV.Instance.LoadLV(info.LvId, LevelSelector.Instance.IsEasy, onlyInfo: true, isRun: false);
		MapImage.sprite = LV.Instance.GetLvSprite();
		int num = info.LvId / 10000;
		int num2 = info.LvId % 10000 / 1000;
		int num3 = info.LvId % 1000 - 1;
		if (num2 == 1)
		{
			if (num == 1 && num3 < NormalSprite.Instance.YardMiniGame.Count)
			{
				MapImage.sprite = NormalSprite.Instance.YardMiniGame[num3];
			}
		}
		else
		{
			_ = 2;
		}
		if (LV.Instance.BootyPlant == PlantType.Nope)
		{
			LvCard.transform.localScale = Vector3.zero;
		}
		else
		{
			LvCard.sprite = SeedChooser.Instance.GetCardInfo(LV.Instance.BootyPlant).OwnerSprite;
			LvCard.transform.localScale = Vector3.one;
		}
		LevelText.text = LV.Instance.LvName;
		if (LevelSelector.Instance.SelectedLvSave != null && LevelSelector.Instance.SelectedLvSave.LvId == lVInfo.LvId)
		{
			REnderer.color = new Color32(150, 150, 150, byte.MaxValue);
		}
		if (lastInfo == null || lastInfo.PassNum > 0 || lastInfo.HardPNum > 0)
		{
			LockImg.localScale = Vector3.zero;
			MapImage.color = Color.white;
			LvCard.color = Color.white;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (lVInfo == null)
		{
			return;
		}
		if (LockImg.localScale.x > 0f && LevelSelector.Instance.transform.localScale.x > 0f)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Buzzer, base.transform.position, isAll: true);
		}
		else if (LevelSelector.Instance.SelectedLvSave == null || LevelSelector.Instance.SelectedLvSave.LvId != lVInfo.LvId)
		{
			if (eventData != null)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
			}
			LevelSelector.Instance.SelectThis(this);
			REnderer.color = new Color32(150, 150, 150, byte.MaxValue);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (lVInfo != null && !(LockImg.localScale.x > 0f))
		{
			REnderer.color = new Color32(150, 150, 150, byte.MaxValue);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (lVInfo != null && (LevelSelector.Instance.SelectedLvSave == null || LevelSelector.Instance.SelectedLvSave.LvId != lVInfo.LvId))
		{
			REnderer.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}
}
