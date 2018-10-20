using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofCreator : MonoBehaviour
{
    public GameObject eavesTilePrefab;
    public GameObject dripTilePrefab;
    public GameObject baoDingPrefab;
    public Material frontRoofMaterial;
    public Material topRoofMaterial;
    public Material insideRoofMaterial;
    public Material roofMaterial;
    private int UVScale = 4;
    private List<Vector3> roofTileCurve;

    public GameObject CreateTopRoof(List<Vector3> topRidgePoints, List<Vector3> rightRidgePoints, float roofLength, float roofDeep, int rafterRes)
    {
        GameObject roof = new GameObject();
        roof.name = "TopRoof";
        //加入mesh
        Mesh mesh = AddMeshRenderer(roof, topRoofMaterial);
        //生成mesh
        MeshRoof meshRoof = new MeshRoof();
        mesh = meshRoof.CreateTopRoofMesh(topRidgePoints, rightRidgePoints, roofLength, roofDeep, rafterRes, mesh);
        roof.GetComponent<MeshFilter>().mesh = mesh;
        return roof;
    }

    /// <summary>
    /// 創造屋面
    /// </summary>
    /// <param name="flyingRafterPoints">翼角的點</param>
    /// <param name="bargeboardPoints">垂脊的點</param>
    /// <param name="topRoofPoints">正脊的點</param>
    /// <param name="length">正脊的長度</param>
    /// <param name="roof">屋頂的參數</param>
    /// <param name="reverse">是否反轉?</param>
    /// <returns></returns>
    public GameObject CreateRoofEaves(List<Vector3> flyingRafterPoints, List<Vector3> bargeboardPoints, List<Vector3> topRoofPoints, float length, Roof roof, bool reverse)
    {
        int flyingBargePointNum = flyingRafterPoints.Count - 1;

        GameObject roofObj = new GameObject();
        //加入mesh
        Mesh mesh;
        if (!reverse)
        {
            mesh = AddMeshRenderer(roofObj, roofMaterial);
        }
        else
        {
            mesh = AddMeshRenderer(roofObj, insideRoofMaterial);
        }
        //生成mesh
        MeshRoof meshRoof = new MeshRoof();

        //節點
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        #region Vertices
        // 飛簷的最大位置
        int frMax = flyingRafterPoints.Count - 1;
        //左側翼角的點
        int rightPartPointsCount = 0;
        int centerPartPointsCount = 0;
        int leftPartPointsCount = 0;
        List<Vector3> wingAngleCurve = new List<Vector3>();
        //如果有翼角的話
        if (frMax >= 0)
        {
            CircleCurve circleCurve = new CircleCurve();
            wingAngleCurve = circleCurve.CreateCircleCurve(0, (flyingRafterPoints[frMax]).x, roof.wingAngleCurve, frMax);
            roofTileCurve = wingAngleCurve;

            // 算右側的翼角的 vertices
            for (int i = frMax; i >= 1; i--)
            {
                Vector3 newPoint = flyingRafterPoints[i];
                newPoint.x = -(newPoint.x + length / 2);
                vertices.Add(newPoint);

                float jStart = bargeboardPoints.Count - (frMax - i);
                for (int j = (int)jStart; j < bargeboardPoints.Count; j++)
                {
                    //算內插 數學!
                    int a = j - bargeboardPoints.Count + flyingRafterPoints.Count;
                    int b = i;
                    newPoint = (bargeboardPoints[j] * ((float)(a - b) / a)) + (flyingRafterPoints[a] * ((float)b / a));
                    newPoint.x = -(newPoint.x + length / 2);
                    float yOffset = ((j - jStart + 1) / (bargeboardPoints.Count - jStart));
                    newPoint.y = newPoint.y + wingAngleCurve[i].y * yOffset;
                    //newPoint.z = bargeboardPoints[j].z;
                    vertices.Add(newPoint);
                }
            }
            rightPartPointsCount = vertices.Count;
        }
        if (length >= 0.01f)
        {
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
        }
        else
        {
            for (int y = 0; y < bargeboardPoints.Count; y++)
            {
                vertices.Add(bargeboardPoints[y]);
            }
        }
        //  算中間的面的 vertices
        centerPartPointsCount = vertices.Count;

        //如果有翼角的話
        if (frMax >= 0)
        {
            // 算左側的翼角的 vertices
            for (int i = 1; i < frMax + 1; i++)
            {
                Vector3 newPoint = flyingRafterPoints[i];
                newPoint.x = (newPoint.x + length / 2);
                vertices.Add(newPoint);

                float jStart = bargeboardPoints.Count - (frMax - i);
                for (int j = (int)jStart; j < bargeboardPoints.Count; j++)
                {
                    //算內插 數學!
                    int a = j - bargeboardPoints.Count + flyingRafterPoints.Count;
                    int b = i;
                    newPoint = (bargeboardPoints[j] * ((float)(a - b) / a)) + (flyingRafterPoints[a] * ((float)b / a));
                    newPoint.x = (newPoint.x + length / 2);
                    float yOffset = ((j - jStart + 1) / (bargeboardPoints.Count - jStart));
                    newPoint.y = newPoint.y + wingAngleCurve[i].y * yOffset;
                    // newPoint.y = newPoint.y + wingAngleCurve[i].y * (float)b / a;
                    vertices.Add(newPoint);
                }
            }
            leftPartPointsCount = vertices.Count;
        }
        // Debug.Log(rightPartPointsCount);
        #endregion

        #region Triangles
        // 計算右邊部分的面
        for (int i = 0, counter = 0; i < flyingRafterPoints.Count - 1; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                int a = counter >= rightPartPointsCount ? counter + (bargeboardPoints.Count - flyingBargePointNum - 1) : counter;
                int b = counter + (i + 2) >= rightPartPointsCount ? counter + (i + 2) + (bargeboardPoints.Count - flyingBargePointNum - 1) : counter + (i + 2);
                int c = counter + (i + 1) >= rightPartPointsCount ? counter + (i + 1) + (bargeboardPoints.Count - flyingBargePointNum - 1) : counter + (i + 1);
                if (!reverse)
                {
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);
                    if (j != i)
                    {
                        triangles.Add(b);
                        triangles.Add(a);
                        triangles.Add(a + 1);
                    }
                }
                else
                {
                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(b);
                    if (j != i)
                    {
                        triangles.Add(b);
                        triangles.Add(a + 1);
                        triangles.Add(a);
                    }
                }
                counter++;
            }
        }
        if (length >= 0.01f)
        {
            // 計算中間部分的面
            for (int x = 0; x < topRoofPoints.Count - 1; x++)
            {
                for (int y = rightPartPointsCount; y < bargeboardPoints.Count - 1 + rightPartPointsCount; y++)
                {
                    if (!reverse)
                    {
                        triangles.Add(x * bargeboardPoints.Count + y);
                        triangles.Add(x * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);

                        triangles.Add(x * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);
                    }
                    else
                    {
                        triangles.Add(x * bargeboardPoints.Count + y);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);
                        triangles.Add(x * bargeboardPoints.Count + y + 1);

                        triangles.Add(x * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y + 1);
                    }
                }
            }
        }
        // 計算左邊部分的面
        for (int i = 0, counter = 0; i < frMax; i++)
        {
            for (int j = i; j < frMax; j++)
            {
                int a = counter + centerPartPointsCount - (flyingRafterPoints.Count);
                int b = a + 1;
                int c = b + (frMax - i);
                if (!reverse)
                {
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);
                    if (j != i)
                    {
                        triangles.Add(a);
                        triangles.Add(c);
                        triangles.Add(c - 1);
                    }
                }
                else
                {
                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(b);
                    if (j != i)
                    {
                        triangles.Add(a);
                        triangles.Add(c - 1);
                        triangles.Add(c);
                    }
                }
                counter++;
            }
            counter++;
        }
        #endregion

        #region UV
        float widthDistance = UVScale;
        float heighDistance = UVScale;
        float offsetY;
        if (frMax >= 0)
            offsetY = bargeboardPoints[flyingBargePointNum].y / bargeboardPoints[bargeboardPoints.Count - 1].y * UVScale;
        else
            offsetY = 0;

        CaculateUV(uvs, vertices, rightPartPointsCount, widthDistance, heighDistance);
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

        roofObj.GetComponent<MeshFilter>().mesh = mesh;
        return roofObj;
    }

    public GameObject RemoveOverlap(GameObject obj, int axis = 0, float value = 0, bool less = true)
    {
        List<Vector3> wantedVertices = new List<Vector3>();
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        if(less)
        {
            for(int i = 0; i < mf.mesh.vertices.Length; i++)
            {
                //if(mf.mesh.vertices.)
            }
        }

        return obj;
    }

    public GameObject CreateCombinedEaves(List<Vector3> flyingRafterPoints, List<Vector3> bargeboardPoints, List<Vector3> topRoofPoints, float length, Roof roof, bool reverse)
    {
        int flyingBargePointNum = flyingRafterPoints.Count - 1;

        GameObject roofObj = new GameObject();
        //加入mesh
        Mesh mesh;
        if (!reverse)
        {
            mesh = AddMeshRenderer(roofObj, roofMaterial);
        }
        else
        {
            mesh = AddMeshRenderer(roofObj, insideRoofMaterial);
        }
        //生成mesh
        MeshRoof meshRoof = new MeshRoof();

        //節點
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        #region Vertices
        // 飛簷的最大位置
        int frMax = flyingRafterPoints.Count - 1;
        //左側翼角的點
        int rightPartPointsCount = 0;
        int centerPartPointsCount = 0;
        int leftPartPointsCount = 0;
        List<Vector3> wingAngleCurve = new List<Vector3>();
        //如果有翼角的話
        if (frMax >= 0)
        {
            CircleCurve circleCurve = new CircleCurve();
            wingAngleCurve = circleCurve.CreateCircleCurve(0, (flyingRafterPoints[frMax]).x, roof.wingAngleCurve, frMax);
            roofTileCurve = wingAngleCurve;

            // 算右側的翼角的 vertices
            for (int i = frMax; i >= 1; i--)
            {
                Vector3 newPoint = flyingRafterPoints[i];
                newPoint.x = -(newPoint.x + length / 2);
                vertices.Add(newPoint);

                float jStart = bargeboardPoints.Count - (frMax - i);
                for (int j = (int)jStart; j < bargeboardPoints.Count; j++)
                {
                    //算內插 數學!
                    int a = j - bargeboardPoints.Count + flyingRafterPoints.Count;
                    int b = i;
                    newPoint = (bargeboardPoints[j] * ((float)(a - b) / a)) + (flyingRafterPoints[a] * ((float)b / a));
                    newPoint.x = -(newPoint.x + length / 2);
                    float yOffset = ((j - jStart + 1) / (bargeboardPoints.Count - jStart));
                    newPoint.y = newPoint.y + wingAngleCurve[i].y * yOffset;
                    //newPoint.z = bargeboardPoints[j].z;
                    vertices.Add(newPoint);
                }
            }
            rightPartPointsCount = vertices.Count;
        }
        if (length >= 0.01f)
        {
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
        }
        else
        {
            for (int y = 0; y < bargeboardPoints.Count; y++)
            {
                vertices.Add(bargeboardPoints[y]);
            }
        }
        //  算中間的面的 vertices
        centerPartPointsCount = vertices.Count;

        //如果有翼角的話
        if (frMax >= 0)
        {
            // 算左側的翼角的 vertices
            for (int i = 1; i < frMax + 1; i++)
            {
                Vector3 newPoint = flyingRafterPoints[i];
                newPoint.x = (newPoint.x + length / 2);
                vertices.Add(newPoint);

                float jStart = bargeboardPoints.Count - (frMax - i);
                for (int j = (int)jStart; j < bargeboardPoints.Count; j++)
                {
                    //算內插 數學!
                    int a = j - bargeboardPoints.Count + flyingRafterPoints.Count;
                    int b = i;
                    newPoint = (bargeboardPoints[j] * ((float)(a - b) / a)) + (flyingRafterPoints[a] * ((float)b / a));
                    newPoint.x = (newPoint.x + length / 2);
                    float yOffset = ((j - jStart + 1) / (bargeboardPoints.Count - jStart));
                    newPoint.y = newPoint.y + wingAngleCurve[i].y * yOffset;
                    // newPoint.y = newPoint.y + wingAngleCurve[i].y * (float)b / a;
                    vertices.Add(newPoint);
                }
            }
            leftPartPointsCount = vertices.Count;
        }
        // Debug.Log(rightPartPointsCount);
        #endregion

        #region Triangles
        // 計算右邊部分的面
        for (int i = 0, counter = 0; i < flyingRafterPoints.Count - 1; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                int a = counter >= rightPartPointsCount ? counter + (bargeboardPoints.Count - flyingBargePointNum - 1) : counter;
                int b = counter + (i + 2) >= rightPartPointsCount ? counter + (i + 2) + (bargeboardPoints.Count - flyingBargePointNum - 1) : counter + (i + 2);
                int c = counter + (i + 1) >= rightPartPointsCount ? counter + (i + 1) + (bargeboardPoints.Count - flyingBargePointNum - 1) : counter + (i + 1);
                if (!reverse)
                {
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);
                    if (j != i)
                    {
                        triangles.Add(b);
                        triangles.Add(a);
                        triangles.Add(a + 1);
                    }
                }
                else
                {
                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(b);
                    if (j != i)
                    {
                        triangles.Add(b);
                        triangles.Add(a + 1);
                        triangles.Add(a);
                    }
                }
                counter++;
            }
        }
        if (length >= 0.01f)
        {
            // 計算中間部分的面
            for (int x = 0; x < topRoofPoints.Count - 1; x++)
            {
                for (int y = rightPartPointsCount; y < bargeboardPoints.Count - 1 + rightPartPointsCount; y++)
                {
                    if (!reverse)
                    {
                        triangles.Add(x * bargeboardPoints.Count + y);
                        triangles.Add(x * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);

                        triangles.Add(x * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);
                    }
                    else
                    {
                        triangles.Add(x * bargeboardPoints.Count + y);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);
                        triangles.Add(x * bargeboardPoints.Count + y + 1);

                        triangles.Add(x * bargeboardPoints.Count + y + 1);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y);
                        triangles.Add((x + 1) * bargeboardPoints.Count + y + 1);
                    }
                }
            }
        }
        // 計算左邊部分的面
        for (int i = 0, counter = 0; i < frMax; i++)
        {
            for (int j = i; j < frMax; j++)
            {
                int a = counter + centerPartPointsCount - (flyingRafterPoints.Count);
                int b = a + 1;
                int c = b + (frMax - i);
                if (!reverse)
                {
                    triangles.Add(a);
                    triangles.Add(b);
                    triangles.Add(c);
                    if (j != i)
                    {
                        triangles.Add(a);
                        triangles.Add(c);
                        triangles.Add(c - 1);
                    }
                }
                else
                {
                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(b);
                    if (j != i)
                    {
                        triangles.Add(a);
                        triangles.Add(c - 1);
                        triangles.Add(c);
                    }
                }
                counter++;
            }
            counter++;
        }
        #endregion

        #region UV
        float widthDistance = UVScale;
        float heighDistance = UVScale;
        float offsetY;
        if (frMax >= 0)
            offsetY = bargeboardPoints[flyingBargePointNum].y / bargeboardPoints[bargeboardPoints.Count - 1].y * UVScale;
        else
            offsetY = 0;

        CaculateUV(uvs, vertices, rightPartPointsCount, widthDistance, heighDistance);
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

        roofObj.GetComponent<MeshFilter>().mesh = mesh;
        return roofObj;
    }
    
    /// <summary>
    /// 銅瓦與滴水
    /// </summary>
    /// <param name="length"></param>
    /// <param name="roof"></param>
    /// <param name="flyingRafferPointX"></param>
    /// <param name="flyingRafterPoints"></param>
    /// <returns></returns>
    public GameObject CreateRoofEavesTile(float length, Roof roof, float flyingRafferPointX, List<Vector3> flyingRafterPoints)
    {
        //分母不能為0
        length = length < 0.05f ? 0.05f : length;
        GameObject eavesTilesObj = new GameObject();
        List<Transform> eavesTiles = new List<Transform>();
        eavesTilesObj.name = "tiles";
        float sideCount = flyingRafferPointX * 1.2f;
        float midCount = length * 1.2f;
        #region 銅瓦
        for (int i = (int)-sideCount; i < midCount + (sideCount - 0.5); i++)
        {
            Vector3 vec3 = new Vector3(-length / 2 + (length * (i / midCount)) + 0.03f, 0, -0.1f);

            GameObject tile = Instantiate(this.eavesTilePrefab);
            tile.transform.Translate(vec3);
            if (i < 0)
            {
                float current = roofTileCurve.Count * (i / (-sideCount)) - 1;
                float upperY = roofTileCurve[(int)Mathf.Ceil(current)].y;
                float lowerY = roofTileCurve[(int)Mathf.Floor(current)].y;
                float point = current - Mathf.Floor(current);
                float heightCurveYOffset = upperY * ((1 - point) / 1) + lowerY * (point / 1);
                tile.transform.Translate(new Vector3(0, 0, -(heightCurveYOffset + roof.sideEaveHeight * (i / -sideCount + 0.01f))));
            }
            if (i >= midCount)
            {
                float current = roofTileCurve.Count * ((i - midCount) / sideCount);
                float upperY = roofTileCurve[(int)Mathf.Ceil(current)].y;
                float lowerY = roofTileCurve[(int)Mathf.Floor(current)].y;
                float point = current - Mathf.Floor(current);
                float heightCurveYOffset = upperY * (point / 1) + lowerY * ((1 - point) / 1);
                tile.transform.Translate(new Vector3(0, 0, -heightCurveYOffset - (roof.sideEaveHeight) * (((float)i - midCount + 0.25f) / sideCount)));
            }
            eavesTiles.Add(tile.transform);
            tile.transform.parent = eavesTilesObj.transform;
        }
        #endregion

        #region 滴水
        float baseDistance = 0.83333f;
        for (int i = -1; i < eavesTiles.Count; i++)
        {
            Vector3 currentPoint;
            Vector3 nextPoint;
            if (i == -1)
            {
                currentPoint = new Vector3(
                   -length / 2 + (length * ((-sideCount) / midCount)), roofTileCurve[roofTileCurve.Count - 1].y + roof.sideEaveHeight, 0);

                nextPoint = eavesTiles[i + 1].position;
            }
            else if (i == eavesTiles.Count - 1)
            {
                nextPoint = new Vector3(
                 -(-length / 2 + (length * (-sideCount / midCount))), roofTileCurve[roofTileCurve.Count - 1].y + roof.sideEaveHeight, 0);
                currentPoint = eavesTiles[i].position;
            }
            else
            {
                currentPoint = eavesTiles[i].position;
                nextPoint = eavesTiles[i + 1].position;
            }
            //兩個銅瓦的中間點
            Vector3 midPoint = (currentPoint + nextPoint) / 2;
            //兩個銅瓦間的距離
            float currentDistance = Vector3.Distance(currentPoint, nextPoint);
            if (currentDistance < 0.25f)
                continue;
            //兩個銅瓦間的角度
            float rotateAngle = Mathf.Atan((currentPoint.y - nextPoint.y) / (currentPoint.x - nextPoint.x)) * 180f / Mathf.PI;

            GameObject dripTile = Instantiate(dripTilePrefab);
            dripTile.transform.Translate(midPoint);
            dripTile.transform.localScale = new Vector3(currentDistance / baseDistance, 1, 1);
            dripTile.transform.Rotate(new Vector3(0, 0, rotateAngle));
            dripTile.transform.parent = eavesTilesObj.transform;
        }
        #endregion

        return eavesTilesObj;
    }

    /// <summary>
    /// 寶頂
    /// </summary>
    /// <returns></returns>
    public GameObject CreateBaoDing()
    {
        GameObject baoDingObj = Instantiate(baoDingPrefab);

        return baoDingObj;
    }

    /// <summary>
    /// 計算UV
    /// </summary>
    /// <param name="uvs"></param>
    /// <param name="vertices"></param>
    /// <param name="pivotPoint"></param>
    /// <param name="widthDistance"></param>
    /// <param name="heighDistance"></param>
    private void CaculateUV(List<Vector2> uvs, List<Vector3> vertices, int pivotPoint, float widthDistance, float heighDistance)
    {
        uvs.Add(new Vector2(0, 0));

        for (int i = 1; i < vertices.Count; i++)
        {
            float vecDisY = Mathf.Sqrt(Mathf.Pow((vertices[pivotPoint].y - vertices[i].y), 2) + Mathf.Pow((vertices[pivotPoint].z - vertices[i].z), 2)) / heighDistance;
            float vecDisX = (vertices[pivotPoint].x - vertices[i].x) / widthDistance;
            Vector2 newUV = new Vector2(uvs[0].x - vecDisX, uvs[0].y - vecDisY);
            uvs.Add(newUV);
        }
    }

    //public GameObject CreateSideRoofWall(List<Vector3> bargeboardPoints, float roofLength, int flyingBargePointNum, float roofDeep)
    //{
    //    //飛簷以上的點
    //    List<Vector3> bargeboardWallPoint = new List<Vector3>();
    //    List<Vector3> emptyVec = new List<Vector3>();
    //    for (int i = 0; i < bargeboardPoints.Count - flyingBargePointNum; i++)
    //    {
    //        bargeboardWallPoint.Add(bargeboardPoints[i]);
    //    }
    //    GameObject roof = new GameObject();
    //    roof.name = "SideRoof";
    //    //加入mesh
    //    Mesh mesh = AddMeshRenderer(roof, sideRoofWallMaterial);
    //    //生成mesh
    //    MeshRoof meshRoof = new MeshRoof();
    //    mesh = meshRoof.CreateSideRoofMesh(bargeboardWallPoint, bargeboardPoints, emptyVec, flyingBargePointNum, roofLength, roofDeep, mesh);
    //    roof.GetComponent<MeshFilter>().mesh = mesh;
    //    return roof;
    //}

    /// <summary>
    /// 加入畫mesh所需的部件
    /// </summary>
    /// <param name="roof">要加入的物件</param>
    /// <returns>回傳mesh</returns>
    public Mesh AddMeshRenderer(GameObject roof, Material material)
    {
        MeshFilter filter = roof.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();
        MeshRenderer meshRenderer = roof.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        return mesh;
    }
}
