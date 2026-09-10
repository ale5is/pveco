using UnityEngine;

public class Pot : PlantBase
{
	public override float MaxHp => 300f;

	public override bool CanCarryOtherPlant => true;

	protected override Vector2 offSet => new Vector2(0f, -0.05f);

	public override Vector2 CarryOffset => new Vector2(0f, 0.3f);

	public override bool CanPlaceOnHardGround => true;

	public override bool CanPlaceOnPuddle => true;

	public override bool SnowHurt => false;

	public override bool IsLowPlant => true;
}
