using UnityEngine;

public class MapWall : MonoBehaviour
{
	public bool BlockLeft;

	public bool BlockRight;

	public bool BlockUp;

	public bool BlockDown;

	public bool IsPass(Vector2 dir)
	{
		if (BlockLeft && dir.x < 0f)
		{
			return false;
		}
		if (BlockRight && dir.x > 0f)
		{
			return false;
		}
		if (BlockUp && dir.y > 0f)
		{
			return false;
		}
		if (BlockDown && dir.y < 0f)
		{
			return false;
		}
		return true;
	}
}
