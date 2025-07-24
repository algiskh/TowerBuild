using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PolygonArea : MonoBehaviour
{
	private RectTransform _rt;
	public RectTransform RT => _rt;

	private void Awake()
	{
		_rt = GetComponent<RectTransform>();
	}

	// Get points with local coordinates
	private List<Vector2> GetPolygonPoints()
	{
		var points = new List<Vector2>();
		foreach (Transform child in transform)
			points.Add(child.localPosition);
		return points;
	}

	/// <summary>
	/// Check if position is inside polygon defined by points
	/// </summary>
	public bool IsInsidePolygon(Vector2 pos)
	{
		var polyPoints = GetPolygonPoints();
		int i, j = polyPoints.Count - 1;
		bool inside = false;
		for (i = 0; i < polyPoints.Count; j = i++)
		{
			if (((polyPoints[i].y > pos.y) != (polyPoints[j].y > pos.y)) &&
				(pos.x < (polyPoints[j].x - polyPoints[i].x) * (pos.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
				inside = !inside;
		}
		return inside;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var points = GetPolygonPoints();
		int count = points.Count;
		if (count > 1)
		{
			Gizmos.color = Color.cyan;
			for (int i = 0; i < count; i++)
			{
				Vector3 a = transform.TransformPoint(points[i]);
				Vector3 b = transform.TransformPoint(points[(i + 1) % count]);
				Gizmos.DrawLine(a, b);
				Gizmos.DrawSphere(a, 0.04f);
			}
		}
	}
#endif
}