using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshRafter
{

    //弧線
    public float alpha = 0.5f;
    //飛簷上面的點
    private int nbSides;
    private int resolution;
    private float height;
    private float tall;
    private float radius;

    //節點
    private List<Vector3> vertices;

    /// <summary>
    /// 製作屋頂(?)
    /// </summary>
    /// <param name="points">控制點</param>
    /// <param name="mesh">要處理的mesh</param>
    /// <param name="rafterRadius">半徑</param>
    /// <param name="rafterTall">高</param>
    public Mesh CreateRafter(List<Vector3> curvePoints, Mesh mesh, int nbSides, int resolution, float height, float tall, float radius)
    {
        this.nbSides = nbSides;
        this.resolution = resolution;
        this.height = height;
        this.tall = tall;
        this.radius = radius;

        vertices = new List<Vector3>();
        //製作mesh
        mesh = CreateMesh(mesh, curvePoints);
        return mesh;
    }

    /// <summary>
    /// 繪製Mesh
    /// </summary>
    /// <param name="mesh">目標mesh</param>
    /// <param name="rafterRadius">半徑</param>
    /// <param name="rafterTall">高度</param>
    Mesh CreateMesh(Mesh mesh, List<Vector3> curvePoints)
    {

        //string filePath = "Assets/Prefabs/MeshCreate/Rafter_" + nbSides + "_" + resolution + "_" + height + "_" + tall + "_" + radius + "_" + flyEaveHeight + "_" + flyEaveLength + ".asset";
        //if (File.Exists(filePath))
        //{
        //	Mesh meshFromFile = (Mesh)AssetDatabase.LoadAssetAtPath(filePath, typeof(Mesh));
        //	return meshFromFile;
        //}

        //side
        //上面半圓形點的數量
        int halfPoint = nbSides / 2 + 1;
        //切片的點數
        int facePoint = halfPoint + 2;

        #region Vertices
        for (int i = 0; i < curvePoints.Count; i++)
        {
            Vector3 vec = i != curvePoints.Count - 1 ? curvePoints[i + 1] - curvePoints[i] : curvePoints[i] - curvePoints[i - 1];

            //上面半圓形
            for (int j = 0; j < halfPoint; j++)
            {
                vertices.Add(FindCirclePoint(curvePoints[i], vec, (360 / nbSides), radius, j - (nbSides / 2 + 1) / 2));
            }
            //下面正方形
            Vector3 pointA = new Vector3(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].y - tall, vertices[vertices.Count - 1].z);
            Vector3 pointB = new Vector3(vertices[vertices.Count - halfPoint].x, vertices[vertices.Count - halfPoint].y - tall, vertices[vertices.Count - halfPoint].z);
            vertices.Add(pointA);
            vertices.Add(pointB);
        }
        //foreach (Vector3 vec in vertices) Debug.Log(vec.ToString("F4"));
        //bottom
        //最後一個面再加一次點 normal才會算對
        int vCount = vertices.Count;
        for (int i = vCount - facePoint; i < vCount; i++)
        {
            Vector3 point = vertices[i];
            vertices.Add(vertices[i]);
        }
        vertices.Add(curvePoints[curvePoints.Count - 1]);
        #endregion

        #region UVs
        List<Vector2> uvs = new List<Vector2>();
        float uvY = 0;
        for (int y = 0; y < curvePoints.Count; y++)
        {
            if (y > 0)
            {
                float distance = Vector3.Distance(curvePoints[y], curvePoints[y - 1]);
                uvY -= distance * 3;
            }
            //上面半圓形
            for (int x = 0; x < halfPoint; x++)
            {
                float uvX = 1f - (float)x / (halfPoint - 1);

                uvs.Add(new Vector2(uvX, uvY));
            }
            uvs.Add(new Vector2(1, uvY));
            uvs.Add(new Vector2(0, uvY));
        }

        for (int i = vertices.Count; i >= vertices.Count - facePoint; i--)
        {
            uvs.Add(new Vector2(0, 0));
        }

        #endregion

        #region Triangles
        List<int> triangles = new List<int>();

        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            for (int j = 0; j < facePoint; j++)
            {
                //a---b
                //|\  |
                //| \ |
                //c---d
                int a = i * facePoint + j;
                int b = i * facePoint + (j + 1) % facePoint;
                int c = a + facePoint;
                int d = b + facePoint;
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(d);

                triangles.Add(a);
                triangles.Add(d);
                triangles.Add(c);
            }
        }
        ////bottom
        for (int i = 0; i < facePoint; i++)
        {
            int a = i + (vertices.Count - 1 - facePoint);
            int b = (i + 1) % facePoint + (vertices.Count - 1 - facePoint);
            int c = vertices.Count - 1;
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
        #endregion

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        #region tangents
        //speed up math by copying the mesh arrays
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        //variable definitions
        int triangleCount = triangles.ToArray().Length;
        int vertexCount = vertices.ToArray().Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (int a = 0; a < triangleCount; a += 3)
        {
            int i1 = triangles[a + 0];
            int i2 = triangles[a + 1];
            int i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }    


        for (long a = 0; a < vertexCount; ++a)
        {
            Vector3 n = normals[a];
            Vector3 t = tan1[a];

            //Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            //tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }

        mesh.tangents = tangents;
        #endregion
        mesh.RecalculateBounds();

        //AssetDatabase.CreateAsset(mesh, filePath);
        //AssetDatabase.SaveAssets();

        return mesh;
    }


    Vector3 FindCirclePoint(Vector3 centerPos, Vector3 vec, float deg, float r, float i)
    {
        //Debug.Log(deg * i + 90);
        float x = centerPos.x;
        float y = centerPos.y + r * Mathf.Sin((deg * i + 90) * Mathf.Deg2Rad);
        float z = centerPos.z + r * Mathf.Cos((deg * i + 90) * Mathf.Deg2Rad);
        Vector3 newCirclePoint = new Vector3(x, y, z);
        //Debug.Log(centerPos.ToString("F4"));

        //這個好：https://answers.unity.com/questions/750049/calculate-points-on-a-circle-sphere.html
        //Vector3 newCirclePoint = centerPos + Quaternion.AngleAxis(deg * i, vec) * (Vector3.up * r);
        //Debug.Log((centerPos + Quaternion.AngleAxis(deg * i, vec) * (Vector3.up * r)).ToString("F4"));

        // Debug.Log((Quaternion.AngleAxis(deg * i, vec) * (Vector3.up * r)).ToString("F4"));
        // Debug.Log(newCirclePoint.ToString("F4"));
        return newCirclePoint;
    }
}
