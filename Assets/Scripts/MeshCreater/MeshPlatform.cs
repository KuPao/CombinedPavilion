using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshPlatform : MonoBehaviour 
{
	public Mesh CreateMeshPrism(Mesh mesh, float radius, float height, int sides)
	{
		int i;
        int nbVertices = sides * 2 * 3;
		Vector3[] vertices = new Vector3[nbVertices];
        float angle = Mathf.PI * 2 / sides;
      
        // Top face
        for(i = 0; i < sides; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), height / 2, radius * Mathf.Cos(i * angle));
        }

        // Bottom face
        for(i = sides; i < sides * 2; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), -height / 2, radius * Mathf.Cos(i * angle));
        }

		// Sides
		for(i = sides * 2; i < sides * 3; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), height / 2, radius * Mathf.Cos(i * angle));
        }

		for(i = sides * 3; i < sides * 4; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), -height / 2, radius * Mathf.Cos(i * angle));
        }

		for(i = sides * 4; i < sides * 5; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), height / 2, radius * Mathf.Cos(i * angle));
        }

		for(i = sides * 5; i < sides * 6; i++)
        {
            vertices[i] = new Vector3(radius * Mathf.Sin(i * angle), -height / 2, radius * Mathf.Cos(i * angle));
        }
     
        mesh.vertices = vertices;
     
        int[] tri = new int[(sides - 2 + sides) * 2 * 3];

        for(i = 0; i < (sides - 2) * 3; i++)
        {
            tri[i] = 0;
            tri[++i] = i / 3 + 1;
            tri[++i] = i / 3 + 2;
        }
        for(; i < (sides - 2) * 3 * 2; i++)
        {
            tri[i] = sides;
            tri[++i] = i / 3 + 4;
            tri[++i] = i / 3 + 3;
        }
        // for(; i < (sides - 2) * 3 * 2 + sides * 3 - 3; i++)
        // {
        //     tri[i] = sides * 2 + (i - (sides - 2) * 6) / 3;
        //     tri[++i] = sides * 3 + (i - (sides - 2) * 6) / 3;
        //     tri[++i] = i / 3 + 3;
        // }
        for(; i < (sides - 2) * 3 * 2 + sides * 3 * 2 - 6; i++)
        {
            tri[i] = sides * 2 + (i - (sides - 2) * 6) / 6;
            tri[++i] = sides * 3 + (i - (sides - 2) * 6) / 6;
            tri[++i] = sides * 4 + (i - (sides - 2) * 6) / 6 + 1;
            tri[++i] = tri[i - 2];
            tri[++i] = tri[i - 2] + sides;
            tri[++i] = tri[i - 3];
        }
        tri[i] = sides * 2 + (i - (sides - 2) * 6) / 6;
        tri[++i] = sides * 3 + (i - (sides - 2) * 6) / 6;
        tri[++i] = tri[i - 1] + 1;
        tri[++i] = tri[i - 2];
        tri[++i] = tri[i - 2] + sides;
        tri[++i] = tri[i - 3];
		/*{
        // Top face:
        0, 1, 2,2
        0, 2, 3,5
        0, 3, 4,8
        0, 4, 5,11
        // Bottom face:
        6, 8, 7,14
        6, 9, 8,17
        6, 10, 9,20
        6, 11, 10,23
        // Sides:
        12, 18, 25,26
        18, 31, 25,29
        13, 19, 26,32
        19, 32, 26,35
        14, 20, 27,38
        20, 33, 27,41
        15, 21, 28,44
        21, 34, 28,47
        16, 22, 29,50
        22, 35, 29,53
        17, 23, 24,56
        23, 30, 24 59
		};*/

        mesh.triangles = tri;
     
        Vector3[] normals = new Vector3[nbVertices];
    
        for(i = 0; i < nbVertices; i++) 
        {
			if (i < sides)
            	normals[i] = Vector3.up;
			else if (i < sides * 2)
				normals[i] = Vector3.down;
			else if (i < sides * 4)
				normals[i] = new Vector3(Mathf.Sin(i * angle + Mathf.PI / sides), 0, Mathf.Cos(i * angle + Mathf.PI / sides));
			else
				normals[i] = new Vector3(Mathf.Sin(i * angle - Mathf.PI / sides), 0, Mathf.Cos(i * angle - Mathf.PI / sides));
        }
    
        mesh.normals = normals;

		return mesh;
	}
}
