using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CobCannonTarget : MonoBehaviour
{
	public static CobCannonTarget Instance;

	private PlantBase plant;

	private bool isUsing;

	private UnityAction<Vector2> shootAction;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		base.transform.GetComponent<SpriteRenderer>().enabled = false;
	}

	private void Update()
	{
		if (isUsing)
		{
			if (plant.Hp <= 0f)
			{
				StopAim();
			}
			Vector3 mousePosition = Input.mousePosition;
			Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y));
			position.z = 0f;
			base.transform.position = position;
		}
	}

	public void StartAim(PlantBase cobCannon, UnityAction<Vector2> ShootAction)
	{
		shootAction = ShootAction;
		plant = cobCannon;
		isUsing = true;
		base.transform.GetComponent<SpriteRenderer>().enabled = true;
	}

	public void StopAim()
	{
		plant = null;
		isUsing = false;
		base.transform.GetComponent<SpriteRenderer>().enabled = false;
		base.transform.position = new Vector3(0f, 10f);
	}

	private void OnMouseOver()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (shootAction != null)
			{
				shootAction(base.transform.position);
			}
			StopAim();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			StopAim();
		}
	}
}
