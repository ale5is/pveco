using UnityEngine;
using UnityEngine.UI;

public class MapInfoIcon : MonoBehaviour
{
	public Sprite YesSprite;

	public Sprite NoSprite;

	public Image IconImage;

	public Image YesImage;

	public void CreateInit(Sprite sprite, bool isYes)
	{
		IconImage.sprite = sprite;
		if (isYes)
		{
			YesImage.sprite = YesSprite;
		}
		else
		{
			YesImage.sprite = NoSprite;
		}
	}
}
