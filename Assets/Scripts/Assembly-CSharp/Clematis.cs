using System.Collections;
using SocketSave;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Clematis : PlantBase
{
	public Light2D light2d;

	public ParticleSystem Particle;

	public Renderer SunRender;

	public Sprite Flower;

	public Sprite BlueFlower;

	public SpriteRenderer Flower1;

	public SpriteRenderer Flower2;

	public SpriteRenderer GlowFlower1;

	public SpriteRenderer GlowFlower2;

	private int EnergyNum;

	private Vector3 creatBulletOffsetPos = new Vector2(0.6f, 0.2f);

	public override float MaxHp => 300f;

	public override bool CanPlaceOnWaterCarry => false;

	protected override int attackValue => 900;

	protected override void OnInitForAll()
	{
		EnergyNum = 0;
		SetFlower(0);
		SunRender.gameObject.SetActive(value: false);
	}

	public void StruckThis()
	{
		if (GameManager.Instance.isClient)
		{
			return;
		}
		if (EnergyNum == 0)
		{
			int num = Random.Range(0, 2);
			if (num > 0)
			{
				SetFlower(1);
			}
			else
			{
				SetFlower(2);
			}
			light2d.enabled = true;
			Particle.Play();
			ServerSendSyn(1, num);
		}
		else if (EnergyNum == 1)
		{
			SetFlower(4);
			ServerSendSyn(2);
		}
		EnergyNum++;
		if (EnergyNum > 2)
		{
			Dead();
		}
	}

	private void SetFlower(int type)
	{
		switch (type)
		{
		case 0:
			Particle.Stop();
			light2d.enabled = false;
			Flower1.sprite = Flower;
			Flower2.sprite = Flower;
			GlowFlower1.enabled = false;
			GlowFlower2.enabled = false;
			break;
		case 1:
			Flower1.sprite = BlueFlower;
			Flower2.sprite = Flower;
			GlowFlower1.enabled = true;
			GlowFlower2.enabled = false;
			break;
		case 2:
			Flower1.sprite = Flower;
			Flower2.sprite = BlueFlower;
			GlowFlower1.enabled = false;
			GlowFlower2.enabled = true;
			break;
		default:
			Flower1.sprite = BlueFlower;
			Flower2.sprite = BlueFlower;
			GlowFlower1.enabled = true;
			GlowFlower2.enabled = true;
			break;
		}
	}

	public override void SpecialAnimEvent1()
	{
		CreateBall();
		SetAnimChange(12);
	}

	private void CreateBall()
	{
		if (!NormalDontAttackCondition())
		{
			SetFlower(0);
			if (Random.Range(0, 2) == 1)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Throw2, base.transform.position);
			}
			EnergyBall component = PoolManager.Instance.GetObj(GameManager.Instance.GameConf.EnergyBall).GetComponent<EnergyBall>();
			component.transform.SetParent(null);
			if (base.IsFacingLeft)
			{
				component.Init(GetAttackValue(), base.transform.position + MyTool.ReverseX(creatBulletOffsetPos), base.currGrid.Point.y, Vector2.left, GetBulletSortOrder(), isHypno);
			}
			else
			{
				component.Init(GetAttackValue(), base.transform.position + creatBulletOffsetPos, base.currGrid.Point.y, Vector2.right, GetBulletSortOrder(), isHypno);
			}
		}
	}

	private void OnMouseOver()
	{
		if (!MyTool.IsPointerOverGameObject() && !SpectatorList.Instance.LocalIsSpectator && (LV.Instance.CurrLVType != LVType.PvP || PvPSelector.Instance.IsSameTeam(PlacePlayer)) && base.currGrid != null && Input.GetMouseButtonDown(0) && EnergyNum >= 2 && PlayerManager.Instance.GetSunNum(isSun: true, PlacePlayer) >= 50f && !isSleeping && LVManager.Instance.GameIsStart)
		{
			EnergyNum = 0;
			StartCoroutine(SunAnim());
			if (GameManager.Instance.isClient)
			{
				SynItem synItem = new SynItem();
				synItem.OnlineId = OnlineId;
				synItem.Type = SynItemType.Plant;
				synItem.SynCode[0] = 1;
				SocketClient.Instance.SendSynBag(synItem);
			}
			else
			{
				PlayerManager.Instance.AddSunNum(-50f, isSun: true, PlacePlayer);
			}
			ServerSendSyn(0);
		}
	}

	protected override void OnlineSyn(SynItem syn)
	{
		if (syn.SynCode[1] == 0)
		{
			StartCoroutine(SunAnim());
		}
		else if (syn.SynCode[1] == 1)
		{
			EnergyNum = 1;
			if (syn.SynCode[2] > 0)
			{
				SetFlower(1);
			}
			else
			{
				SetFlower(2);
			}
			light2d.enabled = true;
			Particle.Play();
		}
		else if (syn.SynCode[1] == 2)
		{
			EnergyNum = 2;
			SetFlower(4);
		}
	}

	private IEnumerator SunAnim()
	{
		SunRender.gameObject.SetActive(value: true);
		SunRender.transform.localPosition = new Vector3(0f, 1f);
		float alp = 1f;
		while (alp > 0.05f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				alp -= 5f * Time.deltaTime;
				SunRender.transform.position += new Vector3(0f, -2f * Time.deltaTime);
				SunRender.material.SetColor("_Color", new Color(1f, 1f, 1f, alp));
			}
		}
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.SunClick, base.transform.position);
		SunRender.gameObject.SetActive(value: false);
		SetAnimChange(11);
	}
}
