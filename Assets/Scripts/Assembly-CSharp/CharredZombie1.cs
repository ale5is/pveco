using FTRuntime;
using UnityEngine;

public class CharredZombie1 : MonoBehaviour
{
	public SwfClipAsset Normal;

	public SwfClipAsset Catpult;

	public SwfClipAsset Digger;

	public SwfClipAsset Gargantuar;

	public SwfClipAsset Imp;

	public SwfClipAsset Zamboni;

	public Sprite Swampgar;

	public SwfClipController ClipController { get; private set; }

	public void InitCharred(int type, Vector2 pos, Vector2 offset, int sort, bool isLeft, Vector3 scale)
	{
		MapBase currMap = MapManager.Instance.GetCurrMap(pos);
		if (currMap != null)
		{
			base.transform.SetParent(currMap.transform);
		}
		base.transform.localScale = scale * 1.6f;
		base.transform.localScale = new Vector2(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		if (!isLeft)
		{
			base.transform.localScale = new Vector2(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		if (ClipController == null)
		{
			ClipController = base.transform.GetComponent<SwfClipController>();
			ClipController.clip.OnChangeCurrentFrameEvent += FrameChangeEvent;
		}
		if (!isLeft)
		{
			base.transform.position = pos + MyTool.ReverseX(offset);
		}
		else
		{
			base.transform.position = pos + offset;
		}
		ClipController.clip.sortingOrder = sort;
		ClipController.clip.clip = GetCharredType(type);
		if (type == 7)
		{
			ClipController.clip.NewSprite = Swampgar;
		}
		else
		{
			ClipController.clip.NewSprite = null;
		}
		base.transform.GetComponent<Renderer>().material.SetTexture("_EyeTex", null);
	}

	private void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == swfClip.frameCount - 1)
		{
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.CharredZombie1, base.gameObject);
		}
	}

	private SwfClipAsset GetCharredType(int type)
	{
		SwfClipAsset result = Normal;
		switch (type)
		{
		case 1:
			result = Normal;
			break;
		case 2:
			result = Catpult;
			break;
		case 3:
			result = Digger;
			break;
		case 4:
			result = Gargantuar;
			break;
		case 5:
			result = Imp;
			break;
		case 6:
			result = Zamboni;
			break;
		case 7:
			result = Gargantuar;
			break;
		}
		return result;
	}
}
