using UnityEngine;
using UnityEngine.UI;

public class Coinbank : MonoBehaviour
{
	public static Coinbank Instance;

	private Text CoinbankText;

	private void Awake()
	{
		Instance = this;
		base.gameObject.SetActive(value: false);
		CoinbankText = base.transform.Find("Cointext").GetComponent<Text>();
	}

	public Vector3 GetCoinbankTextPos()
	{
		return CoinbankText.transform.position;
	}

	public void UpdateCoinbankNum(int Num)
	{
		CoinbankText.text = Num.ToString();
	}

	public void ShowCoinbank()
	{
		CancelInvoke();
		base.gameObject.SetActive(value: true);
		Invoke("HideCoinbank", 5f);
	}

	private void HideCoinbank()
	{
		base.gameObject.SetActive(value: false);
	}
}
