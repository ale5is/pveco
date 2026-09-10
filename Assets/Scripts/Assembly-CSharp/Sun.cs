using System.Collections;
using System.Collections.Generic;
using SocketSave;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Sun : MonoBehaviour
{
	public int OnlineSunId;

	public string OwnerPlayer;

	private float downTargetPosY;

	private bool isFromSky;

	private float Sunnum;

	private SphereCollider sphereCollider;

	private bool isClicked;

	private bool isTombAbsorb;

	private int AbsorbNum;

	private SunType sunType;

	public List<Sprite> SunSprites = new List<Sprite>();

	public List<Sprite> MoonSprites = new List<Sprite>();

	public List<Sprite> RedSprites = new List<Sprite>();

	public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

	public Light2D light2d;

	private Coroutine FlyCoroutine;

	public bool CanGet
	{
		get
		{
			if (!IsClicked)
			{
				return sphereCollider.enabled;
			}
			return false;
		}
	}

	public bool IsClicked
	{
		get
		{
			return isClicked;
		}
		private set
		{
			isClicked = value;
		}
	}

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
	}

	private void Update()
	{
		if (isFromSky && !IsClicked && !(base.transform.position.y <= downTargetPosY))
		{
			base.transform.Translate(Vector3.down * Time.deltaTime);
		}
	}

	public void OnMouseOver()
	{
		if (MyTool.IsPointerOverGameObject() || SpectatorList.Instance.LocalIsSpectator)
		{
			return;
		}
		if (isTombAbsorb)
		{
			if (!Input.GetMouseButtonUp(0))
			{
				return;
			}
			AbsorbNum++;
			if (AbsorbNum > 1)
			{
				if (!IsClicked)
				{
					StatsManager.Instance.AddStatsNum(StatsEnum.ClickSunNum);
				}
				CollectSun();
			}
		}
		else
		{
			if (!IsClicked)
			{
				StatsManager.Instance.AddStatsNum(StatsEnum.ClickSunNum);
			}
			CollectSun();
		}
	}

	public void OnlineSyn(ClickedSun sun)
	{
		if (sun.FlyPos)
		{
			TombFlyToPosDes(sun.pos, null);
		}
		else
		{
			CollectSun(synClient: true);
		}
	}

	public void CollectSun(bool synClient = false)
	{
		if (IsClicked && !synClient)
		{
			return;
		}
		if (!synClient && GameManager.Instance.isClient)
		{
			IsClicked = true;
			ClickedSun clickedSun = new ClickedSun();
			clickedSun.OnlineSunId = OnlineSunId;
			SocketClient.Instance.ClickedSun(clickedSun);
			return;
		}
		if (GameManager.Instance.isServer)
		{
			if (IsClicked)
			{
				return;
			}
			ClickedSun clickedSun2 = new ClickedSun();
			clickedSun2.OnlineSunId = OnlineSunId;
			SocketServer.Instance.ClickedSun(clickedSun2);
		}
		SkyManager.Instance.clickedSunNum++;
		isFromSky = false;
		StopAllCoroutines();
		Vector3 vector = default;
		if (IsSunType())
		{
			vector = SeedBank.Instance.SunPos.transform.position;
		}
		else if (sunType == SunType.Moon)
		{
			vector = SeedBank.Instance.MoonPos.transform.position;
		}
		if (IsSunType() && !SeedBank.Instance.NeedSummonSun)
		{
			vector = base.transform.position;
		}
		else if (sunType == SunType.Moon && !SeedBank.Instance.NeedSummonMoon)
		{
			vector = base.transform.position;
		}
		if (LV.Instance.CurrLVType == LVType.PvP && (OwnerPlayer == null || !PvPSelector.Instance.IsSameTeam(OwnerPlayer)) && OwnerPlayer != null)
		{
			vector = base.transform.position;
		}
		vector = new Vector3(vector.x, vector.y, 0f);
		if (MapManager.Instance.GetCurrMap(base.transform.position) == CameraControl.Instance.CurrMap)
		{
			FlyAnimation(vector);
		}
		else
		{
			DestroySun();
		}
		PlayerManager.Instance.AddSunNum(Sunnum, IsSunType(), OwnerPlayer);
		if (SkyManager.Instance.clickedSunNum < 3)
		{
			SunSound();
		}
		else if (Random.Range(0, 3) > 0)
		{
			SunSound();
		}
		IsClicked = true;
	}

	private bool IsSunType()
	{
		if (sunType != SunType.Normal)
		{
			return sunType == SunType.Red;
		}
		return true;
	}

	public void TombFlyToPosDes(Vector3 pos, UnityAction action)
	{
		if (!IsClicked && !isTombAbsorb && (GameManager.Instance.isClient || !(MapManager.Instance.GetCurrMap(base.transform.position) == null)))
		{
			isTombAbsorb = true;
			FlyAnimation(pos, 0.5f, action);
			if (GameManager.Instance.isServer && !IsClicked)
			{
				ClickedSun clickedSun = new ClickedSun();
				clickedSun.OnlineSunId = OnlineSunId;
				clickedSun.FlyPos = true;
				clickedSun.pos = pos;
				SocketServer.Instance.ClickedSun(clickedSun);
			}
		}
	}

	private void SunSound()
	{
		if (IsSunType())
		{
			if (Random.Range(0, 2) > 0)
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.SunClick, base.transform.position);
			}
			else
			{
				AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.SunClickHigh, base.transform.position);
			}
		}
		else
		{
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.MoonClick, base.transform.position);
		}
	}

	private void SetAllColor(Color color)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			renderers[i].color = color;
		}
	}

	private void InitForAll(SunType type, Vector3 SpawnPos)
	{
		AbsorbNum = 0;
		isTombAbsorb = false;
		sunType = type;
		IsClicked = false;
		for (int i = 0; i < renderers.Count; i++)
		{
			switch (type)
			{
			case SunType.Normal:
				renderers[i].sprite = SunSprites[i];
				break;
			case SunType.Moon:
				renderers[i].sprite = MoonSprites[i];
				break;
			case SunType.Red:
				renderers[i].sprite = RedSprites[i];
				break;
			}
		}
		base.transform.position = SpawnPos;
		SetAllColor(Color.white);
	}

	public void InitForSky(float downTargetPosY, Vector3 SpawnPos, SunType type)
	{
		InitForAll(type, SpawnPos);
		OwnerPlayer = null;
		Sunnum = 25f;
		sphereCollider.radius = 0.7f;
		sphereCollider.enabled = true;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		light2d.pointLightOuterRadius = 1.6f;
		this.downTargetPosY = downTargetPosY;
		base.transform.position = SpawnPos;
		isFromSky = true;
		if (!GameManager.Instance.isClient && SkyManager.Instance.SunAutoCollect && sunType != SunType.Red)
		{
			CollectSun();
		}
		else
		{
			Invoke("TimeOutDestory", 20f);
		}
	}

	public void InitForPlant(Vector2 pos, float sum, SunType type, string Player)
	{
		InitForAll(type, pos);
		OwnerPlayer = Player;
		Sunnum = sum;
		float num = Mathf.Abs(sum) * 1f / 25f;
		sphereCollider.radius = 0.7f * (Mathf.Abs(sum) * 1f / 25f);
		if (sphereCollider.radius < 0.6f)
		{
			sphereCollider.radius = 0.6f;
		}
		base.transform.localScale = new Vector3(num, num, 1f);
		light2d.pointLightOuterRadius = 1.6f * num;
		num = 1f / num;
		sphereCollider.radius *= num;
		isFromSky = false;
		StartCoroutine(DoJump());
		Invoke("TimeOutDestory", 14f);
	}

	private IEnumerator DoJump()
	{
		bool num = Random.Range(0, 2) == 0;
		Vector3 startPos = base.transform.position;
		float x = Random.Range(0.2f, 0.8f);
		if (num)
		{
			x = 0f - x;
		}
		float speed = 0f;
		float scale = base.transform.localScale.y;
		base.transform.localScale = new Vector3(0.1f, 0.1f);
		sphereCollider.enabled = false;
		while (base.transform.localScale.y < scale)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				base.transform.localScale += new Vector3(5f, 5f) * Time.deltaTime;
				speed += 0.04f;
				base.transform.Translate(new Vector3(x, speed) * Time.deltaTime);
			}
		}
		sphereCollider.enabled = true;
		base.transform.localScale = new Vector3(scale, scale);
		if (!GameManager.Instance.isClient && SkyManager.Instance.SunAutoCollect && sunType != SunType.Red)
		{
			CollectSun();
		}
		while ((double)base.transform.position.y >= (double)startPos.y - 0.2)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				speed -= 0.04f;
				base.transform.Translate(new Vector3(x, speed) * Time.deltaTime);
			}
		}
	}

	private void FlyAnimation(Vector3 pos, float time = 0.4f, UnityAction action = null)
	{
		StopAllCoroutines();
		if (FlyCoroutine != null)
		{
			StopCoroutine(FlyCoroutine);
		}
		FlyCoroutine = StartCoroutine(DoFly(pos, time, action));
	}

	private IEnumerator DoFly(Vector3 pos, float time, UnityAction action)
	{
		Vector3 velocity = default;
		while (Vector3.Distance(pos, base.transform.position) > 0.1f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				base.transform.position = Vector3.SmoothDamp(base.transform.position, pos, ref velocity, time);
			}
		}
		SkyManager.Instance.clickedSunNum--;
		FlyCoroutine = null;
		action?.Invoke();
		StartCoroutine(FadeToDestroy());
	}

	private void TimeOutDestory()
	{
		if (!isTombAbsorb)
		{
			if (!GameManager.Instance.isClient && SkyManager.Instance.SlowSunAutoCollect && sunType != SunType.Red)
			{
				CollectSun();
			}
			else if (!IsClicked)
			{
				StartCoroutine(FadeToDestroy());
			}
		}
	}

	private IEnumerator FadeToDestroy()
	{
		IsClicked = true;
		float alp = 1f;
		while (alp > 0f)
		{
			yield return null;
			if (Time.timeScale > 0f)
			{
				alp -= 3f * Time.deltaTime;
				SetAllColor(new Color(1f, 1f, 1f, alp));
			}
		}
		DestroySun();
	}

	public void DestroySun()
	{
		StopAllCoroutines();
		CancelInvoke();
		SkyManager.Instance.RemoveSun(this);
		PoolManager.Instance.PushObj(GameManager.Instance.GameConf.Sun, base.gameObject);
	}
}
