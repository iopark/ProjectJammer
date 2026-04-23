using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public GameObject baseCrosshair;
    public GameObject zoomCrosshair;

    private void Start()
    {
        baseCrosshair.SetActive(true);
        zoomCrosshair.SetActive(false);
    }

    public void ChangeCrosshair(bool isZoom)
    {
        if (isZoom)
        {
            baseCrosshair.SetActive(false);
            zoomCrosshair.SetActive(true);
        }
        else
        {
            baseCrosshair.SetActive(true);
            zoomCrosshair.SetActive(false);
        }
    }
}
