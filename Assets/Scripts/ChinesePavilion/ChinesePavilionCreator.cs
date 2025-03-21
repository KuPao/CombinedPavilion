﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BLINDED_AM_ME;

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
    private Body body;
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
    private List<Vector3> baoShaBargeboardPoints;
    //飛簷的點
    private List<Vector3> flyingRafterPoints;
    private List<Vector3> baoShaFlyingRafterPoints;
    //垂脊與飛簷接合的點
    private int flyingBargePointNum;
    private int baoShaFlyingBargePointNum;
    // 飛簷座標 這邊沒寫好
    float flyingRafterZ;

    //當前高度
    private float CurrentHeight;
    //柱子的位置
    private Vector3[] columnsPos;
    //切面用
    public Material cutMaterial;

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
    public GameObject CreateRoof(Roof roof, Body body)
    {
        this.roof = roof;
        this.body = body;
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
                //前兩個方法是為了初始化一些參數避免後面出錯，對雙單體組合亭沒有實際用處
                //創建正脊
                CreateRightRidge();
                //創建垂脊
                CreateBargeboard();
                //創建方勝亭的脊
                CreateFangShengRidge();
                //生成屋頂Mesh
                DrawFangShengRoofMesh();
                break;
            case 2:
                //創建正脊
                CreateRightRidge();
                //創建垂脊
                CreateBargeboard();
                //創建雙六角亭的脊
                CreateDoubleHexaRidge();
                //生成屋頂Mesh
                DrawDoubleHexaRoofMesh();
                break;
            case 3:
                //創建正脊
                CreateRightRidge();
                //創建垂脊
                CreateBargeboard();
                //創建雙八角亭的脊
                CreateDoubleOctoRidge();
                //生成屋頂Mesh
                DrawDoubleOctoRoofMesh();
                break;
            case 4:
                //創建正脊
                CreateCrossRightRidge();
                //創建垂脊
                CreateBargeboard();
                //創建十字亭的垂脊(包含抱廈)
                CreateCrossRidge();
                //生成屋頂Mesh
                DrawCrossRoofMesh();
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
        switch(roof.combineType)
        {
            case 0 :
                CreateNormalColumn(body);
                break;
            case 1 :
                CreateFangShengColumn(body);
                break;
            case 2 :
                CreateDoubleHexaColumn(body);
                break;
            case 3 :
                CreateDoubleOctoColumn(body);
                break;
            default:
                break;
        }
    }

    public void CreateNormalColumn(Body body)
    {
        roofObject.transform.Translate(0, roof.height - roof.topLowerHeight + body.height, 0);

        bodyObject = new GameObject();
        bodyObject.name = "Body";
        bodyObject.transform.parent = building.transform;


        EaveColumnCreator creator = chinesePavilionCreater.GetComponent<EaveColumnCreator>();

        #region 簷柱
        List<GameObject> cylinders = new List<GameObject>();
        // 先創立柱子

        float lastPointX;

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

        for (int i = 0; i < 4; i++)
        {
            GameObject cylinder = creator.CreateEaveColumn(body.height, body.radius);
            cylinders.Add(cylinder);
        }
        cylinders[0].transform.position = new Vector3(cylinderX, 0, cylinderZ);
        cylinders[1].transform.position = new Vector3(-cylinderX, 0, cylinderZ);
        cylinders[2].transform.position = new Vector3(-cylinderX, 0, -cylinderZ);
        cylinders[3].transform.position = new Vector3(cylinderX, 0, -cylinderZ);

        foreach (GameObject cylinder in cylinders)
        {
            cylinder.transform.parent = bodyObject.transform;
        }

        #endregion

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

    public void CreateFangShengColumn(Body body)
    {
        roofObject.transform.Translate(0, roof.height - roof.topLowerHeight + body.height, 0);

        bodyObject = new GameObject();
        bodyObject.name = "Body";
        bodyObject.transform.parent = building.transform;


        EaveColumnCreator creator = chinesePavilionCreater.GetComponent<EaveColumnCreator>();

        #region 簷柱
        List<GameObject> cylinders = new List<GameObject>();
        // 先創立柱子

        float lastPointX;

        if (roof.width <= roof.sideEaveStart)
            lastPointX = 0;
        else
            lastPointX = -flyingRafterPoints[flyingRafterPoints.Count - 1].x / 2;

        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 180, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.length / 2, flyingRafterY, flyingRafterZ + roof.deep / 2);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
        int pointNum = (int)((newFlyingRafterPoints.Count - 1) * (body.pos / 10));

        float cylinderX = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].x : offset.x;
        float cylinderZ = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].z : offset.z;

        for (int i = 0; i < 8; i++)
        {
            GameObject cylinder = creator.CreateEaveColumn(body.height, body.radius);
            cylinders.Add(cylinder);
        }
        cylinders[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX, 0, 0);
        cylinders[1].transform.position = new Vector3(roof.width / roof.disBetween, 0, cylinderX);
        cylinders[2].transform.position = new Vector3(0, 0, roof.width - roof.width / roof.disBetween);
        cylinders[3].transform.position = new Vector3(-roof.width / roof.disBetween, 0, cylinderX);
        cylinders[4].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX, 0, 0);
        cylinders[5].transform.position = new Vector3(-roof.width / roof.disBetween, 0, -cylinderX);
        cylinders[6].transform.position = new Vector3(0, 0, -(roof.width - roof.width / roof.disBetween));
        cylinders[7].transform.position = new Vector3(roof.width / roof.disBetween, 0, -cylinderX);

        foreach (GameObject cylinder in cylinders)
        {
            cylinder.transform.parent = bodyObject.transform;
        }

        #endregion

        #region  額枋 楣子 雀替
        #region  生成所有額枋 並調整位置
        GameObject architraveObj = new GameObject();
        architraveObj.name = "Architraves";
        float architravesHeight = 0.75f;

        GameObject architrave1 = creator.CreateArchitrave(cylinderX * Mathf.Sqrt(2), architravesHeight);
        //算交叉柱與連接的柱子的長度
        float intersectLength = Mathf.Sqrt(Mathf.Pow(roof.width / roof.disBetween, 2) + Mathf.Pow((-cylinderX + (roof.width - roof.width / roof.disBetween)), 2));
        //算交叉柱與連接的柱子的角度
        double angle = Mathf.Atan((roof.width / roof.disBetween) / (-cylinderX + (roof.width - roof.width / roof.disBetween))) * 180.0 / Mathf.PI;
        GameObject architrave2 = creator.CreateArchitrave(intersectLength, architravesHeight);
        List<GameObject> architraves = new List<GameObject>()
        {
            architrave1,
            Instantiate(architrave1),
            Instantiate(architrave1),
            Instantiate(architrave1),
            architrave2,
            Instantiate(architrave2),
            Instantiate(architrave2),
            Instantiate(architrave2)
        };
        architraves[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX / 2, body.height - architravesHeight / 2, cylinderX / 2);
        architraves[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX / 2, body.height - architravesHeight / 2, -cylinderX / 2);
        architraves[2].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX / 2, body.height - architravesHeight / 2, -cylinderX / 2);
        architraves[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX / 2, body.height - architravesHeight / 2, cylinderX / 2);
        //關於Z座標: I don't know why, but it works.
        architraves[4].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        architraves[5].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        architraves[6].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        architraves[7].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);

        architraves[0].transform.Rotate(new Vector3(0, 45, 0));
        architraves[1].transform.Rotate(new Vector3(0, 135, 0));
        architraves[2].transform.Rotate(new Vector3(0, -135, 0));
        architraves[3].transform.Rotate(new Vector3(0, -45, 0));
        architraves[4].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        architraves[5].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        architraves[6].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        architraves[7].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
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
        GameObject meiZi1 = creator.CreateMeiZi(cylinderX * Mathf.Sqrt(2), meiZiHeight);
        GameObject meiZi2 = creator.CreateMeiZi(intersectLength, meiZiHeight);
        List<GameObject> meiZis = new List<GameObject>()
        {
            meiZi1,
            Instantiate(meiZi1),
            Instantiate(meiZi1),
            Instantiate(meiZi1),
            meiZi2,
            Instantiate(meiZi2),
            Instantiate(meiZi2),
            Instantiate(meiZi2)
        };
        meiZis[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX / 2, body.height - meiZiHeight / 2 - architravesHeight, cylinderX / 2);
        meiZis[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX / 2, body.height - meiZiHeight / 2 - architravesHeight, -cylinderX / 2);
        meiZis[2].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX / 2, body.height - meiZiHeight / 2 - architravesHeight, -cylinderX / 2);
        meiZis[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX / 2, body.height - meiZiHeight / 2 - architravesHeight, cylinderX / 2);
        meiZis[4].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        meiZis[5].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        meiZis[6].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        meiZis[7].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);

        meiZis[0].transform.Rotate(new Vector3(0, 45, 0));
        meiZis[1].transform.Rotate(new Vector3(0, 135, 0));
        meiZis[2].transform.Rotate(new Vector3(0, -135, 0));
        meiZis[3].transform.Rotate(new Vector3(0, -45, 0));
        meiZis[4].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        meiZis[5].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        meiZis[6].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        meiZis[7].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
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
        for (int i = 0; i < 11; i++)
        {
            sparrowBraces.Add(Instantiate(sparrowBrace));
        }

        //從最右側的柱子開始逆時針，交叉的柱子不做
        sparrowBraces[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[2].transform.position = new Vector3(roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[3].transform.position = new Vector3(roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[4].transform.position = new Vector3(-roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[5].transform.position = new Vector3(-roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[6].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[7].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[8].transform.position = new Vector3(-roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[9].transform.position = new Vector3(-roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[10].transform.position = new Vector3(roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[11].transform.position = new Vector3(roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));

        sparrowBraces[0].transform.Rotate(new Vector3(0, 135, 0));
        sparrowBraces[1].transform.Rotate(new Vector3(0, -135, 0));
        sparrowBraces[2].transform.Rotate(new Vector3(0, 45, 0));
        sparrowBraces[3].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        sparrowBraces[4].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
        sparrowBraces[5].transform.Rotate(new Vector3(0, 135, 0));
        sparrowBraces[6].transform.Rotate(new Vector3(0, -45, 0));
        sparrowBraces[7].transform.Rotate(new Vector3(0, 45, 0));
        sparrowBraces[8].transform.Rotate(new Vector3(0, -135, 0));
        sparrowBraces[9].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        sparrowBraces[10].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        sparrowBraces[11].transform.Rotate(new Vector3(0, -45, 0));

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
        GameObject frieze1 = creator.CreateFriezeIn(cylinderX * Mathf.Sqrt(2), friezeHeight);
        GameObject frieze2 = creator.CreateFriezeIn(intersectLength, friezeHeight);
        List<GameObject> friezes = new List<GameObject>()
        {
            frieze1,
            Instantiate(frieze1),
            Instantiate(frieze1),
            Instantiate(frieze1),
            frieze2,
            Instantiate(frieze2),
            Instantiate(frieze2),
            Instantiate(frieze2)
        };
        friezes[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX / 2, 0, cylinderX / 2);
        friezes[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX / 2, 0, -cylinderX / 2);
        friezes[2].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX / 2, 0, -cylinderX / 2);
        friezes[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX / 2, 0, cylinderX / 2);
        friezes[4].transform.position = new Vector3(roof.width / roof.disBetween / 2, 0, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        friezes[5].transform.position = new Vector3(roof.width / roof.disBetween / 2, 0, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        friezes[6].transform.position = new Vector3(-roof.width / roof.disBetween / 2, 0, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        friezes[7].transform.position = new Vector3(-roof.width / roof.disBetween / 2, 0, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);

        friezes[0].transform.Rotate(new Vector3(0, 45, 0));
        friezes[1].transform.Rotate(new Vector3(0, 135, 0));
        friezes[2].transform.Rotate(new Vector3(0, -135, 0));
        friezes[3].transform.Rotate(new Vector3(0, -45, 0));
        friezes[4].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        friezes[5].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        friezes[6].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        friezes[7].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
        foreach (GameObject obj in friezes)
        {
            obj.transform.parent = friezeInObj.transform;
        }
        friezes[1].SetActive(false);
        friezes[3].SetActive(false);
        friezeInObj.transform.parent = bodyObject.transform;
        #endregion

        #endregion
    }

    public void CreateDoubleHexaColumn(Body body)
    {
        roofObject.transform.Translate(0, roof.height - roof.topLowerHeight + body.height, 0);

        bodyObject = new GameObject();
        bodyObject.name = "Body";
        bodyObject.transform.parent = building.transform;


        EaveColumnCreator creator = chinesePavilionCreater.GetComponent<EaveColumnCreator>();

        #region 簷柱
        List<GameObject> cylinders = new List<GameObject>();
        // 先創立柱子

        float lastPointX;

        if (roof.width <= roof.sideEaveStart)
            lastPointX = 0;
        else
            lastPointX = -flyingRafterPoints[flyingRafterPoints.Count - 1].x / 2;

        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 180, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.length / 2, flyingRafterY, flyingRafterZ + roof.deep / 2);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
        int pointNum = (int)((newFlyingRafterPoints.Count - 1) * (body.pos / 10));

        float cylinderX = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].x : offset.x;
        float cylinderZ = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].z : offset.z;

        for (int i = 0; i < 10; i++)
        {
            GameObject cylinder = creator.CreateEaveColumn(body.height, body.radius);
            cylinders.Add(cylinder);
        }
        cylinders[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, 0, cylinderX / 2);
        cylinders[1].transform.position = new Vector3(roof.width / roof.disBetween, 0, cylinderX);
        cylinders[2].transform.position = new Vector3(0, 0, roof.width - roof.width / roof.disBetween);
        cylinders[3].transform.position = new Vector3(-roof.width / roof.disBetween, 0, cylinderX);
        cylinders[4].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, 0, cylinderX / 2);
        cylinders[5].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, 0, -cylinderX / 2);
        cylinders[6].transform.position = new Vector3(-roof.width / roof.disBetween, 0, -cylinderX);
        cylinders[7].transform.position = new Vector3(0, 0, -(roof.width - roof.width / roof.disBetween));
        cylinders[8].transform.position = new Vector3(roof.width / roof.disBetween, 0, -cylinderX);
        cylinders[9].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, 0, -cylinderX / 2);

        foreach (GameObject cylinder in cylinders)
        {
            cylinder.transform.parent = bodyObject.transform;
        }

        #endregion

        #region  額枋 楣子 雀替
        #region  生成所有額枋 並調整位置
        GameObject architraveObj = new GameObject();
        architraveObj.name = "Architraves";
        float architravesHeight = 0.75f;

        GameObject architrave1 = creator.CreateArchitrave(cylinderX, architravesHeight);
        //算交叉柱與連接的柱子的長度
        float intersectLength = Mathf.Sqrt(Mathf.Pow(roof.width / roof.disBetween, 2) + Mathf.Pow((-cylinderX + (roof.width - roof.width / roof.disBetween)), 2));
        //算交叉柱與連接的柱子的角度
        double angle = Mathf.Atan((roof.width / roof.disBetween) / (-cylinderX + (roof.width - roof.width / roof.disBetween))) * 180.0 / Mathf.PI;
        GameObject architrave2 = creator.CreateArchitrave(intersectLength, architravesHeight);
        List<GameObject> architraves = new List<GameObject>()
        {
            architrave1,
            Instantiate(architrave1),
            Instantiate(architrave1),
            Instantiate(architrave1),
            Instantiate(architrave1),
            Instantiate(architrave1),
            architrave2,
            Instantiate(architrave2),
            Instantiate(architrave2),
            Instantiate(architrave2)
        };
        architraves[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 4, body.height - architravesHeight / 2, cylinderX * 3 / 4);
        architraves[1].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 4, body.height - architravesHeight / 2, cylinderX * 3 / 4);
        architraves[2].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, body.height - architravesHeight / 2, 0);
        architraves[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 4, body.height - architravesHeight / 2, -cylinderX * 3 / 4);
        architraves[4].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 4, body.height - architravesHeight / 2, -cylinderX * 3 / 4);
        architraves[5].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, body.height - architravesHeight / 2, 0);
        //關於Z座標: I don't know why, but it works.
        architraves[6].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        architraves[7].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        architraves[8].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        architraves[9].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - architravesHeight / 2, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);

        architraves[0].transform.Rotate(new Vector3(0, 30, 0));
        architraves[1].transform.Rotate(new Vector3(0, -30, 0));
        architraves[2].transform.Rotate(new Vector3(0, -90, 0));
        architraves[3].transform.Rotate(new Vector3(0, -150, 0));
        architraves[4].transform.Rotate(new Vector3(0, 150, 0));
        architraves[5].transform.Rotate(new Vector3(0, 90, 0));
        architraves[6].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        architraves[7].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        architraves[8].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        architraves[9].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
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
        GameObject meiZi1 = creator.CreateMeiZi(cylinderX, meiZiHeight);
        GameObject meiZi2 = creator.CreateMeiZi(intersectLength, meiZiHeight);
        List<GameObject> meiZis = new List<GameObject>()
        {
            meiZi1,
            Instantiate(meiZi1),
            Instantiate(meiZi1),
            Instantiate(meiZi1),
            Instantiate(meiZi1),
            Instantiate(meiZi1),
            meiZi2,
            Instantiate(meiZi2),
            Instantiate(meiZi2),
            Instantiate(meiZi2)
        };
        meiZis[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 4, body.height - meiZiHeight / 2 - architravesHeight, cylinderX * 3 / 4);
        meiZis[1].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 4, body.height - meiZiHeight / 2 - architravesHeight, cylinderX * 3 / 4);
        meiZis[2].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, body.height - meiZiHeight / 2 - architravesHeight, 0);
        meiZis[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 4, body.height - meiZiHeight / 2 - architravesHeight, -cylinderX * 3 / 4);
        meiZis[4].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 4, body.height - meiZiHeight / 2 - architravesHeight, -cylinderX * 3 / 4);
        meiZis[5].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, body.height - meiZiHeight / 2 - architravesHeight, 0);
        meiZis[6].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        meiZis[7].transform.position = new Vector3(roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        meiZis[8].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        meiZis[9].transform.position = new Vector3(-roof.width / roof.disBetween / 2, body.height - meiZiHeight / 2 - architravesHeight, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);

        meiZis[0].transform.Rotate(new Vector3(0, 30, 0));
        meiZis[1].transform.Rotate(new Vector3(0, -30, 0));
        meiZis[2].transform.Rotate(new Vector3(0, -90, 0));
        meiZis[3].transform.Rotate(new Vector3(0, -150, 0));
        meiZis[4].transform.Rotate(new Vector3(0, 150, 0));
        meiZis[5].transform.Rotate(new Vector3(0, 90, 0));
        meiZis[6].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        meiZis[7].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        meiZis[8].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        meiZis[9].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
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
        for (int i = 0; i < 15; i++)
        {
            sparrowBraces.Add(Instantiate(sparrowBrace));
        }

        //從最右側的柱子開始逆時針，交叉的柱子不做
        sparrowBraces[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, cylinderX / 2 - body.radius / 2);
        sparrowBraces[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2 - body.radius / 2 * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, cylinderX / 2 + body.radius / 2 / 2);
        sparrowBraces[2].transform.position = new Vector3(roof.width / roof.disBetween + body.radius / 2 * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / 2);
        sparrowBraces[3].transform.position = new Vector3(roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[4].transform.position = new Vector3(-roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[5].transform.position = new Vector3(-roof.width / roof.disBetween - body.radius / 2 * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, cylinderX - body.radius / 2 / 2);
        sparrowBraces[6].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2 + body.radius / 2 * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, cylinderX / 2 + body.radius / 2 / 2);
        sparrowBraces[7].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, cylinderX / 2 - body.radius / 2);
        sparrowBraces[8].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, -cylinderX / 2 + body.radius / 2);
        sparrowBraces[9].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2 + body.radius / 2 * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, -cylinderX / 2 - body.radius / 2 / 2);
        sparrowBraces[10].transform.position = new Vector3(-roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[11].transform.position = new Vector3(-roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[12].transform.position = new Vector3(roof.width / roof.disBetween - body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[13].transform.position = new Vector3(roof.width / roof.disBetween + body.radius / 2 / Mathf.Sqrt(2), body.height - architravesHeight - meiZiHeight, -cylinderX + body.radius / 2 / Mathf.Sqrt(2));
        sparrowBraces[14].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2 - body.radius / 2 * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, -cylinderX / 2 - body.radius / 2 / 2);
        sparrowBraces[15].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, body.height - architravesHeight - meiZiHeight, -cylinderX / 2 + body.radius / 2);

        sparrowBraces[0].transform.Rotate(new Vector3(0, 90, 0));
        sparrowBraces[1].transform.Rotate(new Vector3(0, -150, 0));
        sparrowBraces[2].transform.Rotate(new Vector3(0, 30, 0));
        sparrowBraces[3].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        sparrowBraces[4].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
        sparrowBraces[5].transform.Rotate(new Vector3(0, 150, 0));
        sparrowBraces[6].transform.Rotate(new Vector3(0, -30, 0));
        sparrowBraces[7].transform.Rotate(new Vector3(0, 90, 0));
        sparrowBraces[8].transform.Rotate(new Vector3(0, -90, 0));
        sparrowBraces[9].transform.Rotate(new Vector3(0, 30, 0));
        sparrowBraces[10].transform.Rotate(new Vector3(0, -150, 0));
        sparrowBraces[11].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        sparrowBraces[12].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        sparrowBraces[13].transform.Rotate(new Vector3(0, -30, 0));
        sparrowBraces[14].transform.Rotate(new Vector3(0, 150, 0));
        sparrowBraces[15].transform.Rotate(new Vector3(0, -90, 0));

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
        GameObject frieze1 = creator.CreateFriezeIn(cylinderX, friezeHeight);
        GameObject frieze2 = creator.CreateFriezeIn(intersectLength, friezeHeight);
        List<GameObject> friezes = new List<GameObject>()
        {
            frieze1,
            Instantiate(frieze1),
            Instantiate(frieze1),
            Instantiate(frieze1),
            Instantiate(frieze1),
            Instantiate(frieze1),
            frieze2,
            Instantiate(frieze2),
            Instantiate(frieze2),
            Instantiate(frieze2)
        };
        friezes[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 4, 0, cylinderX * 3 / 4);
        friezes[1].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 4, 0, cylinderX * 3 / 4);
        friezes[2].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 2, 0, 0);
        friezes[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Sqrt(3) / 4, 0, -cylinderX * 3 / 4);
        friezes[4].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 4, 0, -cylinderX * 3 / 4);
        friezes[5].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Sqrt(3) / 2, 0, 0);
        friezes[6].transform.position = new Vector3(roof.width / roof.disBetween / 2, 0, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        friezes[7].transform.position = new Vector3(roof.width / roof.disBetween / 2, 0, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        friezes[8].transform.position = new Vector3(-roof.width / roof.disBetween / 2, 0, (-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);
        friezes[9].transform.position = new Vector3(-roof.width / roof.disBetween / 2, 0, -(-cylinderX - (roof.width - roof.width / roof.disBetween)) / 2);

        friezes[0].transform.Rotate(new Vector3(0, 30, 0));
        friezes[1].transform.Rotate(new Vector3(0, -30, 0));
        friezes[2].transform.Rotate(new Vector3(0, -90, 0));
        friezes[3].transform.Rotate(new Vector3(0, -150, 0));
        friezes[4].transform.Rotate(new Vector3(0, 150, 0));
        friezes[5].transform.Rotate(new Vector3(0, 90, 0));
        friezes[6].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
        friezes[7].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
        friezes[8].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
        friezes[9].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
        foreach (GameObject obj in friezes)
        {
            obj.transform.parent = friezeInObj.transform;
        }
        friezes[2].SetActive(false);
        friezes[5].SetActive(false);
        friezeInObj.transform.parent = bodyObject.transform;
        #endregion

        #endregion
    }

    public void CreateDoubleOctoColumn(Body body)
    {
        roofObject.transform.Translate(0, roof.height - roof.topLowerHeight + body.height, 0);

        bodyObject = new GameObject();
        bodyObject.name = "Body";
        bodyObject.transform.parent = building.transform;


        EaveColumnCreator creator = chinesePavilionCreater.GetComponent<EaveColumnCreator>();

        #region 簷柱
        List<GameObject> cylinders = new List<GameObject>();
        // 先創立柱子

        float lastPointX;

        if (roof.width <= roof.sideEaveStart)
            lastPointX = 0;
        else
            lastPointX = -flyingRafterPoints[flyingRafterPoints.Count - 1].x / 2;

        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 180, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.length / 2, flyingRafterY, flyingRafterZ + roof.deep / 2);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
        int pointNum = (int)((newFlyingRafterPoints.Count - 1) * (body.pos / 10));

        float cylinderX = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].x : offset.x;
        float cylinderZ = newFlyingRafterPoints.Count > 0 ? newFlyingRafterPoints[pointNum].z : offset.z;

        if(roof.disBetween <= 1.8)
        {
            for (int i = 0; i < 14; i++)
            {
                GameObject cylinder = creator.CreateEaveColumn(body.height, body.radius);
                cylinders.Add(cylinder);
            }
            cylinders[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), 0, cylinderX * Mathf.Sin(Mathf.PI / 8));
            cylinders[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            cylinders[2].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            cylinders[3].transform.position = new Vector3(0, 0, roof.width - roof.width / roof.disBetween);
            cylinders[4].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI * 5 / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            cylinders[5].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            cylinders[6].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 7 / 8));
            cylinders[7].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 7 / 8));
            cylinders[8].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            cylinders[9].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI * 5 / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            cylinders[10].transform.position = new Vector3(0, 0, -(roof.width - roof.width / roof.disBetween));
            cylinders[11].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            cylinders[12].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            cylinders[13].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI / 8));
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject cylinder = creator.CreateEaveColumn(body.height, body.radius);
                cylinders.Add(cylinder);
            }
            cylinders[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), 0, cylinderX * Mathf.Sin(Mathf.PI / 8));
            cylinders[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            cylinders[2].transform.position = new Vector3(0, 0, (roof.width - roof.width / roof.disBetween) * 1.2f);
            cylinders[3].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            cylinders[4].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI / 8), 0, cylinderX * Mathf.Sin(Mathf.PI * 7 / 8));
            cylinders[5].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 7 / 8));
            cylinders[6].transform.position = new Vector3(-roof.width / roof.disBetween - cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            cylinders[7].transform.position = new Vector3(0, 0, -(roof.width - roof.width / roof.disBetween) * 1.2f);
            cylinders[8].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            cylinders[9].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), 0, -cylinderX * Mathf.Sin(Mathf.PI / 8));
        }

        foreach (GameObject cylinder in cylinders)
        {
            cylinder.transform.parent = bodyObject.transform;
        }

        #endregion

        #region  額枋 楣子 雀替
        #region  生成所有額枋 並調整位置
        GameObject architraveObj = new GameObject();
        architraveObj.name = "Architraves";
        float architravesHeight = 0.75f;
        GameObject architrave1 = creator.CreateArchitrave(cylinderX * Mathf.Sin(Mathf.PI / 8) * 2, architravesHeight);
        float intersectLength;
        double angle;

        if(roof.disBetween <= 1.8)
        {
            //算交叉柱與連接的柱子的長度
            intersectLength =
            Mathf.Sqrt(
            Mathf.Pow(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8), 2) +
            Mathf.Pow((cylinderX * Mathf.Sin(Mathf.PI * 5 / 8)) - (roof.width - roof.width / roof.disBetween), 2));
            //算交叉柱與連接的柱子的角度
            angle =
            Mathf.Atan((roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8)) /
            (-(cylinderX * Mathf.Sin(Mathf.PI * 5 / 8)) + (roof.width - roof.width / roof.disBetween)))
            * 180.0 / Mathf.PI;
            GameObject architrave2 = creator.CreateArchitrave(intersectLength, architravesHeight);
            List<GameObject> architraves = new List<GameObject>()
            {
                architrave1,
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                architrave2,
                Instantiate(architrave2),
                Instantiate(architrave2),
                Instantiate(architrave2)
            };
            float xValue = cylinderX * Mathf.Cos(Mathf.PI / 8);
            float zValue;
            architraves[0].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            architraves[1].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            architraves[2].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            architraves[3].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            architraves[4].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 4 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 4 / 4));
            architraves[5].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            architraves[6].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            architraves[7].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            architraves[8].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            architraves[9].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 0 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 0 / 4));
            xValue = (roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8)) / 2;
            zValue = (roof.width - roof.width / roof.disBetween) + ((cylinderX * Mathf.Sin(Mathf.PI * 5 / 8)) - (roof.width - roof.width / roof.disBetween)) / 2;
            architraves[10].transform.position = new Vector3(xValue, body.height - architravesHeight / 2, zValue);
            architraves[11].transform.position = new Vector3(xValue, body.height - architravesHeight / 2, -zValue);
            architraves[12].transform.position = new Vector3(-xValue, body.height - architravesHeight / 2, -zValue);
            architraves[13].transform.position = new Vector3(-xValue, body.height - architravesHeight / 2, zValue);

            architraves[0].transform.Rotate(new Vector3(0, 45, 0));
            architraves[1].transform.Rotate(new Vector3(0, 0, 0));
            architraves[2].transform.Rotate(new Vector3(0, 0, 0));
            architraves[3].transform.Rotate(new Vector3(0, -45, 0));
            architraves[4].transform.Rotate(new Vector3(0, -90, 0));
            architraves[5].transform.Rotate(new Vector3(0, -135, 0));
            architraves[6].transform.Rotate(new Vector3(0, -180, 0));
            architraves[7].transform.Rotate(new Vector3(0, 180, 0));
            architraves[8].transform.Rotate(new Vector3(0, 135, 0));
            architraves[9].transform.Rotate(new Vector3(0, 90, 0));
            architraves[10].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            architraves[11].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            architraves[12].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            architraves[13].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
            foreach (GameObject obj in architraves)
            {
                obj.transform.parent = architraveObj.transform;
            }
            architraveObj.transform.parent = bodyObject.transform;
        }
        else
        {
            //算交叉柱與連接的柱子的長度
            intersectLength =
            Mathf.Sqrt(
            Mathf.Pow(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8), 2) +
            Mathf.Pow((cylinderX * Mathf.Sin(Mathf.PI * 3 / 8)) - (roof.width - roof.width / roof.disBetween) * 1.2f, 2));
            //算交叉柱與連接的柱子的角度
            angle =
            Mathf.Atan((roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8)) /
            (-(cylinderX * Mathf.Sin(Mathf.PI * 3 / 8)) + (roof.width - roof.width / roof.disBetween) * 1.2f))
            * 180.0 / Mathf.PI;
            GameObject architrave2 = creator.CreateArchitrave(intersectLength, architravesHeight);
            List<GameObject> architraves = new List<GameObject>()
            {
                architrave1,
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                Instantiate(architrave1),
                architrave2,
                Instantiate(architrave2),
                Instantiate(architrave2),
                Instantiate(architrave2)
            };
            float xValue = cylinderX * Mathf.Cos(Mathf.PI / 8);
            float zValue;
            architraves[0].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            architraves[1].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            architraves[2].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 4 / 4), body.height - architravesHeight / 2, xValue * Mathf.Sin(Mathf.PI * 4 / 4));
            architraves[3].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            architraves[4].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            architraves[5].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 0 / 4), body.height - architravesHeight / 2, -xValue * Mathf.Sin(Mathf.PI * 0 / 4));
            xValue = (roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8)) / 2;
            zValue = (roof.width - roof.width / roof.disBetween) * 1.2f + ((cylinderX * Mathf.Sin(Mathf.PI * 3 / 8)) - (roof.width - roof.width / roof.disBetween) * 1.2f) / 2;
            architraves[6].transform.position = new Vector3(xValue, body.height - architravesHeight / 2, zValue);
            architraves[7].transform.position = new Vector3(xValue, body.height - architravesHeight / 2, -zValue);
            architraves[8].transform.position = new Vector3(-xValue, body.height - architravesHeight / 2, -zValue);
            architraves[9].transform.position = new Vector3(-xValue, body.height - architravesHeight / 2, zValue);

            architraves[0].transform.Rotate(new Vector3(0, 45, 0));
            architraves[1].transform.Rotate(new Vector3(0, -45, 0));
            architraves[2].transform.Rotate(new Vector3(0, -90, 0));
            architraves[3].transform.Rotate(new Vector3(0, -135, 0));
            architraves[4].transform.Rotate(new Vector3(0, 135, 0));
            architraves[5].transform.Rotate(new Vector3(0, 90, 0));
            architraves[6].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            architraves[7].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            architraves[8].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            architraves[9].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
            foreach (GameObject obj in architraves)
            {
                obj.transform.parent = architraveObj.transform;
            }
            architraveObj.transform.parent = bodyObject.transform;
        }
        #endregion

        #region  生成所有楣子 並調整位置
        GameObject meiZiObj = new GameObject();
        meiZiObj.name = "MeiZis";
        float meiZiHeight = 0.75f;
        GameObject meiZi1 = creator.CreateMeiZi(cylinderX * Mathf.Sin(Mathf.PI / 8) * 2, meiZiHeight);
        if(roof.disBetween <= 1.8)
        {
            GameObject meiZi2 = creator.CreateMeiZi(intersectLength, meiZiHeight);
            List<GameObject> meiZis = new List<GameObject>()
            {
                meiZi1,
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                meiZi2,
                Instantiate(meiZi2),
                Instantiate(meiZi2),
                Instantiate(meiZi2)
            };
            float xValue = cylinderX * Mathf.Cos(Mathf.PI / 8);
            float zValue;
            meiZis[0].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            meiZis[1].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            meiZis[2].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            meiZis[3].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            meiZis[4].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 4 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 4 / 4));
            meiZis[5].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            meiZis[6].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            meiZis[7].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            meiZis[8].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            meiZis[9].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 0 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 0 / 4));
            xValue = (roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8)) / 2;
            zValue = (roof.width - roof.width / roof.disBetween) + ((cylinderX * Mathf.Sin(Mathf.PI * 5 / 8)) - (roof.width - roof.width / roof.disBetween)) / 2;
            meiZis[10].transform.position = new Vector3(xValue, body.height - meiZiHeight / 2 - architravesHeight, zValue);
            meiZis[11].transform.position = new Vector3(xValue, body.height - meiZiHeight / 2 - architravesHeight, -zValue);
            meiZis[12].transform.position = new Vector3(-xValue, body.height - meiZiHeight / 2 - architravesHeight, -zValue);
            meiZis[13].transform.position = new Vector3(-xValue, body.height - meiZiHeight / 2 - architravesHeight, zValue);

            meiZis[0].transform.Rotate(new Vector3(0, 45, 0));
            meiZis[1].transform.Rotate(new Vector3(0, 0, 0));
            meiZis[2].transform.Rotate(new Vector3(0, 0, 0));
            meiZis[3].transform.Rotate(new Vector3(0, -45, 0));
            meiZis[4].transform.Rotate(new Vector3(0, -90, 0));
            meiZis[5].transform.Rotate(new Vector3(0, -135, 0));
            meiZis[6].transform.Rotate(new Vector3(0, -180, 0));
            meiZis[7].transform.Rotate(new Vector3(0, 180, 0));
            meiZis[8].transform.Rotate(new Vector3(0, 135, 0));
            meiZis[9].transform.Rotate(new Vector3(0, 90, 0));
            meiZis[10].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            meiZis[11].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            meiZis[12].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            meiZis[13].transform.Rotate(new Vector3(0, (float)angle + 90, 0));

            foreach (GameObject obj in meiZis)
            {
                obj.transform.parent = meiZiObj.transform;
            }
            meiZiObj.transform.parent = bodyObject.transform;
        }
        else
        {
            GameObject meiZi2 = creator.CreateMeiZi(intersectLength, meiZiHeight);
            List<GameObject> meiZis = new List<GameObject>()
            {
                meiZi1,
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                Instantiate(meiZi1),
                meiZi2,
                Instantiate(meiZi2),
                Instantiate(meiZi2),
                Instantiate(meiZi2)
            };
            float xValue = cylinderX * Mathf.Cos(Mathf.PI / 8);
            float zValue;
            meiZis[0].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            meiZis[1].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            meiZis[2].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 4 / 4), body.height - meiZiHeight / 2 - architravesHeight, xValue * Mathf.Sin(Mathf.PI * 4 / 4));
            meiZis[3].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            meiZis[4].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            meiZis[5].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 0 / 4), body.height - meiZiHeight / 2 - architravesHeight, -xValue * Mathf.Sin(Mathf.PI * 0 / 4));
            xValue = (roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8)) / 2;
            zValue = (roof.width - roof.width / roof.disBetween) * 1.2f + ((cylinderX * Mathf.Sin(Mathf.PI * 3 / 8)) - (roof.width - roof.width / roof.disBetween) * 1.2f) / 2;
            meiZis[6].transform.position = new Vector3(xValue, body.height - meiZiHeight / 2 - architravesHeight, zValue);
            meiZis[7].transform.position = new Vector3(xValue, body.height - meiZiHeight / 2 - architravesHeight, -zValue);
            meiZis[8].transform.position = new Vector3(-xValue, body.height - meiZiHeight / 2 - architravesHeight, -zValue);
            meiZis[9].transform.position = new Vector3(-xValue, body.height - meiZiHeight / 2 - architravesHeight, zValue);

            meiZis[0].transform.Rotate(new Vector3(0, 45, 0));
            meiZis[1].transform.Rotate(new Vector3(0, -45, 0));
            meiZis[2].transform.Rotate(new Vector3(0, -90, 0));
            meiZis[3].transform.Rotate(new Vector3(0, -135, 0));
            meiZis[4].transform.Rotate(new Vector3(0, 135, 0));
            meiZis[5].transform.Rotate(new Vector3(0, 90, 0));
            meiZis[6].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            meiZis[7].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            meiZis[8].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            meiZis[9].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
            foreach (GameObject obj in meiZis)
            {
                obj.transform.parent = meiZiObj.transform;
            }
            meiZiObj.transform.parent = bodyObject.transform;
        }
        #endregion

        #region  生成所有雀替 並調整位置

        GameObject sparrowBraceObj = new GameObject();
        sparrowBraceObj.name = "SparrowBrace";
        float sparrowBraceHeight = 0.75f;
        GameObject sparrowBrace = creator.CreateSparrowBrace(cylinderX * 2, sparrowBraceHeight);

        if(roof.disBetween <= 1.8)
        {
            List<GameObject> sparrowBraces = new List<GameObject>();
            sparrowBraces.Add(sparrowBrace);
            for (int i = 0; i < 23; i++)
            {
                sparrowBraces.Add(Instantiate(sparrowBrace));
            }

            //從最右側的柱子開始逆時針，交叉的柱子不做
            sparrowBraces[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI / 8) - body.radius / 2);
            sparrowBraces[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI / 8));
            sparrowBraces[2].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 3 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[3].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) - body.radius / 2, body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[4].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2, body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[5].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 5 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[6].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 3 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[7].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) - body.radius / 2, body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[8].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2, body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[9].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 5 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[10].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 7 / 8));
            sparrowBraces[11].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) - body.radius / 2);
            sparrowBraces[12].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) + body.radius / 2);
            sparrowBraces[13].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 7 / 8));
            sparrowBraces[14].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 5 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[15].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2, body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[16].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) - body.radius / 2, body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[17].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 3 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[18].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 5 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[19].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2, body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[20].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) - body.radius / 2, body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[21].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 3 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[22].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI / 8));
            sparrowBraces[23].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI / 8) + body.radius / 2);

            sparrowBraces[0].transform.Rotate(new Vector3(0, 90, 0));
            sparrowBraces[1].transform.Rotate(new Vector3(0, 225, 0));
            sparrowBraces[2].transform.Rotate(new Vector3(0, 45, 0));
            sparrowBraces[3].transform.Rotate(new Vector3(0, 180, 0));
            sparrowBraces[4].transform.Rotate(new Vector3(0, 0, 0));
            sparrowBraces[5].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            sparrowBraces[6].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
            sparrowBraces[7].transform.Rotate(new Vector3(0, 180, 0));
            sparrowBraces[8].transform.Rotate(new Vector3(0, 0, 0));
            sparrowBraces[9].transform.Rotate(new Vector3(0, 135, 0));
            sparrowBraces[10].transform.Rotate(new Vector3(0, -45, 0));
            sparrowBraces[11].transform.Rotate(new Vector3(0, 90, 0));
            sparrowBraces[12].transform.Rotate(new Vector3(0, -90, 0));
            sparrowBraces[13].transform.Rotate(new Vector3(0, 45, 0));
            sparrowBraces[14].transform.Rotate(new Vector3(0, -135, 0));
            sparrowBraces[15].transform.Rotate(new Vector3(0, -0, 0));
            sparrowBraces[16].transform.Rotate(new Vector3(0, -180, 0));
            sparrowBraces[17].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            sparrowBraces[18].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            sparrowBraces[19].transform.Rotate(new Vector3(0, -0, 0));
            sparrowBraces[20].transform.Rotate(new Vector3(0, -180, 0));
            sparrowBraces[21].transform.Rotate(new Vector3(0, -45, 0));
            sparrowBraces[22].transform.Rotate(new Vector3(0, -225, 0));
            sparrowBraces[23].transform.Rotate(new Vector3(0, -90, 0));

            foreach (GameObject obj in sparrowBraces)
            {
                obj.transform.parent = sparrowBraceObj.transform;
            }
            sparrowBraceObj.transform.parent = bodyObject.transform;
        }
        else
        {
            List<GameObject> sparrowBraces = new List<GameObject>();
            sparrowBraces.Add(sparrowBrace);
            for (int i = 0; i < 15; i++)
            {
                sparrowBraces.Add(Instantiate(sparrowBrace));
            }

            //從最右側的柱子開始逆時針，交叉的柱子不做
            sparrowBraces[0].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI / 8) - body.radius / 2);
            sparrowBraces[1].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI / 8));
            sparrowBraces[2].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 3 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[3].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) - body.radius / 2, body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[4].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2, body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[5].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 5 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 5 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[6].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 7 / 8));
            sparrowBraces[7].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) - body.radius / 2);
            sparrowBraces[8].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) + body.radius / 2);
            sparrowBraces[9].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 7 / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI * 7 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 7 / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI * 7 / 8));
            sparrowBraces[10].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 5 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[11].transform.position = new Vector3(-roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8) + body.radius / 2, body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 5 / 8));
            sparrowBraces[12].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) - body.radius / 2, body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[13].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Cos(Mathf.PI * 3 / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI * 3 / 8) + body.radius / 2 * Mathf.Sin(Mathf.PI * 3 / 8));
            sparrowBraces[14].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8) - body.radius / 2 * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI / 8) - body.radius / 2 * Mathf.Sin(Mathf.PI / 8));
            sparrowBraces[15].transform.position = new Vector3(roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI / 8), body.height - architravesHeight - meiZiHeight, -cylinderX * Mathf.Sin(Mathf.PI / 8) + body.radius / 2);

            sparrowBraces[0].transform.Rotate(new Vector3(0, 90, 0));
            sparrowBraces[1].transform.Rotate(new Vector3(0, 225, 0));
            sparrowBraces[2].transform.Rotate(new Vector3(0, 45, 0));
            sparrowBraces[3].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            sparrowBraces[4].transform.Rotate(new Vector3(0, (float)angle + 90, 0));
            sparrowBraces[5].transform.Rotate(new Vector3(0, 135, 0));
            sparrowBraces[6].transform.Rotate(new Vector3(0, -45, 0));
            sparrowBraces[7].transform.Rotate(new Vector3(0, 90, 0));
            sparrowBraces[8].transform.Rotate(new Vector3(0, -90, 0));
            sparrowBraces[9].transform.Rotate(new Vector3(0, 45, 0));
            sparrowBraces[10].transform.Rotate(new Vector3(0, -135, 0));
            sparrowBraces[11].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            sparrowBraces[12].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            sparrowBraces[13].transform.Rotate(new Vector3(0, -45, 0));
            sparrowBraces[14].transform.Rotate(new Vector3(0, -225, 0));
            sparrowBraces[15].transform.Rotate(new Vector3(0, -90, 0));

            foreach (GameObject obj in sparrowBraces)
            {
                obj.transform.parent = sparrowBraceObj.transform;
            }
            sparrowBraceObj.transform.parent = bodyObject.transform;
        }
        #endregion

        #region  生成所有欄杆 並調整位置
        GameObject friezeInObj = new GameObject();
        friezeInObj.name = "Friezes";
        float friezeHeight = 0.75f;
        GameObject frieze1 = creator.CreateFriezeIn(cylinderX * Mathf.Sin(Mathf.PI / 8) * 2, friezeHeight);
        if(roof.disBetween <= 1.8)
        {
            GameObject frieze2 = creator.CreateFriezeIn(intersectLength, friezeHeight);
            List<GameObject> friezes = new List<GameObject>()
            {
                frieze1,
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                frieze2,
                Instantiate(frieze2),
                Instantiate(frieze2),
                Instantiate(frieze2)
            };

            float xValue = cylinderX * Mathf.Cos(Mathf.PI / 8);
            float zValue;
            friezes[0].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            friezes[1].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            friezes[2].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            friezes[3].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            friezes[4].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 4 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 4 / 4));
            friezes[5].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            friezes[6].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            friezes[7].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 2 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 2 / 4));
            friezes[8].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            friezes[9].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 0 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 0 / 4));
            xValue = (roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 5 / 8)) / 2;
            zValue = (roof.width - roof.width / roof.disBetween) + ((cylinderX * Mathf.Sin(Mathf.PI * 5 / 8)) - (roof.width - roof.width / roof.disBetween)) / 2;
            friezes[10].transform.position = new Vector3(xValue, 0, zValue);
            friezes[11].transform.position = new Vector3(xValue, 0, -zValue);
            friezes[12].transform.position = new Vector3(-xValue, 0, -zValue);
            friezes[13].transform.position = new Vector3(-xValue, 0, zValue);

            friezes[0].transform.Rotate(new Vector3(0, 45, 0));
            friezes[1].transform.Rotate(new Vector3(0, 0, 0));
            friezes[2].transform.Rotate(new Vector3(0, 0, 0));
            friezes[3].transform.Rotate(new Vector3(0, -45, 0));
            friezes[4].transform.Rotate(new Vector3(0, -90, 0));
            friezes[5].transform.Rotate(new Vector3(0, -135, 0));
            friezes[6].transform.Rotate(new Vector3(0, -180, 0));
            friezes[7].transform.Rotate(new Vector3(0, 180, 0));
            friezes[8].transform.Rotate(new Vector3(0, 135, 0));
            friezes[9].transform.Rotate(new Vector3(0, 90, 0));
            friezes[10].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            friezes[11].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            friezes[12].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            friezes[13].transform.Rotate(new Vector3(0, (float)angle + 90, 0));

            foreach (GameObject obj in friezes)
            {
                obj.transform.parent = friezeInObj.transform;
            }
            friezes[4].SetActive(false);
            friezes[9].SetActive(false);
            friezeInObj.transform.parent = bodyObject.transform;
        }
        else
        {
            GameObject frieze2 = creator.CreateFriezeIn(intersectLength, friezeHeight);
            List<GameObject> friezes = new List<GameObject>()
            {
                frieze1,
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                Instantiate(frieze1),
                frieze2,
                Instantiate(frieze2),
                Instantiate(frieze2),
                Instantiate(frieze2)
            };
            float xValue = cylinderX * Mathf.Cos(Mathf.PI / 8);
            float zValue;
            friezes[0].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            friezes[1].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            friezes[2].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 4 / 4), 0, xValue * Mathf.Sin(Mathf.PI * 4 / 4));
            friezes[3].transform.position = new Vector3(-roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 3 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 3 / 4));
            friezes[4].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 1 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 1 / 4));
            friezes[5].transform.position = new Vector3(roof.width / roof.disBetween + xValue * Mathf.Cos(Mathf.PI * 0 / 4), 0, -xValue * Mathf.Sin(Mathf.PI * 0 / 4));
            xValue = (roof.width / roof.disBetween + cylinderX * Mathf.Cos(Mathf.PI * 3 / 8)) / 2;
            zValue = (roof.width - roof.width / roof.disBetween) * 1.2f + ((cylinderX * Mathf.Sin(Mathf.PI * 3 / 8)) - (roof.width - roof.width / roof.disBetween) * 1.2f) / 2;
            friezes[6].transform.position = new Vector3(xValue, 0, zValue);
            friezes[7].transform.position = new Vector3(xValue, 0, -zValue);
            friezes[8].transform.position = new Vector3(-xValue, 0, -zValue);
            friezes[9].transform.position = new Vector3(-xValue, 0, zValue);

            friezes[0].transform.Rotate(new Vector3(0, 45, 0));
            friezes[1].transform.Rotate(new Vector3(0, -45, 0));
            friezes[2].transform.Rotate(new Vector3(0, -90, 0));
            friezes[3].transform.Rotate(new Vector3(0, -135, 0));
            friezes[4].transform.Rotate(new Vector3(0, 135, 0));
            friezes[5].transform.Rotate(new Vector3(0, 90, 0));
            friezes[6].transform.Rotate(new Vector3(0, (float)-angle - 90, 0));
            friezes[7].transform.Rotate(new Vector3(0, (float)angle - 90, 0));
            friezes[8].transform.Rotate(new Vector3(0, (float)-angle + 90, 0));
            friezes[9].transform.Rotate(new Vector3(0, (float)angle + 90, 0));

            foreach (GameObject obj in friezes)
            {
                obj.transform.parent = friezeInObj.transform;
            }
            friezes[2].SetActive(false);
            friezes[5].SetActive(false);
            friezeInObj.transform.parent = bodyObject.transform;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    /// <param name="platform"></param>
    public void CreatePlatform(Platform platform)
    {
        switch(roof.combineType)
        {
            case 0 :
                CreateNormalPlatform(platform);
                break;
            case 1 :
                CreateFangShengPlatform(platform);
                break;
            case 2 :
                CreateDoubleHexaPlatform(platform);
                break;
            case 3 :
                CreateDoubleOctoPlatform(platform);
                break;
            default:
                break;
        }
    }

    public void CreateNormalPlatform(Platform platform)
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
        // GameObject frence = platformCreator.CreateFrence(new Vector3(platformX, 12, platformZ), false);
        // frence.transform.Translate(new Vector3(0, platform.height, 0));
        //階層關係
        edges1.transform.parent = platformBody.transform;
        edges2.transform.parent = platformBody.transform;
        // frence.transform.parent = platformBody.transform;
        platformBody.transform.Translate(new Vector3(0, platform.height, 0));
        platformBody.transform.parent = platformObject.transform;
        platformObject.transform.parent = building.transform;
    }

    public void CreateFangShengPlatform(Platform platform)
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

        platformBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platformBody.transform.localScale = new Vector3(platformX, platform.height * 2, platformX);
        platformBody.GetComponent<MeshRenderer>().material = platformCreator.platFormMaterial;
        platformBody.name = "platform";

        // 邊界
        GameObject edges1 = platformCreator.CreateFangShengEdges(platform.height, platformX, roof.width / roof.disBetween);
        GameObject edges2 = platformCreator.CreateFangShengEdges(-platform.height, platformX, roof.width / roof.disBetween);

        // 圍牆
        // 變數名稱1為長邊 2為短邊
        // distance為模型與模型間的距離
        // scale為1個模型的長度
        // 先決定長邊模型數
        // 推算出每個模型多長
        // 再依照長短邊的比例算出短邊需要幾個模型(因為要是整數 所以先決定數量)
        // 最後推算短邊每個模型的長度

        int fenceCount1 = Mathf.FloorToInt(platformX / 2);
        if (fenceCount1 % 2 == 0)
            fenceCount1++;

        float spacingDistance1 = platformX / (float)fenceCount1;
        // 0.317是模型寬度 目的是以原始模型寬度為1來縮放
        float fenceScale1 = spacingDistance1 / 0.317f;
        GameObject fence1 = platformCreator.CreateFrence(platformX, fenceScale1, spacingDistance1, platform.height, fenceCount1);
        float length2 = (roof.width * Mathf.Sqrt(2) / roof.disBetween);
        int fenceCount2 = Mathf.CeilToInt(fenceCount1 * length2 / platformX);
        float spacingDistance2 = length2 / fenceCount2;
        float fenceScale2 = spacingDistance2 / 0.317f;
        GameObject fence2 = platformCreator.CreateFrence(length2, fenceScale2, spacingDistance2, platform.height, fenceCount2);
        List<GameObject> fences = new List<GameObject>()
        {
            fence1,
            Instantiate(fence1),
            fence2,
            Instantiate(fence2),
        };
        fences[0].transform.position = new Vector3(0, 0, -platformX / 2);
        fences[1].transform.position = new Vector3(platformX / 2, 0, 0);
        fences[2].transform.position = new Vector3(platformX / 2 - length2 / 2, 0, platformX / 2);
        fences[3].transform.position = new Vector3(-platformX / 2, 0, -platformX / 2 + length2 / 2);
        fences[1].transform.Rotate(new Vector3(0, 90, 0));
        fences[3].transform.Rotate(new Vector3(0, 90, 0));
        GameObject fencesObject = new GameObject();
        foreach(GameObject obj in fences)
        {
            obj.transform.parent = fencesObject.transform;
        }

        //階層關係
        edges1.transform.parent = platformBody.transform;
        edges2.transform.parent = platformBody.transform;
        fencesObject.transform.parent = platformBody.transform;
        platformBody.transform.Translate(new Vector3(0, platform.height, 0));

        GameObject leftPlatformBody = Instantiate(platformBody);
        platformBody.transform.parent = platformObject.transform;
        leftPlatformBody.transform.parent = platformObject.transform;
        platformObject.transform.parent = building.transform;

        platformBody.transform.Translate(new Vector3(roof.width / roof.disBetween, 0, 0));
        platformBody.transform.Rotate(new Vector3(0, -45, 0));
        leftPlatformBody.transform.Translate(new Vector3(-roof.width / roof.disBetween, 0, 0));
        leftPlatformBody.transform.Rotate(new Vector3(0, 135, 0));

        // 合併左右兩邊的臺基
        // CombineInstance[] combine = new CombineInstance[2];
        // combine[0].mesh = platformBody.gameObject.GetComponent<MeshFilter>().mesh;
        // combine[0].transform = platformBody.transform.localToWorldMatrix;
        // combine[1].mesh = leftPlatformBody.gameObject.GetComponent<MeshFilter>().mesh;
        // combine[1].transform = leftPlatformBody.transform.localToWorldMatrix;
        // Mesh combinedMesh= new Mesh();
        // combinedMesh.CombineMeshes(combine);
        // combinedMesh.RecalculateBounds();
        // platformBody.gameObject.GetComponent<MeshFilter>().mesh = combinedMesh;
        // platformBody.transform.position = new Vector3(0, 0, 0);
        // platformBody.transform.rotation = new Quaternion(0, 0, 0, 0);
        // platformBody.transform.localScale = new Vector3(1, 1, 1);
    }

    public void CreateDoubleHexaPlatform(Platform platform)
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

        float platformX = (roof.length / 2 + lastPointX) + platform.length;

        platformBody = platformCreator.CreatePrism(platformX, platform.height * 2, 6);
        platformBody.GetComponent<MeshRenderer>().material = platformCreator.platFormMaterial;
        platformBody.name = "platform";

        // 邊界
        GameObject edges1 = platformCreator.CreateDoubleHexaEdges(platform.height, platformX, roof.width / roof.disBetween);
        GameObject edges2 = platformCreator.CreateDoubleHexaEdges(-platform.height, platformX, roof.width / roof.disBetween);

        // 圍牆
        // 變數名稱1為長邊 2為短邊
        // distance為模型與模型間的距離
        // scale為1個模型的長度
        // 先決定長邊模型數
        // 推算出每個模型多長
        // 再依照長短邊的比例算出短邊需要幾個模型(因為要是整數 所以先決定數量)
        // 最後推算短邊每個模型的長度

        int fenceCount1 = Mathf.FloorToInt(platformX / 2);
        if (fenceCount1 % 2 == 0)
            fenceCount1++;

        float spacingDistance1 = platformX / (float)fenceCount1;
        // 0.317是模型寬度 目的是以原始模型寬度為1來縮放
        float fenceScale1 = spacingDistance1 / 0.317f;
        GameObject fence1 = platformCreator.CreateFrence(platformX, fenceScale1, spacingDistance1, platform.height, fenceCount1);
        //稍微縮一點不然底部會露出來
        float length2 = (roof.width * 2 / Mathf.Sqrt(3) / roof.disBetween) * 0.99f;
        int fenceCount2 = Mathf.CeilToInt(fenceCount1 * length2 / platformX);
        float spacingDistance2 = length2 / fenceCount2;
        float fenceScale2 = spacingDistance2 / 0.317f;
        GameObject fence2 = platformCreator.CreateFrence(length2, fenceScale2, spacingDistance2, platform.height, fenceCount2);
        List<GameObject> fences = new List<GameObject>()
        {
            fence1,
            Instantiate(fence1),
            Instantiate(fence1),
            fence2,
            Instantiate(fence2),
        };
        fences[0].transform.position = new Vector3(platformX * Mathf.Sqrt(3) / 4, 0, platformX * 3 / 4);
        fences[1].transform.position = new Vector3(platformX * Mathf.Sqrt(3) / 2, 0, 0);
        fences[2].transform.position = new Vector3(platformX * Mathf.Sqrt(3) / 4, 0, -platformX * 3 / 4);
        fences[3].transform.position = new Vector3(-roof.width / roof.disBetween / 2, 0, -platformX + roof.width / roof.disBetween / 2 / Mathf.Sqrt(3));
        fences[4].transform.position = new Vector3(-roof.width / roof.disBetween / 2, 0, platformX - roof.width / roof.disBetween / 2 / Mathf.Sqrt(3));
        fences[0].transform.Rotate(new Vector3(0, 30, 0));
        fences[1].transform.Rotate(new Vector3(0, 90, 0));
        fences[2].transform.Rotate(new Vector3(0, 150, 0));
        fences[3].transform.Rotate(new Vector3(0, 210, 0));
        fences[4].transform.Rotate(new Vector3(0, -30, 0));
        GameObject fencesObject = new GameObject();
        foreach(GameObject obj in fences)
        {
            obj.transform.parent = fencesObject.transform;
        }

        //階層關係
        edges1.transform.parent = platformBody.transform;
        edges2.transform.parent = platformBody.transform;
        fencesObject.transform.parent = platformBody.transform;
        platformBody.transform.Translate(new Vector3(0, platform.height, 0));

        GameObject leftPlatformBody = Instantiate(platformBody);
        platformBody.transform.parent = platformObject.transform;
        leftPlatformBody.transform.parent = platformObject.transform;
        platformObject.transform.parent = building.transform;

        platformBody.transform.Translate(new Vector3(roof.width / roof.disBetween, 0, 0));
        leftPlatformBody.transform.Translate(new Vector3(-roof.width / roof.disBetween, 0, 0));
        leftPlatformBody.transform.Rotate(new Vector3(0, 180, 0));

        // 合併左右兩邊的臺基
        // CombineInstance[] combine = new CombineInstance[2];
        // combine[0].mesh = platformBody.gameObject.GetComponent<MeshFilter>().mesh;
        // combine[0].transform = platformBody.transform.localToWorldMatrix;
        // combine[1].mesh = leftPlatformBody.gameObject.GetComponent<MeshFilter>().mesh;
        // combine[1].transform = leftPlatformBody.transform.localToWorldMatrix;
        // Mesh combinedMesh= new Mesh();
        // combinedMesh.CombineMeshes(combine);
        // combinedMesh.RecalculateBounds();
        // platformBody.gameObject.GetComponent<MeshFilter>().mesh = combinedMesh;
        // platformBody.transform.position = new Vector3(0, 0, 0);
        // platformBody.transform.rotation = new Quaternion(0, 0, 0, 0);
        // platformBody.transform.localScale = new Vector3(1, 1, 1);
    }

    public void CreateDoubleOctoPlatform(Platform platform)
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

        float platformX = lastPointX + platform.length;

        platformBody = platformCreator.CreatePrism(platformX, platform.height * 2, 8);
        platformBody.GetComponent<MeshRenderer>().material = platformCreator.platFormMaterial;
        platformBody.name = "platform";

        // 邊界
        GameObject edges1 = platformCreator.CreateDoubleOctoEdges(platform.height, platformX, roof.width / roof.disBetween, roof.disBetween < 1.6);
        GameObject edges2 = platformCreator.CreateDoubleOctoEdges(-platform.height, platformX, roof.width / roof.disBetween, roof.disBetween < 1.6);

        // 圍牆
        // 變數名稱1為長邊 2為短邊
        // distance為模型與模型間的距離
        // scale為1個模型的長度
        // 先決定長邊模型數
        // 推算出每個模型多長
        // 再依照長短邊的比例算出短邊需要幾個模型(因為要是整數 所以先決定數量)
        // 最後推算短邊每個模型的長度

        List<GameObject> fences;
        float angle225 = Mathf.PI / 8;
        float angle45 = Mathf.PI / 4;

        float length1 = platformX * Mathf.Sin(Mathf.PI / 8) * 2;
        int fenceCount1 = Mathf.FloorToInt(length1 / 2);
        if (fenceCount1 % 2 == 0)
            fenceCount1++;

        float spacingDistance1 = length1 / (float)fenceCount1;
        // 0.317是模型寬度 目的是以原始模型寬度為1來縮放
        float fenceScale1 = spacingDistance1 / 0.317f;
        GameObject fence1 = platformCreator.CreateFrence(length1, fenceScale1, spacingDistance1, platform.height, fenceCount1);
        //稍微縮一點不然底部會露出來
        float length2;
        if(roof.disBetween < 1.6)
            length2 = (roof.width / roof.disBetween - length1 / 2) / Mathf.Sin(Mathf.PI / 4) * 0.99f;
        else
            length2 = (roof.width / roof.disBetween + length1 / 2) * 0.99f;
        int fenceCount2 = Mathf.CeilToInt(fenceCount1 * length2 / length1);
        float spacingDistance2 = length2 / fenceCount2;
        float fenceScale2 = spacingDistance2 / 0.317f;
        GameObject fence2 = platformCreator.CreateFrence(length2, fenceScale2, spacingDistance2, platform.height, fenceCount2);

        fences = new List<GameObject>()
        {
            fence1,
            Instantiate(fence1),
            Instantiate(fence1)
        };
        if(roof.disBetween < 1.6)
        {
            fences.Add(Instantiate(fence1));
            fences.Add(Instantiate(fence1));
            fences.Add(fence2);
            fences.Add(Instantiate(fence2));
        }
        else
        {
            fences.Add(fence2);
            fences.Add(Instantiate(fence2));
        }
        for(int i = 0; i < 3; i++)
        {
            fences[i].transform.position =
            new Vector3(platformX * Mathf.Cos(angle225) * Mathf.Cos(-angle225 + (-i + 2) * angle45), 0, platformX * Mathf.Cos(angle225) * Mathf.Sin(-angle225 + (-i + 2) * angle45));
            fences[i].transform.Rotate(0, 22.5f + i * 45, 0);
        }

        if(roof.disBetween < 1.6)
        {
            // 先處理固定邊長
            fences[3].transform.position = new Vector3(platformX * Mathf.Cos(angle225) * Mathf.Cos(-angle225 + 3 * angle45), 0, platformX * Mathf.Cos(angle225) * Mathf.Sin(-angle225 + 3 * angle45));
            fences[4].transform.position = new Vector3(platformX * Mathf.Cos(angle225) * Mathf.Cos(-angle225 - 1 * angle45), 0, platformX * Mathf.Cos(angle225) * Mathf.Sin(-angle225 - 1 * angle45));
            fences[3].transform.Rotate(0, 22.5f + 3 * 45, 0);
            fences[4].transform.Rotate(0, 22.5f - 1 * 45, 0);

            // 再做接觸邊
            float intersectL = (roof.width / roof.disBetween - length1 / 2) / Mathf.Sin(angle45);
            fences[5].transform.position = new Vector3(-platformX * Mathf.Cos(angle45) - intersectL * Mathf.Sin(angle225) / 2, 0, platformX * Mathf.Sin(angle45) - intersectL * Mathf.Cos(angle225) / 2);
            fences[6].transform.position = new Vector3(-intersectL * Mathf.Cos(angle225) / 2, 0, -platformX + intersectL * Mathf.Sin(angle225) / 2);
            fences[5].transform.Rotate(0, 112.5f, 0);
            fences[6].transform.Rotate(0, 22.5f, 0);
        }
        else
        {
            float intersectL = roof.width / roof.disBetween + length1 / 2;
            fences[3].transform.position = new Vector3(-intersectL * Mathf.Cos(angle225) / 2, 0, platformX - intersectL * Mathf.Sin(angle225) / 2);
            fences[4].transform.position = new Vector3(platformX - length1 * Mathf.Sin(angle225) - intersectL* Mathf.Cos(angle225) / 2, 0, -length1 * Mathf.Cos(angle225) - intersectL * Mathf.Sin(angle225) / 2);
            fences[3].transform.Rotate(0, 22.5f + 3 * 45, 0);
            fences[4].transform.Rotate(0, 22.5f - 1 * 45, 0);
        }

        GameObject fencesObject = new GameObject();
        foreach(GameObject obj in fences)
        {
            obj.transform.parent = fencesObject.transform;
        }

        //階層關係
        edges1.transform.parent = platformBody.transform;
        edges2.transform.parent = platformBody.transform;
        fencesObject.transform.parent = platformBody.transform;
        platformBody.transform.Translate(new Vector3(0, platform.height, 0));

        GameObject leftPlatformBody = Instantiate(platformBody);
        platformBody.transform.parent = platformObject.transform;
        leftPlatformBody.transform.parent = platformObject.transform;
        platformObject.transform.parent = building.transform;

        platformBody.transform.Translate(new Vector3(roof.width / roof.disBetween, 0, 0));
        platformBody.transform.Rotate(new Vector3(0, 22.5f, 0));
        leftPlatformBody.transform.Translate(new Vector3(-roof.width / roof.disBetween, 0, 0));
        leftPlatformBody.transform.Rotate(new Vector3(0, 202.5f, 0));
    }

    private void Cut(GameObject victim, Vector3 anchorPoint, Vector3 normalDirection, string name)
    {
        GameObject[] product;
        product = BLINDED_AM_ME.MeshCut.Cut(victim, anchorPoint, normalDirection, cutMaterial);
        product[0].name = name;
        victim = product[0];
        product[1].GetComponent<MeshFilter>().mesh.Clear();
        Destroy(product[1]);
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
        GameObject ridgeWidth;

        if(roof.combineType == 0)
            ridgeWidth = creator.Create(totalPoints, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        else
            ridgeWidth = new GameObject();
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

    public void CreateCrossRightRidge()
    {
        float lengthAnchor = roof.length - roof.topLowerLength * 2;
        float deepAnchor = roof.deep - roof.topLowerLength * 2;
        CircleCurve circleCurve = new CircleCurve();
        //生成左右邊的弧

        List<Vector3> totalPoints = new List<Vector3>();

        totalPoints.Add(new Vector3(-roof.length / 2, 0, 0));
        totalPoints.Add(new Vector3(roof.length / 2, 0, 0));

        rightRidgeFrontPoints = new List<Vector3>(totalPoints);
        totalPoints.Reverse();
        RafterCreator creator = chinesePavilionCreater.GetComponent<RafterCreator>();
        //生成正脊mesh
        GameObject ridgeWidth = creator.Create(totalPoints, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        // GameObject ridgeWidth = new GameObject();
        ridgeWidth.name = "RightRidge";
        //存到bargeboardPoints 畫mesh用

        //deep RightRidge
        totalPoints = new List<Vector3>();

        totalPoints.Add(new Vector3(-roof.deep / 2, roof.topLowerHeight, 0));
        totalPoints.Add(new Vector3(roof.deep / 2, roof.topLowerHeight, 0));

        totalPoints.Reverse();
        List<GameObject> ridges = new List<GameObject>();
        ridges.Add(ridgeWidth);
        ridges.Add(Instantiate(ridgeWidth));
        ridges.Add(Instantiate(ridgeWidth));
        ridges.Add(Instantiate(ridgeWidth));
        // float baoShaFlyingRafterY = -roof.baoShaHeight + roof.topLowerHeight + roof.sideEaveHeight;
        ridges[0].transform.position = new Vector3(0, 0, -roof.width - roof.baoShaWidth / 2);
        ridges[1].transform.position = new Vector3(roof.width + roof.baoShaWidth / 2, 0, 0);
        ridges[1].transform.Rotate(new Vector3(0, 90, 0));
        ridges[2].transform.position = new Vector3(0, 0, roof.width + roof.baoShaWidth / 2);
        ridges[2].transform.Rotate(new Vector3(0, 180, 0));
        ridges[3].transform.position = new Vector3(-roof.width - roof.baoShaWidth / 2, 0, 0);
        ridges[3].transform.Rotate(new Vector3(0, 270, 0));
        
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
        List<Vector3> curve;
        if(roof.combineType == 4)
        {
            curve = circleCurve.CreateCircleCurve(roof.baoShaHeight, roof.baoShaWidth, roof.curve, circleCurveRes * 2);
            //倒過來
            curve.Reverse();

            //bargeboardPoints 畫mesh用
            baoShaBargeboardPoints = new List<Vector3>();
            float angle = Mathf.PI / 2;
            foreach (Vector3 pos in curve)
            {
                float x = pos.x * Mathf.Cos(angle) + pos.z * Mathf.Sin(angle);
                float y = pos.y + -(roof.baoShaHeight - roof.topLowerHeight);
                float newZ = -pos.x * Mathf.Sin(angle) + pos.z * Mathf.Cos(angle) + roof.baoShaWidth;
                baoShaBargeboardPoints.Add(new Vector3(x, y, newZ));
            }

            curve = circleCurve.CreateCircleCurve(roof.height, roof.width, roof.curve, circleCurveRes * 2);
            //倒過來
            curve.Reverse();

            //bargeboardPoints 畫mesh用
            bargeboardPoints = new List<Vector3>();
            angle = Mathf.PI / 2;
            foreach (Vector3 pos in curve)
            {
                float x = pos.x * Mathf.Cos(angle) + pos.z * Mathf.Sin(angle);
                float y = pos.y + -(roof.height - roof.topLowerHeight);
                float newZ = -pos.x * Mathf.Sin(angle) + pos.z * Mathf.Cos(angle) + roof.width;
                bargeboardPoints.Add(new Vector3(x, y, newZ));
            }
        }
        else
        {
            curve = circleCurve.CreateCircleCurve(roof.height, roof.width, roof.curve, circleCurveRes * 2);
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
        }

        if (roof.length < 0.02f)
            return;

        //生成ridge的mesh
        RafterCreator creator = chinesePavilionCreater.GetComponent<RafterCreator>();
        GameObject ridge;
        if(roof.combineType == 0)
        {
            ridge = creator.Create(curve, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
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
        // else if(roof.combineType == 4)
        // {
        //     ridge = creator.Create(curve, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        //     //生成四個，並調整位置
        //     List<GameObject> ridges = new List<GameObject>
        //     {
        //         ridge,
        //         Instantiate(ridge),
        //         Instantiate(ridge),
        //         Instantiate(ridge)
        //     };
        //     float z;
        //     z = roof.deep < 0.01f ? (roof.width + roof.deep) : (roof.width + roof.deep / 2);

        //     ridges[0].transform.position = new Vector3(roof.length / 2, -(roof.height - roof.topLowerHeight), z);
        //     ridges[0].transform.Rotate(new Vector3(0, 90f, 0));
        //     ridges[1].transform.position = new Vector3(roof.length / 2, -(roof.height - roof.topLowerHeight), -z);
        //     ridges[1].transform.Rotate(new Vector3(0, 270f, 0));
        //     ridges[2].transform.position = new Vector3(-roof.length / 2, -(roof.height - roof.topLowerHeight), -z);
        //     ridges[2].transform.Rotate(new Vector3(0, 270, 0));
        //     ridges[3].transform.position = new Vector3(-roof.length / 2, -(roof.height - roof.topLowerHeight), z);
        //     ridges[3].transform.Rotate(new Vector3(0, 90, 0));
        //     GameObject ridgesObj = new GameObject("Ridges");
        //     foreach (GameObject obj in ridges)
        //     {
        //         obj.name = "Bargeboard";
        //         obj.transform.parent = ridgesObj.transform;
        //         // obj.SetActive(false);
        //     }
        //     ridgesObj.transform.parent = roofObject.transform;
        // }

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
        //留下最上面1/disBetween的點
        for (int i = 0; i < connectedFlyingRafterCurve.Count; i++)
            if (connectedFlyingRafterCurve[i].x < Mathf.Floor(sideEaveWidth * (roof.disBetween - 1) / roof.disBetween))
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
        flyingRafters[0].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[0].transform.Rotate(new Vector3(0, 90, 0));
        flyingRafters[1].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[1].transform.Rotate(new Vector3(0, 180, 0));
        flyingRafters[2].transform.position = new Vector3(roof.width / roof.disBetween -roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[2].transform.Rotate(new Vector3(0, 270, 0));
        flyingRafters[3].transform.position = new Vector3(roof.width / roof.disBetween -roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[3].transform.Rotate(new Vector3(0, 0, 0));
        flyingRafters[4].transform.position = new Vector3(-roof.width / roof.disBetween + roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
        flyingRafters[4].transform.Rotate(new Vector3(0, 90, 0));
        flyingRafters[5].transform.position = new Vector3(-roof.width / roof.disBetween + roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[5].transform.Rotate(new Vector3(0, 180, 0));
        flyingRafters[6].transform.position = new Vector3(-roof.width / roof.disBetween  -roof.length / 2, flyingRafterY, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[6].transform.Rotate(new Vector3(0, 270, 0));
        flyingRafters[7].transform.position = new Vector3(-roof.width / roof.disBetween  -roof.length / 2, flyingRafterY, (flyingRafterZ + roof.deep / 2));
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
    /// 創建雙六角亭的屋脊
    /// </summary>
    public void CreateDoubleHexaRidge()
    {
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
        //留下最上面1/disBetween的點
        for (int i = 0; i < connectedFlyingRafterCurve.Count; i++)
            if (connectedFlyingRafterCurve[i].x < Mathf.Floor((sideEaveWidth * (roof.disBetween - 1) / roof.disBetween) / Mathf.Sqrt(3) * 2))
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
        // GameObject connectedFlyingRafter = creator.Create(connectedOffsetPoint, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        List<GameObject> flyingRafters = new List<GameObject>
        {
            Instantiate(flyingRafter),
            flyingRafter,
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
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
        flyingRafters[0].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[0].transform.Rotate(new Vector3(0, 30, 0));
        flyingRafters[1].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[1].transform.Rotate(new Vector3(0, 90, 0));
        flyingRafters[2].transform.position = new Vector3(roof.width / roof.disBetween -roof.length / 2, flyingRafterY, 0);
        flyingRafters[2].transform.Rotate(new Vector3(0, 150, 0));
        flyingRafters[3].transform.position = new Vector3(roof.width / roof.disBetween -roof.length / 2, flyingRafterY, 0);
        flyingRafters[3].transform.Rotate(new Vector3(0, -150, 0));
        flyingRafters[4].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[4].transform.Rotate(new Vector3(0, -90, 0));
        flyingRafters[5].transform.position = new Vector3(roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[5].transform.Rotate(new Vector3(0, -30, 0));
        flyingRafters[6].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[6].transform.Rotate(new Vector3(0, 30, 0));
        flyingRafters[7].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[7].transform.Rotate(new Vector3(0, 90, 0));
        flyingRafters[8].transform.position = new Vector3(-roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[8].transform.Rotate(new Vector3(0, 150, 0));
        flyingRafters[9].transform.position = new Vector3(-roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[9].transform.Rotate(new Vector3(0, -150, 0));
        flyingRafters[10].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[10].transform.Rotate(new Vector3(0, -90, 0));
        flyingRafters[11].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[11].transform.Rotate(new Vector3(0, -30, 0));

        Cut(flyingRafters[0], new Vector3(0,0,0), new Vector3(-1,0,0), "FlyingRafter");
        Cut(flyingRafters[5], new Vector3(0,0,0), new Vector3(-1,0,0), "FlyingRafter");
        Cut(flyingRafters[8], new Vector3(0,0,0), new Vector3(1,0,0), "FlyingRafter");
        Cut(flyingRafters[9], new Vector3(0,0,0), new Vector3(1,0,0), "FlyingRafter");

        foreach (GameObject obj in flyingRafters)
        {
            obj.name = "FlyingRafter";
            obj.transform.parent = raftersObj.transform;
        }
        raftersObj.transform.parent = roofObject.transform;

        // flyingRafterPoints 畫mesh用
        flyingRafterPoints = offsetPoint;
        flyingBargePointNum = bargeboardPointUpper;

    }

    public void CreateDoubleOctoRidge()
    {
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
        //留下最上面1/disBetween的點
        for (int i = 0; i < connectedFlyingRafterCurve.Count; i++)
            if (connectedFlyingRafterCurve[i].x < Mathf.Floor((sideEaveWidth * (roof.disBetween - 1) / roof.disBetween) / Mathf.Sqrt(3) * 2))
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
        // GameObject connectedFlyingRafter = creator.Create(connectedOffsetPoint, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        List<GameObject> flyingRafters = new List<GameObject>
        {
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            flyingRafter,
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
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
        flyingRafters[0].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[0].transform.Rotate(new Vector3(0, 22.5f, 0));
        flyingRafters[1].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[1].transform.Rotate(new Vector3(0, 67.5f, 0));
        flyingRafters[2].transform.position = new Vector3(roof.width / roof.disBetween -roof.length / 2, flyingRafterY, 0);
        flyingRafters[2].transform.Rotate(new Vector3(0, 112.5f, 0));
        flyingRafters[3].transform.position = new Vector3(roof.width / roof.disBetween -roof.length / 2, flyingRafterY, 0);
        flyingRafters[3].transform.Rotate(new Vector3(0, 157.5f, 0));
        flyingRafters[4].transform.position = new Vector3(roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[4].transform.Rotate(new Vector3(0, -157.5f, 0));
        flyingRafters[5].transform.position = new Vector3(roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[5].transform.Rotate(new Vector3(0, -112.5f, 0));
        flyingRafters[6].transform.position = new Vector3(roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[6].transform.Rotate(new Vector3(0, -67.5f, 0));
        flyingRafters[7].transform.position = new Vector3(roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[7].transform.Rotate(new Vector3(0, -22.5f, 0));
        flyingRafters[8].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[8].transform.Rotate(new Vector3(0, 22.5f, 0));
        flyingRafters[9].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[9].transform.Rotate(new Vector3(0, 67.5f, 0));
        flyingRafters[10].transform.position = new Vector3(-roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[10].transform.Rotate(new Vector3(0, 112.5f, 0));
        flyingRafters[11].transform.position = new Vector3(-roof.width / roof.disBetween + roof.length / 2, flyingRafterY, 0);
        flyingRafters[11].transform.Rotate(new Vector3(0, 157.5f, 0));
        flyingRafters[12].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[12].transform.Rotate(new Vector3(0, -157.5f, 0));
        flyingRafters[13].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[13].transform.Rotate(new Vector3(0, -112.5f, 0));
        flyingRafters[14].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[14].transform.Rotate(new Vector3(0, -67.5f, 0));
        flyingRafters[15].transform.position = new Vector3(-roof.width / roof.disBetween - roof.length / 2, flyingRafterY, 0);
        flyingRafters[15].transform.Rotate(new Vector3(0, -22.5f, 0));

        flyingRafters[0].GetComponent<MeshRenderer>().material.shader =
        flyingRafters[1].GetComponent<MeshRenderer>().material.shader =
        flyingRafters[6].GetComponent<MeshRenderer>().material.shader =
        flyingRafters[7].GetComponent<MeshRenderer>().material.shader = Shader.Find("Tessellation/Clip");
        flyingRafters[10].GetComponent<MeshRenderer>().material.shader =
        flyingRafters[11].GetComponent<MeshRenderer>().material.shader =
        flyingRafters[12].GetComponent<MeshRenderer>().material.shader =
        flyingRafters[13].GetComponent<MeshRenderer>().material.shader = Shader.Find("Tessellation/Clip Opposite");

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

    public void CreateCrossRidge()
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

        #region BaoSha
        // 製作抱廈的垂脊
        List<Vector3> baoShaCurve = bargeBoardCurve.CreateCircleCurve(roof.baoShaHeight, roof.baoShaWidth, roof.curve, circleCurveRes * 2);
        float baoShaSideEaveLength = roof.baoShaWidth - roof.sideEaveStart;

        //拿取垂脊點的百分比 這樣才能精準吻合
        bargeboardPercent = (baoShaSideEaveLength / roof.baoShaWidth * (baoShaCurve.Count - 1f));
        int baoShaBargeboardPointUpper = Mathf.CeilToInt(bargeboardPercent);
        bargeboardPointLower = Mathf.FloorToInt(bargeboardPercent);
        t = bargeboardPercent - bargeboardPointLower;
        flyingRafterHeight = baoShaCurve[baoShaBargeboardPointUpper].y;
        sideEaveWidth = Mathf.Sqrt(Mathf.Pow(baoShaCurve[baoShaBargeboardPointUpper].x, 2) + Mathf.Pow(baoShaCurve[baoShaBargeboardPointUpper].x, 2));

        //生成飛簷的弧
        circleCurve = new CircleCurve();
        flyingRafterCurve = circleCurve.CreateCircleCurve(flyingRafterHeight - roof.sideEaveHeight, sideEaveWidth, roof.sideEaveCurve, baoShaBargeboardPointUpper);

        //進行位移
        List<Vector3> baoShaOffsetPoint = new List<Vector3>();
        foreach (Vector3 pos in flyingRafterCurve)
        {
            baoShaOffsetPoint.Add(new Vector3(pos.x - sideEaveWidth, pos.y));
        }

        baoShaOffsetPoint.Reverse();

        GameObject baoShaRafter = creator.Create(baoShaOffsetPoint, rafterNbSides, circleCurveRes / 2 + 1, rafterHeight, rafterTall, rafterRadius);
        #endregion

        List<GameObject> flyingRafters = new List<GameObject>
        {
            flyingRafter,
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            Instantiate(flyingRafter),
            baoShaRafter,
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter),
            Instantiate(baoShaRafter)
        };
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        float baoShaFlyingRafterY = -roof.baoShaHeight + roof.sideEaveHeight;
        if (roof.deep < 0.01f)
            flyingRafterZ = curve[curve.Count - 1].x - curve[bargeboardPointUpper].x;
        else
            flyingRafterZ = curve[curve.Count - 1].x - curve[bargeboardPointUpper].x;
        // Debug.Log("height:"+height);
        flyingRafters[0].transform.position = new Vector3(0, flyingRafterY + body.secondFloorHeight, (flyingRafterZ + roof.deep / 2));
        flyingRafters[0].transform.Rotate(new Vector3(0, 135, 0));
        flyingRafters[1].transform.position = new Vector3(0, flyingRafterY + body.secondFloorHeight, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[1].transform.Rotate(new Vector3(0, 225f, 0));
        flyingRafters[2].transform.position = new Vector3(0, flyingRafterY + body.secondFloorHeight, -(flyingRafterZ + roof.deep / 2));
        flyingRafters[2].transform.Rotate(new Vector3(0, 315, 0));
        flyingRafters[3].transform.position = new Vector3(0, flyingRafterY + body.secondFloorHeight, (flyingRafterZ + roof.deep / 2));
        flyingRafters[3].transform.Rotate(new Vector3(0, 45, 0));

        flyingRafters[4].transform.position = new Vector3(roof.length / 2, baoShaFlyingRafterY, flyingRafterZ - roof.width - roof.baoShaWidth / 2);
        flyingRafters[4].transform.Rotate(new Vector3(0, 135, 0));
        flyingRafters[5].transform.position = new Vector3(roof.length / 2, baoShaFlyingRafterY, -flyingRafterZ - roof.width - roof.baoShaWidth / 2);
        flyingRafters[5].transform.Rotate(new Vector3(0, 225f, 0));
        flyingRafters[6].transform.position = new Vector3(-roof.length / 2, baoShaFlyingRafterY, -flyingRafterZ - roof.width - roof.baoShaWidth / 2);
        flyingRafters[6].transform.Rotate(new Vector3(0, 315, 0));
        flyingRafters[7].transform.position = new Vector3(-roof.length / 2, baoShaFlyingRafterY, flyingRafterZ - roof.width - roof.baoShaWidth / 2);
        flyingRafters[7].transform.Rotate(new Vector3(0, 45, 0));

        flyingRafters[8].transform.position = new Vector3(-flyingRafterZ + roof.width + roof.baoShaWidth / 2, baoShaFlyingRafterY, -roof.length / 2);
        flyingRafters[8].transform.Rotate(new Vector3(0, 225, 0));
        flyingRafters[9].transform.position = new Vector3(flyingRafterZ + roof.width + roof.baoShaWidth / 2, baoShaFlyingRafterY, -roof.length / 2);
        flyingRafters[9].transform.Rotate(new Vector3(0, 315, 0));
        flyingRafters[10].transform.position = new Vector3(flyingRafterZ + roof.width + roof.baoShaWidth / 2, baoShaFlyingRafterY, roof.length / 2);
        flyingRafters[10].transform.Rotate(new Vector3(0, 45, 0));
        flyingRafters[11].transform.position = new Vector3(-flyingRafterZ + roof.width + roof.baoShaWidth / 2, baoShaFlyingRafterY, roof.length / 2);
        flyingRafters[11].transform.Rotate(new Vector3(0, 135, 0));

        flyingRafters[12].transform.position = new Vector3(-roof.length / 2, baoShaFlyingRafterY, -flyingRafterZ + roof.width + roof.baoShaWidth / 2);
        flyingRafters[12].transform.Rotate(new Vector3(0, 315, 0));
        flyingRafters[13].transform.position = new Vector3(-roof.length / 2, baoShaFlyingRafterY, flyingRafterZ + roof.width + roof.baoShaWidth / 2);
        flyingRafters[13].transform.Rotate(new Vector3(0, 45, 0));
        flyingRafters[14].transform.position = new Vector3(roof.length / 2, baoShaFlyingRafterY, flyingRafterZ + roof.width + roof.baoShaWidth / 2);
        flyingRafters[14].transform.Rotate(new Vector3(0, 135, 0));
        flyingRafters[15].transform.position = new Vector3(roof.length / 2, baoShaFlyingRafterY, -flyingRafterZ + roof.width + roof.baoShaWidth / 2);
        flyingRafters[15].transform.Rotate(new Vector3(0, 225, 0));

        flyingRafters[16].transform.position = new Vector3(flyingRafterZ - roof.width - roof.baoShaWidth / 2, baoShaFlyingRafterY, roof.length / 2);
        flyingRafters[16].transform.Rotate(new Vector3(0, 45, 0));
        flyingRafters[17].transform.position = new Vector3(-flyingRafterZ - roof.width - roof.baoShaWidth / 2, baoShaFlyingRafterY, roof.length / 2);
        flyingRafters[17].transform.Rotate(new Vector3(0, 135, 0));
        flyingRafters[18].transform.position = new Vector3(-flyingRafterZ - roof.width - roof.baoShaWidth / 2, baoShaFlyingRafterY, -roof.length / 2);
        flyingRafters[18].transform.Rotate(new Vector3(0, 225, 0));
        flyingRafters[19].transform.position = new Vector3(flyingRafterZ - roof.width - roof.baoShaWidth / 2, baoShaFlyingRafterY, -roof.length / 2);
        flyingRafters[19].transform.Rotate(new Vector3(0, 315, 0));

        foreach (GameObject obj in flyingRafters)
        {
            obj.name = "FlyingRafter";
            obj.transform.parent = raftersObj.transform;
        }
        raftersObj.transform.parent = roofObject.transform;

        //flyingRafterPoints 畫mesh用
        flyingRafterPoints = offsetPoint;
        flyingBargePointNum = bargeboardPointUpper;
        baoShaFlyingRafterPoints = baoShaOffsetPoint;
        baoShaFlyingBargePointNum = baoShaBargeboardPointUpper;
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
        Vector3 offset = new Vector3(roof.width / roof.disBetween, flyingRafterY + roof.height / 2.85f, flyingRafterZ);
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
            sideRoofs[0].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[0].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[1].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[1].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[2].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[2].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[3].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[3].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[4].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[4].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[5].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[5].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[6].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[6].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[7].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[7].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[8].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[8].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[9].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[9].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[10].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[10].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[11].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[11].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[12].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[12].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[13].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[13].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[14].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[14].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[15].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[15].transform.Rotate(new Vector3(0, 135, 0));

            float tessellateOffeset = 0.0575f;
            Cut(sideRoofs[1], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[2], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[9], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[10], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");

            sideRoofs[7].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[4].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[12].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[15].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[7] = Instantiate(sideRoofs[1]);
            sideRoofs[4] = Instantiate(sideRoofs[2]);
            sideRoofs[12] = Instantiate(sideRoofs[9]);
            sideRoofs[15] = Instantiate(sideRoofs[10]);
            sideRoofs[7].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[4].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[12].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[15].transform.localScale = new Vector3(-1, 1, -1);
            foreach (GameObject obj in sideRoofs)
            {
                obj.name = "sideroof";
                obj.transform.parent = roofObject.transform;
            }
            sideRoofs[7].transform.localPosition = new Vector3(-sideRoofs[1].transform.localPosition.x, sideRoofs[1].transform.localPosition.y, sideRoofs[1].transform.localPosition.z);
            sideRoofs[4].transform.localPosition = new Vector3(-sideRoofs[2].transform.localPosition.x, sideRoofs[2].transform.localPosition.y, sideRoofs[2].transform.localPosition.z);
            sideRoofs[12].transform.localPosition = new Vector3(-sideRoofs[9].transform.localPosition.x, sideRoofs[9].transform.localPosition.y, sideRoofs[9].transform.localPosition.z);
            sideRoofs[15].transform.localPosition = new Vector3(-sideRoofs[10].transform.localPosition.x, sideRoofs[10].transform.localPosition.y, sideRoofs[10].transform.localPosition.z);

            sideRoofs[8].name = sideRoofs[9].name = sideRoofs[10].name = sideRoofs[11].name = "sideRoofInside";
            sideRoofs[12].name = sideRoofs[13].name = sideRoofs[14].name = sideRoofs[15].name = "sideRoofInside";
        }
        #endregion
    }

    public void DrawDoubleHexaRoofMesh()
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
        Quaternion rotateEuler = Quaternion.Euler(0, 120, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.width / roof.disBetween, flyingRafterY + roof.height / 2.85f, flyingRafterZ);
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
            sideRoofs[0].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[0].transform.Rotate(new Vector3(0, 30, 0));
            sideRoofs[1].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[1].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[2].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[2].transform.Rotate(new Vector3(0, 150, 0));
            sideRoofs[3].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[3].transform.Rotate(new Vector3(0, -30, 0));
            sideRoofs[4].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[4].transform.Rotate(new Vector3(0, -90, 0));
            sideRoofs[5].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[5].transform.Rotate(new Vector3(0, -150, 0));
            sideRoofs[6].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[6].transform.Rotate(new Vector3(0, 30, 0));
            sideRoofs[7].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[7].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[8].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[8].transform.Rotate(new Vector3(0, 150, 0));
            sideRoofs[9].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[9].transform.Rotate(new Vector3(0, -30, 0));
            sideRoofs[10].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[10].transform.Rotate(new Vector3(0, -90, 0));
            sideRoofs[11].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[11].transform.Rotate(new Vector3(0, -150, 0));
            sideRoofs[12].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[12].transform.Rotate(new Vector3(0, 30, 0));
            sideRoofs[13].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[13].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[14].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[14].transform.Rotate(new Vector3(0, 150, 0));
            sideRoofs[15].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[15].transform.Rotate(new Vector3(0, -30, 0));
            sideRoofs[16].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[16].transform.Rotate(new Vector3(0, -90, 0));
            sideRoofs[17].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[17].transform.Rotate(new Vector3(0, -150, 0));
            sideRoofs[18].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[18].transform.Rotate(new Vector3(0, 30, 0));
            sideRoofs[19].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[19].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[20].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[20].transform.Rotate(new Vector3(0, 150, 0));
            sideRoofs[21].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[21].transform.Rotate(new Vector3(0, -30, 0));
            sideRoofs[22].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[22].transform.Rotate(new Vector3(0, -90, 0));
            sideRoofs[23].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[23].transform.Rotate(new Vector3(0, -150, 0));

            float tessellateOffeset = 0.0575f;
            Cut(sideRoofs[3], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[4], new Vector3(-tessellateOffeset * 1.6f,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[5], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[15], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[16], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[17], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");


            sideRoofs[6].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[7].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[8].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[18].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[19].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[20].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[6] = Instantiate(sideRoofs[3]);
            sideRoofs[7] = Instantiate(sideRoofs[4]);
            sideRoofs[8] = Instantiate(sideRoofs[5]);
            sideRoofs[18] = Instantiate(sideRoofs[15]);
            sideRoofs[19] = Instantiate(sideRoofs[16]);
            sideRoofs[20] = Instantiate(sideRoofs[17]);
            sideRoofs[6].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[7].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[8].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[18].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[19].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[20].transform.localScale = new Vector3(-1, 1, -1);
            foreach (GameObject obj in sideRoofs)
            {
                obj.name = "sideroof";
                obj.transform.parent = roofObject.transform;
            }
            sideRoofs[6].transform.localPosition = new Vector3(-sideRoofs[3].transform.localPosition.x, sideRoofs[3].transform.localPosition.y, sideRoofs[3].transform.localPosition.z);
            sideRoofs[7].transform.localPosition = new Vector3(-sideRoofs[4].transform.localPosition.x, sideRoofs[4].transform.localPosition.y, sideRoofs[4].transform.localPosition.z);
            sideRoofs[8].transform.localPosition = new Vector3(-sideRoofs[5].transform.localPosition.x, sideRoofs[5].transform.localPosition.y, sideRoofs[5].transform.localPosition.z);
            sideRoofs[18].transform.localPosition = new Vector3(-sideRoofs[15].transform.localPosition.x, sideRoofs[15].transform.localPosition.y, sideRoofs[15].transform.localPosition.z);
            sideRoofs[19].transform.localPosition = new Vector3(-sideRoofs[16].transform.localPosition.x, sideRoofs[16].transform.localPosition.y, sideRoofs[16].transform.localPosition.z);
            sideRoofs[20].transform.localPosition = new Vector3(-sideRoofs[17].transform.localPosition.x, sideRoofs[17].transform.localPosition.y, sideRoofs[17].transform.localPosition.z);
        }
        #endregion
    }
    public void DrawDoubleOctoRoofMesh()
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
        Quaternion rotateEuler = Quaternion.Euler(0, 112.5f, 0);
        float flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(roof.width / roof.disBetween, flyingRafterY + roof.height / 2.85f, flyingRafterZ);
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
                Instantiate(sideRoof),
                sideRoof,
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoofInside),
                sideRoofInside,
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
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
            sideRoofs[0].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[0].transform.Rotate(new Vector3(0, 0, 0));
            sideRoofs[1].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[1].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[2].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[2].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[3].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[3].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[4].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[4].transform.Rotate(new Vector3(0, 180, 0));
            sideRoofs[5].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[5].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[6].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[6].transform.Rotate(new Vector3(0, 270, 0));
            sideRoofs[7].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[7].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[8].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[8].transform.Rotate(new Vector3(0, 0, 0));
            sideRoofs[9].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[9].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[10].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[10].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[11].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[11].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[12].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[12].transform.Rotate(new Vector3(0, 180, 0));
            sideRoofs[13].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[13].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[14].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[14].transform.Rotate(new Vector3(0, 270, 0));
            sideRoofs[15].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), 0, 0);
            sideRoofs[15].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[16].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[16].transform.Rotate(new Vector3(0, 0, 0));
            sideRoofs[17].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[17].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[18].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[18].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[19].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[19].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[20].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[20].transform.Rotate(new Vector3(0, 180, 0));
            sideRoofs[21].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[21].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[22].transform.position = new Vector3(roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[22].transform.Rotate(new Vector3(0, 270, 0));
            sideRoofs[23].transform.position = new Vector3(roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[23].transform.Rotate(new Vector3(0, 315, 0));
            sideRoofs[24].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[24].transform.Rotate(new Vector3(0, 0, 0));
            sideRoofs[25].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[25].transform.Rotate(new Vector3(0, 45, 0));
            sideRoofs[26].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[26].transform.Rotate(new Vector3(0, 90, 0));
            sideRoofs[27].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[27].transform.Rotate(new Vector3(0, 135, 0));
            sideRoofs[28].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[28].transform.Rotate(new Vector3(0, 180, 0));
            sideRoofs[29].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[29].transform.Rotate(new Vector3(0, 225, 0));
            sideRoofs[30].transform.position = new Vector3(-roof.width / roof.disBetween - (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[30].transform.Rotate(new Vector3(0, 270, 0));
            sideRoofs[31].transform.position = new Vector3(-roof.width / roof.disBetween + (roof.length / 2 - newFlyingRafterPoints[0].z), -0.25f, 0);
            sideRoofs[31].transform.Rotate(new Vector3(0, 315, 0));

            float tessellateOffeset = 0.0575f;
            Cut(sideRoofs[0], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[4], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[5], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[6], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[7], new Vector3(-tessellateOffeset,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[16], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[20], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[21], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[22], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");
            Cut(sideRoofs[23], new Vector3(0,0,0), new Vector3(-1,0,0), "sideroof");


            sideRoofs[8].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[9].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[10].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[11].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[12].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[24].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[25].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[26].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[27].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[28].GetComponent<MeshFilter>().mesh.Clear();
            sideRoofs[8] = Instantiate(sideRoofs[0]);
            sideRoofs[9] = Instantiate(sideRoofs[4]);
            sideRoofs[10] = Instantiate(sideRoofs[5]);
            sideRoofs[11] = Instantiate(sideRoofs[6]);
            sideRoofs[12] = Instantiate(sideRoofs[7]);
            sideRoofs[24] = Instantiate(sideRoofs[16]);
            sideRoofs[25] = Instantiate(sideRoofs[20]);
            sideRoofs[26] = Instantiate(sideRoofs[21]);
            sideRoofs[27] = Instantiate(sideRoofs[22]);
            sideRoofs[28] = Instantiate(sideRoofs[23]);
            sideRoofs[8].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[9].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[10].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[11].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[12].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[24].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[25].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[26].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[27].transform.localScale = new Vector3(-1, 1, -1);
            sideRoofs[28].transform.localScale = new Vector3(-1, 1, -1);
            foreach (GameObject obj in sideRoofs)
            {
                obj.name = "sideroof";
                obj.transform.parent = roofObject.transform;
            }
            sideRoofs[8].transform.localPosition = new Vector3(-sideRoofs[0].transform.localPosition.x, sideRoofs[0].transform.localPosition.y, sideRoofs[0].transform.localPosition.z);
            sideRoofs[9].transform.localPosition = new Vector3(-sideRoofs[4].transform.localPosition.x, sideRoofs[4].transform.localPosition.y, sideRoofs[4].transform.localPosition.z);
            sideRoofs[10].transform.localPosition = new Vector3(-sideRoofs[5].transform.localPosition.x, sideRoofs[5].transform.localPosition.y, sideRoofs[5].transform.localPosition.z);
            sideRoofs[11].transform.localPosition = new Vector3(-sideRoofs[6].transform.localPosition.x, sideRoofs[6].transform.localPosition.y, sideRoofs[6].transform.localPosition.z);
            sideRoofs[12].transform.localPosition = new Vector3(-sideRoofs[7].transform.localPosition.x, sideRoofs[7].transform.localPosition.y, sideRoofs[7].transform.localPosition.z);
            sideRoofs[24].transform.localPosition = new Vector3(-sideRoofs[16].transform.localPosition.x, sideRoofs[16].transform.localPosition.y, sideRoofs[16].transform.localPosition.z);
            sideRoofs[25].transform.localPosition = new Vector3(-sideRoofs[20].transform.localPosition.x, sideRoofs[20].transform.localPosition.y, sideRoofs[20].transform.localPosition.z);
            sideRoofs[26].transform.localPosition = new Vector3(-sideRoofs[21].transform.localPosition.x, sideRoofs[21].transform.localPosition.y, sideRoofs[21].transform.localPosition.z);
            sideRoofs[27].transform.localPosition = new Vector3(-sideRoofs[22].transform.localPosition.x, sideRoofs[22].transform.localPosition.y, sideRoofs[22].transform.localPosition.z);
            sideRoofs[28].transform.localPosition = new Vector3(-sideRoofs[23].transform.localPosition.x, sideRoofs[23].transform.localPosition.y, sideRoofs[23].transform.localPosition.z);
        }
        #endregion
    }

    public void DrawCrossRoofMesh()
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
        GameObject besides = new GameObject("SideRoofs");
        GameObject baoShaObject = new GameObject("BaoSha");
        GameObject baoShaObjects = new GameObject("BaoShas");

        topRoof.transform.parent = baoShaObject.transform;
        topRoof.name = "TopRoofs";
        besides.transform.parent = baoShaObject.transform;
        baoShaObjects.transform.parent = roofObject.transform;

        #region 抱廈
        #region 正面的屋簷
        //找正面飛簷的點
        List<Vector3> newFlyingRafterPoints = new List<Vector3>();
        Quaternion rotateEuler = Quaternion.Euler(0, 135, 0);
        float flyingRafterY = -(roof.baoShaHeight - roof.topLowerHeight) + roof.sideEaveHeight;
        Vector3 offset = new Vector3(0, flyingRafterY, flyingRafterZ);
        for (int i = 0; i < baoShaFlyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * baoShaFlyingRafterPoints[i] + offset);
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
        GameObject frontRoof = creator.CreateRoofEaves(newFlyingRafterPoints, baoShaBargeboardPoints, topRoofPoints, roof.length, roof, false);
        GameObject frontRoofInside = creator.CreateRoofEaves(newFlyingRafterPoints, baoShaBargeboardPoints, topRoofPoints, roof.length, roof, true);
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
            for (int i = 0; i < baoShaFlyingRafterPoints.Count; i++)
            {
                sideFlyingRafterPoints.Add(rotateEuler * baoShaFlyingRafterPoints[i] + offset);
            }

            if (baoShaFlyingRafterPoints.Count > 0)
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

            for (int i = baoShaBargeboardPoints.Count - baoShaFlyingBargePointNum - 1; i < baoShaBargeboardPoints.Count; i++)
            {
                fakeBargeboardPoints.Add(baoShaBargeboardPoints[i]);
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
                obj.transform.parent = besides.transform;
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
        List<GameObject> baoShas = new List<GameObject>
        {
            baoShaObject,
            Instantiate(baoShaObject),
            Instantiate(baoShaObject),
            Instantiate(baoShaObject)
        };
        foreach (GameObject obj in baoShas)
        {
            obj.name = "baoSha";
            obj.transform.parent = baoShaObjects.transform;
        }
        float baoShaFlyingRafterY = -2.5f;
        baoShas[0].transform.position = new Vector3(0, baoShaFlyingRafterY, -roof.width - roof.baoShaWidth / 2);
        baoShas[0].transform.Rotate(new Vector3(0, 0, 0));
        baoShas[1].transform.position = new Vector3(roof.width + roof.baoShaWidth / 2, baoShaFlyingRafterY, 0);
        baoShas[1].transform.Rotate(new Vector3(0, 90, 0));
        baoShas[2].transform.position = new Vector3(0, baoShaFlyingRafterY, roof.width + roof.baoShaWidth / 2);
        baoShas[2].transform.Rotate(new Vector3(0, 180, 0));
        baoShas[3].transform.position = new Vector3(-roof.width - roof.baoShaWidth / 2, baoShaFlyingRafterY, 0);
        baoShas[3].transform.Rotate(new Vector3(0, 270, 0));
        #endregion

        #region 中央
        GameObject centerObjects = new GameObject("CenterRoofs");
        centerObjects.transform.parent = roofObject.transform;
        flyingRafterY = -(roof.height - roof.topLowerHeight) + roof.sideEaveHeight;
        offset = new Vector3(0, flyingRafterY, flyingRafterZ);
        for (int i = 0; i < flyingRafterPoints.Count; i++)
        {
            newFlyingRafterPoints.Add(rotateEuler * flyingRafterPoints[i] + offset);
        }
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

            List<GameObject> centerRoofs = new List<GameObject>
            {
                sideRoof,
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                Instantiate(sideRoof),
                sideRoofInside,
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside),
                Instantiate(sideRoofInside)
            };
            foreach (GameObject obj in centerRoofs)
            {
                obj.name = "sideroof";
                obj.transform.parent = centerObjects.transform;
            }
            
            centerRoofs[0].transform.position = new Vector3(0, 0, -newFlyingRafterPoints[0].z);
            centerRoofs[0].transform.Rotate(new Vector3(0, 0, 0));
            centerRoofs[1].transform.position = new Vector3(newFlyingRafterPoints[0].z, 0, 0);
            centerRoofs[1].transform.Rotate(new Vector3(0, 90, 0));
            centerRoofs[2].transform.position = new Vector3(0, 0, newFlyingRafterPoints[0].z);
            centerRoofs[2].transform.Rotate(new Vector3(0, 180, 0));
            centerRoofs[3].transform.position = new Vector3(-newFlyingRafterPoints[0].z, 0, 0);
            centerRoofs[3].transform.Rotate(new Vector3(0, 270, 0));
            centerRoofs[4].transform.position = new Vector3(0, -0.25f, -newFlyingRafterPoints[0].z);
            centerRoofs[4].transform.Rotate(new Vector3(0, 0, 0));
            centerRoofs[5].transform.position = new Vector3(newFlyingRafterPoints[0].z, -0.25f, 0);
            centerRoofs[5].transform.Rotate(new Vector3(0, 90, 0));
            centerRoofs[6].transform.position = new Vector3(0, -0.25f, newFlyingRafterPoints[0].z);
            centerRoofs[6].transform.Rotate(new Vector3(0, 180, 0));
            centerRoofs[7].transform.position = new Vector3(-newFlyingRafterPoints[0].z, -0.25f, 0);
            centerRoofs[7].transform.Rotate(new Vector3(0, 270, 0));
            centerRoofs[4].name = centerRoofs[5].name = centerRoofs[6].name = centerRoofs[7].name = "sideRoofInside";

            centerObjects.transform.position = new Vector3(0, body.secondFloorHeight, 0);
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
