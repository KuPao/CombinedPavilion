using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChinesePavilionCreator : MonoBehaviour
{
    public GameObject chinesePavilionCreater;

    [Header("Platform")]
    //幾層
    public int platformNum;
    //平台大小
    public Vector3 platformSize;
    //平台底部半徑
    public float platformBottomRadius;
    //平台上面半徑
    public float platformTopRadius;

    [Header("EaveColume")]
    //幾個柱子
    public int eaveColumeNum;
    //柱子大小
    public float eaveColumeHigh;
    //柱子半徑
    public float eaveColumeRadius;

    [Header("Rafter")]
    public int circleCurveRes;
    public int rafterNbSides;
    public int rafterRes;
    private float rafterHeight;
    public float rafterTall;
    public float rafterRadius;
    public float flyEaveHeight;
    public float flyEaveLength;

    [Header("Roof")]
    public int roofRes;
    public float roofEaveHeight;

    private Roof roof;
    // 建築物實體物件
    private GameObject building;
    // 屋頂實體物件
    private GameObject roofObject;
    // 屋身實體物件
    private GameObject bodyObject;
    // 平台實體物件
    private GameObject platformObject;
    // 正脊前方的點
    private List<Vector3> rightRidgeFrontPoints = new List<Vector3>();
    // 正脊側邊的點
    private List<Vector3> rightRidgeSidePoints;
    //垂脊的點
    private List<Vector3> bargeboardPoints;
    //飛簷的點
    private List<Vector3> flyingRafterPoints;
    //垂脊與飛簷接合的點
    private int flyingBargePointNum;
    // 飛簷座標 這邊沒寫好
    float flyingRafterZ;

    //當前高度
    private float CurrentHeight;
    //柱子的位置
    private Vector3[] columnsPos;

    // Use this for initialization
    void Start()
    {
    }
    public void CreateSetUp()
    {
        if (roofObject != null)
        {
            Destroy(building);
        }
        building = new GameObject();
        building.name = "ChinesePavilion";
    }
    /// <summary>
    /// 生成屋頂
    /// </summary>
    /// <param name="roof"></param>
    /// <returns></returns>
    public GameObject CreateRoof(Roof roof)
    {
        this.roof = roof;
        CurrentHeight = 0;
        roofObject = new GameObject();
        roofObject.name = "Roof";
        roofObject.transform.parent = building.transform;
        switch(roof.combineType)
        {
            case 0:
                //創建正脊
                CreateRightRidge();
                //創建垂脊
                CreateBargeboard();
                //創建戧脊翼腳
                CreateFlyingRafter();
                //生成屋頂Mesh
                DrawRoofMesh();
                break;
            case 1:
                //創建正脊
                CreateRightRidge();
                //創建垂脊
                CreateBargeboard();
                //創建方勝亭的脊
                CreateFangShengRidge();
                //生成屋頂Mesh
                DrawFangShengRoofMesh();
                break;
            default:
                break;
        }
        //屋頂的裝飾
        //PutRoofDecoration();

        return roofObject;
    }

    /// <summary>
    /// 生成屋身
    /// </summary>
    /// <param name="body"></param>
    public void CreateBody(Body body)
    {
        roofObject.transform.Translate(0, roof.height - roof.topLowerHeight + body.height, 0);

        bodyObject = new GameObject();
        bodyObject.name = "Body";
        bodyObject.transform.parent = building.transform;

        float lastPointX;
        EaveColumnCreator creator = chinesePavilionCreater.GetComponent<EaveColumnCreator>();
        // GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        List<GameObject> cylinders = new List<GameObject>();
        // 先創立四邊的柱子
        for (int i = 0; i < 4; i++)
        {
            GameObject cylinder = creator.CreateEaveColumn(body.height, body.radius);
            cylinders.Add(cylinder);
        }

        if (roof.width <= roof.sideEaveStart)
            lastPointX = 0;
        else
            lastPointX = -flyingRafterPoints[flyingRafterPoints.Count - 1].x / 2;

        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.length / 2, flyingRafterY, flyingRafterZ + roof.deep / 2);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
        int pointNum = (int)((newFlyingRafterPoints.Count - 1) * (body.pos / 10));

        float cylinderX = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].x : offset.x;
        float cylinderZ = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].z : offset.z;

        cylinders[0].transform.position = new Vector3(cylinderX, 0, cylinderZ);
        cylinders[1].transform.position = new Vector3(-cylinderX, 0, cylinderZ);
        cylinders[2].transform.position = new Vector3(-cylinderX, 0, -cylinderZ);
        cylinders[3].transform.position = new Vector3(cylinderX, 0, -cylinderZ);

        foreach (GameObject cylinder in cylinders)
        {
            cylinder.transform.parent = bodyObject.transform;
        }
        #region  額枋 楣子 雀替
        #region  生成所有額枋 並調整位置
        GameObject architraveObj = new GameObject();
        architraveObj.name = "Architraves";
        float architravesHeight = 0.75f;

        GameObject architrave1 = creator.CreateArchitrave(cylinderX * 2, architravesHeight);
        GameObject architrave2 = creator.CreateArchitrave(cylinderZ * 2, architravesHeight);
        List<GameObject> architraves = new List<GameObject>()
        {
            architrave1,
            Instantiate(architrave1),
            architrave2,
            Instantiate(architrave2),
        };
        architraves[0].transform.position = new Vector3(0, body.height - architravesHeight / 2, cylinderZ);
        architraves[1].transform.position = new Vector3(0, body.height - architravesHeight / 2, -cylinderZ);
        architraves[2].transform.position = new Vector3(cylinderX, body.height - architravesHeight / 2, 0);
        architraves[3].transform.position = new Vector3(-cylinderX, body.height - architravesHeight / 2, 0);
        architraves[2].transform.Rotate(new Vector3(0, 90, 0));
        architraves[3].transform.Rotate(new Vector3(0, 90, 0));
        foreach (GameObject obj in architraves)
        {
            obj.transform.parent = architraveObj.transform;
        }
        architraveObj.transform.parent = bodyObject.transform;
        #endregion

        #region  生成所有楣子 並調整位置
        GameObject meiZiObj = new GameObject();
        meiZiObj.name = "MeiZis";
        float meiZiHeight = 0.75f;
        GameObject meiZi1 = creator.CreateMeiZi(cylinderX * 2, meiZiHeight);
        GameObject meiZi2 = creator.CreateMeiZi(cylinderZ * 2, meiZiHeight);
        List<GameObject> meiZis = new List<GameObject>()
        {
            meiZi1,
            Instantiate(meiZi1),
            meiZi2,
            Instantiate(meiZi2),
        };
        meiZis[0].transform.position = new Vector3(0, body.height - meiZiHeight / 2 - architravesHeight, cylinderZ);
        meiZis[1].transform.position = new Vector3(0, body.height - meiZiHeight / 2 - architravesHeight, -cylinderZ);
        meiZis[2].transform.position = new Vector3(cylinderX, body.height - meiZiHeight / 2 - architravesHeight, 0);
        meiZis[3].transform.position = new Vector3(-cylinderX, body.height - meiZiHeight / 2 - architravesHeight, 0);
        meiZis[2].transform.Rotate(new Vector3(0, 90, 0));
        meiZis[3].transform.Rotate(new Vector3(0, 90, 0));
        foreach (GameObject obj in meiZis)
        {
            obj.transform.parent = meiZiObj.transform;
        }
        meiZiObj.transform.parent = bodyObject.transform;
        #endregion

        #region  生成所有雀替 並調整位置

        GameObject sparrowBraceObj = new GameObject();
        sparrowBraceObj.name = "SparrowBrace";
        float sparrowBraceHeight = 0.75f;
        GameObject sparrowBrace = creator.CreateSparrowBrace(cylinderX * 2, sparrowBraceHeight);


        List<GameObject> sparrowBraces = new List<GameObject>();
        sparrowBraces.Add(sparrowBrace);
        for (int i = 0; i < 7; i++)
        {
            sparrowBraces.Add(Instantiate(sparrowBrace));
        }
        sparrowBraces[0].transform.position = new Vector3(-(cylinderX - body.radius / 2), body.height - architravesHeight - meiZiHeight, cylinderZ);
        sparrowBraces[1].transform.position = new Vector3((cylinderX - body.radius / 2), body.height - architravesHeight - meiZiHeight, cylinderZ);
        sparrowBraces[2].transform.position = new Vector3(-(cylinderX - body.radius / 2), body.height - architravesHeight - meiZiHeight, -cylinderZ);
        sparrowBraces[3].transform.position = new Vector3((cylinderX - body.radius / 2), body.height - architravesHeight - meiZiHeight, -cylinderZ);

        sparrowBraces[4].transform.position = new Vector3(cylinderX, body.height - architravesHeight - meiZiHeight, cylinderZ - body.radius / 2);
        sparrowBraces[5].transform.position = new Vector3(cylinderX, body.height - architravesHeight - meiZiHeight, -(cylinderZ - body.radius / 2));
        sparrowBraces[6].transform.position = new Vector3(-cylinderX, body.height - architravesHeight - meiZiHeight, cylinderZ - body.radius / 2);
        sparrowBraces[7].transform.position = new Vector3(-cylinderX, body.height - architravesHeight - meiZiHeight, -(cylinderZ - body.radius / 2));

        sparrowBraces[1].transform.Rotate(new Vector3(0, 180, 0));
        sparrowBraces[3].transform.Rotate(new Vector3(0, 180, 0));
        sparrowBraces[4].transform.Rotate(new Vector3(0, 90, 0));
        sparrowBraces[5].transform.Rotate(new Vector3(0, -90, 0));
        sparrowBraces[6].transform.Rotate(new Vector3(0, 90, 0));
        sparrowBraces[7].transform.Rotate(new Vector3(0, -90, 0));

        foreach (GameObject obj in sparrowBraces)
        {
            obj.transform.parent = sparrowBraceObj.transform;
        }
        sparrowBraceObj.transform.parent = bodyObject.transform;
        #endregion

        #region  生成所有欄杆 並調整位置
        GameObject friezeInObj = new GameObject();
        friezeInObj.name = "Friezes";
        float friezeHeight = 0.75f;
        GameObject frieze1 = creator.CreateFriezeIn(cylinderX * 2, meiZiHeight);
        GameObject frieze2 = creator.CreateFriezeIn(cylinderZ * 2, meiZiHeight);
        List<GameObject> friezes = new List<GameObject>()
        {
            frieze1,
            Instantiate(frieze1),
            frieze2,
            Instantiate(frieze2),
        };
        friezes[0].transform.position = new Vector3(0, 0, cylinderZ);
        friezes[1].transform.position = new Vector3(0, 0, -cylinderZ);
        friezes[2].transform.position = new Vector3(cylinderX, 0, 0);
        friezes[3].transform.position = new Vector3(-cylinderX, 0, 0);
        friezes[2].transform.Rotate(new Vector3(0, 90, 0));
        friezes[3].transform.Rotate(new Vector3(0, 90, 0));
        foreach (GameObject obj in friezes)
        {
            obj.transform.parent = friezeInObj.transform;
        }
        friezes[1].SetActive(false);
        friezeInObj.transform.parent = bodyObject.transform;
        #endregion

        #endregion
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    /// <param name="platform"></param>
    public void CreatePlatform(Platform platform)
    {
        PlatformCreator platformCreator = FindObjectOfType<PlatformCreator>();
        roofObject.transform.Translate(0, platform.height * 2, 0);
        bodyObject.transform.Translate(0, platform.height * 2, 0);

        platformObject = new GameObject();
        platformObject.name = "Platform";

        GameObject platformBody;
        // 飛簷最右邊的點
        float lastPointX;
        if (roof.width <= roof.sideEaveStart)
            lastPointX = 0;
        else
            lastPointX = -flyingRafterPoints[flyingRafterPoints.Count - 1].x;

        float platformX = (roof.length / 2 + lastPointX) * 2 + platform.length;
        float platformZ = (roof.width + roof.deep / 2) * 2 + platform.width;

        platformBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platformBody.transform.localScale = new Vector3(platformX, platform.height * 2, platformZ);
        platformBody.GetComponent<MeshRenderer>().material = platformCreator.platFormMaterial;
        platformBody.name = "platform";

        // 邊界
        GameObject edges1 = platformCreator.CreateEdges(4, platform.height, platformX, platformZ);
        GameObject edges2 = platformCreator.CreateEdges(4, -platform.height, platformX, platformZ);

        //圍牆
        GameObject frence = platformCreator.CreateFrence(new Vector3(platformX, 12, platformZ), true);
        frence.transform.Translate(new Vector3(0, platform.height, 0));
        //階層關係
        edges1.transform.parent = platformBody.transform;
        edges2.transform.parent = platformBody.transform;
        frence.transform.parent = platformBody.transform;
        platformBody.transform.Translate(new Vector3(0, platform.height, 0));
        platformBody.transform.parent = platformObject.transform;
        platformObject.transform.parent = building.transform;
    }

    /// <summary>
    /// 創建正脊
    /// </summary>
    /// <param name="roof">傳入屋頂資訊</param>
    public void CreateRightRidge()
    {
        float lengthAnchor = roof.length - roof.topLowerLength * 2;
        float deepAnchor = roof.deep - roof.topLowerLength * 2;
        CircleCurve circleCurve = new CircleCurve();
        //生成左右邊的弧
        List<Vector3> leftCurve = circleCurve.CreateCircleCurve(roof.topLowerHeight, roof.topLowerLength, roof.topLowerCurve, circleCurveRes / 2);
        List<Vector3> rightCurve = circleCurve.CreateCircleCurve(roof.topLowerHeight, roof.topLowerLength, roof.topLowerCurve, circleCurveRes / 2);
        List<Vector3> totalPoints = new List<Vector3>();

        if (roof.topLowerLength > 0.01f && roof.topLowerHeight > 0.01f)
        {
            //加入左邊的點 要倒過來放
            for (int i = leftCurve.Count - 1; i >= 0; i--)
                totalPoints.Add(new Vector3(-(leftCurve[i].x + lengthAnchor / 2), leftCurve[i].y, leftCurve[i].z));
            //加入右邊的點
            foreach (Vector3 point in rightCurve)
                totalPoints.Add(new Vector3(point.x + lengthAnchor / 2, point.y, point.z));
        }
        else
        {
            totalPoints.Add(new Vector3(-roof.length / 2, 0, 0));
            totalPoints.Add(new Vector3(roof.length / 2, 0, 0));
        }
        rightRidgeFrontPoints = new List<Vector3>(totalPoints);
        totalPoints.Reverse();
        RafterCreator creator = chinesePavilionCreater.GetComponent<RafterCreator>();
        //生成正脊mesh
        GameObject ridgeWidth = creator.Create(totalPoints, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        ridgeWidth.name = "RightRidge";
        //存到bargeboardPoints 畫mesh用

        //deep RightRidge
        totalPoints = new List<Vector3>();
        int type = 1;

        if (type == 2 && roof.topLowerLength > 0.01f && roof.topLowerHeight > 0.01f)
        {
            //加入左邊的點 要倒過來放
            for (int i = leftCurve.Count - 1; i >= 0; i--)
                totalPoints.Add(new Vector3(-(leftCurve[i].x + deepAnchor / 2), leftCurve[i].y, leftCurve[i].z));
            //加入右邊的點
            foreach (Vector3 point in rightCurve)
                totalPoints.Add(new Vector3(point.x + deepAnchor / 2, point.y, point.z));
        }
        else if (type == 1)
        {
            totalPoints.Add(new Vector3(-roof.deep / 2, roof.topLowerHeight, 0));
            totalPoints.Add(new Vector3(roof.deep / 2, roof.topLowerHeight, 0));
        }
        totalPoints.Reverse();
        List<GameObject> ridges = new List<GameObject>();
        ridges.Add(ridgeWidth);
        if (roof.deep > 0.01f)
        {
            GameObject ridgeDeep = creator.Create(totalPoints, rafterNbSides, rafterRes, rafterHeight, rafterTall, rafterRadius);
            ridgeDeep.name = "RightRidgeDeep";

            ridges.Add(Instantiate(ridgeWidth));
            ridges.Add(ridgeDeep);
            ridges.Add(Instantiate(ridgeDeep));


            ridges[0].transform.position = new Vector3(0, 0, roof.deep / 2);
            ridges[0].transform.Rotate(new Vector3(0, 0, 0));
            ridges[1].transform.position = new Vector3(0, 0, -(roof.deep / 2));
            ridges[1].transform.Rotate(new Vector3(0, 0, 0));
            ridges[2].transform.position = new Vector3(roof.length / 2, 0, 0);
            ridges[2].transform.Rotate(new Vector3(0, 90, 0));
            ridges[3].transform.position = new Vector3(-roof.length / 2, 0, 0);
            ridges[3].transform.Rotate(new Vector3(0, 90, 0));
        }
        //存到bargeboardPoints 畫mesh用
        rightRidgeSidePoints = totalPoints;

        GameObject ridgesObj = new GameObject("RightRidges");
        foreach (GameObject ridge in ridges)
            ridge.transform.parent = ridgesObj.transform;

        ridgesObj.transform.parent = roofObject.transform;
    }

    /// <summary>
    /// 創建垂脊
    /// </summary>
    /// <param name="roof">傳入屋頂資訊</param>
    public void CreateBargeboard()
    {
        CircleCurve circleCurve = new CircleCurve();
        //生成弧
        List<Vector3> curve = circleCurve.CreateCircleCurve(roof.height, roof.width, roof.curve, circleCurveRes * 2);
        //倒過來
        curve.Reverse();

        //bargeboardPoints 畫mesh用
        bargeboardPoints = new List<Vector3>();
        float angle = Mathf.PI / 2;
        foreach (Vector3 pos in curve)
        {
            float x = pos.x * Mathf.Cos(angle) + pos.z * Mathf.Sin(angle);
            float y = pos.y + -(roof.height - roof.topLowerHeight);
            float newZ = -pos.x * Mathf.Sin(angle) + pos.z * Mathf.Cos(angle) + roof.width;
            bargeboardPoints.Add(new Vector3(x, y, newZ));
        }

        if (roof.length < 0.02f)
            return;

        //生成ridge的mesh
        RafterCreator creator = chinesePavilionCreater.GetComponent<RafterCreator>();
        GameObject ridge = creator.Create(curve, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        //生成四個，並調整位置
        List<GameObject> ridges = new List<GameObject>
        {
            ridge,
            Instantiate(ridge),
            Instantiate(ridge),
            Instantiate(ridge)
        };
        float z;
        z = roof.deep < 0.01f ? (roof.width + roof.deep) : (roof.width + roof.deep / 2);

        ridges[0].transform.position = new Vector3(roof.length / 2, -(roof.height - roof.topLowerHeight), z);
        ridges[0].transform.Rotate(new Vector3(0, 90f, 0));
        ridges[1].transform.position = new Vector3(roof.length / 2, -(roof.height - roof.topLowerHeight), -z);
        ridges[1].transform.Rotate(new Vector3(0, 270f, 0));
        ridges[2].transform.position = new Vector3(-roof.length / 2, -(roof.height - roof.topLowerHeight), -z);
        ridges[2].transform.Rotate(new Vector3(0, 270, 0));
        ridges[3].transform.position = new Vector3(-roof.length / 2, -(roof.height - roof.topLowerHeight), z);
        ridges[3].transform.Rotate(new Vector3(0, 90, 0));

        GameObject ridgesObj = new GameObject("Ridges");
        foreach (GameObject obj in ridges)
        {
            obj.name = "Bargeboard";
            obj.transform.parent = ridgesObj.transform;
            // obj.SetActive(false);
        }
        ridgesObj.transform.parent = roofObject.transform;
    }

    /// <summary>
    /// 創建飛簷
    /// </summary>
    /// <param name="roof"></param>
    public void CreateFlyingRafter()
    {
        // 如果寬比起始點小 則無法繪製飛簷
        if (roof.width <= roof.sideEaveStart)
        {
            flyingRafterPoints = new List<Vector3>();
            return;
        }
        GameObject raftersObj = new GameObject("FlyingRafters");

        CircleCurve bargeBoardCurve = new CircleCurve();
        List<Vector3> curve = bargeBoardCurve.CreateCircleCurve(roof.height, roof.width, roof.curve, circleCurveRes * 2);
        float sideEaveLength = roof.width - roof.sideEaveStart;

        //拿取垂脊點的百分比 這樣才能精準吻合
        float bargeboardPercent = (sideEaveLength / roof.width * (curve.Count - 1f));
        // Debug.Log(bargeboardPercent);
        int bargeboardPointUpper = Mathf.CeilToInt(bargeboardPercent);
        int bargeboardPointLower = Mathf.FloorToInt(bargeboardPercent);
        float t = bargeboardPercent - bargeboardPointLower;
        //float flyingRafterHeight = Mathf.Lerp(curve[bargeboardPointLower].y, curve[bargeboardPointUpper].y, t);
        // Debug.Log(bargeboardPointUpper);
        float flyingRafterHeight = curve[bargeboardPointUpper].y;
        float sideEaveWidth = Mathf.Sqrt(Mathf.Pow(curve[bargeboardPointUpper].x, 2) + Mathf.Pow(curve[bargeboardPointUpper].x, 2));

        //生成飛簷的弧
        CircleCurve circleCurve = new CircleCurve();
        List<Vector3> flyingRafterCurve = circleCurve.CreateCircleCurve(flyingRafterHeight - roof.sideEaveHeight, sideEaveWidth, roof.sideEaveCurve, bargeboardPointUpper);

        //進行位移
        List<Vector3> offsetPoint = new List<Vector3>();
        foreach (Vector3 pos in flyingRafterCurve)
        {
            offsetPoint.Add(new Vector3(pos.x - sideEaveWidth, pos.y));
        }

        offsetPoint.Reverse();

        RafterCreator creator = chinesePavilionCreater.GetComponent<RafterCreator>();
        GameObject flyingRafter = creator.Create(offsetPoint, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        List<GameObject> flyingRafters = new List<GameObject>
        {
            flyingRafter,
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter)
        };
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        if (roof.deep < 0.01f)
            flyingRafterZ = curve[curve.Count - 1].x - curve[bargeboardPointUpper].x;
        else
            flyingRafterZ = curve[curve.Count - 1].x - curve[bargeboardPointUpper].x;
        // Debug.Log("height:"+height);
        flyingRafters[0].transform.position = new Vector3(roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[0].transform.Rotate(new Vector3(0, 135, 0));
        flyingRafters[1].transform.position = new Vector3(roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[1].transform.Rotate(new Vector3(0, 225f, 0));
        flyingRafters[2].transform.position = new Vector3(-roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[2].transform.Rotate(new Vector3(0, 315, 0));
        flyingRafters[3].transform.position = new Vector3(-roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[3].transform.Rotate(new Vector3(0, 45, 0));

        //List<GameObject> ridgesUpper = new List<GameObject>()
        //{
        //    Instantiate(flyingRafters[0]),
        //    Instantiate(flyingRafters[1]),
        //    Instantiate(flyingRafters[2]),
        //    Instantiate(flyingRafters[3])
        //};
        //foreach (GameObject obj in ridgesUpper)
        //{
        //    obj.transform.localScale = new Vector3(1.1f, 0.8f, 1);
        //    obj.transform.Translate(new Vector3(0, 0.15f, 0));
        //    obj.name = "FlyingRafterUpper";
        //    obj.transform.parent = raftersObj.transform;
        //}

        foreach (GameObject obj in flyingRafters)
        {
            obj.name = "FlyingRafter";
            obj.transform.parent = raftersObj.transform;
        }
        raftersObj.transform.parent = roofObject.transform;

        //flyingRafterPoints 畫mesh用
        flyingRafterPoints = offsetPoint;
        flyingBargePointNum = bargeboardPointUpper;

    }

    /// <summary>
    /// 創建方勝亭的屋脊
    /// </summary>
    public void CreateFangShengRidge()
    {
        //roof.length = 0;
        //roof.sideEaveStart = 0;
        //roof.topLowerHeight = 2.5f;
        //roof.topLowerLength = 0;
        // 如果寬比起始點小 則無法繪製飛簷
        if (roof.width <= roof.sideEaveStart)
        {
            flyingRafterPoints = new List<Vector3>();
            return;
        }
        GameObject raftersObj = new GameObject("FlyingRafters");

        CircleCurve bargeBoardCurve = new CircleCurve();
        List<Vector3> curve = bargeBoardCurve.CreateCircleCurve(roof.height, roof.width, roof.curve, circleCurveRes * 2);
        float sideEaveLength = roof.width - roof.sideEaveStart;

        //拿取垂脊點的百分比 這樣才能精準吻合
        float bargeboardPercent = (sideEaveLength / roof.width * (curve.Count - 1f));

        int bargeboardPointUpper = Mathf.CeilToInt(bargeboardPercent);
        int bargeboardPointLower = Mathf.FloorToInt(bargeboardPercent);
        float t = bargeboardPercent - bargeboardPointLower;

        float flyingRafterHeight = curve[bargeboardPointUpper].y;
        float sideEaveWidth = Mathf.Sqrt(Mathf.Pow(curve[bargeboardPointUpper].x, 2) + Mathf.Pow(curve[bargeboardPointUpper].x, 2));

        //生成飛簷的弧
        CircleCurve circleCurve = new CircleCurve();
        List<Vector3> flyingRafterCurve = circleCurve.CreateCircleCurve(flyingRafterHeight - roof.sideEaveHeight, sideEaveWidth, roof.sideEaveCurve, bargeboardPointUpper);
        List<Vector3> connectedFlyingRafterCurve = new List<Vector3>();
        foreach (Vector3 pos in flyingRafterCurve)
        {
            connectedFlyingRafterCurve.Add(new Vector3(pos.x, pos.y));
        }
        //留下最上面1/3的點
        for (int i = 0; i < connectedFlyingRafterCurve.Count; i++)
            if (connectedFlyingRafterCurve[i].y < Mathf.Floor((flyingRafterHeight - roof.sideEaveHeight) * 2 / 3))
                connectedFlyingRafterCurve.Remove(connectedFlyingRafterCurve[i--]);
                


        //進行位移
        List<Vector3> offsetPoint = new List<Vector3>();
        List<Vector3> connectedOffsetPoint = new List<Vector3>();
        foreach (Vector3 pos in flyingRafterCurve)
        {
            offsetPoint.Add(new Vector3(pos.x - sideEaveWidth, pos.y));
        }
        foreach (Vector3 pos in connectedFlyingRafterCurve)
        {
            connectedOffsetPoint.Add(new Vector3(pos.x - sideEaveWidth, pos.y));
        }

        offsetPoint.Reverse();
        connectedOffsetPoint.Reverse();

        RafterCreator creator = chinesePavilionCreater.GetComponent<RafterCreator>();
        GameObject flyingRafter = creator.Create(offsetPoint, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        GameObject connectedFlyingRafter = creator.Create(connectedOffsetPoint, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        List<GameObject> flyingRafters = new List<GameObject>
        {
            flyingRafter,
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            connectedFlyingRafter,
            Instantiate(flyingRafter),
            Instantiate(connectedFlyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter)
        };
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        if (roof.deep < 0.01f)
            flyingRafterZ = curve[curve.Count - 1].x - curve[bargeboardPointUpper].x;
        else
            flyingRafterZ = curve[curve.Count - 1].x - curve[bargeboardPointUpper].x;
        // Debug.Log("height:"+height);
        flyingRafters[0].transform.position = new Vector3(roof.width / 3 + roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[0].transform.Rotate(new Vector3(0, 90, 0));
        flyingRafters[1].transform.position = new Vector3(roof.width / 3 + roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[1].transform.Rotate(new Vector3(0, 180, 0));
        flyingRafters[2].transform.position = new Vector3(roof.width / 3 -roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[2].transform.Rotate(new Vector3(0, 270, 0));
        flyingRafters[3].transform.position = new Vector3(roof.width / 3 -roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[3].transform.Rotate(new Vector3(0, 0, 0));
        flyingRafters[4].transform.position = new Vector3(-roof.width / 3 + roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[4].transform.Rotate(new Vector3(0, 90, 0));
        flyingRafters[5].transform.position = new Vector3(-roof.width / 3 + roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[5].transform.Rotate(new Vector3(0, 180, 0));
        flyingRafters[6].transform.position = new Vector3(-roof.width / 3  -roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[6].transform.Rotate(new Vector3(0, 270, 0));
        flyingRafters[7].transform.position = new Vector3(-roof.width / 3  -roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[7].transform.Rotate(new Vector3(0, 0, 0));

        foreach (GameObject obj in flyingRafters)
        {
            obj.name = "FlyingRafter";
            obj.transform.parent = raftersObj.transform;
        }
        raftersObj.transform.parent = roofObject.transform;

        //flyingRafterPoints 畫mesh用
        flyingRafterPoints = offsetPoint;
        flyingBargePointNum = bargeboardPointUpper;

    }

    /// <summary>
    /// 繪製一般屋頂的mesh
    /// </summary>
    public void DrawRoofMesh()
    {
        DebugHandler debugHandler = FindObjectOfType<DebugHandler>();

        RoofCreator creator = FindObjectOfType<RoofCreator>();

        // 繪製上面屋頂的mesh
        List<Vector3> DeepTotalPoints = new List<Vector3>
        {
            new Vector3(0, 0, -roof.deep / 2),
            new Vector3(0, 0, roof.deep / 2)
        };
        //開始生成
        GameObject topRoof = creator.CreateTopRoof(DeepTotalPoints, rightRidgeFrontPoints, roof.length / 2, roof.deep / 2, circleCurveRes / 2 + 1);

        topRoof.transform.parent = roofObject.transform;
        topRoof.name = "TopRoofs";

        #region 正面的屋簷
        //找正面飛簷的點
        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(0, flyingRafterY, flyingRafterZ);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
        // 算所有正脊的點
        List<Vector3> topRoofPoints = new List<Vector3>();
        int res = rightRidgeFrontPoints.Count < rafterRes ? 1 : rafterRes;
        int cutNum = (int)(roof.length / 2 - roof.topLowerLength) * 3;
        for (int i = 0; i < res; i++)
            topRoofPoints.Add(rightRidgeFrontPoints[i]);
        for (int i = 1; i < cutNum; i++)
        {
            Vector3 newPoint = (rightRidgeFrontPoints[res - 1] * (cutNum - i) / cutNum) + (rightRidgeFrontPoints[res] * i / cutNum);
            topRoofPoints.Add(newPoint);
        }
        for (int i = res; i < rightRidgeFrontPoints.Count; i++)
            topRoofPoints.Add(rightRidgeFrontPoints[i]);
        //翼角計算
        // 開始製作正面的屋簷
        GameObject frontRoof = creator.CreateRoofEaves(newFlyingRafterPoints, bargeboardPoints, topRoofPoints, roof.length, roof, false);
        GameObject frontRoofInside = creator.CreateRoofEaves(newFlyingRafterPoints, bargeboardPoints, topRoofPoints, roof.length, roof, true);
        List<GameObject> frontRoofs = new List<GameObject>
        {
            frontRoof,
            Instantiate(frontRoof),
            frontRoofInside,
            Instantiate(frontRoofInside)
        };
        foreach (GameObject obj in frontRoofs)
        {
            obj.transform.parent = topRoof.transform;
            obj.name = "frontRoof";
        }
        frontRoofs[0].transform.position = new Vector3(0, 0, roof.deep / 2);
        frontRoofs[1].transform.position = new Vector3(0, 0, -roof.deep / 2);
        frontRoofs[2].transform.position = new Vector3(0, -0.25f, roof.deep / 2);
        frontRoofs[3].transform.position = new Vector3(0, -0.25f, -roof.deep / 2);
        frontRoofs[1].transform.Rotate(new Vector3(0, 180));
        frontRoofs[3].transform.Rotate(new Vector3(0, 180));
        frontRoofs[1].name = frontRoofs[3].name = "frontRoofInside";

        #endregion

        #region 側面的屋簷
        if (roof.width > roof.sideEaveStart)
        {
            List<Vector3> sideRoofPoints = new List<Vector3>();
            List<Vector3> fakeBargeboardPoints = new List<Vector3>();
            List<Vector3> sideFlyingRafterPoints = new List<Vector3>();

            offset = new Vector3(0, flyingRafterY, flyingRafterZ);
            for (int i = 0; i < flyingRafterPoints.Count; i++)
            {
                sideFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
            }

            if (flyingRafterPoints.Count > 0)
                cutNum = Mathf.CeilToInt((roof.deep / 2 + newFlyingRafterPoints[0].z) * 3);
            else
                cutNum = 0;

            for (int i = 0; i <= cutNum; i++)
            {
                Vector3 pointA = newFlyingRafterPoints[0];
                pointA.x = -(newFlyingRafterPoints[0].z + roof.deep / 2);
                Vector3 pointB = new Vector3(-pointA.x, newFlyingRafterPoints[0].y, newFlyingRafterPoints[0].z);

                Vector3 newPoint = (pointA * (cutNum - i) / cutNum) + (pointB * i / cutNum);
                sideRoofPoints.Add(newPoint);
            }

            for (int i = bargeboardPoints.Count - flyingBargePointNum - 1; i < bargeboardPoints.Count; i++)
            {
                fakeBargeboardPoints.Add(bargeboardPoints[i]);
            }

            GameObject sideRoof = creator.CreateRoofEaves(sideFlyingRafterPoints, fakeBargeboardPoints, sideRoofPoints, roof.deep + newFlyingRafterPoints[0].z * 2, roof, false);
            GameObject sideRoofInside = creator.CreateRoofEaves(sideFlyingRafterPoints, fakeBargeboardPoints, sideRoofPoints, roof.deep + newFlyingRafterPoints[0].z * 2, roof, true);

            List<GameObject> sideRoofs = new List<GameObject>
            {
                sideRoof,
                Instantiate(sideRoof),
                sideRoofInside,
                Instantiate(sideRoofInside)
            };
            foreach (GameObject obj in sideRoofs)
            {
                obj.name = "sideroof";
                obj.transform.parent = roofObject.transform;
            }
            sideRoofs[0].transform.position = new Vector3((roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[0].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[1].transform.position = new Vector3(-(roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[1].transform.Rotate(new Vector3(0, 270, 0));
            sideRoofs[2].transform.position = new Vector3((roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[2].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[3].transform.position = new Vector3(-(roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[3].transform.Rotate(new Vector3(0, 270, 0));
            sideRoofs[2].name = sideRoofs[3].name = "sideRoofInside";
        }
        #endregion
    }

    /// <summary>
    /// 繪製方勝亭屋頂的mesh
    /// </summary>
    public void DrawFangShengRoofMesh()
    {
        DebugHandler debugHandler = FindObjectOfType<DebugHandler>();

        RoofCreator creator = FindObjectOfType<RoofCreator>();

        // 繪製上面屋頂的mesh
        List<Vector3> DeepTotalPoints = new List<Vector3>
        {
            new Vector3(0, 0, -roof.deep / 2),
            new Vector3(0, 0, roof.deep / 2)
        };
        //開始生成
        GameObject topRoof = creator.CreateTopRoof(DeepTotalPoints, rightRidgeFrontPoints, roof.length / 2, roof.deep / 2, circleCurveRes / 2 + 1);

        topRoof.transform.parent = roofObject.transform;
        topRoof.name = "TopRoofs";

        #region 正面的屋簷
        //找正面飛簷的點
        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.width / 3, flyingRafterY + roof.height / 2.85f, flyingRafterZ);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
        // 算所有正脊的點
        List<Vector3> topRoofPoints = new List<Vector3>();
        int res = rightRidgeFrontPoints.Count < rafterRes ? 1 : rafterRes;
        int cutNum = (int)(roof.length / 2 - roof.topLowerLength) * 3;
        for (int i = 0; i < res; i++)
            topRoofPoints.Add(rightRidgeFrontPoints[i]);
        for (int i = 1; i < cutNum; i++)
        {
            Vector3 newPoint = (rightRidgeFrontPoints[res - 1] * (cutNum - i) / cutNum) + (rightRidgeFrontPoints[res] * i / cutNum);
            topRoofPoints.Add(newPoint);
        }
        for (int i = res; i < rightRidgeFrontPoints.Count; i++)
            topRoofPoints.Add(rightRidgeFrontPoints[i]);
        //翼角計算
        // 開始製作正面的屋簷
        //GameObject frontRoof = creator.CreateCombinedEaves(newFlyingRafterPoints, bargeboardPoints, topRoofPoints, roof.length, roof, false);
        //GameObject frontRoofInside = creator.CreateCombinedEaves(newFlyingRafterPoints, bargeboardPoints, topRoofPoints, roof.length, roof, true);
        GameObject frontRoof = new GameObject();
        GameObject frontRoofInside = new GameObject();
        List<GameObject> frontRoofs = new List<GameObject>
        {
            frontRoof,
            Instantiate(frontRoof),
            frontRoofInside,
            Instantiate(frontRoofInside)
        };
        foreach (GameObject obj in frontRoofs)
        {
            obj.transform.parent = topRoof.transform;
            obj.name = "frontRoof";
        }
        frontRoofs[0].transform.position = new Vector3(0, 0, roof.deep / 2);
        frontRoofs[1].transform.position = new Vector3(0, 0, -roof.deep / 2);
        frontRoofs[2].transform.position = new Vector3(0, -0.25f, roof.deep / 2);
        frontRoofs[3].transform.position = new Vector3(0, -0.25f, -roof.deep / 2);
        frontRoofs[1].transform.Rotate(new Vector3(0, 180));
        frontRoofs[3].transform.Rotate(new Vector3(0, 180));
        frontRoofs[1].name = frontRoofs[3].name = "frontRoofInside";

        #endregion

        #region 側面的屋簷
        if (roof.width > roof.sideEaveStart)
        {
            List<Vector3> sideRoofPoints = new List<Vector3>();
            List<Vector3> fakeBargeboardPoints = new List<Vector3>();
            List<Vector3> sideFlyingRafterPoints = new List<Vector3>();

            offset = new Vector3(0, flyingRafterY, flyingRafterZ);
            for (int i = 0; i < flyingRafterPoints.Count; i++)
            {
                sideFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
            }

            if (flyingRafterPoints.Count > 0)
                cutNum = Mathf.CeilToInt((roof.deep / 2 + newFlyingRafterPoints[0].z) * 3);
            else
                cutNum = 0;

            for (int i = 0; i <= cutNum; i++)
            {
                Vector3 pointA = newFlyingRafterPoints[0];
                pointA.x = -(newFlyingRafterPoints[0].z + roof.deep / 2);
                Vector3 pointB = new Vector3(-pointA.x, newFlyingRafterPoints[0].y, newFlyingRafterPoints[0].z);

                Vector3 newPoint = (pointA * (cutNum - i) / cutNum) + (pointB * i / cutNum);
                sideRoofPoints.Add(newPoint);
            }

            for (int i = bargeboardPoints.Count - flyingBargePointNum - 1; i < bargeboardPoints.Count; i++)
            {
                fakeBargeboardPoints.Add(bargeboardPoints[i]);
            }

            GameObject sideRoof = creator.CreateRoofEaves(sideFlyingRafterPoints, fakeBargeboardPoints, sideRoofPoints, roof.deep + newFlyingRafterPoints[0].z * 2, roof, false);
            GameObject sideRoofInside = creator.CreateRoofEaves(sideFlyingRafterPoints, fakeBargeboardPoints, sideRoofPoints, roof.deep + newFlyingRafterPoints[0].z * 2, roof, true);

            List<GameObject> sideRoofs = new List<GameObject>
            {
                sideRoof,
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                sideRoofInside,
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside)
            };
            foreach (GameObject obj in sideRoofs)
            {
                obj.name = "sideroof";
                obj.transform.parent = roofObject.transform;
            }
            sideRoofs[0].transform.position = new Vector3(roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[0].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[1].transform.position = new Vector3(roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[1].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[2].transform.position = new Vector3(roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[2].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[3].transform.position = new Vector3(roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[3].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[4].transform.position = new Vector3(-roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[4].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[5].transform.position = new Vector3(-roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[5].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[6].transform.position = new Vector3(-roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[6].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[7].transform.position = new Vector3(-roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[7].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[8].transform.position = new Vector3(roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[8].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[9].transform.position = new Vector3(roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[9].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[10].transform.position = new Vector3(roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[10].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[11].transform.position = new Vector3(roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[11].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[12].transform.position = new Vector3(-roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[12].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[13].transform.position = new Vector3(-roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[13].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[14].transform.position = new Vector3(-roof.width / 3 + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[14].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[15].transform.position = new Vector3(-roof.width / 3 - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[15].transform.Rotate(new Vector3(0, 135, 0));

            Shader shader = Shader.Find("Tessellation/Clip Tessellation");
            Renderer rend = sideRoofs[1].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            rend = sideRoofs[2].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            rend = sideRoofs[9].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            rend = sideRoofs[10].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            shader = Shader.Find("Tessellation/Clip Tessellation Opposite");
            rend = sideRoofs[4].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            rend = sideRoofs[7].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            rend = sideRoofs[12].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            rend = sideRoofs[15].GetComponent<MeshRenderer>();
            rend.material.shader = shader;
            //sideRoofs[2].name = sideRoofs[3].name = "sideRoofInside";
        }
        #endregion
    }

    /// <summary>
    /// 放置裝飾
    /// </summary>
    public void PutRoofDecoration()
    {
        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(0, flyingRafterY, flyingRafterZ);
        Vector3 flyingRafterPoint;
        if (flyingRafterPoints.Count > 0)
            flyingRafterPoint = rotateEuler * flyingRafterPoints[flyingRafterPoints.Count - 1] + offset;
        else
            flyingRafterPoint = new Vector3(0, 0, 0);
        // Debug.Log(flyingRafterZ);
        RoofCreator creator = FindObjectOfType<RoofCreator>();
        // 製作銅瓦與滴水
        Transform frontEavesTile1 = creator.CreateRoofEavesTile(roof.length, roof, flyingRafterPoint.x, flyingRafterPoints).transform;
        Transform frontEavesTile2 = creator.CreateRoofEavesTile(roof.length, roof, flyingRafterPoint.x, flyingRafterPoints).transform;
        Transform sideEavesTile1 = creator.CreateRoofEavesTile(roof.deep + flyingRafterZ * 2, roof, flyingRafterPoint.x, flyingRafterPoints).transform;
        Transform sideEavesTile2 = creator.CreateRoofEavesTile(roof.deep + flyingRafterZ * 2, roof, flyingRafterPoint.x, flyingRafterPoints).transform;

        //roof.deep + newFlyingRafterPoints[0].z * 2
        frontEavesTile1.position = new Vector3(0, -roof.height + roof.topLowerHeight - 0.25f, roof.deep / 2 + roof.width);
        frontEavesTile2.position = new Vector3(0, -roof.height + roof.topLowerHeight - 0.25f, -(roof.deep / 2 + roof.width));
        sideEavesTile1.position = new Vector3(roof.length / 2 + (flyingRafterPoint.x), -roof.height + roof.topLowerHeight - 0.22f, 0);
        sideEavesTile2.position = new Vector3(-(roof.length / 2 + (flyingRafterPoint.x)), -roof.height + roof.topLowerHeight - 0.22f, 0);

        frontEavesTile2.Rotate(new Vector3(0, 180, 0));
        sideEavesTile1.Rotate(new Vector3(0, 90, 0));
        sideEavesTile2.Rotate(new Vector3(0, -90, 0));
        frontEavesTile1.parent = frontEavesTile2.parent = sideEavesTile1.parent = sideEavesTile2.parent = roofObject.transform;

        if (roof.length < 0.01 && roof.deep < 0.01)
        {
            Transform baoDing = creator.CreateBaoDing().transform;
            baoDing.Translate(new Vector3(0, 0, 0));
            baoDing.parent = roofObject.transform;
        }

    }
}
