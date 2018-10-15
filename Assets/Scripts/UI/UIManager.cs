using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public ChBuildManager chBuildManager;
    public SliderManager sliderManager;
    public GameObject[] buildingPartButton;
    public GameObject[] buildingPartComponent;

    private void Start()
    {

    }

    public void ReCreate()
    {
        chBuildManager.CreateBuilding();
    }

    public Roof SetAllVlaueFromRoofSlider(Roof roof)
    {
        return sliderManager.SetAllVlaueFromRoofSlider(roof);
    }

    public Body SetAllVlaueFromBodySlider(Body body)
    {
        return sliderManager.SetAllVlaueFromBodySlider(body);
    }
    public Platform SetAllVlaueFromPlatformSlider(Platform platform)
    {
        return sliderManager.SetAllVlaueFromPlatformSlider(platform);
    }
    public void ChangePartEdit(GameObject activeUI)
    {
        foreach (GameObject obj in buildingPartComponent)
        {
            obj.SetActive(false);
        }
        activeUI.SetActive(true);
    }
    public void ChangeColor(GameObject button)
    {
        foreach (GameObject obj in buildingPartButton)
        {
            obj.GetComponent<Image>().color = new Color(150f / 255f, 120f / 255f, 90f / 255f);
        }
        button.GetComponent<Image>().color = new Color(190f / 255f, 150f / 255f, 120f / 255f);
    }
}
