using UnityEngine;

public class PlantAnimEvent : MonoBehaviour
{
	private PlantBase plant;

	public bool EventDisable;

	private void Start()
	{
		plant = base.transform.parent.GetComponent<PlantBase>();
	}

	public void WaitAnim()
	{
		if (!EventDisable)
		{
			plant.WaitAnim();
		}
	}

	public void SpecialEvent1()
	{
		if (!EventDisable)
		{
			plant.SpecialAnimEvent1();
		}
	}

	public void SpecialEvent2()
	{
		if (!EventDisable)
		{
			plant.SpecialAnimEvent2();
		}
	}

	public void SpecialEvent3()
	{
		if (!EventDisable)
		{
			plant.SpecialAnimEvent3();
		}
	}
}
