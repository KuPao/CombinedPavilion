using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCreator : MonoBehaviour
{
    public Material platFormMaterial;
    public Color platFormMaterialColor;
    public GameObject fencePerfabs;
    public GameObject Create(int nbSides, float bottomRadius, float topRadius, float height)
    {
        //創建柱子的位置

        GameObject cylinder = new GameObject();
        cylinder.name = "PlatForm";

        MeshFilter filter = cylinder.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        MeshRenderer meshRenderer = cylinder.AddComponent<MeshRenderer>();
        meshRenderer.material = platFormMaterial;
        meshRenderer.material.color = platFormMaterialColor;

        MeshCylinder meshCylinder = new MeshCylinder();
        mesh = meshCylinder.CreateCylinderMesh(mesh, nbSides, height, bottomRadius, topRadius);
        cylinder.GetComponent<MeshFilter>().mesh = mesh;

        return cylinder;
    }
    public GameObject CreateEdges(int nbSides, float height, float scaleX, float scaleZ)
    {
        GameObject edgesObj = new GameObject();
        edgesObj.name = "Edges";
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);

        List<GameObject> edges = new List<GameObject>()
        {
            edge,
            Instantiate(edge),
            Instantiate(edge),
            Instantiate(edge)
        };
        edges[0].transform.localScale = edges[1].transform.localScale = new Vector3(scaleX + 0.499f, 0.5f, 0.5f);
        edges[2].transform.localScale = edges[3].transform.localScale = new Vector3(scaleZ + 0.499f, 0.5f, 0.5f);
        edges[0].transform.position = new Vector3(0, 0, scaleZ / 2);
        edges[1].transform.position = new Vector3(0, 0, -scaleZ / 2);
        edges[2].transform.position = new Vector3(scaleX / 2, 0, 0);
        edges[3].transform.position = new Vector3(-scaleX / 2, 0, 0);
        edges[2].transform.Rotate(0, 90, 0);
        edges[3].transform.Rotate(0, 90, 0);

        foreach (GameObject obj in edges)
        {
            obj.transform.parent = edgesObj.transform;
            obj.GetComponent<MeshRenderer>().material = platFormMaterial;
            obj.GetComponent<MeshRenderer>().material.color = platFormMaterialColor;
            obj.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(scaleX, 0.5f);
        }
        edgesObj.transform.position = new Vector3(0, height, 0);
        return edgesObj;
    }

    public GameObject CreateFangShengEdges(float height, float scaleX, float disScale)
    {
        GameObject edgesObj = new GameObject();
        edgesObj.name = "Edges";
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);

        List<GameObject> edges = new List<GameObject>()
        {
            edge,
            Instantiate(edge),
            Instantiate(edge),
            Instantiate(edge)
        };
        edges[1].transform.localScale = edges[2].transform.localScale = new Vector3(scaleX + 0.499f, 0.5f, 0.5f);
        edges[0].transform.localScale = edges[3].transform.localScale = new Vector3(disScale * Mathf.Sqrt(2) + 0.499f, 0.5f, 0.5f);
        edges[0].transform.position = new Vector3(scaleX / 2 - disScale * Mathf.Sqrt(2) / 2, 0, scaleX / 2);
        edges[1].transform.position = new Vector3(0, 0, -scaleX / 2);
        edges[2].transform.position = new Vector3(scaleX / 2, 0, 0);
        edges[3].transform.position = new Vector3(-scaleX / 2, 0, -scaleX / 2 + disScale * Mathf.Sqrt(2) / 2);
        edges[2].transform.Rotate(0, 90, 0);
        edges[3].transform.Rotate(0, 90, 0);

        foreach (GameObject obj in edges)
        {
            obj.transform.parent = edgesObj.transform;
            obj.GetComponent<MeshRenderer>().material = platFormMaterial;
            obj.GetComponent<MeshRenderer>().material.color = platFormMaterialColor;
            obj.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(scaleX, 0.5f);
        }
        edgesObj.transform.position = new Vector3(0, height, 0);
        return edgesObj;
    }
    public GameObject CreateDoubleHexaEdges(float height, float scaleX, float disScale)
    {
        GameObject edgesObj = new GameObject();
        edgesObj.name = "Edges";
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);

        List<GameObject> edges = new List<GameObject>()
        {
            edge,
            Instantiate(edge),
            Instantiate(edge),
            Instantiate(edge),
            Instantiate(edge)
        };
        edges[1].transform.localScale = 
        edges[2].transform.localScale = 
        edges[3].transform.localScale = new Vector3(scaleX + 0.285f, 0.5f, 0.5f);

        edges[0].transform.localScale = 
        edges[4].transform.localScale = new Vector3(disScale * 2 / Mathf.Sqrt(3) + 0.285f, 0.5f, 0.5f);

        edges[0].transform.position = new Vector3(-disScale / 2, 0, scaleX - disScale / 2 / Mathf.Sqrt(3));
        edges[1].transform.position = new Vector3(scaleX * Mathf.Sqrt(3) / 4, 0, scaleX * 3 / 4);
        edges[2].transform.position = new Vector3(scaleX * Mathf.Sqrt(3) / 2, 0, 0);
        edges[3].transform.position = new Vector3(scaleX * Mathf.Sqrt(3) / 4, 0, -scaleX * 3 / 4);
        edges[4].transform.position = new Vector3(-disScale / 2, 0, -scaleX + disScale / 2 / Mathf.Sqrt(3));
        edges[0].transform.Rotate(0, -30, 0);
        edges[1].transform.Rotate(0, 30, 0);
        edges[2].transform.Rotate(0, 90, 0);
        edges[3].transform.Rotate(0, 150, 0);
        edges[4].transform.Rotate(0, 210, 0);

        foreach (GameObject obj in edges)
        {
            obj.transform.parent = edgesObj.transform;
            obj.GetComponent<MeshRenderer>().material = platFormMaterial;
            obj.GetComponent<MeshRenderer>().material.color = platFormMaterialColor;
            obj.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(scaleX, 0.5f);
        }
        edgesObj.transform.position = new Vector3(0, height, 0);
        return edgesObj;
    }

    public GameObject CreateDoubleOctoEdges(float height, float radius, float disScale, bool sevenSides)
    {
        GameObject edgesObj = new GameObject();
        edgesObj.name = "Edges";
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // 22.5
        float baseAngle = Mathf.PI / 8;
        // 45
        float angle = Mathf.PI * 2 / 8;
        float length = radius * Mathf.Sin(baseAngle) * 2;
        float posRadius = radius * Mathf.Cos(Mathf.PI / 8);

        // 一定會有的邊
        List<GameObject> edges = new List<GameObject>()
        {
            edge,
            Instantiate(edge),
            Instantiate(edge),
        };
        // 會隨距離變動的邊
        if(sevenSides)
            for(int i = 0; i < 4; i++)
                edges.Add(Instantiate(edge));
        else
            for(int i = 0; i < 2; i++)
                edges.Add(Instantiate(edge));
        // 先處理一定會有的邊
        edges[0].transform.localScale = 
        edges[1].transform.localScale = 
        edges[2].transform.localScale = new Vector3(length + 0.285f, 0.5f, 0.5f);

        for(int i = 0; i < 3; i++)
        {
            edges[i].transform.position = new Vector3(posRadius * Mathf.Cos(-baseAngle + (-i + 2) * angle), 0, posRadius * Mathf.Sin(-baseAngle + (-i + 2) * angle));
            edges[i].transform.Rotate(0, 22.5f + i * 45, 0);
        }
        
        if(sevenSides)
        {
            // 先處理固定邊長
            edges[3].transform.localScale = 
            edges[4].transform.localScale = new Vector3(length + 0.285f, 0.5f, 0.5f);
            edges[3].transform.position = new Vector3(posRadius * Mathf.Cos(-baseAngle + 3 * angle), 0, posRadius * Mathf.Sin(-baseAngle + 3 * angle));
            edges[4].transform.position = new Vector3(posRadius * Mathf.Cos(-baseAngle - 1 * angle), 0, posRadius * Mathf.Sin(-baseAngle - 1 * angle));
            edges[3].transform.Rotate(0, 22.5f + 3 * 45, 0);
            edges[4].transform.Rotate(0, 22.5f - 1 * 45, 0);

            // 再做接觸邊
            float intersectL = (disScale - length / 2) / Mathf.Sin(angle);
            edges[5].transform.localScale = 
            edges[6].transform.localScale = new Vector3(intersectL + 0.285f, 0.5f, 0.5f);
            edges[5].transform.position = new Vector3(-radius * Mathf.Cos(angle) - intersectL * Mathf.Sin(baseAngle) / 2, 0, radius * Mathf.Sin(angle) - intersectL * Mathf.Cos(baseAngle) / 2);
            edges[6].transform.position = new Vector3(-intersectL * Mathf.Cos(baseAngle) / 2, 0, -radius+ intersectL * Mathf.Sin(baseAngle) / 2);
            edges[5].transform.Rotate(0, 112.5f, 0);
            edges[6].transform.Rotate(0, 22.5f, 0);
        }
        else
        {
            float intersectL = disScale + length / 2;
            edges[3].transform.localScale = 
            edges[4].transform.localScale = new Vector3(length / 2 + disScale + 0.285f, 0.5f, 0.5f);
            edges[3].transform.position = new Vector3(-intersectL * Mathf.Cos(baseAngle) / 2, 0, radius - intersectL * Mathf.Sin(baseAngle) / 2);
            edges[4].transform.position = new Vector3(radius - length * Mathf.Sin(baseAngle) - intersectL* Mathf.Cos(baseAngle) / 2, 0, -length * Mathf.Cos(baseAngle) - intersectL * Mathf.Sin(baseAngle) / 2);
            edges[3].transform.Rotate(0, 22.5f + 3 * 45, 0);
            edges[4].transform.Rotate(0, 22.5f - 1 * 45, 0);
        }

        foreach (GameObject obj in edges)
        {
            obj.transform.parent = edgesObj.transform;
            obj.GetComponent<MeshRenderer>().material = platFormMaterial;
            obj.GetComponent<MeshRenderer>().material.color = platFormMaterialColor;
            obj.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(radius, 0.5f);
        }
        edgesObj.transform.position = new Vector3(0, height, 0);
        return edgesObj;
    }

    public GameObject CreateFrence(float length, float scale, float spacingDistance, float height, int fenceCount)
    {
        GameObject fences = new GameObject();
        fences.name = "Fence";
        for (int i = 0; i < fenceCount; i++)
        {
            GameObject fence = Instantiate(fencePerfabs);
            fence.transform.transform.position = new Vector3(i * spacingDistance - (fenceCount - 1) * spacingDistance / 2, height, 0);
            fence.transform.localScale = new Vector3(scale, 6f, scale);
            // float x = fence.GetComponentInChildren<MeshFilter>().mesh.bounds.size.x;
            fence.name = "Fence";
            fence.transform.parent = fences.transform;
        }

        return fences;
    }

    public GameObject CreatePrism(float radius, float height, int sides)
    {
        //創建柱子的位置

        GameObject prism = new GameObject();
        prism.name = "PlatForm";

        MeshFilter filter = prism.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        MeshRenderer meshRenderer = prism.AddComponent<MeshRenderer>();
        meshRenderer.material = platFormMaterial;
        meshRenderer.material.color = platFormMaterialColor;

        MeshPlatform meshPlatform = new MeshPlatform();
        mesh = meshPlatform.CreateMeshPrism(mesh, radius, height, sides);
        prism.GetComponent<MeshFilter>().mesh = mesh;

        return prism;
    }
}
