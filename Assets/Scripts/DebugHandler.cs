using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHandler : MonoBehaviour
{
	public List<Vector3> points;

	void Awake()
	{
		points = new List<Vector3>();
	}

	public void AddPoints(Vector3[] points)
	{
		this.points.AddRange(points);
	}

	public void AddPoints(List<Vector3> points)
	{
		this.points = points;
	}

	void OnDrawGizmos()
	{
		if (points != null)
		{
			Gizmos.color = Color.red;

			for (int i = 0; i < points.Count; i++)
			{
				// Gizmos.DrawLine(points[i], points[i + 1]);
				Gizmos.DrawSphere(points[i], 0.1f);
			}
		}
	}
}
