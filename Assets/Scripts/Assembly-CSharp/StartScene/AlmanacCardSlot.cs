using UnityEngine;

namespace StartScene
{
	public class AlmanacCardSlot : MonoBehaviour
	{
		public PlantType plantType;

		public ZombieType zombieType;

		public SpriteRenderer REnderer;

		public TextMesh NeedNumText;

		private Sprite NomalSprite;

		private float CdTime;

		private void Start()
		{
			NomalSprite = REnderer.sprite;
		}

		public void UpdateInfo(UIPlantCardNC nC)
		{
			plantType = nC.CardPlantType;
			zombieType = nC.CardZombieType;
			if (nC.CardPlantType != PlantType.Nope || nC.CardZombieType != ZombieType.Nope)
			{
				REnderer.sprite = nC.OwnerSprite;
				NeedNumText.text = nC.NeedNum.ToString();
				CdTime = nC.CDTime;
			}
			else
			{
				REnderer.sprite = NomalSprite;
				NeedNumText.text = "";
				CdTime = 0f;
			}
			if (nC.CardPlantType != PlantType.MoonTombStone && nC.CardPlantType != PlantType.Nope && !GameManager.Instance.LocalPlayerSave.UnlockedPlants.Contains(nC.CardPlantType))
			{
				plantType = PlantType.Nope;
				REnderer.sprite = NomalSprite;
				NeedNumText.text = "";
				CdTime = 0f;
			}
		}

		public void ResetInfo()
		{
			plantType = PlantType.Nope;
			REnderer.sprite = NomalSprite;
			NeedNumText.text = "";
			CdTime = 0f;
		}

		private void OnMouseEnter()
		{
			if (!MyTool.IsPointerOverGameObject() && (plantType != PlantType.Nope || zombieType != ZombieType.Nope))
			{
				REnderer.material.SetFloat("_Brightness", 1.3f);
			}
		}

		private void OnMouseExit()
		{
			REnderer.material.SetFloat("_Brightness", 1f);
		}

		private void OnMouseDown()
		{
			if ((plantType != PlantType.Nope || zombieType != ZombieType.Nope) && !MyTool.IsPointerOverGameObject())
			{
				if (Random.Range(0, 2) == 1)
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap, base.transform.position, isAll: true);
				}
				else
				{
					AudioManager.Instance.PlayEFAudio(GameManager.Instance.AudioConf.Tap2, base.transform.position, isAll: true);
				}
				LoadThis();
			}
		}

		public void LoadThis()
		{
			AlmcPlantLoader.Instance.LoadPlant(plantType, zombieType, NeedNumText.text, CdTime);
		}
	}
}
