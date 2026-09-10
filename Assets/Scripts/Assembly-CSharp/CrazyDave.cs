using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrazyDave : MonoBehaviour
{
	private Animator animator;

	private bool IsSpeak;

	private int LongTalkNum;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void SpeakOver()
	{
		IsSpeak = false;
		animator.SetInteger("Change", 0);
	}

	public void StoreOpneInit()
	{
		LongTalkNum = 0;
		animator = GetComponent<Animator>();
		animator.Play("enter", 0, 0f);
	}

	public void StoreLongTalk()
	{
		if (IsSpeak)
		{
			return;
		}
		IsSpeak = true;
		animator.SetInteger("Change", Random.Range(1, 3));
		if (LongTalkNum == 0)
		{
			MyTool.RandomOne(new List<UnityAction>
			{
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CrazyDaveLong1, base.transform.position);
				},
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CrazyDaveLong2, base.transform.position);
				},
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CrazyDaveLong3, base.transform.position);
				}
			});
		}
		LongTalkNum++;
		if (LongTalkNum == 3)
		{
			LongTalkNum = 0;
		}
	}

	public void StoreShortTalk()
	{
		if (!IsSpeak)
		{
			IsSpeak = true;
			animator.SetInteger("Change", Random.Range(1, 3));
			MyTool.RandomOne(new List<UnityAction>
			{
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CrazyDaveShort1, base.transform.position);
				},
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CrazyDaveShort2, base.transform.position);
				},
				() =>
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.CrazyDaveShort3, base.transform.position);
				}
			});
		}
	}
}
