using UnityEngine;
using UnityEngine.Rendering;

public abstract class CharredZombie : MonoBehaviour
{
	protected Animator animator;

	protected abstract GameObject Prefab { get; }

	public void CreateInit(Vector2 pos, Vector2 offset, int sort, bool isLeft, Vector3 scale, int SpCode = 0)
	{
		animator = base.transform.Find("Animation").GetComponent<Animator>();
		base.transform.localScale = scale;
		base.transform.localScale = new Vector2(Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y);
		if (!isLeft)
		{
			base.transform.localScale = new Vector2(0f - base.transform.localScale.x, base.transform.localScale.y);
		}
		if (!isLeft)
		{
			base.transform.position = pos + MyTool.ReverseX(offset);
		}
		else
		{
			base.transform.position = pos + offset;
		}
		animator.GetComponent<SortingGroup>().sortingOrder = sort;
		OwnerInit(SpCode);
	}

	protected virtual void OwnerInit(int SpCode)
	{
	}

	private void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
		{
			PoolManager.Instance.PushObj(Prefab, base.gameObject);
		}
	}
}
