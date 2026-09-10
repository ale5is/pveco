using UnityEngine;

public class TransformUtility : MonoBehaviour
{
	public static Camera ScenceCamera;

	public static Camera UGUICamera;

	public static Vector2 WorldToScreenPoint(Vector3 worldPoint)
	{
		return RectTransformUtility.WorldToScreenPoint(ScenceCamera, worldPoint);
	}

	public static Vector2 WorldToScreenPoint(Camera cam, Vector3 worldPoint)
	{
		return RectTransformUtility.WorldToScreenPoint(cam, worldPoint);
	}

	public static bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, out Vector3 worldPoint)
	{
		return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, UGUICamera, out worldPoint);
	}

	public static bool ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
	{
		return RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, out worldPoint);
	}

	public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, out Vector2 localPoint)
	{
		return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, UGUICamera, out localPoint);
	}

	public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint)
	{
		return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, out localPoint);
	}
}
