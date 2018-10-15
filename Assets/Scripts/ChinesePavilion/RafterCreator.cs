using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RafterCreator : MonoBehaviour
{
	public Material material;
	public Color materialColor;
	public GameObject Create(List<Vector3> points, int nbSides, int resolution, float height, float tall, float radius)
	{
		//(curvePoints, rafterNbSides, rafterRadius, rafterTall, flyEaveHeight, flyEaveLength
		GameObject line = new GameObject();
		line.name = "line";
		MeshFilter filter = line.AddComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		MeshRenderer meshRenderer = line.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
		meshRenderer.material.color = materialColor;

		//生成mesh
		MeshRafter meshRafter = new MeshRafter();
		mesh = meshRafter.CreateRafter(points, mesh, nbSides, resolution, height, tall, radius);
		//int nbSides, int resolution, float height, float tall, float radius, float flyEaveHeight, float flyEaveLength)
		line.GetComponent<MeshFilter>().mesh = mesh;

		return line;
	}
}
