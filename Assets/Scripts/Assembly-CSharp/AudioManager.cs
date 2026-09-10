using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	private AudioSource audioSource;

	public AudioSource windAudio;

	public AudioSource windAudio2;

	public AudioSource weatherAudio;

	public AudioSource weatherAudio2;

	private List<EFAudio> CurrEFAudios = new List<EFAudio>();

	private Dictionary<string, int> EfAudioNum = new Dictionary<string, int>();

	public float BgmVolume = 1f;

	public float SoundVolume = 1f;

	private Coroutine BgmFadeCoroutine;

	private Coroutine WindCoroutine;

	private Coroutine WindMixCoroutine;

	private Coroutine WeatherCoroutine;

	private Coroutine WeatherMixCoroutine;

	private AudioSource currWindAudio;

	public AudioSource currWeatherAudio;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		audioSource = GetComponent<AudioSource>();
		currWindAudio = windAudio;
		currWeatherAudio = weatherAudio;
		windAudio.enabled = false;
		windAudio2.enabled = false;
		weatherAudio.enabled = false;
		weatherAudio2.enabled = false;
	}

	private AudioClip GetMusic(BgmType type)
	{
		AudioClip result = null;
		switch (type)
		{
		case BgmType.Nor:
			result = GameManager.Instance.AudioConf.Nor;
			break;
		case BgmType.GrassWalk:
			result = GameManager.Instance.AudioConf.GrassWalk;
			break;
		case BgmType.MoonGrains:
			result = GameManager.Instance.AudioConf.MoonGrains;
			break;
		case BgmType.WateryGraves:
			result = GameManager.Instance.AudioConf.WateryGraves;
			break;
		case BgmType.RigorMormist:
			result = GameManager.Instance.AudioConf.RigorMormist;
			break;
		case BgmType.GrazeTheRoof:
			result = GameManager.Instance.AudioConf.GrazeTheRoof;
			break;
		case BgmType.SwampDay:
			result = GameManager.Instance.AudioConf.SwampDay;
			break;
		case BgmType.SwampNight:
			result = GameManager.Instance.AudioConf.SwampNight;
			break;
		case BgmType.BrainiacManiac:
			result = GameManager.Instance.AudioConf.BrainiacManiac;
			break;
		case BgmType.Loonboon:
			result = GameManager.Instance.AudioConf.Loonboon;
			break;
		case BgmType.UltimateBattle:
			result = GameManager.Instance.AudioConf.UltimateBattle;
			break;
		case BgmType.ZenGarden:
			result = GameManager.Instance.AudioConf.ZenGarden;
			break;
		case BgmType.Cerebrawl:
			result = GameManager.Instance.AudioConf.Cerebrawl;
			break;
		case BgmType.ChooseYourSeeds:
			result = GameManager.Instance.AudioConf.ChooseYourSeeds;
			break;
		case BgmType.NightChooseSeeds:
			result = GameManager.Instance.AudioConf.NightChooseSeeds;
			break;
		}
		return result;
	}

	public void SetVolume(float sound, float bgm)
	{
		currWindAudio.volume = sound;
		currWeatherAudio.volume = sound;
		audioSource.volume = bgm;
		BgmVolume = bgm;
		SoundVolume = sound;
	}

	public void SetRainVolume(bool isStart, bool needFade)
	{
		AudioClip newClip = ((SkyManager.Instance.RainScale <= 4) ? GameManager.Instance.AudioConf.Rain1 : ((SkyManager.Instance.RainScale > 7) ? GameManager.Instance.AudioConf.Rain3 : GameManager.Instance.AudioConf.Rain2));
		SetWeatherVolume(isStart, needFade, newClip);
	}

	public void SetHailVolume(bool isStart, bool needFade)
	{
		AudioClip newClip = ((SkyManager.Instance.HailScale > 6) ? GameManager.Instance.AudioConf.BigHail : GameManager.Instance.AudioConf.SmallHail);
		SetWeatherVolume(isStart, needFade, newClip);
	}

	private void SetWeatherVolume(bool isStart, bool needFade, AudioClip newClip)
	{
		if (currWeatherAudio == null)
		{
			currWeatherAudio = weatherAudio;
		}
		if (WeatherCoroutine != null)
		{
			StopCoroutine(WeatherCoroutine);
		}
		if (isStart != currWeatherAudio.enabled)
		{
			if (isStart)
			{
				currWeatherAudio.clip = newClip;
			}
			if (needFade)
			{
				WeatherCoroutine = StartCoroutine(AudioFade(isStart, currWeatherAudio));
				return;
			}
			if (isStart)
			{
				currWeatherAudio.enabled = true;
				currWeatherAudio.volume = SoundVolume;
				return;
			}
			weatherAudio.volume = 0f;
			weatherAudio.enabled = false;
			weatherAudio2.volume = 0f;
			weatherAudio2.enabled = false;
		}
		else if ((currWeatherAudio.enabled & isStart) && newClip != currWeatherAudio.clip)
		{
			if (WeatherMixCoroutine != null)
			{
				StopCoroutine(WeatherMixCoroutine);
			}
			AudioSource audioSource = ((currWeatherAudio == weatherAudio) ? weatherAudio2 : weatherAudio);
			audioSource.clip = newClip;
			AudioSource oldAudio = currWeatherAudio;
			currWeatherAudio = audioSource;
			audioSource.enabled = true;
			WeatherMixCoroutine = StartCoroutine(CrossFadeRoutine(oldAudio, audioSource));
		}
	}

	public void SetWindVolume(bool isStart, bool needFade)
	{
		if (currWindAudio == null)
		{
			currWindAudio = windAudio;
		}
		if (WindCoroutine != null)
		{
			StopCoroutine(WindCoroutine);
		}
		AudioClip audioClip = ((SkyManager.Instance.WindScale >= 3) ? GameManager.Instance.AudioConf.BigWind : GameManager.Instance.AudioConf.SmallWind);
		if (isStart != currWindAudio.enabled)
		{
			if (isStart)
			{
				currWindAudio.clip = audioClip;
			}
			if (needFade)
			{
				WindCoroutine = StartCoroutine(AudioFade(isStart, currWindAudio));
				return;
			}
			if (isStart)
			{
				currWindAudio.enabled = true;
				currWindAudio.volume = SoundVolume;
				return;
			}
			windAudio.volume = 0f;
			windAudio.enabled = false;
			windAudio2.volume = 0f;
			windAudio2.enabled = false;
		}
		else if ((currWindAudio.enabled & isStart) && audioClip != currWindAudio.clip)
		{
			if (WindMixCoroutine != null)
			{
				StopCoroutine(WindMixCoroutine);
			}
			AudioSource audioSource = ((currWindAudio == windAudio) ? windAudio2 : windAudio);
			audioSource.clip = audioClip;
			AudioSource oldAudio = currWindAudio;
			currWindAudio = audioSource;
			audioSource.enabled = true;
			WindMixCoroutine = StartCoroutine(CrossFadeRoutine(oldAudio, audioSource));
		}
	}

	private IEnumerator CrossFadeRoutine(AudioSource oldAudio, AudioSource nextAudio)
	{
		float fadeDuration = 2f;
		float timer = 0f;
		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;
			float t = timer / fadeDuration;
			if (nextAudio != null)
			{
				nextAudio.volume = Mathf.Lerp(0f, SoundVolume, t);
			}
			yield return null;
		}
		timer = 0f;
		while (timer < fadeDuration)
		{
			timer += Time.deltaTime;
			float t2 = timer / fadeDuration;
			if (oldAudio != null)
			{
				oldAudio.volume = Mathf.Lerp(SoundVolume, 0f, t2);
			}
			yield return null;
		}
		if (nextAudio != null)
		{
			nextAudio.volume = SoundVolume;
		}
		if (oldAudio != null)
		{
			oldAudio.volume = 0f;
			oldAudio.Stop();
			oldAudio.enabled = false;
		}
		WindMixCoroutine = null;
	}

	private IEnumerator AudioFade(bool isStart, AudioSource currSource)
	{
		if (isStart)
		{
			currSource.enabled = true;
			currSource.volume = 0f;
			while (currSource.volume < SoundVolume)
			{
				yield return new WaitForSeconds(0.2f);
				currSource.volume += 0.02f;
			}
		}
		else
		{
			while (currSource.volume > 0f)
			{
				yield return new WaitForSeconds(0.2f);
				currSource.volume -= 0.02f;
			}
			currSource.enabled = false;
		}
	}

	public void RandomPlayEFAudio(List<AudioClip> clips, Vector2 pos)
	{
		List<UnityAction> list = new List<UnityAction>();
		for (int i = 0; i < clips.Count; i++)
		{
			AudioClip clip = clips[i];
			list.Add(() =>
			{
				PlayEFAudio(clip, pos);
			});
		}
		MyTool.RandomOne(list);
	}

	public EFAudio PlayEFAudio(AudioClip clip, Vector2 Pos, bool isAll = false)
	{
		bool flag = false;
		if (EfAudioNum.ContainsKey(clip.name))
		{
			if (EfAudioNum[clip.name] < 5)
			{
				flag = true;
				EfAudioNum[clip.name]++;
			}
		}
		else
		{
			flag = true;
			EfAudioNum.Add(clip.name, 1);
		}
		if (!flag)
		{
			return null;
		}
		EFAudio eFAudio = null;
		if (isAll)
		{
			eFAudio = PoolManager.Instance.GetObj(GameManager.Instance.AudioConf.EFAudio).GetComponent<EFAudio>();
			eFAudio.Init(clip, SoundVolume);
			eFAudio.transform.SetParent(base.transform);
		}
		else if (MapManager.Instance.GetCurrMap(Pos) == CameraControl.Instance.CurrMap)
		{
			eFAudio = PoolManager.Instance.GetObj(GameManager.Instance.AudioConf.EFAudio).GetComponent<EFAudio>();
			eFAudio.Init(clip, SoundVolume);
			eFAudio.transform.SetParent(base.transform);
			CurrEFAudios.Add(eFAudio);
		}
		else if (EfAudioNum.ContainsKey(clip.name))
		{
			EfAudioNum[clip.name]--;
			if (EfAudioNum[clip.name] <= 0)
			{
				EfAudioNum.Remove(clip.name);
			}
		}
		return eFAudio;
	}

	public void RemoveEFAudio(EFAudio eFAudio, string name)
	{
		if (CurrEFAudios.Contains(eFAudio))
		{
			CurrEFAudios.Remove(eFAudio);
		}
		if (EfAudioNum.ContainsKey(name))
		{
			EfAudioNum[name]--;
			if (EfAudioNum[name] <= 0)
			{
				EfAudioNum.Remove(name);
			}
		}
	}

	public void ChangeMapReset()
	{
		for (int i = 0; i < CurrEFAudios.Count; i++)
		{
			CurrEFAudios[i].Close();
		}
	}

	public void PlayBgAudio(BgmType type)
	{
		AudioClip music = GetMusic(type);
		if (music == null)
		{
			audioSource.UnPause();
			return;
		}
		audioSource.Stop();
		audioSource.clip = music;
		audioSource.Play();
	}

	public void FadeBgAndPlayNew(BgmType type, bool faster = false)
	{
		AudioClip music = GetMusic(type);
		if (!(audioSource.clip == music))
		{
			if (BgmFadeCoroutine != null)
			{
				StopCoroutine(BgmFadeCoroutine);
			}
			BgmFadeCoroutine = StartCoroutine(FadeBg(music, faster));
		}
	}

	private IEnumerator FadeBg(AudioClip clip, bool faster)
	{
		while (audioSource.volume > 0f)
		{
			yield return new WaitForSeconds(0.2f);
			if (faster)
			{
				audioSource.volume -= 0.06f;
			}
			else
			{
				audioSource.volume -= 0.02f;
			}
		}
		audioSource.volume = BgmVolume;
		audioSource.Stop();
		if (clip != null)
		{
			audioSource.clip = clip;
			audioSource.Play();
		}
		BgmFadeCoroutine = null;
	}

	public void StopBgAudio()
	{
		if (BgmFadeCoroutine != null)
		{
			StopCoroutine(BgmFadeCoroutine);
		}
		audioSource.Pause();
	}
}
