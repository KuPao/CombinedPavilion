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
}
