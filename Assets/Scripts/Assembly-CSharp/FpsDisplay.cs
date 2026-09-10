using UnityEngine;
using UnityEngine.UI;

public class FpsDisplay : MonoBehaviour
{
	private float _updateInterval = 1f;

	private float _accum;

	private int _frames;

	private float _timeLeft;

	private string fpsFormat;

	private Text fpsText;

	private void Start()
	{
		_timeLeft = _updateInterval;
		fpsText = base.transform.GetComponent<Text>();
	}

	private void Update()
	{
		_timeLeft -= Time.deltaTime;
		_accum += Time.timeScale / Time.deltaTime;
		_frames++;
		if (_timeLeft <= 0f)
		{
			float num = _accum / (float)_frames;
			if (!float.IsNaN(num))
			{
				fpsFormat = $"{num:F0}fps";
			}
			fpsText.text = fpsFormat;
			_timeLeft = _updateInterval;
			_accum = 0f;
			_frames = 0;
		}
	}
}
