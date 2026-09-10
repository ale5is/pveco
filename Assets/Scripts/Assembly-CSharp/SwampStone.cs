using UnityEngine;

public class SwampStone : MonoBehaviour
{
	public TextMesh upText;

	public TextMesh downText;

	public SpriteRenderer time1;

	public SpriteRenderer time2;

	public SpriteRenderer time3;

	public SpriteRenderer time4;

	private int GetNum;

	private int NormalZombieNum;

	private int NormalTime;

	private int zombieNum;

	private int time;

	public void StartInit()
	{
		GetNum = 0;
		NormalTime = 50;
		NormalZombieNum = 3;
		ResetTime();
	}

	private void ResetTime()
	{
		zombieNum = NormalZombieNum;
		time = NormalTime;
		upText.text = zombieNum.ToString();
		downText.text = zombieNum.ToString();
		time1.gameObject.SetActive(value: true);
		time2.gameObject.SetActive(value: true);
		time3.gameObject.SetActive(value: true);
		time4.gameObject.SetActive(value: true);
	}

	private void GiveAward()
	{
		GetNum++;
		int num = ((NormalZombieNum > 8) ? 10 : (NormalZombieNum + 2));
		for (int i = 0; i < num; i++)
		{
			float downY = -3.14f + base.transform.position.y;
			float x = Random.Range(1.5f, 4f);
			SkyManager.Instance.CreateSkySun(new Vector3(x, 7.2f + base.transform.position.y), downY, SunType.Normal);
		}
		NormalTime -= GetNum;
		NormalZombieNum += GetNum;
		if (NormalTime < 4)
		{
			NormalTime = 4;
		}
		if (NormalZombieNum > 15)
		{
			NormalZombieNum = 15;
		}
		ResetTime();
	}

	public void TimeChange()
	{
		time--;
		int num = NormalTime / 5;
		if (time < num * 4)
		{
			time4.gameObject.SetActive(value: false);
		}
		if (time < num * 3)
		{
			time3.gameObject.SetActive(value: false);
		}
		if (time < num * 2)
		{
			time2.gameObject.SetActive(value: false);
		}
		if (time < num)
		{
			time1.gameObject.SetActive(value: false);
		}
		if (time == 0)
		{
			ResetTime();
		}
	}

	public void ZombieDead(ZombieBase zombie)
	{
		if (LVManager.Instance.GameIsStart)
		{
			zombieNum--;
			upText.text = zombieNum.ToString();
			downText.text = zombieNum.ToString();
			if (zombieNum == 0)
			{
				GiveAward();
			}
		}
	}
}
