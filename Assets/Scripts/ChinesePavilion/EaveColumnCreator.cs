using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaveColumnCreator : MonoBehaviour
{
    public GameObject meiziPrefab;
    public GameObject sparrowBracePrefab;
    public GameObject friezePrefab;
    public int nbSides;
    public Material architraveMaterial;
    public Material eaveColumnMaterial;
    public Material pedestalMaterial;
    [Range(0, 1)]
    public float FundationHeightRatio;
    [Range(1, 2)]
    public float FundationScaleRatio;

    private float radius;
    //private GameObject lowerPart;
    /// <summary>
    /// 創建而訪
    /// </summary>
    /// <param name="length">長</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    public GameObject CreateArchitrave(float length, float height)
    {
        GameObject architrave = GameObject.CreatePrimitive(PrimitiveType.Cube);
        architrave.transform.localScale = new Vector3(length, height, 0.25f);
        architrave.GetComponent<MeshRenderer>().material = architraveMaterial;
        architrave.name = "Architrave";
        return architrave;
    }
    /// <summary>
    /// 創建楣子
    /// </summary>
    /// <param name="length"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public GameObject CreateMeiZi(float length, float height)
    {
        GameObject meizi = Instantiate(meiziPrefab);
        meizi.transform.localScale = new Vector3(length, height, 0.25f);
        meizi.name = "MeiZi";
        return meizi;
    }
    public GameObject CreateSparrowBrace(float length, float height)
    {
        GameObject sparrowBrace = Instantiate(sparrowBracePrefab);
        // meizi.transform.localScale = new Vector3(length, height, 0.25f);
        sparrowBrace.name = "SparrowBrace";
        return sparrowBrace;
    }
    public GameObject CreateFriezeIn(float length, float height)
    {
        int friezeCount = (int)length / 2;
        // 不能為偶數 因為要砍掉中間的門
        if (friezeCount % 2 == 0)
            friezeCount++;
        float friezeScale = length / friezeCount;
        float friezeHeight = 1.5f;
        GameObject friezes = new GameObject();
        friezes.name = "Frieze";
        for (int i = 0; i < friezeCount; i++)
        {
            GameObject frieze = Instantiate(friezePrefab);
            frieze.transform.transform.position = new Vector3(i * friezeScale - (length - friezeScale) / 2, friezeHeight, 0);
            frieze.transform.localScale = new Vector3(friezeScale, friezeHeight, 1);
            frieze.name = "Frieze";
            frieze.transform.parent = friezes.transform;
        }

        // meizi.transform.localScale = new Vector3(length, height, 0.25f);
        return friezes;
    }


    /// <summary>
    /// 初始化
    /// </summary>
    public GameObject CreateEaveColumn(float height, float radius)
    {
        this.radius = radius;
        //柱子
        GameObject cylinder = new GameObject
        {
            name = "EaveColumn"
        };
        float upperPartHeight = 0.75f;
        //上半部
        SetUpCylinder("UpperPart", cylinder,
            new Vector3(0, height - upperPartHeight / 2, 0),
            new Vector3(radius * 1.25f, upperPartHeight, radius * 1.25f), architraveMaterial);

        //中部
        SetUpCylinder("LowerPart", cylinder, new Vector3(0, height / 2, 0), new Vector3(radius, height, radius), eaveColumnMaterial);

        //下半部
        SetUpCylinder("Pedestal", cylinder,
            new Vector3(0, (height * FundationHeightRatio) / 2, 0),
            new Vector3(radius * FundationScaleRatio, height * FundationHeightRatio, radius * FundationScaleRatio), pedestalMaterial);

        return cylinder;
    }

    /// <summary>
    /// 設定圓柱體
    /// </summary>
    /// <param name="name">名稱</param>
    /// <param name="parent">母物件</param>
    /// <param name="position">位置</param>
    /// <param name="scale">大小</param>
    /// <returns></returns>
    public GameObject SetUpCylinder(string name, GameObject parent, Vector3 position, Vector3 scale, Material partMaterial)
    {
        // GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        GameObject cylinder = new GameObject
        {
            name = name
        };
        MeshFilter filter = cylinder.AddComponent<MeshFilter>();
        cylinder.AddComponent<MeshRenderer>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        MeshCylinder meshCylinder = new MeshCylinder();
        //創建mesh
        mesh = meshCylinder.CreateCylinderMesh(mesh, nbSides, 1, radius, radius);
        cylinder.GetComponent<MeshFilter>().mesh = mesh;
        //放入材質
        cylinder.GetComponent<MeshRenderer>().material = partMaterial;
        //材質顏色改為設定的
        // cylinder.GetComponent<MeshRenderer>().material.color = materialColor;
        //放入母物件底下
        cylinder.transform.parent = parent.transform;
        //調整位置
        cylinder.transform.localPosition = position;
        //調整大小
        cylinder.transform.localScale = scale;

        return cylinder;
    }
}
