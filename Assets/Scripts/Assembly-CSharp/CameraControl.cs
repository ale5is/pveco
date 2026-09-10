using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraControl : MonoBehaviour
{
	public static CameraControl Instance;

	public MapBase CurrMap;

	private Coroutine ShakeCoroutine;

	private float velocityX;

	private float velocityY;

	private Coroutine MoveCoroutine;

	public bool InAcvment;

	private float scrollSpeed = 10f;

	private float minY = -65f;

	private float maxY = -42f;

	public float targetY;

	private float velocity;

	private bool isDragging;

	private Vector2 lastTouchPosition;

	private void Awake()
	{
		Instance = this;
	}

	public void MoveToAcvment()
	{
		MoveTo(new Vector2(0f, -42f), () =>
		{
			InAcvment = true;
			targetY = base.transform.position.y;
		});
	}

	private void Update()
	{
		if (!InAcvment || !(base.transform.position.y >= -66f) || !(base.transform.position.y <= -41f))
		{
			return;
		}
		float num = Input.GetAxis("Mouse ScrollWheel");
		if (Application.isMobilePlatform && Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			switch (touch.phase)
			{
			case TouchPhase.Began:
				isDragging = true;
				lastTouchPosition = touch.position;
				break;
			case TouchPhase.Moved:
				if (isDragging)
				{
					num = 0f - (touch.position.y - lastTouchPosition.y) * 0.002f;
					lastTouchPosition = touch.position;
				}
				break;
			case TouchPhase.Canceled:
				isDragging = false;
				break;
			}
		}
		if (num != 0f)
		{
			targetY += num * scrollSpeed;
			targetY = Mathf.Clamp(targetY, minY, maxY);
		}
		Vector3 position = base.transform.position;
		position.y = Mathf.SmoothDamp(base.transform.position.y, targetY, ref velocity, 0.2f);
		base.transform.position = position;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Vector3 vector = new Vector3(base.transform.position.x, minY, base.transform.position.z);
		Vector3 vector2 = new Vector3(base.transform.position.x, maxY, base.transform.position.z);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawSphere(vector, 0.5f);
		Gizmos.DrawSphere(vector2, 0.5f);
	}

	public void SetPosition(Vector2 pos)
	{
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		base.transform.position = new Vector3(pos.x, pos.y, -10f);
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
	}

	public void MoveForLVStart(UnityAction action)
	{
		Vector3 position = new Vector3(-3.5f, 0f, -10f);
		if (MapManager.Instance.GetCurrMap(base.transform.position) != null)
		{
			position = new Vector3(position.x, base.transform.position.y, -10f);
		}
		base.transform.position = position;
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		MoveCoroutine = StartCoroutine(DoMove(new Vector2(3.5f, base.transform.position.y), 0.6f, 0.35f, action));
	}

	public void MoveBackForLVStart(UnityAction action)
	{
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		MoveCoroutine = StartCoroutine(DoMove(new Vector2(-3.5f, base.transform.position.y), 0.6f, 0.35f, action));
	}

	public void MoveTo(Vector2 pos, UnityAction action)
	{
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
		}
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		MoveCoroutine = StartCoroutine(DoMove(pos, 0f, 0.15f, action));
	}

	private IEnumerator DoMove(Vector2 targetPos, float waitTime, float smoothTime, UnityAction action)
	{
		yield return new WaitForSeconds(waitTime);
		Vector3 target = new Vector3(targetPos.x, targetPos.y, -10f);
		do
		{
			yield return null;
			float x = Mathf.SmoothDamp(base.transform.position.x, target.x, ref velocityX, smoothTime);
			float y = Mathf.SmoothDamp(base.transform.position.y, target.y, ref velocityY, smoothTime);
			base.transform.position = new Vector3(x, y, -10f);
		}
		while (!(Vector2.Distance(target, base.transform.position) < 0.01f));
		base.transform.position = target;
		action?.Invoke();
		MoveCoroutine = null;
	}

	public bool GoOtherYard(int mapId)
	{
		if (mapId < 0 || MapManager.Instance.mapList.Count <= mapId)
		{
			return false;
		}
		if (MoveCoroutine != null)
		{
			return false;
		}
		base.transform.position = new Vector3(base.transform.position.x, MapManager.Instance.mapList[mapId].transform.position.y, -10f);
		CurrMap = MapManager.Instance.GetCurrMap(base.transform.position);
		Timetable.Instance.UpdateTempt(CurrMap);
		GobalEffManager.Instance.RefreshEffect();
		SkyManager.Instance.clickedSunNum = 0;
		if (ShakeCoroutine != null)
		{
			StopCoroutine(ShakeCoroutine);
		}
		return true;
	}

	public void ShakeCamera(Vector3 pos)
	{
		if (!(MapManager.Instance.GetCurrMap(pos) != CurrMap) && ShakeCoroutine == null)
		{
			ShakeCoroutine = StartCoroutine(Shake());
		}
	}

	private IEnumerator Shake()
	{
		float NormalX = base.transform.position.x + 0.08f;
		float NormalY = base.transform.position.y + 0.08f;
		while (base.transform.position.y < NormalY)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, new Vector3(NormalX, NormalY, base.transform.position.z), 40f * Time.deltaTime);
			yield return null;
		}
		NormalX = base.transform.position.x - 0.08f;
		NormalY = base.transform.position.y - 0.08f;
		while (base.transform.position.y > NormalY)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, new Vector3(NormalX, NormalY, base.transform.position.z), 40f * Time.deltaTime);
			yield return null;
		}
		ShakeCoroutine = null;
	}
}
