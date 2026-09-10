public class Marigold : PlantBase
{
	private float createCoinTime = 24f;

	public override float MaxHp => 300f;

	protected override void OnInitForPlace()
	{
		StartActionCD(createCoinTime, isOver: false);
	}

	protected override bool DoAction()
	{
		InstantiateCoin();
		return true;
	}

	private void InstantiateCoin()
	{
		if (!LVManager.Instance.LvSpawnisOver && NormalProduceCondition())
		{
			LvItemManager.Instance.DropCoin(base.transform.position, AlwaysDrop: true, notDiamond: true);
		}
	}
}
