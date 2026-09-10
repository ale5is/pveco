using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LVInfo", menuName = "LVInfo")]
public class LVInfo : ScriptableObject
{
	public int LevelId;

	public string LevelName;

	public Sprite LevelSprite;

	public bool HaveFog;

	public bool HaveRain;

	public bool HaveThunder;

	public int PassTime;

	public int PassNum;

	public List<Sprite> MapList = new List<Sprite>();
}
