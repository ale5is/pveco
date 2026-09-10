using System.Collections;
using SocketSave;
using UnityEngine;

public class Booty : MonoBehaviour
{
	private bool isClicked;

	public SpriteRenderer MoneyREderer;

	public SpriteRenderer CardREderer;

	public TextMesh SunNumText;

	private bool IsFirstPass;

	public void InitThis()
	{
		isClicked = false;
		IsFirstPass = LV.Instance.CurrLvInfo != null && LV.Instance.CurrLvInfo.PassNum == 0 && LV.Instance.CurrLvInfo.HardPNum == 0;
		if (IsFirstPass && LV.Instance.BootyPlant != PlantType.Nope)
		{
			MoneyREderer.transform.localScale = Vector3.zero;
			UIPlantCardNC plantNc = SeedBank.Instance.GetPlantNc(LV.Instance.BootyPlant);
			CardREderer.sprite = plantNc.OwnerSprite;
			SunNumText.text = plantNc.NeedNum.ToString();
		}
		else
		{
			CardREderer.transform.localScale = Vector3.zero;
			if (IsFirstPass)
			{
				MoneyREderer.sprite = GetBootySprite(LV.Instance.BootySprite);
			}
			else
			{
				MoneyREderer.sprite = NormalSprite.Instance.MoneyBag;
			}
		}
	}

	public void InitThis(SynBooty syn)
	{
		isClicked = false;
		IsFirstPass = LV.Instance.CurrLvInfo != null && LV.Instance.CurrLvInfo.PassNum == 0 && LV.Instance.CurrLvInfo.HardPNum == 0;
		if (IsFirstPass && syn.BootyPlant != PlantType.Nope)
		{
			MoneyREderer.transform.localScale = Vector3.zero;
			UIPlantCardNC plantNc = SeedBank.Instance.GetPlantNc(syn.BootyPlant);
			CardREderer.sprite = plantNc.OwnerSprite;
			SunNumText.text = plantNc.NeedNum.ToString();
		}
		else
		{
			CardREderer.transform.localScale = Vector3.zero;
			if (IsFirstPass)
			{
				MoneyREderer.sprite = GetBootySprite(syn.sprite);
			}
			else
			{
				MoneyREderer.sprite = NormalSprite.Instance.MoneyBag;
			}
		}
	}

	public Sprite GetBootySprite(BootySprite booty)
	{
		Sprite result = null;
		switch (booty)
		{
		case BootySprite.Almanac:
			result = NormalSprite.Instance.Almanac;
			break;
		case BootySprite.Store:
			result = NormalSprite.Instance.Store;
			break;
		case BootySprite.Shovel:
			result = NormalSprite.Instance.Shovel;
			break;
		case BootySprite.MoneyBag:
			result = NormalSprite.Instance.MoneyBag;
			break;
		case BootySprite.Trophy:
			result = NormalSprite.Instance.Trophy;
			break;
		case BootySprite.ZombieNote:
			result = NormalSprite.Instance.ZombieNote;
			break;
		case BootySprite.Taco:
			result = NormalSprite.Instance.Taco;
			break;
		}
		return result;
	}

	public void OnMouseDown()
	{
		if (!GameManager.Instance.isClient && !isClicked)
		{
			CollectBooty();
		}
	}

	public void CollectBooty()
	{
		isClicked = true;
		if (MapManager.Instance.GetCurrMap(base.transform.position) != CameraControl.Instance.CurrMap)
		{
			base.transform.position = new Vector3(CameraControl.Instance.transform.position.x, CameraControl.Instance.transform.position.y, 0f);
		}
		Vector3 vector = new Vector3(CameraControl.Instance.transform.position.x, CameraControl.Instance.transform.position.y, 0f);
		vector = new Vector3(vector.x, vector.y, 0f);
		StartCoroutine(DoFly(vector));
		PoolManager.Instance.GetObj(GameManager.Instance.GameConf.BootyParticle).transform.position = base.transform.position;
		if (LV.Instance.BootyEvent != null)
		{
			LV.Instance.BootyEvent();
		}
		if (!IsFirstPass)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ClickCoin, base.transform.position, isAll: true);
			Coinbank.Instance.ShowCoinbank();
			if (LV.Instance.IsEasy)
			{
				PlayerManager.Instance.Money += 250;
			}
			else
			{
				PlayerManager.Instance.Money += 400;
			}
			AwardScence.Instance.LoadText("你获得了一些钱!", "金币袋", "袋中的钱币已存入你的手中");
		}
		else if (LV.Instance.FirstBootyEvent != null)
		{
			LV.Instance.FirstBootyEvent();
		}
		if (LV.Instance.CurrLvId % 10000 < GameManager.Instance.SelectedStone.AdventureLvNum)
		{
			GameManager.Instance.LocalPlayerSave.LastAdventureId = LV.Instance.CurrLvId + 1;
		}
		GameManager.Instance.AddNewPlant(LV.Instance.BootyPlant);
		if (CardREderer.transform.localScale.x > 0f)
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GetPlant, base.transform.position, isAll: true);
		}
		base.transform.GetComponent<Animator>().Play("BootyShine");
		AudioManager.Instance.StopBgAudio();
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.GameWin, base.transform.position, isAll: true);
		GameManager.Instance.SaveUserInfo();
		GameManager.Instance.SaveLvInfo(saveCurrLv: true);
		SkyManager.Instance.CollectAllSun();
		if (GameManager.Instance.isServer)
		{
			SynBooty synBooty = new SynBooty();
			synBooty.isSpawn = false;
			SocketServer.Instance.SendSynBooty(synBooty);
		}
	}

	public void FillScreen()
	{
		EffectPanel.Instance.WinGame(this);
	}

	public void StartShow()
	{
		base.transform.GetComponent<Animator>().Play("BootyNormal");
	}

	private void FlyGold()
	{
	}

	private IEnumerator DoFly(Vector3 pos)
	{
		float a = 1f;
		Vector3 direction = (pos - base.transform.position).normalized;
		while (Vector3.Distance(pos, base.transform.position) > 0.5f)
		{
			yield return new WaitForSeconds(0.02f);
			base.transform.Translate(direction * 0.1f);
			a += 0.01f;
			base.transform.localScale = new Vector3(a, a, 0f);
		}
	}

	private IEnumerator flash()
	{
		float a = 1f;
		while (isClicked)
		{
			if (a >= 0.5f)
			{
				while (a > 0.5f)
				{
					yield return new WaitForSeconds(0.04f);
					a -= 0.05f;
					CardREderer.color = new Color(a, a, a);
					MoneyREderer.color = new Color(a, a, a);
				}
			}
			else if (a <= 0.5f)
			{
				while (a < 0.95f)
				{
					yield return new WaitForSeconds(0.04f);
					a += 0.05f;
					CardREderer.color = new Color(a, a, a);
					MoneyREderer.color = new Color(a, a, a);
				}
			}
		}
		CardREderer.color = new Color(1f, 1f, 1f);
		MoneyREderer.color = new Color(1f, 1f, 1f);
	}

	public void DestroyThis()
	{
		LVManager.Instance.OnlyBooty = null;
		Object.Destroy(base.gameObject);
	}
}
