using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChBuildManager : MonoBehaviour
{

    public UIManager uIManager;
    public GameObject currentBuilding;
    private Roof roof;
    private Body body;
    private Platform platform;
    // Use this for initialization
    void Start()
    {

        roof = new Roof();
        body = new Body();
        platform = new Platform();
        roof = uIManager.SetAllVlaueFromRoofSlider(roof);
        body = uIManager.SetAllVlaueFromBodySlider(body);
        platform = uIManager.SetAllVlaueFromPlatformSlider(platform);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Roof GetCurrentRoof()
    {
        return roof;
    }
    public Body GetCurrentBody()
    {
        return body;
    }
    public Platform GetCurrentPlatform()
    {
        return platform;
    }
    public void CreateBuilding()
    {
        ChinesePavilionCreator creator = FindObjectOfType<ChinesePavilionCreator>();
        creator.CreateSetUp();
        // assign為備用
        currentBuilding = creator.CreateRoof(roof);
        creator.CreateBody(body);
        //creator.CreatePlatform(platform);

    }
}
