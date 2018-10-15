using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmulRomLine
{

	public List<Vector3> CatmulRom(Vector3[] points, float amountOfPoints)
	{
		//弧線的點
		List<Vector3> curvePoints = new List<Vector3>();

		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector3 p0 = points[Mathf.Max(i - 1, 0)];
			Vector3 p1 = points[i];
			Vector3 p2 = points[Mathf.Min(i + 1, points.Length - 1)];
			Vector3 p3 = points[Mathf.Min(i + 2, points.Length - 1)];

			for (int j = 0; j < amountOfPoints; j++)
			{
				Vector3 newPos = ReturnCatmullRomPos((1.0f / amountOfPoints) * j, p0, p1, p2, p3);
				curvePoints.Add(newPos);
			}

		}
		curvePoints.Add(points[points.Length - 1]);
		return curvePoints;
	}

	private Vector3 ReturnCatmullRomPos(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 pos = 0.5f * ((2f * p1) + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t + (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
		return pos;
	}
}
