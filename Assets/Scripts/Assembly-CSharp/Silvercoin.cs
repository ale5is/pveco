using UnityEngine;

public class Silvercoin : Allcoin
{
	protected override AudioClip sound => GameManager.Instance.AudioConf.ClickCoin;

	protected override int money => 10;

	protected override AudioClip dropSound => GameManager.Instance.AudioConf.DropCoin;

	protected override GameObject Prefab => GameManager.Instance.GameConf.Silvercoin;
}
