using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunViewOverlay : MonoBehaviourPun
{
    Camera overlayCam;
    private void Awake()
    {
        if (!photonView.IsMine)
            return;
        overlayCam = GameObject.FindGameObjectWithTag("GunCamera").GetComponent<Camera>();
        overlayCam.gameObject.transform.SetParent(transform); 
        overlayCam.transform.position = transform.position;
        overlayCam.transform.rotation = transform.rotation;
    }
}
