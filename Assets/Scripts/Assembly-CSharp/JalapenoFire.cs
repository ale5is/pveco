using UnityEngine;
using UnityEngine.Rendering;

public class JalapenoFire : MonoBehaviour
{
	private Animator animator;

	public void CreateInit(Vector2 pos, int sort)
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
		animator.GetComponent<SortingGroup>().sortingOrder = sort;
		base.transform.position = pos + new Vector2(0.6f, 0f);
		animator.Play("anim_flame", 0, 0f);
		animator.SetInteger("Change", 0);
	}

	public void DisAppear()
	{
		animator.SetInteger("Change", 1);
	}
}
