public class PoolCleaner : LawnMower
{
	protected override bool CanInWater => true;

	protected override void InWaterChangeEvent()
	{
		if (base.InWater)
		{
			animator.Play("water");
		}
		else
		{
			animator.Play("PoolCleaner");
		}
	}

	protected override void KillZombieEvent()
	{
		animator.SetInteger("Change", 1);
	}

	protected override void LaunchEvent()
	{
		eFAudio = AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Poolcleaner, base.transform.position);
	}

	protected override void UpdateEvent()
	{
		if (IsRun && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0f)
		{
			animator.SetInteger("Change", 0);
		}
	}
}
