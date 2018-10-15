using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCurve
{
	private int res = 10;
	private float height;
	private float width;
	private float a;
	private List<Vector3> points;

	public List<Vector3> CreateCircleCurve(float height, float width, float a, int res)
	{
		points = new List<Vector3>();
		this.height = height;
		this.width = width;
		this.res = res;
		//Range(0.01f, Mathf.PI / 4)
		a = Mathf.Max(a, 0.01f);
		a = Mathf.Min(a, Mathf.PI / 4);
		this.a = a;
		CalculatePoints();
		return points;
	}

	private void CalculatePoints()
	{
		//Debug.Log("height:" + height + ",width:" + width + ",a:" + a);
		//斜邊半徑
		float L = Mathf.Sqrt(Mathf.Pow(height, 2) + Mathf.Pow(width, 2)) / 2;
		//扇形半徑
		float R = L / (Mathf.Sin(a));
		//R = Mathf.Abs(R);
		//Debug.Log(R);
		//求弧度
		float theta = Mathf.Acos((Mathf.Pow(L * 2, 2) + Mathf.Pow(width, 2) - Mathf.Pow(height, 2)) / (2 * L * 2 * width));
		//角度
		//Debug.Log(theta * 180 / Mathf.PI);
		//Φ = π/2+θ - a  (算得要命 =w=)
		float phi = Mathf.PI / 2 + theta - a;
		//扇形中心
		Vector3 circleCenter = new Vector3(R * Mathf.Cos(phi), R * Mathf.Sin(phi), 0);

		float maxAngle = Mathf.Atan2(height - circleCenter.y, width - circleCenter.x);
		float minAngle = Mathf.Atan2(0 - circleCenter.y, 0 - circleCenter.x);

		//Debug.Log("max:" + maxAngle * 180 / Mathf.PI);
		//Debug.Log("min:" + minAngle * 180 / Mathf.PI);

		int count = 0;
		for (float i = minAngle; i < maxAngle; i = i + (maxAngle - minAngle) / res)
		{
			Vector3 newPoint = new Vector3(0, 0, 0)
			{
				x = circleCenter.x + R * Mathf.Cos(i),
				y = circleCenter.y + R * Mathf.Sin(i)
			};
			points.Add(newPoint);
			count++;
		}

		//因為float精度問題 要看狀況補給他一個
		if (count < res + 1)
		{
			points.Add(new Vector3(width, height));
		}
	}
}
