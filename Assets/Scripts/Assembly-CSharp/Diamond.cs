using UnityEngine;

public class Diamond : Allcoin
{
	protected override AudioClip sound => GameManager.Instance.AudioConf.ClickDiamond;

	protected override int money => 1000;

	protected override AudioClip dropSound => GameManager.Instance.AudioConf.Chime;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Diamond;
}
