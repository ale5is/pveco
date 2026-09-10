using UnityEngine;

namespace Reanim2UnityAnim
{
	public class UniqueMaterialController : MonoBehaviour
	{
		public SpriteRenderer[] spriteRenderers;

		private void Awake()
		{
			SpriteRenderer[] array = spriteRenderers;
			foreach (SpriteRenderer obj in array)
			{
				obj.material = new Material(obj.material);
			}
		}
	}
}
