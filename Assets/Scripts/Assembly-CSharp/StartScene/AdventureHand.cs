using UnityEngine;

namespace StartScene
{
	public class AdventureHand : MonoBehaviour
	{
		public static AdventureHand Instance;

		public Transform Masks;

		public void StartPlay()
		{
			base.transform.localScale = new Vector3(2.2f, 2.2f, 1f);
			Masks.transform.localScale = new Vector3(1.43f, 1f, 1f);
			base.transform.GetComponent<Animator>().Play("anim", 0, 0f);
		}

		private void Awake()
		{
			Instance = this;
		}

		public void AnimEvent()
		{
			Masks.transform.localScale = Vector3.zero;
		}
	}
}
