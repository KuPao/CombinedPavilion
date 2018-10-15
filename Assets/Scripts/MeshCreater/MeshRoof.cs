using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRoof : MonoBehaviour
{
    /// <summary>
    /// </summary>
    /// <param name="rightRidgeFrontPoints">正脊的點</param>
    /// <param name="bargeboardPoints">垂脊的點</param>
    /// <param name="roofLength">長度</param>
    /// <param name="rafterRes">翹起來的節點</param>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public Mesh CreateFrontRoofMesh(List<Vector3> rightRidgeFrontPoints, List<Vector3> bargeboardPoints, float roofLength, int rafterRes, Mesh mesh)
    {
        int UVScale = 4;
        if (rightRidgeFrontPoints.Count < rafterRes)
        {
            rafterRes = 1;
        }
        int cutNum = (int)roofLength * 3;

        List<Vector3> topRoofPoints = new List<Vector3>();
        for (int i = 0; i < rafterRes; i++)
        {
            topRoofPoints.Add(rightRidgeFrontPoints[i]);
        }
        for (int i = 1; i < cutNum; i++)
        {
            Vector3 newPoint = (rightRidgeFrontPoints[rafterRes - 1] * (cutNum - i) / cutNum) + (rightRidgeFrontPoints[rafterRes] * i / cutNum);
            topRoofPoints.Add(newPoint);
        }
        for (int i = rafterRes; i < rightRidgeFrontPoints.Count; i++)
        {
            topRoofPoints.Add(rightRidgeFrontPoints[i]);
        }

        //節點
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        #region Vertices
        for (int x = 0; x < topRoofPoints.Count; x++)
        {
            for (int y = 0; y < bargeboardPoints.Count; y++)
            {
                float heightDif =
                    (float)(topRoofPoints[0].y - topRoofPoints[x].y) * ((float)(bargeboardPoints.Count - y) / bargeboardPoints.Count);
                Vector3 newPoint = new Vector3(topRoofPoints[x].x, bargeboardPoints[y].y - heightDif, bargeboardPoints[y].z);
                vertices.Add(newPoint);
            }
        }
        #endregion

        #region Triangles
        for (int x = 0; x < topRoofPoints.Count - 1; x++)
        {
            for (int y = 0; y < bargeboardPoints.Count - 1; y++)
            {
                triangles.Add(x * bargeboardPoints.Count + y);
                triangles.Add(x * bargeboardPoints.Count + y + 1);
                triangles.Add((x + 1) * bargeboardPoints.Count + y);

                triangles.Add(x * bargeboardPoints.Count + y + 1);
                triangles.Add((x + 1) * bargeboardPoints.Count + y + 1);
                triangles.Add((x + 1) * bargeboardPoints.Count + y);
            }
        }
        #endregion

        #region UV
        // 長寬的距離
        //float widthDistance = roofLength * 2 / UVScale;
        //float heighDistance = Vector3.Distance(bargeboardPoints[0], bargeboardPoints[bargeboardPoints.Count - 1]) / UVScale;
        float widthDistance = UVScale;
        float heighDistance = UVScale;
        CaculateUV(uvs, vertices, widthDistance, heighDistance);
        #endregion


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topRidgePoints"></param>
    /// <param name="rightRidgePoints"></param>
    /// <param name="roofLength"></param>
    /// <param name="roofDeep"></param>
    /// <param name="rafterRes"></param>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public Mesh CreateTopRoofMesh(List<Vector3> topRidgePoints, List<Vector3> rightRidgePoints, float roofLength, float roofDeep, int rafterRes, Mesh mesh)
    {
        List<Vector3> topRoofPoints = new List<Vector3>();
        int lengthCutNum = (int)roofLength * 3;
        if (rightRidgePoints.Count < rafterRes)
        {
            rafterRes = 1;
        }
        for (int i = 0; i < rafterRes; i++)
        {
            topRoofPoints.Add(rightRidgePoints[i]);
        }
        for (int i = 1; i < lengthCutNum; i++)
        {
            Vector3 newPoint = (rightRidgePoints[rafterRes - 1] * (lengthCutNum - i) / lengthCutNum) + (rightRidgePoints[rafterRes] * i / lengthCutNum);
            topRoofPoints.Add(newPoint);
        }
        for (int i = rafterRes; i < rightRidgePoints.Count; i++)
        {
            topRoofPoints.Add(rightRidgePoints[i]);
        }

        List<Vector3> deepRoofPoints = new List<Vector3>();
        int deepCutNum = (int)roofDeep * 3;
        if (topRidgePoints.Count < rafterRes)
        {
            rafterRes = 1;
        }
        for (int i = 0; i < rafterRes; i++)
        {
            deepRoofPoints.Add(topRidgePoints[i]);
        }
        for (int i = 0; i < deepCutNum; i++)
        {
            Vector3 newPoint = (topRidgePoints[rafterRes - 1] * (deepCutNum - i) / deepCutNum) + (topRidgePoints[rafterRes] * i / deepCutNum);
            deepRoofPoints.Add(newPoint);
        }
        for (int i = rafterRes; i < topRidgePoints.Count; i++)
        {
            deepRoofPoints.Add(topRidgePoints[i]);
        }

        //節點
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        #region Vertices
        for (int i = 0; i < deepRoofPoints.Count; i++)
        {
            for (int j = 0; j < topRoofPoints.Count; j++)
            {
                Vector3 newPoint = new Vector3(topRoofPoints[j].x, topRoofPoints[j].y, deepRoofPoints[i].z);
                vertices.Add(newPoint);
            }
        }
        #endregion
        #region Triangles
        for (int x = 0; x < deepRoofPoints.Count - 1; x++)
        {
            for (int y = 0; y < topRoofPoints.Count - 1; y++)
            {
                triangles.Add(x * topRoofPoints.Count + y);
                triangles.Add((x + 1) * topRoofPoints.Count + y);
                triangles.Add(x * topRoofPoints.Count + y + 1);

                triangles.Add(x * topRoofPoints.Count + y + 1);
                triangles.Add((x + 1) * topRoofPoints.Count + y);
                triangles.Add((x + 1) * topRoofPoints.Count + y + 1);
            }
        }
        #endregion
        #region UV
        // 長寬的距離
        float widthDistance = roofLength * 2;
        float heighDistance = roofDeep * 2;

        uvs.Add(new Vector2(1, 1));

        for (int i = 1; i < vertices.Count; i++)
        {
            float vecDisY = Mathf.Sqrt(Mathf.Pow((vertices[0].y - vertices[i].y), 2) + Mathf.Pow((vertices[0].z - vertices[i].z), 2)) / heighDistance;
            float vecDisX = Mathf.Abs(vertices[0].x - vertices[i].x) / widthDistance;
            Vector2 newUV = new Vector2(uvs[0].x + vecDisX, uvs[0].y + vecDisY);
            uvs.Add(newUV);
        }
        #endregion

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flyingRafterPoints"></param>
    /// <param name="bargeboardPoints"></param>
    /// <param name="wingAngleCurve"></param>
    /// <param name="roofLength"></param>
    /// <param name="flyingBargePointNum"></param>
    /// <param name="flyingRafterY"></param>
    /// <param name="flyingRafterZ"></param>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public Mesh CreateFlyRoofMesh(List<Vector3> flyingRafterPoints, List<Vector3> bargeboardPoints, List<Vector3> wingAngleCurve, float roofLength, int flyingBargePointNum, float flyingRafterY, float flyingRafterZ, Mesh mesh)
    {
        int UVScale = 4;

        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        // offset是對飛簷的位置調整
        Vector3 offset = new Vector3(0, flyingRafterY, flyingRafterZ);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            flyingRafterPoints[i] = rotateEuler * flyingRafterPoints[i] + offset;
        }
        //節點
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        #region Vertices
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            int bargeboardPoint = i + (bargeboardPoints.Count - flyingBargePointNum) - 1;
            vertices.Add(bargeboardPoints[bargeboardPoint]);

            for (int j = 1; j <= i; j++)
            {
                Vector3 newPoint = (bargeboardPoints[bargeboardPoint] * (i - j) / i) + (flyingRafterPoints[i] * (j) / (i));
                if (j != i)
                {
                    newPoint.y = newPoint.y + ((wingAngleCurve[j].y) * ((float)i / flyingRafterPoints.Count));
                }
                vertices.Add(newPoint);
            }
        }
        #endregion

        #region Triangles
        for (int i = 0, counter = 0; i < flyingBargePointNum; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                triangles.Add(counter);
                triangles.Add(counter + (i + 1));
                triangles.Add(counter + (i + 2));

                if (j != i)
                {
                    triangles.Add(counter + (i + 2));
                    triangles.Add(counter + 1);
                    triangles.Add(counter);
                }
                counter++;
                // triangles.Add(j + i * 2 + 1);
            }
        }
        #endregion

        #region UV
        // 長寬的距離
        //float widthDistance = roofLength * 2 / UVScale;
        //float heighDistance = Vector3.Distance(bargeboardPoints[0], bargeboardPoints[bargeboardPoints.Count - 1]) / UVScale;
        float widthDistance = UVScale;
        float heighDistance = UVScale;
        float offsetY = bargeboardPoints[flyingBargePointNum].y / bargeboardPoints[bargeboardPoints.Count - 1].y * UVScale;

        CaculateUV(uvs, vertices, widthDistance, heighDistance);
        for (int i = 0; i < uvs.Count; i++)
        {
            Vector2 newPoint = uvs[i];
            newPoint.y = newPoint.y + offsetY;
            uvs[i] = newPoint;
        }
        #endregion

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    public Mesh CreateSideRoofMesh(List<Vector3> flyingRafterPoints, List<Vector3> bargeboardPoints, List<Vector3> wingAngleCurve, float flyingRafterY, float flyingRafterZ, int flyingBargePointNum, float roofLength, float res, Mesh mesh)
    {
        //節點
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        // offset是對飛簷的位置調整
        Vector3 offset = new Vector3(0, flyingRafterY, 0);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            flyingRafterPoints[i] = rotateEuler * flyingRafterPoints[i] + offset;
        }

        #region Vertices
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            int bargeboardPoint = i + (bargeboardPoints.Count - flyingBargePointNum) - 1;
            vertices.Add(bargeboardPoints[bargeboardPoint]);

            for (int j = 1; j <= i; j++)
            {
                Vector3 newPoint = (bargeboardPoints[bargeboardPoint] * (i - j) / i) + (flyingRafterPoints[i] * (j) / (i));
                vertices.Add(newPoint);
            }
        }
        #endregion
        DebugHandler debugHandler = FindObjectOfType<DebugHandler>();
        debugHandler.AddPoints(vertices);
        #region Triangles
        #endregion

        #region UV
        #endregion

        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = triangles.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private void CaculateUV(List<Vector2> uvs, List<Vector3> vertices, float widthDistance, float heighDistance)
    {
        uvs.Add(new Vector2(1, 1));

        for (int i = 1; i < vertices.Count; i++)
        {
            float vecDisY = Mathf.Sqrt(Mathf.Pow((vertices[0].y - vertices[i].y), 2) + Mathf.Pow((vertices[0].z - vertices[i].z), 2)) / heighDistance;
            float vecDisX = Mathf.Abs(vertices[0].x - vertices[i].x) / widthDistance;
            Vector2 newUV = new Vector2(uvs[0].x - vecDisX, uvs[0].y - vecDisY);
            uvs.Add(newUV);
        }
    }
}
