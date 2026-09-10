using FTRuntime;
using UnityEngine;

public class EFObj : MonoBehaviour
{
	private Renderer REnderer;

	private SwfClipController clipController;

	public SwfClipAsset DoomCloud;

	public SwfClipAsset Fire;

	public SwfClipAsset Spalsh;

	public void CreateInit(Vector2 pos, int Type, Color color, int sort)
	{
		base.transform.position = pos;
		if (clipController == null)
		{
			REnderer = base.transform.GetComponent<Renderer>();
			clipController = base.transform.GetComponent<SwfClipController>();
			clipController.clip.OnChangeCurrentFrameEvent += FrameChangeEvent;
		}
		REnderer.material.SetColor("_Color", color);
		clipController.clip.sortingOrder = sort;
		switch (Type)
		{
		case 1:
			clipController.clip.clip = DoomCloud;
			break;
		case 2:
			clipController.clip.clip = Fire;
			break;
		case 3:
			clipController.clip.clip = Spalsh;
			REnderer.material.SetColor("_Color", MapManager.Instance.GetCurrMap(base.transform.position).SplashColor);
			break;
		}
	}

	private void FrameChangeEvent(SwfClip swfClip)
	{
		if (swfClip.currentFrame == swfClip.frameCount - 1)
		{
			PoolManager.Instance.PushObj(GameManager.Instance.GameConf.EFObj, base.gameObject);
		}
	}
}
