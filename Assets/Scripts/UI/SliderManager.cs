using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    //管理所有的slider
    [Header("Roof")]
    public Slider length;
    public Slider width;
    public Slider deep;
    public Slider height;
    public Slider curve;
    public Slider sideEaveStart;
    public Slider sideEaveHeight;
    public Slider sideEaveCurve;
    public Slider topLowerHeight;
    public Slider topLowerLength;
    public Slider topLowerCurve;
    public Slider WingAngleHeight;
    public Slider wingAngleLength;
    public Slider wingAngleCurve;
    public Slider disBetween;
    public Slider baoShaWidth;
    public Slider baoShaHeight;
    [Header("Column")]
    public Slider columnPos;
    public Slider columnRadius;
    public Slider columnHeight;
    public Slider SecondFloorHeight;
    [Header("PlatForm")]
    public Slider platformLength;
    public Slider platformWidth;
    public Slider platformHeight;

    private UIManager uIManager;
    private ChBuildManager chBuildManager;

    // Use this for initialization
    void Start()
    {
        uIManager = GetComponent<UIManager>();
        chBuildManager = GetComponent<ChBuildManager>();
    }

    public Roof SetAllVlaueFromRoofSlider(Roof roof)
    {
        roof.length = length.value;
        roof.width = width.value;
        roof.deep = deep.value;
        roof.height = height.value;
        roof.curve = curve.value;
        roof.sideEaveStart = sideEaveStart.value;
        roof.sideEaveHeight = sideEaveHeight.value;
        roof.sideEaveCurve = sideEaveCurve.value;
        roof.topLowerHeight = topLowerHeight.value;
        roof.topLowerLength = topLowerLength.value;
        roof.topLowerCurve = topLowerCurve.value;
        roof.wingAngleHeight = WingAngleHeight.value;
        roof.wingAngleLength = wingAngleLength.value;
        roof.wingAngleCurve = wingAngleCurve.value;
        roof.disBetween = disBetween.value;
        roof.baoShaWidth = baoShaWidth.value;
        roof.baoShaHeight = baoShaHeight.value;

        return roof;
    }

    public Body SetAllVlaueFromBodySlider(Body body)
    {
        //屋頂的長度為基準
        body.pos = columnPos.value;
        body.height = columnHeight.value;
        body.radius = columnRadius.value;
        body.secondFloorHeight = SecondFloorHeight.value;
        return body;
    }
    public Platform SetAllVlaueFromPlatformSlider(Platform platform)
    {
        //屋頂的長度為基準
        platform.length = platformLength.value;
        //屋頂的寬度為基準
        platform.width = platformWidth.value;
        platform.height = platformHeight.value;
        return platform;
    }
    public void LengthChange(float value)
    {
        //關係連動
        topLowerLength.maxValue = value / 2 - 0.1f;
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.length = value;

        // 變成攢尖頂
        if (value < 0.01f)
        {
            roof.topLowerHeight = topLowerHeight.value = 0;
            roof.topLowerLength = topLowerLength.value = 0;
            roof.sideEaveStart = sideEaveStart.value = 0;
            // roof.sideEaveHeight = sideEaveHeight.value = 0;
        }
        chBuildManager.CreateBuilding();
    }

    public void WidthChange(float value)
    {
        //關係連動
        SetRoofHeight();
        baoShaWidth.maxValue = value / 2 - 0.1f;
        baoShaWidth.minValue = baoShaWidth.maxValue / 2;
        length.maxValue = (value - baoShaWidth.value) * 2 - 0.1f;
        length.minValue = (value - baoShaWidth.value) / 2 - 0.1f;
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.width = value;
        chBuildManager.CreateBuilding();
    }

    public void DeepChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.deep = value;
        chBuildManager.CreateBuilding();
    }
    public void HeightChange(float value)
    {
        //關係連動
        SetRoofHeight();
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.height = value;
        chBuildManager.CreateBuilding();
    }

    public void CurveChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.curve = value;
        chBuildManager.CreateBuilding();
    }

    public void SideEaveStartChange(float value)
    {
        //關係連動
        SetRoofHeight();
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.sideEaveStart = value;
        chBuildManager.CreateBuilding();
    }

    public void SideEaveHeightChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.sideEaveHeight = value;
        chBuildManager.CreateBuilding();
    }

    public void SideEaveCurveChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.sideEaveCurve = value;
        chBuildManager.CreateBuilding();
    }


    public void TopLowerHeightChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.topLowerHeight = value;
        chBuildManager.CreateBuilding();
    }

    public void TopLowerLengthChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.topLowerLength = value;
        //避免交叉
        if (roof.length <= roof.topLowerLength * 2)
        {
            roof.length = roof.topLowerLength * 2 + 0.1f;
            length.value = roof.length;
        }
        chBuildManager.CreateBuilding();
    }

    public void TopLowerCurveChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.topLowerCurve = value;
        chBuildManager.CreateBuilding();
    }

    public void WingAngleHeightChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.wingAngleHeight = value;
        chBuildManager.CreateBuilding();
    }

    public void WingAngleCurveChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.wingAngleCurve = value;
        chBuildManager.CreateBuilding();
    }
    public void WingAngleLengthChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.wingAngleLength = value;
        chBuildManager.CreateBuilding();
    }
    public void DisBetweenChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.disBetween = value;
        chBuildManager.CreateBuilding();
    }
    public void BaoShaWidthChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        length.maxValue = (width.value - value) * 2 - 0.1f;
        length.minValue = (width.value - value) / 2 - 0.1f;
        roof.baoShaWidth = value;
        chBuildManager.CreateBuilding();
    }
    public void BaoShaHeightChange(float value)
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.baoShaHeight = value;
        chBuildManager.CreateBuilding();
    }
    public void ColumnPosChange(float value)
    {
        Body body = chBuildManager.GetCurrentBody();
        body.pos = value;
        chBuildManager.CreateBuilding();
    }
    public void ColumnRadiusChange(float value)
    {
        Body body = chBuildManager.GetCurrentBody();
        body.radius = value;
        chBuildManager.CreateBuilding();
    }
    public void BodyHeightChange(float value)
    {
        Body body = chBuildManager.GetCurrentBody();
        body.height = value;
        chBuildManager.CreateBuilding();
    }
    public void SecondFloorHeightChange(float value)
    {
        Body body = chBuildManager.GetCurrentBody();
        baoShaHeight.maxValue = value / 2 + 0.1f;
        baoShaHeight.minValue = value / 4 + 0.1f;
        body.secondFloorHeight = value;
        chBuildManager.CreateBuilding();
    }
    public void PlatformLengthChange(float value)
    {
        Platform platform = chBuildManager.GetCurrentPlatform();
        platform.length = value;
        chBuildManager.CreateBuilding();
    }

    public void PlatformWidthChange(float value)
    {
        Platform platform = chBuildManager.GetCurrentPlatform();
        platform.width = value;
        chBuildManager.CreateBuilding();
    }

    public void PlatformHeightChange(float value)
    {
        Platform platform = chBuildManager.GetCurrentPlatform();
        platform.height = value;
        chBuildManager.CreateBuilding();
    }

    private void SetRoofHeight()
    {
        //關係連動
        CircleCurve bargeBoardCurve = new CircleCurve();
        List<Vector3> circleCurve = bargeBoardCurve.CreateCircleCurve(height.value, width.value, curve.value, 10);
        float sideEaveLength = width.value - sideEaveStart.value;
        float bargeboardPercent = (sideEaveLength / width.value * (circleCurve.Count - 1f));
        if (Mathf.CeilToInt(bargeboardPercent) > 0)
        {
            float flyingRafterHeight = circleCurve[Mathf.CeilToInt(bargeboardPercent)].y;
            sideEaveHeight.maxValue = flyingRafterHeight * 0.5f;
        }
    }

    public void SetRoofSingle()
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.combineType = 0;
        disBetween.transform.parent.gameObject.SetActive(false);
        length.transform.parent.gameObject.SetActive(true);
        deep.transform.parent.gameObject.SetActive(true);
        sideEaveStart.transform.parent.gameObject.SetActive(true);
        wingAngleLength.transform.parent.gameObject.SetActive(true);
        WingAngleHeight.transform.parent.gameObject.SetActive(true);
        topLowerHeight.transform.parent.gameObject.SetActive(true);
        topLowerLength.transform.parent.gameObject.SetActive(true);
        topLowerCurve.transform.parent.gameObject.SetActive(true);
        baoShaWidth.transform.parent.gameObject.SetActive(false);
        baoShaHeight.transform.parent.gameObject.SetActive(false);
        chBuildManager.CreateBuilding();
    }

    public void SetRoofFangSheng()
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.combineType = 1;
        roof.length = 0;
        disBetween.transform.parent.gameObject.SetActive(true);
        length.transform.parent.gameObject.SetActive(false);
        deep.transform.parent.gameObject.SetActive(false);
        sideEaveStart.transform.parent.gameObject.SetActive(false);
        wingAngleLength.transform.parent.gameObject.SetActive(false);
        WingAngleHeight.transform.parent.gameObject.SetActive(false);
        topLowerHeight.transform.parent.gameObject.SetActive(false);
        topLowerLength.transform.parent.gameObject.SetActive(false);
        topLowerCurve.transform.parent.gameObject.SetActive(false);
        baoShaWidth.transform.parent.gameObject.SetActive(false);
        baoShaHeight.transform.parent.gameObject.SetActive(false);
        chBuildManager.CreateBuilding();
    }

    public void SetRoofDoubldeHexa()
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.combineType = 2;
        roof.length = 0;
        disBetween.transform.parent.gameObject.SetActive(true);
        length.transform.parent.gameObject.SetActive(false);
        deep.transform.parent.gameObject.SetActive(false);
        sideEaveStart.transform.parent.gameObject.SetActive(false);
        wingAngleLength.transform.parent.gameObject.SetActive(false);
        WingAngleHeight.transform.parent.gameObject.SetActive(false);
        topLowerHeight.transform.parent.gameObject.SetActive(false);
        topLowerLength.transform.parent.gameObject.SetActive(false);
        topLowerCurve.transform.parent.gameObject.SetActive(false);
        baoShaWidth.transform.parent.gameObject.SetActive(false);
        baoShaHeight.transform.parent.gameObject.SetActive(false);
        chBuildManager.CreateBuilding();
    }

    public void SetRoofDoubldeOcto()
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.combineType = 3;
        roof.length = 0;
        disBetween.transform.parent.gameObject.SetActive(true);
        length.transform.parent.gameObject.SetActive(false);
        deep.transform.parent.gameObject.SetActive(false);
        sideEaveStart.transform.parent.gameObject.SetActive(false);
        wingAngleLength.transform.parent.gameObject.SetActive(false);
        WingAngleHeight.transform.parent.gameObject.SetActive(false);
        topLowerHeight.transform.parent.gameObject.SetActive(false);
        topLowerLength.transform.parent.gameObject.SetActive(false);
        topLowerCurve.transform.parent.gameObject.SetActive(false);
        baoShaWidth.transform.parent.gameObject.SetActive(false);
        baoShaHeight.transform.parent.gameObject.SetActive(false);
        chBuildManager.CreateBuilding();
    }

    public void SetRoofCross()
    {
        Roof roof = chBuildManager.GetCurrentRoof();
        roof.combineType = 4;
        roof.length = length.value;
        disBetween.transform.parent.gameObject.SetActive(false);
        length.transform.parent.gameObject.SetActive(true);
        deep.transform.parent.gameObject.SetActive(false);
        sideEaveStart.transform.parent.gameObject.SetActive(false);
        wingAngleLength.transform.parent.gameObject.SetActive(false);
        WingAngleHeight.transform.parent.gameObject.SetActive(false);
        topLowerHeight.transform.parent.gameObject.SetActive(false);
        topLowerLength.transform.parent.gameObject.SetActive(false);
        topLowerCurve.transform.parent.gameObject.SetActive(false);
        baoShaWidth.transform.parent.gameObject.SetActive(true);
        baoShaHeight.transform.parent.gameObject.SetActive(true);
        chBuildManager.CreateBuilding();
    }
}
