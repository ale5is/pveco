using UnityEngine;
using UnityEngine.Rendering;

public class ArmDropParticle : MonoBehaviour
{
	public ParticleSystem Ps;

	private ParticleSystem.MainModule PsMain;

	public void InitCreate(Vector2 pos, int sortorder, Sprite head, float scale)
	{
		PsMain = Ps.main;
		PsMain.stopAction = ParticleSystemStopAction.Callback;
		PsMain.startSize = scale;
		Ps.textureSheetAnimation.SetSprite(0, head);
		base.transform.position = pos;
		base.transform.GetComponent<SortingGroup>().sortingOrder = sortorder + 1;
	}

	private void OnParticleSystemStopped()
	{
	}
}
