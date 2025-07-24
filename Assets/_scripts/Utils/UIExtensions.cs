
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIExtensions
{

	public static Canvas GetParentCanvas(this Component component)
	{
		if (component == null) return null;
		return component.GetComponentInParent<Canvas>();
	}

	/// <summary>
	/// Returns pivot of the RectTransform in the coordinate system of another RectTransform.
	/// </summary>
	public static Vector2 GetPositionInOtherRectTranform(
		this RectTransform fromRect,
		RectTransform toRect,
		float xOffset = 0f,
		float yOffset = 0f
	)
	{
		if (fromRect == null || toRect == null)
			return Vector2.zero;

		var fromCanvas = fromRect.GetParentCanvas();
		var toCanvas = toRect.GetParentCanvas();
		if (fromCanvas == null || toCanvas == null)
			return Vector2.zero;

		// 1) Pivot world ? screen
		var fromCam = fromCanvas.renderMode == RenderMode.ScreenSpaceOverlay
			? null
			: fromCanvas.worldCamera;
		var screenPt = RectTransformUtility.WorldToScreenPoint(fromCam, fromRect.position);

		// 2) Screen ? local toRect
		var toCam = toCanvas.renderMode == RenderMode.ScreenSpaceOverlay
			? null
			: toCanvas.worldCamera;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			toRect, screenPt, toCam, out Vector2 localPt);

		// 3) optional offset
		localPt.x += xOffset;
		localPt.y += yOffset;
		return localPt;
	}

	/// <summary>
	/// Returns the anchoredPosition for any local point of rectFrom in the coordinate system of rectTo's parent.
	/// </summary>
	public static Vector2 GetAnchoredPositionInOtherRect(
		this RectTransform rectFrom,
		RectTransform rectTo,
		Vector2? localPointInFrom = null)
	{
		if (rectFrom == null) return Vector2.zero;
		if (rectTo == null) return Vector2.zero;

		var canvas = rectFrom.GetComponentInParent<Canvas>();
		if (canvas == null) return Vector2.zero;
		var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;


		Vector2 point = localPointInFrom ?? rectFrom.rect.center;
		Vector3 worldPoint = rectFrom.TransformPoint(point);

		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPoint);

		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rectTo, screenPoint, cam, out Vector2 anchoredPos);

		return anchoredPos;
	}

	/// <summary>
	/// Simple check if two RectTransforms are overlapping (checks only center of object)
	/// </summary>
	public static bool IsRectOverlapping(this RectTransform fromRect, RectTransform toRect)
	{
		var localPt = fromRect.GetPositionInOtherRectTranform(toRect);

		var size = toRect.rect.size;
		var pivot = toRect.pivot;

		var minX = -pivot.x * size.x;
		var maxX = (1 - pivot.x) * size.x;
		var minY = -pivot.y * size.y;
		var maxY = (1 - pivot.y) * size.y;

		return (localPt.x >= minX && localPt.x <= maxX) && (localPt.y >= minY && localPt.y <= maxY);
	}

	public static Vector3 GetTopEdgeCenterWorldPosition(this RectTransform rectTransform)
	{
		var rect = rectTransform.rect;
		var pivot = rectTransform.pivot;

		float localX = (0.5f - pivot.x) * rect.width; // Get the center X position based on pivot
		float localY = (1f - pivot.y) * rect.height; // Get the top Y position based on pivot

		var localPoint = new Vector3(localX, localY, 0f);

		return rectTransform.TransformPoint(localPoint);
	}

	public static Vector2 GetTopEdgeCenterScreenPosition(this RectTransform rectTransform, Camera cam = null)
	{
		Vector3 worldPos = rectTransform.GetTopEdgeCenterWorldPosition();
		return RectTransformUtility.WorldToScreenPoint(cam, worldPos);
	}

	public static void SetAnchoredPositionByScreenPoint(this RectTransform rectTransform, Vector2 screenPosition, Vector2 grabOffset, Camera camera = null)
	{
		RectTransform parentRect = rectTransform.parent as RectTransform;
		if (parentRect == null)
			return;

		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			parentRect,
			screenPosition,
			camera,
			out localPoint
		);
		rectTransform.anchoredPosition = localPoint + grabOffset;
	}

	public static Vector2 CalculateGrabOffset(this RectTransform rectTransform, PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rectTransform,
			eventData.position,
			eventData.pressEventCamera,
			out var localPointInSelf
		);

		Vector2 centerLocal = new Vector2(
			(0.5f - rectTransform.pivot.x) * rectTransform.rect.width,
			(0.5f - rectTransform.pivot.y) * rectTransform.rect.height
		);

		return centerLocal - localPointInSelf;
	}

	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Vector2 SetX(this Vector2 value, float x)
	{
		return new Vector2(x, value.y); 
	}

	public static Vector2 SetY(this Vector2 value, float y)
	{
		return new Vector2(value.x, y);
	}

	public static Vector2 TransformPointToOtherRect(this RectTransform from, Vector2 localPoint, RectTransform to)
	{
		Vector3 worldPoint = from.TransformPoint(localPoint);
		return to.InverseTransformPoint(worldPoint);
	}
}
