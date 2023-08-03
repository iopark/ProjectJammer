using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunViewOverlay : MonoBehaviour
{
    Camera overlayCam; 
    private void Awake()
    {
        overlayCam = GameObject.Find("GunCamera").GetComponent<Camera>();
        overlayCam.gameObject.transform.SetParent(transform); 
        overlayCam.transform.position = transform.position;
        overlayCam.transform.rotation = transform.rotation;
    }
}
