using UnityEngine;

public class ChangeUser : MonoBehaviour
{
	public Sprite Normal;

	public Sprite EnterGreen;

	public Animator clipController;

	public TextMesh NameText;

	public Collider2D Collider;

	public SpriteRenderer SignRe;

	public void AnimAction()
	{
		Collider.enabled = true;
	}

	public void PlayAnimation()
	{
		Collider.enabled = false;
		AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.woodSignRoll_in, base.transform.position, isAll: true);
		clipController.Play("anim_drop", 0, 0f);
	}

	private void OnMouseEnter()
	{
		if (!MyTool.IsPointerOverGameObject())
		{
			SignRe.sprite = EnterGreen;
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Bleep, base.transform.position, isAll: true);
		}
	}

	private void OnMouseExit()
	{
		SignRe.sprite = Normal;
	}

	private void OnMouseDown()
	{
		if (!GameManager.Instance.isOnline && !MyTool.IsPointerOverGameObject())
		{
			ChooseSave.Instance.transform.localScale = Vector3.one;
			ChooseSave.Instance.gameObject.SetActive(value: true);
			ChooseSave.Instance.LoadSavegroup();
			AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.ButtonClick, base.transform.position, isAll: true);
		}
	}
}
