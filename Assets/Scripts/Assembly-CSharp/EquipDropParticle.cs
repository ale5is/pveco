using UnityEngine;
using UnityEngine.Rendering;

public class EquipDropParticle : MonoBehaviour
{
	public ParticleSystem Ps;

	private ParticleSystem.MainModule PsMain;

	private void Start()
	{
		PsMain = Ps.main;
		PsMain.stopAction = ParticleSystemStopAction.Callback;
	}

	public void InitCreate(Vector2 pos, int sortorder, Sprite equip)
	{
		Ps.textureSheetAnimation.SetSprite(0, equip);
		base.transform.position = pos;
		base.transform.GetComponent<SortingGroup>().sortingOrder = sortorder;
	}

	private void OnParticleSystemStopped()
	{
	}
}
