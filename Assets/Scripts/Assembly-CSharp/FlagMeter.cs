using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.UI;

public class FlagMeter : MonoBehaviour
{
	public static FlagMeter Instance;

	private Image Head;

	private Image MaskImg;

	private int Allweight;

	private List<LVFlag> FlagList = new List<LVFlag>();

	public RectTransform SizeMark1;

	public RectTransform SizeMark2;

	public RectTransform SizeMark3;

	public Text name1;

	public Text name2;

	public Text name3;

	public Text LvSpText1;

	public Text LvSpText2;

	public Text LvSpText3;

	public Text NextSubLvText;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Head = base.transform.Find("Head").GetComponent<Image>();
		MaskImg = base.transform.Find("FullMeter").GetComponent<Image>();
		Head.transform.localPosition = SizeMark2.transform.localPosition;
		MaskImg.fillAmount = 0f;
	}

	public void SetLvlName(string name, bool isEasy)
	{
		if (isEasy)
		{
			name2.color = new Color32(byte.MaxValue, 236, 0, byte.MaxValue);
		}
		else
		{
			name2.color = new Color32(byte.MaxValue, 74, 0, byte.MaxValue);
		}
		name1.text = name;
		name2.text = name;
		name3.text = name;
		LvSpText1.text = "";
		LvSpText2.text = "";
		LvSpText3.text = "";
	}

	private void ResetFlagMeter()
	{
		SetRestTimeText(null);
		Head.transform.localScale = Vector3.one;
		Head.transform.localPosition = SizeMark2.transform.localPosition;
		MaskImg.fillAmount = 0f;
		for (int i = 0; i < FlagList.Count; i++)
		{
			FlagList[i].Destroy();
		}
		FlagList.Clear();
	}

	public void UpdateHead(int weight)
	{
		if (!GameManager.Instance.isClient && !LVManager.Instance.isBigWave)
		{
			float num = Mathf.Abs(SizeMark1.transform.position.x - SizeMark2.transform.position.x) / (float)Allweight;
			Head.transform.position += new Vector3((0f - num) * (float)weight, 0f, 0f);
			if (Head.transform.position.x < SizeMark1.transform.position.x)
			{
				Head.transform.position = new Vector3(SizeMark1.transform.position.x, Head.transform.position.y);
			}
			MaskImg.fillAmount += 1f / (float)Allweight * (float)weight;
			if (GameManager.Instance.isServer)
			{
				FlagMeterSyn flagMeterSyn = new FlagMeterSyn();
				flagMeterSyn.Type = 1;
				flagMeterSyn.FillA = MaskImg.fillAmount;
				SocketServer.Instance.SynFlagMeter(flagMeterSyn);
			}
		}
	}

	public void IZUpdate(int curr, int allNum)
	{
		if (!GameManager.Instance.isClient)
		{
			LvSpText1.text = $"食脑{curr}/{allNum}";
			LvSpText2.text = LvSpText1.text;
			LvSpText3.text = LvSpText1.text;
			MaskImg.fillAmount = 1f / (float)allNum * (float)curr;
			if (GameManager.Instance.isServer)
			{
				FlagMeterSyn flagMeterSyn = new FlagMeterSyn();
				flagMeterSyn.Type = 1;
				flagMeterSyn.FillA = MaskImg.fillAmount;
				flagMeterSyn.SpTxt = LvSpText1.text;
				SocketServer.Instance.SynFlagMeter(flagMeterSyn);
			}
		}
	}

	public void SetRestTimeText(string text)
	{
		if (text == null || text == "")
		{
			NextSubLvText.transform.parent.localScale = Vector3.zero;
		}
		else
		{
			NextSubLvText.text = text;
			NextSubLvText.transform.parent.localScale = Vector3.one;
		}
		if (GameManager.Instance.isServer)
		{
			FlagMeterSyn flagMeterSyn = new FlagMeterSyn();
			flagMeterSyn.Type = 3;
			flagMeterSyn.SpTxt = LvSpText1.text;
			SocketServer.Instance.SynFlagMeter(flagMeterSyn);
		}
	}

	public void ClientSyn(FlagMeterSyn syn)
	{
		if (syn.Type == 1)
		{
			MaskImg.fillAmount = syn.FillA;
			LvSpText1.text = syn.SpTxt;
			LvSpText2.text = LvSpText1.text;
			LvSpText3.text = LvSpText1.text;
			float num = Mathf.Abs(SizeMark1.transform.position.x - SizeMark2.transform.position.x);
			Head.transform.position = new Vector3(SizeMark2.transform.position.x - num * syn.FillA, Head.transform.position.y);
			if (Head.transform.position.x < SizeMark1.transform.position.x)
			{
				Head.transform.position = new Vector3(SizeMark1.transform.position.x, Head.transform.position.y);
			}
		}
		else if (syn.Type == 2)
		{
			ResetFlagMeter();
			CreateFlag(syn.FlagPos);
		}
		else if (syn.Type == 3)
		{
			SetRestTimeText(syn.SpTxt);
		}
	}

	public void CreateFlag(int allWeight)
	{
		ResetFlagMeter();
		Allweight = allWeight;
		base.transform.localScale = Vector3.one;
		name3.rectTransform.position = SizeMark3.position;
		if (LV.Instance.CurrLVType == LVType.IZombie)
		{
			Head.transform.localScale = Vector3.zero;
			return;
		}
		if (Allweight == 0)
		{
			name3.transform.position = base.transform.position;
			base.transform.localScale = Vector3.zero;
		}
		if (Allweight == 0)
		{
			return;
		}
		int num = 0;
		float num2 = 0f;
		List<float> list = new List<float>();
		for (int i = 0; i < LV.Instance.Weights[0].Count; i++)
		{
			if (i == LV.Instance.BigWaveNum[num])
			{
				float item = num2 / (float)Allweight;
				list.Add(item);
				num++;
			}
			else
			{
				for (int j = 0; j < LV.Instance.Weights.Count; j++)
				{
					num2 += (float)LV.Instance.Weights[j][i];
				}
			}
		}
		CreateFlag(list);
		if (GameManager.Instance.isServer)
		{
			FlagMeterSyn flagMeterSyn = new FlagMeterSyn();
			flagMeterSyn.Type = 2;
			flagMeterSyn.FlagPos = list;
			SocketServer.Instance.SynFlagMeter(flagMeterSyn);
		}
	}

	public void CreateFlag(List<float> FlagPos)
	{
		float num = Mathf.Abs(SizeMark1.transform.position.x - SizeMark2.transform.position.x);
		for (int i = 0; i < FlagPos.Count; i++)
		{
			LVFlag component = Object.Instantiate(GameManager.Instance.GameConf.LVFlag).GetComponent<LVFlag>();
			FlagList.Add(component);
			component.transform.SetParent(base.transform);
			component.CreateInit(SizeMark1.rect);
			component.transform.position = new Vector3(SizeMark2.transform.position.x - num * FlagPos[i], base.transform.position.y, 0f);
			component.transform.SetAsLastSibling();
		}
		Head.transform.SetSiblingIndex(Head.transform.parent.childCount - 1);
		for (int j = 0; j < FlagList.Count; j++)
		{
			FlagList[j].GetComponent<RectTransform>().localScale = Vector3.one;
		}
		NextSubLvText.transform.parent.SetSiblingIndex(Head.transform.parent.childCount - 1);
	}

	public void FlagRise(int index)
	{
		FlagList[index].GoRise(SizeMark2.transform.localPosition.x);
	}
}
