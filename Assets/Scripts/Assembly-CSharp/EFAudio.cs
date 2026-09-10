using UnityEngine;

public class EFAudio : MonoBehaviour
{
	private AudioSource audioSource;

	public string PlayClipName;

	public void Init(AudioClip clip, float Volume)
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.PlayOneShot(clip);
		audioSource.volume = Volume;
		PlayClipName = clip.name;
	}

	public void Close()
	{
		AudioManager.Instance.RemoveEFAudio(this, PlayClipName);
		PoolManager.Instance.PushObj(GameManager.Instance.AudioConf.EFAudio, base.gameObject);
	}

	private void Update()
	{
		if (!audioSource.isPlaying)
		{
			Close();
		}
	}
}
